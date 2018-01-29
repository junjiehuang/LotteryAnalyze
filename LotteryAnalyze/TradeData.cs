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
        public List<byte> tradeNumbers = new List<byte>();
        public int tradeCount = 0;
    }

    class TradeDataBase
    {
        public TradeStatus tradeStatus = TradeStatus.eWaiting;
        public TradeType tradeType = TradeType.eNone;
        public DataItem lastDateItem = null;
        public DataItem targetLotteryItem = null;

        public float reward = 0;
        public float cost = 0;
        public float moneyBeforeTrade = 0;
        public float moneyAtferTrade = 0;

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
                            cost += SingleTradeCost * tns.tradeCount;
                        }
                        moneyBeforeTrade = TradeDataManager.Instance.currentMoney;
                        TradeDataManager.Instance.currentMoney += reward - cost;
                        moneyAtferTrade = TradeDataManager.Instance.currentMoney;
                        tradeStatus = TradeStatus.eDone;
                    }
                }
            }
        }
    }


    class TradeDataManager
    {
        static TradeDataManager sInst = null;
        public List<TradeDataBase> historyTradeDatas = new List<TradeDataBase>();
        public List<TradeDataBase> waitingTradeDatas = new List<TradeDataBase>();
        public float currentMoney = 2000;


        TradeDataManager()
        {
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
                        historyTradeDatas.Add(waitingTradeDatas[i]);
                        waitingTradeDatas.RemoveAt(i);
                    }
                }
            }
        }
    }
}
