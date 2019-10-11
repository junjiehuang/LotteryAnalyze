using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class KDataDictContainer
    {
        public static string[] C_TAGS = new string[] { "万位", "千位", "百位", "十位", "个位", };

        // 数字序号0-4，对应万千百十个位
        public int numberIndex = -1;
        // 一条K线
        public List<KDataMap> dataLst = new List<KDataMap>();
        // 多条均线
        public Dictionary<int, AvgDataContainer> avgDataContMap = new Dictionary<int, AvgDataContainer>();
        // 一个布林指标
        public BollinDataContainer bollinDataLst = new BollinDataContainer();
        // 一个macd指标
        public MACDDataContianer macdDataLst = new MACDDataContianer();

        public KDataDictContainer()
        {
            for (int i = 0; i < GraphDataContainerKGraph.S_AVG_LINE_SETTINGS.Count; ++i)
            {
                GraphDataContainerKGraph.AvgLineSetting als = GraphDataContainerKGraph.S_AVG_LINE_SETTINGS[i];
                CreateAvgDataContainer(als);
            }
        }

        void CreateAvgDataContainer(GraphDataContainerKGraph.AvgLineSetting als)
        {
            AvgDataContainer adc = new AvgDataContainer();
            adc.avgLineSetting = als;
            adc.cycle = als.cycle;
            adc.enable = als.enable;
            avgDataContMap.Add(als.cycle, adc);
        }

        public KDataMap CreateKDataDict()
        {
            KDataMap kdd = new KDataMap();
            kdd.index = dataLst.Count;
            kdd.parent = this;
            dataLst.Add(kdd);
            return kdd;
        }

        public string GetNumberIndexName()
        {
            return C_TAGS[numberIndex];
        }

        public KDataMap GetKDataDict(DataItem item)
        {
            if (item != null && item.idGlobal < dataLst.Count)
            {
                KDataMap kdd = dataLst[item.idGlobal];
                if (kdd.endItem == kdd.startItem && kdd.endItem == item)
                    return kdd;
            }
            return null;
        }

        public KDataMap GetKDataDict(int id)
        {
            if (id >= 0 && id < dataLst.Count)
            {
                return dataLst[id];
            }
            return null;
        }

        public AvgPointMap GetAvgPointMap(int avgIndex, KDataMap kdd)
        {
            if (kdd != null && avgDataContMap.ContainsKey(avgIndex))
            {
                AvgDataContainer adc = avgDataContMap[avgIndex];
                if (adc.avgPointMapLst.Count > kdd.index)
                {
                    AvgPointMap apm = adc.avgPointMapLst[kdd.index];
                    if (apm.kdd == kdd)
                        return apm;
                }
            }
            return null;
        }

        public BollinPointMap GetBollinPointMap(KDataMap kdd)
        {
            if (kdd != null && bollinDataLst.bollinMapLst.Count > kdd.index)
            {
                BollinPointMap bpm = bollinDataLst.bollinMapLst[kdd.index];
                if (bpm.kdd == kdd)
                    return bpm;
            }
            return null;
        }

        public BollinPointMap GetBollinPointMap(int index)
        {
            if (index >= 0 && index < bollinDataLst.bollinMapLst.Count)
                return bollinDataLst.bollinMapLst[index];
            return null;
        }

        public MACDPointMap GetMacdPointMap(KDataMap kdd)
        {
            if (kdd != null && macdDataLst.macdMapLst.Count > kdd.index)
            {
                MACDPointMap mpm = macdDataLst.macdMapLst[kdd.index];
                return mpm;
            }
            return null;
        }

        public MACDPointMap GetMacdPointMap(int index)
        {
            if (index >= 0 && index < macdDataLst.macdMapLst.Count)
                return macdDataLst.macdMapLst[index];
            return null;
        }
    }
}
