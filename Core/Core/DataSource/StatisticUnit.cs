using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    // 针对指定类型的统计信息
    public class StatisticUnit
    {
        // 统计类型
        public CollectDataType cdt = CollectDataType.eNone;
        // 针对该统计类型的遗漏值
        public Byte missCount = 0;
        // 连续开出的期数
        public Byte appearCount = 0;
        public StatisticData fastData = new StatisticData();
        public StatisticData shortData = new StatisticData();
        public StatisticData longData = new StatisticData();
    }
}
