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

        void CollectCount(StatisticUnit su, bool isAppear, bool on3, bool on5, bool on10, bool on30)
        {
            if (isAppear)
            {
                if (on3) su.sample3Data.appearCount++;
                if (on5) su.sample5Data.appearCount++;
                if (on10) su.sample10Data.appearCount++;
                if (on30) su.sample30Data.appearCount++;
            }
            else
            {
                if (on3) su.sample3Data.disappearCount++;
                if (on5) su.sample5Data.disappearCount++;
                if (on10) su.sample10Data.disappearCount++;
                if (on30) su.sample30Data.disappearCount++;
            }
        }

        public void CollectCount(DataItem cmpItem, int numIndex, bool on3, bool on5, bool on10, bool on30)
        {
            if (cmpItem == null)
                return;
            foreach (StatisticUnit su in statisticUnitMap.Values)
            {
                switch (su.cdt)
                {
                    case CollectDataType.ePath0:
                        CollectCount(su, cmpItem.path012OfEachSingle[numIndex] == 0, on3, on5, on10, on30);
                        break;
                    case CollectDataType.ePath1:
                        CollectCount(su, cmpItem.path012OfEachSingle[numIndex] == 1, on3, on5, on10, on30);
                        break;
                    case CollectDataType.ePath2:
                        CollectCount(su, cmpItem.path012OfEachSingle[numIndex] == 2, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eBigNum:
                        CollectCount(su, cmpItem.bigOfEachSingle[numIndex], on3, on5, on10, on30);
                        break;
                    case CollectDataType.eSmallNum:
                        CollectCount(su, cmpItem.bigOfEachSingle[numIndex] == false, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eOddNum:
                        CollectCount(su, cmpItem.oddOfEachSingle[numIndex], on3, on5, on10, on30);
                        break;
                    case CollectDataType.eEvenNum:
                        CollectCount(su, cmpItem.oddOfEachSingle[numIndex] == false, on3, on5, on10, on30);
                        break;
                    case CollectDataType.ePrimeNum:
                        CollectCount(su, cmpItem.primeOfEachSingle[numIndex], on3, on5, on10, on30);
                        break;
                    case CollectDataType.eCompositeNum:
                        CollectCount(su, cmpItem.primeOfEachSingle[numIndex] == false, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum0:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 0, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum1:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 1, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum2:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 2, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum3:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 3, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum4:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 4, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum5:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 5, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum6:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 6, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum7:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 7, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum8:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 8, on3, on5, on10, on30);
                        break;
                    case CollectDataType.eNum9:
                        CollectCount(su, cmpItem.fiveNumLst[numIndex] == 9, on3, on5, on10, on30);
                        break;
                }
            }
        }
        public void CollectProbability()
        {
            if (parent.validCount30 < 1)
                return;
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
                StatisticUnit su = statisticUnitMap[cdt];
                su.sample3Data.appearProbability = (float)su.sample3Data.appearCount * 100.0f / parent.validCount3;
                su.sample5Data.appearProbability = (float)su.sample5Data.appearCount * 100.0f / parent.validCount5;
                su.sample10Data.appearProbability = (float)su.sample10Data.appearCount * 100.0f / parent.validCount10;
                su.sample30Data.appearProbability = (float)su.sample30Data.appearCount * 100.0f / parent.validCount30;
                float theoryProbability = GraphDataManager.GetTheoryProbability(cdt);
                su.sample3Data.appearProbabilityDiffWithTheory = (su.sample3Data.appearProbability - theoryProbability) / theoryProbability;
                su.sample5Data.appearProbabilityDiffWithTheory = (su.sample5Data.appearProbability - theoryProbability) / theoryProbability;
                su.sample10Data.appearProbabilityDiffWithTheory = (su.sample10Data.appearProbability - theoryProbability) / theoryProbability;
                su.sample30Data.appearProbabilityDiffWithTheory = (su.sample30Data.appearProbability - theoryProbability) / theoryProbability;
            }
        }
    }
}
