using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class KDataMap
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
}
