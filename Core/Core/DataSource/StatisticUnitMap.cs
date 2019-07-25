using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    // 记录某个数字位的所有类型的统计信息
    public class StatisticUnitMap
    {
        public LotteryStatisticInfo parent;
        public Dictionary<CollectDataType, StatisticUnit> statisticUnitMap = new Dictionary<CollectDataType, StatisticUnit>();

        public StatisticUnitMap()
        {
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
                StatisticUnit su = new StatisticUnit();
                su.cdt = cdt;
                statisticUnitMap.Add(cdt, su);
            }
        }
        public void CollectMissCount(DataItem prevItem, int numIndex)
        {
            foreach (StatisticUnit su in statisticUnitMap.Values)
            {
                switch (su.cdt)
                {
                    case CollectDataType.ePath0:
                        if (parent.lotteryData.path012OfEachSingle[numIndex] == 0)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.ePath1:
                        if (parent.lotteryData.path012OfEachSingle[numIndex] == 1)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.ePath2:
                        if (parent.lotteryData.path012OfEachSingle[numIndex] == 2)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eBigNum:
                        if (parent.lotteryData.bigOfEachSingle[numIndex])
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eSmallNum:
                        if (parent.lotteryData.bigOfEachSingle[numIndex] == false)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eOddNum:
                        if (parent.lotteryData.oddOfEachSingle[numIndex])
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eEvenNum:
                        if (parent.lotteryData.oddOfEachSingle[numIndex] == false)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.ePrimeNum:
                        if (parent.lotteryData.primeOfEachSingle[numIndex])
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eCompositeNum:
                        if (parent.lotteryData.primeOfEachSingle[numIndex] == false)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum0:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 0)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum1:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 1)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum2:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 2)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum3:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 3)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum4:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 4)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum5:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 5)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum6:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 6)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum7:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 7)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);

                        break;
                    case CollectDataType.eNum8:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 8)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum9:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 9)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                }

                if (su.missCount > 0)
                    su.appearCount = 0;
                else
                {
                    if (prevItem != null)
                        su.appearCount = (Byte)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].appearCount + 1);
                    else
                        su.appearCount = 1;
                }
            }
        }

        void CollectCount(StatisticUnit su, bool isAppear, bool onFast, bool onShort, bool onLong)
        {
            if (isAppear)
            {
                if (onFast) su.fastData.appearCount++;
                if (onShort) su.shortData.appearCount++;
                if (onLong) su.longData.appearCount++;
            }
            else
            {
                if (onFast) su.fastData.disappearCount++;
                if (onShort) su.shortData.disappearCount++;
                if (onLong) su.longData.disappearCount++;
            }
        }

        public void CollectCount(DataItem cmpItem, int numIndex, bool onFast, bool onShort, bool onLong)
        {
            if (cmpItem == null)
                return;
            foreach (StatisticUnit su in statisticUnitMap.Values)
            {
                switch (su.cdt)
                {
                    case CollectDataType.ePath0:
                        CollectCount(su, cmpItem.path012OfEachSingle[numIndex] == 0, onFast, onShort, onLong);
                        break;
                    case CollectDataType.ePath1:
                        CollectCount(su, cmpItem.path012OfEachSingle[numIndex] == 1, onFast, onShort, onLong);
                        break;
                    case CollectDataType.ePath2:
                        CollectCount(su, cmpItem.path012OfEachSingle[numIndex] == 2, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eBigNum:
                        CollectCount(su, cmpItem.bigOfEachSingle[numIndex], onFast, onShort, onLong);
                        break;
                    case CollectDataType.eSmallNum:
                        CollectCount(su, cmpItem.bigOfEachSingle[numIndex] == false, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eOddNum:
                        CollectCount(su, cmpItem.oddOfEachSingle[numIndex], onFast, onShort, onLong);
                        break;
                    case CollectDataType.eEvenNum:
                        CollectCount(su, cmpItem.oddOfEachSingle[numIndex] == false, onFast, onShort, onLong);
                        break;
                    case CollectDataType.ePrimeNum:
                        CollectCount(su, cmpItem.primeOfEachSingle[numIndex], onFast, onShort, onLong);
                        break;
                    case CollectDataType.eCompositeNum:
                        CollectCount(su, cmpItem.primeOfEachSingle[numIndex] == false, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum0:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 0, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum1:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 1, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum2:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 2, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum3:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 3, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum4:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 4, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum5:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 5, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum6:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 6, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum7:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 7, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum8:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 8, onFast, onShort, onLong);
                        break;
                    case CollectDataType.eNum9:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 9, onFast, onShort, onLong);
                        break;
                }
            }
        }
        public void CollectProbability()
        {
            if (parent.validLongCount < 1)
                return;
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
                StatisticUnit su = statisticUnitMap[cdt];
                su.fastData.appearProbability = (float)su.fastData.appearCount * 100.0f / parent.validFastCount;
                su.shortData.appearProbability = (float)su.shortData.appearCount * 100.0f / parent.validShortCount;
                su.longData.appearProbability = (float)su.longData.appearCount * 100.0f / parent.validLongCount;
                float theoryProbability = GraphDataManager.GetTheoryProbability(cdt);
                su.fastData.appearProbabilityDiffWithTheory = (su.fastData.appearProbability - theoryProbability) / theoryProbability;
                su.shortData.appearProbabilityDiffWithTheory = (su.shortData.appearProbability - theoryProbability) / theoryProbability;
                su.longData.appearProbabilityDiffWithTheory = (su.longData.appearProbability - theoryProbability) / theoryProbability;
            }
        }
    }
}
