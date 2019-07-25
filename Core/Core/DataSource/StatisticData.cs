using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public class StatisticData
    {
        // 统计之前出现的最大遗漏值
        public int prevMaxMissCount;
        // 统计之前出现的最大遗漏值的那期的索引值
        public int prevMaxMissCountIndex;
        // 统计遗漏面积
        public float missCountArea;
        public float missCountAreaTotal;
        // 统计出现该统计类型数据的次数
        public Byte appearCount;
        // 统计没有出现该统计类型数据的次数
        public Byte disappearCount;
        // 统计出现该统计类型数据的百分比
        public float appearProbability;
        // 统计出现该统计类型数据的百分比与理论概率的差值
        public float appearProbabilityDiffWithTheory;
        // 连续低于理论概率的个数
        public int underTheoryCount = 0;

    }
}
