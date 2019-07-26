using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class NumberCmpInfo
    {
        public SByte number;
        public float rate;
        public bool largerThanTheoryProbability;
        public int appearCount;

        public string ToString()
        {
            return number + "(" + rate.ToString("f2") + "%) ";
        }

        public static int FindIndex(List<NumberCmpInfo> nums, SByte number, bool createIfNotExist)
        {
            for (int i = 0; i < nums.Count; ++i)
            {
                if (nums[i].number == number)
                    return i;
            }
            if (createIfNotExist)
            {
                NumberCmpInfo info = new NumberCmpInfo();
                info.appearCount = 0;
                info.number = number;
                nums.Add(info);
                return nums.Count - 1;
            }
            return -1;
        }
        public static int SortByAppearCount(NumberCmpInfo a, NumberCmpInfo b)
        {
            if (a == null || b == null)
                return 0;
            if (a == b)
                return 0;
            if (a.appearCount < b.appearCount)
                return 1;
            return -1;
        }
        public static int SortByNumber(NumberCmpInfo a, NumberCmpInfo b)
        {
            if (a == null || b == null)
                return 0;
            if (a == b)
                return 0;
            if (a.number > b.number)
                return 1;
            return -1;
        }
        public static int SortByRate(NumberCmpInfo a, NumberCmpInfo b)
        {
            if (a == null || b == null)
                return 0;
            if (a == b)
                return 0;
            if (a.rate > b.rate)
                return -1;
            if (a.rate < b.rate)
                return 1;
            if (a.appearCount > b.appearCount)
                return -1;
            if (a.appearCount < b.appearCount)
                return 1;
            return 0;
        }
    }
}
