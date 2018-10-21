using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze.UI
{
    public partial class AnalyzeInfoCollectWindow : Form, UpdaterBase
    {
        List<int> dateIDLst = new List<int>();
        double updateInterval = 0.05;
        double updateCountDown = 0;
        int startIndex = -1;
        int endIndex = -1;
        int batchCount = 10;
        string lastDataItemTag = "";
        bool hasFinished = true;
        int allDataItemCount = 0;

        enum ProcStatus
        {
            eNotStart,
            eStart,
            ePrepBatch,
            eDoBatch,
            eCompleted,
        }
        ProcStatus status = ProcStatus.eNotStart;

        List<Dictionary<CollectDataType, Dictionary<int, TreeNode>>> allCDTMissCountTreeNodeMap = new List<Dictionary<CollectDataType, Dictionary<int, TreeNode>>>();
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

            //RefreshTree();
            this.Invalidate(true);
        }

        void RefreshTree()
        {
            for (int i = 0; i < 5; ++i)
            {
                cdtMissCountTreeNodeMap = allCDTMissCountTreeNodeMap[i];
                cdtMissCountNumMap = allCDTMissCountNumMap[i];
                numNode = treeViewMissCount.Nodes[i];

                for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    missCountTreeNodeMap = cdtMissCountTreeNodeMap[cdt];
                    missCountNumMap = cdtMissCountNumMap[cdt];
                    cdtNode = numNode.Nodes[j];

                    foreach (int key in missCountNumMap.Keys)
                    {
                        if (missCountTreeNodeMap.ContainsKey(key))
                        {
                            missCountTreeNodeMap[key].Text = key + " - " + missCountNumMap[key];
                        }
                        else
                        {
                            TreeNode subNode = new TreeNode(key + " - " + missCountNumMap[key]);
                            cdtNode.Nodes.Add(subNode);
                            missCountTreeNodeMap.Add(key, subNode);
                        }
                    }
                }
            }

        }

        void DoBatch()
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
                        if (missCountNumMap.ContainsKey(su.missCount))
                            missCountNumMap[su.missCount] = missCountNumMap[su.missCount] + 1;
                        else
                            missCountNumMap[su.missCount] = 1;
                    }
                }
                lastDataItemTag = cItem.idTag;
                cItem = cItem.parent.GetNextItem(cItem);

                if(cItem == null)
                {
                    if (endIndex == dateIDLst[dateIDLst.Count - 1])
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
                    this.Invalidate(true);
                    return;
                }
                
                progressBarSub.Value = (int)(((float)(cItem.idGlobal + 1) / allDataItemCount) * 100.0f);
                textBoxProgress.Text = "当前" + cItem.idGlobal + "/" + allDataItemCount + ", 总(" + startIndex + "-" + endIndex + ")/" + dateIDLst.Count;
                this.Invalidate(true);
            }
        }

        void DoUpdate()
        {
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

        private void buttonCollectMissCount_Click(object sender, EventArgs e)
        {
            status = ProcStatus.eStart;
            allCDTMissCountTreeNodeMap.Clear();
            progressBarMain.Value = 0;
            treeViewMissCount.Nodes.Clear();
            DataManager dataMgr = DataManager.GetInst();
            dateIDLst.Clear();
            dateIDLst.AddRange(dataMgr.mFileMetaInfo.Keys);
            startIndex = 0;
            allCDTMissCountNumMap.Clear();
            hasFinished = false;
            for(int i = 0; i < 5; ++i )
            {
                TreeNode posNode = new TreeNode(KDataDictContainer.C_TAGS[i]);
                posNode.Tag = i;
                treeViewMissCount.Nodes.Add(posNode);
                Dictionary<CollectDataType, Dictionary<int, TreeNode>> cid = new Dictionary<CollectDataType, Dictionary<int, TreeNode>>();
                allCDTMissCountTreeNodeMap.Add(cid);

                Dictionary<CollectDataType, Dictionary<int, int>> dct = new Dictionary<CollectDataType, Dictionary<int, int>>();
                allCDTMissCountNumMap.Add(dct);
                for( int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j )
                {
                    TreeNode cdtNode = new TreeNode(GraphDataManager.S_CDT_TAG_LIST[j]);
                    cdtNode.Tag = GraphDataManager.S_CDT_LIST[j];
                    posNode.Nodes.Add(cdtNode);

                    dct.Add(GraphDataManager.S_CDT_LIST[j], new Dictionary<int, int>());
                    cid.Add(GraphDataManager.S_CDT_LIST[j], new Dictionary<int, TreeNode>());
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
            string fileName = "..\\tools\\遗漏统计结果.xml";
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string info = "<root>\n";

            for (int i = 0; i < 5; ++i)
            {
                info += "\t<Num pos=\"" + KDataDictContainer.C_TAGS[i] + "\">\n";
                cdtMissCountTreeNodeMap = allCDTMissCountTreeNodeMap[i];
                cdtMissCountNumMap = allCDTMissCountNumMap[i];

                for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                {
                    info += "\t\t<CDT name=\"" + GraphDataManager.S_CDT_TAG_LIST[j] + "\">\n";

                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    missCountTreeNodeMap = cdtMissCountTreeNodeMap[cdt];
                    missCountNumMap = cdtMissCountNumMap[cdt];

                    foreach (int key in missCountNumMap.Keys)
                    {
                        info += "\t\t\t<MissCount miss=\""+key+"\" count=\""+ missCountNumMap[key]+"\"/>\n";
                    }

                    info += "\t\t</CDT>\n";
                }

                info += "\t</Num>\n";
            }

            info += "</root>";

            //开始写入
            sw.Write(info);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }
    }
}
