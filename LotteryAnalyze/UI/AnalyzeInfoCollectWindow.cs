using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace LotteryAnalyze.UI
{
    public partial class AnalyzeInfoCollectWindow : Form, UpdaterBase
    {
        public enum DataAnalyseType
        {
            // 所有的遗漏统计数据
            eDAT_MissCountTotal,
            // 统计K线触碰到布林上轨后的遗漏值
            eDAT_MissCountOnTouchBolleanUp,
            // 开出长遗漏后继续开出长遗漏的信息
            eDAT_ContinueLongMissCount,
        }
        static String[] DataAnalyseTypeStrs =
        {
            "所有的遗漏数据",
            "K线触碰到布林上轨后的遗漏值",
            "连续开出长遗漏的统计信息",
        };
        static String[] ExportDataPaths =
        {
            "..\\tools\\遗漏统计结果.xml",
            "..\\tools\\从布林上轨连续遗漏的统计结果.xml",
            "..\\tools\\连续开出长遗漏的统计信息.xml",
        };
        public DataAnalyseType currentDataAnalyseType = DataAnalyseType.eDAT_MissCountTotal;

        List<int> dateIDLst = new List<int>();
        double updateInterval = 0.05;
        double updateCountDown = 0;
        int startIndex = -1;
        int endIndex = -1;
        int batchCount = 10;
        string lastDataItemTag = "";
        bool hasFinished = true;
        int allDataItemCount = 0;
        int CALC_PER_COUNT = 100;
        bool isStop = false;

        enum ProcStatus
        {
            eNotStart,
            eStart,
            ePrepBatch,
            eDoBatch,
            eCompleted,
        }
        ProcStatus status = ProcStatus.eNotStart;

        //List<Dictionary<CollectDataType, Dictionary<int, TreeNode>>> allCDTMissCountTreeNodeMap = new List<Dictionary<CollectDataType, Dictionary<int, TreeNode>>>();
        List<Dictionary<CollectDataType, Dictionary<int, int>>> allCDTMissCountNumMap = new List<Dictionary<CollectDataType, Dictionary<int, int>>>();
        DataItem cItem;

        Dictionary<CollectDataType, Dictionary<int, TreeNode>> cdtMissCountTreeNodeMap = null;
        Dictionary<CollectDataType, Dictionary<int, int>> cdtMissCountNumMap = null;
        Dictionary<int, TreeNode> missCountTreeNodeMap = null;
        Dictionary<int, int> missCountNumMap = null;
        TreeNode numNode = null;
        TreeNode cdtNode = null;
        TreeNode missCountNode = null;
        StatisticUnitMap sum = null;
        StatisticUnit su = null;

        List<Dictionary<CollectDataType, int>> numberPathMissCount = new List<Dictionary<CollectDataType, int>>();
        List<Dictionary<CollectDataType, Dictionary<int, List<string>>>> overMissCountAndTouchBolleanUpInfos = new List<Dictionary<CollectDataType, Dictionary<int, List<string>>>>();

        List<Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>>> continueMissCountInfos = new List<Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>>>();
        List<Dictionary<CollectDataType, int>> lastLongMissCountInfo = new List<Dictionary<CollectDataType, int>>();
        List<Dictionary<CollectDataType, int>> currentLongMissCountInfo = new List<Dictionary<CollectDataType, int>>();

        static AnalyzeInfoCollectWindow sInst;
        public static void Open()
        {
            if (sInst == null)
                sInst = new AnalyzeInfoCollectWindow();
            sInst.Show();
        }


        public AnalyzeInfoCollectWindow()
        {
            //采用双缓冲技术的控件必需的设置
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            InitializeComponent();

            comboBoxDataAnalyseType.DataSource = DataAnalyseTypeStrs;
            comboBoxDataAnalyseType.SelectedIndex = (int)currentDataAnalyseType;

            FormMain.AddWindow(this);
            Program.AddUpdater(this);
        }

        public void OnUpdate()
        {
            DoUpdate();
        }
        
        void DoPrepBatch()
        {
            DataManager dataMgr = DataManager.GetInst();
            endIndex = startIndex + batchCount;
            if (endIndex >= dateIDLst.Count)
                endIndex = dateIDLst.Count - 1;
            dataMgr.ClearAllDatas();
            for (int i = startIndex; i <= endIndex; ++i)
            {
                int key = dateIDLst[i];
                dataMgr.LoadData(key);
            }
            dataMgr.SetDataItemsGlobalID();
            if (dataMgr.GetAllDataItemCount() > 0)
            {
                Util.CollectPath012Info(null);
                if (currentDataAnalyseType == DataAnalyseType.eDAT_MissCountOnTouchBolleanUp ||
                    currentDataAnalyseType == DataAnalyseType.eDAT_ContinueLongMissCount)
                {
                    GraphDataManager.Instance.CollectGraphData(GraphType.eKCurveGraph);
                }

                cItem = dataMgr.GetFirstItem();
                if (string.IsNullOrEmpty(lastDataItemTag) == false)
                {
                    DataItem lastItem = dataMgr.GetDataItemByIdTag(lastDataItemTag);
                    if(lastItem != null)
                        cItem = lastItem.parent.GetNextItem(lastItem);
                }
                allDataItemCount = dataMgr.GetAllDataItemCount();
                status = ProcStatus.eDoBatch;
            }
            else
                status = ProcStatus.ePrepBatch;
            progressBarSub.Value = 0;
            progressBarMain.Value = (int)(100.0f * ((float)(endIndex + 1) / dateIDLst.Count));
            
            this.Invalidate(true);
        }

        void DoBatch()
        {
            switch(currentDataAnalyseType)
            {
                case DataAnalyseType.eDAT_MissCountTotal:
                    AnalyseForMissCountTotal();
                    break;
                case DataAnalyseType.eDAT_MissCountOnTouchBolleanUp:
                    AnalyseForMissCountOnTouchBolleanUp();
                    break;
                case DataAnalyseType.eDAT_ContinueLongMissCount:
                    AnalyseContinueLongMissCount();
                    break;
            }
        }

        void RecordContinueLongMissCount(int numIndex, CollectDataType cdt, int lastMissCount, int curMissCount, string idTag)
        {
            Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>> numInfo = continueMissCountInfos[numIndex];
            Dictionary<int, Dictionary<int, List<string>>> cdtInfo = null;
            if (numInfo.ContainsKey(cdt))
                cdtInfo = numInfo[cdt];
            else
            {
                cdtInfo = new Dictionary<int, Dictionary<int, List<string>>>();
                numInfo[cdt] = cdtInfo;
            }
            Dictionary<int, List<string>> lastInfo = null;
            if (cdtInfo.ContainsKey(lastMissCount))
                lastInfo = cdtInfo[lastMissCount];
            else
            {
                lastInfo = new Dictionary<int, List<string>>();
                cdtInfo[lastMissCount] = lastInfo;
            }

            List<string> curInfo = null;
            if (lastInfo.ContainsKey(curMissCount))
                curInfo = lastInfo[curMissCount];
            else
            {
                curInfo = new List<string>();
                lastInfo[curMissCount] = curInfo;
            }

            curInfo.Add(idTag);
        }

        void AnalyseContinueLongMissCount()
        {
            int loop = CALC_PER_COUNT;
            while (cItem != null && loop-- > 0)
            {
                if (cItem != null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        sum = cItem.statisticInfo.allStatisticInfo[i];

                        for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                        {
                            CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                            su = sum.statisticUnitMap[cdt];

                            if (su.missCount == 0)
                            {
                                int currentLongMissCount = currentLongMissCountInfo[i][cdt];
                                int lastLongMissCount = lastLongMissCountInfo[i][cdt];
                                if (lastLongMissCount == 0)
                                {
                                    if (currentLongMissCount >= GlobalSetting.G_MISS_COUNT_FIRST)
                                        lastLongMissCountInfo[i][cdt] = currentLongMissCount;
                                    currentLongMissCountInfo[i][cdt] = 0;
                                }
                                else
                                {
                                    if(currentLongMissCount >= GlobalSetting.G_MISS_COUNT_SECOND)
                                    {
                                        // record
                                        RecordContinueLongMissCount(i, cdt, lastLongMissCount, currentLongMissCount, cItem.idTag);
                                    }

                                    if(currentLongMissCount >= GlobalSetting.G_MISS_COUNT_FIRST)
                                    {
                                        lastLongMissCountInfo[i][cdt] = currentLongMissCount;
                                    }
                                    else
                                    {
                                        lastLongMissCountInfo[i][cdt] = 0;
                                    }
                                    currentLongMissCountInfo[i][cdt] = 0;
                                }
                            }
                            // 只有当上次记录的值>=0时才记录
                            else
                            {
                                currentLongMissCountInfo[i][cdt] = su.missCount;
                            }
                        }
                    }
                    lastDataItemTag = cItem.idTag;
                    cItem = cItem.parent.GetNextItem(cItem);

                    if (cItem == null)
                    {
                        if (endIndex == dateIDLst.Count - 1)
                        {
                            hasFinished = true;
                            lastDataItemTag = "";
                            status = ProcStatus.eCompleted;
                        }
                        else
                        {
                            startIndex = endIndex;
                            status = ProcStatus.ePrepBatch;
                        }
                        RefreshProgress();
                        this.Invalidate(true);
                        return;
                    }
                    RefreshProgress();
                }
            }
            this.Invalidate(true);
        }

        void AnalyseForMissCountTotal()
        {
            int loop = CALC_PER_COUNT;
            while (cItem != null && loop-- > 0)
            {
                if (cItem != null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        cdtMissCountNumMap = allCDTMissCountNumMap[i];
                        sum = cItem.statisticInfo.allStatisticInfo[i];

                        for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                        {
                            CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                            missCountNumMap = cdtMissCountNumMap[cdt];
                            su = sum.statisticUnitMap[cdt];

                            if (su.missCount == 0)
                            {
                                int lastMissCount = numberPathMissCount[i][cdt];
                                numberPathMissCount[i][cdt] = 0;
                                if (missCountNumMap.ContainsKey(lastMissCount))
                                    missCountNumMap[lastMissCount] = missCountNumMap[lastMissCount] + 1;
                                else
                                    missCountNumMap[lastMissCount] = 1;
                            }
                            else
                            {
                                numberPathMissCount[i][cdt] = su.missCount;
                            }
                            //if (missCountNumMap.ContainsKey(su.missCount))
                            //    missCountNumMap[su.missCount] = missCountNumMap[su.missCount] + 1;
                            //else
                            //    missCountNumMap[su.missCount] = 1;
                        }
                    }
                    lastDataItemTag = cItem.idTag;
                    cItem = cItem.parent.GetNextItem(cItem);

                    if (cItem == null)
                    {
                        //if (endIndex == dateIDLst[dateIDLst.Count - 1])
                        if (endIndex == dateIDLst.Count - 1)
                        {
                            hasFinished = true;
                            lastDataItemTag = "";
                            status = ProcStatus.eCompleted;
                        }
                        else
                        {
                            startIndex = endIndex;
                            status = ProcStatus.ePrepBatch;
                        }

                        RefreshProgress();
                        this.Invalidate(true);
                        return;
                    }

                    RefreshProgress();                }
            }
            this.Invalidate(true);
        }

        void AnalyseForMissCountOnTouchBolleanUp()
        {
            int loop = CALC_PER_COUNT;
            while (cItem != null && loop-- > 0)
            {
                if (cItem != null)
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        sum = cItem.statisticInfo.allStatisticInfo[i];

                        for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                        {
                            CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                            su = sum.statisticUnitMap[cdt];

                            if (su.missCount == 0)
                            {
                                int lastMissCount = numberPathMissCount[i][cdt];

                                // 判断当前这一期是否在触及布林中轨
                                bool isCurrentHitBooleanUp = CheckIsFullUp(i, cdt, cItem);

                                // 上次记录的遗漏值如果超过指定的遗漏值,就记录之
                                if (lastMissCount > GlobalSetting.G_OVER_SPEC_MISS_COUNT)
                                {
                                    Dictionary<CollectDataType, Dictionary<int, List<string>>> numInfo = overMissCountAndTouchBolleanUpInfos[i];
                                    Dictionary<int, List<string>> cdtMissCountInfo = null;
                                    numInfo.TryGetValue(cdt, out cdtMissCountInfo);
                                    if (cdtMissCountInfo == null)
                                    {
                                        cdtMissCountInfo = new Dictionary<int, List<string>>();
                                        numInfo[cdt] = cdtMissCountInfo;
                                    }
                                    List<string> missInfo = null;
                                    cdtMissCountInfo.TryGetValue(lastMissCount, out missInfo);
                                    if (missInfo == null)
                                    {
                                        missInfo = new List<string>();
                                        cdtMissCountInfo[lastMissCount] = missInfo;
                                    }
                                    missInfo.Add(cItem.idTag);
                                }

                                // 当前触到布林中轨,就设置下次开始记录的遗漏值
                                if (isCurrentHitBooleanUp)
                                {
                                    numberPathMissCount[i][cdt] = 0;
                                }
                                // 否则就不需要记录了
                                else
                                {
                                    numberPathMissCount[i][cdt] = -1;
                                }
                            }
                            // 只有当上次记录的值>=0时才记录
                            else if (numberPathMissCount[i][cdt] >= 0)
                            {
                                numberPathMissCount[i][cdt] = su.missCount;
                            }
                        }
                    }
                    lastDataItemTag = cItem.idTag;
                    cItem = cItem.parent.GetNextItem(cItem);

                    if (cItem == null)
                    {
                        if (endIndex == dateIDLst.Count - 1)
                        {
                            hasFinished = true;
                            lastDataItemTag = "";
                            status = ProcStatus.eCompleted;
                        }
                        else
                        {
                            startIndex = endIndex;
                            status = ProcStatus.ePrepBatch;
                        }
                        RefreshProgress();
                        this.Invalidate(true);
                        return;
                    }
                    RefreshProgress();
                }
            }
            this.Invalidate(true);
        }

        void RefreshProgress()
        {
            if (dateIDLst == null || dateIDLst.Count == 0)
            {
                progressBarMain.Value = 100;
            }
            else
            {
                progressBarMain.Value = (int)((float)(endIndex + 1) * 100.0f / dateIDLst.Count);
            }
            if (cItem != null)
            {
                progressBarSub.Value = (int)(((float)(cItem.idGlobal + 1) / allDataItemCount) * 100.0f);
                textBoxProgress.Text = "当前" + cItem.idGlobal + "/" + allDataItemCount + ", 总(" + startIndex + "-" + endIndex + ")/" + dateIDLst.Count;
            }
            else
            {
                progressBarSub.Value = 100;
                //textBoxProgress.Text = "完成";
            }
        }

        bool CheckIsTouchBolleanUp(int numIndex, CollectDataType cdt, DataItem testItem)
        {
            DataItem pItem = testItem;
            KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
            KDataDict kdd = kddc.GetKDataDict(pItem);
            KData kd = kdd.GetData(cdt, false);
            BollinPoint bp = kddc.GetBollinPointMap(kdd).GetData(cdt, false);
            return (kd.RelateDistTo(bp.upValue) <= 0);
        }

        bool CheckIsFullUp(int numIndex, CollectDataType cdt, DataItem testItem)
        {
            DataItem prevItem = testItem.parent.GetPrevItem(testItem);
            if (prevItem == null)
                return false;

            KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
            KDataDict kddCur = kddc.GetKDataDict(testItem);
            KData kdCur = kddCur.GetData(cdt, false);
            BollinPoint bpCur = kddc.GetBollinPointMap(kddCur).GetData(cdt, false);
            MACDPoint macdCur = kddc.GetMacdPointMap(kddCur).GetData(cdt, false);
            bool isCurTouchBU = kdCur.RelateDistTo(bpCur.upValue) <= 0;

            KDataDict kddPrv = kddc.GetKDataDict(prevItem);
            KData kdPrv = kddCur.GetData(cdt, false);
            BollinPoint bpPrv = kddc.GetBollinPointMap(kddPrv).GetData(cdt, false);
            MACDPoint macdPrv = kddc.GetMacdPointMap(kddPrv).GetData(cdt, false);
            bool isPrvTouchBU = kdPrv.RelateDistTo(bpPrv.upValue) <= 0;

            return isCurTouchBU && isPrvTouchBU && macdCur.BAR > macdPrv.BAR && macdCur.DIF > macdPrv.DIF; 
        }

        void DoUpdate()
        {
            if (isStop)
                return;
            switch(status)
            {
                case ProcStatus.eStart:
                case ProcStatus.ePrepBatch:
                    {
                        if (updateCountDown <= 0)
                        {
                            DoPrepBatch();
                            updateCountDown = updateInterval;
                        }
                        else
                        {
                            updateCountDown -= Program.DeltaTime;
                        }
                    }
                    break;
                case ProcStatus.eDoBatch:
                    DoBatch();
                    break;
            }
        }

        public static void CollectSelectDateMissCount(List<int> tags)
        {
            AnalyzeInfoCollectWindow.Open();
            AnalyzeInfoCollectWindow.sInst.CollectSelectDateMissCountImpl(tags);
        }

        void CollectSelectDateMissCountImpl(List<int> tags)
        {
            isStop = false;
            status = ProcStatus.eStart;
            //allCDTMissCountTreeNodeMap.Clear();
            progressBarMain.Value = 0;
            treeViewMissCount.Nodes.Clear();
            DataManager dataMgr = DataManager.GetInst();
            dateIDLst.Clear();
            dateIDLst.AddRange(tags);
            startIndex = 0;
            allCDTMissCountNumMap.Clear();
            hasFinished = false;
            numberPathMissCount.Clear();
            overMissCountAndTouchBolleanUpInfos.Clear();
            continueMissCountInfos.Clear();
            lastLongMissCountInfo.Clear();
            currentLongMissCountInfo.Clear();
            for (int i = 0; i < 5; ++i)
            {
                Dictionary<CollectDataType, Dictionary<int, int>> dct = new Dictionary<CollectDataType, Dictionary<int, int>>();
                allCDTMissCountNumMap.Add(dct);

                Dictionary<CollectDataType, int> cdtMissCountMap = new Dictionary<CollectDataType, int>();
                numberPathMissCount.Add(cdtMissCountMap);

                overMissCountAndTouchBolleanUpInfos.Add(new Dictionary<CollectDataType, Dictionary<int, List<string>>>());

                continueMissCountInfos.Add(new Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>>());
                Dictionary<CollectDataType, int> lastLongMissCount = new Dictionary<CollectDataType, int>();
                Dictionary<CollectDataType, int> currentLongMissCount = new Dictionary<CollectDataType, int>();
                lastLongMissCountInfo.Add(lastLongMissCount);
                currentLongMissCountInfo.Add(currentLongMissCount);

                for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    TreeNode cdtNode = new TreeNode(GraphDataManager.S_CDT_TAG_LIST[j]);
                    cdtNode.Tag = cdt;
                    dct.Add(cdt, new Dictionary<int, int>());
                    cdtMissCountMap.Add(cdt, 0);
                    lastLongMissCount.Add(cdt, 0);
                    currentLongMissCount.Add(cdt, 0);
                }
            }
            this.Invalidate(true);

        }

        private void buttonCollectMissCount_Click(object sender, EventArgs e)
        {
            isStop = false;
            status = ProcStatus.eStart;
            //allCDTMissCountTreeNodeMap.Clear();
            progressBarMain.Value = 0;
            treeViewMissCount.Nodes.Clear();
            DataManager dataMgr = DataManager.GetInst();
            dateIDLst.Clear();
            dateIDLst.AddRange(dataMgr.mFileMetaInfo.Keys);
            startIndex = 0;
            allCDTMissCountNumMap.Clear();
            hasFinished = false;
            numberPathMissCount.Clear();
            overMissCountAndTouchBolleanUpInfos.Clear();
            continueMissCountInfos.Clear();
            lastLongMissCountInfo.Clear();
            currentLongMissCountInfo.Clear();
            for (int i = 0; i < 5; ++i )
            { 
                Dictionary<CollectDataType, Dictionary<int, int>> dct = new Dictionary<CollectDataType, Dictionary<int, int>>();
                allCDTMissCountNumMap.Add(dct);

                Dictionary<CollectDataType, int> cdtMissCountMap = new Dictionary<CollectDataType, int>();
                numberPathMissCount.Add(cdtMissCountMap);

                overMissCountAndTouchBolleanUpInfos.Add(new Dictionary<CollectDataType, Dictionary<int, List<string>>>());

                continueMissCountInfos.Add(new Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>>());
                Dictionary<CollectDataType, int> lastLongMissCount = new Dictionary<CollectDataType, int>();
                Dictionary<CollectDataType, int> currentLongMissCount = new Dictionary<CollectDataType, int>();
                lastLongMissCountInfo.Add(lastLongMissCount);
                currentLongMissCountInfo.Add(currentLongMissCount);

                for ( int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j )
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    TreeNode cdtNode = new TreeNode(GraphDataManager.S_CDT_TAG_LIST[j]);
                    cdtNode.Tag = cdt;
                    dct.Add(cdt, new Dictionary<int, int>());
                    cdtMissCountMap.Add(cdt, 0);
                    lastLongMissCount.Add(cdt, 0);
                    currentLongMissCount.Add(cdt, 0);
                }
            }
            this.Invalidate(true);
        }

        private void AnalyzeInfoCollectWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMain.RemoveWindow(this);
            Program.RemoveUpdater(this);
            sInst = null;
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            switch(currentDataAnalyseType)
            {
                case DataAnalyseType.eDAT_MissCountTotal:
                    ExportForMissCountTotal();
                    break;
                case DataAnalyseType.eDAT_MissCountOnTouchBolleanUp:
                    ExportForMissCountOnTouchBolleanUp();
                    break;
                case DataAnalyseType.eDAT_ContinueLongMissCount:
                    ExportForContinueLongMissCount();
                    break;
            }
        }

        private void comboBoxDataAnalyseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentDataAnalyseType = (DataAnalyseType)comboBoxDataAnalyseType.SelectedIndex;
        }

        void ExportForContinueLongMissCount()
        {
            string fileName = ExportDataPaths[(int)currentDataAnalyseType];
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string info = "<root type=\"" + currentDataAnalyseType.ToString() + "\">\n";
            sw.Write(info);

            for (int i = 0; i < 5; ++i)
            {
                info = "\t<Num pos=\"" + KDataDictContainer.C_TAGS[i] + "\">\n";
                sw.Write(info);

                Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>> numInfo = continueMissCountInfos[i];
                for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    Dictionary<int, Dictionary<int, List<string>>> cdtInfo = null;
                    if (numInfo.ContainsKey(cdt) == false)
                        continue;
                    cdtInfo = numInfo[cdt];

                    info = "\t\t<CDT name=\"" + GraphDataManager.S_CDT_TAG_LIST[j] + "\">\n";
                    sw.Write(info);

                    List<int> mcFirstLst = new List<int>();
                    mcFirstLst.AddRange(cdtInfo.Keys);
                    mcFirstLst.Sort((x, y) => { if (x < y) return -1; return 1; });
                    for( int fi = 0; fi < mcFirstLst.Count; ++fi )
                    //foreach (int firstMissCount in cdtInfo.Keys)
                    {
                        int firstMissCount = mcFirstLst[fi];
                        Dictionary<int, List<string>> curMissCountInfo = cdtInfo[firstMissCount];
                        info = "\t\t\t<FirstMissCount count=\"" + firstMissCount + "\">\n";
                        sw.Write(info);

                        List<int> mcSecondLst = new List<int>();
                        mcSecondLst.AddRange(curMissCountInfo.Keys);
                        mcSecondLst.Sort((x, y) => { if (x < y) return -1; return 1; });
                        for (int si = 0; si < mcSecondLst.Count; ++si)
                        //foreach(int secMissCount in curMissCountInfo.Keys)
                        {
                            int secMissCount = mcSecondLst[si];
                            List<string> tags = curMissCountInfo[secMissCount];
                            info = "\t\t\t\t<SecondMissCount count=\"" + secMissCount + "\">\n";
                            sw.Write(info);

                            for(int t = 0; t < tags.Count; ++t)
                            {
                                info = "\t\t\t\t\t<MissCountInfo>" + tags[t] + "</MissCountInfo>\n";
                                sw.Write(info);
                            }

                            info = "\t\t\t\t</SecondMissCount>\n";
                            sw.Write(info);
                        }

                        info = "\t\t\t</FirstMissCount>\n";
                        sw.Write(info);
                    }

                    info = "\t\t</CDT>\n";
                    sw.Write(info);
                }

                info = "\t</Num>\n";
                sw.Write(info);
            }

            info = "</root>";
            sw.Write(info);

            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        void ExportForMissCountTotal()
        {
            string fileName = ExportDataPaths[(int)currentDataAnalyseType];
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string info = "<root type=\"" + currentDataAnalyseType.ToString() + "\">\n";
            sw.Write(info);

            for (int i = 0; i < 5; ++i)
            {
                info = "\t<Num pos=\"" + KDataDictContainer.C_TAGS[i] + "\">\n";
                sw.Write(info);

                //cdtMissCountTreeNodeMap = allCDTMissCountTreeNodeMap[i];
                cdtMissCountNumMap = allCDTMissCountNumMap[i];

                for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                {
                    info = "\t\t<CDT name=\"" + GraphDataManager.S_CDT_TAG_LIST[j] + "\">\n";
                    sw.Write(info);

                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    //missCountTreeNodeMap = cdtMissCountTreeNodeMap[cdt];
                    missCountNumMap = cdtMissCountNumMap[cdt];

                    foreach (int key in missCountNumMap.Keys)
                    {
                        info = "\t\t\t<MissCount miss=\"" + key + "\" count=\"" + missCountNumMap[key] + "\"/>\n";
                        sw.Write(info);
                    }

                    info = "\t\t</CDT>\n";
                    sw.Write(info);
                }

                info = "\t</Num>\n";
                sw.Write(info);
            }

            info = "</root>";
            sw.Write(info);

            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        void ExportForMissCountOnTouchBolleanUp()
        {
            string fileName = ExportDataPaths[(int)currentDataAnalyseType];// "..\\tools\\从布林上轨连续遗漏的统计结果.xml";
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string info = "<root type=\"" + currentDataAnalyseType.ToString() + "\">\n";
            sw.Write(info);

            for (int i = 0; i < 5; ++i)
            {
                info = "\t<Num pos=\"" + KDataDictContainer.C_TAGS[i] + "\">\n";
                sw.Write(info);

                Dictionary<CollectDataType, Dictionary<int, List<string>>> cdtMissInfo = overMissCountAndTouchBolleanUpInfos[i];

                for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    info = "\t\t<CDT name=\"" + cdt + "\">\n";
                    sw.Write(info);

                    Dictionary<int, List<string>> countInfo = cdtMissInfo[cdt];
                    if (countInfo == null)
                        continue;
                    
                    foreach (int key in countInfo.Keys)
                    {
                        info = "\t\t\t<Count count=\"" + key + "\">\n";
                        sw.Write(info);

                        List<string> missInfo = countInfo[key];
                        for( int jj = 0; jj < missInfo.Count; ++jj )
                        {
                            info = "\t\t\t\t<MissInfo item=\"" + missInfo[jj] + "\"/>\n";
                            sw.Write(info);
                        }
                        
                        info = "\t\t\t</Count>\n";
                        sw.Write(info);
                    }

                    info = "\t\t</CDT>\n";
                    sw.Write(info);
                }

                info = "\t</Num>\n";
                sw.Write(info);
            }

            info = "</root>";
            sw.Write(info);
            
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            isStop = true;
        }

        void ImportData()
        {
            switch(currentDataAnalyseType)
            {
                case DataAnalyseType.eDAT_MissCountOnTouchBolleanUp:
                    ImportFromMissCountOnTouchBolleanUp();
                    break;
                case DataAnalyseType.eDAT_MissCountTotal:
                    ImportFromMissCountTotal();
                    break;
                case DataAnalyseType.eDAT_ContinueLongMissCount:
                    ImportFromContinueLongMissCount();
                    break;
            }
        }

        void ImportFromMissCountTotal()
        {

        }

        void ImportFromMissCountOnTouchBolleanUp()
        {
            string fileName = ExportDataPaths[(int)currentDataAnalyseType];

            XmlDocument x = new XmlDocument();
            try
            {
                x.Load(fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

            XmlNode pNode = x.SelectSingleNode("root");
            if (pNode == null)
                return;

            treeViewMissCount.Nodes.Clear();
            foreach ( XmlNode numNode in pNode.ChildNodes )
            {
                TreeNode tnNum = new TreeNode(numNode.Attributes[0].Value);
                treeViewMissCount.Nodes.Add(tnNum);

                foreach( XmlNode cdtNode in numNode.ChildNodes )
                {
                    TreeNode tnCDT = new TreeNode(cdtNode.Attributes[0].Value);
                    tnNum.Nodes.Add(tnCDT);
                    
                    foreach (XmlNode coundNode in cdtNode.ChildNodes)
                    {
                        TreeNode tnCount = new TreeNode(coundNode.Attributes[0].Value);
                        tnCDT.Nodes.Add(tnCount);

                        foreach (XmlNode missNode in coundNode.ChildNodes)
                        {
                            TreeNode tnMiss = new TreeNode(missNode.Attributes[0].Value);
                            tnCount.Nodes.Add(tnMiss);

                            int date = int.Parse(tnMiss.Text.Split('-')[0]);
                            tnMiss.Tag = date;
                        }
                    }
                }
            }
        }

        void ImportFromContinueLongMissCount()
        {
            string fileName = ExportDataPaths[(int)currentDataAnalyseType];

            XmlDocument x = new XmlDocument();
            try
            {
                x.Load(fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

            XmlNode pNode = x.SelectSingleNode("root");
            if (pNode == null)
                return;

            treeViewMissCount.Nodes.Clear();
            foreach (XmlNode numNode in pNode.ChildNodes)
            {
                TreeNode tnNum = new TreeNode(numNode.Attributes[0].Value);
                treeViewMissCount.Nodes.Add(tnNum);

                foreach (XmlNode cdtNode in numNode.ChildNodes)
                {
                    TreeNode tnCDT = new TreeNode(cdtNode.Attributes[0].Value);
                    tnNum.Nodes.Add(tnCDT);

                    foreach (XmlNode firstNode in cdtNode.ChildNodes)
                    {
                        TreeNode tnFirst = new TreeNode(firstNode.Attributes[0].Value);
                        tnCDT.Nodes.Add(tnFirst);

                        foreach (XmlNode secNode in firstNode.ChildNodes)
                        {
                            TreeNode tnSec = new TreeNode(secNode.Attributes[0].Value);
                            tnFirst.Nodes.Add(tnSec);

                            foreach (XmlNode missNode in secNode.ChildNodes)
                            {
                                TreeNode tnMiss = new TreeNode(missNode.InnerXml);
                                tnSec.Nodes.Add(tnMiss);

                                int date = int.Parse(tnMiss.Text.Split('-')[0]);
                                tnMiss.Tag = date;
                            }
                        }
                    }
                }
            }
        }

        private void buttonImportData_Click(object sender, EventArgs e)
        {
            ImportData();
        }

        private void treeViewMissCount_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = treeViewMissCount.SelectedNode;
            if (node != null && node.Tag != null)
            {
                FormMain.Instance.ShowSpecData((int)node.Tag);
                int itemID = DataManager.GetInst().GetDataItemByIdTag(node.Text).idGlobal;
                string cdtSTR = node.Parent.Parent.Parent.Text;
                string numSTR = node.Parent.Parent.Parent.Parent.Text;
                int cdtIndex = GraphDataManager.GetCdtIndex(cdtSTR);
                int numIndex = GraphDataManager.GetNumIndex(numSTR);
                LotteryGraph.G_SetSelCollectDataType(cdtIndex);
                LotteryGraph.G_SetSelNumIndex(numIndex);
                LotteryGraph.G_SelItem(itemID);
                LotteryGraph.NotifyAllGraphsRefresh(true);
            }
        }
    }
}
