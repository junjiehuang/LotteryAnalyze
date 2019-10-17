using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public class DataManager
    {
        public Dictionary<int, OneDayDatas> allDatas = new Dictionary<int, OneDayDatas>();
        public List<int> indexs = new List<int>();
        public Dictionary<int, string> mFileMetaInfo = new Dictionary<int, string>();
        public Dictionary<int, DataItem> allItemMap = new Dictionary<int, DataItem>();
        public List<int> fileKeys = new List<int>();

#if ENABLE_GROUP_COLLECT
        public SimData simData;
#endif
        public long curProfit = 0;

        DataManager()
        {
        }

        static DataManager sInst = null;
        public static DataManager GetInst()
        {
            if (sInst == null)
                sInst = new DataManager();
            return sInst;
        }

        public void GenFileKeys()
        {
            fileKeys.Clear();
            foreach( int key in mFileMetaInfo.Keys)
            {
                fileKeys.Add(key);
            }
        }

        public void ClearAllDatas()
        {
            allDatas.Clear();
            indexs.Clear();
            allItemMap.Clear();
        }

        public void ClearDatasExcept(OneDayDatas odd)
        {
            ClearAllDatas();
            indexs.Add(odd.dateID);
            allDatas.Add(odd.dateID, odd);
        }

        public void AddMetaInfo(int key, string fileFullName)
        {
            if (mFileMetaInfo.ContainsKey(key) == false)
                mFileMetaInfo.Add(key, fileFullName);
        }

        public void LoadAllDatas(ref List<int> selectIDs)
        {
            for (int i = 0; i < selectIDs.Count; ++i)
            {
                int key = selectIDs[i];
                LoadData(key);
            }
        }
        public void LoadData(int key)
        {
            if (allDatas.ContainsKey(key) == false)
            {
                OneDayDatas data = null;
                string fullPath = mFileMetaInfo[key];
                if (Util.ReadFile(key, fullPath, ref data))
                {
                    allDatas.Add(key, data);
                    if (indexs.IndexOf(key) == -1)
                        indexs.Add(key);
                    indexs.Sort();
                }
            }
        }
        public bool LoadDataExt(int key, ref OneDayDatas newODD, ref int newIndex)
        {
            newODD = null;
            newIndex = -1;

            OneDayDatas data = null;
            if (allDatas.ContainsKey(key))
                data = allDatas[key];
            if (data == null)
            {
                LoadData(key);
                if (allDatas.ContainsKey(key))
                {
                    newODD = allDatas[key];
                    newIndex = 0;
                    return true;
                }
            }
            else
            {
                string fullPath = mFileMetaInfo[key];
                if (Util.ReadFile(key, fullPath, ref data, ref newIndex))
                {
                    newODD = data;
                    return true;
                }
            }
            return false;
        }
        public void SetDataItemsGlobalID()
        {
            allItemMap.Clear();
            int curID = 0;
            //foreach (int key in allDatas.Keys)
            for (int id = 0; id < indexs.Count; ++id)
            {
                int key = indexs[id];
                OneDayDatas data = allDatas[key];
                for (int i = 0; i < data.datas.Count; ++i)
                {
                    DataItem di = data.datas[i];
                    di.idGlobal = curID++;
                    allItemMap.Add(di.idGlobal, di);
                }
            }
        }

        public DataItem FindDataItem(int globalID)
        {
            if (allItemMap.ContainsKey(globalID))
                return allItemMap[globalID];
            return null;
        }

        public OneDayDatas GetFirstOneDayDatas()
        {
            if (indexs.Count > 0)
            {
                int firstOddID = indexs[0];
                return allDatas[firstOddID];
            }
            return null;
        }

        /// <summary>
        /// 获取上一日的开奖数据列表
        /// </summary>
        /// <param name="curData"></param>
        /// <returns></returns>
        public OneDayDatas GetPrevOneDayDatas(OneDayDatas curData)
        {
            int index = indexs.IndexOf(curData.dateID);
            if (index > 0)
            {
                --index;
                int newDateID = indexs[index];
                return allDatas[newDateID];
            }
            return null;
        }
        /// <summary>
        /// 获取下一日的开奖数据列表
        /// </summary>
        /// <param name="curData"></param>
        /// <returns></returns>
        public OneDayDatas GetNextOneDayDatas(OneDayDatas curData)
        {
            int index = indexs.IndexOf(curData.dateID);
            if (index < indexs.Count - 1)
            {
                ++index;
                int newDateID = indexs[index];
                return allDatas[newDateID];
            }
            return null;
        }
        /// <summary>
        /// 获取上一期的开奖数据
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public DataItem GetPrevItem(DataItem curItem)
        {
            DataItem prevItem = curItem.parent.GetPrevItem(curItem);
            if (prevItem != null)
                return prevItem;
            OneDayDatas prevODD = GetPrevOneDayDatas(curItem.parent);
            if (prevODD != null)
                return prevODD.GetTailItem();
            return null;
        }
        /// <summary>
        /// 获取下一期的开奖数据
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public DataItem GetNextItem(DataItem curItem)
        {
            DataItem nextItem = curItem.parent.GetNextItem(curItem);
            if (nextItem != null)
                return nextItem;
            OneDayDatas nextODD = GetNextOneDayDatas(curItem.parent);
            if (nextODD != null)
                return nextODD.GetFirstItem();
            return null;
        }
        /// <summary>
        /// 获取最新的开奖数据
        /// </summary>
        /// <returns></returns>
        public DataItem GetLatestItem()
        {
            if (allDatas.Count > 0)
            {
                int lastIndex = indexs[indexs.Count - 1];
                if (allDatas.ContainsKey(lastIndex))
                {
                    OneDayDatas odd = allDatas[lastIndex];
                    if (odd.datas.Count > 0)
                        return odd.datas[odd.datas.Count - 1];
                    else if (indexs.Count > 1)
                    {
                        lastIndex = indexs[indexs.Count - 2];
                        odd = allDatas[lastIndex];
                        if (odd.datas.Count > 0)
                            return odd.datas[odd.datas.Count - 1];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取第一个开奖数据
        /// </summary>
        /// <returns></returns>
        public DataItem GetFirstItem()
        {
            if (allDatas.Count > 0)
            {
                int firstDayIndex = indexs[0];
                if (allDatas.ContainsKey(firstDayIndex))
                {
                    OneDayDatas odd = allDatas[firstDayIndex];
                    if (odd.datas.Count > 0)
                        return odd.datas[0];
                }
            }
            return null;
        }

        public DataItem GetDataItemByIdTag(string idTag)
        {
            if (string.IsNullOrEmpty(idTag))
                return null;
            DataItem item = null;
            string[] strs = idTag.Split('-');
            int key = int.Parse(strs[0]);
            if (allDatas.ContainsKey(key))
            {
                OneDayDatas odd = allDatas[key];
                item = odd.FindItem(idTag);
            }
            return item;
        }

        public int GetAllDataItemCount()
        {
            int count = 0;
            foreach (OneDayDatas odd in allDatas.Values)
            {
                count += odd.datas.Count;
            }
            return count;
        }
    }

}
