using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public class OneDayDatas
    {
        public int dateID = 0;
        public List<DataItem> datas = new List<DataItem>();
        public Dictionary<string, DataItem> searchMap = new Dictionary<string, DataItem>();

#if ENABLE_GROUP_COLLECT
        public SimData simData = new SimData();
#endif

        public OneDayDatas()
        {
#if ENABLE_GROUP_COLLECT
            simData.Reset();
#endif
        }
        public void AddItem(DataItem item)
        {
            if (searchMap.ContainsKey(item.idTag) == false)
            {
                item.parent = this;
                item.idInOneDay = datas.Count;

                searchMap.Add(item.idTag, item);
                datas.Add(item);
            }
            else
            {
                throw new Exception("has contains dataitem " + item.idTag);
            }
        }
        public DataItem FindItem(string idTag)
        {
            if (searchMap.ContainsKey(idTag))
            {
                return searchMap[idTag];
            }
            return null;
        }

        public DataItem GetTailItem()
        {
            if (datas.Count > 0)
                return datas[datas.Count - 1];
            return null;
        }
        public DataItem GetFirstItem()
        {
            if (datas.Count > 0)
                return datas[0];
            return null;
        }
        public DataItem GetPrevItem(DataItem curItem)
        {
            int curID = curItem.idInOneDay - 1;
            if (curID >= 0)
                return datas[curID];
            else
            {
                OneDayDatas prevODD = DataManager.GetInst().GetPrevOneDayDatas(this);
                if (prevODD != null)
                    return prevODD.GetTailItem();
            }
            return null;
        }
        public DataItem GetNextItem(DataItem curItem)
        {
            int curID = curItem.idInOneDay + 1;
            if (curID < datas.Count)
                return datas[curID];
            else
            {
                OneDayDatas nextODD = DataManager.GetInst().GetNextOneDayDatas(this);
                if (nextODD != null)
                    return nextODD.GetFirstItem();
            }
            return null;
        }
    }

}
