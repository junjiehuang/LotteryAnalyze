using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    // K线图数据容器
    public class GraphDataContainerKGraph : GraphDataContainerBase
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
        static GraphDataContainerKGraph()
        {
            AddAvgLineSetting("5", 5, Color.Red, true);
            AddAvgLineSetting("10", 10, Color.Pink, true);
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
            als.pen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, col, 2);
            S_AVG_LINE_SETTINGS.Add(als);
        }
        #endregion

        int dataLength = 0;
        int cycleLength = 5;
        int bollinBandCycleLength = 20;
        int macdEMAShortCycle = 5;//10;
        int macdEMALongCycle = 10;//20;
        List<KDataDictContainer> allKDatas = new List<KDataDictContainer>();

        List<Dictionary<CollectDataType, float>> G_CUR_KVALUE_MAP = new List<Dictionary<CollectDataType, float>>();
        public void ResetCurKValueMap()
        {
            for (int i = 0; i < G_CUR_KVALUE_MAP.Count; ++i)
            {
                Dictionary<CollectDataType, float> dct = G_CUR_KVALUE_MAP[i];
                if (dct != null)
                {
                    for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                    {
                        dct[GraphDataManager.S_CDT_LIST[t]] = 0;
                    }
                }
            }
        }

        public GraphDataContainerKGraph(int _circle)
        {
            cycleLength = _circle;
        }

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

        public override void CollectGraphDataExcept(OneDayDatas odd)
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
                    List<KDataMap> lst = new List<KDataMap>();
                    for (int j = 0; j < kddc.dataLst.Count; ++j)
                    {
                        if (kddc.dataLst[j].startItem.parent == odd)
                        {
                            kddc.dataLst[j].index = lst.Count;
                            lst.Add(kddc.dataLst[j]);
                        }
                    }
                    kddc.dataLst.Clear();
                    kddc.dataLst.AddRange(lst);
                }
            }

            CollectKDatas(odd);
            CollectAvgDatas();
            CollectBollinDatas();
            CollectMACDDatas();
        }

        public void CollectKDatas(OneDayDatas exceptODD = null)
        {
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            int loop = 0;
            KDataMap[] curDatas = null;
            for (int i = 0; i < count; ++i)
            {
                int oneDayID = DataManager.GetInst().indexs[i];
                OneDayDatas odd = DataManager.GetInst().allDatas[oneDayID];
                if (odd == exceptODD)
                    continue;

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

            for (int i = 0; i < allKDatas.Count; ++i)
            {
                Dictionary<CollectDataType, float> valueMap = null;
                if (G_CUR_KVALUE_MAP.Count <= i)
                {
                    valueMap = new Dictionary<CollectDataType, float>();
                    G_CUR_KVALUE_MAP.Add(valueMap);
                }
                else
                {
                    valueMap = G_CUR_KVALUE_MAP[i];
                }

                KDataDictContainer kddc = allKDatas[i];
                for (int j = 0; j < kddc.dataLst.Count; ++j)
                {
                    KDataMap kdd = kddc.dataLst[j];
                    foreach (CollectDataType cdt in kdd.dataDict.Keys)
                    {
                        float lastValue = 0;
                        KData kd = kdd.dataDict[cdt];
                        //int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                        //float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];
                        //float valueChange = kd.HitValue - kd.MissValue * missRelHeight;
                        float valueChange = kd.EndValue - kd.StartValue;
                        //if (valueMap.ContainsKey(cdt))
                        //{
                        //    lastValue = valueMap[cdt];
                        //    lastValue += valueChange;
                        //    valueMap[cdt] = lastValue;
                        //}
                        //else
                        //{
                        //    lastValue = valueChange;
                        //    valueMap.Add(cdt, lastValue);
                        //}
                        if (valueMap.ContainsKey(cdt))
                            lastValue = valueMap[cdt];
                        kd.KValue += lastValue;
                        kd.UpValue += lastValue;
                        kd.DownValue += lastValue;
                        kd.StartValue += lastValue;
                        kd.EndValue += lastValue;

                        lastValue += valueChange;
                        valueMap[cdt] = lastValue;
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
                if (kddc.avgDataContMap.ContainsKey(cycle))
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

        void RecordValue(bool hit, KData data, float missHeight)
        {
            if (hit)
            {
                data.HitValue++;
                data.KValue += 1;
                data.UpValue += 1;
                data.EndValue += 1;
            }
            else
            {
                data.MissValue++;
                data.KValue -= missHeight;
                data.DownValue -= missHeight;
                data.EndValue -= missHeight;
            }
        }

        void CollectItem(int NUM_INDEX, DataItem item, KDataMap kd)
        {
            if (kd.startItem == null)
                kd.startItem = item;
            if (kd.endItem == null || item.idGlobal > kd.endItem.idGlobal)
                kd.endItem = item;
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
                KData data = kd.GetData(cdt, true);
                float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[i];
                switch (cdt)
                {
                    case CollectDataType.ePath0:
                        RecordValue((item.path012OfEachSingle[NUM_INDEX] == 0), data, missRelHeight);
                        break;
                    case CollectDataType.ePath1:
                        RecordValue((item.path012OfEachSingle[NUM_INDEX] == 1), data, missRelHeight);
                        break;
                    case CollectDataType.ePath2:
                        RecordValue((item.path012OfEachSingle[NUM_INDEX] == 2), data, missRelHeight);
                        break;
                    case CollectDataType.eBigNum:
                        RecordValue((item.bigOfEachSingle[NUM_INDEX]), data, missRelHeight);
                        break;
                    case CollectDataType.eSmallNum:
                        RecordValue((item.bigOfEachSingle[NUM_INDEX] == false), data, missRelHeight);
                        break;
                    case CollectDataType.eOddNum:
                        RecordValue((item.oddOfEachSingle[NUM_INDEX]), data, missRelHeight);
                        break;
                    case CollectDataType.eEvenNum:
                        RecordValue((item.oddOfEachSingle[NUM_INDEX] == false), data, missRelHeight);
                        break;
                    case CollectDataType.ePrimeNum:
                        RecordValue((item.primeOfEachSingle[NUM_INDEX]), data, missRelHeight);
                        break;
                    case CollectDataType.eCompositeNum:
                        RecordValue((item.primeOfEachSingle[NUM_INDEX] == false), data, missRelHeight);
                        break;
                    case CollectDataType.eNum0:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 0), data, missRelHeight);
                        break;
                    case CollectDataType.eNum1:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 1), data, missRelHeight);
                        break;
                    case CollectDataType.eNum2:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 2), data, missRelHeight);
                        break;
                    case CollectDataType.eNum3:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 3), data, missRelHeight);
                        break;
                    case CollectDataType.eNum4:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 4), data, missRelHeight);
                        break;
                    case CollectDataType.eNum5:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 5), data, missRelHeight);
                        break;
                    case CollectDataType.eNum6:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 6), data, missRelHeight);
                        break;
                    case CollectDataType.eNum7:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 7), data, missRelHeight);
                        break;
                    case CollectDataType.eNum8:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 8), data, missRelHeight);
                        break;
                    case CollectDataType.eNum9:
                        RecordValue((item.fiveNumLst[NUM_INDEX] == 9), data, missRelHeight);
                        break;
                }
            }
        }
        KDataMap[] CreateKDataDicts()
        {
            KDataMap[] curDatas = new KDataMap[5];
            for (int i = 0; i < 5; ++i)
            {
                curDatas[i] = allKDatas[i].CreateKDataDict();
            }
            return curDatas;
        }
    }
}
