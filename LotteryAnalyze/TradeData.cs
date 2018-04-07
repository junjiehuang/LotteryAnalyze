﻿#define TRADE_DBG


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public enum TradeType
    {
        eNone,
        // 1星
        eOneStar,
        // 十位个位2星
        eTenSingleTwoStar,
        // 百位十位2星
        eHundredTenTwoStar,
        // 千位百位2星
        eThousandHundredTwoStar,
        // 万位千位2星
        eTenThousandHundredTwoStar,
        // 百十个位3星
        eHundredTenSingleThreeStar,
        // 千百十位3星
        eThousandHundredTenThreeStart,
        // 万千百位3星
        eTenThousandThousandHundredThreeStart,
        // 5星
        eFiveStar,
    }

    public struct PathCmpInfo
    {
        public int pathIndex;
        public float pathValue;
        public PathCmpInfo(int id, float v)
        {
            pathIndex = id;
            pathValue = v;
        }
    }

    public enum TradeStatus
    {
        // 等待状态
        eWaiting,
        // 交易完成状态
        eDone,
    }

    class NumberCmpInfo
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
            for(int i = 0; i < nums.Count; ++i )
            {
                if (nums[i].number == number)
                    return i;
            }
            if(createIfNotExist)
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
            if (a.appearCount < b.appearCount)
                return 1;
            return -1;
        }
        public static int SortByNumber(NumberCmpInfo a, NumberCmpInfo b)
        {
            if (a == null || b == null)
                return 0;
            if (a.number > b.number)
                return 1;
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
        public void SetMaxProbabilityNumber(int tradeCount, ref List<NumberCmpInfo> nums, bool needGetLessProbabilityNum, int maxNumCount)
        {
            this.tradeCount = tradeCount;
            int count = 0;
            for( int i = 0; i < nums.Count; ++i )
            {
                if (nums[i].largerThanTheoryProbability || needGetLessProbabilityNum)
                {
                    ++count;
                    tradeNumbers.Add(nums[i]);
                    if (count == maxNumCount)
                        break;
                }
            }
        }
    }

    // 基本交易数据
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
        //protected string tips = "";
        public bool isAutoTrade = false;

        public virtual string GetTips()
        {
            return "";
            //return tips;
        }
        public virtual string GetDbgInfo() { return ""; }
        public virtual void Update() { }
        public virtual void GetTradeNumIndexAndPathIndex(ref int numIndex, ref int pathIndex) { }
    }

    // 一星交易数据
    class TradeDataOneStar : TradeDataBase
    {
        public static float SingleTradeCost = 1;
        public static float SingleTradeReward = 9.8f;

        public Dictionary<int, TradeNumbers> tradeInfo = new Dictionary<int, TradeNumbers>();

#if TRADE_DBG
        public List<List<PathCmpInfo>> pathCmpInfos = new List<List<PathCmpInfo>>();
#endif

        public TradeDataOneStar()
        {
#if TRADE_DBG
            for(int i = 0; i < 5; ++i)
            {
                pathCmpInfos.Add(new List<PathCmpInfo>());
            }
#endif
            tradeType = TradeType.eOneStar;
        }

        public override string GetDbgInfo()
        {
#if TRADE_DBG
            string dbgtxt = "";
            for(int i = 0; i < pathCmpInfos.Count; ++i)
            {
                if(pathCmpInfos[i].Count > 0)
                {
                    for(int j = 0; j < pathCmpInfos[i].Count; ++j)
                    {
                        dbgtxt += "[" + pathCmpInfos[i][j].pathIndex + " = " + pathCmpInfos[i][j].pathValue + "]\n";
                    }
                }
            }
            return dbgtxt;
#endif
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
                    int index = NumberCmpInfo.FindIndex(nums, selNums[i],false);
                    if (index != -1)
                    {
                        tn.tradeNumbers.Add(nums[index]);
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
            string tips = "";
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

        public override void GetTradeNumIndexAndPathIndex(ref int numIndex, ref int pathIndex)
        {
            numIndex = -1;
            pathIndex = -1;
            foreach ( int key in tradeInfo.Keys )
            {
                if(tradeInfo[key].tradeCount > 0 && tradeInfo[key].tradeNumbers.Count > 0)
                {
                    numIndex = key;
                    pathIndex = tradeInfo[key].tradeNumbers[0].number % 3;
                }
            }
        }
    }

    // 交易数据管理器
    class TradeDataManager
    {
        public const int LOOP_COUNT = 10;

        // 交易策略
        public enum TradeStrategy
        {
            // 选择最优的某个数值位的某个012路
            eSingleBestPath,
            // 选择某个数值位的最优的某2个012路
            eSingleBestTwoPath,
            // 只要哪个数字位的最优012路满足就进行交易
            eMultiNumPath,
            // 选择某个数字位连续N期出号概率最高的几个数
            eSingleMostPosibilityNums,
            // 所有数字位都选择连续N期出号概率最高的几个数
            eMultiMostPosibilityNums,

        }
        public static string[] STRATEGY_NAMES = {
            "eSingleBestPath",
            "eSingleBestTwoPath",
            "eMultiNumPath",
            "eSingleMostPosibilityNums",
            "eMultiMostPosibilityNums",
        };

        // 是否强制每次交易都取指定的最大的数字个数
        public bool forceTradeByMaxNumCount = false;
        // 单次交易每个数字位投注的个数的最大值
        public int maxNumCount = 5;

        public TradeStrategy curTradeStrategy = TradeStrategy.eSingleBestPath;
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
        bool tradeOneStep = false;
        public bool StopAtTheLatestItem
        {
            get { return stopAtTheLatestItem; }
            set { stopAtTheLatestItem = value; }
        }
        List<NumberCmpInfo> maxProbilityNums = new List<NumberCmpInfo>();
        public delegate void OnTradeComleted();
        public OnTradeComleted tradeCompletedCallBack;

        public AutoAnalyzeTool autoAnalyzeTool = new AutoAnalyzeTool();
        public AutoAnalyzeTool curPreviewAnalyzeTool = new AutoAnalyzeTool();

        TradeDataManager()
        {
            minValue = maxValue = currentMoney;
            tradeCountList.Add(1);
            tradeCountList.Add(2);
            tradeCountList.Add(4);
            tradeCountList.Add(8);
            tradeCountList.Add(16);
            ClearAllTradeDatas();
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
                        if (tradeCompletedCallBack != null)
                            tradeCompletedCallBack();
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
                if(string.IsNullOrEmpty(nums[i]) == false)
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
        public void SetStartMoney(float _startMoney)
        {
            startMoney = _startMoney;
            if(historyTradeDatas.Count == 0 && waitingTradeDatas.Count == 0)
            {
                minValue = maxValue = currentMoney = startMoney;
            }
        }
        public DataItem StartAutoTradeJob(StartTradeType srt, string idTag)
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
            return curTestTradeItem;
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
        public bool IsPause()
        {
            return pauseAutoTrade;
        }
        public bool IsCompleted()
        {
            return hasCompleted;
        }
        public void ResumeAutoTradeJob()
        {
            pauseAutoTrade = false;
        }
        public void SimTradeOneStep()
        {
            tradeOneStep = true;
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
            {
                if (tradeOneStep == false)
                    return;
                else
                    tradeOneStep = false;
            }
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

        /// <summary>
        /// 针对当前的DataItem进行预测研判，并决定交易细节
        /// </summary>
        /// <param name="item"></param>
        void PredictAndTrade(DataItem item)
        {
            // 自动计算辅助线
            autoAnalyzeTool.Analyze(item.idGlobal);

            TradeDataOneStar trade = TradeDataManager.Instance.NewTrade(TradeType.eOneStar) as TradeDataOneStar;
            trade.lastDateItem = item;
            trade.isAutoTrade = true;
            trade.tradeInfo.Clear();

            switch (curTradeStrategy)
            {
                case TradeStrategy.eSingleBestPath:
                    TradeSingleBestPath(item, trade);
                    break;
                case TradeStrategy.eSingleBestTwoPath:
                    TradeSingleBestTwoPath(item, trade);
                    break;
                case TradeStrategy.eMultiNumPath:
                    TradeMultiNumPath(item, trade);
                    break;
                case TradeStrategy.eSingleMostPosibilityNums:
                    TradeSingleMostPosibilityNums(item, trade);
                    break;
                case TradeStrategy.eMultiMostPosibilityNums:
                    TradeMultiMostPosibilityNums(item, trade);
                    break;
            }
        }

        void TradeSingleBestPath(DataItem item, TradeDataOneStar trade)
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
            if (bestNumIndex >= 0 && bestPath >= 0)
            {
                TradeNumbers tn = new TradeNumbers();
                tn.tradeCount = tradeCount;
                FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);
                tn.SelPath012Number(bestPath, tradeCount, ref maxProbilityNums);
                trade.tradeInfo.Add(bestNumIndex, tn);
            }
        }

        void TradeSingleBestTwoPath(DataItem item, TradeDataOneStar trade)
        {
            int bestNumIndex = 0;
            if (simSelNumIndex != -1)
                bestNumIndex = simSelNumIndex;
            List<PathCmpInfo> res = trade.pathCmpInfos[bestNumIndex];
            SortNumberPath(item, bestNumIndex, ref res);

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

            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            trade.tradeInfo.Add(bestNumIndex, tn);
            FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);

            for ( int i = 0; i < 2; ++i )
            {
                PathCmpInfo pci = res[i];
                if(pci.pathValue > 0)
                    tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
            }
        }

        void TradeMultiNumPath(DataItem item, TradeDataOneStar trade)
        {
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

            for (int i = 0; i < 5; ++i)
            {
                float maxV = -10;
                int bestNumIndex = -1;
                int bestPath = -1;
                JudgeNumberPath(item, i, ref maxV, ref bestNumIndex, ref bestPath);
                if (bestNumIndex >= 0 && bestPath >= 0)
                {
                    TradeNumbers tn = new TradeNumbers();
                    tn.tradeCount = tradeCount;
                    FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);
                    tn.SelPath012Number(bestPath, tradeCount, ref maxProbilityNums);
                    trade.tradeInfo.Add(bestNumIndex, tn);
                }
            }
        }

        void TradeSingleMostPosibilityNums(DataItem item, TradeDataOneStar trade)
        {
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
            int numID = 0;
            if(simSelNumIndex != -1)
                numID = simSelNumIndex;
            bool needGetLessProbabilityNum = forceTradeByMaxNumCount || (tradeCountList.Count > 5 && currentTradeCountIndex > 4);
            FindAllNumberProbabilities(item, ref maxProbilityNums);
            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            tn.SetMaxProbabilityNumber(tradeCount, ref maxProbilityNums, needGetLessProbabilityNum, maxNumCount);
            trade.tradeInfo.Add(numID, tn);
        }

        void TradeMultiMostPosibilityNums(DataItem item, TradeDataOneStar trade)
        {
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
            bool needGetLessProbabilityNum = forceTradeByMaxNumCount || (tradeCountList.Count > 5 && currentTradeCountIndex > 4);
            FindAllNumberProbabilities(item, ref maxProbilityNums);
            for (int i = 0; i < 5; ++i)
            {
                TradeNumbers tn = new TradeNumbers();
                tn.tradeCount = tradeCount;
                tn.SetMaxProbabilityNumber(tradeCount, ref maxProbilityNums, needGetLessProbabilityNum, maxNumCount);
                trade.tradeInfo.Add(i, tn);
            }
        }

        /// <summary>
        /// 判断numIndex位是否连续n期没有出pathId路的数字
        /// </summary>
        /// <param name="item"></param>
        /// <param name="numIndex"></param>
        /// <param name="pathId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 比较同一个数字位的012路的任意两路，哪一路在下一期出现的概率更高一些
        /// </summary>
        /// <param name="suA"></param>
        /// <param name="suB"></param>
        /// <param name="pathValueA"></param>
        /// <param name="pathValueB"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        /// <param name="curBestV"></param>
        /// <param name="curBestPath"></param>
        /// <param name="curBeshSU"></param>
        void Check(StatisticUnit suA, StatisticUnit suB, float pathValueA, float pathValueB, int indexA, int indexB, ref float curBestV, ref int curBestPath, ref StatisticUnit curBeshSU)
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

        public enum ValueCmpState
        {
            eNone,
            eDifGreaterDea,
            eDifEqualDea,
            eDifLessDea,
        }
        public enum WaveConfig
        {
            eNone,

            // 直线上升
            ePureUp,
            // 上升后回调下降
            eFirstUpThenSlowDown,
            // 上升后快速下降
            eFirstUpThenFastDown,
            // 直线下降
            ePureDown,
            // 下降后回调上升
            eFirstDownThenSlowUp,
            // 下降后快速上升
            eFirstDownThenFastUp,
            // 水平震荡
            eFlatShake,
            // 震荡上升
            eShakeUp,
            // 震荡下降
            eShakeDown,
        }
        public static float GetWaveConfigValue(WaveConfig cfg)
        {
            switch(cfg)
            {
                case WaveConfig.eNone:
                case WaveConfig.ePureDown:
                case WaveConfig.eShakeDown:
                case WaveConfig.eFirstUpThenFastDown:
                    return 0;
                case WaveConfig.eFirstDownThenSlowUp:
                case WaveConfig.eFirstUpThenSlowDown:
                    return 0.5f;
                case WaveConfig.eFlatShake:
                    return 1;
                case WaveConfig.eFirstDownThenFastUp:
                    return 1.5f;
                case WaveConfig.eShakeUp:
                    return 2;
                case WaveConfig.ePureUp:
                    return 4;
            }
            return 1;
        }

        public static ValueCmpState GetValueCmpState(MACDPoint mp)
        {
            ValueCmpState res = ValueCmpState.eNone;
            if (mp.DIF > mp.DEA)
                res = ValueCmpState.eDifGreaterDea;
            else if (mp.DIF < mp.DEA)
                res = ValueCmpState.eDifLessDea;
            else
                res = ValueCmpState.eDifEqualDea;
            return res;
        }

        public static WaveConfig CheckMACDGoldenCrossAndDeadCross(MACDPointMap curMpm, CollectDataType cdt, ref int goldenCrossCount, ref int deadCrossCount, ref int confuseCount)
        {
            WaveConfig res = WaveConfig.eNone;
            
            goldenCrossCount = 0;
            deadCrossCount = 0;
            confuseCount = 0;
            int loop = LOOP_COUNT;
            ValueCmpState lastVCS = ValueCmpState.eNone;
            ValueCmpState startVCS = ValueCmpState.eNone;
            float maxDIF = 0;
            int maxDIFIndex = -1;
            float minDIF = 0;
            int minDIFIndex = -1;
            float leftDIF = 0;
            int leftIndex = -1;
            float rightDIF = 0;
            int rightIndex = -1;
            MACDPointMap tmpMPM = curMpm;
            MACDPoint mp = tmpMPM.GetData(cdt, false);
            rightDIF = mp.DIF;
            rightIndex = curMpm.index;

            while ( tmpMPM != null && loop >= 0 )
            {
                mp = tmpMPM.GetData(cdt, false);
                ValueCmpState tmpVCS = GetValueCmpState(mp);
                if (tmpVCS == ValueCmpState.eDifEqualDea)
                    ++confuseCount;

                if (lastVCS == ValueCmpState.eNone)
                {
                    if(maxDIFIndex == -1)
                    {
                        maxDIFIndex = tmpMPM.index;
                        maxDIF = mp.DIF;
                    }
                    if(minDIFIndex == -1)
                    {
                        minDIFIndex = tmpMPM.index;
                        minDIF = mp.DIF;
                    }

                    if (startVCS == ValueCmpState.eNone)
                        startVCS = tmpVCS;
                    if (tmpVCS != ValueCmpState.eDifEqualDea)
                        lastVCS = tmpVCS;
                }
                else
                {
                    if(maxDIF <= mp.DIF)
                    {
                        maxDIF = mp.DIF;
                        maxDIFIndex = tmpMPM.index;
                    }
                    if(minDIF >= mp.DIF)
                    {
                        minDIF = mp.DIF;
                        minDIFIndex = tmpMPM.index;
                    }

                    if (lastVCS != tmpVCS)
                    {
                        if (tmpVCS == ValueCmpState.eDifGreaterDea)
                        {
                            ++deadCrossCount;
                            lastVCS = tmpVCS;
                        }
                        else if (tmpVCS == ValueCmpState.eDifLessDea)
                        {
                            ++goldenCrossCount;
                            lastVCS = tmpVCS;
                        }
                    }
                }

                leftDIF = mp.DIF;
                leftIndex = tmpMPM.index;
                --loop;
                tmpMPM = tmpMPM.GetPrevMACDPM();
            }

            bool isShake = (goldenCrossCount > 0 && deadCrossCount > 0) || confuseCount > 0;
            if (Math.Abs(leftDIF - rightDIF) <= 0.0001f)
                res = WaveConfig.eFlatShake;
            else if (leftIndex == minDIFIndex && rightIndex == maxDIFIndex)
            {
                if (!isShake)
                    res = WaveConfig.ePureUp;
                else
                    res = WaveConfig.eShakeUp;
            }
            else if (leftIndex == minDIFIndex && maxDIFIndex < rightIndex)
                res = WaveConfig.eFirstUpThenSlowDown;
            else if (leftIndex < maxDIFIndex && rightIndex == minDIFIndex)
                res = WaveConfig.eFirstUpThenFastDown;
            else if (leftIndex == maxDIFIndex && rightIndex == minDIFIndex)
            {
                if(!isShake)
                    res = WaveConfig.ePureDown;
                else
                    res = WaveConfig.eShakeDown;
            }
            else if (leftIndex == maxDIFIndex && minDIFIndex < rightIndex)
                res = WaveConfig.eFirstDownThenSlowUp;
            else if (minDIFIndex > leftIndex && maxDIFIndex == rightIndex)
                res = WaveConfig.eFirstDownThenFastUp;
#if TRADE_DBG
            mp = curMpm.GetData(cdt, false);
            mp.WAVE_CFG = (byte)(res);
            mp.MAX_DIF_INDEX = maxDIFIndex;
            mp.MIN_DIF_INDEX = minDIFIndex;
            mp.LEFT_DIF_INDEX = leftIndex;
#endif
            return res;
        } 

        void CheckMACD(MACDPointMap curMpm, CollectDataType cdt, ref float value)
        {
            if (curMpm == null || curMpm.index == 0)
                return;
            int goldenCrossCount = 0, deadCrossCount = 0, confuseCount = 0;
            WaveConfig waveCfg = CheckMACDGoldenCrossAndDeadCross(
                curMpm, cdt, 
                ref goldenCrossCount, ref deadCrossCount, ref confuseCount);

            value = GetWaveConfigValue(waveCfg);
            MACDPoint mp = curMpm.GetData(cdt, false);
            if (mp.DIF > 0)
                value *= 2;
            if (mp.DIF > mp.BAR && mp.BAR > 0)
                value *= 2;
        }
        
        void SortNumberPath(DataItem item, int numIndex, ref List<PathCmpInfo> res)
        {
            res.Clear();
            float[] pathValues = CalcPathValue(item, numIndex);
            for( int i = 0; i < pathValues.Length; ++i )
            {
                res.Add(new PathCmpInfo(i, pathValues[i]));
            }
            res.Sort((x, y) =>
            {
                if (x.pathValue > y.pathValue)
                    return -1;
                return 1;
            });
        }

        float[] CalcPathValue(DataItem item, int numIndex)
        {
            float[] res = new float[3] { 1, 1, 1 };
            KGraphDataContainer kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataDict kdd = kddc.GetKDataDict(item);
            // 5期均线
            AvgPointMap apm5 = kddc.GetAvgPointMap(5, kdd);
            // 10期均线
            AvgPointMap apm10 = kddc.GetAvgPointMap(10, kdd);
            // 布林带数据
            BollinPointMap bpm = kddc.GetBollinPointMap(kdd);
            // MACD数据
            MACDPointMap mpm = kddc.GetMacdPointMap(kdd);
            //float path0Value = 1, path1Value = 1, path2Value = 1;
            float path0Avg5 = apm5.GetData(CollectDataType.ePath0, false).avgKValue;
            float path1Avg5 = apm5.GetData(CollectDataType.ePath1, false).avgKValue;
            float path2Avg5 = apm5.GetData(CollectDataType.ePath2, false).avgKValue;
            float path0Avg10 = apm10.GetData(CollectDataType.ePath0, false).avgKValue;
            float path1Avg10 = apm10.GetData(CollectDataType.ePath1, false).avgKValue;
            float path2Avg10 = apm10.GetData(CollectDataType.ePath2, false).avgKValue;
            float path0Bpm = bpm.GetData(CollectDataType.ePath0, false).midValue;
            float path1Bpm = bpm.GetData(CollectDataType.ePath1, false).midValue;
            float path2Bpm = bpm.GetData(CollectDataType.ePath2, false).midValue;
            {
                CheckMACD(mpm, CollectDataType.ePath0, ref res[0]);
                CheckMACD(mpm, CollectDataType.ePath1, ref res[1]);
                CheckMACD(mpm, CollectDataType.ePath2, ref res[2]);
            }
            if (path0Avg5 > path0Bpm) res[0] *= 2;
            if (path0Avg10 > path0Bpm) res[0] *= 2;
            if (path0Avg5 > path0Avg10) res[0] *= 2;
            if (path1Avg5 > path1Bpm) res[1] *= 2;
            if (path1Avg10 > path1Bpm) res[1] *= 2;
            if (path1Avg5 > path1Avg10) res[1] *= 2;
            if (path2Avg5 > path2Bpm) res[2] *= 2;
            if (path2Avg10 > path2Bpm) res[2] *= 2;
            if (path2Avg5 > path2Avg10) res[2] *= 2;
            return res;
        }

        void JudgeNumberPath(DataItem item, int numIndex, ref float maxV, ref int bestNumIndex, ref int bestPath)
        {
            float[] pathValues = CalcPathValue(item, numIndex);
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            StatisticUnit su0 = sum.statisticUnitMap[CollectDataType.ePath0];
            StatisticUnit su1 = sum.statisticUnitMap[CollectDataType.ePath1];
            StatisticUnit su2 = sum.statisticUnitMap[CollectDataType.ePath2];
            StatisticUnit curBestSU = null;
            float curBestV = 0;
            int curBestPath = -1;
            if(pathValues[0] > pathValues[1])
                Check(su0, su2, pathValues[0], pathValues[2], 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
            else if(pathValues[0] < pathValues[1])
                Check(su1, su2, pathValues[1], pathValues[2], 1, 2, ref curBestV, ref curBestPath, ref curBestSU);
            else
            { 
                if(su0.appearProbabilityShort > su1.appearProbabilityShort)
                    Check(su0, su2, pathValues[0], pathValues[2], 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
                else if(su0.appearProbabilityShort < su1.appearProbabilityShort)
                    Check(su1, su2, pathValues[1], pathValues[2], 1, 2, ref curBestV, ref curBestPath, ref curBestSU);
                else
                {
                    if (su0.appearProbabilityLong > su1.appearProbabilityLong)
                        Check(su0, su2, pathValues[0], pathValues[2], 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
                    else if (su0.appearProbabilityLong < su1.appearProbabilityLong)
                        Check(su1, su2, pathValues[1], pathValues[2], 1, 2, ref curBestV, ref curBestPath, ref curBestSU);
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
        }

        public static NumberCmpInfo GetNumberCmpInfo(ref List<NumberCmpInfo> nums, SByte number, bool createIfNotExist)
        {
            for( int i = 0; i < nums.Count; ++i )
            {
                if (nums[i].number == number)
                    return nums[i];
            }
            if(createIfNotExist)
            {
                NumberCmpInfo info = new NumberCmpInfo();
                info.number = number;
                info.appearCount = 0;
                nums.Add(info);
                return info;
            }
            return null;
        }
        public static void FindAllNumberProbabilities(DataItem item, ref List<NumberCmpInfo> nums)
        {
            nums.Clear();
            int realCount = item.idGlobal + 1;
            if (realCount > LotteryStatisticInfo.LONG_COUNT)
                realCount = LotteryStatisticInfo.LONG_COUNT;
            float total = 5 * realCount;
            for ( int i = 0; i < 5; ++i )
            {
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[i];
                int startIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum0);
                int endIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum9);
                for (int num = startIndex; num <= endIndex; ++num)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[num];
                    SByte number = (SByte)(num - startIndex);
                    NumberCmpInfo info = GetNumberCmpInfo(ref nums, number, true);
                    info.appearCount += sum.statisticUnitMap[cdt].appearCountLong;
                    info.rate = info.appearCount * 100 / total;
                    info.largerThanTheoryProbability = info.rate > GraphDataManager.GetTheoryProbability(cdt);
                }
            }
            nums.Sort(NumberCmpInfo.SortByAppearCount);
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
    

    /// <summary>
    /// 批量模拟交易
    /// </summary>
    class BatchTradeSimulator
    {
        enum SimState
        {
            eNone,
            // 准备原始数据和分析数据状态
            ePrepareData,
            // 模拟交易阶段
            eSimTrade,
            // 暂停模拟交易
            eSimPause,
            // 针对当前这批原始数据的交易模拟完成
            eFinishBatch,
            // 针对所有的原始数据的交易模拟结束
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

        public delegate void CallBackOnPrepareDataItems(DataItem startTradeItem);
        public static CallBackOnPrepareDataItems onPrepareDataItems;


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

        public void Start(ref int startDateID, ref int endDateID)
        {
            TradeDataManager.Instance.startMoney = startMoney;
            TradeDataManager.Instance.StopAtTheLatestItem = true;
            minMoney = maxMoney = currentMoney = startMoney;
            totalCount = tradeRightCount = tradeWrongCount = untradeCount = 0;

            fileIDLst.Clear();
            DataManager dm = DataManager.GetInst();
            foreach( int id in dm.mFileMetaInfo.Keys )
            {
                if (startDateID != -1 && id < startDateID)
                    continue;
                if (endDateID != -1 && id > endDateID)
                    continue;
                fileIDLst.Add(id);
            }
            fileIDLst.Sort();
            state = SimState.ePrepareData;
            lastIndex = -1;
            if (fileIDLst.Count > 0)
            {
                startDateID = fileIDLst[0];
                endDateID = fileIDLst[fileIDLst.Count - 1];
            }
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
                DataItem startTradeItem = null;
                if (string.IsNullOrEmpty(lastTradeIDTag))
                    startTradeItem = TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromFirst, lastTradeIDTag);
                else
                    startTradeItem = TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromSpec, lastTradeIDTag);
                if(startTradeItem != null)
                {
                    if (onPrepareDataItems != null)
                        onPrepareDataItems(startTradeItem);
                }
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
