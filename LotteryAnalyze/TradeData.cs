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

    class TradeNumbers
    {
        public List<byte> tradeNumbers = new List<byte>();
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

        public void AddSelNum(int numIndex, ref List<byte> selNums, int tradeCount)
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
                            byte dstValue = targetLotteryItem.GetNumberByIndex(numIndex);
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
        public float currentMoney = 2000;
        public float minValue = 0;
        public float maxValue = 0;
        public int rightCount = 0;
        public int wrongCount = 0;
        public int untradeCount = 0;

        public bool simTradeFromFirstEveryTime = true;
        public int simSelNumIndex = 0;
        DataItem curTestTradeItem = null;


        TradeDataManager()
        {
            minValue = maxValue = currentMoney;
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
                        if (maxValue < waitingTradeDatas[i].moneyAtferTrade)
                            maxValue = waitingTradeDatas[i].moneyAtferTrade;
                        if (minValue > waitingTradeDatas[i].moneyAtferTrade)
                            minValue = waitingTradeDatas[i].moneyAtferTrade;
                        historyTradeDatas.Add(waitingTradeDatas[i]);
                        waitingTradeDatas.RemoveAt(i);
                    }
                }
            }
        }


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
            if (bestNumIndex >=0 && bestPath >= 0)
            {
                TradeNumbers tn = new TradeNumbers();
                tn.SelPath012Number(bestPath, 1);
                trade.tradeInfo.Add(bestNumIndex, tn);
            }
        }

        void JudgeNumberPath(DataItem item, int numIndex, ref float maxV, ref int bestNumIndex, ref int bestPath)
        {
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
        }
    }
}
