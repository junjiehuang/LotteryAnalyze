using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

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
            data.parent = this;
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
}
