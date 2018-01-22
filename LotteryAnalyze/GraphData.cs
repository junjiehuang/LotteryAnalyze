using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    #region Data Manager

    public enum CollectDataType
    {
        eNone,

        ePath0 = 1 << 0,
        ePath1 = 1 << 1,
        ePath2 = 1 << 2,

        eMax = 0xFFFFFFF,
    }

    class KData
    {
        public CollectDataType cdt;
        public KDataDict parent;
        public int index;
        public float HitValue;
        public float MissValue;
        public float KValue;

        string info = null;
        public string GetInfo()
        {
            if (info == null)
            {
                int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                info = "[" + parent.startItem.idTag + "-" + parent.endItem.idTag + "] [" +
                    GraphDataManager.S_CDT_TAG_LIST[cdtID] + "] [" +
                    HitValue + " : " + MissValue + "]";
            }
            return info;
        }
    }

    class KDataDict
    {
        public int index;
        public Dictionary<CollectDataType, KData> dataDict = new Dictionary<CollectDataType, KData>();
        public DataItem startItem = null;
        public DataItem endItem = null;

        public KData GetData(CollectDataType collectDataType, bool createIfNotExist)
        {
            if (dataDict.ContainsKey(collectDataType))
                return dataDict[collectDataType];
            else if (createIfNotExist == false)
                return null;
            KData data = new KData();
            data.parent = this;
            data.cdt = collectDataType;
            dataDict.Add(collectDataType, data);
            return data;
        }

        public void Clear()
        {
            dataDict.Clear();
        }
    }

    // 均线计算方式
    public enum AvgAlgorithm
    {
        // 简单算术平均
        eSMA,
        // 末日加权平均
        eWMALastDay,
        // 线性加权平均
        eWMALinear,
        // 平方系数加权平均
        eWMASqr,
        // 指数平滑移动平均
        eEMA,
    }

    class AvgPoint
    {
        //public float avgHit = 0;
        //public float avgMiss = 0;
        public float avgKValue = 0;
    }

    class AvgPointMap
    {
        public AvgDataContainer parent;
        public int index = -1;
        public Dictionary<CollectDataType, AvgPoint> apMap = new Dictionary<CollectDataType, AvgPoint>();
        
        public AvgPointMap()
        {
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
                GetData(GraphDataManager.S_CDT_LIST[i], true);
        }

        public AvgPoint GetData(CollectDataType collectDataType, bool createIfNotExist)
        {
            if (apMap.ContainsKey(collectDataType))
                return apMap[collectDataType];
            else if (createIfNotExist == false)
                return null;
            AvgPoint data = new AvgPoint();
            apMap.Add(collectDataType, data);
            return data;
        }

        public AvgPointMap GetPrevAPM()
        {
            if (index > 0)
                return parent.avgPointMapLst[index - 1];
            return null;
        }
    }

    class AvgDataContainer
    {
        public KGraphDataContainer.AvgLineSetting avgLineSetting = null;
        public bool enable = false;
        public int cycle = 5;
        public List<AvgPointMap> avgPointMapLst = new List<AvgPointMap>();

        public void Process(List<KDataDict> srcData)
        {
            if(enable == false)
            {
                avgPointMapLst.Clear();
                return;
            }
            switch(KGraphDataContainer.S_AVG_ALGORITHM)
            {
                case AvgAlgorithm.eEMA:
                    ProcessByEMA(srcData);
                    break;
                case AvgAlgorithm.eSMA:
                    ProcessBySMA(srcData);
                    break;
                case AvgAlgorithm.eWMALastDay:
                    ProcessByWMALastDay(srcData);
                    break;
                case AvgAlgorithm.eWMALinear:
                    ProcessByWMALinear(srcData);
                    break;
                case AvgAlgorithm.eWMASqr:
                    ProcessByWMASqr(srcData);
                    break;
            }
        }
        AvgPointMap CreateAvgPointMap()
        {
            AvgPointMap apm = new AvgPointMap();
            apm.index = avgPointMapLst.Count;
            apm.parent = this;
            avgPointMapLst.Add(apm);
            return apm;
        }
        void ProcessBySMA(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for ( int i = 0; i < srcData.Count; ++i )
            {
                AvgPointMap apm = CreateAvgPointMap();

                int startIndex = i - cycle + 1;
                if (startIndex < 0)
                    startIndex = 0;
                int totalSub = i - startIndex + 1;
                for ( ; startIndex <= i; ++startIndex)
                {
                    KDataDict kdd = srcData[startIndex];
                    for( int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t )
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                        AvgPoint ap = apm.apMap[cdt];
                        KData kd = kdd.dataDict[cdt];
                        //ap.avgHit += kd.HitValue;
                        //ap.avgMiss += kd.MissValue;
                        ap.avgKValue += kd.KValue;
                    }
                }
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    //ap.avgHit /= totalSub;
                    //ap.avgMiss /= totalSub;
                    ap.avgKValue /= totalSub;
                }
            }
        }
        void ProcessByWMALastDay(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap();

                int startIndex = i - cycle + 1;
                if (startIndex < 0)
                    startIndex = 0;
                int totalSub = 0;                
                for (; startIndex <= i; ++startIndex)
                {
                    KDataDict kdd = srcData[startIndex];
                    bool isLastDay = (startIndex == i && totalSub > 0);
                    for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                        AvgPoint ap = apm.apMap[cdt];
                        KData kd = kdd.dataDict[cdt];
                        if (isLastDay)
                        {
                            //ap.avgHit += kd.HitValue * 2;
                            //ap.avgMiss += kd.MissValue * 2;
                            ap.avgKValue += kd.KValue * 2;
                        }
                        else
                        {
                            //ap.avgHit += kd.HitValue;
                            //ap.avgMiss += kd.MissValue;
                            ap.avgKValue += kd.KValue;
                        }
                    }
                    totalSub += isLastDay ? 2 : 1;
                }
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    //ap.avgHit /= totalSub;
                    //ap.avgMiss /= totalSub;
                    ap.avgKValue /= totalSub;
                }
            }
        }
        void ProcessByWMALinear(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap();

                int startIndex = i - cycle + 1;
                if (startIndex < 0)
                    startIndex = 0;
                int totalSub = 0;
                int loop = 1;
                for (; startIndex <= i; ++startIndex)
                {
                    KDataDict kdd = srcData[startIndex];
                    for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                        AvgPoint ap = apm.apMap[cdt];
                        KData kd = kdd.dataDict[cdt];
                        //ap.avgHit += kd.HitValue * loop;
                        //ap.avgMiss += kd.MissValue * loop;
                        ap.avgKValue += kd.KValue * loop;
                    }
                    totalSub += loop;
                    ++loop;
                }
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    //ap.avgHit /= totalSub;
                    //ap.avgMiss /= totalSub;
                    ap.avgKValue /= totalSub;
                }
            }
        }
        void ProcessByWMASqr(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap();

                int startIndex = i - cycle + 1;
                if (startIndex < 0)
                    startIndex = 0;
                int totalSub = 0;
                int loop = 1;
                for (; startIndex <= i; ++startIndex)
                {
                    int sqrLoop = loop * loop;
                    KDataDict kdd = srcData[startIndex];
                    for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                        AvgPoint ap = apm.apMap[cdt];
                        KData kd = kdd.dataDict[cdt];
                        //ap.avgHit += kd.HitValue * sqrLoop;
                        //ap.avgMiss += kd.MissValue * sqrLoop;
                        ap.avgKValue += kd.KValue * sqrLoop;
                    }
                    totalSub += sqrLoop;
                    ++loop;
                }
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    //ap.avgHit /= totalSub;
                    //ap.avgMiss /= totalSub;
                    ap.avgKValue /= totalSub;
                }
            }
        }
        void ProcessByEMA(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap();

                KDataDict kdd = srcData[i];
                AvgPointMap prevApm = null;
                if (i > 0)
                    prevApm = avgPointMapLst[i - 1];

                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    KData kd = kdd.dataDict[cdt];
                    if (prevApm == null)
                    {
                        //ap.avgHit = kd.HitValue;
                        //ap.avgMiss = kd.MissValue;
                        ap.avgKValue = kd.KValue;
                    }
                    else
                    {
                        //ap.avgHit = (kd.HitValue * 2 + (cycle - 1) * prevApm.apMap[cdt].avgHit) / (cycle + 1);
                        //ap.avgMiss = (kd.MissValue * 2 + (cycle - 1) * prevApm.apMap[cdt].avgMiss) / (cycle + 1);
                        ap.avgKValue = (kd.KValue * 2 + (cycle - 1) * prevApm.apMap[cdt].avgKValue) / (cycle + 1);
                    }
                }
            }
        }
    }

    class KDataDictContainer
    {
        public List<KDataDict> dataLst = new List<KDataDict>();
        public Dictionary<int, AvgDataContainer> avgDataContMap = new Dictionary<int, AvgDataContainer>();

        public KDataDictContainer()
        {
            for (int i = 0; i < KGraphDataContainer.S_AVG_LINE_SETTINGS.Count; ++i)
            {
                KGraphDataContainer.AvgLineSetting als = KGraphDataContainer.S_AVG_LINE_SETTINGS[i];
                CreateAvgDataContainer(als);
            }
        }

        void CreateAvgDataContainer(KGraphDataContainer.AvgLineSetting als)
        {
            AvgDataContainer adc = new AvgDataContainer();
            adc.avgLineSetting = als;
            adc.cycle = als.cycle;
            adc.enable = als.enable;
            avgDataContMap.Add(als.cycle, adc);
        }

        public KDataDict CreateKDataDict()
        {
            KDataDict kdd = new KDataDict();
            kdd.index = dataLst.Count;
            dataLst.Add(kdd);
            return kdd;
        }
    }

    // 图表数据容器基类
    class GraphDataContainerBase
    {
        public virtual int DataLength() { return 0; }
        public virtual bool HasData() { return false; }
        public virtual void CollectGraphData() { }
    }

    // K线图数据容器
    class KGraphDataContainer : GraphDataContainerBase
    {
        #region Avg line setting
        public class AvgLineSetting
        {
            public string tag;
            public int cycle;
            public Color color;
            public bool enable;
            public Pen pen;
        }
        public static List<AvgLineSetting> S_AVG_LINE_SETTINGS = new List<AvgLineSetting>();
        public static AvgAlgorithm S_AVG_ALGORITHM = AvgAlgorithm.eEMA;
        public static List<string> S_AVG_ALGORITHM_STRS = new List<string>();
        static KGraphDataContainer()
        {
            AddAvgLineSetting("5", 5, Color.Red, true);
            AddAvgLineSetting("10", 10, Color.Purple, true);
            AddAvgLineSetting("20", 20, Color.Orange, true);
            AddAvgLineSetting("30", 30, Color.Yellow, true);
            AddAvgLineSetting("50", 50, Color.Green, true);
            AddAvgLineSetting("100", 100, Color.White, true);
            S_AVG_ALGORITHM_STRS.Add("简单算术平均(SMA)");
            S_AVG_ALGORITHM_STRS.Add("末日加权平均(WMALastDay)");
            S_AVG_ALGORITHM_STRS.Add("线性加权平均(WMALinear)");
            S_AVG_ALGORITHM_STRS.Add("平方系数加权平均(WMASqr)");
            S_AVG_ALGORITHM_STRS.Add("指数平滑移动平均(EMA)");
        }
        static void AddAvgLineSetting(string tag, int cycle, Color col, bool enable)
        {
            AvgLineSetting als = new AvgLineSetting();
            als.tag = tag;
            als.cycle = cycle;
            als.color = col;
            als.enable = enable;
            als.pen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, col, 1);
            S_AVG_LINE_SETTINGS.Add(als);
        }
        #endregion

        int dataLength = 0;
        int cycleLength = 5;
        List<KDataDictContainer> allKDatas = new List<KDataDictContainer>();
        public int CycleLength
        {
            get { return cycleLength; }
            set { cycleLength = value; }
        }
        public override int DataLength() { return dataLength; }
        public override bool HasData()
        {
            return allKDatas.Count > 0;
        }
        public override void CollectGraphData()
        {
            Init();
            CollectKDatas();
            CollectAvgDatas();
        }

        public void CollectKDatas()
        {
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            int loop = 0;
            KDataDict[] curDatas = null;
            for (int i = 0; i < count; ++i)
            {
                int oneDayID = DataManager.GetInst().indexs[i];
                OneDayDatas odd = DataManager.GetInst().allDatas[oneDayID];

                for (int j = 0; j < odd.datas.Count; ++j)
                {
                    DataItem item = odd.datas[j];
                    if (curDatas == null)
                        curDatas = CreateKDataDicts();
                    for (int numIndex = 0; numIndex < 5; ++numIndex)
                    {
                        CollectItem(numIndex, item, curDatas[numIndex]);
                    }

                    ++loop;
                    if (loop == cycleLength)
                    {
                        curDatas = null;
                        loop = 0;
                    }
                }
            }
            dataLength = allKDatas[0].dataLst.Count;

            Dictionary<CollectDataType, float> valueMap = new Dictionary<CollectDataType, float>();
            for (int i = 0; i < allKDatas.Count; ++i)
            {
                valueMap.Clear();
                KDataDictContainer kddc = allKDatas[i];
                for (int j = 0; j < kddc.dataLst.Count; ++j)
                {
                    KDataDict kdd = kddc.dataLst[j];
                    foreach (CollectDataType cdt in kdd.dataDict.Keys)
                    {
                        float lastValue = 0;
                        KData kd = kdd.dataDict[cdt];
                        int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                        float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];
                        float valueChange = kd.HitValue - kd.MissValue * missRelHeight;
                        if (valueMap.ContainsKey(cdt))
                        {
                            lastValue = valueMap[cdt];
                            lastValue += valueChange;
                            valueMap[cdt] = lastValue;
                        }
                        else
                        {
                            lastValue = valueChange;
                            valueMap.Add(cdt, lastValue);
                        }
                        kd.KValue = lastValue;
                    }
                }
            }
        }

        public void CollectAvgDatas()
        {
            for (int i = 0; i < allKDatas.Count; ++i)
            {
                KDataDictContainer kddc = allKDatas[i];
                foreach (AvgDataContainer avd in kddc.avgDataContMap.Values)
                {
                    avd.Process(kddc.dataLst);
                }
            }
        }

        public AvgDataContainer GetAvgDataContainer(int numIndex, int cycle)
        {
            if (allKDatas.Count > 0 && allKDatas.Count > numIndex)
            {
                KDataDictContainer kddc = allKDatas[numIndex];
                if(kddc.avgDataContMap.ContainsKey(cycle))
                    return kddc.avgDataContMap[cycle];
            }
            return null;
        }
        public KDataDictContainer GetKDataDictContainer(int numIndex)
        {
            if (allKDatas.Count > numIndex)
                return allKDatas[numIndex];
            return null;
        }
        void Init()
        {
            KDataDictContainer kddc = null;
            for (int i = 0; i < 5; ++i)
            {
                if (i >= allKDatas.Count)
                {
                    kddc = new KDataDictContainer();
                    allKDatas.Add(kddc);
                }
                else
                {
                    kddc = allKDatas[i];
                    kddc.dataLst.Clear();
                }
            }
        }
        void CollectItem(int NUM_INDEX, DataItem item, KDataDict kd)
        {
            if (kd.startItem == null)
                kd.startItem = item;
            if (kd.endItem == null || item.idGlobal > kd.endItem.idGlobal)
                kd.endItem = item;
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
                KData data = kd.GetData(cdt, true);
                data.index = kd.index;
                switch (cdt)
                {
                    case CollectDataType.ePath0:
                        if (item.path012OfEachSingle[NUM_INDEX] == 0)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.ePath1:
                        if (item.path012OfEachSingle[NUM_INDEX] == 1)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.ePath2:
                        if (item.path012OfEachSingle[NUM_INDEX] == 2)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                }
            }
        }
        KDataDict[] CreateKDataDicts()
        {
            KDataDict[] curDatas = new KDataDict[5];
            for (int i = 0; i < 5; ++i)
            {
                curDatas[i] = allKDatas[i].CreateKDataDict();
            }
            return curDatas;
        }
    }

    // 柱状图数据容器
    class BarGraphDataContianer : GraphDataContainerBase
    {
        // 统计类型
        public enum StatisticsType
        {
            // 0-9出现次数统计
            eAppearCountFrom0To9,
            // 012路出现次数统计
            eAppearCountPath012,
        }

        // 统计范围
        public enum StatisticsRange
        {
            // 最近10期
            e10,
            // 最近20期
            e20,
            // 最近30期
            e30,
            // 最近50期
            e50,
            // 最近100期
            e100,
            // 自定义期数
            eCustom,
        }
        public static List<string> S_StatisticsType_STRS = new List<string>();
        public static List<string> S_StatisticsRange_STRS = new List<string>();
        static BarGraphDataContianer()
        {
            S_StatisticsType_STRS.Add("0-9出现次数");
            S_StatisticsType_STRS.Add("012路出现次数");

            S_StatisticsRange_STRS.Add("最近10期");
            S_StatisticsRange_STRS.Add("最近20期");
            S_StatisticsRange_STRS.Add("最近30期");
            S_StatisticsRange_STRS.Add("最近50期");
            S_StatisticsRange_STRS.Add("最近100期");
            S_StatisticsRange_STRS.Add("自定义期数");
        }

        public class DataUnit
        {
            public StatisticsType type;
            public int data;
            public string tag;
        }
        public class DataUnitLst
        {
            public List<DataUnit> dataLst = new List<DataUnit>();
        }
        public StatisticsType curStatisticsType = StatisticsType.eAppearCountFrom0To9;
        public StatisticsRange curStatisticsRange = StatisticsRange.e100;
        public int customStatisticsRange = 120;
        public List<DataUnitLst> allDatas = new List<DataUnitLst>();
        public int totalCollectCount = 0;

        public BarGraphDataContianer()
        {
            for (int i = 0; i < 5; ++i)
                allDatas.Add(new DataUnitLst());
        }
        void Init()
        {
            for (int c = 0; c < 5; ++c)
            {
                allDatas[c].dataLst.Clear();
                switch (curStatisticsType)
                {
                    case StatisticsType.eAppearCountFrom0To9:
                        {
                            for (int i = 0; i <= 9; ++i)
                            {
                                DataUnit du = new DataUnit();
                                du.data = 0;
                                du.type = StatisticsType.eAppearCountFrom0To9;
                                du.tag = i.ToString();
                                allDatas[c].dataLst.Add(du);
                            }
                        }
                        break;
                    case StatisticsType.eAppearCountPath012:
                        {
                            for (int i = 0; i <= 2; ++i)
                            {
                                DataUnit du = new DataUnit();
                                du.data = 0;
                                du.type = StatisticsType.eAppearCountPath012;
                                du.tag = i.ToString();
                                allDatas[c].dataLst.Add(du);
                            }
                        }
                        break;
                }
            }
        }
        void CollectItem(DataItem item)
        {
            for (int i = 0; i < 5; ++i)
            {
                switch (curStatisticsType)
                {
                    case StatisticsType.eAppearCountFrom0To9:
                        {
                            int num = item.GetNumberByIndex(i);
                            DataUnit du = allDatas[i].dataLst[num];
                            du.data = du.data + 1;
                        }
                        break;
                    case StatisticsType.eAppearCountPath012:
                        {
                            int num = item.path012OfEachSingle[i];
                            DataUnit du = allDatas[i].dataLst[num];
                            du.data = du.data + 1;
                        }
                        break;
                }
            }
        }

        public override int DataLength() { return allDatas[0].dataLst.Count; }
        public override bool HasData() { return allDatas[0].dataLst.Count > 0; }
        public override void CollectGraphData()
        {
            totalCollectCount = 0;
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            Init();

            int CollectCount = customStatisticsRange;
            switch (curStatisticsRange)
            {
                case StatisticsRange.e10: CollectCount = 10; break;
                case StatisticsRange.e20: CollectCount = 20; break;
                case StatisticsRange.e30: CollectCount = 30; break;
                case StatisticsRange.e50: CollectCount = 50; break;
                case StatisticsRange.e100: CollectCount = 100; break;
            }
            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (currentItem == null)
                return;
            for (int i = 0; i < CollectCount; ++i)
            {
                CollectItem(currentItem);
                ++totalCollectCount;
                currentItem = DataManager.GetInst().GetPrevItem(currentItem);
                if (currentItem == null)
                    break;
            }
        }
    }

    // 图表数据管理器
    class GraphDataManager
    {
        public static List<CollectDataType> S_CDT_LIST = new List<CollectDataType>();
        public static List<string> S_CDT_TAG_LIST = new List<string>();
        public static List<float> S_CDT_PROBABILITY_LIST = new List<float>();
        public static List<float> S_CDT_MISS_REL_LENGTH_LIST = new List<float>();
        public static Dictionary<GraphType, GraphDataContainerBase> S_GRAPH_DATA_CONTS = new Dictionary<GraphType, GraphDataContainerBase>();

        public static KGraphDataContainer KGDC;
        public static BarGraphDataContianer BGDC;

        static GraphDataManager()
        {
            AddPreInfo(CollectDataType.ePath0, "0路", 4.0f / 10);
            AddPreInfo(CollectDataType.ePath1, "1路", 3.0f / 10);
            AddPreInfo(CollectDataType.ePath2, "2路", 3.0f / 10);

            S_GRAPH_DATA_CONTS.Add(GraphType.eKCurveGraph, KGDC = new KGraphDataContainer());
            S_GRAPH_DATA_CONTS.Add(GraphType.eBarGraph, BGDC = new BarGraphDataContianer());
        }
        static void AddPreInfo(CollectDataType cdt, string name, float probability)
        {
            S_CDT_LIST.Add(cdt);
            S_CDT_TAG_LIST.Add(name);
            S_CDT_PROBABILITY_LIST.Add(probability);
            S_CDT_MISS_REL_LENGTH_LIST.Add(probability / (1.0f - probability));
        }


        static GraphDataManager sInst = null;
        public static GraphDataManager Instance
        {
            get
            {
                if (sInst == null) sInst = new GraphDataManager();
                return sInst;
            }
        }

        public static GraphDataContainerBase GetGraphDataContianer(GraphType gt)
        {
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                return (S_GRAPH_DATA_CONTS[gt]);
            return null;
        }

        public int DataLength(GraphType gt)
        {
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                return S_GRAPH_DATA_CONTS[gt].DataLength();
            return 0;
        }

        public bool HasData(GraphType gt)
        {
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                return S_GRAPH_DATA_CONTS[gt].HasData();
            return false;
        }

        public void CollectGraphData(GraphType gt)
        {
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                S_GRAPH_DATA_CONTS[gt].CollectGraphData();
        }
    }

    #endregion
}