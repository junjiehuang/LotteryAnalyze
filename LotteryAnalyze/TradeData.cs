﻿using System;
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

    class TradeNumbers
    {
        public List<SByte> tradeNumbers = new List<SByte>();
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
        public void SelPath012Number(int path, int tradeCount)
        {
            this.tradeCount = tradeCount;
            tradeNumbers.Clear();
            if (path == 0)
            { tradeNumbers.Add(0); tradeNumbers.Add(3); tradeNumbers.Add(6); tradeNumbers.Add(9); }
            else if (path == 1)
            { tradeNumbers.Add(1); tradeNumbers.Add(4); tradeNumbers.Add(7); }
            else if (path == 2)
            { tradeNumbers.Add(2); tradeNumbers.Add(5); tradeNumbers.Add(8); }
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

        public void AddSelNum(int numIndex, ref List<SByte> selNums, int tradeCount)
        {
            if(tradeInfo.ContainsKey(numIndex) == false)
            {
                TradeNumbers tn = new TradeNumbers();
                tradeInfo.Add(numIndex, tn);
                tn.tradeNumbers.AddRange(selNums);
                tn.tradeCount = tradeCount;
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
                            if(tns.tradeNumbers.Contains(dstValue))
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
                hasCompleted = true;
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
                tn.SelPath012Number(bestPath, tradeCount);
                trade.tradeInfo.Add(bestNumIndex, tn);
            }
        }

        void JudgeNumberPath(DataItem item, int numIndex, ref float maxV, ref int bestNumIndex, ref int bestPath)
        {
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            StatisticUnit su0 = sum.statisticUnitMap[CollectDataType.ePath0];
            StatisticUnit su1 = sum.statisticUnitMap[CollectDataType.ePath1];
            StatisticUnit su2 = sum.statisticUnitMap[CollectDataType.ePath2];
            StatisticUnit maxSU;
            if (su0.appearProbabilityShort >= su1.appearProbabilityShort)
                maxSU = su0;
            else
                maxSU = su1;
            if (su2.appearProbabilityShort >= maxSU.appearProbabilityShort)
                maxSU = su2;

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
        SimState state = SimState.ePrepareData;
        string lastTradeIDTag = null;
        DataItem curTradeItem;
        SimState backUpState = SimState.eNone;

        public int batch = 5;
        public float currentMoney;
        public float startMoney;
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
            int v = TradeDataManager.Instance.historyTradeDatas.Count * 100 / DataManager.GetInst().GetAllDataItemCount();
            return v;
        }

        public void Start()
        {
            TradeDataManager.Instance.startMoney = startMoney;
            minMoney = maxMoney = currentMoney = startMoney;
            totalCount = tradeRightCount = tradeWrongCount = untradeCount = 0;

            fileIDLst.Clear();
            DataManager dm = DataManager.GetInst();
            foreach( int id in dm.mFileMetaInfo.Keys )
            {
                fileIDLst.Add(id);
            }
            fileIDLst.Sort();
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