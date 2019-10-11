using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    // 基本交易数据
    public class TradeDataBase
    {
        static int S_COUNT = 0;
        public static string[] NUM_TAGS = new string[] { "万位", "千位", "百位", "十位", "个位", };

        public TradeStatus tradeStatus = TradeStatus.eWaiting;
        public TradeType tradeType = TradeType.eNone;
        DataItem _lastDateItem = null;
        public DataItem lastDateItem
        {
            get { return _lastDateItem; }
            set
            {
                _lastDateItem = value;
                if (_lastDateItem != null)
                {
                    _lastDateItem.tag = this;
                }
            }
        }
        public DataItem targetLotteryItem = null;

        public float reward = 0;
        public float cost = 0;
        public float moneyBeforeTrade = 0;
        public float moneyAtferTrade = 0;
        //protected string tips = "";
        public bool isAutoTrade = false;

        int _INDEX = -1;
        public int INDEX
        {
            get { return _INDEX; }
            set { _INDEX = value; }
        }

        public void UpdateIndex()
        {
            if (_INDEX == -1)
            {
                _INDEX = S_COUNT++;
            }
        }

        public virtual string GetTips()
        {
            return "";
            //return tips;
        }
        public virtual string GetDbgInfo() { return ""; }
        public virtual void Update() { }
        public virtual void GetTradeNumIndexAndPathIndex(ref int numIndex, ref int pathIndex) { }
        public virtual float CalcCost() { return 0; }

        public virtual string GetTradeXML() { return ""; }
    }
}
