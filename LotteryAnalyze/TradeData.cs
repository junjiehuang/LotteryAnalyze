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
                    }
                }
            }
        }

        public override string GetTips()
        {
            if(tips.Length == 0 && targetLotteryItem != null)
            {
                tips += targetLotteryItem.idTag + "\n";
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
                waitingTradeDatas.Add(trade);
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
            DataItem latestItem = DataManager.GetInst().GetLatestItem();
            if (latestItem == curTestTradeItem)
                return;

            DataItem curItem = DataManager.GetInst().GetFirstItem();
            if (curTestTradeItem != null)
                curItem = curTestTradeItem;
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
            TradeDataOneStar trade = TradeDataManager.Instance.NewTrade(TradeType.eOneStar) as TradeDataOneStar;
            trade.lastDateItem = item;
            TradeNumbers tn = new TradeNumbers();
            tn.SelPath012Number(2, 2);
            trade.tradeInfo.Add(0, tn);
        }
    }
}
