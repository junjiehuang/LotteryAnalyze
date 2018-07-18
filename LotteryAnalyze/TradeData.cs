#define TRADE_DBG


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
        public TradeDataManager.MACDLineWaveConfig macdLineCfg;
        public TradeDataManager.MACDBarConfig macdBarCfg;
        public TradeDataManager.KGraphConfig kGraphCfg;

        public PathCmpInfo(int id, float v, TradeDataManager.MACDLineWaveConfig lineCFG, TradeDataManager.MACDBarConfig barCFG, TradeDataManager.KGraphConfig kCFG)
        {
            pathIndex = id;
            pathValue = v;
            macdLineCfg = lineCFG;
            macdBarCfg = barCFG;
            kGraphCfg = kCFG;
        }
    }

    public enum TradeStatus
    {
        // 等待状态
        eWaiting,
        // 交易完成状态
        eDone,
    }

    public class NumberCmpInfo
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
            if (a == b)
                return 0;
            if (a.appearCount < b.appearCount)
                return 1;
            return -1;
        }
        public static int SortByNumber(NumberCmpInfo a, NumberCmpInfo b)
        {
            if (a == null || b == null)
                return 0;
            if (a == b)
                return 0;
            if (a.number > b.number)
                return 1;
            return -1;
        }
    }

    public class TradeNumbers
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
        public void AddProbabilityNumber(ref List<NumberCmpInfo> nums, int num)
        {
            for (int i = 0; i < nums.Count; ++i)
            {
                if (nums[i].number == num && tradeNumbers.Contains(nums[i]) == false)
                {
                    tradeNumbers.Add(nums[i]);
                    break;
                }
            }
        }
    }

    // 基本交易数据
    public class TradeDataBase
    {
        static int S_COUNT = 0;
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

        int _INDEX = -1;
        public int INDEX
        {
            get { return _INDEX; }
        }

        public void UpdateIndex()
        {
            if(_INDEX == -1)
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
    }

    // 一星交易数据
    public class TradeDataOneStar : TradeDataBase
    {
        public static float SingleTradeCost = 1;
        public static float SingleTradeReward = 9.8f;

        public Dictionary<int, TradeNumbers> tradeInfo = new Dictionary<int, TradeNumbers>();

#if TRADE_DBG
        public List<List<PathCmpInfo>> pathCmpInfos = new List<List<PathCmpInfo>>();
#endif

        public TradeDataOneStar()
        {
            UpdateIndex();
#if TRADE_DBG
            for (int i = 0; i < 5; ++i)
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
                        dbgtxt += "[" + pathCmpInfos[i][j].pathIndex + " = " + pathCmpInfos[i][j].pathValue + 
                            ", K = " + pathCmpInfos[i][j].kGraphCfg.ToString() + 
                            ", L = " + pathCmpInfos[i][j].macdLineCfg.ToString() + 
                            ", B = " + pathCmpInfos[i][j].macdBarCfg.ToString() + "]\n";
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
        public override float CalcCost()
        {
            float _cost = 0;
            foreach (int numIndex in tradeInfo.Keys)
            {
                TradeNumbers tns = tradeInfo[numIndex];
                _cost += SingleTradeCost * tns.tradeCount * tns.tradeNumbers.Count;
            }
            return _cost;
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

            BatchTradeSimulator.Instance.RefreshMoney();
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

    public class DebugInfo
    {
        public Dictionary<TradeDataManager.KGraphConfig, bool> kGraphCfgBPs = new Dictionary<TradeDataManager.KGraphConfig, bool>();
        public string dataItemTagBP = "";
        public int wrongCountBP = -1;

        public DebugInfo()
        {
            kGraphCfgBPs.Clear();
            for (TradeDataManager.KGraphConfig i = TradeDataManager.KGraphConfig.eNone; i < TradeDataManager.KGraphConfig.eMAX; ++i)
            {
                kGraphCfgBPs.Add(i, false);
            }
        }

        public void ClearAllBreakPoints()
        {
            foreach(TradeDataManager.KGraphConfig key in kGraphCfgBPs.Keys)
            {
                kGraphCfgBPs[key] = false;
            }
            dataItemTagBP = "";
            wrongCountBP = -1;
        }

        public bool Hit(TradeDataManager.KGraphConfig cfg)
        {
            if (kGraphCfgBPs.ContainsKey(cfg))
                return kGraphCfgBPs[cfg];
            return false;
        }

        public bool Hit(string tag)
        {
            return dataItemTagBP.CompareTo(tag) == 0;
        }

        public bool Hit(int wrongCount)
        {
            return wrongCountBP >= 0 && wrongCountBP <= wrongCount;
        }
    }

    public class LongWrongTradeInfo
    {
        public int tradeID = -1;
        public int count = 0;
        public string startDataItemTag;
        public string endDataItemTag; 
    }

    // 交易数据管理器
    public class TradeDataManager
    {
        public const int MACD_LOOP_COUNT = 5;
        public const int KGRAPH_LOOP_COUNT = 10;

        public enum ValueCmpState
        {
            eNone,
            eDifGreaterDea,
            eDifEqualDea,
            eDifLessDea,
        }
        public enum MACDLineWaveConfig
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
        public enum MACDBarConfig
        {
            eNone,

            // 蓝区回调上升
            eBlueSlowUp,
            // 蓝区回调上升穿进红区
            eBlue2RedUp,
            // 红区上升
            eRedUp,
            // 红区震荡
            eRedShake,
            // 红区回调下降
            eRedSlowDown,
            // 红区回调下降穿进蓝区
            eRed2BlueDown,
            // 蓝区下降
            eBlueDown,
            // 蓝区震荡
            eBlueShake,
            // 0线震荡
            eZeroShake,

            // 有下降的趋势
            ePrepareDown,
            // 有上升的趋势
            ePrepareUp,
        }
        public enum KGraphConfig
        {
            eNone,
            eSlowUpPrepareDown,
            ePureUp,
            eSlowDownPrepareUp,
            ePureDown,
            eShake,

            // 连续下降到达布林线中轨
            ePureDownToBML,
            // 连续在布林线中轨之上上升
            ePureUpUponBML,
            // 震荡上升
            eShakeUp,

            // 触摸到布林带上方并准备下降
            eTouchBolleanUpThenGoDown,
            // 贴着布林上轨上升
            eNearBolleanUpAndKeepUp,
            eShakeUp1,
            eShakeUp2,
            eShakeUp3,
            // K线从布林下轨升到布林中轨
            eFromBMDownTouchBMMid,

            eMAX,
        }

        // 获取MACD线形态评估值
        public static float GetMACDLineWaveConfigValue(MACDLineWaveConfig cfg)
        {
            switch (cfg)
            {
                case MACDLineWaveConfig.eNone:
                case MACDLineWaveConfig.ePureDown:
                case MACDLineWaveConfig.eShakeDown:
                case MACDLineWaveConfig.eFirstUpThenFastDown:
                    return 0;
                case MACDLineWaveConfig.eFirstDownThenSlowUp:
                case MACDLineWaveConfig.eFirstUpThenSlowDown:
                    return 0.5f;
                case MACDLineWaveConfig.eFlatShake:
                    return 1;
                case MACDLineWaveConfig.eFirstDownThenFastUp:
                    return 1.5f;
                case MACDLineWaveConfig.eShakeUp:
                    return 2;
                case MACDLineWaveConfig.ePureUp:
                    return 4;
            }
            return 1;
        }

        // 获取MACD柱形态评估值
        public static float GetMACDBarConfigValue(MACDBarConfig cfg)
        {
            switch (cfg)
            {
                case MACDBarConfig.eNone:
                case MACDBarConfig.eRedSlowDown:
                case MACDBarConfig.eRed2BlueDown:
                case MACDBarConfig.eBlueDown:
                case MACDBarConfig.eBlueShake:
                case MACDBarConfig.ePrepareDown:
                    return 0;
                case MACDBarConfig.eBlueSlowUp:
                case MACDBarConfig.eZeroShake:
                    return 0.5f;
                case MACDBarConfig.eBlue2RedUp:
                case MACDBarConfig.eRedShake:
                case MACDBarConfig.ePrepareUp:
                    return 1;
                case MACDBarConfig.eRedUp:
                    return 2;
            }
            return 1;
        }

        // 获取K线形态评估值
        public static float GetKGraphConfigValue(KGraphConfig cfg, int belowAvgLineCount, int uponAvgLineCount)
        {
            switch (cfg)
            {
                case KGraphConfig.eNone:
                case KGraphConfig.ePureDown:
                case KGraphConfig.eSlowDownPrepareUp:
                case KGraphConfig.eTouchBolleanUpThenGoDown:
                case KGraphConfig.eFromBMDownTouchBMMid:
                    return 0;
                case KGraphConfig.eShake:
                    return 0.5f;
                case KGraphConfig.eSlowUpPrepareDown:
                    return 1;
                case KGraphConfig.ePureUp:
                    return 2;
                case KGraphConfig.ePureUpUponBML:
                    return 4;
                case KGraphConfig.eShakeUp:
                    return 8;
                case KGraphConfig.ePureDownToBML:
                    return 6;
                case KGraphConfig.eShakeUp3:
                    return 9;
                case KGraphConfig.eShakeUp2:
                    return 10;
                case KGraphConfig.eShakeUp1:
                    return 11;
                case KGraphConfig.eNearBolleanUpAndKeepUp:
                    return 12;
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


        public enum StartTradeType
        {
            eFromFirst,
            eFromLatest,
            eFromSpec,
        }

        // 交易策略
        public enum TradeStrategy
        {
            // 选择最优的某个数值位的单个012路的号码
            eSinglePositionBestPath,
            // 选择某个数值位评分最高的2个012路的号码
            eSinglePositionBestTwoPath,
            // 选择某个数值位最优的012路的号码（评分一致则多个）
            eSinglePositionBestPaths,
            // 选择某个数值位012路的分值高于指定值的号码
            eSinglePositionPathsUponSpecValue,
            // 选择某个数值位012路最小遗漏的号码
            eSinglePositionSmallestMissCountPath,
            // 只要哪个数字位的最优012路满足就进行交易
            eMultiNumPath,

            // 选择某个数字位连续N期出号概率最高的几个数
            eSingleMostPosibilityNums,
            // 所有数字位都选择连续N期出号概率最高的几个数
            eMultiMostPosibilityNums,
            eSingleShortLongMostPosibilityNums,
            eSingleMostPosibilityPath,

        }
        public static string[] STRATEGY_NAMES = {
            "eSinglePositionBestPath",
            "eSinglePositionBestTwoPath",
            "eSinglePositionBestPaths",
            "eSinglePositionPathsUponSpecValue",
            "eSinglePositionSmallestMissCountPath",
            "eMultiNumPath",
            "eSingleMostPosibilityNums",
            "eMultiMostPosibilityNums",
            "eSingleShortLongMostPosibilityNums",
            "eSingleMostPosibilityPath",
        };

        // 是否强制每次交易都取指定的最大的数字个数
        public bool forceTradeByMaxNumCount = false;
        // 单次交易每个数字位投注的个数的最大值
        public int maxNumCount = 5;

        TradeStrategy _curTradeStrategy = TradeStrategy.eSinglePositionBestPath;
        public TradeStrategy curTradeStrategy
        {
            get { return _curTradeStrategy; }
            set { _curTradeStrategy = value; }
        }
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
        public int uponValue = 0;

        public DebugInfo debugInfo = new DebugInfo();

        //public bool simTradeFromFirstEveryTime = true;
        int _simSelNumIndex = 0;
        public int simSelNumIndex
        {
            get { return _simSelNumIndex; }
            set { _simSelNumIndex = value; }
        }
        DataItem curTestTradeItem = null;
        public DataItem CurTestTradeItem
        {
            get { return curTestTradeItem; }
        }
        bool pauseAutoTrade = true;
        bool needGetLatestItem = false;
        public List<int> tradeCountList = new List<int>();
        public int defaultTradeCount = 1;
        int currentTradeCountIndex = -1;
        public int CurrentTradeCountIndex
        {
            get { return currentTradeCountIndex; }
            set { currentTradeCountIndex = value; }
        }
        int _strongUpStartTradeIndex = 5;
        public int strongUpStartTradeIndex
        {
            get { return _strongUpStartTradeIndex; }
            set { _strongUpStartTradeIndex = curStrongUpTradeIndex = value; }
        }
        bool _onlyTradeOnStrongUpPath = false;
        public bool onlyTradeOnStrongUpPath
        {
            get { return _onlyTradeOnStrongUpPath; }
            set { _onlyTradeOnStrongUpPath = value; }
        }
        int curStrongUpTradeIndex = 5;
        public bool hasCompleted = false;
        bool stopAtTheLatestItem = false;
        bool tradeOneStep = false;
        public bool StopAtTheLatestItem
        {
            get { return stopAtTheLatestItem; }
            set { stopAtTheLatestItem = value; }
        }
        float _riskControl = 1;
        public float RiskControl
        {
            get { return _riskControl; }
            set { _riskControl = value; }
        }
        List<NumberCmpInfo> maxProbilityNums = new List<NumberCmpInfo>();
        List<NumberCmpInfo> maxProbilityPaths = new List<NumberCmpInfo>();
        public delegate void OnTradeComleted();
        public OnTradeComleted tradeCompletedCallBack;
        public delegate void OnLongWrongTrade(LongWrongTradeInfo info);
        public OnLongWrongTrade longWrongTradeCallBack;

        public AutoAnalyzeTool autoAnalyzeTool = new AutoAnalyzeTool();
        public AutoAnalyzeTool curPreviewAnalyzeTool = new AutoAnalyzeTool();

        public Dictionary<int, List<LongWrongTradeInfo>> longWrongTradeInfo = new Dictionary<int, List<LongWrongTradeInfo>>();
        public LongWrongTradeInfo tmpLongWrongTradeInfo = null;
        public int continueTradeMissCount = 0;

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

        public TradeDataBase GetTrade(int index)
        {
            if( historyTradeDatas.Count > 0)
            {
                TradeDataBase sTD = historyTradeDatas[0];
                TradeDataBase eTD = historyTradeDatas[historyTradeDatas.Count - 1];
                if (sTD.INDEX == index)
                    return sTD;
                else if (eTD.INDEX == index)
                    return eTD;
                else if(sTD.INDEX < index && index < eTD.INDEX)
                {
                    TradeDataBase tTD = historyTradeDatas[index - sTD.INDEX];
                    if(tTD.INDEX != index)
                    {
                        throw new Exception("invalid index!");
                    }
                    return tTD;
                }
            }
            return null;
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


        void RefreshTradeCountOnOneTradeCompleted(TradeDataBase trade)
        {
            if (curTradeStrategy == TradeStrategy.eSinglePositionBestPath)
            {
                if(currentTradeCountIndex >= 0 && currentTradeCountIndex < tradeCountList.Count)
                {
                    if (trade.reward > 0)
                    {
                        if(currentTradeCountIndex >= strongUpStartTradeIndex)
                            curStrongUpTradeIndex = strongUpStartTradeIndex;
                        currentTradeCountIndex = 0;
                    }
                    else if(trade.cost > 0)
                    {
                        if (currentTradeCountIndex < strongUpStartTradeIndex)
                        {
                            ++currentTradeCountIndex;
                            if (currentTradeCountIndex == tradeCountList.Count)
                                currentTradeCountIndex = 0;
                        }
                        else
                            currentTradeCountIndex = 0;
                    }
                }
            }
            else
            {
                if (currentTradeCountIndex >= 0 && currentTradeCountIndex < tradeCountList.Count)
                {
                    if (trade.reward > 0)
                    {
                        currentTradeCountIndex = 0;
                    }
                    else if (trade.cost > 0)
                    {
                        ++currentTradeCountIndex;
                    }
                    if (currentTradeCountIndex == tradeCountList.Count)
                        currentTradeCountIndex = 0;
                }
            }
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
                        //if (waitingTradeDatas[i].cost != 0)
                        {
                            BatchTradeSimulator.Instance.OnOneTradeCompleted(waitingTradeDatas[i]);
                        }

                        RefreshTradeCountOnOneTradeCompleted(waitingTradeDatas[i]);
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
            {
                curTestTradeItem = DataManager.GetInst().GetDataItemByIdTag(idTag);
                if(curTestTradeItem == null)
                    curTestTradeItem = DataManager.GetInst().GetFirstItem();
            }
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
            if (TradeDataManager.Instance.debugInfo.Hit(item.idTag) ||
                TradeDataManager.Instance.debugInfo.Hit(continueTradeMissCount))
            {
                TradeDataManager.Instance.PauseAutoTradeJob();
            }
            

            // 自动计算辅助线
            autoAnalyzeTool.Analyze(item.idGlobal);

            TradeDataOneStar trade = TradeDataManager.Instance.NewTrade(TradeType.eOneStar) as TradeDataOneStar;
            trade.lastDateItem = item;
            trade.isAutoTrade = true;
            trade.tradeInfo.Clear();

            switch (curTradeStrategy)
            {
                case TradeStrategy.eSinglePositionBestPath:
                    TradeSinglePositionBestPath(item, trade);
                    break;
                case TradeStrategy.eSinglePositionBestTwoPath:
                    TradeSinglePositionBestTwoPath(item, trade);
                    break;
                case TradeStrategy.eSinglePositionBestPaths:
                    TradeSinglePositionBestPaths(item, trade);
                    break;
                case TradeStrategy.eSinglePositionPathsUponSpecValue:
                    TradeSinglePositionPathsUponSpecValue(item, trade);
                    break;
                case TradeStrategy.eSinglePositionSmallestMissCountPath:
                    TradeSinglePositionSmallestMissCountPath(item, trade);
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
                case TradeStrategy.eSingleShortLongMostPosibilityNums:
                    TradeeSingleShortLongMostPosibilityNums(item, trade);
                    break;
                case TradeStrategy.eSingleMostPosibilityPath:
                    TradeSingleMostPosibilityPath(item, trade);
                    break;
            }
        }

        void TradeSinglePositionBestPath(DataItem item, TradeDataOneStar trade)
        {
            float maxV = -10;
            int bestNumIndex = -1;
            int bestPath = -1;
            bool isFinalPathStrongUp = false;
            if (simSelNumIndex == -1)
            {
                for (int i = 0; i < 5; ++i)
                {
                    JudgeNumberPath(item, trade, i, ref maxV, ref bestNumIndex, ref bestPath, ref isFinalPathStrongUp);
                }
            }
            else
            {
                JudgeNumberPath(item, trade, simSelNumIndex, ref maxV, ref bestNumIndex, ref bestPath, ref isFinalPathStrongUp);
            }

            if (bestNumIndex >= 0 && bestPath >= 0)
            {
                int tradeCount = defaultTradeCount;
                if (item.idGlobal >= LotteryStatisticInfo.SHOR_COUNT)
                {
                    if (tradeCountList.Count > 0)
                    {
                        if (currentTradeCountIndex == -1)
                            currentTradeCountIndex = 0;
                        if (isFinalPathStrongUp && curStrongUpTradeIndex < tradeCountList.Count)
                        {
                            currentTradeCountIndex = curStrongUpTradeIndex;
                            ++curStrongUpTradeIndex;
                            if (curStrongUpTradeIndex == tradeCountList.Count)
                                curStrongUpTradeIndex = strongUpStartTradeIndex;
                        }
                        tradeCount = tradeCountList[currentTradeCountIndex];
                    }
                }
                else
                    tradeCount = 0;

                TradeNumbers tn = new TradeNumbers();
                tn.tradeCount = tradeCount;
                FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);
                tn.SelPath012Number(bestPath, tradeCount, ref maxProbilityNums);
                trade.tradeInfo.Add(bestNumIndex, tn);
                if (TradeDataManager.Instance.RiskControl > 0 && trade.CalcCost() > currentMoney * TradeDataManager.Instance.RiskControl)
                {
                    currentTradeCountIndex = 0;
                    tradeCount = tradeCountList[currentTradeCountIndex];
                    tn.tradeCount = tradeCount;
                }
            }
        }

        void TradeSinglePositionBestTwoPath(DataItem item, TradeDataOneStar trade)
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

        void TradeSinglePositionBestPaths(DataItem item, TradeDataOneStar trade)
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

            float lastPathValue = 0;
            for (int i = 0; i < 2; ++i)
            {
                PathCmpInfo pci = res[i];
                if (pci.pathValue > 0)
                {
                    //if (i == 0)
                    //{
                    //    lastPathValue = pci.pathValue;
                    //    tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
                    //}
                    //else if( lastPathValue <= pci.pathValue )
                    {
                        tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
                    }
                }
            }
        }

        void TradeSinglePositionPathsUponSpecValue(DataItem item, TradeDataOneStar trade)
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

            if (currentTradeCountIndex >= tradeCountList.Count - 3)
            {
                float lastPathValue = 0;
                for (int i = 0; i < 2; ++i)
                {
                    PathCmpInfo pci = res[i];
                    if (pci.pathValue > uponValue)
                    {
                        tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
                    }
                }
            }
            else
            {
                PathCmpInfo pci = res[0];
                if (pci.pathValue > 0)
                    tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
            }
        }

        void TradeSinglePositionSmallestMissCountPath(DataItem item, TradeDataOneStar trade)
        {
            int bestNumIndex = 0;
            if (simSelNumIndex != -1)
                bestNumIndex = simSelNumIndex;

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

            GetBestPath(item, bestNumIndex, trade);

            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            trade.tradeInfo.Add(bestNumIndex, tn);
            FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);

            PathCmpInfo pci = trade.pathCmpInfos[bestNumIndex][0];
            tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
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

            bool isFinalPathStrongUp = false;
            for (int i = 0; i < 5; ++i)
            {
                float maxV = -10;
                int bestNumIndex = -1;
                int bestPath = -1;
                JudgeNumberPath(item, trade, i, ref maxV, ref bestNumIndex, ref bestPath, ref isFinalPathStrongUp);
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

        void TradeeSingleShortLongMostPosibilityNums(DataItem item, TradeDataOneStar trade)
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
            if (simSelNumIndex != -1)
                numID = simSelNumIndex;
            FindAllNumberProbabilities(item, ref maxProbilityNums);
            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            tn.SetMaxProbabilityNumber(tradeCount, ref maxProbilityNums, false, maxNumCount);

            List<NumberCmpInfo> posNumInfos = new List<NumberCmpInfo>();
            CollectDataItemNumPosInfo(ref posNumInfos, item, numID);
            for( int i = 0; i < posNumInfos.Count; ++i )
            {
                if (posNumInfos[i].largerThanTheoryProbability)
                    tn.AddProbabilityNumber(ref maxProbilityNums, posNumInfos[i].number);
            }

            trade.tradeInfo.Add(numID, tn);
        }

        void TradeSingleMostPosibilityPath(DataItem item, TradeDataOneStar trade)
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
            if (simSelNumIndex != -1)
                numID = simSelNumIndex;
            else
                numID = 0;

            float maxV = 0;
            int bestNumIndex = 0;
            int bestPathOnGraphConfig = -1;
            bool isFinalPathStrongUp = false;
            JudgeNumberPath(item, trade, numID, ref maxV, ref bestNumIndex, ref bestPathOnGraphConfig, ref isFinalPathStrongUp);

            FindAllNumberProbabilities(item, ref maxProbilityNums, false);
            byte ac0 = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[CollectDataType.ePath0].appearCountShort;
            byte ac1 = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[CollectDataType.ePath1].appearCountShort;
            byte ac2 = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[CollectDataType.ePath2].appearCountShort;
            int bestPathOnProbability = -1;
            if(ac0 > ac1)
            {
                if (ac0 > ac2)
                    bestPathOnProbability = 0;
                else if (ac0 < ac2)
                    bestPathOnProbability = 2;
                else
                    bestPathOnProbability = bestPathOnGraphConfig;
            }
            else if(ac0 < ac1)
            {
                if (ac1 > ac2)
                    bestPathOnProbability = 1;
                else if(ac1 < ac2)
                    bestPathOnProbability = 2;
                else
                    bestPathOnProbability = bestPathOnGraphConfig;
            }
            else
            {
                if (ac1 < ac2)
                    bestPathOnProbability = 2;
                else
                    bestPathOnProbability = bestPathOnGraphConfig;
            }
            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            tn.SelPath012Number(bestPathOnProbability, tradeCount, ref maxProbilityNums);
            trade.tradeInfo.Add(numID, tn);
            if (TradeDataManager.Instance.RiskControl > 0 && trade.CalcCost() > currentMoney * TradeDataManager.Instance.RiskControl)
            {
                currentTradeCountIndex = 0;
                tradeCount = tradeCountList[currentTradeCountIndex];
                tn.tradeCount = tradeCount;
            }
        }

        void CollectDataItemNumPosInfo(ref List<NumberCmpInfo> nums, DataItem dataItem, int numID)
        {
            nums.Clear();
            for( int i = 0; i < 10; ++i )
            {
                NumberCmpInfo info = new NumberCmpInfo();
                info.appearCount = 0;
                info.largerThanTheoryProbability = false;
                info.number = (SByte)i;
                info.rate = 0;
                nums.Add(info);
            }
            int loop = 10;
            int totalCount = 1;
            DataItem curItem = dataItem;
            int startCDTIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum0);
            while ( curItem != null && totalCount <= loop)
            {
                sbyte num = curItem.GetNumberByIndex(numID);
                nums[num].appearCount = nums[num].appearCount + 1;
                nums[num].rate = nums[num].appearCount * 100 / totalCount;
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[startCDTIndex + num];
                nums[num].largerThanTheoryProbability = nums[num].rate > GraphDataManager.GetTheoryProbability(cdt);

                curItem = curItem.parent.GetPrevItem(curItem);
                ++totalCount;
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


        public static void CheckMACDGoldenCrossAndDeadCross(MACDPointMap curMpm, CollectDataType cdt, ref int goldenCrossCount, ref int deadCrossCount, ref int confuseCount, ref MACDLineWaveConfig waveCfg, ref MACDBarConfig barCfg)
        {
            waveCfg = MACDLineWaveConfig.eNone;
            barCfg = MACDBarConfig.eNone;

            goldenCrossCount = 0;
            deadCrossCount = 0;
            confuseCount = 0;
            int loop = MACD_LOOP_COUNT;
            ValueCmpState lastVCS = ValueCmpState.eNone;
            ValueCmpState startVCS = ValueCmpState.eNone;
            float maxDIF = 0, minDIF = 0, leftDIF = 0, rightDIF = 0, leftBar = 0, rightBar = 0, maxBAR = 0, minBAR = 0;
            int maxDIFIndex = -1, minDIFIndex = -1, leftIndex = -1, rightIndex = -1, maxBARIndex = -1, minBARIndex = -1;
            MACDPointMap tmpMPM = curMpm;
            MACDPoint mp = tmpMPM.GetData(cdt, false);
            rightDIF = leftDIF = mp.DIF;
            rightIndex = leftIndex = curMpm.index;
            rightBar = leftBar = mp.BAR;
            maxBAR = minBAR = mp.BAR;
            maxBARIndex = minBARIndex = curMpm.index;

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
                leftBar = mp.BAR;
                leftIndex = tmpMPM.index;
                if(maxBAR < mp.BAR)
                {
                    maxBAR = mp.BAR;
                    maxBARIndex = tmpMPM.index;
                }
                if(minBAR > mp.BAR)
                {
                    minBAR = mp.BAR;
                    minBARIndex = tmpMPM.index;
                }

                --loop;
                tmpMPM = tmpMPM.GetPrevMACDPM();
            }

            bool isShake = (goldenCrossCount > 0 && deadCrossCount > 0) || confuseCount > 0;
            if (Math.Abs(leftDIF - rightDIF) <= 0.0001f)
                waveCfg = MACDLineWaveConfig.eFlatShake;
            else if (leftIndex == minDIFIndex && rightIndex == maxDIFIndex)
            {
                if (!isShake)
                    waveCfg = MACDLineWaveConfig.ePureUp;
                else
                    waveCfg = MACDLineWaveConfig.eShakeUp;
            }
            else if (leftIndex == minDIFIndex && maxDIFIndex < rightIndex)
                waveCfg = MACDLineWaveConfig.eFirstUpThenSlowDown;
            else if (leftIndex < maxDIFIndex && rightIndex == minDIFIndex)
                waveCfg = MACDLineWaveConfig.eFirstUpThenFastDown;
            else if (leftIndex == maxDIFIndex && rightIndex == minDIFIndex)
            {
                if(!isShake)
                    waveCfg = MACDLineWaveConfig.ePureDown;
                else
                    waveCfg = MACDLineWaveConfig.eShakeDown;
            }
            else if (leftIndex == maxDIFIndex && minDIFIndex < rightIndex)
                waveCfg = MACDLineWaveConfig.eFirstDownThenSlowUp;
            else if (minDIFIndex > leftIndex && maxDIFIndex == rightIndex)
                waveCfg = MACDLineWaveConfig.eFirstDownThenFastUp;

            if(leftBar < rightBar)
            {
                if(maxBARIndex < rightIndex && maxBARIndex > leftIndex)
                {
                    if (maxBAR > rightBar)
                        barCfg = MACDBarConfig.ePrepareDown;
                }
                if (barCfg == MACDBarConfig.eNone)
                {
                    if (rightBar <= 0)
                        barCfg = MACDBarConfig.eBlueSlowUp;
                    else if (leftBar <= 0 && rightBar >= 0)
                        barCfg = MACDBarConfig.eBlue2RedUp;
                    else if (leftBar >= 0)
                        barCfg = MACDBarConfig.eRedUp;
                }
            }
            else if(leftBar > rightBar)
            {
                if(minBARIndex > leftIndex && minBARIndex < rightIndex)
                {
                    if (minBAR < rightBar)
                        barCfg = MACDBarConfig.ePrepareUp;
                }
                if (barCfg == MACDBarConfig.eNone)
                {
                    if (leftBar >= 0 && rightBar <= 0)
                        barCfg = MACDBarConfig.eRed2BlueDown;
                    else if (leftBar <= 0)
                        barCfg = MACDBarConfig.eBlueDown;
                    else if (rightBar >= 0)
                        barCfg = MACDBarConfig.eRedSlowDown;
                }
            }
            else
            {
                if (leftBar > 0)
                    barCfg = MACDBarConfig.eRedShake;
                else if (leftBar < 0)
                    barCfg = MACDBarConfig.eBlueShake;
                else
                    barCfg = MACDBarConfig.eZeroShake;
            }
#if TRADE_DBG
            mp = curMpm.GetData(cdt, false);
            mp.WAVE_CFG = (byte)(waveCfg);
            mp.MAX_DIF_INDEX = maxDIFIndex;
            mp.MIN_DIF_INDEX = minDIFIndex;
            mp.LEFT_DIF_INDEX = leftIndex;
            mp.BAR_CFG = (byte)(barCfg);
            mp.MAX_BAR_INDEX = maxBARIndex;
            mp.MIN_BAR_INDEX = minBARIndex;
#endif
        }

        public static KGraphConfig CheckKGraphConfig(DataItem di, int numIndex, KDataDict item, BollinPointMap bpm, CollectDataType cdt, ref int belowAvgLineCount, ref int uponAvgLineCount)
        {
            int[] missCountCollect = new int[4] { 0, 0, 0, 0, };
            int maxMissCount = 0;
            int curMissCount = 0;
            int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);

            bool shouldCheckUnderAvgLineCount = true;
            belowAvgLineCount = 0;
            uponAvgLineCount = 0;
            KGraphConfig cfg = KGraphConfig.eNone;
            int loop = KGRAPH_LOOP_COUNT;
            KDataDict curItem = item;
            BollinPointMap curBPM = bpm, maxMissBPM = null;
            float rightKV = 0, leftKV = 0, maxKV = 0, minKV = 0, rightBpMid = 0, rightBpUp = 0, leftBpMid = 0, maxMissKV = 0, relateDist = 0;
            float rightID = -1, leftID = -1, maxID = -1, minID = -1, maxMissID = -1;
            rightKV = leftKV = maxKV = minKV = curItem.GetData(cdt, false).KValue;
            rightID = leftID = maxID = minID = curItem.index;
            rightBpMid = bpm.GetData(cdt, false).midValue;
            rightBpUp = bpm.GetData(cdt, false).upValue;
            DataItem curDI = di;
            curMissCount = curDI.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
            maxMissCount = curMissCount;
            // 达到布林线上轨的个数
            int reachBollinUpCount = 0;
            // 最大遗漏值回溯是否碰到布林线下轨
            bool hasMaxMissCountTouchBolleanDown = false;
            DataItem maxMissDataItem = null;
            KDataDict maxMissKdd = null;
            BollinPoint maxMissBP = null, leftBP = null, rightBP = curBPM.GetData(cdt, false), maxBP = rightBP, minBP = rightBP;
            KData leftKData = null, rightKData = item.GetData(cdt, false), maxMissKData = null, maxKData = rightKData, minKData = rightKData;

            while (curItem != null && loop >= 0)
            {
                KData data = curItem.GetData(cdt, false);
                leftKData = data;
                BollinPoint bp = curBPM.GetData(cdt, false);
                int mc = curDI.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                if (maxMissCount <= mc)
                {
                    maxMissBPM = curBPM;
                    maxMissCount = mc;
                    maxMissKV = data.KValue;
                    maxMissID = curItem.index;
                    maxMissDataItem = curDI;
                    maxMissKdd = curItem;
                    maxMissBP = bp;
                    maxMissKData = data;
                }
                if (mc < 4)
                    missCountCollect[mc] = missCountCollect[mc] + 1;

                leftKV = data.KValue;
                leftID = curItem.index;
                leftBpMid = bp.midValue;
                leftBP = bp;
                leftKData = data;

                if (maxKV < leftKV)
                {
                    maxKV = leftKV;
                    maxID = leftID;
                    maxBP = bp;
                    maxKData = data;
                }
                if (minKV > leftKV)
                {
                    minKV = leftKV;
                    minID = leftID;
                    minBP = bp;
                    minKData = data;
                }

                relateDist = data.RelateDistTo(bp.upValue);
                // 达到布林上轨的个数
                if (relateDist <= 0)
                    ++reachBollinUpCount;
                relateDist = data.RelateDistTo(bp.midValue);
                // 超过布林中轨的个数
                if (relateDist < 0)
                    ++uponAvgLineCount;
                // 低于布林中轨的个数
                else if (relateDist > 0)
                    ++belowAvgLineCount;

                if (curItem.index == 0)
                    break;
                curItem = curItem.parent.dataLst[curItem.index - 1];
                curBPM = curBPM.parent.bollinMapLst[curBPM.index - 1];
                curDI = curDI.parent.GetPrevItem(curDI);
                --loop;
            }
            DataItem testMCI = maxMissDataItem;
            KDataDict testKDD = maxMissKdd;
            BollinPointMap testBPM = maxMissBPM;
            // 从最大遗漏值那期开始回溯
            while (testMCI != null)
            {
                // 如果这期还是遗漏
                if (testMCI.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount > 0)
                {
                    KData data = testKDD.GetData(cdt, false);
                    BollinPoint bp = testBPM.GetData(cdt, false);
                    relateDist = data.RelateDistTo(bp.downValue);
                    // 如果出现在布林下轨
                    if(relateDist <= 0)
                    {
                        hasMaxMissCountTouchBolleanDown = true;
                        break;
                    }
                }
                else
                    break;

                testMCI = testMCI.parent.GetPrevItem(testMCI);
                testKDD = testKDD.parent.dataLst[testKDD.index - 1];
                testBPM = testBPM.parent.bollinMapLst[testBPM.index - 1];
            }
            
            // 左右边k值与布林中轨关系
            relateDist = item.GetData(cdt, false).RelateDistTo(rightBpMid);
            // 右端是否在布林中轨
            bool isRightNearBolleanMidLine = relateDist >= -0.5f && relateDist <= 0.5f;
            // 是否连续出现 0-2 期的遗漏值 如果是，那么就是一种震荡的形态
            bool isContinusSmallMissCount = missCountCollect[1] > 1 || missCountCollect[2] > 1 || missCountCollect[3] > 1;
            // 左边与布林中轨的相对值
            float LSideRelateDistToBolleanMid = leftKData.RelateDistTo(leftBpMid);
            // 右边与布林上轨的相对值
            float RSideRelateDistToBolleanUp = rightKData.RelateDistTo(rightBpUp);
            // 右边与布林中轨的相对值
            float RSideRelateDistToBolleanMid = relateDist;
            // 最大遗漏那期与布林中轨的相对值
            float maxMissCountRelateDistToBolleanMid = 0;
            float maxMissCountKV = leftKV;
            if (maxMissKdd != null)
            {
                maxMissCountRelateDistToBolleanMid = maxMissKData.RelateDistTo(maxMissBP.midValue);
                maxMissCountKV = maxMissKData.KValue;
            }
            else
            {
                maxMissCountRelateDistToBolleanMid = LSideRelateDistToBolleanMid;
                maxMissID = leftID;
            }
            // 从最大遗漏期到最新期的K线是否在布林中轨之上,且右边还没触及布林中轨
            bool isKGraphUponBolleanMid = 
                (maxMissCountRelateDistToBolleanMid <= 0 && RSideRelateDistToBolleanMid < -1 && (rightID - maxMissID >= (KGRAPH_LOOP_COUNT/2)));

            // k线贴着布林上轨持续上升（大买）
            if (isKGraphUponBolleanMid && 
                reachBollinUpCount > 0 &&
                maxMissCountKV < rightKV && 
                maxMissCount < 2 && 
                curMissCount < 2 )
                cfg = KGraphConfig.eNearBolleanUpAndKeepUp;
            // K线在布林中轨之上，触碰到布林上轨，连降2期以上，可能是要连续下降了，（大卖）
            else if(isKGraphUponBolleanMid && 
                RSideRelateDistToBolleanUp > 1 && 
                reachBollinUpCount > 0 && 
                leftKV < rightKV && 
                curMissCount > 1)
                cfg = KGraphConfig.eTouchBolleanUpThenGoDown;
            // k线碰到布林下轨，且最大遗漏值到右边是上升的
            else if(hasMaxMissCountTouchBolleanDown && isContinusSmallMissCount && maxMissKV < rightKV)
            {
                if (curMissCount == 3)
                    cfg = KGraphConfig.eShakeUp3;
                else if (curMissCount == 2)
                    cfg = KGraphConfig.eShakeUp2;
                else
                    cfg = KGraphConfig.eShakeUp1;
            }
            // K线从布林下轨升到中轨
            else if(maxMissKV < rightKV && 
                hasMaxMissCountTouchBolleanDown && 
                reachBollinUpCount == 0 && 
                rightKV > rightBpMid &&
                RSideRelateDistToBolleanMid >= -0.5f && RSideRelateDistToBolleanMid <= 0.5f)
            {
                cfg = KGraphConfig.eFromBMDownTouchBMMid;
            }

            if (cfg == KGraphConfig.eNone)
            {
                // 当前的遗漏值超过2
                if (curMissCount >= 2)
                {
                    // 如果当前k值在布林带中轨附近，认为这是从布林带上轨下降到中轨，有较大概率收到支撑反弹
                    if (isRightNearBolleanMidLine)
                        cfg = KGraphConfig.ePureDownToBML;
                    // 最大的遗漏值不超过2
                    else if (maxMissCount <= 2 || (belowAvgLineCount == 0 && leftKV < rightKV))
                        cfg = KGraphConfig.eShakeUp;
                    // 否则，如果是在中轨之上或者中轨之下，那么还是有较大的概率会延续下降趋势
                    else
                        cfg = KGraphConfig.ePureDown;
                }
                // 最大的遗漏值不超过2
                else if (maxMissCount <= 2)
                {
                    cfg = KGraphConfig.eShakeUp;
                }
                // 左边小于右边
                else if (leftKV < rightKV)
                {
                    // 最大值在中间
                    if (maxID > leftID && maxID < rightID)
                    {
                        // 最大值超过右边
                        if (maxKV > rightKV)
                        {
                            //if (rightKV - bpMidRight >= -missRelHeight)
                            if( relateDist <= 0.5f )
                                cfg = KGraphConfig.eShakeUp;
                            else
                                cfg = KGraphConfig.eSlowUpPrepareDown;
                        }
                    }
                    if (cfg == KGraphConfig.eNone)
                    {
                        //if (uponAvgLineCount > belowAvgLineCount && leftKV - bpMidLeft >= -missRelHeight && curMissCount < 2)
                        if(uponAvgLineCount > belowAvgLineCount && curMissCount < 2 && LSideRelateDistToBolleanMid <= 0.5f)
                            cfg = KGraphConfig.ePureUpUponBML;
                        else
                            cfg = KGraphConfig.ePureUp;
                    }
                }
                else if (leftKV > rightKV)
                {
                    if (minID > leftID && minID < rightID)
                    {
                        if (minKV < rightKV)
                            cfg = KGraphConfig.eSlowDownPrepareUp;
                    }
                    if (cfg == KGraphConfig.eNone)
                    {
                        if (uponAvgLineCount > belowAvgLineCount)
                            cfg = KGraphConfig.ePureDownToBML;
                        else
                            cfg = KGraphConfig.ePureDown;
                    }
                }
                else
                {
                    cfg = KGraphConfig.eShake;
                }
            }

            if(TradeDataManager.Instance.debugInfo.Hit(cfg))
            {
                TradeDataManager.Instance.PauseAutoTradeJob();
            }
            return cfg;
        }


        void CheckMACD(MACDPointMap curMpm, CollectDataType cdt, ref float value, ref MACDLineWaveConfig waveCfg, ref MACDBarConfig barCfg)
        {
            if (curMpm == null || curMpm.index == 0)
                return;
            int goldenCrossCount = 0, deadCrossCount = 0, confuseCount = 0;
            waveCfg = MACDLineWaveConfig.eNone;
            barCfg = MACDBarConfig.eNone;
            CheckMACDGoldenCrossAndDeadCross(
                curMpm, cdt, 
                ref goldenCrossCount, ref deadCrossCount, ref confuseCount, ref waveCfg, ref barCfg);

            //value = GetWaveConfigValue(waveCfg);
            //MACDPoint mp = curMpm.GetData(cdt, false);
            //if (mp.DIF > 0)
            //    value *= 2;
            //if (mp.DIF > mp.BAR && mp.BAR > 0)
            //    value *= 2;
            value = GetMACDBarConfigValue(barCfg);
        }
        
        void SortNumberPath(DataItem item, int numIndex, ref List<PathCmpInfo> res)
        {
            res.Clear();
            bool[] isStrongUp = new bool[3] { false, false, false };
            float[] pathValues;
            MACDLineWaveConfig[] lineCfgs;
            MACDBarConfig[] barCfgs;
            KGraphConfig[] kCfgs;
            CalcPathValue(item, numIndex, ref isStrongUp, out pathValues, out lineCfgs, out barCfgs, out kCfgs);
            for( int i = 0; i < pathValues.Length; ++i )
            {
                res.Add(new PathCmpInfo(i, pathValues[i], lineCfgs[i], barCfgs[i], kCfgs[i]));
            }
            res.Sort((x, y) =>
            {
                if (x.pathValue > y.pathValue)
                    return -1;
                return 1;
            });
        }

        bool CheckIsStrongUp(MACDLineWaveConfig mlCFG, MACDBarConfig mbCFG, KGraphConfig kgCFG)
        {
            if (
                mlCFG == MACDLineWaveConfig.eFirstDownThenSlowUp
                || mlCFG == MACDLineWaveConfig.eFirstUpThenFastDown
                || mlCFG == MACDLineWaveConfig.eFirstUpThenSlowDown
                || mlCFG == MACDLineWaveConfig.eFlatShake
                || mlCFG == MACDLineWaveConfig.eNone
                || mlCFG == MACDLineWaveConfig.ePureDown
                || mlCFG == MACDLineWaveConfig.eShakeDown
                //|| mlCFG == MACDLineWaveConfig.eShakeUp
                //|| mlCFG == MACDLineWaveConfig.eFirstDownThenFastUp
                //|| mlCFG == MACDLineWaveConfig.ePureUp
                )
                return false;
            if (
                mbCFG == MACDBarConfig.eBlueDown 
                || mbCFG == MACDBarConfig.eBlueShake
                || mbCFG == MACDBarConfig.eNone
                || mbCFG == MACDBarConfig.ePrepareDown
                || mbCFG == MACDBarConfig.eRed2BlueDown
                || mbCFG == MACDBarConfig.eRedShake
                || mbCFG == MACDBarConfig.eRedSlowDown
                || mbCFG == MACDBarConfig.eZeroShake
                || mbCFG == MACDBarConfig.ePrepareUp
                || mbCFG == MACDBarConfig.eBlueSlowUp
                //|| mbCFG == MACDBarConfig.eBlue2RedUp
                //|| mbCFG == MACDBarConfig.eRedUp
                )
                return false;
            if (kgCFG == KGraphConfig.eNone
                || kgCFG == KGraphConfig.ePureDown
                || kgCFG == KGraphConfig.ePureDownToBML
                || kgCFG == KGraphConfig.eShake
                || kgCFG == KGraphConfig.eSlowUpPrepareDown
                //|| kgCFG == KGraphConfig.eShakeUp
                || kgCFG == KGraphConfig.eSlowDownPrepareUp
                //|| kgCFG == KGraphConfig.ePureUp
                //|| kgCFG == KGraphConfig.ePureUpUponBML
                )
                return false;
            return true;
        }
        
        void GetBestPath(DataItem item, int numIndex, TradeDataOneStar trade)
        {
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            int loop = KGRAPH_LOOP_COUNT;
            int[] maxMissCounts = new int[3] { 0, 0, 0, };
            int[] prevMaxMissCounts = new int[3] { 0, 0, 0, };
            int[] curMissCounts = new int[3] { 0, 0, 0, };
            int[] maxMissCountIDs = new int[3] { item.idGlobal, item.idGlobal, item.idGlobal, };
            int[] prevMaxMissCountIDs = new int[3] { -1, -1, -1, };
            float[] pathValues = new float[3] { 0, 0, 0, };
            int[] stepCount = new int[3] { 0, 0, 0, };
            CollectDataType[] cdts = new CollectDataType[3] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
            for( int i = 0; i < 3; ++i )
            {
                curMissCounts[i] = sum.statisticUnitMap[cdts[i]].missCount;
                maxMissCounts[i] = curMissCounts[i];
                stepCount[i] = curMissCounts[i];
            }
            DataItem testItem = item.parent.GetPrevItem(item);
            while (testItem != null && loop > 0)
            {
                sum = testItem.statisticInfo.allStatisticInfo[numIndex];
                for (int i = 0; i < 3; ++i)
                {
                    int misscount = sum.statisticUnitMap[cdts[i]].missCount;
                    if (maxMissCounts[i] <= misscount)
                    {
                        maxMissCounts[i] = misscount;
                        maxMissCountIDs[i] = testItem.idGlobal;
                    }

                    if (stepCount[i] > 0)
                    {
                        --stepCount[i];
                        continue;
                    }
                    if(prevMaxMissCounts[i] <= misscount)
                    {
                        prevMaxMissCounts[i] = misscount;
                        prevMaxMissCountIDs[i] = testItem.idGlobal;
                    }
                }
                testItem = testItem.parent.GetPrevItem(testItem);
                --loop;
            }
            for (int i = 0; i < 3; ++i)
            {
                float main_rate = (float)maxMissCounts[i] / KGRAPH_LOOP_COUNT;
                if(prevMaxMissCountIDs[i] == -1)
                {
                    if (curMissCounts[i] > 0)
                        pathValues[i] = 1;
                    else
                        pathValues[i] = 0;
                }
                else
                    pathValues[i] = (float)curMissCounts[i] / (float)prevMaxMissCounts[i] * main_rate;
            }

            trade.pathCmpInfos[numIndex].Clear();
            for (int i = 0; i < 3; ++i)
            {
                trade.pathCmpInfos[numIndex].Add(new PathCmpInfo(i, pathValues[i], MACDLineWaveConfig.eNone, MACDBarConfig.eNone, KGraphConfig.eNone));
            }
            trade.pathCmpInfos[numIndex].Sort((x, y) =>
            {
                if (x.pathValue < y.pathValue)
                    return -1;
                return 1;
            });
        }

        void CalcPathValue(DataItem item, int numIndex, ref bool[] isPathStrongUp, out float[] pathValues, out MACDLineWaveConfig[] mlCfgs, out MACDBarConfig[] mbCfgs, out KGraphConfig[] kgCfgs)
        {
            pathValues = new float[3] { 1, 1, 1 };
            float[] kValues = new float[3] { 1, 1, 1 };
            int[] belowAvgLineCounts = new int[3] { 0, 0, 0 };
            int[] uponAvgLineCounts = new int[3] { 0, 0, 0 };
            float[] proShort = new float[3] { 1, 1, 1, };
            mlCfgs = new MACDLineWaveConfig[3] { MACDLineWaveConfig.eNone, MACDLineWaveConfig.eNone, MACDLineWaveConfig.eNone, };
            mbCfgs = new MACDBarConfig[3] { MACDBarConfig.eNone, MACDBarConfig.eNone, MACDBarConfig.eNone, };
            kgCfgs = new KGraphConfig[3] { KGraphConfig.eNone, KGraphConfig.eNone, KGraphConfig.eNone, };

            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            //int[] missCounts = new int[3] 
            //{
            //    sum.statisticUnitMap[CollectDataType.ePath0].missCount,
            //    sum.statisticUnitMap[CollectDataType.ePath1].missCount,
            //    sum.statisticUnitMap[CollectDataType.ePath2].missCount,
            //};
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
            MACDPoint mp0 = mpm.GetData(CollectDataType.ePath0, false);
            MACDPoint mp1 = mpm.GetData(CollectDataType.ePath1, false);
            MACDPoint mp2 = mpm.GetData(CollectDataType.ePath2, false);

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
                // 计算012路的K线图形态
                kgCfgs[0] = CheckKGraphConfig(item, numIndex, kdd, bpm, CollectDataType.ePath0, ref belowAvgLineCounts[0], ref uponAvgLineCounts[0]);
                kgCfgs[1] = CheckKGraphConfig(item, numIndex, kdd, bpm, CollectDataType.ePath1, ref belowAvgLineCounts[1], ref uponAvgLineCounts[1]);
                kgCfgs[2] = CheckKGraphConfig(item, numIndex, kdd, bpm, CollectDataType.ePath2, ref belowAvgLineCounts[2], ref uponAvgLineCounts[2]);

                // 计算012路MACD形态的评估值
                CheckMACD(mpm, CollectDataType.ePath0, ref pathValues[0], ref mlCfgs[0], ref mbCfgs[0]);
                CheckMACD(mpm, CollectDataType.ePath1, ref pathValues[1], ref mlCfgs[1], ref mbCfgs[1]);
                CheckMACD(mpm, CollectDataType.ePath2, ref pathValues[2], ref mlCfgs[2], ref mbCfgs[2]);
                pathValues[0] = pathValues[1] = pathValues[2] = 1;

                // 计算012路k线形态的评估值
                kValues[0] = GetKGraphConfigValue(kgCfgs[0], belowAvgLineCounts[0], uponAvgLineCounts[0]);
                kValues[1] = GetKGraphConfigValue(kgCfgs[1], belowAvgLineCounts[1], uponAvgLineCounts[1]);
                kValues[2] = GetKGraphConfigValue(kgCfgs[2], belowAvgLineCounts[2], uponAvgLineCounts[2]);
                mp0.KGRAPH_CFG = (byte)kgCfgs[0];
                mp1.KGRAPH_CFG = (byte)kgCfgs[1];
                mp2.KGRAPH_CFG = (byte)kgCfgs[2];

                //proShort[0] = sum.statisticUnitMap[CollectDataType.ePath0].appearProbabilityShort;
                //proShort[1] = sum.statisticUnitMap[CollectDataType.ePath1].appearProbabilityShort;
                //proShort[2] = sum.statisticUnitMap[CollectDataType.ePath2].appearProbabilityShort;
                
                pathValues[0] = pathValues[0] * kValues[0] * proShort[0];
                pathValues[1] = pathValues[1] * kValues[1] * proShort[1];
                pathValues[2] = pathValues[2] * kValues[2] * proShort[2];

                isPathStrongUp[0] = CheckIsStrongUp(mlCfgs[0], mbCfgs[0], kgCfgs[0]);
                isPathStrongUp[1] = CheckIsStrongUp(mlCfgs[1], mbCfgs[1], kgCfgs[1]);
                isPathStrongUp[2] = CheckIsStrongUp(mlCfgs[2], mbCfgs[2], kgCfgs[2]);

                if (onlyTradeOnStrongUpPath)
                {
                    if (isPathStrongUp[0] == false)
                        pathValues[0] = 0;
                    if (isPathStrongUp[1] == false)
                        pathValues[1] = 0;
                    if (isPathStrongUp[2] == false)
                        pathValues[2] = 0;
                }

                mp0.IS_STRONG_UP = isPathStrongUp[0];
                mp1.IS_STRONG_UP = isPathStrongUp[1];
                mp2.IS_STRONG_UP = isPathStrongUp[2];

                mp0.LAST_DATA_TAG = item.idTag;
                mp1.LAST_DATA_TAG = item.idTag;
                mp2.LAST_DATA_TAG = item.idTag;
            }
            //if (path0Avg5 > path0Bpm) pathValues[0] *= 2;
            //if (path0Avg10 > path0Bpm) pathValues[0] *= 2;
            //if (path0Avg5 > path0Avg10) pathValues[0] *= 2;
            //if (path1Avg5 > path1Bpm) pathValues[1] *= 2;
            //if (path1Avg10 > path1Bpm) pathValues[1] *= 2;
            //if (path1Avg5 > path1Avg10) pathValues[1] *= 2;
            //if (path2Avg5 > path2Bpm) pathValues[2] *= 2;
            //if (path2Avg10 > path2Bpm) pathValues[2] *= 2;
            //if (path2Avg5 > path2Avg10) pathValues[2] *= 2;

            //return pathValues;
        }

        void JudgeNumberPath(DataItem item, TradeDataOneStar trade, int numIndex, ref float maxV, ref int bestNumIndex, ref int bestPath, ref bool isFinalPathStrongUp)
        {
            bool[] isStrongUp = new bool[3] { false, false, false, };

            float[] pathValues;
            MACDLineWaveConfig[] lineCfgs;
            MACDBarConfig[] barCfgs;
            KGraphConfig[] kCfgs;
            CalcPathValue(item, numIndex, ref isStrongUp, out pathValues, out lineCfgs, out barCfgs, out kCfgs);

            //--------------------------
            trade.pathCmpInfos[numIndex].Clear();
            for (int i = 0; i < pathValues.Length; ++i)
            {
                trade.pathCmpInfos[numIndex].Add(new PathCmpInfo(i, pathValues[i], lineCfgs[i], barCfgs[i], kCfgs[i]));
            }
            trade.pathCmpInfos[numIndex].Sort((x, y) =>
            {
                if (x.pathValue > y.pathValue)
                    return -1;
                return 1;
            });
            //--------------------------

            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            StatisticUnit su0 = sum.statisticUnitMap[CollectDataType.ePath0];
            StatisticUnit su1 = sum.statisticUnitMap[CollectDataType.ePath1];
            StatisticUnit su2 = sum.statisticUnitMap[CollectDataType.ePath2];
            StatisticUnit curBestSU = null;
            float curBestV = 0;
            int curBestPath = -1;

            if (onlyTradeOnStrongUpPath == false || (isStrongUp[0] == false && isStrongUp[1] == false && isStrongUp[2] == false))
            {
                if (pathValues[0] > pathValues[1])
                    Check(su0, su2, pathValues[0], pathValues[2], 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
                else if (pathValues[0] < pathValues[1])
                    Check(su1, su2, pathValues[1], pathValues[2], 1, 2, ref curBestV, ref curBestPath, ref curBestSU);
                else
                {
                    if (su0.appearProbabilityShort > su1.appearProbabilityShort)
                        Check(su0, su2, pathValues[0], pathValues[2], 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
                    else if (su0.appearProbabilityShort < su1.appearProbabilityShort)
                        Check(su1, su2, pathValues[1], pathValues[2], 1, 2, ref curBestV, ref curBestPath, ref curBestSU);
                    else
                    {
                        if (su0.appearProbabilityLong > su1.appearProbabilityLong)
                            Check(su0, su2, pathValues[0], pathValues[2], 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
                        else if (su0.appearProbabilityLong < su1.appearProbabilityLong)
                            Check(su1, su2, pathValues[1], pathValues[2], 1, 2, ref curBestV, ref curBestPath, ref curBestSU);
                    }
                }
            }
            else if (isStrongUp[0] && isStrongUp[1] == false && isStrongUp[2] == false)
            {
                curBestPath = 0;
                curBestV = pathValues[0];
                curBestSU = su0;
            }
            else if (isStrongUp[0] == false && isStrongUp[1] && isStrongUp[2] == false)
            {
                curBestPath = 1;
                curBestV = pathValues[1];
                curBestSU = su1;
            }
            else if (isStrongUp[0] == false && isStrongUp[1] == false && isStrongUp[2])
            {
                curBestPath = 2;
                curBestV = pathValues[2];
                curBestSU = su2;
            }
            else if (isStrongUp[0] && isStrongUp[1] && isStrongUp[2] == false)
            {
                if (pathValues[0] > pathValues[1])
                {
                    curBestPath = 0;
                    curBestV = pathValues[0];
                    curBestSU = su0;
                }
                else if (pathValues[0] < pathValues[1])
                {
                    curBestPath = 1;
                    curBestV = pathValues[1];
                    curBestSU = su1;
                }
                else
                    Check(su0, su1, pathValues[0], pathValues[1], 0, 1, ref curBestV, ref curBestPath, ref curBestSU);
            }
            else if (isStrongUp[0] == false && isStrongUp[1] && isStrongUp[2])
            {
                if (pathValues[2] > pathValues[1])
                {
                    curBestPath = 2;
                    curBestV = pathValues[2];
                    curBestSU = su2;
                }
                else if (pathValues[2] < pathValues[1])
                {
                    curBestPath = 1;
                    curBestV = pathValues[1];
                    curBestSU = su1;
                }
                else
                    Check(su2, su1, pathValues[2], pathValues[1], 2, 1, ref curBestV, ref curBestPath, ref curBestSU);
            }
            else if (isStrongUp[0] && isStrongUp[1] == false && isStrongUp[2])
            {
                if (pathValues[0] > pathValues[2])
                {
                    curBestPath = 0;
                    curBestV = pathValues[0];
                    curBestSU = su0;
                }
                else if (pathValues[0] < pathValues[2])
                {
                    curBestPath = 2;
                    curBestV = pathValues[2];
                    curBestSU = su2;
                }
                else
                    Check(su0, su2, pathValues[0], pathValues[2], 0, 2, ref curBestV, ref curBestPath, ref curBestSU);
            }

            if (curBestPath != -1 && curBestV > 0)
            {
                if(curBestV > maxV)
                {
                    bestPath = curBestPath;
                    bestNumIndex = numIndex;
                    maxV = curBestV;
                    isFinalPathStrongUp = isStrongUp[curBestPath];
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
                        isFinalPathStrongUp = isStrongUp[curBestPath];
                    }
                    else if(suBest.appearProbabilityShort == curBestSU.appearProbabilityShort)
                    {
                        if (suBest.appearProbabilityLong < curBestSU.appearProbabilityLong)
                        {
                            bestPath = curBestPath;
                            bestNumIndex = numIndex;
                            maxV = curBestV;
                            isFinalPathStrongUp = isStrongUp[curBestPath];
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
        public static void FindAllNumberProbabilities(DataItem item, ref List<NumberCmpInfo> nums, bool collectByLongCount = true)
        {
            nums.Clear();
            int realCount = item.idGlobal + 1;
            int MAX_COUNT = collectByLongCount ? LotteryStatisticInfo.LONG_COUNT : LotteryStatisticInfo.SHOR_COUNT;
            if (realCount > MAX_COUNT)
                realCount = MAX_COUNT;
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
                    info.appearCount += collectByLongCount ? sum.statisticUnitMap[cdt].appearCountLong : sum.statisticUnitMap[cdt].appearCountShort;
                    info.rate = info.appearCount * 100 / total;
                    info.largerThanTheoryProbability = info.rate > GraphDataManager.GetTheoryProbability(cdt);
                }
            }
            nums.Sort(NumberCmpInfo.SortByAppearCount);
        }
        public static void FindAllPathProbabilities(DataItem item, ref List<NumberCmpInfo> nums, bool collectByLongCount = true)
        {
            nums.Clear();
            int realCount = item.idGlobal + 1;
            int MAX_COUNT = collectByLongCount ? LotteryStatisticInfo.LONG_COUNT : LotteryStatisticInfo.SHOR_COUNT;
            if (realCount > MAX_COUNT)
                realCount = MAX_COUNT;
            float total = 5 * realCount;
            for (int i = 0; i < 5; ++i)
            {
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[i];
                int startIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
                int endIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath2);
                for (int num = startIndex; num <= endIndex; ++num)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[num];
                    SByte number = (SByte)(num - startIndex);
                    NumberCmpInfo info = GetNumberCmpInfo(ref nums, number, true);
                    info.appearCount += collectByLongCount ? sum.statisticUnitMap[cdt].appearCountLong : sum.statisticUnitMap[cdt].appearCountShort;
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
    public class BatchTradeSimulator
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

        
        public Dictionary<int, int> tradeMissInfo = new Dictionary<int, int>();
        List<int> fileIDLst = new List<int>();
        int lastIndex = -1;        
        SimState state = SimState.eNone;
        string lastTradeIDTag = null;
        int lastTradeCountIndex = -1;
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

        public BatchTradeSimulator()
        {

        }

        public void OnOneTradeCompleted(TradeDataBase trade)
        {
            bool tradeSuccess = trade.reward > 0;
            if (tradeSuccess)
            {
                if (TradeDataManager.Instance.continueTradeMissCount > 0)
                {
                    if (tradeMissInfo.ContainsKey(TradeDataManager.Instance.continueTradeMissCount))
                        tradeMissInfo[TradeDataManager.Instance.continueTradeMissCount] = tradeMissInfo[TradeDataManager.Instance.continueTradeMissCount] + 1;
                    else
                        tradeMissInfo.Add(TradeDataManager.Instance.continueTradeMissCount, 1);

                    if (TradeDataManager.Instance.continueTradeMissCount >= TradeDataManager.Instance.tradeCountList.Count)
                    {
                        TradeDataManager.Instance.tmpLongWrongTradeInfo.endDataItemTag = trade.targetLotteryItem.idTag;
                        TradeDataManager.Instance.tmpLongWrongTradeInfo.count = TradeDataManager.Instance.continueTradeMissCount;
                        TradeDataManager.Instance.tmpLongWrongTradeInfo.tradeID = trade.INDEX;
                        if (TradeDataManager.Instance.longWrongTradeCallBack != null)
                            TradeDataManager.Instance.longWrongTradeCallBack(TradeDataManager.Instance.tmpLongWrongTradeInfo);
                        List<LongWrongTradeInfo> lst = null;
                        if (TradeDataManager.Instance.longWrongTradeInfo.ContainsKey(TradeDataManager.Instance.continueTradeMissCount))
                            lst = TradeDataManager.Instance.longWrongTradeInfo[TradeDataManager.Instance.continueTradeMissCount];
                        else
                        {
                            lst = new List<LongWrongTradeInfo>();
                            TradeDataManager.Instance.longWrongTradeInfo.Add(TradeDataManager.Instance.continueTradeMissCount,lst);
                        }
                        lst.Add(TradeDataManager.Instance.tmpLongWrongTradeInfo);
                        TradeDataManager.Instance.tmpLongWrongTradeInfo = null;
                    }
                }
                TradeDataManager.Instance.continueTradeMissCount = 0;
                if (TradeDataManager.Instance.tmpLongWrongTradeInfo != null)
                    TradeDataManager.Instance.tmpLongWrongTradeInfo.startDataItemTag = null;
            }
            else
            {
                if(TradeDataManager.Instance.tmpLongWrongTradeInfo == null)
                    TradeDataManager.Instance.tmpLongWrongTradeInfo = new LongWrongTradeInfo();
                if(string.IsNullOrEmpty(TradeDataManager.Instance.tmpLongWrongTradeInfo.startDataItemTag))
                    TradeDataManager.Instance.tmpLongWrongTradeInfo.startDataItemTag = trade.targetLotteryItem.idTag;
                if(trade.cost > 0)
                    ++TradeDataManager.Instance.continueTradeMissCount;
            }
        }

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
            lastTradeIDTag = "";
            tradeMissInfo.Clear();
            TradeDataManager.Instance.longWrongTradeInfo.Clear();
            TradeDataManager.Instance.tmpLongWrongTradeInfo = null;
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
            return (fileIDLst.Count > 0 && lastIndex < fileIDLst.Count) || (state != SimState.eFinishAll);
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
                {
                    startTradeItem = TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromSpec, lastTradeIDTag);
                    TradeDataManager.Instance.CurrentTradeCountIndex = lastTradeCountIndex;
                }
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
        public void RefreshMoney()
        {
            currentMoney = TradeDataManager.Instance.currentMoney;
            if (maxMoney < currentMoney)
                maxMoney = currentMoney;
            if (minMoney > currentMoney)
                minMoney = currentMoney;
        }
        void DoSimTrade()
        {
            RefreshMoney();
            if (TradeDataManager.Instance.hasCompleted == true)
            {
                lastTradeIDTag = TradeDataManager.Instance.CurTestTradeItem.idTag;
                lastTradeCountIndex = TradeDataManager.Instance.CurrentTradeCountIndex;
                state = SimState.eFinishBatch;
            }
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
