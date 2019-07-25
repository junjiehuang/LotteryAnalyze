using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    // 记录某期开奖号的所有统计信息
    public class LotteryStatisticInfo
    {
        bool hasCollect = false;

        public const int LONG_COUNT = 30;
        public const int SHOR_COUNT = 10;
        public const int FAST_COUNT = 5;
        public int validShortCount = 0;
        public int validLongCount = 0;
        public int validFastCount = 0;
        public DataItem lotteryData;
        public List<StatisticUnitMap> allStatisticInfo = new List<StatisticUnitMap>();

        public LotteryStatisticInfo(DataItem item)
        {
            lotteryData = item;
            for (int i = 0; i < 5; ++i)
            {
                StatisticUnitMap sum = new StatisticUnitMap();
                sum.parent = this;
                allStatisticInfo.Add(sum);
            }
        }

        public void Collect()
        {
            if (hasCollect)
                return;
            if (lotteryData == null)
                return;
            DataItem prevItem = lotteryData.parent.GetPrevItem(lotteryData);
            for (int i = 0; i < 5; ++i)
            {
                allStatisticInfo[i].CollectMissCount(prevItem, i);
            }
            DataItem curItem = lotteryData;
            validLongCount = 0;
            while (curItem != null && validLongCount < LONG_COUNT)
            {
                for (int i = 0; i < 5; ++i)
                {
                    allStatisticInfo[i].CollectCount(curItem, i, validLongCount < FAST_COUNT, validLongCount < SHOR_COUNT, validLongCount < LONG_COUNT);
                }
                curItem = curItem.parent.GetPrevItem(curItem);
                ++validLongCount;
            }

            validShortCount = validLongCount;
            if (validShortCount > SHOR_COUNT)
                validShortCount = SHOR_COUNT;

            validFastCount = validLongCount;
            if (validFastCount > FAST_COUNT)
                validFastCount = FAST_COUNT;

            for (int i = 0; i < 5; ++i)
            {
                allStatisticInfo[i].CollectProbability();
            }

            CollectMissCountArea();

            hasCollect = true;
        }

        void CollectMissCountArea()
        {
            DataItem prevItem = lotteryData.parent.GetPrevItem(lotteryData);
            for (int i = 0; i < 5; ++i)
            {
                StatisticUnitMap sumME = allStatisticInfo[i];
                StatisticUnitMap sumPre = null;
                if (prevItem != null)
                {
                    sumPre = prevItem.statisticInfo.allStatisticInfo[i];
                }
                for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    StatisticUnit su = sumME.statisticUnitMap[cdt];
                    StatisticUnit suPre = null;

                    if (sumPre != null)
                    {
                        suPre = sumPre.statisticUnitMap[cdt];
                    }
                    if (su.fastData.appearProbabilityDiffWithTheory <= 0)
                        su.fastData.underTheoryCount = sumPre == null ? 1 : (suPre.fastData.underTheoryCount + 1);
                    else
                        su.fastData.underTheoryCount = 0;
                    if (su.shortData.appearProbabilityDiffWithTheory <= 0)
                        su.shortData.underTheoryCount = sumPre == null ? 1 : (suPre.shortData.underTheoryCount + 1);
                    else
                        su.shortData.underTheoryCount = 0;
                    if (su.longData.appearProbabilityDiffWithTheory <= 0)
                        su.longData.underTheoryCount = sumPre == null ? 1 : (suPre.longData.underTheoryCount + 1);
                    else
                        su.longData.underTheoryCount = 0;

                    su.fastData.prevMaxMissCount = su.missCount;
                    su.shortData.prevMaxMissCount = su.missCount;
                    su.longData.prevMaxMissCount = su.missCount;
                    su.fastData.prevMaxMissCountIndex = lotteryData.idGlobal;
                    su.shortData.prevMaxMissCountIndex = lotteryData.idGlobal;
                    su.longData.prevMaxMissCountIndex = lotteryData.idGlobal;
                }
            }

            int loopCount = 0;
            DataItem cItem = lotteryData;
            while (cItem != null)
            {
                DataItem pItem = cItem.parent.GetPrevItem(cItem);
                if (pItem == null)
                    break;
                for (int i = 0; i < 5; ++i)
                {
                    StatisticUnitMap sumME = allStatisticInfo[i];
                    StatisticUnitMap sumC = cItem.statisticInfo.allStatisticInfo[i];
                    StatisticUnitMap sumP = pItem.statisticInfo.allStatisticInfo[i];
                    for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                        StatisticUnit suC = sumC.statisticUnitMap[cdt];
                        StatisticUnit suP = sumP.statisticUnitMap[cdt];
                        StatisticUnit suM = sumME.statisticUnitMap[cdt];

                        float subArea = (suC.missCount + suP.missCount) * 0.5f;
                        if (loopCount < FAST_COUNT)
                        {
#if USE_EMA_CALC
                            suM.fastData.missCountAreaTotal += suC.missCount;// subArea;
                            suM.fastData.missCountArea = suM.fastData.missCountAreaTotal / (loopCount + 1);
#else
                            suM.fastData.missCountArea += subArea;
#endif
                            if (suM.fastData.prevMaxMissCount < suP.missCount)
                            {
                                suM.fastData.prevMaxMissCount = suP.missCount;
                                suM.fastData.prevMaxMissCountIndex = pItem.idGlobal;
                            }
                        }
                        if (loopCount < SHOR_COUNT)
                        {
#if USE_EMA_CALC
                            suM.shortData.missCountAreaTotal += suC.missCount;// subArea;
                            suM.shortData.missCountArea = suM.shortData.missCountAreaTotal / (loopCount + 1);
#else
                            suM.shortData.missCountArea += subArea;
#endif
                            if (suM.shortData.prevMaxMissCount < suP.missCount)
                            {
                                suM.shortData.prevMaxMissCount = suP.missCount;
                                suM.shortData.prevMaxMissCountIndex = pItem.idGlobal;
                            }
                        }
                        if (loopCount < LONG_COUNT)
                        {
#if USE_EMA_CALC
                            suM.longData.missCountAreaTotal += suC.missCount;// subArea;
                            suM.longData.missCountArea = suM.longData.missCountAreaTotal / (loopCount + 1);
#else
                            suM.longData.missCountArea += subArea;
#endif
                            if (suM.longData.prevMaxMissCount < suP.missCount)
                            {
                                suM.longData.prevMaxMissCount = suP.missCount;
                                suM.longData.prevMaxMissCountIndex = pItem.idGlobal;
                            }
                        }
                    }
                }
                cItem = pItem;
                ++loopCount;
                if (loopCount == LONG_COUNT)
                    break;
            }
        }
    }

#if ENABLE_GROUP_COLLECT
    public class SimData
    {
        public string killList;
        public TestResultType predictResult;
        public long cost;
        public long reward;
        public long costTotal;
        public long rewardTotal;
        public long predictCount;
        public long rightCount;
        public long profit;
        public KillType killType;
        public List<SByte> killAndValue;
        public GroupType killAndValueAtGroup;
        public float g6Score;
        public float g3Score;
        public float g1Score;

        public SimData()
        {
        }

        public void Reset()
        {
            predictResult = TestResultType.eTRTIgnore;
            cost = reward = costTotal = rewardTotal = predictCount = rightCount = profit = 0;
            killList = null;

            killAndValue = null;
            killAndValueAtGroup = GroupType.eGT6;

            g6Score = g3Score = g1Score = 0.0f;
        }
    }
#endif
}
