//#define ENABLE_GROUP_COLLECT

#define USE_EMA_CALC

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
#region data manage

    public class DataItem
    {
        public object tag = null;

        static int[] TheoryProbabilityOfPath012 = new int[3] { 40, 30, 30, };

        public OneDayDatas parent = null;

        // 在所有数据里的索引值
        public int idGlobal = -1;
        // 在一天的数据里的索引值
        public int idInOneDay;
        // item的tag
        public string idTag;
        public string lotteryNumber;

#if ENABLE_GROUP_COLLECT
        public SByte andValue;
        public SByte rearValue;
        public SByte crossValue;
        public GroupType groupType;
        // 后三数字
        public List<SByte> valuesOfLastThree = new List<SByte>();
        public SimData simData;
#endif

        // 5个数字
        public List<SByte> fiveNumLst = new List<SByte>();
        // 每个数字的012路属性
        public List<SByte> path012OfEachSingle = new List<SByte>();
        // 每个数是否奇数
        public List<bool> oddOfEachSingle = new List<bool>();
        // 每个数是否质数
        public List<bool> primeOfEachSingle = new List<bool>();
        // 每个数是否大数
        public List<bool> bigOfEachSingle = new List<bool>();
        public LotteryStatisticInfo statisticInfo;


        public DataItem(string idStr, string numStr, int fileID)
        {
            lotteryNumber = numStr;
            idTag = fileID + "-" + idStr;

            fiveNumLst.Clear();
            fiveNumLst.Add(GetWanNumber());
            fiveNumLst.Add(GetQianNumber());
            fiveNumLst.Add(GetBaiNumber());
            fiveNumLst.Add(GetShiNumber());
            fiveNumLst.Add(GetGeNumber());
            path012OfEachSingle.Clear();
            oddOfEachSingle.Clear();
            bigOfEachSingle.Clear();
            primeOfEachSingle.Clear();
            for (int i = 0; i < fiveNumLst.Count; ++i)
            {
                int v = fiveNumLst[i];
                path012OfEachSingle.Add((SByte)(v % 3));
                oddOfEachSingle.Add((v % 2) == 1);
                bigOfEachSingle.Add(v > 4);
                if(v == 1 || v == 2 || v == 3 || v == 5 || v ==7)
                    primeOfEachSingle.Add(true);
                else
                    primeOfEachSingle.Add(false);
            }
            statisticInfo = new LotteryStatisticInfo(this);

#if ENABLE_GROUP_COLLECT
            andValue = Util.CalAndValue(lotteryNumber);
            rearValue = Util.CalRearValue(lotteryNumber);
            crossValue = Util.CalCrossValue(lotteryNumber);
            groupType = Util.GetGroupType(lotteryNumber);
            GetValuesInThreePos();
            simData = new SimData();
#endif
        }
        public SByte GetNumberByIndex(int index)
        {
            return (SByte)Util.CharValue(lotteryNumber[index]);
        }
        public SByte GetGeNumber()
        {
            int value = Util.CharValue(lotteryNumber[4]);
            return (SByte)value;
        }
        public SByte GetShiNumber()
        {
            int value = Util.CharValue(lotteryNumber[3]);
            return (SByte)value;
        }
        public SByte GetBaiNumber()
        {
            int value = Util.CharValue(lotteryNumber[2]);
            return (SByte)value;
        }
        public SByte GetQianNumber()
        {
            int value = Util.CharValue(lotteryNumber[1]);
            return (SByte)value;
        }
        public SByte GetWanNumber()
        {
            int value = Util.CharValue(lotteryNumber[0]);
            return (SByte)value;
        }

#if ENABLE_GROUP_COLLECT
        public void GetValuesInThreePos()
        {
            if (valuesOfLastThree.Count == 0)
            {
                valuesOfLastThree.Add(GetBaiNumber());
                valuesOfLastThree.Add(GetShiNumber());
                valuesOfLastThree.Add(GetGeNumber());
            }
        }
#endif
    }
    
#endregion


}
