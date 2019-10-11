using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class KData
    {
        public CollectDataType cdt;
        public KDataMap parent;

        public float HitValue;
        public float MissValue;
        public float KValue
        {
            get
            {
                return kvalue;
            }
            set
            {
                kvalue = value;
                //RefreshUpDownValue();
            }
        }

        //string info = null;
        //bool hasRefreshUpDownValue = false;
        float upValue = 0;
        float downValue = 0;
        float kvalue = 0;
        float startValue = 0;
        float endValue = 0;

        public float StartValue
        {
            get { return startValue; }
            set { startValue = value; }
        }

        public float EndValue
        {
            get { return endValue; }
            set { endValue = value; }
        }

        public float UpValue
        {
            get { return upValue; }
            set { upValue = value; }
        }

        public float DownValue
        {
            get { return downValue; }
            set { downValue = value; }
        }


        // 测试点与K值的相对距离
        // > 0, 测试点在K线的上方
        // = 0，测试点在K线内
        // < 0，测试点在K线的下方
        public float RelateDistTo(float testValue)
        {
            float ret = 0;
            //float KDIST = UpValue - DownValue;
            if (testValue >= DownValue && testValue <= UpValue)
            {
                ret = 0;
            }
            else if (testValue > UpValue)
            {
                //if(KDIST != 0)
                //    return (testValue - UpValue) / KDIST;
                ret = testValue - UpValue;
            }
            else
            {
                //if (KDIST != 0)
                //    return (testValue - DownValue) / KDIST;
                ret = testValue - DownValue;
            }
            return ret;
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

        //void RefreshUpDownValue()
        //{
        //    int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
        //    float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];
        //    float missV = MissValue * missRelHeight;
        //    if (HitValue > MissValue)
        //    {
        //        upValue = kvalue;
        //        downValue = kvalue - HitValue;
        //    }
        //    else
        //    {
        //        upValue = kvalue + missV;
        //        downValue = kvalue;
        //    }
        //}

        public string GetInfo()
        {
            //if (info == null)
            string info = "";
            {
                int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                if (parent.startItem == parent.endItem)
                {
                    info = parent.startItem.idGlobal + " [" + parent.startItem.idTag + "-" + parent.startItem.lotteryNumber + "] [" +
                    parent.parent.GetNumberIndexName() + " " +
                    GraphDataManager.S_CDT_TAG_LIST[cdtID] + "] [" +
                    HitValue + " : " + MissValue + "]\n";
                }
                else
                {
                    info = parent.startItem.idGlobal + " [" + parent.startItem.idTag + "-" + parent.endItem.idTag + "] [" +
                    GraphDataManager.S_CDT_TAG_LIST[cdtID] + "] [" +
                    HitValue + " : " + MissValue + "]\n";
                }
            }
            return info;
        }

        public KData GetPrevKData()
        {
            if (index > 0)
            {
                return parent.parent.dataLst[index - 1].dataDict[cdt];
            }
            return null;
        }

        public KData GetNextKData()
        {
            if (index < parent.parent.dataLst.Count - 1)
                return parent.parent.dataLst[index + 1].dataDict[cdt];
            return null;
        }
    }
}
