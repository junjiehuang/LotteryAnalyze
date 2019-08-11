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

        public const int SAMPLE_COUNT_30 = 30;
        public const int SAMPLE_COUNT_10 = 10;
        public const int SAMPLE_COUNT_5 = 5;
        public const int SAMPLE_COUNT_3 = 3;

        public int validCount30 = 0;
        public int validCount10 = 0;
        public int validCount5 = 0;
        public int validCount3 = 0;

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
            validCount30 = 0;
            while (curItem != null && validCount30 < SAMPLE_COUNT_30)
            {
                for (int i = 0; i < 5; ++i)
                {
                    allStatisticInfo[i].CollectCount(
                        curItem, i, 
                        validCount30 < SAMPLE_COUNT_3, 
                        validCount30 < SAMPLE_COUNT_5, 
                        validCount30 < SAMPLE_COUNT_10, 
                        validCount30 < SAMPLE_COUNT_30);
                }
                curItem = curItem.parent.GetPrevItem(curItem);
                ++validCount30;
            }

            validCount10 = validCount30;
            if (validCount10 > SAMPLE_COUNT_10)
                validCount10 = SAMPLE_COUNT_10;

            validCount5 = validCount30;
            if (validCount5 > SAMPLE_COUNT_5)
                validCount5 = SAMPLE_COUNT_5;

            validCount3 = validCount30;
            if (validCount3 > SAMPLE_COUNT_3)
                validCount3 = SAMPLE_COUNT_3;

            for (int i = 0; i < 5; ++i)
            {
                allStatisticInfo[i].CollectProbability();
            }

            CollectMissCountArea();

            hasCollect = true;
        }

        void ResetSample(StatisticData sd, StatisticUnit su, StatisticUnitMap sumPre, StatisticUnit suPre, int idGlobal)
        {
            if (sd.appearProbabilityDiffWithTheory <= 0)
                sd.underTheoryCount = sumPre == null ? 1 : (suPre.sample3Data.underTheoryCount + 1);
            else
                sd.underTheoryCount = 0;

            sd.prevMaxMissCount = su.missCount;
            sd.prevMaxMissCountIndex = idGlobal;
        }

        void ProcSample(int loopCount, int sampleCount, float subArea, StatisticData sdM, StatisticUnit suC, StatisticUnit suP, DataItem pItem)
        {
            if (loopCount < sampleCount)
            {
#if USE_EMA_CALC
                sdM.missCountAreaTotal += suC.missCount;
                sdM.missCountArea = sdM.missCountAreaTotal / (loopCount + 1);
#else
                sdM.missCountArea += subArea;
#endif
                if (sdM.prevMaxMissCount < suP.missCount)
                {
                    sdM.prevMaxMissCount = suP.missCount;
                    sdM.prevMaxMissCountIndex = pItem.idGlobal;
                }
            }
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
                    ResetSample(su.sample3Data, su, sumPre, suPre, lotteryData.idGlobal);
                    ResetSample(su.sample5Data, su, sumPre, suPre, lotteryData.idGlobal);
                    ResetSample(su.sample10Data, su, sumPre, suPre, lotteryData.idGlobal);
                    ResetSample(su.sample30Data, su, sumPre, suPre, lotteryData.idGlobal);
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
                        ProcSample(loopCount, SAMPLE_COUNT_3, subArea, suM.sample3Data, suC, suP, pItem);
                        ProcSample(loopCount, SAMPLE_COUNT_5, subArea, suM.sample5Data, suC, suP, pItem);
                        ProcSample(loopCount, SAMPLE_COUNT_10, subArea, suM.sample10Data, suC, suP, pItem);
                        ProcSample(loopCount, SAMPLE_COUNT_30, subArea, suM.sample30Data, suC, suP, pItem);
                    }
                }
                cItem = pItem;
                ++loopCount;
                if (loopCount == SAMPLE_COUNT_30)
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
