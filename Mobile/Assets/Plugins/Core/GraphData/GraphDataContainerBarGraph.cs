using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    // 柱状图数据容器
    public class GraphDataContainerBarGraph : GraphDataContainerBase
    {
        // 统计类型
        public enum StatisticsType
        {
            // 0-9出现次数统计
            eAppearCountFrom0To9,
            // 012路出现次数统计
            eAppearCountPath012,
        }

        // 统计范围
        public enum StatisticsRange
        {
            e3,
            e5,
            // 最近10期
            e10,
            // 最近20期
            e20,
            // 最近30期
            e30,
            // 最近50期
            e50,
            // 最近100期
            e100,
            // 自定义期数
            eCustom,
        }
        public static List<string> S_StatisticsType_STRS = new List<string>();
        public static List<string> S_StatisticsRange_STRS = new List<string>();
        static GraphDataContainerBarGraph()
        {
            S_StatisticsType_STRS.Add("0-9出现次数");
            S_StatisticsType_STRS.Add("012路出现次数");

            S_StatisticsRange_STRS.Add("最近3期");
            S_StatisticsRange_STRS.Add("最近5期");
            S_StatisticsRange_STRS.Add("最近10期");
            S_StatisticsRange_STRS.Add("最近20期");
            S_StatisticsRange_STRS.Add("最近30期");
            S_StatisticsRange_STRS.Add("最近50期");
            S_StatisticsRange_STRS.Add("最近100期");
            S_StatisticsRange_STRS.Add("自定义期数");
        }

        public class DataUnit
        {
            public StatisticsType type;
            public int data;
            public string tag;
            public float rate;
            public float relRate;
        }
        public class DataUnitLst
        {
            public List<DataUnit> dataLst = new List<DataUnit>();
        }
        public StatisticsType curStatisticsType = StatisticsType.eAppearCountFrom0To9;
        public StatisticsRange curStatisticsRange = StatisticsRange.e3;
        public int customStatisticsRange = 120;
        public List<DataUnitLst> allDatas = new List<DataUnitLst>();
        public int totalCollectCount = 0;
        DataItem currentSelectItem = null;
        public DataItem CurrentSelectItem
        {
            get { return currentSelectItem; }
            set { currentSelectItem = value; }
        }

        public int StatisticRangeCount
        {
            get
            {
                int CollectCount = customStatisticsRange;
                switch (curStatisticsRange)
                {
                    case StatisticsRange.e3: CollectCount = 3; break;
                    case StatisticsRange.e5: CollectCount = 5; break;
                    case StatisticsRange.e10: CollectCount = 10; break;
                    case StatisticsRange.e20: CollectCount = 20; break;
                    case StatisticsRange.e30: CollectCount = 30; break;
                    case StatisticsRange.e50: CollectCount = 50; break;
                    case StatisticsRange.e100: CollectCount = 100; break;
                }
                return CollectCount;
            }
        }

        public GraphDataContainerBarGraph()
        {
            for (int i = 0; i < 5; ++i)
            {
                allDatas.Add(new DataUnitLst());
            }
        }

        public void Clear()
        {
            allDatas.Clear();
        }
        void Init()
        {
            for (int c = 0; c < 5; ++c)
            {
                allDatas[c].dataLst.Clear();
                switch (curStatisticsType)
                {
                    case StatisticsType.eAppearCountFrom0To9:
                        {
                            for (int i = 0; i <= 9; ++i)
                            {
                                DataUnit du = new DataUnit();
                                du.data = 0;
                                du.type = StatisticsType.eAppearCountFrom0To9;
                                du.tag = i.ToString();
                                allDatas[c].dataLst.Add(du);
                            }
                        }
                        break;
                    case StatisticsType.eAppearCountPath012:
                        {
                            for (int i = 0; i <= 2; ++i)
                            {
                                DataUnit du = new DataUnit();
                                du.data = 0;
                                du.type = StatisticsType.eAppearCountPath012;
                                du.tag = i.ToString();
                                allDatas[c].dataLst.Add(du);
                            }
                        }
                        break;
                }
            }
        }
        void CollectItem(DataItem item)
        {
            for (int i = 0; i < 5; ++i)
            {
                switch (curStatisticsType)
                {
                    case StatisticsType.eAppearCountFrom0To9:
                        {
                            int num = item.GetNumberByIndex(i);
                            DataUnit du = allDatas[i].dataLst[num];
                            du.data = du.data + 1;
                        }
                        break;
                    case StatisticsType.eAppearCountPath012:
                        {
                            int num = item.path012OfEachSingle[i];
                            DataUnit du = allDatas[i].dataLst[num];
                            du.data = du.data + 1;
                        }
                        break;
                }
            }
        }

        public override int DataLength() { return allDatas[0].dataLst.Count; }
        public override bool HasData() { return allDatas[0].dataLst.Count > 0; }
        public override void CollectGraphData()
        {
            totalCollectCount = 0;
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            Init();

            int CollectCount = StatisticRangeCount;
            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (CurrentSelectItem != null)
                currentItem = CurrentSelectItem;
            if (currentItem == null)
                return;
            for (int i = 0; i < CollectCount; ++i)
            {
                CollectItem(currentItem);
                ++totalCollectCount;
                currentItem = DataManager.GetInst().GetPrevItem(currentItem);
                if (currentItem == null)
                    break;
            }

            for (int i = 0; i < 5; ++i)
            {
                switch (curStatisticsType)
                {
                    case StatisticsType.eAppearCountFrom0To9:
                        for(int j = 0; j < 10; ++j)
                        {
                            DataUnit du = allDatas[i].dataLst[j];
                            du.rate = ((float)(du.data) / (float)totalCollectCount);
                            du.relRate = (du.rate - 0.1f) / 0.1f;
                        }
                        break;
                    case StatisticsType.eAppearCountPath012:
                        for(int j = 0; j < 3; ++j)
                        {
                            DataUnit du = allDatas[i].dataLst[j];
                            du.rate = ((float)(du.data) / (float)totalCollectCount);
                            if(j== 0)
                                du.relRate = (du.rate - 0.4f) / 0.4f;
                            else
                                du.relRate = (du.rate - 0.3f) / 0.3f;
                        }
                        break;
                }
               
            }
        }
    }
}
