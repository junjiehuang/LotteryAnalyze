using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class AvgDataContainer
    {
        public GraphDataContainerKGraph.AvgLineSetting avgLineSetting = null;
        public bool enable = false;
        public int cycle = 5;
        public List<AvgPointMap> avgPointMapLst = new List<AvgPointMap>();

        public void Process(List<KDataMap> srcData)
        {
            if (enable == false)
            {
                avgPointMapLst.Clear();
                return;
            }
            switch (GraphDataContainerKGraph.S_AVG_ALGORITHM)
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
        AvgPointMap CreateAvgPointMap(KDataMap kdd)
        {
            AvgPointMap apm = new AvgPointMap();
            apm.index = avgPointMapLst.Count;
            apm.parent = this;
            apm.kdd = kdd;
            avgPointMapLst.Add(apm);
            return apm;
        }
        void ProcessBySMA(List<KDataMap> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap(srcData[i]);

                int startIndex = i - cycle + 1;
                if (startIndex < 0)
                    startIndex = 0;
                int totalSub = i - startIndex + 1;
                for (; startIndex <= i; ++startIndex)
                {
                    KDataMap kdd = srcData[startIndex];
                    for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
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
        void ProcessByWMALastDay(List<KDataMap> srcData)
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
                    KDataMap kdd = srcData[startIndex];
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
        void ProcessByWMALinear(List<KDataMap> srcData)
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
                    KDataMap kdd = srcData[startIndex];
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
        void ProcessByWMASqr(List<KDataMap> srcData)
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
                    KDataMap kdd = srcData[startIndex];
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
        void ProcessByEMA(List<KDataMap> srcData)
        {
            avgPointMapLst.Clear();
            for (int i = 0; i < srcData.Count; ++i)
            {
                AvgPointMap apm = CreateAvgPointMap(srcData[i]);

                KDataMap kdd = srcData[i];
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
}
