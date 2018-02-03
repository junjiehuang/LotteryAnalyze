using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    #region data manage

    // 针对指定类型的统计信息
    public class StatisticUnit
    {
        // 统计类型
        public CollectDataType cdt = CollectDataType.eNone;
        // 针对该统计类型的遗漏值
        public short missCount = 0;
        // 统计短期内出现该统计类型数据的次数
        public short appearCountShort = 0;
        // 统计长期内出现该统计类型数据的次数
        public short appearCountLong = 0;
        // 统计短期内出现该统计类型数据的百分比
        public float appearProbabilityShort = 0;
        // 统计长期内出现该统计类型数据的百分比
        public float appearProbabilityLong = 0;
        // 统计短期内出现该统计类型数据的百分比与理论概率的差值
        public float appearProbabilityDiffWithTheoryShort = 0;
        // 统计长期内出现该统计类型数据的百分比与理论概率的差值
        public float appearProbabilityDiffWithTheoryLong = 0;
    }

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
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.ePath1:
                        if (parent.lotteryData.path012OfEachSingle[numIndex] == 1)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.ePath2:
                        if (parent.lotteryData.path012OfEachSingle[numIndex] == 2)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eBigNum:
                        if (parent.lotteryData.bigOfEachSingle[numIndex])
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eSmallNum:
                        if (parent.lotteryData.bigOfEachSingle[numIndex] == false)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eOddNum:
                        if (parent.lotteryData.oddOfEachSingle[numIndex])
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eEvenNum:
                        if (parent.lotteryData.oddOfEachSingle[numIndex] == false)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.ePrimeNum:
                        if (parent.lotteryData.primeOfEachSingle[numIndex])
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eCompositeNum:
                        if (parent.lotteryData.primeOfEachSingle[numIndex] == false)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum0:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 0)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum1:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 1)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum2:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 2)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum3:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 3)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum4:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 4)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum5:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 5)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum6:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 6)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum7:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 7)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);

                        break;
                    case CollectDataType.eNum8:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 8)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                    case CollectDataType.eNum9:
                        if (parent.lotteryData.fiveNumLst[numIndex] == 9)
                            su.missCount = 0;
                        else if (prevItem == null)
                            su.missCount++;
                        else
                            su.missCount = (short)(prevItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[su.cdt].missCount + 1);
                        break;
                }
            }
        }
        public void CollectCount(DataItem cmpItem, int numIndex, bool onShort, bool onLong)
        {
            if (cmpItem == null)
                return;
            foreach(StatisticUnit su in statisticUnitMap.Values)
            {
                switch (su.cdt)
                {
                    case CollectDataType.ePath0:
                        if (cmpItem.path012OfEachSingle[numIndex] == 0)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.ePath1:
                        if (cmpItem.path012OfEachSingle[numIndex] == 1)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.ePath2:
                        if (cmpItem.path012OfEachSingle[numIndex] == 2)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eBigNum:
                        if (cmpItem.bigOfEachSingle[numIndex])
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eSmallNum:
                        if (cmpItem.bigOfEachSingle[numIndex] == false)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eOddNum:
                        if (cmpItem.oddOfEachSingle[numIndex])
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eEvenNum:
                        if (cmpItem.oddOfEachSingle[numIndex] == false)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.ePrimeNum:
                        if (cmpItem.primeOfEachSingle[numIndex])
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eCompositeNum:
                        if (cmpItem.primeOfEachSingle[numIndex] == false)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum0:
                        if (cmpItem.fiveNumLst[numIndex] == 0)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum1:
                        if (cmpItem.fiveNumLst[numIndex] == 1)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum2:
                        if (cmpItem.fiveNumLst[numIndex] == 2)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum3:
                        if (cmpItem.fiveNumLst[numIndex] == 3)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum4:
                        if (cmpItem.fiveNumLst[numIndex] == 4)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum5:
                        if (cmpItem.fiveNumLst[numIndex] == 5)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum6:
                        if (cmpItem.fiveNumLst[numIndex] == 6)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum7:
                        if (cmpItem.fiveNumLst[numIndex] == 7)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum8:
                        if (cmpItem.fiveNumLst[numIndex] == 8)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
                        break;
                    case CollectDataType.eNum9:
                        if (cmpItem.fiveNumLst[numIndex] == 9)
                        {
                            if (onShort) su.appearCountShort++;
                            if (onLong) su.appearCountLong++;
                        }
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
                su.appearProbabilityLong = su.appearCountLong * 100 / parent.validLongCount;
                su.appearProbabilityShort = su.appearCountShort * 100 / parent.validShortCount;
                float theoryProbability = GraphDataManager.GetTheoryProbability(cdt);
                su.appearProbabilityDiffWithTheoryLong = su.appearProbabilityLong - theoryProbability;
                su.appearProbabilityDiffWithTheoryShort = su.appearProbabilityShort - theoryProbability;
            }
        }
    }

    // 记录某期开奖号的所有统计信息
    public class LotteryStatisticInfo
    {
        public const int LONG_COUNT = 30;
        public const int SHOR_COUNT = 10;
        public int validShortCount = 0;
        public int validLongCount = 0;
        public DataItem lotteryData;
        public List<StatisticUnitMap> allStatisticInfo = new List<StatisticUnitMap>(); 

        public LotteryStatisticInfo(DataItem item)
        {
            lotteryData = item;
            for( int i = 0; i < 5; ++i )
            {
                StatisticUnitMap sum = new StatisticUnitMap();
                sum.parent = this;
                allStatisticInfo.Add(sum);
            }
        }

        public void Collect()
        {
            if (lotteryData == null)
                return;
            DataItem curItem = lotteryData.parent.GetPrevItem(lotteryData);
            for (int i = 0; i < 5; ++i)
            {
                allStatisticInfo[i].CollectMissCount(curItem, i);
            }
            curItem = lotteryData;
            validLongCount = 0;
            while (curItem != null && validLongCount < LONG_COUNT)
            {
                for (int i = 0; i < 5; ++i)
                {
                    allStatisticInfo[i].CollectCount(curItem, i, validLongCount < SHOR_COUNT, validLongCount < LONG_COUNT);
                }
                curItem = curItem.parent.GetPrevItem(curItem);
                ++validLongCount;
            }
            validShortCount = validLongCount;
            if (validShortCount > SHOR_COUNT)
                validShortCount = SHOR_COUNT;

            for (int i = 0; i < 5; ++i)
            {
                allStatisticInfo[i].CollectProbability();
            }
        }
    }


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

        /*
        // 记录当期每个数字位012路的遗漏值
        public List<int[]> path012MissingInfo = new List<int[]>();

        // 统计当期前N期（长期）每个数字位012路开出的数量
        public List<int[]> path012CountInfoLong = new List<int[]>();
        // 统计当期前N期（长期）每个数字位012路开出的比率
        public List<int[]> path012ProbabilityLong = new List<int[]>();
        // 统计当期前N期（长期）每个数字位012路开出的比率与理论开出概率的差值
        public List<int[]> path012ProbabilityLongDiffWithTheory = new List<int[]>();

        // 统计当期前M期（短期）每个数字位012路开出的数量
        public List<int[]> path012CountInfoShort = new List<int[]>();
        // 统计当期前M期（短期）每个数字位012路开出的比率
        public List<int[]> path012ProbabilityShort = new List<int[]>();
        // 统计当期前M期（短期）每个数字位012路开出的比率与理论开出概率的差值
        public List<int[]> path012ProbabilityShortDiffWithTheory = new List<int[]>();
        */

        public SimData()
        {
            /*
            for( int i = 0; i < 5; ++i )
            {
                path012MissingInfo.Add(new int[3]);
                path012CountInfoLong.Add(new int[3]);
                path012CountInfoShort.Add(new int[3]);
                path012ProbabilityShort.Add(new int[3]);
                path012ProbabilityShortDiffWithTheory.Add(new int[3]);
                path012ProbabilityLong.Add(new int[3]);
            }
            */
        }

        public void Reset()
        {
            predictResult = TestResultType.eTRTIgnore;
            cost = reward = costTotal = rewardTotal = predictCount = rightCount = profit = 0;
            killList = null;

            killAndValue = null;
            killAndValueAtGroup = GroupType.eGT6;

            g6Score = g3Score = g1Score = 0.0f;
            ResetPath012Info();
        }

        public void ResetPath012Info()
        {
            /*
            for (int i = 0; i < 5; ++i)
            {
                path012MissingInfo[i][0] = path012MissingInfo[i][1] = path012MissingInfo[i][2] = 0;
                path012CountInfoLong[i][0] = path012CountInfoLong[i][1] = path012CountInfoLong[i][2] = 0;
                path012ProbabilityLong[i][0] = path012ProbabilityLong[i][1] = path012ProbabilityLong[i][2] = 0;                
                path012CountInfoShort[i][0] = path012CountInfoShort[i][1] = path012CountInfoShort[i][2] = 0;
                path012ProbabilityShort[i][0] = path012ProbabilityShort[i][1] = path012ProbabilityShort[i][2] = 0;
                path012ProbabilityShortDiffWithTheory[i][0] = path012ProbabilityShortDiffWithTheory[i][1] = path012ProbabilityShortDiffWithTheory[i][2] = 0;
            }
            */
        }
    }

    public class DataItem
    {
        static int[] TheoryProbabilityOfPath012 = new int[3] { 40, 30, 30, };

        public OneDayDatas parent = null;

        public int idGlobal = -1;
        public int idInOneDay;
        public string idTag;
        public string lotteryNumber;
        public SByte andValue;
        public SByte rearValue;
        public SByte crossValue;
        public GroupType groupType;
        // 5个数字
        public List<SByte> fiveNumLst = new List<SByte>();
        // 后三数字
        public List<SByte> valuesOfLastThree = new List<SByte>();

        // 每个数字的012路属性
        public List<SByte> path012OfEachSingle = new List<SByte>();
        // 每个数是否奇数
        public List<bool> oddOfEachSingle = new List<bool>();
        // 每个数是否质数
        public List<bool> primeOfEachSingle = new List<bool>();
        // 每个数是否大数
        public List<bool> bigOfEachSingle = new List<bool>();

        public LotteryStatisticInfo statisticInfo;
        public SimData simData;

        public DataItem(string idStr, string numStr, int fileID)
        {
            //id = int.Parse(idStr);
            lotteryNumber = numStr;
            idTag = fileID + "-" + idStr;
            andValue = Util.CalAndValue(lotteryNumber);
            rearValue = Util.CalRearValue(lotteryNumber);
            crossValue = Util.CalCrossValue(lotteryNumber);
            groupType = Util.GetGroupType(lotteryNumber);
            GetValuesInThreePos();
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
            simData = new SimData();
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
        public void GetValuesInThreePos()
        {
            if (valuesOfLastThree.Count == 0)
            {
                valuesOfLastThree.Add(GetBaiNumber());
                valuesOfLastThree.Add(GetShiNumber());
                valuesOfLastThree.Add(GetGeNumber());
            }
        }
        public void CollectShortPath012Info()
        {           
            /* 
            for (int i = 0; i < 5; ++i)
            {
                simData.path012CountInfoLong[i][0] = simData.path012CountInfoLong[i][1] = simData.path012CountInfoLong[i][2] = 0;
                simData.path012ProbabilityLong[i][0] = simData.path012ProbabilityLong[i][1] = simData.path012ProbabilityLong[i][2] = 0;                
                simData.path012CountInfoShort[i][0] = simData.path012CountInfoShort[i][1] = simData.path012CountInfoShort[i][2] = 0;
                simData.path012ProbabilityShort[i][0] = simData.path012ProbabilityShort[i][1] = simData.path012ProbabilityShort[i][2] = 0;
                simData.path012ProbabilityShortDiffWithTheory[i][0] = simData.path012ProbabilityShortDiffWithTheory[i][1] = simData.path012ProbabilityShortDiffWithTheory[i][2] = 0;
            }

            int validCount = 0;
            DataItem prevItem = this;
            for( int i = 0; i < ColumnSimulateSingleBuyLottery.S_SHORT_COUNT; ++i )
            {
                prevItem = prevItem.parent.GetPrevItem(prevItem);
                if (prevItem == null)
                    break;
                ++validCount;
                for ( int j = 0; j < 5; ++j )
                {
                    for( int k = 0; k < 3; ++k )
                    {
                        if (prevItem.path012OfEachSingle[j] == k)
                            simData.path012CountInfoShort[j][k] = simData.path012CountInfoShort[j][k] + 1;
                    }
                }
            }
            if (validCount > 0)
            {
                for (int i = 0; i < 5; ++i)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        simData.path012ProbabilityShort[i][k] = simData.path012CountInfoShort[i][k] * 100 / validCount;
                        simData.path012ProbabilityShortDiffWithTheory[i][k] = simData.path012ProbabilityShort[i][k] - TheoryProbabilityOfPath012[k];
                    }
                }
            }
            */
        }
    }

    public class OneDayDatas
    {
        public int dateID = 0;
        public List<DataItem> datas = new List<DataItem>();
        public Dictionary<string, DataItem> searchMap = new Dictionary<string, DataItem>();
        public SimData simData = new SimData();


        public OneDayDatas()
        {
            simData.Reset();
        }
        public void AddItem(DataItem item)
        {
            if(searchMap.ContainsKey(item.idTag) == false)
            {
                item.parent = this;
                item.idInOneDay = datas.Count;

                searchMap.Add(item.idTag, item);
                datas.Add(item);
            }
            else
            {
                throw new Exception("has contains dataitem " + item.idTag);
            }
        }
        public DataItem FindItem(string idTag)
        {
            if (searchMap.ContainsKey(idTag))
            {
                return searchMap[idTag];
            }
            return null;
        }

        public DataItem GetTailItem()
        {
            if (datas.Count > 0)
                return datas[datas.Count-1];
            return null;
        }
        public DataItem GetFirstItem()
        {
            if (datas.Count > 0)
                return datas[0];
            return null;
        }
        public DataItem GetPrevItem(DataItem curItem)
        {
            int curID = curItem.idInOneDay - 1;
            if (curID >= 0)
                return datas[curID];
            else
            {
                OneDayDatas prevODD = DataManager.GetInst().GetPrevOneDayDatas(this);
                if (prevODD != null)
                    return prevODD.GetTailItem();
            }
            return null;
        }
        public DataItem GetNextItem(DataItem curItem)
        {
            int curID = curItem.idInOneDay + 1;
            if (curID < datas.Count)
                return datas[curID];
            else
            {
                OneDayDatas nextODD = DataManager.GetInst().GetNextOneDayDatas(this);
                if (nextODD != null)
                    return nextODD.GetFirstItem();
            }
            return null;
        }
        public void CollectOneDayLotteryInfo()
        {
            /*
            simData.ResetPath012Info();
            for (int lotIndex = 0; lotIndex < datas.Count; ++lotIndex)
            {
                DataItem data = datas[lotIndex];
                for (int numIndex = 0; numIndex < 5; ++numIndex)
                {
                    for (int pathIndex = 0; pathIndex < 3; ++pathIndex)
                    {
                        if (data.path012OfEachSingle[numIndex] == pathIndex)
                            simData.path012CountInfoShort[numIndex][pathIndex] = simData.path012CountInfoShort[numIndex][pathIndex] + 1;
                    }
                }
            }
            */
        }
    }

    public class DataManager
    {
        public Dictionary<int, OneDayDatas> allDatas = new Dictionary<int, OneDayDatas>();
        public List<int> indexs = new List<int>();
        public Dictionary<int, string> mFileMetaInfo = new Dictionary<int, string>();
        public SimData simData;
        public long curProfit = 0;

        DataManager()
        {
        }

        static DataManager sInst = null;
        public static DataManager GetInst()
        {
            if (sInst == null)
                sInst = new DataManager();
            return sInst;
        }

        public void ClearAllDatas()
        {
            allDatas.Clear();
            indexs.Clear();
        }

        public void AddMetaInfo(int key, string fileFullName)
        {
            if (mFileMetaInfo.ContainsKey(key) == false)
                mFileMetaInfo.Add(key, fileFullName);
        }

        public void LoadAllDatas(ref List<int> selectIDs)
        {
            for (int i = 0; i < selectIDs.Count; ++i)
            {
                int key = selectIDs[i];
                LoadData(key);
            }
        }
        public void LoadData(int key)
        {
            OneDayDatas data = null;
            string fullPath = mFileMetaInfo[key];
            if (Util.ReadFile(key, fullPath, ref data))
            {
                allDatas.Add(key, data);
                if (indexs.IndexOf(key) == -1)
                {
                    indexs.Add(key);
                    indexs.Sort();
                }
            }
        }
        public bool LoadDataExt(int key, ref OneDayDatas newODD, ref int newIndex)
        {
            newODD = null;
            newIndex = -1;

            OneDayDatas data = null;
            if (allDatas.ContainsKey(key))
                data = allDatas[key];
            if (data == null)
            {
                LoadData(key);
                newODD = allDatas[key];
                newIndex = 0;
                return true;
            }
            else
            {
                string fullPath = mFileMetaInfo[key];
                if (Util.ReadFile(key, fullPath, ref data, ref newIndex))
                {
                    newODD = data;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取上一日的开奖数据列表
        /// </summary>
        /// <param name="curData"></param>
        /// <returns></returns>
        public OneDayDatas GetPrevOneDayDatas(OneDayDatas curData)
        {
            int index = indexs.IndexOf(curData.dateID);
            if (index > 0)
            {
                --index;
                int newDateID = indexs[index];
                return allDatas[newDateID];
            }
            return null;
        }
        /// <summary>
        /// 获取下一日的开奖数据列表
        /// </summary>
        /// <param name="curData"></param>
        /// <returns></returns>
        public OneDayDatas GetNextOneDayDatas(OneDayDatas curData)
        {
            int index = indexs.IndexOf(curData.dateID);
            if (index < indexs.Count-1)
            {
                ++index;
                int newDateID = indexs[index];
                return allDatas[newDateID];
            }
            return null;
        }
        /// <summary>
        /// 获取上一期的开奖数据
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public DataItem GetPrevItem(DataItem curItem)
        {
            DataItem prevItem = curItem.parent.GetPrevItem(curItem);
            if (prevItem != null)
                return prevItem;
            OneDayDatas prevODD = GetPrevOneDayDatas(curItem.parent);
            if (prevODD != null)
                return prevODD.GetTailItem();
            return null;
        }
        /// <summary>
        /// 获取下一期的开奖数据
        /// </summary>
        /// <param name="curItem"></param>
        /// <returns></returns>
        public DataItem GetNextItem(DataItem curItem)
        {
            DataItem nextItem = curItem.parent.GetNextItem(curItem);
            if (nextItem != null)
                return nextItem;
            OneDayDatas nextODD = GetNextOneDayDatas(curItem.parent);
            if (nextODD != null)
                return nextODD.GetFirstItem();
            return null;
        }
        /// <summary>
        /// 获取最新的开奖数据
        /// </summary>
        /// <returns></returns>
        public DataItem GetLatestItem()
        {
            if (allDatas.Count > 0)
            {
                int lastIndex = indexs[indexs.Count - 1];
                if (allDatas.ContainsKey(lastIndex))
                {
                    OneDayDatas odd = allDatas[lastIndex];
                    if(odd.datas.Count > 0)
                        return odd.datas[odd.datas.Count - 1];
                    else if(indexs.Count > 1)
                    { 
                        lastIndex = indexs[indexs.Count - 2];
                        odd = allDatas[lastIndex];
                        if(odd.datas.Count > 0)
                            return odd.datas[odd.datas.Count - 1];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取第一个开奖数据
        /// </summary>
        /// <returns></returns>
        public DataItem GetFirstItem()
        {
            if (allDatas.Count > 0)
            {
                int firstDayIndex = indexs[0];
                if (allDatas.ContainsKey(firstDayIndex))
                {
                    OneDayDatas odd = allDatas[firstDayIndex];
                    if (odd.datas.Count > 0)
                        return odd.datas[0];
                }
            }
            return null;
        }

        public DataItem GetDataItemByIdTag(string idTag)
        {
            if (string.IsNullOrEmpty(idTag))
                return null;
            string[] strs = idTag.Split('-');
            int key = int.Parse(strs[0]);
            OneDayDatas odd = allDatas[key];
            DataItem item = odd.FindItem(idTag);
            return item;
        }
    }

    #endregion

    #region simulation

    public enum SimState
    {
        eNotStart = 0,
        eSimulating,
        eFinished,
    }

    public class WrongInfo
    {
        public long costTotal;
        public string startTag;
        public int round;

        public WrongInfo()
        {
            costTotal = 0;
            round = 0;
            startTag = "";
        }
        public void CopyFrom(WrongInfo other)
        {
            costTotal = other.costTotal;
            startTag = other.startTag;
            round = other.round;
        }
    }

    public enum KillType
    {
        // 只匹配组三
        eKTGroup3 = 0,
        // 只匹配组六
        eKTGroup6,
        // 交叉匹配
        eKTBlend,
        // 根据组三形态匹配组六
        eKTGroup6OnGroup3,
        // 不做
        eKTNone,
    }

    public enum SimType
    {
        eGroup3,
        eGroup2,
    }

    public abstract class SimulationBase
    {
        public virtual void SortWrongInfos(bool byRound) { }
        public virtual void StepRatio() { }
        public virtual void ResetRatio() { }
        public virtual void StartSimulate() { }
        public virtual void UpdateSimulate() { }
    } 

    public class SimulationGroup3 : SimulationBase
    {
        static SimState curState = SimState.eNotStart;
        static int curSimIndex = -1;
        static int curItemIndex = -1;
        static WrongInfo curCal = null;
        public static bool isCurKillGroup3 = false;
        public static List<WrongInfo> allWrongInfos = new List<WrongInfo>();
        public static KillType killType = KillType.eKTGroup3;
        public static int g3Round = 0;
        public static int g6Round = 0;
        public static KillType curKillType = KillType.eKTGroup6;

        const float G1SCORE = 1000.0f / 10.0f;
        const float G3SCORE = 1000.0f / 270.0f;
        const float G6SCORE = 1000.0f / 720.0f;

        static int curRatio = 1;
        public static bool enableDoubleRatioIfFailed = true;
        public static int firmRatio = 10;
        public static int maxRatio = 32;

        public override void SortWrongInfos(bool byRound)
        {
            if (byRound)
            {
                allWrongInfos.Sort((x, y) =>
                {
                    if (x.round > y.round)
                        return -1;
                    return 1;
                });
            }
            else
            {
                allWrongInfos.Sort((x, y) =>
                {
                    if (x.costTotal > y.costTotal)
                        return -1;
                    return 1;
                });
            }
        }

        public override void StepRatio()
        {
            if (enableDoubleRatioIfFailed)
            {
                curRatio *= 2;
                if (maxRatio > 0 && curRatio > maxRatio)
                    curRatio = maxRatio;
            }
            else
            {
                curRatio = firmRatio;
            }
        }
        public override void ResetRatio()
        {
            curRatio = 1;
            if (!enableDoubleRatioIfFailed)
                curRatio = firmRatio;
        }
        public override void StartSimulate()
        {
            DataManager mgr = DataManager.GetInst();
            mgr.simData.Reset();
            for (int i = 0; i < mgr.indexs.Count; ++i)
            {
                int curFileID = mgr.indexs[i];
                OneDayDatas odd = mgr.allDatas[curFileID];
                if (odd != null)
                {
                    odd.simData.Reset();
                    for (int j = 0; j < odd.datas.Count; ++j)
                    {
                        odd.datas[j].simData.Reset();
                    }
                }
            }
            curItemIndex = 0;
            curSimIndex = 0;
            mgr.curProfit = 0;
            ResetRatio();
            Program.mainForm.ResetResult();
            curState = SimState.eSimulating;
            allWrongInfos.Clear();
            g3Round = 0;
            g6Round = 0;
            killType = Program.mainForm.GetCurSelectedKillType();
            if (killType == KillType.eKTBlend)
                curKillType = KillType.eKTGroup6;
            else
                curKillType = KillType.eKTNone;
        }
        public override void UpdateSimulate()
        {
            DataManager mgr = DataManager.GetInst();
            if (curState == SimState.eSimulating)
            {
                if (curSimIndex < mgr.indexs.Count)
                {
                    int curFileID = mgr.indexs[curSimIndex];
                    OneDayDatas odd = mgr.allDatas[curFileID];
                    if (odd != null)
                    {
                        for (int i = 0; i < odd.datas.Count; ++i)
                        {
                            DataItem item = odd.datas[i];
                            item.simData.g1Score = mgr.simData.g1Score;
                            item.simData.g3Score = mgr.simData.g3Score;
                            item.simData.g6Score = mgr.simData.g6Score;
                            switch (item.groupType)
                            {
                                case GroupType.eGT1: mgr.simData.g1Score += G1SCORE; break;
                                case GroupType.eGT3: mgr.simData.g3Score += G3SCORE; break;
                                case GroupType.eGT6: mgr.simData.g6Score += G6SCORE; break;
                            }

                            TestResultType curResult = TestResultType.eTRTIgnore;
                            if (killType == KillType.eKTGroup3)
                                curResult = Util.SimButG3OnG1Out(item, curRatio);
                            else if (killType == KillType.eKTGroup6)
                                curResult = Util.SimBuyG6(item, curRatio);
                            else if (killType == KillType.eKTBlend)
                                curResult = Util.SimCrossBuyG6G3(item, curRatio);
                            else if (killType == KillType.eKTGroup6OnGroup3)
                                curResult = Util.SimBuyG6On5G3Out(item, curRatio);

                            Program.mainForm.RefreshResultItem(curItemIndex, item);
                            ++curItemIndex;

                            if (curCal == null && curResult != TestResultType.eTRTIgnore)
                            {
                                curCal = new WrongInfo();
                                curCal.costTotal = item.simData.cost;
                                curCal.startTag = item.idTag;
                                curCal.round = 1;
                                allWrongInfos.Add(curCal);
                                if (curResult == TestResultType.eTRTSuccess)
                                    curCal = null;
                            }
                            else if (curCal != null)
                            {
                                if (curResult == TestResultType.eTRTFailed)
                                {
                                    curCal.round++;
                                    curCal.costTotal += item.simData.cost;
                                }
                                else if (curResult == TestResultType.eTRTSuccess)
                                {
                                    curCal.round++;
                                    curCal.costTotal += item.simData.cost;
                                    curCal = null;
                                }
                            }
                        }
                    }
                    ++curSimIndex;
                }
                if (curSimIndex == mgr.indexs.Count)
                {
                    curCal = null;
                    curState = SimState.eFinished;
                    Program.mainForm.RefreshResultPanel();
                }
            }
        }
    }


    public class SimulationGroup2 : SimulationBase
    {
        public override void SortWrongInfos(bool byRound)
        {

        }
        public override void StepRatio()
        {
        }
        public override void ResetRatio()
        {
        }
        public override void StartSimulate()
        {
        }
        public override void UpdateSimulate()
        {
        }
    }


    public class Simulator
    {
        static SimulationBase curSim = null;
        static Dictionary<SimType, SimulationBase> simDict = null;

        static Simulator()
        {
            simDict = new Dictionary<SimType, SimulationBase>();
            simDict.Add(SimType.eGroup3, new SimulationGroup3());
            simDict.Add(SimType.eGroup2, new SimulationGroup2());
            curSim = simDict[SimType.eGroup3];
        }

        public static void SortWrongInfos(bool byRound)
        {
            curSim.SortWrongInfos(byRound);
        }

        public static void StepRatio()
        {
            curSim.StepRatio();
        }

        public static void ResetRatio()
        {
            curSim.ResetRatio();
        }

        public static void StartSimulate()
        {
            curSim.StartSimulate();
        }

        public static void UpdateSimulate()
        {
            curSim.UpdateSimulate();
        }
    }

    #endregion
}
