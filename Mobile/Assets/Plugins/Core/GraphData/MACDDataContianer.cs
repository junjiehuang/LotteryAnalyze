using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

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

        public void Process(List<KDataMap> srcData, AvgDataContainer avgContainerShort, AvgDataContainer avgContainerLong)
        {
            int cycle = avgContainerShort.cycle * 3 / 4;
            macdMapLst.Clear();
            if (ENABLE == false)
            {
                return;
            }
            for (int i = 0; i < srcData.Count; ++i)
            {
                KDataMap kdd = srcData[i];
                AvgPointMap apmS = avgContainerShort.avgPointMapLst[i];
                AvgPointMap apmL = avgContainerLong.avgPointMapLst[i];

                MACDPointMap bpm = CreateMACDPointMap();
                bpm.isReal = kdd.startItem.isReal;

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
}
