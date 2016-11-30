using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public class DataItem
    {
        public int id;
        public string idTag;
        public string lotteryNumber;
        public int andValue;
        public int rearValue;
        public int crossValue;
        public int groupType;

        public DataItem()
        {
        }

        public int GetGeNumber()
        {
            int value = Util.CharValue(lotteryNumber[lotteryNumber.Length - 1]);
            return value;
        }
        public int GetShiNumber()
        {
            int value = Util.CharValue(lotteryNumber[lotteryNumber.Length - 2]);
            return value;
        }
        public int GetBaiNumber()
        {
            int value = Util.CharValue(lotteryNumber[lotteryNumber.Length - 3]);
            return value;
        }
    }

    public class OneDayDatas
    {
        public List<DataItem> datas = new List<DataItem>();

        public OneDayDatas()
        {

        }
    }

    public class DataManager
    {
        public Dictionary<int, OneDayDatas> allDatas = new Dictionary<int, OneDayDatas>();
        public List<int> indexs = new List<int>();

        public Dictionary<int, string> mFileMetaInfo = new Dictionary<int, string>();

        public DataManager()
        {
        }

        public void ClearAllDatas()
        {
            allDatas.Clear();
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
            OneDayDatas data = null;
            string fullPath = mFileMetaInfo[key];
            if (Util.ReadFile(key, fullPath, ref data))
            {
                allDatas.Add(key, data);
            }
        }
    }
}
