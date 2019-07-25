using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class BollinPointMap
    {
        public BollinDataContainer parent;
        public int index = -1;
        public KDataMap kdd = null;
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
            data.parent = this;
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
}
