using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public class DataItem
    {
        public int id;
        public string lotteryNumber;

        public DataItem()
        {

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

        public DataManager()
        {
        }

        public void ClearAllDatas()
        {
            allDatas.Clear();
        }

        public void LoadAllDatas(ref List<string> fileNames, ref List<string> dataPaths)
        {
            for (int i = 0; i < fileNames.Count; ++i)
            {
                OneDayDatas data = null;
                string fullPath = dataPaths[i] + fileNames[i] + ".txt";
                int dateID = int.Parse(fileNames[i]);
                if (Util.ReadFile(fullPath, ref data))
                {
                    allDatas.Add(dateID, data);
                }
            }
        }
    }
}
