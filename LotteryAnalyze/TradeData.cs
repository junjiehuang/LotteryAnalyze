using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public enum TradeType
    {
        eNone,
        eOneStar,
        eTenSingleTwoStar,
        eHundredTenTwoStar,
        eThousandHundredTwoStar,
        eTenThousandHundredTwoStar,
        eHundredTenSingleThreeStar,
        eThousandHundredTenThreeStart,
        eTenThousandThousandHundredThreeStart,
        eFiveStar,
    }

    public enum TradeStatus
    {
        eWaiting,
        eDone,
    }

    struct NumberCmpInfo
    {
        public SByte number;
        public float rate;
        public bool largerThanTheoryProbability;

        public string ToString()
        {
            return number + "(" + rate.ToString("f2") + "%) ";
        }

        public static int FindIndex(List<NumberCmpInfo> nums, SByte number)
        {
            for(int i = 0; i < nums.Count; ++i )
            {
                if (nums[i].number == number)
                    return i;
            }
            return -1;
        }
    }

    class TradeNumbers
    {
        public List<NumberCmpInfo> tradeNumbers = new List<NumberCmpInfo>();
        public int tradeCount = 0;

        public void GetInfo(ref string info)
        {
            if (tradeNumbers.Count > 0)
            {
                info += " { ";
                for (int i = 0; i < tradeNumbers.Count; ++i)
                {
                    info += tradeNumbers[i].ToString();
                    if (i != tradeNumbers.Count - 1)
                        info += ",";
                }
                info += "} 倍数：" + tradeCount + "\n";
            }
        }
        public void SelPath012Number(int path, int tradeCount, ref List<NumberCmpInfo> nums)
        {
            //this.tradeCount = tradeCount;
            //tradeNumbers.Clear();
            //if (path == 0)
            //{ tradeNumbers.Add(0); tradeNumbers.Add(3); tradeNumbers.Add(6); tradeNumbers.Add(9); }
            //else if (path == 1)
            //{ tradeNumbers.Add(1); tradeNumbers.Add(4); tradeNumbers.Add(7); }
            //else if (path == 2)
            //{ tradeNumbers.Add(2); tradeNumbers.Add(5); tradeNumbers.Add(8); }
            this.tradeCount = tradeCount;
            for( int i = 0; i < nums.Count; ++i )
            {
                if(nums[i].number % 3 == path && ContainsNumber(nums[i].number) == false)
                {
                    tradeNumbers.Add(nums[i]);
                }
            }
        }
        public bool ContainsNumber( SByte number)
        {
            for(int i = 0; i < tradeNumbers.Count; ++i )
            {
                if (tradeNumbers[i].number == number)
                    return true;
            }
            return false;
        }
    }

    class TradeDataBase
    {
        public static string[] NUM_TAGS = new string[] { "万位", "千位", "百位", "十位", "个位", };

        public TradeStatus tradeStatus = TradeStatus.eWaiting;
        public TradeType tradeType = TradeType.eNone;
        public DataItem lastDateItem = null;
        public DataItem targetLotteryItem = null;

        public float reward = 0;
        public float cost = 0;
        public float moneyBeforeTrade = 0;
        public float moneyAtferTrade = 0;
        protected string tips = "";
        public bool isAutoTrade = false;

        public virtual string GetTips() { return tips; }
        public virtual void Update() { }
    }

    class TradeDataOneStar : TradeDataBase
    {
        public static float SingleTradeCost = 1;
        public static float SingleTradeReward = 9.8f;

        public Dictionary<int, TradeNumbers> tradeInfo = new Dictionary<int, TradeNumbers>();


        public TradeDataOneStar()
        {
            tradeType = TradeType.eOneStar;
        }

        public void AddSelNum(int numIndex, ref List<SByte> selNums, int tradeCount, ref List<NumberCmpInfo> nums)
        {
            if(tradeInfo.ContainsKey(numIndex) == false)
            {
                TradeNumbers tn = new TradeNumbers();
                tradeInfo.Add(numIndex, tn);                
                tn.tradeCount = tradeCount;

                for (int i = 0; i < selNums.Count; ++i)
                {
                    int index = NumberCmpInfo.FindIndex(nums, selNums[i]);
                    if (index != -1)
                    {
                        tn.tradeNumbers.Add(nums[i]);
                    }
                }
            }
        }

        public override void Update()
        {
            if(tradeStatus == TradeStatus.eWaiting)
            {
                if(lastDateItem != null)
                {
                    targetLotteryItem = lastDateItem.parent.GetNextItem(lastDateItem);
                    if(targetLotteryItem != null)
                    {
                        reward = 0;
                        cost = 0;
                        foreach( int numIndex in tradeInfo.Keys)
                        {
                            TradeNumbers tns = tradeInfo[numIndex];
                            SByte dstValue = targetLotteryItem.GetNumberByIndex(numIndex);
                            if(tns.ContainsNumber(dstValue))
                                reward += SingleTradeReward * tns.tradeCount;
                            cost += SingleTradeCost * tns.tradeCount * tns.tradeNumbers.Count;
                        }
                        moneyBeforeTrade = TradeDataManager.Instance.currentMoney;
                        TradeDataManager.Instance.currentMoney += reward - cost;
                        moneyAtferTrade = TradeDataManager.Instance.currentMoney;
                        tradeStatus = TradeStatus.eDone;
                        if (cost == 0)
                            TradeDataManager.Instance.untradeCount++;
                        else if (reward > 0)
                            TradeDataManager.Instance.rightCount++;
                        else
                            TradeDataManager.Instance.wrongCount++;
                    }
                }
            }
        }

        public override string GetTips()
        {
            // 已开奖
            if(tips.Length == 0 && targetLotteryItem != null)
            {
                tips += "[期号：" + targetLotteryItem.idTag + "] [号码：" + targetLotteryItem.lotteryNumber + "]\n";
                foreach( int key in tradeInfo.Keys)
                {
                    TradeNumbers tn = tradeInfo[key];
                    if(tn.tradeNumbers.Count > 0)
                    {
                        tips += TradeDataBase.NUM_TAGS[key];
                        tn.GetInfo(ref tips);
                    }
                }
                tips += "[成本：" + cost + "] [奖金：" + reward + "] [剩余：" + moneyAtferTrade + "]";
            }
            // 等待开奖
            else if(targetLotteryItem == null)
            {
                string info = "";
                foreach (int key in tradeInfo.Keys)
                {
                    TradeNumbers tn = tradeInfo[key];
                    if (tn.tradeNumbers.Count > 0)
                    {
                        info += TradeDataBase.NUM_TAGS[key];
                        tn.GetInfo(ref info);
                    }
                }
                return info;
            }
            return tips;
        }
    }


    class TradeDataManager
    {
        static TradeDataManager sInst = null;
        public List<TradeDataBase> historyTradeDatas = new List<TradeDataBase>();
        public List<TradeDataBase> waitingTradeDatas = new List<TradeDataBase>();
        public float startMoney = 2000;
        public float currentMoney = 0;
        public float minValue = 0;
        public float maxValue = 0;
        public int rightCount = 0;
        public int wrongCount = 0;
        public int untradeCount = 0;

        //public bool simTradeFromFirstEveryTime = true;
        public int simSelNumIndex = 0;
        DataItem curTestTradeItem = null;
        bool pauseAutoTrade = true;
        bool needGetLatestItem = false;
        public List<int> tradeCountList = new List<int>();
        public int defaultTradeCount = 1;
        int currentTradeCountIndex = -1;
        public bool hasCompleted = false;
        bool stopAtTheLatestItem = false;
        public bool StopAtTheLatestItem
        {
            get { return stopAtTheLatestItem; }
            set { stopAtTheLatestItem = value; }
        }
        List<NumberCmpInfo> maxProbilityNums = new List<NumberCmpInfo>();

        TradeDataManager()
        {
            minValue = maxValue = currentMoney;
            tradeCountList.Add(1);
            tradeCountList.Add(2);
            tradeCountList.Add(4);
            tradeCountList.Add(8);
            tradeCountList.Add(16);
        }
        public static TradeDataManager Instance
        {
            get
            {
                if (sInst == null)
                    sInst = new TradeDataManager();
                return sInst;
            }
        }
        
        public DataItem GetLatestTradedDataItem()
        {
            if (historyTradeDatas.Count > 0)
                return historyTradeDatas[historyTradeDatas.Count - 1].targetLotteryItem;
            return null;
        }

        public TradeDataBase NewTrade(TradeType tradeType)
        {
            TradeDataBase trade = null;
            switch(tradeType)
            {
                case TradeType.eOneStar:
                    trade = new TradeDataOneStar();
                    break;
            }
            if (trade != null)
                waitingTradeDatas.Insert(0,trade);
            return trade;
        }

        public void Update()
        {
            if(waitingTradeDatas.Count > 0)
            {
                for( int i = waitingTradeDatas.Count-1; i >= 0; --i )
                {
                    waitingTradeDatas[i].Update();
                    if (waitingTradeDatas[i].tradeStatus == TradeStatus.eDone)
                    {
                        if(currentTradeCountIndex >= 0 && currentTradeCountIndex < tradeCountList.Count)
                        {
                            if (waitingTradeDatas[i].reward > 0)
                                currentTradeCountIndex = 0;
                            else if(waitingTradeDatas[i].cost > 0)
                                ++currentTradeCountIndex;
                            if (currentTradeCountIndex == tradeCountList.Count)
                                currentTradeCountIndex = 0;
                        }

                        if (maxValue < waitingTradeDatas[i].moneyAtferTrade)
                            maxValue = waitingTradeDatas[i].moneyAtferTrade;
                        if (minValue > waitingTradeDatas[i].moneyAtferTrade)
                            minValue = waitingTradeDatas[i].moneyAtferTrade;
                        historyTradeDatas.Add(waitingTradeDatas[i]);
                        waitingTradeDatas.RemoveAt(i);
                    }
                }
            }
            UpdateAutoTrade();
        }

        public void SetTradeCountInfo(string info)
        {
            string[] nums = info.Split(',');
            tradeCountList.Clear();
            for( int i = 0; i < nums.Length; ++i )
            {
                tradeCountList.Add(int.Parse(nums[i]));
            }
        }
        public string GetTradeCountInfoStr()
        {
            string info = "";
            for (int i = 0; i < tradeCountList.Count; ++i)
            {
                info += tradeCountList[i];
                if (i != tradeCountList.Count - 1)
                    info += ",";
            }
            return info;
        }

        public enum StartTradeType
        {
            eFromFirst,
            eFromLatest,
            eFromSpec,
        }
        public void StartAutoTradeJob(StartTradeType srt, string idTag)
        {
            ClearAllTradeDatas();
            if (srt == StartTradeType.eFromFirst)
                curTestTradeItem = DataManager.GetInst().GetFirstItem();
            else if (srt == StartTradeType.eFromLatest)
                curTestTradeItem = DataManager.GetInst().GetLatestItem();
            else if (srt == StartTradeType.eFromSpec)
                curTestTradeItem = DataManager.GetInst().GetDataItemByIdTag(idTag);
            pauseAutoTrade = false;
            hasCompleted = false;
        }
        public void StopAutoTradeJob()
        {
            curTestTradeItem = null;
            hasCompleted = true;
        }
        public void PauseAutoTradeJob()
        {
            pauseAutoTrade = true;
        }
        public void ResumeAutoTradeJob()
        {
            pauseAutoTrade = false;
        }
        public void ClearAllTradeDatas()
        {
            waitingTradeDatas.Clear();
            historyTradeDatas.Clear();
            pauseAutoTrade = true;
            needGetLatestItem = false;
            curTestTradeItem = null;
            currentTradeCountIndex = -1;
            currentMoney = startMoney;
            minValue = startMoney;
            maxValue = startMoney;
            rightCount = 0;
            wrongCount = 0;
            untradeCount = 0;
    }
        void UpdateAutoTrade()
        {
            if (hasCompleted)
                return;
            if (pauseAutoTrade)
                return;
            if (curTestTradeItem == null)
                return;
            if (waitingTradeDatas.Count > 0)
                return;
            if (stopAtTheLatestItem && curTestTradeItem == DataManager.GetInst().GetLatestItem())
            {
                hasCompleted = true;
                return;
            }
            if (needGetLatestItem)
            {
                curTestTradeItem = curTestTradeItem.parent.GetNextItem(curTestTradeItem);
                needGetLatestItem = false;
            }
            PredictAndTrade(curTestTradeItem);
            if (curTestTradeItem == DataManager.GetInst().GetLatestItem())
                needGetLatestItem = true;
            else
                curTestTradeItem = curTestTradeItem.parent.GetNextItem(curTestTradeItem);
        }
        /*
        public void SimTrade()
        {
            DataItem curItem = DataManager.GetInst().GetFirstItem();

            if (simTradeFromFirstEveryTime)
            {
                waitingTradeDatas.Clear();
                historyTradeDatas.Clear();
            }
            else
            {
                DataItem latestItem = DataManager.GetInst().GetLatestItem();
                if (latestItem == curTestTradeItem)
                    return;
                if (curTestTradeItem != null)
                    curItem = curTestTradeItem;
            }

            while(curItem != null)
            {
                PredictAndTrade(curItem);
                if (curItem != null)
                    curTestTradeItem = curItem;
                curItem = curItem.parent.GetNextItem(curItem);
            }
        }
        */
        void PredictAndTrade(DataItem item)
        {
            float maxV = -10;
            int bestNumIndex = -1;
            int bestPath = -1;
            if (simSelNumIndex == -1)
            {
                for (int i = 0; i < 5; ++i)
                {
                    JudgeNumberPath(item, i, ref maxV, ref bestNumIndex, ref bestPath);
                }
            }
            else
            {
                JudgeNumberPath(item, simSelNumIndex, ref maxV, ref bestNumIndex, ref bestPath);
            }
            TradeDataOneStar trade = TradeDataManager.Instance.NewTrade(TradeType.eOneStar) as TradeDataOneStar;
            trade.lastDateItem = item;
            trade.isAutoTrade = true;
            int tradeCount = defaultTradeCount;
            if (item.idGlobal >= LotteryStatisticInfo.SHOR_COUNT)
            {
                if (tradeCountList.Count > 0)
                {
                    if (currentTradeCountIndex == -1)
                        currentTradeCountIndex = 0;
                    tradeCount = tradeCountList[currentTradeCountIndex];
                }
            }
            else
                tradeCount = 0;
            if (bestNumIndex >=0 && bestPath >= 0)
            {
                TradeNumbers tn = new TradeNumbers();
                tn.tradeCount = tradeCount;
                FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);
                /*
                for (int i = 0; i < maxProbilityNums.Count; ++i)
                {
                    if (maxProbilityNums[i].largerThanTheoryProbability)
                        tn.tradeNumbers.Add(maxProbilityNums[i]);
                    //if (tn.tradeNumbers.Count == 5)
                    //    break;
                }
                */
                tn.SelPath012Number(bestPath, tradeCount, ref maxProbilityNums);
                trade.tradeInfo.Add(bestNumIndex, tn);
            }
        }

        bool isContinuousMiss(DataItem item, int numIndex, int pathId, ref int value)
        {
            bool hasMiss = true;
            value = 1;
            int count = 3;
            DataItem curItem = item;
            while(curItem != null && count > 0)
            {
                if (curItem.path012OfEachSingle[numIndex] == pathId)
                {
                    hasMiss = false;
                    value *= 2;
                }
                --count;
                curItem = curItem.parent.GetPrevItem(curItem);
            }
            if (hasMiss)
                value = 0;
            return hasMiss;
        }

        void Check(StatisticUnit suA, StatisticUnit suB, int pathValueA, int pathValueB, int indexA, int indexB, ref int curBestV, ref int curBestPath, ref StatisticUnit curBeshSU)
        {
            if (pathValueA > pathValueB)
            {
                curBestPath = indexA;
                curBestV = pathValueA;
                curBeshSU = suA;
            }
            else if (pathValueA < pathValueB)
            {
                curBestPath = indexB;
                curBestV = pathValueB;
                curBeshSU = suB;
            }
            else
            {
                if (suA.appearProbabilityShort > suB.appearProbabilityShort)
                {
                    curBestPath = indexA;
                    curBestV = pathValueA;
                    curBeshSU = suA;
                }
                else if (suA.appearProbabilityShort < suB.appearProbabilityShort)
                {
                    curBestPath = indexB;
                    curBestV = pathValueB;
                    curBeshSU = suB;
                }
                else
                {
                    if (suA.appearProbabilityLong > suB.appearProbabilityLong)
                    {
                        curBestPath = indexA;
                        curBestV = pathValueA;
                        curBeshSU = suA;
                    }
                    else if (suA.appearProbabilityLong < suB.appearProbabilityLong)
                    {
                        curBestPath = indexB;
                        curBestV = pathValueB;
                        curBeshSU = suB;
                    }
                }
            }

        }

        void JudgeNumberPath(DataItem item, int numIndex, ref float maxV, ref int bestNumIndex, ref int bestPath)
        {
            KGraphDataContainer kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataDict kdd = kddc.GetKDataDict(item);
            AvgPointMap apm5 = kddc.GetAvgPointMap(5, kdd);
            AvgPointMap apm10 = kddc.GetAvgPointMap(10, kdd);
            BollinPointMap bpm = kddc.GetBollinPointMap(kdd);
            int path0Value = 1, path1Value = 1, path2Value = 1;
            float path0Avg5 = apm5.GetData(CollectDataType.ePath0, false).avgKValue;
            float path1Avg5 = apm5.GetData(CollectDataType.ePath1, false).avgKValue;
            float path2Avg5 = apm5.GetData(CollectDataType.ePath2, false).avgKValue;
            float path0Avg10 = apm10.GetData(CollectDataType.ePath0, false).avgKValue;
            float path1Avg10 = apm10.GetData(CollectDataType.ePath1, false).avgKValue;
            float path2Avg10 = apm10.GetData(CollectDataType.ePath2, false).avgKValue;
            float path0Bpm = bpm.GetData(CollectDataType.ePath0, false).midValue;
            float path1Bpm = bpm.GetData(CollectDataType.ePath1, false).midValue;
            float path2Bpm = bpm.GetData(CollectDataType.ePath2, false).midValue;
            bool isPath0OK = false;
            bool isPath1OK = false;
            bool isPath2OK = false;
            int missValue0 = 1, missValue1 = 1, missValue2 = 1;
            bool isPath0ContinueMiss = isContinuousMiss(item, numIndex, 0, ref missValue0);
            bool isPath1ContinueMiss = isContinuousMiss(item, numIndex, 1, ref missValue1);
            bool isPath2ContinueMiss = isContinuousMiss(item, numIndex, 2, ref missValue2);
            path0Value *= missValue0;
            path1Value *= missValue1;
            path2Value *= missValue2;
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            StatisticUnit su0 = sum.statisticUnitMap[CollectDataType.ePath0];
            StatisticUnit su1 = sum.statisticUnitMap[CollectDataType.ePath1];
            StatisticUnit su2 = sum.statisticUnitMap[CollectDataType.ePath2];
            StatisticUnit curBestSU = null;
            /*
            if (
                path0Avg5 > path0Bpm &&
                path0Avg10 > path0Bpm &&
                !isPath0ContinueMiss)
                isPath0OK = true;
            if (
                path1Avg5 > path1Bpm &&
                path1Avg10 > path1Bpm &&
                !isPath1ContinueMiss)
                isPath1OK = true;
            if (
                path2Avg5 > path2Bpm &&
                path2Avg10 > path2Bpm &&
                !isPath2ContinueMiss)
                isPath2OK = true;
            bool isPath0GoUp = false;
            bool isPath1GoUp = false;
            bool isPath2GoUp = false;
            int loop = 5;
            DataItem startItem = item;
            while (loop > 0 && startItem != null)
            {
                startItem = startItem.parent.GetPrevItem(startItem);
                --loop;
            }
            if(loop == 0 && startItem != null)
            {
                KDataDict kddStart = kddc.GetKDataDict(startItem);
                isPath0GoUp = kddStart.GetData(CollectDataType.ePath0, false).KValue < kdd.GetData(CollectDataType.ePath0, false).KValue;
                isPath1GoUp = kddStart.GetData(CollectDataType.ePath1, false).KValue < kdd.GetData(CollectDataType.ePath1, false).KValue;
                isPath2GoUp = kddStart.GetData(CollectDataType.ePath2, false).KValue < kdd.GetData(CollectDataType.ePath2, false).KValue;
            }
            */
            if (path0Avg5 > path0Bpm) path0Value *= 2;
            if (path0Avg10 > path0Bpm) path0Value *= 2;
            if (path0Avg5 > path0Avg10) path0Value *= 2;
            if (path1Avg5 > path1Bpm) path1Value *= 2;
            if (path1Avg10 > path1Bpm) path1Value *= 2;
            if (path1Avg5 > path1Avg10) path1Value *= 2;
            if (path2Avg5 > path2Bpm) path2Value *= 2;
            if (path2Avg10 > path2Bpm) path2Value *= 2;
            if (path2Avg5 > path2Avg10) path2Value *= 2;
            int curBestV = 0;
            int curBestPath = -1;
            if(path0Value > path1Value)
                Check(su0, su2, path0Value, path2Value, 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
            else if(path0Value < path1Value)
                Check(su1, su2, path1Value, path2Value, 1, 2, ref curBestV, ref curBestPath, ref curBestSU);
            else
            { 
                if(su0.appearProbabilityShort > su1.appearProbabilityShort)
                    Check(su0, su2, path0Value, path2Value, 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
                else if(su0.appearProbabilityShort < su1.appearProbabilityShort)
                    Check(su1, su2, path1Value, path2Value, 1, 2, ref curBestV, ref curBestPath, ref curBestSU);
                else
                {
                    if (su0.appearProbabilityLong > su1.appearProbabilityLong)
                        Check(su0, su2, path0Value, path2Value, 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
                    else if (su0.appearProbabilityLong < su1.appearProbabilityLong)
                        Check(su1, su2, path1Value, path2Value, 1, 2, ref curBestV, ref curBestPath, ref curBestSU);
                }
            }
            if(curBestPath != -1 && curBestV > 0)
            {
                if(curBestV > maxV)
                {
                    bestPath = curBestPath;
                    bestNumIndex = numIndex;
                    maxV = curBestV;
                }
                else if(curBestV == maxV)
                {
                    StatisticUnitMap sumBest = item.statisticInfo.allStatisticInfo[bestNumIndex];
                    CollectDataType curCDT = (CollectDataType)(1 << curBestPath);
                    StatisticUnit suBest = sum.statisticUnitMap[curCDT];
                    if(suBest.appearProbabilityShort < curBestSU.appearProbabilityShort)
                    {
                        bestPath = curBestPath;
                        bestNumIndex = numIndex;
                        maxV = curBestV;
                    }
                    else if(suBest.appearProbabilityShort == curBestSU.appearProbabilityShort)
                    {
                        if (suBest.appearProbabilityLong < curBestSU.appearProbabilityLong)
                        {
                            bestPath = curBestPath;
                            bestNumIndex = numIndex;
                            maxV = curBestV;
                        }
                    }
                }
            }

            /*
            if (!isPath0OK && !isPath1OK && !isPath2OK)
                return;
            else if(isPath0OK && !isPath1OK && !isPath2OK)
            {
                if (isPath0GoUp)
                {
                    bestNumIndex = numIndex;
                    bestPath = 0;
                }
            }
            else if (!isPath0OK && isPath1OK && !isPath2OK)
            {
                if (isPath1GoUp)
                {
                    bestNumIndex = numIndex;
                    bestPath = 1;
                }
            }
            else if (!isPath0OK && !isPath1OK && isPath2OK)
            {
                if (isPath2GoUp)
                {
                    bestNumIndex = numIndex;
                    bestPath = 2;
                }
            }
            else if (isPath0OK && isPath1OK && !isPath2OK)
            {
                bestNumIndex = numIndex;
                if (isPath0GoUp && !isPath1GoUp)
                    bestPath = 0;
                else if (!isPath0GoUp && isPath1GoUp)
                    bestPath = 1;
                else if (isPath0GoUp && isPath1GoUp)
                {
                    if (su0.appearProbabilityShort > su1.appearProbabilityShort)
                        bestPath = 0;
                    else if (su0.appearProbabilityShort < su1.appearProbabilityShort)
                        bestPath = 1;
                    else
                    {
                        if (su0.appearProbabilityLong > su1.appearProbabilityLong)
                            bestPath = 0;
                        else if (su0.appearProbabilityLong < su1.appearProbabilityLong)
                            bestPath = 1;
                    }
                }
            }
            else if (!isPath0OK && isPath1OK && isPath2OK)
            {
                bestNumIndex = numIndex;
                if (isPath1GoUp && !isPath2GoUp)
                    bestPath = 1;
                else if (!isPath1GoUp && isPath2GoUp)
                    bestPath = 2;
                else if (isPath1GoUp && isPath2GoUp)
                {
                    if (su1.appearProbabilityShort > su2.appearProbabilityShort)
                        bestPath = 1;
                    else if (su1.appearProbabilityShort < su2.appearProbabilityShort)
                        bestPath = 2;
                    else
                    {
                        if (su1.appearProbabilityLong > su2.appearProbabilityLong)
                            bestPath = 1;
                        else if (su1.appearProbabilityLong < su2.appearProbabilityLong)
                            bestPath = 2;
                    }
                }
            }
            else if (isPath0OK && !isPath1OK && isPath2OK)
            {
                bestNumIndex = numIndex;
                if (isPath0GoUp && !isPath2GoUp)
                    bestPath = 0;
                else if (!isPath0GoUp && isPath2GoUp)
                    bestPath = 2;
                else if (isPath1GoUp && isPath2GoUp)
                {
                    if (su0.appearProbabilityShort > su2.appearProbabilityShort)
                        bestPath = 0;
                    else if (su0.appearProbabilityShort < su2.appearProbabilityShort)
                        bestPath = 2;
                    else
                    {
                        if (su0.appearProbabilityLong > su2.appearProbabilityLong)
                            bestPath = 0;
                        else if (su0.appearProbabilityLong < su2.appearProbabilityLong)
                            bestPath = 2;
                    }
                }
            }
            else
            {
                bestNumIndex = numIndex;
                if (isPath0GoUp && !isPath1GoUp && !isPath2GoUp)
                    bestPath = 0;
                else if (!isPath0GoUp && isPath1GoUp && !isPath2GoUp)
                    bestPath = 1;
                else if (!isPath0GoUp && !isPath1GoUp && isPath2GoUp)
                    bestPath = 2;
                else if (isPath0GoUp && isPath1GoUp && !isPath2GoUp)
                {
                    if (su0.appearProbabilityShort > su1.appearProbabilityShort)
                        bestPath = 0;
                    else
                        bestPath = 1;
                }
                else if (!isPath0GoUp && isPath1GoUp && isPath2GoUp)
                {
                    if (su1.appearProbabilityShort > su2.appearProbabilityShort)
                        bestPath = 1;
                    else
                        bestPath = 2;
                }
                else if (isPath0GoUp && !isPath1GoUp && isPath2GoUp)
                {
                    if (su0.appearProbabilityShort > su2.appearProbabilityShort)
                        bestPath = 0;
                    else
                        bestPath = 2;
                }
                else
                {
                    if (su0.appearProbabilityShort > su2.appearProbabilityShort)
                    {
                        if (su0.appearProbabilityShort > su1.appearProbabilityShort)
                            bestPath = 0;
                        else
                            bestPath = 1;
                    }
                    else
                    {
                        if (su2.appearProbabilityShort > su1.appearProbabilityShort)
                            bestPath = 2;
                        else
                            bestPath = 1;
                    }
                }
            }
            */

            /*
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            StatisticUnit su0 = sum.statisticUnitMap[CollectDataType.ePath0];
            StatisticUnit su1 = sum.statisticUnitMap[CollectDataType.ePath1];
            StatisticUnit su2 = sum.statisticUnitMap[CollectDataType.ePath2];
            float v0 = su0.appearProbabilityLong;// * su0.appearProbabilityShort;
            float v1 = su1.appearProbabilityLong;// * su1.appearProbabilityShort;
            float v2 = su2.appearProbabilityLong;// * su2.appearProbabilityShort;
            float vm, dm;
            StatisticUnit maxSU;
            if (v0 > v1)
            {
                dm = su0.appearProbabilityDiffWithTheoryShort;
                vm = v0;
                maxSU = su0;
            }
            else if(v0 < v1)
            {
                dm = su1.appearProbabilityDiffWithTheoryShort;
                vm = v1;
                maxSU = su1;
            }
            else
            {
                if(su0.appearProbabilityDiffWithTheoryShort > su1.appearProbabilityDiffWithTheoryShort)
                {
                    dm = su0.appearProbabilityDiffWithTheoryShort;
                    vm = v0;
                    maxSU = su0;
                }
                else
                {
                    dm = su1.appearProbabilityDiffWithTheoryShort;
                    vm = v1;
                    maxSU = su1;
                }
            }
            if (v2 > vm)
            {
                maxSU = su2;
            }
            else if(su2.appearProbabilityDiffWithTheoryShort > dm)
            {
                maxSU = su2;
            }

            if (maxSU.appearProbabilityShort > maxV)
            {
                if (maxSU.cdt == CollectDataType.ePath0)
                    bestPath = 0;
                else if (maxSU.cdt == CollectDataType.ePath1)
                    bestPath = 1;
                else
                    bestPath = 2;
                bestNumIndex = numIndex;
                maxV = maxSU.appearProbabilityShort;
            }
            */

            /*
            KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
            BollinPointMap bpm = kddc.bollinDataLst.bollinMapLst[item.idGlobal];
            KDataDict kdd = kddc.dataLst[item.idGlobal];
            KData kdPath0 = kdd.dataDict[CollectDataType.ePath0];
            KData kdPath1 = kdd.dataDict[CollectDataType.ePath1];
            KData kdPath2 = kdd.dataDict[CollectDataType.ePath2];
            BollinPoint bmPath0 = bpm.bpMap[CollectDataType.ePath0];
            BollinPoint bmPath1 = bpm.bpMap[CollectDataType.ePath1];
            BollinPoint bmPath2 = bpm.bpMap[CollectDataType.ePath2];
            float d0 = (kdPath0.KValue - bmPath0.midValue) / bmPath0.standardDeviation * 0.5f;
            float d1 = (kdPath1.KValue - bmPath1.midValue) / bmPath1.standardDeviation * 0.5f;
            float d2 = (kdPath2.KValue - bmPath2.midValue) / bmPath2.standardDeviation * 0.5f;
            float tmpMaxV = Math.Max(d0, Math.Max(d1, d2));
            if (tmpMaxV > maxV)
            {
                maxV = tmpMaxV;
                bestNumIndex = numIndex;
                if (tmpMaxV == d0)
                    bestPath = 0;
                else if (tmpMaxV == d1)
                    bestPath = 1;
                else
                    bestPath = 2;
            }
            */
        }


        public static void FindOverTheoryProbabilityNums(DataItem item, int numIndex, ref List<NumberCmpInfo> nums)
        {
            nums.Clear();
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            int startIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum0);
            int endIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum9);
            for (int i = startIndex; i <= endIndex; ++i)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
                {
                    NumberCmpInfo info = new NumberCmpInfo();
                    info.number = (SByte)(i - startIndex);
                    info.rate = sum.statisticUnitMap[cdt].appearProbabilityLong;
                    info.largerThanTheoryProbability = sum.statisticUnitMap[cdt].appearProbabilityLong > GraphDataManager.GetTheoryProbability(cdt);

                    if (nums.Count == 0)
                    {
                        nums.Add(info);
                    }
                    else
                    {
                        bool hasInsert = false;
                        for( int j = 0; j < nums.Count; ++j )
                        {
                            if(sum.statisticUnitMap[cdt].appearProbabilityLong > nums[j].rate)
                            {
                                nums.Insert(j, info);
                                hasInsert = true;
                                break;
                            }
                        }
                        if(hasInsert == false)
                        {
                            nums.Add(info);
                        }
                    }
                }
            }
        }
    }

    class BatchTradeSimulator
    {
        enum SimState
        {
            eNone,
            ePrepareData,
            eSimTrade,
            eSimPause,
            eFinishBatch,
            eFinishAll,
        }

        static BatchTradeSimulator sInst;
        public static BatchTradeSimulator Instance
        {
            get
            {
                if (sInst == null)
                    sInst = new BatchTradeSimulator();
                return sInst;
            }
        }

        List<int> fileIDLst = new List<int>();
        int lastIndex = -1;        
        SimState state = SimState.eNone;
        string lastTradeIDTag = null;
        DataItem curTradeItem;
        SimState backUpState = SimState.eNone;

        public int batch = 5;
        public float currentMoney;
        public float startMoney = 2000;
        public float minMoney;
        public float maxMoney;
        public int totalCount;
        public int tradeRightCount;
        public int tradeWrongCount;
        public int untradeCount;

        public int GetMainProgress()
        {
            if (state == SimState.eFinishAll)
                return 100;
            else if (lastIndex < 0 || fileIDLst.Count == 0)
                return 0;
            else
                return (lastIndex * 100 / fileIDLst.Count);
        }
        public int GetBatchProgress()
        {
            if (state == SimState.ePrepareData)
                return 0;
            else if (state == SimState.eFinishBatch || state == SimState.eFinishAll)
                return 100;
            int totalItemCount = DataManager.GetInst().GetAllDataItemCount();
            if (totalItemCount == 0)
                return 0;
            int v = TradeDataManager.Instance.historyTradeDatas.Count * 100 / totalItemCount;
            return v;
        }

        public void Start()
        {
            TradeDataManager.Instance.startMoney = startMoney;
            TradeDataManager.Instance.StopAtTheLatestItem = true;
            minMoney = maxMoney = currentMoney = startMoney;
            totalCount = tradeRightCount = tradeWrongCount = untradeCount = 0;

            fileIDLst.Clear();
            DataManager dm = DataManager.GetInst();
            foreach( int id in dm.mFileMetaInfo.Keys )
            {
                fileIDLst.Add(id);
            }
            fileIDLst.Sort();
            state = SimState.ePrepareData;
            lastIndex = -1;
        }

        public void Update()
        {
            switch (state)
            {
                case SimState.ePrepareData:
                    DoPrepareData();
                    break;
                case SimState.eSimTrade:
                    DoSimTrade();
                    break;
                case SimState.eFinishBatch:
                    DoFinishBatch();
                    break;
            }
        }

        public bool IsPause()
        {
            return state == SimState.eSimPause;
        }

        public void Pause()
        {
            backUpState = state;
            state = SimState.eSimPause;
            TradeDataManager.Instance.PauseAutoTradeJob();
        }
        public void Resume()
        {
            state = backUpState;
            TradeDataManager.Instance.ResumeAutoTradeJob();
        }
        public void Stop()
        {
            TradeDataManager.Instance.StopAutoTradeJob();
            DoFinishBatch();
            state = SimState.eFinishAll;
            lastIndex = fileIDLst.Count;
        }
        public bool HasFinished()
        {
            return state == SimState.eFinishAll;
        }
        public bool HasJob()
        {
            return fileIDLst.Count > 0 && lastIndex < fileIDLst.Count;
        }

        void DoPrepareData()
        {
            if (fileIDLst.Count > lastIndex)
            {
                if (lastIndex == -1)
                    lastIndex = 0;
                DataManager dataMgr = DataManager.GetInst();
                dataMgr.ClearAllDatas();
                int startIndex = lastIndex;
                int endIndex = lastIndex + batch;
                if (endIndex >= fileIDLst.Count)
                {
                    endIndex = fileIDLst.Count - 1;
                    lastIndex = fileIDLst.Count;
                }
                else
                    lastIndex = endIndex - 1;

                for (int i = startIndex; i <= endIndex; ++i)
                {
                    int key = fileIDLst[i];
                    dataMgr.LoadData(key);
                }
                dataMgr.SetDataItemsGlobalID();
                if (dataMgr.GetAllDataItemCount() == 0)
                    return;
                Util.CollectPath012Info(null);
                GraphDataManager.Instance.CollectGraphData(GraphType.eKCurveGraph);

                TradeDataManager.Instance.startMoney = currentMoney;
                if (string.IsNullOrEmpty(lastTradeIDTag))
                    TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromFirst, lastTradeIDTag);
                else
                    TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromSpec, lastTradeIDTag);
                state = SimState.eSimTrade;
            }
            else
                state = SimState.eFinishAll;
        }
        void DoSimTrade()
        {
            currentMoney = TradeDataManager.Instance.currentMoney;
            if (maxMoney < currentMoney)
                maxMoney = currentMoney;
            if (minMoney > currentMoney)
                minMoney = currentMoney;
            if (TradeDataManager.Instance.hasCompleted == true)
                state = SimState.eFinishBatch;
        }
        void DoFinishBatch()
        {
            currentMoney = TradeDataManager.Instance.currentMoney;
            tradeRightCount += TradeDataManager.Instance.rightCount;
            tradeWrongCount += TradeDataManager.Instance.wrongCount;
            untradeCount += TradeDataManager.Instance.untradeCount;
            totalCount += TradeDataManager.Instance.rightCount + TradeDataManager.Instance.wrongCount + TradeDataManager.Instance.untradeCount;
            state = SimState.ePrepareData;
        }
    }
}
