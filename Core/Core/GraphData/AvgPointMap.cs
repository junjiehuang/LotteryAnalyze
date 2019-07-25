using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class AvgPointMap
    {
        public AvgDataContainer parent;
        public int index = -1;
        public KDataMap kdd = null;
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
}
