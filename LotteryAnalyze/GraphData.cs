#define TRADE_DBG

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

        ePath0          = 1 << 0,
        ePath1          = 1 << 1,
        ePath2          = 1 << 2,
        eBigNum         = 1 << 3,
        eSmallNum       = 1 << 4,
        eOddNum         = 1 << 5,
        eEvenNum        = 1 << 6,
        ePrimeNum       = 1 << 7,
        eCompositeNum   = 1 << 8,
        eNum0           = 1 << 9,
        eNum1           = 1 << 10,
        eNum2           = 1 << 11,
        eNum3           = 1 << 12,
        eNum4           = 1 << 13,
        eNum5           = 1 << 14,
        eNum6           = 1 << 15,
        eNum7           = 1 << 16,
        eNum8           = 1 << 17,
        eNum9           = 1 << 18,


        eMax = 0xFFFFFFF,
    }

    public class KData
    {
        public CollectDataType cdt;
        public KDataDict parent;

        public float HitValue;
        public float MissValue;
        public float KValue;

        //string info = null;
        bool hasRefreshUpDownValue = false;
        float upValue;
        float downValue;

        public float UpValue
        {
            get
            {
                RefreshUpDownValue();
                return upValue;
            }
        }

        public float DownValue
        {
            get
            {
                RefreshUpDownValue();
                return downValue;
            }
        }

        public float RelateDistTo(float testValue)
        {
            float KDIST = UpValue - DownValue;
            if (testValue >= DownValue && testValue <= UpValue)
                return 0;
            else if (testValue > UpValue)
            {
                if(KDIST != 0)
                    return (testValue - UpValue) / KDIST;
                return testValue - UpValue;
            }
            else
            {
                if (KDIST != 0)
                    return (testValue - DownValue) / KDIST;
                return testValue - DownValue;
            }
        }

        public int index
        {
            get
            {
                if (parent == null)
                    return -1;
                return parent.index;
            }
        }

        void RefreshUpDownValue()
        {
            if (hasRefreshUpDownValue)
                return;
            hasRefreshUpDownValue = true;
            int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
            float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];
            upValue = KValue + HitValue;
            downValue = KValue - MissValue * missRelHeight;
        }

        public string GetInfo()
        {
            //if (info == null)
            string info = "";
            {
                int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                if (parent.startItem == parent.endItem)
                {
                    info = "[" + parent.startItem.idTag + "-" + parent.startItem.lotteryNumber + "] [" +
                    parent.parent.GetNumberIndexName() + " " +
                    GraphDataManager.S_CDT_TAG_LIST[cdtID] + "] [" +
                    HitValue + " : " + MissValue + "]\n";
                }
                else
                {
                    info = "[" + parent.startItem.idTag + "-" + parent.endItem.idTag + "] [" +
                    GraphDataManager.S_CDT_TAG_LIST[cdtID] + "] [" +
                    HitValue + " : " + MissValue + "]\n";
                }
            }
            return info;
        }

        public KData GetPrevKData()
        {
            if(index > 0)
            {
                return parent.parent.dataLst[index - 1].dataDict[cdt];
            }
            return null;
        }

        public KData GetNextKData()
        {
            if(index < parent.parent.dataLst.Count - 1)
                return parent.parent.dataLst[index + 1].dataDict[cdt];
            return null;
        }
    }

    public class KDataDict
    {
        public int index;
        public Dictionary<CollectDataType, KData> dataDict = new Dictionary<CollectDataType, KData>();
        public DataItem startItem = null;
        public DataItem endItem = null;
        public KDataDictContainer parent = null;

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

        public bool IsFitDataItem(DataItem item)
        {
            if (startItem == endItem && startItem == item)
                return true;
            return false;
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

    public class AvgPoint
    {
        public float avgKValue = 0;
    }

    public class AvgPointMap
    {
        public AvgDataContainer parent;
        public int index = -1;
        public KDataDict kdd = null;
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

    public class AvgDataContainer
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
        AvgPointMap CreateAvgPointMap(KDataDict kdd)
        {
            AvgPointMap apm = new AvgPointMap();
            apm.index = avgPointMapLst.Count;
            apm.parent = this;
            apm.kdd = kdd;
            avgPointMapLst.Add(apm);
            return apm;
        }
        void ProcessBySMA(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for ( int i = 0; i < srcData.Count; ++i )
            {
                AvgPointMap apm = CreateAvgPointMap(srcData[i]);

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
                        ap.avgKValue += kd.KValue;
                    }
                }
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    ap.avgKValue /= totalSub;
                }
            }
        }
        void ProcessByWMALastDay(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap(srcData[i]);

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
                            ap.avgKValue += kd.KValue * 2;
                        else
                            ap.avgKValue += kd.KValue;
                    }
                    totalSub += isLastDay ? 2 : 1;
                }
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    ap.avgKValue /= totalSub;
                }
            }
        }
        void ProcessByWMALinear(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap(srcData[i]);

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
                        ap.avgKValue += kd.KValue * loop;
                    }
                    totalSub += loop;
                    ++loop;
                }
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    ap.avgKValue /= totalSub;
                }
            }
        }
        void ProcessByWMASqr(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap(srcData[i]);

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
                        ap.avgKValue += kd.KValue * sqrLoop;
                    }
                    totalSub += sqrLoop;
                    ++loop;
                }
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    ap.avgKValue /= totalSub;
                }
            }
        }
        void ProcessByEMA(List<KDataDict> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap(srcData[i]);

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
                        ap.avgKValue = kd.KValue;
                    }
                    else
                    {
                        ap.avgKValue = (kd.KValue * 2 + (cycle - 1) * prevApm.apMap[cdt].avgKValue) / (cycle + 1);
                    }
                }
            }
        }
    }

    public class BollinPoint
    {
        //// 标准差
        //public float standardDeviation = 0;
        // 上轨值
        public float upValue = 0;
        // 中轨值
        public float midValue = 0;
        // 下轨值
        public float downValue = 0;
    }
    public class BollinPointMap
    {
        public BollinDataContainer parent;
        public int index = -1;
        public KDataDict kdd = null;
        public Dictionary<CollectDataType, BollinPoint> bpMap = new Dictionary<CollectDataType, BollinPoint>();

        public BollinPointMap()
        {
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
                GetData(GraphDataManager.S_CDT_LIST[i], true);
        }

        public BollinPoint GetData(CollectDataType collectDataType, bool createIfNotExist)
        {
            if (bpMap.ContainsKey(collectDataType))
                return bpMap[collectDataType];
            else if (createIfNotExist == false)
                return null;
            BollinPoint data = new BollinPoint();
            bpMap.Add(collectDataType, data);
            return data;
        }

        public BollinPointMap GetPrevBPM()
        {
            if (index > 0)
                return parent.bollinMapLst[index - 1];
            return null;
        }
    }
    public class BollinDataContainer
    {
        public static bool ENABLE = true;
        public List<BollinPointMap> bollinMapLst = new List<BollinPointMap>();

        BollinPointMap CreateBollinPointMap(KDataDict kdd)
        {
            BollinPointMap bpm = new BollinPointMap();
            bpm.index = bollinMapLst.Count;
            bpm.parent = this;
            bpm.kdd = kdd;
            bollinMapLst.Add(bpm);
            return bpm;
        }

        public void Process(List<KDataDict> srcData, AvgDataContainer avgContainer)
        {
            bollinMapLst.Clear();
            if (ENABLE == false)
            {                
                return;
            }
            for( int i = 0; i < srcData.Count; ++i )
            {
                KDataDict kdd = srcData[i];
                AvgPointMap apm = avgContainer.avgPointMapLst[i];

                BollinPointMap bpm = CreateBollinPointMap(srcData[i]);
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    KData kd = kdd.dataDict[cdt];

                    float MA = ap.avgKValue;
                    int startIndex = i - avgContainer.cycle + 1;
                    if (startIndex < 0)
                        startIndex = 0;
                    float SD = 0;
                    int N = 0;
                    for( int k = startIndex; k <= i; ++k )
                    {
                        KData ckd = srcData[k].dataDict[cdt];
                        SD += (ckd.KValue - MA) * (ckd.KValue - MA);
                        ++N;
                    }
                    SD = (float)Math.Sqrt(SD / N);

                    BollinPoint bp = bpm.GetData(cdt, true);
                    const float scale = 2;
                    //bp.standardDeviation = SD;
                    bp.midValue = MA;
                    bp.upValue = MA + scale * SD;
                    bp.downValue = MA - scale * SD;
                }
            }
        }
    }


    public class MACDLimitValue
    {
        public float MaxValue = 0;
        public float MinValue = 0;
    }
    public class MACDPoint
    {
        public float DIF = 0;
        public float DEA = 0;
        public float BAR = 0;
#if TRADE_DBG
        public byte WAVE_CFG = 0;
        public int MAX_DIF_INDEX = -1;
        public int MIN_DIF_INDEX = -1;
        public int LEFT_DIF_INDEX = -1;
        public byte BAR_CFG = 0;
        public int MAX_BAR_INDEX = -1;
        public int MIN_BAR_INDEX = -1;
        public byte KGRAPH_CFG = 0;
        public bool IS_STRONG_UP = false;
        public string LAST_DATA_TAG = "";
#endif
    }
    public class MACDPointMap
    {
        public MACDDataContianer parent;
        public int index = -1;
        public Dictionary<CollectDataType, MACDPoint> macdpMap = new Dictionary<CollectDataType, MACDPoint>();

        public MACDPointMap()
        {
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
                GetData(GraphDataManager.S_CDT_LIST[i], true);
        }
        public MACDPoint GetData(CollectDataType collectDataType, bool createIfNotExist)
        {
            if (macdpMap.ContainsKey(collectDataType))
                return macdpMap[collectDataType];
            else if (createIfNotExist == false)
                return null;
            MACDPoint data = new MACDPoint();
            macdpMap.Add(collectDataType, data);
            return data;
        }
        public MACDPointMap GetPrevMACDPM()
        {
            if (index > 0)
                return parent.macdMapLst[index - 1];
            return null;
        }
        public MACDPointMap GetNextMACDPM()
        {
            if (index < parent.macdMapLst.Count - 1)
                return parent.macdMapLst[index + 1];
            return null;
        }
    }
    public class MACDDataContianer
    {
        public static bool ENABLE = true;
        public List<MACDPointMap> macdMapLst = new List<MACDPointMap>();
        public Dictionary<CollectDataType, MACDLimitValue> macdLimitValueMap = new Dictionary<CollectDataType, MACDLimitValue>();

        public MACDDataContianer()
        {
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                macdLimitValueMap.Add(GraphDataManager.S_CDT_LIST[i], new MACDLimitValue());
            }
        }

        MACDPointMap CreateMACDPointMap()
        {
            MACDPointMap obj = new MACDPointMap();
            obj.index = macdMapLst.Count;
            obj.parent = this;
            macdMapLst.Add(obj);
            return obj;
        }

        public void Process(List<KDataDict> srcData, AvgDataContainer avgContainerShort, AvgDataContainer avgContainerLong)
        {
            int cycle = avgContainerShort.cycle * 3 / 4;
            macdMapLst.Clear();
            if (ENABLE == false)
            {
                return;
            }
            for (int i = 0; i < srcData.Count; ++i)
            {
                KDataDict kdd = srcData[i];
                AvgPointMap apmS = avgContainerShort.avgPointMapLst[i];
                AvgPointMap apmL = avgContainerLong.avgPointMapLst[i];

                MACDPointMap bpm = CreateMACDPointMap();
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {                    
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    MACDLimitValue mlv = macdLimitValueMap[cdt];
                    AvgPoint apS = apmS.apMap[cdt];
                    AvgPoint apL = apmL.apMap[cdt];

                    float DIF = apS.avgKValue - apL.avgKValue;
                    float DEA = 0;
                    float BAR = 0;
                    if (i == 0)
                        DEA = DIF;
                    else
                    {
                        MACDPointMap prevMPM = bpm.GetPrevMACDPM();
                        MACDPoint prevMP = prevMPM.GetData(cdt, false);
                        DEA = (DIF * 2 + (cycle - 1) * prevMP.DEA) / (cycle + 1);
                    }
                    BAR = 2 * (DIF - DEA);

                    MACDPoint bp = bpm.GetData(cdt, true);
                    bp.DIF = DIF;
                    bp.DEA = DEA;
                    bp.BAR = BAR;

                    mlv.MaxValue = Math.Max(mlv.MaxValue, Math.Max(BAR, Math.Max(DIF, DEA)));
                    mlv.MinValue = Math.Min(mlv.MinValue, Math.Min(BAR, Math.Min(DIF, DEA)));
                }
            }
        }

    }


    public class KDataDictContainer
    {
        public static string[] C_TAGS = new string[] { "万位", "千位", "百位", "十位", "个位", };

        // 数字序号0-4，对应万千百十个位
        public int numberIndex = -1;
        // 一条K线
        public List<KDataDict> dataLst = new List<KDataDict>();
        // 多条均线
        public Dictionary<int, AvgDataContainer> avgDataContMap = new Dictionary<int, AvgDataContainer>();
        // 一个布林指标
        public BollinDataContainer bollinDataLst = new BollinDataContainer();
        // 一个macd指标
        public MACDDataContianer macdDataLst = new MACDDataContianer();

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
            kdd.parent = this;
            dataLst.Add(kdd);
            return kdd;
        }

        public string GetNumberIndexName()
        {
            return C_TAGS[numberIndex];
        }

        public KDataDict GetKDataDict(DataItem item)
        {
            if(item != null && item.idGlobal < dataLst.Count)
            {
                KDataDict kdd = dataLst[item.idGlobal];
                if (kdd.endItem == kdd.startItem && kdd.endItem == item)
                    return kdd;
            }
            return null;
        }

        public AvgPointMap GetAvgPointMap(int avgIndex, KDataDict kdd)
        {
            if(kdd != null && avgDataContMap.ContainsKey(avgIndex))
            {
                AvgDataContainer adc = avgDataContMap[avgIndex];
                if(adc.avgPointMapLst.Count > kdd.index)
                {
                    AvgPointMap apm = adc.avgPointMapLst[kdd.index];
                    if (apm.kdd == kdd)
                        return apm;
                }
            }
            return null;
        }

        public BollinPointMap GetBollinPointMap(KDataDict kdd)
        {
            if (kdd != null && bollinDataLst.bollinMapLst.Count > kdd.index)
            {
                BollinPointMap bpm = bollinDataLst.bollinMapLst[kdd.index];
                if (bpm.kdd == kdd)
                    return bpm;
            }
            return null;
        }

        public MACDPointMap GetMacdPointMap(KDataDict kdd)
        {
            if(kdd != null && macdDataLst.macdMapLst.Count > kdd.index)
            {
                MACDPointMap mpm = macdDataLst.macdMapLst[kdd.index];
                return mpm;
            }
            return null;
        }
    }

    // 图表数据容器基类
    public class GraphDataContainerBase
    {
        public virtual int DataLength() { return 0; }
        public virtual bool HasData() { return false; }
        public virtual void CollectGraphData() { }
    }

    // K线图数据容器
    public class KGraphDataContainer : GraphDataContainerBase
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
        int cycleLength = 1;
        int bollinBandCycleLength = 20;
        int macdEMAShortCycle = 10;
        int macdEMALongCycle = 20;
        List<KDataDictContainer> allKDatas = new List<KDataDictContainer>();

        public void Clear()
        {
            allKDatas.Clear();
        }
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
            CollectBollinDatas();
            CollectMACDDatas();
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

        public void CollectBollinDatas()
        {
            for (int i = 0; i < allKDatas.Count; ++i)
            {
                KDataDictContainer kddc = allKDatas[i];
                AvgDataContainer adc = kddc.avgDataContMap[bollinBandCycleLength];
                kddc.bollinDataLst.Process(kddc.dataLst, adc);
            }
        }

        public void CollectMACDDatas()
        {
            for (int i = 0; i < allKDatas.Count; ++i)
            {
                KDataDictContainer kddc = allKDatas[i];
                AvgDataContainer adcS = kddc.avgDataContMap[macdEMAShortCycle];
                AvgDataContainer adcL = kddc.avgDataContMap[macdEMALongCycle];
                kddc.macdDataLst.Process(kddc.dataLst, adcS, adcL);
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
                    kddc.numberIndex = allKDatas.Count;
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
                //data.index = kd.index;
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
                    case CollectDataType.eBigNum:
                        if (item.bigOfEachSingle[NUM_INDEX])
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eSmallNum:
                        if (item.bigOfEachSingle[NUM_INDEX] == false)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eOddNum:
                        if (item.oddOfEachSingle[NUM_INDEX])
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eEvenNum:
                        if (item.oddOfEachSingle[NUM_INDEX] == false)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.ePrimeNum:
                        if (item.primeOfEachSingle[NUM_INDEX])
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eCompositeNum:
                        if (item.primeOfEachSingle[NUM_INDEX] == false)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum0:
                        if (item.fiveNumLst[NUM_INDEX] == 0)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum1:
                        if (item.fiveNumLst[NUM_INDEX] == 1)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum2:
                        if (item.fiveNumLst[NUM_INDEX] == 2)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum3:
                        if (item.fiveNumLst[NUM_INDEX] == 3)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum4:
                        if (item.fiveNumLst[NUM_INDEX] == 4)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum5:
                        if (item.fiveNumLst[NUM_INDEX] == 5)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum6:
                        if (item.fiveNumLst[NUM_INDEX] == 6)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum7:
                        if (item.fiveNumLst[NUM_INDEX] == 7)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum8:
                        if (item.fiveNumLst[NUM_INDEX] == 8)
                            data.HitValue++;
                        else
                            data.MissValue++;
                        break;
                    case CollectDataType.eNum9:
                        if (item.fiveNumLst[NUM_INDEX] == 9)
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
    public class BarGraphDataContianer : GraphDataContainerBase
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
        DataItem currentSelectItem = null;
        public DataItem CurrentSelectItem
        {
            get { return currentSelectItem; }
            set { currentSelectItem = value; }
        }

        public BarGraphDataContianer()
        {
            for (int i = 0; i < 5; ++i)
            {
                allDatas.Add(new DataUnitLst());
            }
        }

        public void Clear()
        {
            allDatas.Clear();
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
            if (CurrentSelectItem != null)
                currentItem = CurrentSelectItem;
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
    public class GraphDataManager
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
            AddPreInfo(CollectDataType.eBigNum, "大数", 5.0f / 10);
            AddPreInfo(CollectDataType.eSmallNum, "小数", 5.0f / 10);
            AddPreInfo(CollectDataType.eOddNum, "奇数", 5.0f / 10);
            AddPreInfo(CollectDataType.eEvenNum, "偶数", 5.0f / 10);
            AddPreInfo(CollectDataType.ePrimeNum, "质数", 5.0f / 10);
            AddPreInfo(CollectDataType.eCompositeNum, "合数", 5.0f / 10);
            AddPreInfo(CollectDataType.eNum0, "数字0", 1.0f / 10);
            AddPreInfo(CollectDataType.eNum1, "数字1", 1.0f / 10);
            AddPreInfo(CollectDataType.eNum2, "数字2", 1.0f / 10);
            AddPreInfo(CollectDataType.eNum3, "数字3", 1.0f / 10);
            AddPreInfo(CollectDataType.eNum4, "数字4", 1.0f / 10);
            AddPreInfo(CollectDataType.eNum5, "数字5", 1.0f / 10);
            AddPreInfo(CollectDataType.eNum6, "数字6", 1.0f / 10);
            AddPreInfo(CollectDataType.eNum7, "数字7", 1.0f / 10);
            AddPreInfo(CollectDataType.eNum8, "数字8", 1.0f / 10);
            AddPreInfo(CollectDataType.eNum9, "数字9", 1.0f / 10);

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
        public static float GetTheoryProbability(CollectDataType cdt)
        {
            int index = S_CDT_LIST.IndexOf(cdt);
            return S_CDT_PROBABILITY_LIST[index] * 100.0f;
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

        public void Clear()
        {
            KGDC.Clear();
            BGDC.Clear();
        }
    }

    public class AutoAnalyzeTool
    {
        public static int C_ANALYZE_LOOP_COUNT = 50;

        public enum CalcType
        {
            eUpLine,
            eDownLine,
        }

        public class AuxLineData
        {
            public bool valid = false;
            public KData dataSharp = null;
            public KData dataPrevSharp = null;
            public KData dataNextSharp = null;

            public List<KData> candictDatas = new List<KData>();

            public void Reset()
            {
                valid = false;
                dataSharp = null;
                dataPrevSharp = null;
                dataNextSharp = null;
                candictDatas.Clear();
            }
            public void CheckValid()
            {
                if (dataSharp != null && (dataPrevSharp != null || dataNextSharp != null))
                    valid = true;
                else
                    valid = false;
            }
            public void CalcSecondPoint(bool bGetSmallSecPt)
            {
                if (dataSharp == null || candictDatas.Count == 0)
                    return;
                float prevKV = 0, nextKV = 0;

                if (bGetSmallSecPt)
                {
                    for( int i = 0; i < candictDatas.Count; ++i )
                    {
                        KData testD = candictDatas[i];
                        if (testD == dataSharp || testD == null)
                            continue;
                        float k = (dataSharp.KValue - testD.KValue) / (dataSharp.index - testD.index);
                        if (testD.index < dataSharp.index)
                        {
                            if(k < prevKV || dataPrevSharp == null)
                            {
                                prevKV = k;
                                dataPrevSharp = testD;
                            }
                        }
                        else
                        {
                            if (k > nextKV || dataNextSharp == null)
                            {
                                nextKV = k;
                                dataNextSharp = testD;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < candictDatas.Count; ++i)
                    {
                        KData testD = candictDatas[i];
                        if (testD == dataSharp || testD == null)
                            continue;
                        float k = (dataSharp.KValue - testD.KValue) / (dataSharp.index - testD.index);
                        if (testD.index < dataSharp.index)
                        {
                            if (k > prevKV || dataPrevSharp == null)
                            {
                                prevKV = k;
                                dataPrevSharp = testD;
                            }
                        }
                        else
                        {
                            if (k < nextKV || dataNextSharp == null)
                            {
                                nextKV = k;
                                dataNextSharp = testD;
                            }
                        }
                    }
                }
            }
        }

        public class SingleAuxLineInfo
        {
            public AuxLineData upLineData = new AuxLineData();
            public AuxLineData downLineData = new AuxLineData();

            public void Reset()
            {
                upLineData.Reset();
                downLineData.Reset();
            }

            public void CheckData(KData kd, int endKDIndex)
            {
                if (kd.index == endKDIndex)
                    return;
                // 取得下一个K值
                KData nxtData = kd.GetNextKData();
                //KData prvData = kd.GetPrevKData();

                // 当前K值上升
                if (kd.HitValue > kd.MissValue)
                {
                    // 下一个K值是下降，表明当前K值可能是一个波峰
                    if (nxtData != null && nxtData.HitValue < nxtData.MissValue)
                    {
                        // 当前没有波峰
                        if (upLineData.dataSharp == null)
                        {
                            // 记录该K值为波峰
                            upLineData.dataSharp = kd;
                        }
                        // 如果当前K值高于波峰的K值
                        else if (kd.KValue > upLineData.dataSharp.KValue)
                        {
                            // 把当前的波峰K值放到候选列表中
                            upLineData.candictDatas.Add(upLineData.dataSharp);
                            // 设置当前K值为波峰
                            upLineData.dataSharp = kd;
                        }
                        // 把当前的K值放到候选列表中
                        else
                            upLineData.candictDatas.Add(kd);
                    }
                }
                // 当前K值下降
                else if(kd.HitValue < kd.MissValue)
                {
                    // 下一个K值上升，表明当前K值可能是一个波谷
                    if (nxtData != null && nxtData.HitValue > nxtData.MissValue)
                    {
                        // 当前没有波谷
                        if (downLineData.dataSharp == null)
                        {
                            // 记录该K值为波谷
                            downLineData.dataSharp = kd;
                        }
                        // 如果当前K值低于波谷的K值
                        else if (kd.KValue < downLineData.dataSharp.KValue)
                        {
                            // 把当前的波谷K值放到候选列表中
                            downLineData.candictDatas.Add(downLineData.dataSharp);
                            // 设置当前K值为波谷
                            downLineData.dataSharp = kd;
                        }
                        // 把当前的K值放到候选列表中
                        else
                            downLineData.candictDatas.Add(kd);
                    }
                }
            }
            public void CheckValid()
            {
                upLineData.CalcSecondPoint(true);
                downLineData.CalcSecondPoint(false);
                upLineData.CheckValid();
                downLineData.CheckValid();
            }
        }

        List<Dictionary<CollectDataType, SingleAuxLineInfo>> allAuxInfo = new List<Dictionary<CollectDataType, SingleAuxLineInfo>>();

        public SingleAuxLineInfo GetSingleAuxLineInfo(int numID, CollectDataType cdt)
        {
            return allAuxInfo[numID][cdt];
        }

        public AutoAnalyzeTool()
        {
            allAuxInfo.Clear();
            for ( int i = 0; i < 5; ++i )
            {
                Dictionary<CollectDataType, SingleAuxLineInfo> dict = new Dictionary<CollectDataType, SingleAuxLineInfo>();
                for( int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j )
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    dict.Add(cdt, new SingleAuxLineInfo());
                }
                allAuxInfo.Add(dict);
            }
        }

        void Reset()
        {
            for (int i = 0; i < allAuxInfo.Count; ++i)
            {
                Dictionary<CollectDataType, SingleAuxLineInfo> dict = allAuxInfo[i];
                foreach(SingleAuxLineInfo sali in dict.Values)
                {
                    sali.Reset();
                }
            }
        }

        void FinalCheckValid()
        {
            for (int i = 0; i < allAuxInfo.Count; ++i)
            {
                Dictionary<CollectDataType, SingleAuxLineInfo> dict = allAuxInfo[i];
                foreach (SingleAuxLineInfo sali in dict.Values)
                {
                    sali.CheckValid();
                }
            }
        }

        void ProcessCheck(int curKDataIndex, int loopCount)
        {
            // 遍历每个数字位
            for (int numID = 0; numID < 5; ++numID)
            {
                int loop = loopCount;
                int kdID = curKDataIndex;
                // 往前回溯loopCount个K值
                while (kdID >= 0 && loop >= 0)
                {
                    // 取K值映射表
                    KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numID);
                    KDataDict kdd = kddc.dataLst[kdID];
                    // 遍历每类数据的K值
                    foreach (CollectDataType cdt in kdd.dataDict.Keys)
                    {
                        KData kd = kdd.GetData(cdt, false);
                        SingleAuxLineInfo sali = GetSingleAuxLineInfo(numID, cdt);
                        sali.CheckData(kd, curKDataIndex);
                    }

                    --kdID;
                    --loop;
                }
            }
        }

        public void Analyze(int curKDataIndex)
        {
            int loopCount = C_ANALYZE_LOOP_COUNT;
            if (curKDataIndex < 2)
                return;
            Reset();
            ProcessCheck(curKDataIndex, loopCount);
            FinalCheckValid();
        }

        // 计算第dataIndex个数据在压力线或者支撑线上的K值
        public bool CalculateValue(int numIndex, int dataIndex, CollectDataType cdt, CalcType ct, bool prevLine, ref float value)
        {
            SingleAuxLineInfo sali = allAuxInfo[numIndex][cdt];
            if(ct == CalcType.eUpLine)
            {
                if (sali.upLineData.valid == false)
                    return false;
                float x1 = sali.upLineData.dataSharp.index, y1 = sali.upLineData.dataSharp.KValue, x2, y2;
                if(prevLine)
                {
                    if (sali.upLineData.dataPrevSharp == null)
                        return false;
                    x2 = sali.upLineData.dataPrevSharp.index;
                    y2 = sali.upLineData.dataPrevSharp.KValue;
                }
                else
                {
                    if (sali.upLineData.dataNextSharp == null)
                        return false;
                    x2 = sali.upLineData.dataNextSharp.index;
                    y2 = sali.upLineData.dataNextSharp.KValue;
                }
                float k = (y1 - y2) / (x1 - x2);
                float b = (x1 * y2 - y1 * x2) / (x1 - x2);
                value = k * dataIndex + b;
                return true;
            }
            else if(ct == CalcType.eDownLine)
            {
                if (sali.downLineData.valid == false)
                    return false;
                float x1 = sali.downLineData.dataSharp.index, y1 = sali.downLineData.dataSharp.KValue, x2, y2;
                if (prevLine)
                {
                    if (sali.downLineData.dataPrevSharp == null)
                        return false;
                    x2 = sali.downLineData.dataPrevSharp.index;
                    y2 = sali.downLineData.dataPrevSharp.KValue;
                }
                else
                {
                    if (sali.downLineData.dataNextSharp == null)
                        return false;
                    x2 = sali.downLineData.dataNextSharp.index;
                    y2 = sali.downLineData.dataNextSharp.KValue;
                }
                float k = (y1 - y2) / (x1 - x2);
                float b = (x1 * y2 - y1 * x2) / (x1 - x2);
                value = k * dataIndex + b;
                return true;
            }
            return false;
        }
    }

#endregion
}