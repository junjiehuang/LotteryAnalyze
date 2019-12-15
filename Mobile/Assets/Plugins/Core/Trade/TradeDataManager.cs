//#define TRADE_DBG


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public enum GenerateType
    {
        eMinCost,
        eFixedProfit,
        eFixedScaleCount,
    }


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
    
    public enum TradeStatus
    {
        // 等待状态
        eWaiting,
        // 交易完成状态
        eDone,
    }
    
    // 交易数据管理器
    public class TradeDataManager
    {
        public static List<string> S_GenerateTypeStr = new List<string>()
        {
            "按总成本最小计算",
            "按每次中出有固定收益计算",
            "按以上次注数乘以指定倍率计算",
        };

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
            // 选择某个数值位012路顶低区间在交易次数范围内的号码
            eSinglePositionPathOnArea,
            // 选择某个数值位012路遗漏图面积最小的那一路的号码
            eSinglePositionSmallestMissCountArea,
            // 根据012路出号概率来决定选中那一路的号码
            eSinglePositionPathByAppearencePosibility,
            // 只要哪个数字位的最优012路满足就进行交易
            eMultiNumPath,

            // 选择某个数字位的热号
            eSinglePositionHotestNums,

            // 选择某个数字位连续N期出号概率最高的几个数
            eSingleMostPosibilityNums,
            // 所有数字位都选择连续N期出号概率最高的几个数
            eMultiMostPosibilityNums,
            // 选择某个数字位短期长期概率最好的几个数
            eSingleShortLongMostPosibilityNums,
            // 选择几率最大的012路
            eSingleMostPosibilityPath,

            // 单个数字位按所有排列顺序权重叠加筛选
            eSinglePositionCondictionsSuperposition,

            // 根据均线选择某个数字位的最优012路
            eSinglePositionBestPathByAvgLine,

            // 在Macd柱经过长期走低开始走高的时候进行交易
            eTradeOnMacdBarGoUp,
            // 在K线触碰到布林线下轨的时候进行交易
            eTradeOnKCurveTouchBolleanDown,
            // 交易热路的那些号码
            eTradeHotestPathNums,
            // 交易小遗漏路的号码
            eTradeOnSmallMissCount,
        }
        public static List<string> STRATEGY_NAMES = new List<string>()
        {
            "eSinglePositionBestPath",
            "eSinglePositionBestTwoPath",
            "eSinglePositionBestPaths",
            "eSinglePositionPathsUponSpecValue",
            "eSinglePositionSmallestMissCountPath",
            "eSinglePositionPathOnArea",
            "eSinglePositionSmallestMissCountArea",
            "eSinglePositionPathByAppearencePosibility",
            "eMultiNumPath",

            "eSinglePositionHotestNums",

            "eSingleMostPosibilityNums",
            "eMultiMostPosibilityNums",
            "eSingleShortLongMostPosibilityNums",
            "eSingleMostPosibilityPath",

            "sSinglePositionCondictionsSuperposition",

            "sSinglePositionBestPathByAvgLine",
            "eTradeOnMacdBarGoUp",
            "eTradeOnKCurveTouchBolleanDown",
            "eTradeHotestPathNums",
            "eTradeOnSmallMissCount",
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
        public bool killLastNumber = false;

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
        bool pauseAutoTrade = false;
        bool needGetLatestItem = false;
        //public List<int> tradeCountList = new List<int>();
        List<int> _tradeCountList = new List<int>();
        public List<int> tradeCountList
        {
            get
            {
                if(GlobalSetting.G_CUR_TRADE_INDEX == -1)
                {
                    return _tradeCountList;
                }
                else
                {
                    if(GlobalSetting.G_CUR_TRADE_INDEX >= 0 && GlobalSetting.G_CUR_TRADE_INDEX < GlobalSetting.TradeSets.Count)
                        return GlobalSetting.TradeSets[GlobalSetting.G_CUR_TRADE_INDEX];
                    return _tradeCountList;
                }
            }
        }

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
        int _multiTradePathCount = 3;
        public int MultiTradePathCount
        {
            get { return _multiTradePathCount; }
            set { _multiTradePathCount = value; }
        }

        List<NumberCmpInfo> maxProbilityNums = new List<NumberCmpInfo>();
        List<NumberCmpInfo> maxProbilityPaths = new List<NumberCmpInfo>();
        public delegate void OnTradeComleted();
        public OnTradeComleted tradeCompletedCallBack;
        public delegate void OnLongWrongTrade(LongWrongTradeInfo info);
        public OnLongWrongTrade longWrongTradeCallBack;
        public delegate void OnOneTradeCompleted(TradeDataBase trade);
        public OnOneTradeCompleted evtOneTradeCompleted;

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

        public TradeDataBase GetFirstHistoryTradeData()
        {
            if (historyTradeDatas.Count > 0)
                return historyTradeDatas[0];
            return null;
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
        public TradeDataBase GetTradeByItemGlobalID(int itemGlobalID)
        {
            for(int i = 0; i < historyTradeDatas.Count; ++i)
            {
                if (historyTradeDatas[i].targetLotteryItem.idGlobal == itemGlobalID)
                    return historyTradeDatas[i];
            }
            return null;
        }
        public int GetTradeIndex(int itemGlobalID)
        {
            for (int i = 0; i < historyTradeDatas.Count; ++i)
            {
                if (historyTradeDatas[i].targetLotteryItem.idGlobal == itemGlobalID)
                    return i;
            }
            return -1;
        }
        public int GetTradeIndex(TradeDataBase trade)
        {
            return historyTradeDatas.IndexOf(trade);
        }
        
        public DataItem GetLatestTradedDataItem()
        {
            if (historyTradeDatas.Count > 0)
                return historyTradeDatas[historyTradeDatas.Count - 1].targetLotteryItem;
            return null;
        }

        public TradeDataBase GetLatestTradeData()
        {
            if (historyTradeDatas.Count > 0)
                return historyTradeDatas[historyTradeDatas.Count - 1];
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

            TradeDataOneStar ost = trade as TradeDataOneStar;
            foreach(int numid in ost.tradeInfo.Keys)
            {
                TradeNumbers tn = ost.tradeInfo[numid];
                sbyte num = trade.targetLotteryItem.GetNumberByIndex(numid);
                if (tn.tradeCount > 0)
                {
                    if (tn.ContainsNumber(num))
                        numPosCurTradeIndexs[numid] = 0;
                    else if (numPosCurTradeIndexs[numid] >= tradeCountList.Count)
                        numPosCurTradeIndexs[numid] = 0;
                    else
                        numPosCurTradeIndexs[numid] = numPosCurTradeIndexs[numid] + 1;
                }
            }
        }

        public void Update()
        {
            if(waitingTradeDatas.Count > 0)
            {
                for( int i = waitingTradeDatas.Count-1; i >= 0; --i )
                {
                    TradeDataBase trade = waitingTradeDatas[i];
                    trade.Update();
                    if (trade.tradeStatus == TradeStatus.eDone)
                    {
                        //if (waitingTradeDatas[i].cost != 0)
                        {
                            BatchTradeSimulator.Instance.OnOneTradeCompleted(trade);
                        }

                        RefreshTradeCountOnOneTradeCompleted(trade);
                        if (maxValue < trade.moneyAtferTrade)
                            maxValue = trade.moneyAtferTrade;
                        if (minValue > trade.moneyAtferTrade)
                            minValue = trade.moneyAtferTrade;
                        historyTradeDatas.Add(trade);
                        waitingTradeDatas.RemoveAt(i);
                        if (tradeCompletedCallBack != null)
                            tradeCompletedCallBack();
                        if (evtOneTradeCompleted != null)
                            evtOneTradeCompleted(trade);
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
            if (srt == StartTradeType.eFromFirst)
            {
                ClearAllTradeDatas();
                curTestTradeItem = DataManager.GetInst().GetFirstItem();
            }
            else if (srt == StartTradeType.eFromLatest)
            {
                ClearAllTradeDatas();
                curTestTradeItem = DataManager.GetInst().GetLatestItem();
            }
            else if (srt == StartTradeType.eFromSpec)
            {
                curTestTradeItem = DataManager.GetInst().GetDataItemByIdTag(idTag);
                if (curTestTradeItem == null)
                    curTestTradeItem = DataManager.GetInst().GetFirstItem();

                int dateID = int.Parse(idTag.Split('-')[0]);
                for (int i = historyTradeDatas.Count - 1; i >= 0; --i)
                {
                    if (historyTradeDatas[i].lastDateItem == null)
                        historyTradeDatas.RemoveAt(i);
                    else if (historyTradeDatas[i].lastDateItem.parent.dateID < dateID)
                        historyTradeDatas.RemoveAt(i);
                }
                for(int i = 0; i < historyTradeDatas.Count; ++i)
                {
                    DataItem litem = DataManager.GetInst().GetDataItemByIdTag(historyTradeDatas[i].lastDateItem.idTag);
                    DataItem titem = DataManager.GetInst().GetDataItemByIdTag(historyTradeDatas[i].targetLotteryItem.idTag);
                    historyTradeDatas[i].lastDateItem = litem;
                    historyTradeDatas[i].targetLotteryItem = titem;
                    historyTradeDatas[i].INDEX = i;
                }
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
            pauseAutoTrade = false;
            needGetLatestItem = false;
            curTestTradeItem = null;
            currentTradeCountIndex = -1;
            currentMoney = startMoney;
            minValue = startMoney;
            maxValue = startMoney;
            rightCount = 0;
            wrongCount = 0;
            untradeCount = 0;
            continueTradeMissCount = 0;
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
            if(GlobalSetting.G_EANBLE_ANALYZE_TOOL)
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
                case TradeStrategy.eSinglePositionPathOnArea:
                    TradeSingleSinglePositionPathOnArea(item, trade);
                    break;
                case TradeStrategy.eSinglePositionSmallestMissCountArea:
                    TradeSinglePositionSmallestMissCountArea(item, trade);
                    break;
                case TradeStrategy.eSinglePositionPathByAppearencePosibility:
                    TradeSinglePositionPathByAppearencePosibility(item, trade);
                    break;
                case TradeStrategy.eMultiNumPath:
                    TradeMultiNumPath(item, trade);
                    break;
                case TradeStrategy.eSinglePositionHotestNums:
                    TradeSinglePositionHotestNums(item, trade);
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
                case TradeStrategy.eSinglePositionCondictionsSuperposition:
                    TradeSinglePositionCondictionsSuperposition(item, trade);
                    break;
                case TradeStrategy.eSinglePositionBestPathByAvgLine:
                    TradeSinglePositionBestPathByAvgLine(item, trade);
                    break;
                case TradeStrategy.eTradeOnMacdBarGoUp:
                    TradeOnMacdBarGoUp(item, trade);
                    break;
                case TradeStrategy.eTradeOnKCurveTouchBolleanDown:
                    TradeOnKCurveTouchBolleanDown(item, trade);
                    break;
                case TradeStrategy.eTradeHotestPathNums:
                    TradeHotestPathNums(item, trade);
                    break;
                case TradeStrategy.eTradeOnSmallMissCount:
                    TradeOnSmallMissCount(item, trade);
                    break;
            }

            // 杀掉上期开出的号，为了节省开销
            if(killLastNumber)
            {
                if (item.idGlobal % 2 == 0 && currentTradeCountIndex < tradeCountList.Count - MultiTradePathCount)
                {
                    DataItem lastItem = item;// item.parent.GetPrevItem(item);
                    if (lastItem != null)
                    {
                        foreach (int numID in trade.tradeInfo.Keys)
                        {
                            SByte lastNum = lastItem.GetNumberByIndex(numID);
                            trade.tradeInfo[numID].RemoveNumber(lastNum);
                        }
                    }
                }
            }
        }

        public struct NumCDT
        {
            public int numID;
            public CollectDataType cdt;
            public int cdtID;

            public NumCDT(int i, CollectDataType _cdt, int _cdtID)
            {
                numID = i;
                cdt = _cdt;
                cdtID = _cdtID;
            }
        }

        public List<NumCDT> CalcFavorits(DataItem item)
        {
            List<NumCDT> results = new List<NumCDT>();
            if (DataManager.GetInst().GetAllDataItemCount() == 0)
                return results;

            if(item == null)
                item = DataManager.GetInst().GetLatestRealItem();
            for (int numID = 0; numID < 5; ++numID)
            {
                KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numID);
                AvgDataContainer adc5 = GraphDataManager.KGDC.GetAvgDataContainer(numID, 5);
                AvgDataContainer adc10 = GraphDataManager.KGDC.GetAvgDataContainer(numID, 10);
                MACDPointMap mpm = kddc.GetMacdPointMap(item.idGlobal);
                AvgPointMap apm5 = adc5.avgPointMapLst[item.idGlobal];
                AvgPointMap apm10 = adc10.avgPointMapLst[item.idGlobal];
                BollinPointMap bpm = kddc.GetBollinPointMap(item.idGlobal);
                KDataMap kdm = kddc.GetKDataDict(item.idGlobal);
                CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };

                for (int i = 0; i < 3; ++i)
                {
                    CollectDataType cdt = cdts[i];
                    PathCmpInfo pci = new PathCmpInfo(i, 0);
                    int missCount = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[cdt].missCount;

                    MACDPoint mp = mpm.macdpMap[cdt];
                    // macd快慢线都在0区以上
                    bool isMacdUpon0 = mp.DIF > 0 && mp.DEA > 0;
                    // 5级均线高于10级均线
                    bool is5H10 = apm5.apMap[cdt].avgKValue > apm10.apMap[cdt].avgKValue;
                    // 10级均线高于布林中轨
                    bool is10HBM = apm10.apMap[cdt].avgKValue > bpm.bpMap[cdt].midValue;
                    // k值高于5级均线
                    bool isKH5 = kdm.dataDict[cdt].KValue > apm5.apMap[cdt].avgKValue;
                    // k值高于或者等于5级均线
                    //bool isKFH5 = (kdm.dataDict[cdt].DownValue - apm5.apMap[cdt].avgKValue) / GraphDataManager.GetMissRelLength(cdt) > -0.2f;
                    // k值高于或者等于10级均线
                    bool isKFH10 = (kdm.dataDict[cdt].DownValue - apm10.apMap[cdt].avgKValue) / GraphDataManager.GetMissRelLength(cdt) > -0.2f;
                    //bool isKL10 = kdm.dataDict[cdt].UpValue < apm10.apMap[cdt].avgKValue;
                    float budist = kdm.dataDict[cdt].RelateDistTo(bpm.bpMap[cdt].upValue);

                    if (is5H10 && is10HBM && isKFH10 && isMacdUpon0)
                    {
                        results.Add(new NumCDT(numID, cdt, GraphDataManager.S_CDT_LIST.IndexOf(cdt)));
                    }
                }
            }
            return results;
        }

        int lastTradeNumID = -1;
        int lastTradePathIndex = -1;
        float lastTradePathValue = -1;
        int lastTradeMissCount = -1;

        void TradeSinglePositionBestPathByAvgLine(DataItem item, TradeDataOneStar trade)
        {
            TradeDataOneStar prevTrade = GetTrade(trade.INDEX - 1) as TradeDataOneStar;

            int tradeCount = defaultTradeCount;
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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

            int dstNumID = -1;
            int dstPathIndex = -1;
            int dstMissCount = -1;
            float dstPathValue = -1;
            for(int numID = 0; numID < 5; ++numID)
            {
                if (numID == 0 && GlobalSetting.G_SIM_SEL_NUM_AT_POS_0 == false)
                    continue;
                if (numID == 1 && GlobalSetting.G_SIM_SEL_NUM_AT_POS_1 == false)
                    continue;
                if (numID == 2 && GlobalSetting.G_SIM_SEL_NUM_AT_POS_2 == false)
                    continue;
                if (numID == 3 && GlobalSetting.G_SIM_SEL_NUM_AT_POS_3 == false)
                    continue;
                if (numID == 4 && GlobalSetting.G_SIM_SEL_NUM_AT_POS_4 == false)
                    continue;
                KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numID);
                AvgDataContainer adc5 = GraphDataManager.KGDC.GetAvgDataContainer(numID, 5);
                AvgDataContainer adc10 = GraphDataManager.KGDC.GetAvgDataContainer(numID, 10);
                MACDPointMap mpm = kddc.GetMacdPointMap(item.idGlobal);
                AvgPointMap apm5 = adc5.avgPointMapLst[item.idGlobal];
                AvgPointMap apm10 = adc10.avgPointMapLst[item.idGlobal];
                BollinPointMap bpm = kddc.GetBollinPointMap(item.idGlobal);
                KDataMap kdm = kddc.GetKDataDict(item.idGlobal);
                CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };

                List<PathCmpInfo> res = trade.pathCmpInfos[numID];
                res.Clear();

                for (int i = 0; i < 3; ++i)
                {
                    CollectDataType cdt = cdts[i];
                    PathCmpInfo pci = new PathCmpInfo(i, 0);
                    int missCount = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[cdt].missCount;
                    pci.pathValue = 0;
                    pci.paramMap["MissCount"] = missCount;
                    res.Add(pci);

                    if (prevTrade == null)
                        continue;

                    MACDPoint mp = mpm.macdpMap[cdt];
                    bool isMacdUpon0 = mp.DIF > 0 && mp.DEA > 0;
                    bool is5H10 = apm5.apMap[cdt].avgKValue > apm10.apMap[cdt].avgKValue;
                    bool is10HBM = apm10.apMap[cdt].avgKValue > bpm.bpMap[cdt].midValue;
                    bool isKH5 = kdm.dataDict[cdt].KValue > apm5.apMap[cdt].avgKValue;
                    bool isKFH5 = (kdm.dataDict[cdt].DownValue - apm5.apMap[cdt].avgKValue) / GraphDataManager.GetMissRelLength(cdt) > -0.2f;
                    bool isKFH10 = (kdm.dataDict[cdt].DownValue - apm10.apMap[cdt].avgKValue) / GraphDataManager.GetMissRelLength(cdt) > -0.2f;
                    bool isKL10 = kdm.dataDict[cdt].UpValue < apm10.apMap[cdt].avgKValue;
                    float budist = kdm.dataDict[cdt].RelateDistTo(bpm.bpMap[cdt].upValue);

                    PathCmpInfo prevCmp = GetPathInfo(prevTrade, numID, i);
                    float prevValue = prevCmp.pathValue;
                    if (prevValue <= 0)
                    {
                        //if (is5H10 && is10HBM && isKFH5 && budist <= 0.5f && isMacdUpon0)
                        if (is5H10 && is10HBM && isKFH10 && isMacdUpon0)
                            pci.pathValue = 1;
                        else
                            pci.pathValue = prevValue - 1;
                    }
                    else
                    {
                        if (!is5H10 || isKL10)
                            pci.pathValue = 0;
                        else
                            pci.pathValue = prevValue + 1;
                    }
                    
                    if(pci.pathValue > 0)
                    {
                        bool findBetter = dstNumID == -1;
                        if(!findBetter && dstMissCount >= 0)
                        {
                            int missCmp = missCount < dstMissCount ? -1 : (dstMissCount == missCount ? 0 : 1);
                            if (missCmp == -1)
                                findBetter = true;
                            else if(missCmp == 0 && dstPathValue < pci.pathValue)
                                findBetter = true;
                        }
                        if (findBetter)
                        {
                            dstNumID = numID;
                            dstPathIndex = i;
                            dstPathValue = pci.pathValue;
                            dstMissCount = missCount;
                        }
                    }
                }
            }
            if(dstNumID != -1)
            {
                if(lastTradeNumID != -1)
                {
                    PathCmpInfo lastPci = trade.pathCmpInfos[lastTradeNumID][dstPathIndex];
                    if (lastPci.pathValue < 1 || (int)lastPci.paramMap["MissCount"] > 2)
                    {
                        lastTradeNumID = dstNumID;
                        lastTradePathIndex = dstPathIndex;
                        lastTradePathValue = dstPathValue;
                        lastTradeMissCount = dstMissCount;
                    }
                    else
                    {
                        lastTradePathValue = lastPci.pathValue;
                        lastTradeMissCount = (int)lastPci.paramMap["MissCount"];
                    }
                }
                else
                {
                    lastTradeNumID = dstNumID;
                    lastTradePathIndex = dstPathIndex;
                    lastTradePathValue = dstPathValue;
                    lastTradeMissCount = dstMissCount;
                }
            }
            else
            {
                lastTradeNumID = -1;
                lastTradePathIndex = -1;
                lastTradePathValue = -1;
                lastTradeMissCount = -1;
            }
            if (lastTradeNumID >= 0 && lastTradeMissCount < 3)
            {
                TradeNumbers tn = new TradeNumbers();
                tn.tradeCount = tradeCount;
                trade.tradeInfo.Add(lastTradeNumID, tn);
                FindOverTheoryProbabilityNums(item, lastTradeNumID, ref maxProbilityNums);
                tn.SelPath012Number(lastTradePathIndex, tradeCount, ref maxProbilityNums);
            }


            //TradeDataOneStar prevTrade = GetTrade(trade.INDEX - 1) as TradeDataOneStar;

            //int numID = simSelNumIndex;
            //if (numID == -1)
            //    numID = 0;
            //int tradeCount = defaultTradeCount;
            //if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
            //{
            //    if (tradeCountList.Count > 0)
            //    {
            //        if (currentTradeCountIndex == -1)
            //            currentTradeCountIndex = 0;
            //        tradeCount = tradeCountList[currentTradeCountIndex];
            //    }
            //}
            //else
            //    tradeCount = 0;

            //KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numID);
            //AvgDataContainer adc5 = GraphDataManager.KGDC.GetAvgDataContainer(numID, 5);
            //AvgDataContainer adc10 = GraphDataManager.KGDC.GetAvgDataContainer(numID, 10);
            //AvgPointMap apm5 = adc5.avgPointMapLst[item.idGlobal];
            //AvgPointMap apm10 = adc10.avgPointMapLst[item.idGlobal];
            //BollinPointMap bpm = kddc.GetBollinPointMap(item.idGlobal);
            //KDataMap kdm = kddc.GetKDataDict(item.idGlobal);
            //CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };

            //List<PathCmpInfo> res = trade.pathCmpInfos[numID];
            //res.Clear();

            //for (int i = 0; i < 3; ++i)
            //{
            //    CollectDataType cdt = cdts[i];
            //    PathCmpInfo pci = new PathCmpInfo(i, 0);
            //    pci.pathValue = 0;
            //    pci.paramMap["MissCount"] = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[cdt].missCount;
            //    res.Add(pci);

            //    if (prevTrade == null)
            //        continue;

            //    bool is5H10 = apm5.apMap[cdt].avgKValue > apm10.apMap[cdt].avgKValue;
            //    bool is10HBM = apm10.apMap[cdt].avgKValue > bpm.bpMap[cdt].midValue;
            //    bool isKH5 = kdm.dataDict[cdt].KValue > apm5.apMap[cdt].avgKValue;
            //    bool isKFH5 = (kdm.dataDict[cdt].DownValue - apm5.apMap[cdt].avgKValue) / GraphDataManager.GetMissRelLength(cdt) > -0.2f;
            //    bool isKL10 = kdm.dataDict[cdt].UpValue < apm10.apMap[cdt].avgKValue;
            //    float budist = kdm.dataDict[cdt].RelateDistTo(bpm.bpMap[cdt].upValue);

            //    PathCmpInfo prevCmp = GetPathInfo(prevTrade, numID, i);
            //    float prevValue = prevCmp.pathValue;
            //    if(prevValue <= 0 )
            //    {
            //        if(is5H10 && is10HBM && isKFH5 && budist <= 0.5f)
            //            pci.pathValue = 1;
            //        else
            //            pci.pathValue = prevValue - 1;
            //    }
            //    else
            //    {
            //        if (!is5H10 || isKL10)
            //            pci.pathValue = 0;
            //        else
            //            pci.pathValue = prevValue + 1;
            //    }
            //}
            //res.Sort((a, b) => 
            //{
            //    if (a.pathValue > b.pathValue)
            //        return -1;
            //    return 1;
            //});

            //if (res[0].pathValue > 0 && (byte)res[0].paramMap["MissCount"] < 3)
            //{
            //    TradeNumbers tn = new TradeNumbers();
            //    tn.tradeCount = tradeCount;
            //    trade.tradeInfo.Add(numID, tn);
            //    FindOverTheoryProbabilityNums(item, numID, ref maxProbilityNums);
            //    tn.SelPath012Number(res[0].pathIndex, tradeCount, ref maxProbilityNums);
            //}

        }

        public static bool CheckKDataIsFullUp(int numIndex, CollectDataType cdt, DataItem testItem)
        {
            DataItem prevItem = testItem.parent.GetPrevItem(testItem);
            if (prevItem == null)
                return false;

            KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
            KDataMap kddCur = kddc.GetKDataDict(testItem);
            KData kdCur = kddCur.GetData(cdt, false);
            BollinPoint bpCur = kddc.GetBollinPointMap(kddCur).GetData(cdt, false);
            MACDPoint macdCur = kddc.GetMacdPointMap(kddCur).GetData(cdt, false);
            bool isCurTouchBU = kdCur.RelateDistTo(bpCur.upValue) <= 0;

            KDataMap kddPrv = kddc.GetKDataDict(prevItem);
            KData kdPrv = kddCur.GetData(cdt, false);
            BollinPoint bpPrv = kddc.GetBollinPointMap(kddPrv).GetData(cdt, false);
            MACDPoint macdPrv = kddc.GetMacdPointMap(kddPrv).GetData(cdt, false);
            bool isPrvTouchBU = kdPrv.RelateDistTo(bpPrv.upValue) <= 0;

            return isCurTouchBU && isPrvTouchBU && macdCur.BAR > macdPrv.BAR && macdCur.DIF > macdPrv.DIF;
        }

        bool[] NeedResetTradeCountID = new bool[] { false, false, false, false, false, };
        void TradeOnMacdBarGoUp(DataItem item, TradeDataOneStar trade)
        {
            bool[] sim_flag = new bool[]
            {
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_0,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_1,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_2,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_3,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_4,
            };

            for (int numID = 0; numID < 5; ++numID)
            {
                if (!sim_flag[numID])
                    continue;

                int tradeCount = defaultTradeCount;
                if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
                {
                    if (tradeCountList.Count > 0)
                    {
                        if (numPosCurTradeIndexs[numID] >= tradeCountList.Count)
                            numPosCurTradeIndexs[numID] = 0;
                        tradeCount = tradeCountList[numPosCurTradeIndexs[numID]];
                    }
                }
                else
                    tradeCount = 0;

                KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numID);
                MACDPointMap mpm = kddc.GetMacdPointMap(item.idGlobal);
                BollinPointMap bpm = kddc.GetBollinPointMap(item.idGlobal);
                KDataMap kdm = kddc.GetKDataDict(item.idGlobal);
                CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };

                List<PathCmpInfo> res = trade.pathCmpInfos[numID];
                res.Clear();

                bool hasFindValidPath = false;
                FindOverTheoryProbabilityNums(item, numID, ref maxProbilityNums);
                for (int i = 0; i < 3; ++i)
                {
                    CollectDataType cdt = cdts[i];
                    PathCmpInfo pci = new PathCmpInfo(i, 0);
                    int missCount = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[cdt].missCount;
                    pci.pathValue = 0;
                    pci.paramMap["MissCount"] = missCount;
                    res.Add(pci);

                    //if (missCount == 0)
                    //    continue;
                    //MACDPoint latestMP = mpm.GetData(cdt, false);
                    //if (latestMP.BAR > 0)
                    //    continue;

                    DataItem headItem = DataManager.GetInst().FindDataItem(item.idGlobal - missCount);
                    if (headItem == null)
                        continue;
                    bool isHeadItemTouchBolleanUp = CheckKDataIsFullUp(numID, cdt, headItem);
                    if (isHeadItemTouchBolleanUp == false)
                        continue;

                    KDataMap headKDM = kddc.GetKDataDict(headItem);
                    MACDPoint minMP = null;
                    while (headKDM != null && headKDM.index <= kdm.index)
                    {
                        MACDPoint curMP = kddc.GetMacdPointMap(headKDM).GetData(cdt, false);
                        if (minMP == null || minMP.BAR > curMP.BAR)
                            minMP = curMP;
                        headKDM = kddc.GetKDataDict(headKDM.index + 1);
                    }
                    if (missCount < 3)
                    {
                        //pci.pathValue = 1;
                        //hasFindValidPath = true;
                        //NeedResetTradeCountID[numID] = true;
                    }
                    else if (minMP.parent.index < bpm.index && minMP.BAR < 0)
                    {
                        pci.pathValue = 1;
                        hasFindValidPath = true;
                        if (NeedResetTradeCountID[numID] == true)
                        {
                            numPosCurTradeIndexs[numID] = 1;
                            tradeCount = tradeCountList[numPosCurTradeIndexs[0]];
                            NeedResetTradeCountID[numID] = false;
                        }
                    }
                }
                if(hasFindValidPath)
                {
                    TradeNumbers tn = new TradeNumbers();
                    tn.tradeCount = tradeCount;
                    for (int i = 0; i < 3; ++i)
                    {
                        if(res[i].pathValue > 0)
                            tn.SelPath012Number(res[i].pathIndex, tradeCount, ref maxProbilityNums);
                    }
                    trade.tradeInfo.Add(numID, tn);
                    if (numPosCurTradeIndexs[numID] == tradeCountList.Count - 1)
                        numPosCurTradeIndexs[numID] = 0;
                }
            }
        }

        void TradeOnKCurveTouchBolleanDown(DataItem item, TradeDataOneStar trade)
        {
            bool[] sim_flag = new bool[]
            {
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_0,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_1,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_2,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_3,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_4,
            };

            for (int numID = 0; numID < 5; ++numID)
            {
                if (!sim_flag[numID])
                    continue;

                int tradeCount = defaultTradeCount;
                if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
                {
                    if (tradeCountList.Count > 0)
                    {
                        if (numPosCurTradeIndexs[numID] >= tradeCountList.Count)
                            numPosCurTradeIndexs[numID] = 0;
                        tradeCount = tradeCountList[numPosCurTradeIndexs[numID]];
                    }
                }
                else
                    tradeCount = 0;

                KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numID);
                MACDPointMap mpm = kddc.GetMacdPointMap(item.idGlobal);
                BollinPointMap bpm = kddc.GetBollinPointMap(item.idGlobal);
                KDataMap kdm = kddc.GetKDataDict(item.idGlobal);
                CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };

                List<PathCmpInfo> res = trade.pathCmpInfos[numID];
                res.Clear();

                bool hasFindValidPath = false;
                FindOverTheoryProbabilityNums(item, numID, ref maxProbilityNums);
                for (int i = 0; i < 3; ++i)
                {
                    CollectDataType cdt = cdts[i];
                    PathCmpInfo pci = new PathCmpInfo(i, 0);
                    int missCount = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[cdt].missCount;
                    pci.pathValue = 0;
                    pci.paramMap["MissCount"] = missCount;
                    res.Add(pci);

                    DataItem headItem = DataManager.GetInst().FindDataItem(item.idGlobal - missCount);
                    if (headItem == null)
                        continue;
                    
                    KDataMap headKDM = kddc.GetKDataDict(headItem);
                    while (headKDM != null && headKDM.index <= kdm.index)
                    {
                        BollinPoint headBP = kddc.GetBollinPointMap(headKDM).GetData(cdt, false);
                        KData headKD = headKDM.GetData(cdt, false);
                        if(headKD.RelateDistTo(headBP.downValue) >= 0)
                        {
                            if(kdm.index - headKD.index > tradeCountList.Count)
                                break;

                            hasFindValidPath = true;
                            pci.pathValue = 1;
                            if (NeedResetTradeCountID[numID] == true)
                            {
                                numPosCurTradeIndexs[numID] = 1;
                                tradeCount = tradeCountList[numPosCurTradeIndexs[0]];
                                NeedResetTradeCountID[numID] = false;
                            }
                            break;
                        }
                        headKDM = kddc.GetKDataDict(headKDM.index + 1);
                    }
                }
                if (hasFindValidPath)
                {
                    TradeNumbers tn = new TradeNumbers();
                    tn.tradeCount = tradeCount;
                    for (int i = 0; i < 3; ++i)
                    {
                        if (res[i].pathValue > 0)
                            tn.SelPath012Number(res[i].pathIndex, tradeCount, ref maxProbilityNums);
                    }
                    trade.tradeInfo.Add(numID, tn);
                    if (numPosCurTradeIndexs[numID] == tradeCountList.Count - 1)
                        numPosCurTradeIndexs[numID] = 0;
                }
            }

        }

        void TradeOnSmallMissCount(DataItem item, TradeDataOneStar trade)
        {
            int startID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
            int endID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath2);

            bool[] sim_flag = new bool[]
            {
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_0,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_1,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_2,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_3,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_4,
            };
            for (int numID = 0; numID < 5; ++numID)
            {
                if (!sim_flag[numID])
                    continue;

                predict_results.Clear();
                int tradeCount = defaultTradeCount;
                if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
                {
                    if (tradeCountList.Count > 0)
                    {
                        if (numPosCurTradeIndexs[numID] >= tradeCountList.Count)
                            numPosCurTradeIndexs[numID] = 0;
                        tradeCount = tradeCountList[numPosCurTradeIndexs[numID]];
                    }
                }
                else
                    tradeCount = 0;

                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numID];
                for (int i = startID; i <= endID; ++i)
                {
                    CollectDataType pcdt = GraphDataManager.S_CDT_LIST[i];
                    StatisticUnit su = sum.statisticUnitMap[pcdt];
                    int pathID = i - startID;
                    if(su.missCount < 2)
                        if (predict_results.Contains(pathID) == false)
                            predict_results.Add(pathID);
                }
                if (predict_results.Count > 0)
                {
                    FindOverTheoryProbabilityNums(item, numID, ref maxProbilityNums);
                    TradeNumbers tn = new TradeNumbers();
                    tn.tradeCount = tradeCount;
                    for (int i = 0; i < predict_results.Count; ++i)
                    {
                        tn.SelPath012Number(predict_results[i], tradeCount, ref maxProbilityNums);
                    }
                    trade.tradeInfo.Add(numID, tn);

                    if (numPosCurTradeIndexs[numID] == tradeCountList.Count - 1)
                        numPosCurTradeIndexs[numID] = 0;
                }
            }

        }

        void TradeSinglePositionBestPath(DataItem item, TradeDataOneStar trade)
        {
            float maxV = -10;
            int bestNumIndex = -1;
            int bestPath = -1;
            bool isFinalPathStrongUp = false;
            //if (simSelNumIndex == -1)
            //{
            //    for (int i = 0; i < 5; ++i)
            //    {
            //        JudgeNumberPath(item, trade, i, ref maxV, ref bestNumIndex, ref bestPath, ref isFinalPathStrongUp);
            //    }
            //}
            //else
            //{
            //    JudgeNumberPath(item, trade, simSelNumIndex, ref maxV, ref bestNumIndex, ref bestPath, ref isFinalPathStrongUp);
            //}
            if (simSelNumIndex == -1)
                simSelNumIndex = 0;
            JudgeNumberPath(item, trade, simSelNumIndex, ref maxV, ref bestNumIndex, ref bestPath, ref isFinalPathStrongUp);

            if (bestNumIndex >= 0 && bestPath >= 0)
            {
                int tradeCount = defaultTradeCount;
                if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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

                FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);

                //TradeNumbers tn = new TradeNumbers();
                //tn.tradeCount = tradeCount;
                //tn.SelPath012Number(bestPath, tradeCount, ref maxProbilityNums);
                //trade.tradeInfo.Add(bestNumIndex, tn);
                //if (TradeDataManager.Instance.RiskControl > 0 && trade.CalcCost() > currentMoney * TradeDataManager.Instance.RiskControl)
                //{
                //    currentTradeCountIndex = 0;
                //    tradeCount = tradeCountList[currentTradeCountIndex];
                //    tn.tradeCount = tradeCount;
                //}
                List<PathCmpInfo> res = trade.pathCmpInfos[bestNumIndex];
                TradeNumbers tn = new TradeNumbers();
                tn.tradeCount = tradeCount;
                if (res[0].pathValue > 0)
                    tn.SelPath012Number(res[0].pathIndex, tradeCount, ref maxProbilityNums);                
                if (res[1].pathValue > 0)
                {
                    float rate = Math.Abs(res[0].pathValue - res[1].pathValue) / res[0].pathValue;
                    if (//rate < 0.1f || 
                        currentTradeCountIndex > tradeCountList.Count - MultiTradePathCount)
                        tn.SelPath012Number(res[1].pathIndex, tradeCount, ref maxProbilityNums);
                }
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
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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

            if (currentTradeCountIndex >= tradeCountList.Count - MultiTradePathCount)
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
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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
            float firstPV = pci.pathValue;
            tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
            if(currentTradeCountIndex > tradeCountList.Count - MultiTradePathCount)
            {
                pci = trade.pathCmpInfos[bestNumIndex][1];
                tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
            }
            //else
            //{
            //    pci = trade.pathCmpInfos[bestNumIndex][1];
            //    if(pci.pathValue == 0)
            //        tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
            //}
        }

        int m_lastTradePath = -1;
        void TradeSingleSinglePositionPathOnArea(DataItem item, TradeDataOneStar trade)
        {
            int bestNumIndex = 0;
            if (simSelNumIndex != -1)
                bestNumIndex = simSelNumIndex;

            int tradeCount = defaultTradeCount;
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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

            CalcPaths(item, bestNumIndex, trade);

            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            trade.tradeInfo.Add(bestNumIndex, tn);
            FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);

            trade.pathCmpInfos[bestNumIndex].Sort(
            (x, y) =>
            {
                if (x.avgPathValue > y.avgPathValue)
                    return -1;
                else if (x.avgPathValue < y.avgPathValue)
                    return 1;
                return 0;
            });
            PathCmpInfo pci = trade.pathCmpInfos[bestNumIndex][0];
            tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
            int lastSelPathID = pci.pathIndex;

            if (currentTradeCountIndex > tradeCountList.Count - MultiTradePathCount)
            {
                trade.pathCmpInfos[bestNumIndex].Sort(
                (x, y) =>
                {
                    if (x.pathValue > y.pathValue)
                        return -1;
                    else if (x.pathValue < y.pathValue)
                        return 1;
                    return 0;
                });
                if(trade.pathCmpInfos[bestNumIndex][0].pathIndex != lastSelPathID)
                {
                    tn.SelPath012Number(trade.pathCmpInfos[bestNumIndex][0].pathIndex, tradeCount, ref maxProbilityNums);
                }
            }

            //int sel_index = -1;
            //if (m_lastTradePath == -1)
            //{
            //    PathCmpInfo pci = trade.pathCmpInfos[bestNumIndex][0];
            //    if (pci.maxVertDist < tradeCountList.Count - 1)
            //    {
            //        m_lastTradePath = pci.pathIndex;
            //        sel_index = 0;
            //    }
            //}
            //else
            //{
            //    trade.pathCmpInfos[bestNumIndex].Sort(
            //    (x, y) =>
            //    {
            //        if (x.avgPathValue > y.avgPathValue)
            //            return -1;
            //        else if (x.avgPathValue < y.avgPathValue)
            //            return 1;
            //        if (x.pathValue > y.pathValue)
            //            return -1;
            //        else if (x.pathValue < y.pathValue)
            //            return 1;
            //        return 0;
            //    });

            //    for( int i = 0; i < trade.pathCmpInfos[bestNumIndex].Count; ++ i)
            //    {
            //        PathCmpInfo pci = trade.pathCmpInfos[bestNumIndex][i];
            //        if(pci.pathIndex == m_lastTradePath)
            //        {
            //            sel_index = i;
            //            bool need_change_path = false;
            //            if (Math.Abs(pci.maxVertDist) > tradeCountList.Count - 1)
            //                need_change_path = true;
            //            if (!need_change_path && pci.maxVertDist > 0)
            //                need_change_path = true;
            //            if (!need_change_path && pci.uponBMCount == 0)
            //                need_change_path = true;
            //            if (!need_change_path && pci.horzDist == 0 && pci.vertDistToBML > 0)
            //                need_change_path = true;
            //            if (need_change_path)
            //            {
            //                m_lastTradePath = -1;
            //                PathCmpInfo pci0 = trade.pathCmpInfos[bestNumIndex][0];
            //                if (pci0.maxVertDist < 0)
            //                {
            //                    m_lastTradePath = pci0.pathIndex;
            //                    sel_index = 0;
            //                }
            //            }
            //            break;
            //        }
            //    }
            //}

            //if (sel_index != -1)
            //{
            //    PathCmpInfo pci = trade.pathCmpInfos[bestNumIndex][sel_index];
            //    if (pci.maxVertDist <= 0)
            //        tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);

            //    //if (currentTradeCountIndex >= tradeCountList.Count - MultiTradePathCount)
            //    {
            //        trade.pathCmpInfos[bestNumIndex].Sort((x, y) =>
            //        {
            //            if (x.pathValue > y.pathValue)
            //                return -1;
            //            else if (x.pathValue < y.pathValue)
            //                return 1;
            //            return 0;
            //        });
            //        pci = trade.pathCmpInfos[bestNumIndex][0];
            //        if (m_lastTradePath != pci.pathIndex)
            //        {
            //            tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
            //        }
            //    }
            //}
        }

        void TradeSinglePositionSmallestMissCountArea(DataItem item, TradeDataOneStar trade)
        {
            int bestNumIndex = 0;
            if (simSelNumIndex != -1)
                bestNumIndex = simSelNumIndex;

            int tradeCount = defaultTradeCount;
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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

            CalcPathMissCountArea(item, trade, bestNumIndex);

            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            trade.tradeInfo.Add(bestNumIndex, tn);
            FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);


            PathCmpInfo pci = trade.pathCmpInfos[bestNumIndex][0];
            tn.SelPath012Number(pci.pathIndex, tradeCount, ref maxProbilityNums);
            int lastSelPathID = pci.pathIndex;

            if (currentTradeCountIndex > tradeCountList.Count - MultiTradePathCount)
            {
                tn.SelPath012Number(trade.pathCmpInfos[bestNumIndex][1].pathIndex, tradeCount, ref maxProbilityNums);
            }
        }

        void TradeSinglePositionPathByAppearencePosibility(DataItem item, TradeDataOneStar trade)
        {
            int bestNumIndex = 0;
            if (simSelNumIndex != -1)
                bestNumIndex = simSelNumIndex;
            int tradeCount = defaultTradeCount;
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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

            if(CurrentTradeCountIndex == 0)
            {
                downFromTopCheckCount = 2;
            }

            CalcPathAppearence(item, trade, bestNumIndex);

            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            trade.tradeInfo.Add(bestNumIndex, tn);
            FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);
            PathCmpInfo pci0 = trade.pathCmpInfos[bestNumIndex][0];

            if(GlobalSetting.G_TRADE_IMMEDIATE_ON_MISS_COUNT0_PATH)
            {
                tn.SelPath012Number(pci0.pathIndex, tradeCount, ref maxProbilityNums);
                return;
            }
            if (GlobalSetting.G_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_UP)
            {
                if ((float)pci0.paramMap["dist2BU"] > 1.2f)
                    return;
            }

            bool tradeImmediate = false;
            // 交易落在布林中轨的k线
            if (GlobalSetting.G_TRADE_IMMEDIATE_AT_BOLLEAN_MID)
            {
                if (GlobalSetting.G_COLLECT_BOLLEAN_ANALYZE_DATA)
                {
                    float count2BM = (float)pci0.paramMap["count2BMs"];
                    if (Math.Abs(count2BM) < 1)
                    {
                        tradeImmediate = true;
                    }
                }
            }
            // 交易刚接触布林下轨的k线
            if (GlobalSetting.G_TRADE_IMMEDIATE_AT_TOUCH_BOLLEAN_DOWN)
            {
                int onBDCC = (int)pci0.paramMap["onDownCC"];
                if (onBDCC > 0 && onBDCC < 2)
                    tradeImmediate = true;
            }
            // 交易在布林中轨之上持续上升的k线
            if(GlobalSetting.G_TRADE_IMMEDIATE_ON_CONTINUE_HIT_UPON_BOLLEAN_MID)
            {
                float dist2BU = (float)pci0.paramMap["dist2BU"];
                if (Math.Abs(dist2BU) <= 1 ||
                    ((int)pci0.paramMap["upMC"] > 0 && 
                     (int)pci0.paramMap["curMissCount"] < GlobalSetting.G_TRADE_IMMEDIATE_TORANCE_MAX_MISS_COUNT))
                {
                    tradeImmediate = true;
                }
            }

            if (tradeImmediate == false)
            {
                // 是否在布林下轨连续开出就忽略当前的交易
                if (GlobalSetting.G_IGNORE_CUR_TRADE_ON_BOLLEAN_DOWN_CONTINUE)
                {
                    // K线如果超过2期运行到布林下轨，那么就不做交易了
                    if ((int)pci0.paramMap["onDownCC"] > 2)
                        return;
                }
                // 是否在布林上轨连续超过3期没开出就忽略当前的交易
                if (GlobalSetting.G_IGNORE_CUR_TRADE_ON_BOLLEAN_UP_CONTINUE_MISS)
                {
                    if (GlobalSetting.G_COLLECT_BOLLEAN_ANALYZE_DATA)
                    {
                        if ((int)pci0.paramMap["KDownFromTop"] == 1)
                            return;
                    }
                }
                if(GlobalSetting.G_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_MID)
                {
                    if (Math.Abs((float)pci0.paramMap["count2BMs"]) > 1)
                        return;
                }
                if(GlobalSetting.G_IGNORE_CUR_TRADE_ON_NOT_CONTINUE_HIT_UPON_BOLLEAN_MID)
                {
                    float dist2BU = (float)pci0.paramMap["dist2BU"];
                    if (!(Math.Abs(dist2BU) <= 1 ||
                         ((int)pci0.paramMap["midKUC"] > 0 &&
                          (int)pci0.paramMap["upMC"] > 0 &&
                          (int)pci0.paramMap["curMissCount"] < GlobalSetting.G_TRADE_IMMEDIATE_TORANCE_MAX_MISS_COUNT)))
                    {
                        return;
                    }
                }


                if (GlobalSetting.G_ONLY_TRADE_BEST_PATH)
                {
                    if (GlobalSetting.G_ENABLE_CHECK_PATH_CAN_TRADE == false)
                    {
                        if ((bool)pci0.paramMap["isAppRatePrefer"] == false)
                            return;
                        /*
                        int checkCounts = tradeCountList.Count - 3;
                        if ((int)pci0.paramMap["maxMissCount"] < checkCounts)
                            checkCounts = (int)pci0.paramMap["maxMissCount"];

                        if ((int)pci0.paramMap["underTheoRateCount"] > 1)
                        {
                            return;
                        }

                        if ((int)pci0.paramMap["maxMissCountID"] < item.idGlobal)
                        {
                            if ((int)pci0.paramMap["curMissCount"] > checkCounts)
                                return;
                        }
                        else if ((int)pci0.paramMap["curMissCount"] > 2)
                        {
                            return;
                        }
                        */
                    }
                    else
                    {
                        if(GlobalSetting.G_ENABLE_MACD_UP_CHECK && GlobalSetting.G_SEQ_PATH_BY_MACD_LINE)
                        {
                            if ((int)pci0.paramMap["MacdUp"] == 0)
                                return;
                        }

                        if(GlobalSetting.G_ENABLE_MACD_UP_CHECK && GlobalSetting.G_SEQ_PATH_BY_MACD_SIGNAL)
                        {
                            if ((MacdSignal)pci0.paramMap["MacdSig"] < MacdSignal.eHalfDown)
                                return;
                        }

                        // K线是否在布林中轨
                        bool isOnBolleanMid = Math.Abs((float)pci0.paramMap["count2BMs"]) <= 1.2;
                        // 判断K值是否落在支撑线上的检测距离
                        float downLineCheckTor = 0.2f;// 1.2f;
                                                      // K线与支撑线的关系：0 - 在支撑线上， 1 - 在支撑线上方， -1 - 在支撑线下方
                        int relationShipToDownLine = 0;
                        // 剩余交易次数是否可用
                        bool isTradeCountLeftNotEnough = false;
                        // 下支撑线的斜率
                        float downLineSlope = 0;
                        // 从支撑线判断当前是否处于支撑线下方或者剩余交易次数不足
                        bool isKCurveUnderDownLineOrTradeCountNotEnough = false;
                        float distFromCur2Min = 999;
                        // 是否在布林中轨之下运行且出现抬升转折点了
                        bool isUnderBolleanMidAndBecomeUp = false;
                        if (GlobalSetting.G_ENABLE_MACD_UP_CHECK)
                        {
                            isUnderBolleanMidAndBecomeUp =
                            (int)pci0.paramMap["KKeepDown"] == 2 &&
                            (int)pci0.paramMap["KAtBMDown"] == 1;
                        }
                        // 布林中轨是否转而向下了
                        bool isBolleanMidBecomeDown = (float)pci0.paramMap["bpmDelta"] < 0;
                        bool isNearDL = false;


                        if (GlobalSetting.G_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL)
                        {
                            // 还剩下多少笔交易可用
                            int countLeft = tradeCountList.Count - CurrentTradeCountIndex;
                            // 下后支撑线存在
                            if ((int)pci0.paramMap["DLNext"] == 1)
                            {
                                float vdist = (float)pci0.paramMap["DLNextVDist"];
                                // k线穿出下后支撑线了
                                if (vdist > downLineCheckTor)
                                    relationShipToDownLine = -1;
                                // k线在下后支撑线上方
                                else if (vdist < -downLineCheckTor)
                                    relationShipToDownLine = 1;
                                // k线落在下后支撑线上
                                else
                                    relationShipToDownLine = 0;

                                // 从当前的K线按照下降的趋势做延长线到下后支撑线的交点，
                                // 那么可以认为剩余次数不足了
                                if ((int)pci0.paramMap["DLHasNextHitPt"] == 1)
                                {
                                    float count2DL = (float)pci0.paramMap["DLNextHitPtXOF"];
                                    if (count2DL > countLeft)
                                        isTradeCountLeftNotEnough = true;
                                    if (0 < count2DL && count2DL <= 2)
                                        isNearDL = true;
                                }
                                downLineSlope = (float)pci0.paramMap["DLNextSlope"];
                                distFromCur2Min = (float)pci0.paramMap["DLNextDist2Min"];
                            }
                            else if ((int)pci0.paramMap["DLPrev"] == 1)
                            {
                                float vdist = (float)pci0.paramMap["DLPrevVDist"];
                                // k线穿出下前支撑线了
                                if (vdist > downLineCheckTor)
                                    relationShipToDownLine = -1;
                                // k线在下前支撑线上方
                                else if (vdist < -downLineCheckTor)
                                    relationShipToDownLine = 1;
                                // k线落在下前支撑线上
                                else
                                    relationShipToDownLine = 0;

                                // 从当前的K线按照下降的趋势做延长线到下前支撑线的交点，
                                // 那么可以认为剩余次数不足了
                                if ((int)pci0.paramMap["DLHasPrevHitPt"] == 1)
                                {
                                    float count2DL = (float)pci0.paramMap["DLPrevHitPtXOF"];
                                    if (count2DL > countLeft)
                                        isTradeCountLeftNotEnough = true;
                                    if (0 < count2DL && count2DL <= 2)
                                        isNearDL = true;
                                }
                                downLineSlope = (float)pci0.paramMap["DLPrevSlope"];
                                distFromCur2Min = (float)pci0.paramMap["DLPrevDist2Min"];
                            }
                        }


                        // test only trade upon bollean mid
                        //if ((int)pci0.paramMap["BollBandLE3Count"] < 3)
                        bool shouldTrade =
                            (int)pci0.paramMap["KDownFromTop"] != 1 &&
                            (
                                (int)pci0.paramMap["aprC"] >= 3
                                || (int)pci0.paramMap["onMCC"] > 0
                            );
                        //|| (isNearDL && downLineSlope >= 0.0)
                        //|| relationShipToDownLine == 0;
                        if (!shouldTrade)
                        {
                            // 如果是在布林中轨上方超过2期没开出，放弃之
                            if ((int)pci0.paramMap["KDownFromTop"] == 1)
                                return;
                            // 如果当前运行在中轨之下，放弃之
                            if ((int)pci0.paramMap["dnMCC"] > 0)
                                return;
                            // 如果少于3期在布林中轨及之上，就放弃之
                            int upMC = (int)pci0.paramMap["upMC"];
                            int onMC = (int)pci0.paramMap["onMC"];
                            int onMCC = (int)pci0.paramMap["onMCC"];
                            int upAndOnMC = upMC + onMC;
                            // 如果当前在中轨以及之上的期数小于3，放弃之
                            if (upAndOnMC < 3)
                                return;
                            // 如果超出3期没开出或者当前k线运行到布林中轨之下，放弃之
                            if (!(onMCC >= 3 || upMC > 0))
                                return;
                            // 如果布林中轨往下运行，放弃之
                            if ((float)pci0.paramMap["bpmDelta"] < -0.01)
                                return;
                        }

                        /*
                        if (GlobalSetting.G_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL)
                        {
                            // 还剩下多少笔交易可用
                            int countLeft = tradeCountList.Count - CurrentTradeCountIndex;
                            // 下后支撑线存在
                            if ((int)pci0.paramMap["DLNext"] == 1)
                            {
                                float vdist = (float)pci0.paramMap["DLNextVDist"];
                                // k线穿出下后支撑线了
                                if (vdist > downLineCheckTor)
                                    relationShipToDownLine = -1;
                                // k线在下后支撑线上方
                                else if (vdist < -downLineCheckTor)
                                    relationShipToDownLine = 1;
                                // k线落在下后支撑线上
                                else
                                    relationShipToDownLine = 0;

                                // 从当前的K线按照下降的趋势做延长线到下后支撑线的交点，
                                // 那么可以认为剩余次数不足了
                                if ((int)pci0.paramMap["DLHasNextHitPt"] == 1 && (float)pci0.paramMap["DLNextHitPtXOF"] > countLeft)
                                    isTradeCountLeftNotEnough = true;
                                downLineSlope = (float)pci0.paramMap["DLNextSlope"];
                                distFromCur2Min = (float)pci0.paramMap["DLNextDist2Min"];
                            }
                            else if ((int)pci0.paramMap["DLPrev"] == 1)
                            {
                                float vdist = (float)pci0.paramMap["DLPrevVDist"];
                                // k线穿出下前支撑线了
                                if (vdist > downLineCheckTor)
                                    relationShipToDownLine = -1;
                                // k线在下前支撑线上方
                                else if (vdist < -downLineCheckTor)
                                    relationShipToDownLine = 1;
                                // k线落在下前支撑线上
                                else
                                    relationShipToDownLine = 0;

                                // 从当前的K线按照下降的趋势做延长线到下前支撑线的交点，
                                // 那么可以认为剩余次数不足了
                                if ((int)pci0.paramMap["DLHasPrevHitPt"] == 1 && (float)pci0.paramMap["DLPrevHitPtXOF"] > countLeft)
                                    isTradeCountLeftNotEnough = true;
                                downLineSlope = (float)pci0.paramMap["DLPrevSlope"];
                                distFromCur2Min = (float)pci0.paramMap["DLPrevDist2Min"];
                            }
                            // 是否长期处于布林中轨之下，且下支撑线是抬升的，当前离支撑点在3个单位之内
                            bool isLongUnderBMAndMayBecomeUp =
                                //downLineSlope > 0 &&
                                //distFromCur2Min > -1 && distFromCur2Min <= 3 &&
                                isUnderBolleanMidAndBecomeUp;

                            isKCurveUnderDownLineOrTradeCountNotEnough = relationShipToDownLine == -1;// || isTradeCountLeftNotEnough;
                            // 如果k线穿过下支撑线或者剩余交易次数不足，那么可以认为当前的交易可以放弃了
                            if (isKCurveUnderDownLineOrTradeCountNotEnough && 
                                !(isOnBolleanMid || isLongUnderBMAndMayBecomeUp))
                            {
                                return;
                            }
                        }

                        if (GlobalSetting.G_ENABLE_BOLLEAN_CFG_CHECK)
                        {
                            //BolleanBandCfg bbCfg = (BolleanBandCfg)pci0.paramMap["BBandCfg"];
                            //if(bbCfg == BolleanBandCfg.eNone)
                            //    return;
                            //else if(bbCfg == BolleanBandCfg.eBecomeLarge || bbCfg == BolleanBandCfg.eBecomeSmall)
                            //{
                            //    if ((int)pci0.paramMap["curMissCount"] > 3 && Math.Abs((float)pci0.paramMap["count2BMs"]) > 1)
                            //        return;
                            //}


                            // k线在布林中轨 或者 k线在下支撑线
                            bool isOnBolleanMidOrOnDownLine = isOnBolleanMid || relationShipToDownLine == 0;
                            // 判断当前的交易是否需要取消
                            bool isCurTradeShouldCancel =
                                // 是否超过3次没出了
                                (int)pci0.paramMap["curMissCount"] > 3
                                // 且剩余次数足够
                                && !isTradeCountLeftNotEnough
                                // 且没有出现下面的情况(k线在布林中轨 或者 k线在下支撑线)
                                && !isOnBolleanMidOrOnDownLine
                                // 如果没有出现抬升转折点
                                && !isUnderBolleanMidAndBecomeUp;
                            if (isCurTradeShouldCancel)
                            {
                                return;
                            }

                            BolleanCfg bmCFG = (BolleanCfg)pci0.paramMap["BMCfg"];
                            if (bmCFG == BolleanCfg.eNone ||
                                bmCFG == BolleanCfg.eDown ||
                                bmCFG == BolleanCfg.eFirstUpThenDown ||
                                isBolleanMidBecomeDown)
                            {
                                if(GlobalSetting.G_ENABLE_MACD_UP_CHECK)
                                {
                                    if (//isOnBolleanMidOrOnDownLine == false && 
                                        downLineSlope < 0 &&
                                        (int)pci0.paramMap["KKeepDown"] == 1 &&
                                        (int)pci0.paramMap["KAtBMDown"] == 1)
                                        return;
                                }
                                else
                                    return;
                            }
                        }

                        //if ((int)pci0.paramMap["KAtBMDown"] == 1)
                        //    return;

                        if (GlobalSetting.G_ENABLE_MACD_UP_CHECK)
                        {
                            //// 当前没有出现坚持等待的信号时
                            //if ((int)pci0.paramMap["WaitUp"] == 0)
                            //{
                            //    int tradeDistToMax = tradeCountList.Count - currentTradeCountIndex;
                            //    if (// K线在布林中轨下方下降运行，就放弃
                            //        (int)pci0.paramMap["KKeepDown"] == 1
                            //         ||
                            //        // 如果K线没出现反弹信号且剩下的交易次数小于MultiTradePathCount，就放弃
                            //        ((tradeDistToMax < MultiTradePathCount) && (int)pci0.paramMap["KKeepDown"] != 2)
                            //         ||
                            //        // 如果当前的遗漏超过3，且MACD柱不是上升或者触底反弹的，就放弃
                            //        (
                            //            (int)pci0.paramMap["MBAR"] <= 0 &&
                            //            (int)pci0.paramMap["curMissCount"] > 3
                            //        )
                            //         ||
                            //        // 如果当前的遗漏超过3，当前遗漏超过最大遗漏且K线是纯下行的话，就放弃
                            //        (
                            //            (float)pci0.paramMap["KGraph"] == -1.0f &&
                            //            (int)pci0.paramMap["curMissCount"] >= (int)pci0.paramMap["maxMissCount"]
                            //            && (int)pci0.paramMap["curMissCount"] > 3
                            //        ))
                            //        return;
                            //}

                            //MacdLineCfg cfg = (MacdLineCfg)pci0.paramMap["MacdCfg"];
                            //if (//(int)pci0.paramMap["KUP"] <= 0
                            //    //(float)pci0.paramMap["KGraph"] != 2.0f 
                            //    //|| (float)pci0.paramMap["MacdUp"] <= 0.0f
                            //    false == (cfg == MacdLineCfg.eGC || cfg == MacdLineCfg.eGCFHES)
                            //    || (int)pci0.paramMap["IsMacdPUP"] != 1
                            //    || (Math.Abs((float)pci0.paramMap["count2BMs"]) > 1 && (int)pci0.paramMap["curMissCount"] > 2)
                            //    //|| (int)pci0.paramMap["KUP"] <= 0
                            //    )
                            //    return;
                        }
                        */
                    }
                }
            }
            
            tn.SelPath012Number(pci0.pathIndex, tradeCount, ref maxProbilityNums);

            PathCmpInfo pci1 = trade.pathCmpInfos[bestNumIndex][1];
            int lastSelPathID = pci0.pathIndex;
            if (currentTradeCountIndex > tradeCountList.Count - MultiTradePathCount 
                //||  ((float)pci0.paramMap["detRate"] == (float)pci1.paramMap["detRate"]) && 
                //    ((float)pci0.paramMap["curRate"] == (float)pci1.paramMap["curRate"])
                )
            {
                tn.SelPath012Number(pci1.pathIndex, tradeCount, ref maxProbilityNums);
            }
        }

        void TradeMultiNumPath(DataItem item, TradeDataOneStar trade)
        {
            int tradeCount = defaultTradeCount;
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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

        int lastTradePath = -1;
        int[] numPosCurTradeIndexs = new int[] { 0, 0, 0, 0, 0 };
        List<int> predict_results = new List<int>();

        void TradeSinglePositionHotestNums(DataItem item, TradeDataOneStar trade)
        {
            int startID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum0);
            int endID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum9);
            
            bool[] sim_flag = new bool[] 
            {
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_0,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_1,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_2,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_3,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_4,
            };
            for(int numID = 0; numID < 5; ++numID)
            {
                if (!sim_flag[numID])
                    continue;

                predict_results.Clear();
                int tradeCount = defaultTradeCount;
                if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
                {
                    if (tradeCountList.Count > 0)
                    {
                        if (numPosCurTradeIndexs[numID] >= tradeCountList.Count)
                            numPosCurTradeIndexs[numID] = 0;
                        tradeCount = tradeCountList[numPosCurTradeIndexs[numID]];
                    }
                }
                else
                    tradeCount = 0;
                
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numID];
                for (int i = startID; i <= endID; ++i)
                {
                    CollectDataType pcdt = GraphDataManager.S_CDT_LIST[i];
                    StatisticUnit su = sum.statisticUnitMap[pcdt];
                    int num = i - startID;

                    if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_3)
                    {
                        if (su.sample3Data.appearProbabilityDiffWithTheory > 0.5f)
                        {
                            if (predict_results.Contains(num) == false)
                                predict_results.Add(num);
                        }
                    }
                    if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_5)
                    {
                        if (su.sample5Data.appearProbabilityDiffWithTheory > 0.5f)
                        {
                            if (predict_results.Contains(num) == false)
                                predict_results.Add(num);
                        }
                    }
                    if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_10)
                    {
                        if (su.sample10Data.appearProbabilityDiffWithTheory > 0.5f)
                        {
                            if (predict_results.Contains(num) == false)
                                predict_results.Add(num);
                        }
                    }
                }
                if (predict_results.Count > 0)
                {
                    //if (predict_results.Count <= GlobalSetting.G_SIM_SEL_MAX_COUNT)
                    {
                        TradeNumbers tn = new TradeNumbers();
                        tn.tradeCount = tradeCount;
                        for (int i = 0; i < predict_results.Count; ++i)
                        {
                            tn.tradeNumbers.Add(new NumberCmpInfo(predict_results[i]));
                        }
                        trade.tradeInfo.Add(numID, tn);

                        if (numPosCurTradeIndexs[numID] == tradeCountList.Count - 1)
                            numPosCurTradeIndexs[numID] = 0;
                    }
                }
            }

            /*
            List<PathCmpInfo> paths = new List<PathCmpInfo>();
            TradeDataManager.FindSpecNumIndexPathsProbabilities(item, ref paths, numID, 3);
            TradeDataManager.FindSpecNumIndexPathsProbabilities(item, ref paths, numID, 5);
            int bestPath = -1;
            if(lastTradePath != -1)
            {
                if( (float)paths[lastTradePath].paramMap["3"] < -0.5f &&
                    (float)paths[lastTradePath].paramMap["5"] < -0.5f)
                {
                    lastTradePath = -1;
                }
                else
                {
                    bestPath = lastTradePath;
                }
            }
            if(bestPath == -1)
            {
                float bestRate = -0.5f;
                for (int i = 0; i < paths.Count; ++i)
                {
                    float curRate = ((float)paths[i].paramMap["3"] + (float)paths[i].paramMap["5"]) * 0.5f;
                    if (i == 0)
                        curRate = (curRate - 40.0f) / 40.0f;
                    else
                        curRate = (curRate - 30.0f) / 30.0f;
                    if(curRate > bestRate)
                    {
                        bestRate = curRate;
                        bestPath = i;
                    }
                }
            }
            lastTradePath = bestPath;

            if (bestPath != -1)
            {
                maxProbilityNums.Clear();
                {
                    StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numID];
                    int startIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum0);
                    int endIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum9);
                    for (int num = startIndex; num <= endIndex; ++num)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[num];
                        SByte number = (SByte)(num - startIndex);
                        NumberCmpInfo info = GetNumberCmpInfo(ref maxProbilityNums, number, true);
                        //info.appearCount = sum.statisticUnitMap[cdt].sample10Data.appearCount;
                        //info.rate = sum.statisticUnitMap[cdt].sample10Data.appearProbabilityDiffWithTheory;
                        //info.largerThanTheoryProbability = info.rate > 0.5f;

                        info.appearCount = sum.statisticUnitMap[cdt].sample5Data.appearCount;
                        if (num % 3 == bestPath)
                        {
                            info.largerThanTheoryProbability = true;
                        }
                        else
                        {
                            info.largerThanTheoryProbability = false;
                        }
                    }
                }

                TradeNumbers tn = new TradeNumbers();
                tn.tradeCount = tradeCount;
                tn.SetHotNumber(tradeCount, ref maxProbilityNums);
                trade.tradeInfo.Add(numID, tn);
            }
            */
        }

        void TradeHotestPathNums(DataItem item, TradeDataOneStar trade)
        {
            int startID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
            int endID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath2);

            bool[] sim_flag = new bool[]
            {
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_0,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_1,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_2,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_3,
                GlobalSetting.G_SIM_SEL_NUM_AT_POS_4,
            };
            for (int numID = 0; numID < 5; ++numID)
            {
                if (!sim_flag[numID])
                    continue;

                predict_results.Clear();
                int tradeCount = defaultTradeCount;
                if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
                {
                    if (tradeCountList.Count > 0)
                    {
                        if (numPosCurTradeIndexs[numID] >= tradeCountList.Count)
                            numPosCurTradeIndexs[numID] = 0;
                        tradeCount = tradeCountList[numPosCurTradeIndexs[numID]];
                    }
                }
                else
                    tradeCount = 0;

                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numID];
                for (int i = startID; i <= endID; ++i)
                {
                    CollectDataType pcdt = GraphDataManager.S_CDT_LIST[i];
                    StatisticUnit su = sum.statisticUnitMap[pcdt];
                    int pathID = i - startID;

                    if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_3)
                    {
                        if (su.sample3Data.appearProbabilityDiffWithTheory > 0.5f)
                        {
                            if (predict_results.Contains(pathID) == false)
                                predict_results.Add(pathID);
                        }
                    }
                    if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_5)
                    {
                        if (su.sample5Data.appearProbabilityDiffWithTheory > 0.5f)
                        {
                            if (predict_results.Contains(pathID) == false)
                                predict_results.Add(pathID);
                        }
                    }
                    if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_10)
                    {
                        if (su.sample10Data.appearProbabilityDiffWithTheory > 0.5f)
                        {
                            if (predict_results.Contains(pathID) == false)
                                predict_results.Add(pathID);
                        }
                    }
                }
                if (predict_results.Count > 0)
                {

                    FindOverTheoryProbabilityNums(item, numID, ref maxProbilityNums);
                    TradeNumbers tn = new TradeNumbers();
                    tn.tradeCount = tradeCount;
                    for (int i = 0; i < predict_results.Count; ++i)
                    {
                        tn.SelPath012Number(predict_results[i], tradeCount, ref maxProbilityNums);
                    }
                    trade.tradeInfo.Add(numID, tn);

                    if (numPosCurTradeIndexs[numID] == tradeCountList.Count - 1)
                        numPosCurTradeIndexs[numID] = 0;
                }
            }

        }

        void TradeSingleMostPosibilityNums(DataItem item, TradeDataOneStar trade)
        {
            int tradeCount = defaultTradeCount;
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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
            byte ac0 = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[CollectDataType.ePath0].sample10Data.appearCount;
            byte ac1 = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[CollectDataType.ePath1].sample10Data.appearCount;
            byte ac2 = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[CollectDataType.ePath2].sample10Data.appearCount;
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

        void TradeSinglePositionCondictionsSuperposition(DataItem item, TradeDataOneStar trade)
        {
            int tradeCount = defaultTradeCount;
            if (item.idGlobal >= LotteryStatisticInfo.SAMPLE_COUNT_10)
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

            List<NumberCmpInfo> num_lst = new List<NumberCmpInfo>();
            for( int i = 0; i < 10; ++i)
            {
                NumberCmpInfo nci = new NumberCmpInfo();
                nci.number = (SByte)i;
                nci.rate = 0;
                num_lst.Add(nci);
            }
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numID];
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
                StatisticUnit su = sum.statisticUnitMap[cdt];
                StatisticData sd = su.sample5Data;
                float v = sd.appearProbability;
                switch(cdt)
                {
                    case CollectDataType.eNum0:
                        num_lst[0].rate += v;
                        break;
                    case CollectDataType.eNum1:
                        num_lst[1].rate += v;
                        break;
                    case CollectDataType.eNum2:
                        num_lst[2].rate += v;
                        break;
                    case CollectDataType.eNum3:
                        num_lst[3].rate += v;
                        break;
                    case CollectDataType.eNum4:
                        num_lst[4].rate += v;
                        break;
                    case CollectDataType.eNum5:
                        num_lst[5].rate += v;
                        break;
                    case CollectDataType.eNum6:
                        num_lst[6].rate += v;
                        break;
                    case CollectDataType.eNum7:
                        num_lst[7].rate += v;
                        break;
                    case CollectDataType.eNum8:
                        num_lst[8].rate += v;
                        break;
                    case CollectDataType.eNum9:
                        num_lst[9].rate += v;
                        break;

                    //case CollectDataType.eBigNum:
                    //    for( int j = 0; j < 9; ++j )
                    //    {
                    //        if(j > 5)
                    //        {
                    //            num_lst[j].rate += v;
                    //        }
                    //    }
                    //    break;
                    //case CollectDataType.eSmallNum:
                    //    for (int j = 0; j < 9; ++j)
                    //    {
                    //        if (j < 5)
                    //        {
                    //            num_lst[j].rate += v;
                    //        }
                    //    }
                    //    break;
                    //case CollectDataType.ePrimeNum:
                    //    for (int j = 0; j < 9; ++j)
                    //    {
                    //        if (j == 1 || j == 2 || j == 3 || j == 5 || j == 7)
                    //        {
                    //            num_lst[j].rate += v;
                    //        }
                    //    }
                    //    break;
                    //case CollectDataType.eCompositeNum:
                    //    for (int j = 0; j < 9; ++j)
                    //    {
                    //        if (j == 0 || j == 4 || j == 6 || j == 8 || j == 9)
                    //        {
                    //            num_lst[j].rate += v;
                    //        }
                    //    }
                    //    break;
                    //case CollectDataType.eEvenNum:
                    //    for (int j = 0; j < 9; ++j)
                    //    {
                    //        if (j % 2 == 0)
                    //        {
                    //            num_lst[j].rate += v;
                    //        }
                    //    }
                    //    break;
                    //case CollectDataType.eOddNum:
                    //    for (int j = 0; j < 9; ++j)
                    //    {
                    //        if (j % 2 == 1)
                    //        {
                    //            num_lst[j].rate += v;
                    //        }
                    //    }
                    //    break;
                    case CollectDataType.ePath0:
                        num_lst[0].rate += v;
                        num_lst[3].rate += v;
                        num_lst[6].rate += v;
                        num_lst[9].rate += v;
                        break;
                    case CollectDataType.ePath1:
                        num_lst[1].rate += v;
                        num_lst[4].rate += v;
                        num_lst[7].rate += v;
                        break;
                    case CollectDataType.ePath2:
                        num_lst[2].rate += v;
                        num_lst[5].rate += v;
                        num_lst[8].rate += v;
                        break;
                }
            }

            num_lst.Sort((x, y) =>
            {
                if (x.rate > y.rate)
                    return -1;
                else if (x.rate < y.rate)
                    return 1;
                return 0;
            });

            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            trade.tradeInfo.Add(numID, tn);

            float lastV = num_lst[0].rate;
            int min_count = MultiTradePathCount - 1;
            //min_count = MultiTradePathCount;
            for ( int i = 0; i < num_lst.Count; ++i )
            {
                tn.AddProbabilityNumber(num_lst[i]);
                if(i < min_count)
                {
                    lastV = num_lst[i].rate;
                }
                else
                {
                    if (num_lst[i].rate < lastV)
                        break;
                    if (i >= MultiTradePathCount)
                        break;
                }
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
                if (suA.sample10Data.appearProbability > suB.sample10Data.appearProbability)
                {
                    curBestPath = indexA;
                    curBestV = pathValueA;
                    curBeshSU = suA;
                }
                else if (suA.sample10Data.appearProbability < suB.sample10Data.appearProbability)
                {
                    curBestPath = indexB;
                    curBestV = pathValueB;
                    curBeshSU = suB;
                }
                else
                {
                    if (suA.sample30Data.appearProbability > suB.sample30Data.appearProbability)
                    {
                        curBestPath = indexA;
                        curBestV = pathValueA;
                        curBeshSU = suA;
                    }
                    else if (suA.sample30Data.appearProbability < suB.sample30Data.appearProbability)
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

        public static KGraphConfig CheckKGraphConfig(DataItem di, int numIndex, KDataMap item, BollinPointMap bpm, CollectDataType cdt, 
            ref int belowAvgLineCount, ref int uponAvgLineCount, ref int maxMissCount, ref int maxMissID, 
            ref int minKValueID, ref int vertCountFromCurToMinKV, ref int vertCountFromCurToBollMidLine, ref int vertCountFromCurToBollDownLine)
        {
            int[] missCountCollect = new int[4] { 0, 0, 0, 0, };
            //int maxMissCount = 0;
            maxMissCount = 0;
            vertCountFromCurToMinKV = 0;
            vertCountFromCurToBollMidLine = 0;
            int curMissCount = 0;
            int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);

            bool shouldCheckUnderAvgLineCount = true;
            belowAvgLineCount = 0;
            uponAvgLineCount = 0;
            KGraphConfig cfg = KGraphConfig.eNone;
            int loop = KGRAPH_LOOP_COUNT;
            KDataMap curItem = item;
            BollinPointMap curBPM = bpm, maxMissBPM = null;
            float rightKV = 0, leftKV = 0, maxKV = 0, minKV = 0, rightBpMid = 0, rightBpUp = 0, rightBpDown = 0, leftBpMid = 0, maxMissKV = 0, relateDist = 0;
            int rightID = -1, leftID = -1, maxID = -1, minID = -1;
            maxMissID = -1;
            rightKV = leftKV = maxKV = minKV = curItem.GetData(cdt, false).KValue;
            rightID = leftID = maxID = minID = curItem.index;
            rightBpMid = bpm.GetData(cdt, false).midValue;
            rightBpUp = bpm.GetData(cdt, false).upValue;
            rightBpDown = bpm.GetData(cdt, false).downValue;
            DataItem curDI = di;
            curMissCount = curDI.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
            maxMissCount = curMissCount;
            // 达到布林线上轨的个数
            int reachBollinUpCount = 0;
            // 最大遗漏值回溯是否碰到布林线下轨
            bool hasMaxMissCountTouchBolleanDown = false;
            DataItem maxMissDataItem = null;
            KDataMap maxMissKdd = null;
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
            minKValueID = minID;
            DataItem testMCI = maxMissDataItem;
            KDataMap testKDD = maxMissKdd;
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

            float missRelH = GraphDataManager.GetMissRelLength(cdt);
            relateDist = rightKData.RelateDistTo(minKV);
            vertCountFromCurToMinKV = (int)(relateDist / missRelH);

            relateDist = rightKData.RelateDistTo(rightBpDown);
            vertCountFromCurToBollDownLine = (int)(relateDist / missRelH);

            // 左右边k值与布林中轨关系
            relateDist = rightKData.RelateDistTo(rightBpMid);
            vertCountFromCurToBollMidLine = (int)(relateDist / missRelH);

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
            int[] uponAvgLineCounts;
            MACDLineWaveConfig[] lineCfgs;
            MACDBarConfig[] barCfgs;
            KGraphConfig[] kCfgs;
            int[] maxMissCounts;
            CalcPathValue(item, numIndex, ref isStrongUp, out uponAvgLineCounts, out pathValues, out lineCfgs, out barCfgs, out kCfgs, out maxMissCounts);

            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            StatisticUnit su0 = sum.statisticUnitMap[CollectDataType.ePath0];
            StatisticUnit su1 = sum.statisticUnitMap[CollectDataType.ePath1];
            StatisticUnit su2 = sum.statisticUnitMap[CollectDataType.ePath2];
            StatisticUnit[] sus = new StatisticUnit[] { su0, su1, su2, };

            for ( int i = 0; i < pathValues.Length; ++i )
            {
                res.Add(new PathCmpInfo(i, pathValues[i], uponAvgLineCounts[i], lineCfgs[i], barCfgs[i], kCfgs[i], sus[i], isStrongUp[i], maxMissCounts[i]));
            }
            res.Sort((x, y) =>
            {
                //if (x.pathValue > y.pathValue)
                //    return -1;
                //return 1;
                if (onlyTradeOnStrongUpPath)
                {
                    if (x.isStrongUp && y.isStrongUp == false)
                        return -1;
                    else if (x.isStrongUp == false && y.isStrongUp)
                        return 1;
                }
                if (x.pathValue > y.pathValue)
                    return -1;
                else if (x.pathValue < y.pathValue)
                    return 1;
                if (x.su.sample10Data.appearProbability > y.su.sample10Data.appearProbability)
                    return -1;
                else if (x.su.sample10Data.appearProbability < y.su.sample10Data.appearProbability)
                    return 1;
                else
                {
                    if (x.su.sample30Data.appearProbability > y.su.sample30Data.appearProbability)
                        return -1;
                    else if (x.su.sample30Data.appearProbability < y.su.sample30Data.appearProbability)
                        return 1;
                }
                return 0;
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
            GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataMap kdd = kddc.GetKDataDict(item);
            BollinPointMap bpm = kddc.GetBollinPointMap(kdd);

            BollinPoint bp;
            KData kdata;
            float rel_dist;
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            int loop = KGRAPH_LOOP_COUNT;
            int HALF_TRADE_LVS = TradeDataManager.Instance.tradeCountList.Count / 2;
            int[] maxMissCounts = new int[3] { 0, 0, 0, };
            int[] prevMaxMissCounts = new int[3] { 0, 0, 0, };
            int[] curMissCounts = new int[3] { 0, 0, 0, };
            int[] maxMissCountIDs = new int[3] { item.idGlobal, item.idGlobal, item.idGlobal, };
            int[] prevMaxMissCountIDs = new int[3] { -1, -1, -1, };
            float[] preMaxCountKValues = new float[3] { 0, 0, 0, };
            float[] curKValues = new float[3] { 0, 0, 0, };
            float[] pathValues = new float[3] { 0, 0, 0, };
            int[] stepCount = new int[3] { 0, 0, 0, };
            int[] uponBMCounts = new int[3] { 0, 0, 0, };
            int[] underBMCounts = new int[3] { 0, 0, 0, };
            bool[] isPreMissCountUponBM = new bool[3] { false, false, false, };
            float[] curBMDists = new float[3] { 0, 0, 0, };
            int[] preMaxMissUponBMCounts = new int[3] { 0, 0, 0, };
            KData[] curKDatas = new KData[3] { null, null, null, };
            float[] auxLineDist = new float[3] { 0, 0, 0, };

            CollectDataType[] cdts = new CollectDataType[3] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
            for( int i = 0; i < 3; ++i )
            {
                curMissCounts[i] = sum.statisticUnitMap[cdts[i]].missCount;
                maxMissCounts[i] = curMissCounts[i];
                stepCount[i] = curMissCounts[i];

                bp = bpm.GetData(cdts[i], false);
                kdata = kdd.GetData(cdts[i], false);
                rel_dist = kdata.RelateDistTo(bp.midValue);
                curBMDists[i] = rel_dist;
                curKValues[i] = kdata.KValue;
                if (rel_dist <= 0)
                    ++uponBMCounts[i];
                if (rel_dist >= 0)
                    ++underBMCounts[i];

                curKDatas[i] = kdata;
            }
            DataItem testItem = item.parent.GetPrevItem(item);
            while (testItem != null && loop > 0)
            {
                sum = testItem.statisticInfo.allStatisticInfo[numIndex];
                for (int i = 0; i < 3; ++i)
                {
                    kdd = kddc.GetKDataDict(testItem);
                    bpm = kddc.GetBollinPointMap(kdd);
                    bp = bpm.GetData(cdts[i], false);
                    kdata = kdd.GetData(cdts[i], false);
                    rel_dist = kdata.RelateDistTo(bp.midValue);
                    if (rel_dist <= 0)
                        ++uponBMCounts[i];
                    if (rel_dist >= 0)
                        ++underBMCounts[i];

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
                        preMaxCountKValues[i] = kdata.KValue;
                        prevMaxMissCounts[i] = misscount;
                        prevMaxMissCountIDs[i] = testItem.idGlobal;
                        isPreMissCountUponBM[i] = (rel_dist <= 0);
                        if (isPreMissCountUponBM[i])
                            preMaxMissUponBMCounts[i] = 1;
                        else
                            preMaxMissUponBMCounts[i] = 0;
                    }
                    if(prevMaxMissCountIDs[i] - testItem.idGlobal < prevMaxMissCounts[i])
                    {
                        if (rel_dist <= 0)
                            ++preMaxMissUponBMCounts[i];
                    }
                }
                testItem = testItem.parent.GetPrevItem(testItem);
                --loop;
            }

            for (int i = 0; i < 3; ++i)
            {
                float prevMaxMissCountLeftTopKV = 0;
                if(prevMaxMissCountIDs[i] != -1)
                {
                    int id = prevMaxMissCountIDs[i] - prevMaxMissCounts[i];
                    if (id < 0)
                        id = 0;
                    DataItem di = DataManager.GetInst().FindDataItem(id);
                    
                    kdd = kddc.GetKDataDict(di);
                    prevMaxMissCountLeftTopKV = kdd.GetData(cdts[i], false).KValue;
                }

                AutoAnalyzeTool.SingleAuxLineInfo lineInfo = autoAnalyzeTool.GetSingleAuxLineInfo(numIndex, cdts[i]);
                KData upL = null, upR = null, downL = null, downR = null;
                if (lineInfo.upLineData.valid)
                {
                    if(lineInfo.upLineData.dataNextSharp != null && lineInfo.upLineData.dataSharp != null)
                    {
                        upL = lineInfo.upLineData.dataSharp;
                        upR = lineInfo.upLineData.dataNextSharp;
                    }
                    else if(lineInfo.upLineData.dataSharp != null && lineInfo.upLineData.dataPrevSharp != null)
                    {
                        upL = lineInfo.upLineData.dataPrevSharp;
                        upR = lineInfo.upLineData.dataSharp;
                    }
                }
                if (lineInfo.downLineData.valid)
                {
                    if (lineInfo.downLineData.dataNextSharp != null && lineInfo.downLineData.dataSharp != null)
                    {
                        downL = lineInfo.downLineData.dataSharp;
                        downR = lineInfo.downLineData.dataNextSharp;
                    }
                    else if (lineInfo.downLineData.dataSharp != null && lineInfo.downLineData.dataPrevSharp != null)
                    {
                        downL = lineInfo.downLineData.dataPrevSharp;
                        downR = lineInfo.downLineData.dataSharp;
                    }
                }
                bool hasUpK = (upL != null && upR != null);
                bool hasDownK = (downL != null && downR != null);
                float distToUpLine = 0, distToDownLine = 0, upK = 0, downK = 0, upLV = 0, downLV = 0;
                if(hasUpK)
                {
                    upK = (upR.KValue - upL.KValue) / (upR.index - upL.index);
                    upLV = upK * (curKDatas[i].index - upR.index) + upR.KValue;
                    distToUpLine = curKDatas[i].KValue - upLV;
                }
                if (hasDownK)
                {
                    downK = (downR.KValue - downL.KValue) / (downR.index - downL.index);
                    downLV = downK * (curKDatas[i].index - downR.index) + downR.KValue;
                    distToDownLine = curKDatas[i].KValue - downLV;
                }

                float curCDTMiss = GraphDataManager.GetMissRelLength(cdts[i]);

                float main_rate = (float)maxMissCounts[i];// KGRAPH_LOOP_COUNT;
                if (prevMaxMissCountIDs[i] == -1)
                {
                    // 连错
                    if (curMissCounts[i] > 0)
                        pathValues[i] = 0xEFFFFFFF;
                    // 连对
                    else
                        pathValues[i] = 0;
                }

                // 前面的最大遗漏超过HALF_TRADE_LVS个，且当先的遗漏小于2
                else if (prevMaxMissCounts[i] >= HALF_TRADE_LVS && curMissCounts[i] < 2 && preMaxCountKValues[i] < curKValues[i])
                {
                    pathValues[i] = 0;
                }
                // 最大遗漏项之前的是否都在布林中轨之上，并且在布林中轨之上的个数达到KGRAPH_LOOP_COUNT个，当前项在布林中轨
                else if (isPreMissCountUponBM[i] && uponBMCounts[i] >= KGRAPH_LOOP_COUNT && curBMDists[i] >= -0.5f && curBMDists[i] <= 0.5f)
                {
                    pathValues[i] = 0;
                }
                // 最大遗漏项之前的是否都在布林中轨之上，并且在布林中轨之上的个数达到KGRAPH_LOOP_COUNT个，当前的遗漏在2以内
                else if (isPreMissCountUponBM[i] && uponBMCounts[i] >= KGRAPH_LOOP_COUNT && curMissCounts[i] < 2)
                {
                    pathValues[i] = 0;
                }
                // 当前K值超过前期的一个峰值，且当前的遗漏值小于2
                else if (prevMaxMissCountLeftTopKV < curKValues[i] && curMissCounts[i] < 2)
                {
                    pathValues[i] = 0;
                }

                //////////////////////////////////
                // 大热
                else if (hasUpK && distToUpLine > -1 && curMissCounts[i] < 2)
                {
                    pathValues[i] = 0;
                }
                // 大冷
                else if (hasDownK && distToDownLine < -curCDTMiss && curMissCounts[i] > 0)
                {
                    pathValues[i] = 0xEFFFFFFF;
                }
                // 在压力线和支撑线之间
                else if (hasUpK && hasDownK)
                {
                    float realDist = Math.Abs(Math.Abs(distToUpLine) > Math.Abs(distToDownLine) ? distToDownLine : distToUpLine);
                    int maxDist = Math.Abs((int)(upLV - downLV));
                    pathValues[i] = maxDist + realDist / maxDist;
                }
                ///////////////////////////////////

                else if (curMissCounts[i] > prevMaxMissCounts[i])
                {
                    if (prevMaxMissCounts[i] > 4)
                        pathValues[i] = 0xEFFFFFFF;
                    else
                        pathValues[i] = main_rate + (float)curMissCounts[i] / KGRAPH_LOOP_COUNT;
                }
                else
                    pathValues[i] = main_rate + (float)curMissCounts[i] / KGRAPH_LOOP_COUNT;// (float)prevMaxMissCounts[i];
            }

            trade.pathCmpInfos[numIndex].Clear();
            for (int i = 0; i < 3; ++i)
            {
                trade.pathCmpInfos[numIndex].Add(new PathCmpInfo(i, pathValues[i], uponBMCounts[i], MACDLineWaveConfig.eNone, MACDBarConfig.eNone, KGraphConfig.eNone, null, false, maxMissCounts[i]));
            }
            trade.pathCmpInfos[numIndex].Sort((x, y) =>
            {
                if (x.pathValue < y.pathValue)
                    return -1;
                else if(x.pathValue > y.pathValue)
                    return 1;
                else
                {
                    if (x.uponBMCount > y.uponBMCount)
                        return -1;
                    else
                        return 1;
                }
            });
        }


        //void CalcPathUpBolleanCount(DataItem item, TradeDataOneStar trade, int numIndex)
        //{
        //    List<PathCmpInfo> pcis = trade.pathCmpInfos[numIndex];
        //    CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
        //    GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
        //    KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
        //    KDataDict kdd = kddc.GetKDataDict(item);
        //    BollinPointMap bpm = kddc.GetBollinPointMap(kdd);
        //    for (int i = 0; i < cdts.Length; ++i)
        //    {
        //        PathCmpInfo pci = pcis[i];
        //        CollectDataType cdt = cdts[i];
        //        KData kd = kdd.GetData(cdt, false);
        //        BollinPoint bp = bpm.GetData(cdt, false);
        //        int upCount = 0;
        //        int totalCount = 0;
        //        float missheight = GraphDataManager.GetMissRelLength(cdt);
        //        int loopCount = GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT;
        //        DataItem pItem = item;
        //        while(pItem != null && loopCount > 0)
        //        {
        //            ++totalCount;
        //            KDataDict pKDD = kddc.GetKDataDict(pItem);
        //            KData pKD = pKDD.GetData(cdt, false);
        //            BollinPoint pBP = kddc.GetBollinPointMap(pKDD).GetData(cdt, false);                    
        //            float rdM = pKD.RelateDistTo(pBP.midValue);
        //            if(rdM < 1)
        //            {
        //                ++upCount;
        //            }
        //            pItem = pItem.parent.GetPrevItem(pItem);
        //            --loopCount;
        //        }
        //        pci.paramMap["UpBolleanCount"] = upCount * 100.0f / totalCount;
        //    }
        //}

        public enum MacdLineCfg
        {
            // 未定义
            eNone,
            // 当前快线小于等于慢线
            eFLES,
            // 金叉
            eGC,
            // 金叉后的快线大于等于慢线
            eGCFHES,
            // 当前快线大于等于慢线
            eFHES,
            // 死叉
            eDC,
        }

        // 布林中轨形态
        public enum BolleanCfg
        {
            // 未看出是啥形态
            eNone,
            // 先升后降
            eFirstUpThenDown,
            // 下降
            eDown,
            // 先降后升
            eFirstDownThenUp,
            // 上升
            eUp,
            // 水平震荡
            eHorz,
        }

        // 布林带开口类型
        public enum BolleanBandCfg
        {
            // 未定义
            eNone,
            // 开口变小（上升到末端或者下降到末端，预示着要开始做调整了）
            eBecomeSmall,
            // 开口增大（准备上升或者准备下降了）
            eBecomeLarge,
            // 保持不变（震荡形态）
            eKeepSize,
        }

        void CalcBolleanCommonData(DataItem item, TradeDataOneStar trade, int numIndex)
        {
            List<PathCmpInfo> pcis = trade.pathCmpInfos[numIndex];
            CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
            GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataMap kdd = kddc.GetKDataDict(item);
            TradeDataOneStar lastTrade = TradeDataManager.Instance.GetLatestTradeData() as TradeDataOneStar;

            for (int i = 0; i < cdts.Length; ++i)
            {
                CollectDataType cdt = cdts[i];
                BollinPoint bpRight = kddc.GetBollinPointMap(kdd).GetData(cdt, false);
                PathCmpInfo pci = pcis[i];

#if RECORD_BOLLEAN_MID_COUNTS
                // 统计落在布林中轨的K值的个数
                pci.paramMap["onMC"] = bpRight.onBolleanMidCount;
                // 统计落在布林中轨之上的K值个数
                pci.paramMap["upMC"] = bpRight.uponBolleanMidCount;
                // 统计落在布林中轨之下的K值个数
                pci.paramMap["dnMC"] = bpRight.underBolleanMidCount;
                // 统计连续落在布林中轨的K值的个数
                pci.paramMap["onMCC"] = bpRight.onBolleanMidCountContinue;
                // 统计连续落在布林中轨之下的K值的个数
                pci.paramMap["dnMCC"] = bpRight.underBolleanMidCountContinue;
                // 统计连续落在布林下轨的K值的个数
                pci.paramMap["onDownCC"] = bpRight.onBolleanDownCountContinue;

                // 计算中轨向上的次数
                pci.paramMap["midKUC"] = bpRight.bolleanMidKeepUpCount;
                // 计算中轨走平的次数
                pci.paramMap["midKHC"] = bpRight.bolleanMidKeepHorzCount;
                // 计算中轨向下的次数
                pci.paramMap["midKDC"] = bpRight.bolleanMidKeepDownCount;
                // 计算中轨连续向上的次数
                pci.paramMap["midKUCC"] = bpRight.bolleanMidKeepUpCountContinue;
                // 计算中轨连续走平的次数
                pci.paramMap["midKHCC"] = bpRight.bolleanMidKeepHorzCountContinue;
                // 计算中轨连续向下的次数
                pci.paramMap["midKDCC"] = bpRight.bolleanMidKeepDownCountContinue;
#endif

                // 计算连续开出的期数
                pci.paramMap["aprC"] = (int)item.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].appearCount;

                int index = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                float missHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[index];
                KData kd = kdd.GetData(cdt, false);
                pci.paramMap["dist2BU"] = kd.RelateDistTo(bpRight.upValue) / missHeight;
            }
        }

        int downFromTopCheckCount = 2;
        void CalcBolleanCfg(DataItem item, TradeDataOneStar trade, int numIndex)
        {
            const int LOOP_COUNT = 10;
            List<PathCmpInfo> pcis = trade.pathCmpInfos[numIndex];
            CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
            GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataMap kdd = kddc.GetKDataDict(item);
            TradeDataOneStar lastTrade = TradeDataManager.Instance.GetLatestTradeData() as TradeDataOneStar;

            for (int i = 0; i < cdts.Length; ++i)
            {
				CollectDataType cdt = cdts[i];
				BollinPoint bpRight = kddc.GetBollinPointMap(kdd).GetData(cdt, false);
                PathCmpInfo pci = pcis[i];
				// 布林中轨的形态
                pci.paramMap["BMCfg"] = BolleanCfg.eNone;
				// 布林中轨的斜率
                pci.paramMap["bpmDelta"] = (float)0.0f;
				// 是否在布林中轨之上连续超过2期没开出
                pci.paramMap["KDownFromTop"] = 0;
				// 布林通道在3期内的个数
                pci.paramMap["BollBandLE3Count"] = 0;

                //if (item.idGlobal < LOOP_COUNT)
                //{
                //    pci.paramMap["BBandCfg"] = BolleanBandCfg.eNone;
                //    continue;
                //}
                if (item.idGlobal < 1)
                {
                    continue;
                }
                
                KData kd = kdd.GetData(cdt, false);
                BollinPoint bpLeft = bpRight, bpMax = bpRight, bpMin = bpRight;
                float missHeight = GraphDataManager.GetMissRelLength(cdt);
                float horzTor = missHeight * 0.3f;
                BollinPoint bpP = kddc.GetBollinPointMap(kdd.index - 1).GetData(cdt, false);
                float deltaCur = bpRight.midValue - bpP.midValue;
                float deltaLast = 0;
                int pathIndex = GraphDataManager.S_CDT_LIST.IndexOf(cdt) - GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);

                while (lastTrade != null)
                {
                    PathCmpInfo lastPCI = lastTrade.FindInfoByPathIndex(numIndex, pathIndex);
                    deltaLast = (float)lastPCI.paramMap["bpmDelta"];
                    float tor = Math.Abs(deltaCur - deltaLast);
                    if (tor > 0.01f)
                        break;
                    lastTrade = TradeDataManager.Instance.GetTrade(lastTrade.INDEX - 1) as TradeDataOneStar;
                }
                // 计算布林中轨当前的斜率
                pci.paramMap["bpmDelta"] = (float)deltaCur;

                // 计算布林中轨曲线的形态
                if (deltaCur > deltaLast)
                    pci.paramMap["BMCfg"] = BolleanCfg.eUp;
                else if(deltaCur < deltaLast)
                    pci.paramMap["BMCfg"] = BolleanCfg.eDown;
                else if(deltaCur > 0)
                    pci.paramMap["BMCfg"] = BolleanCfg.eUp;
                else if(deltaCur < 0)
                    pci.paramMap["BMCfg"] = BolleanCfg.eDown;
                else
                    pci.paramMap["BMCfg"] = BolleanCfg.eHorz;


                // 计算当前k值是否出现在布林中轨之上且连续超过2期没出的情况
                StatisticUnit curSU = item.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt];
                int curMissCount = curSU.missCount;
                int curAppearCount = curSU.appearCount;
                bool isMaxMissCountLE1 = false;
                if (kd.index > 6 && curMissCount < 2)
                {
                    DataItem cit = item.parent.GetPrevItem(item);
                    int loop = 1;
                    while (cit != null && loop <= 4)
                    {
                        int cmc = cit.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                        if (cmc > 1)
                        {
                            break;
                        }
                        cit = cit.parent.GetPrevItem(cit);
                        ++loop;
                    }
                    if (loop == 5)
                        isMaxMissCountLE1 = true;
                }
                if (curMissCount > 0)
                {
                    int tid = kd.index - curMissCount + 1;
                    BollinPoint tbp = kddc.GetBollinPointMap(tid).GetData(cdt, false);
                    KData tkd = kddc.GetKDataDict(tid).GetData(cdt, false);
                    float distToUp = tkd.RelateDistTo(tbp.upValue) / missHeight;
                    // 如果最近开出的那一期是在布林上轨，那么认为这是要连续下降的趋势
                    if(distToUp <= 1)
                    {
                        if (curMissCount > downFromTopCheckCount)
                        {
                            pci.paramMap["KDownFromTop"] = 1;
                            ++downFromTopCheckCount;
                        }
                    }
#if RECORD_BOLLEAN_MID_COUNTS
                    // 如果超过2期没开出，且当前期是在布林中轨上方一个单位以外，我们也认为这是要下降的趋势
                    else if(curMissCount > downFromTopCheckCount && bpRight.distFromBolleanMidToKD < -1)
                    {
                        pci.paramMap["KDownFromTop"] = 1;
                        ++downFromTopCheckCount;
                    }
#endif

                    //// 如果最近开出的那一期是在布林中轨，且连续开出少于2期，并且k线是在布林中轨之下运行的，
                    //// 那么认为这也是下降趋势
                    ////else if(tbp.onMidCountContinue < 3 && tbp.underMidCount > 0)
                    ////else if(curMissCount > 1 && tbp.onMidCountContinue > 0 && tbp.underMidCount > 0)
                    //else if(!
                    //    (curAppearCount >= 3
                    //    || bpRight.onBolleanMidCountContinue >= 3
                    //    || isMaxMissCountLE1
                    //    ))
                    //{
                    //    if(bpRight.underBolleanMidCount > 0)
                    //        pci.paramMap["KDownFromTop"] = 1;
                    //}
                }
                //else if(!isMaxMissCountLE1 && bpRight.underBolleanMidCount > 0)
                //{
                //    pci.paramMap["KDownFromTop"] = 1;
                //}

                int t = kd.index - 2;
                if (t < 0) t = 0;
                int BollBandLE3Count = 0;
                for ( ; t <= kd.index; ++t )
                {
                    KData tkd = kddc.GetKDataDict(t).GetData(cdt, false);
                    BollinPoint tbp = kddc.GetBollinPointMap(t).GetData(cdt, false);
                    float band = (tbp.midValue - tbp.downValue) / missHeight;
                    if (band <= 3.1)
                        ++BollBandLE3Count;
                }
                // 计算布林带小于等于3的数量
                pci.paramMap["BollBandLE3Count"] = BollBandLE3Count;

                /*
                for (int j = 1; j <= LOOP_COUNT; ++j)
                {
                    BollinPoint pBP = kddc.GetBollinPointMap(kdd.index - j).GetData(cdt, false);
                    bpLeft = pBP;
                    if (bpMax.midValue <= pBP.midValue)
                        bpMax = pBP;
                    if (bpMin.midValue >= pBP.midValue)
                        bpMin = pBP;
                }

                float minMaxGap = bpMax.midValue - bpMin.midValue;
                if (bpLeft == bpMax && bpRight == bpMin)
                {
                    pci.paramMap["BMCfg"] = BolleanCfg.eDown;
                }
                else if (bpLeft == bpMin && bpRight == bpMax)
                {
                    pci.paramMap["BMCfg"] = BolleanCfg.eUp;
                }
                else if(minMaxGap <= horzTor)
                {
                    pci.paramMap["BMCfg"] = BolleanCfg.eHorz;
                }
                else if (bpMax.Index < bpMin.Index)
                {
                    if (bpMin.Index < bpRight.Index && bpMin.midValue < bpRight.midValue)
                        pci.paramMap["BMCfg"] = BolleanCfg.eFirstDownThenUp;
                    else
                        pci.paramMap["BMCfg"] = BolleanCfg.eFirstUpThenDown;
                }
                else if (bpMax.Index > bpMin.Index)
                {
                    if (bpMax.Index < bpRight.Index && bpMax.midValue > bpRight.midValue)
                        pci.paramMap["BMCfg"] = BolleanCfg.eFirstUpThenDown;
                    else
                        pci.paramMap["BMCfg"] = BolleanCfg.eFirstDownThenUp;
                }

                float leftBandSize = (bpLeft.upValue - bpLeft.downValue) / missHeight;
                float rightBandSize = (bpRight.upValue - bpRight.downValue) / missHeight;
                float checkSize = 1;
                float deltaSize = rightBandSize - leftBandSize;
                if (deltaSize > checkSize)
                    pci.paramMap["BBandCfg"] = BolleanBandCfg.eBecomeLarge;
                else if (deltaSize < -checkSize)
                    pci.paramMap["BBandCfg"] = BolleanBandCfg.eBecomeSmall;
                else
                    pci.paramMap["BBandCfg"] = BolleanBandCfg.eKeepSize;
                */
            }
        }

        // MACD 信号
        public enum MacdSignal
        {
            // 未定义
            eUnknown = 0,
            // 0 > 慢线 > 快线, 非常确认的下降信号
            eFullDown,
            // 慢线 > 0 && 快线 > 0, 预示回落的信号
            eHalfDown,
            // 柱体 > 快线 > 慢线,且3者呈抬升形态, 这是回升的回调信号
            eHalfUp,
            // 快线 > 慢线 > 柱体 > 0, 非常确认的抬升信号
            eFullUp,
        }

        void CalcPathMacdUp(DataItem item, TradeDataOneStar trade, int numIndex)
        {
            const int TOTAL_LIMIT_CHECK_COUNT = 3;

            List<PathCmpInfo> pcis = trade.pathCmpInfos[numIndex];
            CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
            GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataMap kdd = kddc.GetKDataDict(item);
            MACDPointMap bpm = kddc.GetMacdPointMap(kdd);
            KData minKD = null, maxKD = null;
            DataItem minItem = item, maxItem = item;
            List<KData> minPts = new List<KData>(), maxPts = new List<KData>();
            TradeDataOneStar lastTrade = TradeDataManager.Instance.GetLatestTradeData() as TradeDataOneStar;

            for (int i = 0; i < cdts.Length; ++i)
            {
                minPts.Clear();
                maxPts.Clear();

                PathCmpInfo pci = pcis[i];
                CollectDataType cdt = cdts[i];
                KData kd = kdd.GetData(cdt, false);
                MACDPoint mp = bpm.GetData(cdt, false);
                int curMissCount = item.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;

                /*
                pci.paramMap["MBAR"] = 0;
                MACDPointMap pMPM = kddc.GetMacdPointMap(mp.parent.index - 1);
                MACDPointMap ppMPM = kddc.GetMacdPointMap(mp.parent.index - 2);
                if(pMPM != null && ppMPM != null)
                {
                    MACDPoint pMP = pMPM.GetData(cdt, false);
                    MACDPoint ppMP = ppMPM.GetData(cdt, false);
                    if(mp.BAR > pMP.BAR)
                    {
                        if (pMP.BAR >= ppMP.BAR)
                            pci.paramMap["MBAR"] = 2;
                        else
                            pci.paramMap["MBAR"] = 1;
                    }
                    else if(mp.BAR < pMP.BAR && pMP.BAR < ppMP.BAR)
                    {
                        float rate = (mp.BAR - pMP.BAR) / (pMP.BAR - ppMP.BAR);
                        if(rate < 0.5f)
                            pci.paramMap["MBAR"] = 1;
                        else
                            pci.paramMap["MBAR"] = -1;
                    }
                    else if(mp.BAR < pMP.BAR && mp.BAR >= ppMP.BAR)
                    {
                        pci.paramMap["MBAR"] = 1;
                    }
                }
                */

                minKD = maxKD = kd;
                int totalCount = 0;
                int loopCount = GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT;
                DataItem pItem = item;
                MACDPoint maxMP = mp, minMP = mp, lastMP = mp, firstU = null, firstD = null;
                int dir = 0;

                int dirCalc = 0;
                int dirUpCount = 0;
                int dirDownCount = 0;
                int limitCheckCount = TOTAL_LIMIT_CHECK_COUNT;

                PathCmpInfo lastPCI = null;
                if (lastTrade != null)
                {
                    int pathIndex = GraphDataManager.S_CDT_LIST.IndexOf(cdt) - GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
                    lastPCI = lastTrade.FindInfoByPathIndex(numIndex, pathIndex);
                }

                pci.paramMap["DIF"] = mp.DIF;
                pci.paramMap["DEA"] = mp.DEA;
                pci.paramMap["BAR"] = mp.BAR;
                pci.paramMap["MacdUp"] = 0;
                MACDPointMap lastMPM = mp.parent.GetPrevMACDPM();
                if(lastMPM != null)
                {
                    lastMP = lastMPM.GetData(cdt, false);
                    bool isup = mp.DIF > lastMP.DIF && mp.DEA > lastMP.DEA;
                    if (isup)
                    {
                        pci.paramMap["MacdUp"] = mp.DIF > 0 ? 2 : 1;
                    }
                    else
                    {
                        if(mp.BAR > lastMP.BAR || mp.DIF > lastMP.DIF)
                            pci.paramMap["MacdUp"] = 1;
                    }
                }

                pci.paramMap["MacdSig"] = MacdSignal.eUnknown;
                if (mp.BAR > 0)
                {
                    if (mp.DIF > mp.DEA)
                    {
                        if (mp.DIF < mp.BAR)
                            pci.paramMap["MacdSig"] = MacdSignal.eHalfUp;
                        else
                            pci.paramMap["MacdSig"] = MacdSignal.eFullUp;
                    }
                }
                else
                {
                    if (mp.DIF < mp.DEA)
                    {
                        if (mp.DIF > 0)
                            pci.paramMap["MacdSig"] = MacdSignal.eHalfDown;
                        else
                            pci.paramMap["MacdSig"] = MacdSignal.eFullDown;
                    }
                }

                /*
                MacdLineCfg curCfg = MacdLineCfg.eNone;
                if (lastPCI == null)
                {
                    if(mp.DIF < mp.DEA)
                        curCfg = MacdLineCfg.eFLES;
                    else if(mp.DIF > mp.DEA)
                        curCfg = MacdLineCfg.eFHES;
                    else
                        curCfg = MacdLineCfg.eNone;
                }
                else
                {
                    MacdLineCfg lastCFG = (MacdLineCfg)lastPCI.paramMap["MacdCfg"];
                    switch(lastCFG)
                    {
                        case MacdLineCfg.eNone:
                            {
                                if (mp.DIF < mp.DEA)
                                    curCfg = MacdLineCfg.eFLES;
                                else if (mp.DIF > mp.DEA)
                                    curCfg = MacdLineCfg.eFHES;
                                else
                                    curCfg = MacdLineCfg.eNone;
                            }
                            break;
                        case MacdLineCfg.eFLES:
                            {
                                if (mp.DIF >= mp.DEA)
                                    curCfg = MacdLineCfg.eGC;
                                else
                                    curCfg = MacdLineCfg.eFLES;
                            }
                            break;
                        case MacdLineCfg.eFHES:
                            {
                                if (mp.DIF <= mp.DEA)
                                    curCfg = MacdLineCfg.eDC;
                                else
                                    curCfg = MacdLineCfg.eFHES;
                            }
                            break;
                        case MacdLineCfg.eDC:
                            {
                                if(mp.DIF < mp.DEA)
                                    curCfg = MacdLineCfg.eFLES;
                                else if(mp.DIF > mp.DEA)
                                    curCfg = MacdLineCfg.eGC;
                                else
                                    curCfg = MacdLineCfg.eDC;
                            }
                            break;
                        case MacdLineCfg.eGC:
                        case MacdLineCfg.eGCFHES:
                            {
                                if (mp.DIF > mp.DEA)
                                    curCfg = MacdLineCfg.eGCFHES;
                                else if (mp.DIF < mp.DEA)
                                    curCfg = MacdLineCfg.eDC;
                                else
                                    curCfg = MacdLineCfg.eGC;
                            }
                            break;
                    }
                }
                pci.paramMap["MacdCfg"] = curCfg;
                */

                while (pItem != null && loopCount > 0)
                {
                    ++totalCount;

                    if (pItem != item)
                    {
                        KDataMap pKDD = kddc.GetKDataDict(pItem);
                        KData pKD = pKDD.GetData(cdt, false);
                        MACDPoint pBP = kddc.GetMacdPointMap(pKDD).GetData(cdt, false);

                        MACDPointMap ppm = kddc.GetMacdPointMap(pBP.parent.index - 1);
                        MACDPointMap npm = kddc.GetMacdPointMap(pBP.parent.index + 1);
                        if (ppm != null && npm != null)
                        {
                            MACDPoint prevP = ppm.GetData(cdt, false);
                            MACDPoint nextP = npm.GetData(cdt, false);

                            if (firstU == null && prevP.BAR < pBP.BAR && nextP.BAR < pBP.BAR)
                                firstU = pBP;
                            if (firstD == null && prevP.BAR > pBP.BAR && nextP.BAR > pBP.BAR)
                                firstD = pBP;
                        }

                        // 获取MACD曲线的最高点
                        if (maxMP.BAR < pBP.BAR)
                            maxMP = pBP;

                        // 获取MACD曲线的最低点
                        if (minMP.BAR > pBP.BAR)
                            minMP = pBP;

                        // 获取K线最低点
                        if (minKD.KValue > pKD.KValue)
                        {
                            minKD = pKD;
                            minItem = pItem;
                        }
                        // 获取K线最高点
                        if (maxKD.KValue < pKD.KValue)
                        {
                            maxKD = pKD;
                            maxItem = pItem;
                        }
                        
                        KData prevKD = pKD.GetPrevKData();
                        KData nextKD = pKD.GetNextKData();
                        if(prevKD != null && nextKD != null)
                        {
                            // 获取K线的相对高点并存储起来
                            if (prevKD.KValue < pKD.KValue && nextKD.KValue < pKD.KValue)
                                maxPts.Add(pKD);
                            // 获取K线的相对低点并存储起来
                            if (prevKD.KValue > pKD.KValue && nextKD.KValue > pKD.KValue)
                                minPts.Add(pKD);
                        }
                    }

                    pItem = pItem.parent.GetPrevItem(pItem);
                    --loopCount;
                }

                /*
                if(firstU != null && firstD == null)
                {
                    dir = -1;
                    maxMP = firstU;
                }
                else if(firstU == null && firstD != null)
                {
                    dir = 1;
                    minMP = firstD;
                }
                else if(firstD != null && firstU != null)
                {
                    if(firstD.parent.index < firstU.parent.index)
                    {
                        dir = -1;
                        maxMP = firstU;
                    }
                    else
                    {
                        dir = 1;
                        minMP = firstD;
                    }
                }
                else if(maxMP.parent.index < minMP.parent.index)
                {
                    if (minMP.parent.index < mp.parent.index)
                        dir = 1;
                }
                else if(maxMP.parent.index > minMP.parent.index)
                {
                    if (maxMP.parent.index < mp.parent.index)
                        dir = -1;
                }

                pci.paramMap["MacdUp"] = 0f;
                if(dir == 1)
                {
                    pci.paramMap["MacdUp"] = (mp.BAR - minMP.BAR);
                }
                else if(dir == -1)
                {
                    pci.paramMap["MacdUp"] = (mp.BAR - maxMP.BAR);
                }

                pci.paramMap["IsMacdPUP"] = -1;
                if (curCfg == MacdLineCfg.eGC || curCfg == MacdLineCfg.eGCFHES)
                {
                    if(maxMP.parent.index < mp.parent.index)
                    {
                        if (maxMP.parent.index > minMP.parent.index)
                        {
                            if (mp.DEA >= 0)
                            {
                                pci.paramMap["IsMacdPUP"] = 1;
                            }
                        }
                        else if(minMP.parent.index < mp.parent.index)
                        {
                            pci.paramMap["IsMacdPUP"] = 1;
                        }
                    }
                    else
                    {
                        pci.paramMap["IsMacdPUP"] = 1;
                    }
                }
                */

                /*
                // k线的最小值在左边，最大值在右边，显示上升的形态
                if (maxKD.index > minKD.index)
                {
                    pci.paramMap["KUP"] = 1;

                    // 如果当前就是最大值，或者当前和最大值在3期范围之内，就认为强上升状态
                    if (maxKD == kd || kd.index - maxKD.index < 4)
                        pci.paramMap["KGraph"] = 2.0f;
                    else
                    {
                        // 检测最高点后面的低点，是不是逐步提升的
                        bool isBottomUp = true;
                        float kv = kd.KValue;
                        KData firstMinBeforeMaxPt = null;
                        // 遍历所有的低点
                        for (int p = 0; p < minPts.Count; ++p)
                        {
                            // 如果在最高点的左边，就不用检测了
                            if (minPts[p].index < maxKD.index)
                            {
                                if (firstMinBeforeMaxPt == null)
                                    firstMinBeforeMaxPt = minPts[p];
                                break;
                            }
                            // 如果后面一个低点的k值低于当前这个低点的k值，说明不是低点逐步提升，就不用再检测了
                            if (kv < minPts[p].KValue)
                            {
                                isBottomUp = false;
                                break;
                            }
                            // 把当前低点的k值赋值一下，以便于和左边的低点做比较
                            else
                            {
                                kv = minPts[p].KValue;
                            }
                        }

                        bool hasSet = false;
                        // 最高点右边的低点是逐步抬升的，认为这还是一个可靠的上升形态
                        if (isBottomUp)
                        {
                            if (firstMinBeforeMaxPt != null &&
                                firstMinBeforeMaxPt.KValue <= kd.KValue &&
                                (float)pci.paramMap["count2BMs"] <= 0)
                            {
                                pci.paramMap["KGraph"] = 2.0f;
                                hasSet = true;
                            }
                        }
                        // 如果还没检测出来
                        if(!hasSet)
                        {
                            // 取最大最小值的距离
                            float maxDist = maxKD.KValue - minKD.KValue;
                            // 如果是在4格范围之内，我们认为这还是一个强上升的形态
                            if (maxDist < 4)
                                pci.paramMap["KGraph"] = 1.0f;
                            else
                            {
                                // 如果当前是连续没出，且最近一次出的k值是最大值
                                if (curMissCount == kd.index - maxKD.index)
                                {
                                    // 判断当前k值是否超过上次的最低点，如果是，则表明这个K线是要下行了
                                    if (minPts.Count > 0 && kd.KValue < minPts[0].KValue)
                                    {
                                        pci.paramMap["KGraph"] = -1.0f;
                                        hasSet = true;
                                    }
                                }
                                if (!hasSet)
                                {
                                    pci.paramMap["KGraph"] = (float)Math.Abs(kd.KValue - maxKD.KValue) / maxDist;
                                }
                            }
                        }
                    }
                }
                // K线的最大值在左边，最小值在右边，显示的是下降的形态
                else if(maxKD.index < minKD.index)
                {
                    pci.paramMap["KUP"] = -1;

                    // 如果当前就是最小值，那么就认为是强下降形态
                    if (minKD == kd)
                    {
                        pci.paramMap["KGraph"] = -1.0f;
                    }
                    else
                    {
                        // 计算当前到最低点的最大遗漏值
                        DataItem cItem = item;
                        int maxMissCount = 0;
                        while (cItem != null && cItem.idGlobal > minItem.idGlobal)
                        {
                            int cmc = cItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                            if (cmc > maxMissCount)
                                maxMissCount = cmc;
                            cItem = cItem.parent.GetPrevItem(cItem);
                        }
                        // 如果最大遗漏值在3期范围之内，我们认为这是一个从下降转为上升的一个形态
                        if (maxMissCount < 4)
                        {
                            pci.paramMap["KGraph"] = 2.0f;
                        }
                        else
                        {
                            float maxDist = maxKD.KValue - minKD.KValue;
                            if (maxDist < 4)
                                pci.paramMap["KGraph"] = 1.0f;
                            else
                            {
                                pci.paramMap["KGraph"] = (float)Math.Abs(kd.KValue - minKD.KValue) / maxDist;
                            }
                        }
                    }
                }
                else
                {
                    pci.paramMap["KUP"] = 0;
                    pci.paramMap["KGraph"] = -1.0f;
                }
                */

                //pci.paramMap["WaitUp"] = 0;
                pci.paramMap["KKeepDown"] = 0;
                pci.paramMap["KAtBMDown"] = 0;

                //// 重置上次交易对的WaitUp的值
                //int lastTradeNumID = -1, lastTradePathIndex = -1;
                //if (lastTrade != null)
                //{
                //    lastTrade.GetTradeNumIndexAndPathIndex(ref lastTradeNumID, ref lastTradePathIndex);
                //    // 上一次交易这一路对了，就把WaitUp标记为2
                //    if (lastTrade.reward > 0 && lastTradeNumID == numIndex && lastTradePathIndex == pci.pathIndex)
                //    {
                //        lastPCI.paramMap["WaitUp"] = 2;
                //    }
                //}

                float missheight = GraphDataManager.GetMissRelLength(cdt);
                BollinPoint bp = kddc.GetBollinPointMap(kdd).GetData(cdt,false);
                // 当前K线是否触及布林下轨了
                bool isCurHitBolleanDown = false;
                if(kd.index > 0)
                {
                    BollinPoint bpPrev = kddc.GetBollinPointMap(kd.index - 1).GetData(cdt, false);
                    KData kdPrev = kddc.GetKDataDict(kd.index - 1).GetData(cdt, false);
                    if (kdPrev.DownValue > bpPrev.downValue && kd.DownValue <= bp.downValue)
                        isCurHitBolleanDown = true;
                }

                float curDistToBM = kd.RelateDistTo(bp.midValue) / missheight;
                // 标记当前k线是否运行到布林中轨之下了
                if(curDistToBM > 1)
                {
                    pci.paramMap["KAtBMDown"] = 1;
                }

                bool hasCheckKeepDown = false;
                if(curMissCount >= 3)
                {
                    int idP = kdd.index - (curMissCount - 1);
                    KDataMap kddP = kddc.GetKDataDict(idP);
                    BollinPointMap bpmP = kddc.GetBollinPointMap(idP);
                    KData kdP = kddP.GetData(cdt, false);
                    BollinPoint bpP = bpmP.GetData(cdt, false);
                    float upDistP = kdP.RelateDistTo(bpP.upValue) / missheight;
                    float midDistP = kdP.RelateDistTo(bpP.midValue) / missheight;
                    if (upDistP <= 1 ||
                        (midDistP <= -1 && curDistToBM > 1))
                    {
                        pci.paramMap["KKeepDown"] = 1;
                        hasCheckKeepDown = true;
                    }
                }

                if (hasCheckKeepDown == false)
                {
                    // 当前K值在布林中轨下方
                    if (curDistToBM >= 0)
                    {
                        KData tmpMinKD = null, tmpMaxKD = maxKD;
                        int tmpMinKDID = -1, tmpMaxKDID = maxPts.IndexOf(maxKD);
                        // 遍历最大值右边的相对低点的最低值
                        for (int t = 0; t < minPts.Count; ++t)
                        {
                            KData tkd = minPts[t];
                            if (tkd.parent.index > maxKD.parent.index)
                            {
                                // 找到最低点
                                if (tmpMinKD == null || tmpMinKD.KValue >= tkd.KValue)
                                {
                                    tmpMinKD = tkd;
                                    tmpMinKDID = t;
                                }
                            }
                            else
                                break;
                        }
                        bool findTmpMax = false;
                        // 遍历最大值右边首个在布林中轨之上的相对高点
                        for (int t = 0; t < maxPts.Count; ++t)
                        {
                            KData tkd = maxPts[t];
                            if (tkd.index <= maxKD.index)
                                break;
                            bp = kddc.GetBollinPointMap(tkd.parent).GetData(cdt, false);
                            //float distToMid = tkd.RelateDistTo(bp.midValue) / missheight;
                            float distToMid = (tkd.DownValue - bp.midValue) / missheight;
                            if (distToMid >= 1)
                            {
                                tmpMaxKD = tkd;
                                tmpMaxKDID = t;
                                findTmpMax = true;
                                break;
                            }
                        }
                        bool isTmpMaxKDUpBolleanMidLine = findTmpMax ? true : false;
                        if (!isTmpMaxKDUpBolleanMidLine)
                        {
                            bp = kddc.GetBollinPointMap(tmpMaxKD.parent).GetData(cdt, false);
                            isTmpMaxKDUpBolleanMidLine = tmpMaxKD.RelateDistTo(bp.midValue) / missheight < 0;
                        }

                        if (tmpMinKD != null)
                        {
                            bp = kddc.GetBollinPointMap(tmpMinKD.parent).GetData(cdt, false);
                            float tmpMinDistToBM = tmpMinKD.RelateDistTo(bp.midValue) / missheight;

                            // 如果最低点也在布林中轨下方
                            if (tmpMinDistToBM > 0)
                            {
                                //// 标记是否连续开出2轮以上的反转信号
                                //bool hasExistBecomeUpSignalMoreThanTwice = false;
                                //// 追溯当前K值之前的3期，获取开出的次数
                                //int appcount = curMissCount == 0 ? 1 : 0;
                                //if (kd.index > 4)
                                //{
                                //    for (int t = 1; t <= 3; ++t)
                                //    {
                                //        int nmc = kddc.GetKDataDict(kd.index - t).startItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                                //        if (nmc == 0)
                                //            ++appcount;
                                //    }
                                //}
                                //// 如果最近4期开出大于等于2次
                                //if (appcount >= 2)
                                //{
                                //    int endID = tmpMaxKD.index + 3;
                                //    int firstAppcount = 0;
                                //    for (int t = kd.index - 4; t >= endID; --t)
                                //    {
                                //        firstAppcount = 0;
                                //        for (int k = 0; k <= 3; ++k)
                                //        {
                                //            int nmc = kddc.GetKDataDict(t - k).startItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                                //            if (nmc == 0)
                                //                ++firstAppcount;
                                //        }
                                //        if (firstAppcount >= 2)
                                //            break;
                                //    }
                                //    if (firstAppcount >= 2)
                                //    {
                                //        pci.paramMap["WaitUp"] = 1;
                                //        hasExistBecomeUpSignalMoreThanTwice = true;
                                //    }
                                //}
                                //else if (lastPCI != null && (int)lastPCI.paramMap["WaitUp"] == 1)
                                //{
                                //    pci.paramMap["WaitUp"] = 1;
                                //}

                                // 如果相对高点在布林中轨之上，且在相对低点的右边
                                if (isTmpMaxKDUpBolleanMidLine &&
                                    tmpMaxKD.index > tmpMinKD.index &&
                                    tmpMaxKD.index < kd.index)
                                {
                                    if(isCurHitBolleanDown)
                                        pci.paramMap["KKeepDown"] = 2;
                                    else
                                        pci.paramMap["KKeepDown"] = 1;
                                }
                                else
                                {
                                    float distToMin = (tmpMinKD.KValue - kd.KValue) / missheight;

                                    // 如果当前K值是最低点，或者当前k值低于相对低点，我们认为这段K线是纯下降的
                                    if (tmpMinKD == kd || distToMin > 1.4f)
                                    {
                                        if (isCurHitBolleanDown)
                                            pci.paramMap["KKeepDown"] = 2;
                                        else
                                            pci.paramMap["KKeepDown"] = 1;
                                    }
                                    // 第一个相对低点是最低点
                                    else if (tmpMinKDID == 0)
                                    {
                                        //int prevHitCount = 0, nextHitCount = 0, tid = 0;
                                        //int tmpMinKDMissCount = kddc.GetKDataDict(tmpMinKD.index).startItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                                        //for (int t = 1; t <= 3; ++t)
                                        //{
                                        //    tid = tmpMinKD.index - t;
                                        //    if(tid >= 0 && tmpMinKDMissCount < 3)
                                        //    {
                                        //        int nmc = kddc.GetKDataDict(tid).startItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                                        //        if (nmc == 0)
                                        //            ++prevHitCount;
                                        //    }
                                        //    tid = tmpMinKD.index + t;
                                        //    if(tid <= kd.index)
                                        //    {
                                        //        int nmc = kddc.GetKDataDict(tid).startItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                                        //        if (nmc == 0)
                                        //            ++nextHitCount;
                                        //    }
                                        //}
                                        int hitCount = 0, startID = tmpMinKD.index - 3, endID = tmpMinKD.index + 3;
                                        if(endID > kd.index)
                                        {
                                            endID = kd.index;
                                            if (endID - kd.index == 1)
                                            {
                                                startID -= 1;
                                            }
                                        }
                                        if (startID < 0)
                                            startID = 0;
                                        for (int t = startID; t <= endID; ++t)
                                        {
                                            int nmc = kddc.GetKDataDict(t).startItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                                            if (nmc == 0)
                                                ++hitCount;
                                        }
                                        if (hitCount >= 2)
                                        {
                                            pci.paramMap["KKeepDown"] = 2;
                                        }
                                        //int gap = kd.index - tmpMinKD.index;
                                        //// 如果最低点到当前超过2期
                                        //if (gap >= 2)
                                        //{
                                        //    int contHit = 0;
                                        //    for (int t = 1; t <= gap; ++t)
                                        //    {
                                        //        int nmc = kddc.GetKDataDict(tmpMinKD.index + t).startItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                                        //        if (nmc == 0)
                                        //            ++contHit;
                                        //        else
                                        //            break;
                                        //    }
                                        //    // 如果超过2期且最低点到当前都是连续开出，那么就设置这是强烈上升的
                                        //    if ( contHit >= 2 )
                                        //        pci.paramMap["KKeepDown"] = 2;
                                        //    else
                                        //        pci.paramMap["KKeepDown"] = 1;
                                        //}
                                        //// 从当前项往前遍历4期
                                        //else if(kd.index > 4)
                                        //{
                                        //    int contHit = curMissCount == 0 ? 1 : 0;
                                        //    for(int t = 1; t <= 3; ++t)
                                        //    {
                                        //        int nmc = kddc.GetKDataDict(kd.index - t).startItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;
                                        //        if (nmc == 0)
                                        //            ++contHit;
                                        //    }
                                        //    if (contHit >= 2)
                                        //        pci.paramMap["KKeepDown"] = 2;
                                        //    else
                                        //        pci.paramMap["KKeepDown"] = 1;
                                        //}
                                        // 还没出现转折信号，认为还是下降
                                        else
                                        {
                                            pci.paramMap["KKeepDown"] = 1;
                                        }
                                    }
                                    // 右边还有相对低点，说明已经出现反转信号了，很可能要回升了
                                    else if (tmpMinKDID > 0)
                                    {
                                        // 如果触及布林下轨或者离相对低点的高度没有低于超过1.5个单位，可以认为还有有上升可能
                                        if(isCurHitBolleanDown || distToMin <= 1.4f)
                                            pci.paramMap["KKeepDown"] = 2;
                                        // 否则就认为这是下降趋势
                                        else
                                            pci.paramMap["KKeepDown"] = 1;
                                    }
                                    
                                    else
                                    {
                                        //if(hasExistBecomeUpSignalMoreThanTwice)
                                        //    pci.paramMap["KKeepDown"] = 2;
                                        //else
                                        pci.paramMap["KKeepDown"] = 1;
                                    }
                                }
                            }
                            // 低点在布林中轨之上
                            else
                            {
                                pci.paramMap["KKeepDown"] = 1;
                            }
                        }
                    }
                    // 当前k值在布林中轨之上
                    else if (minKD.index < maxKD.index)
                    {
                        // 如果最高值接触到布林上轨，且当前遗漏超过2期，那么可以认为k线要下行了
                        bp = kddc.GetBollinPointMap(maxKD.parent).GetData(cdt, false);
                        float maxKDistToBU = maxKD.RelateDistTo(bp.upValue) / missheight;
                        if (maxKDistToBU <= 0 && curMissCount > 2)
                        {
                            // 在布林中轨附近的时候，认为有反弹的可能性
                            if (Math.Abs(curDistToBM) <= 1)
                                pci.paramMap["KKeepDown"] = 0;
                            else
                                pci.paramMap["KKeepDown"] = 1;
                        }
                    }
                }
            }
        }

        void CalcPathIfBecomeUp(DataItem item, TradeDataOneStar trade, int numIndex, ref int mayUpPathsCount)
        {
            mayUpPathsCount = 0;
            List<PathCmpInfo> pcis = trade.pathCmpInfos[numIndex];
            CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
            GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataMap kdd = kddc.GetKDataDict(item);
            BollinPointMap bpm = kddc.GetBollinPointMap(kdd);
            List<int> uponBMIndexs = new List<int>();

            for( int i = 0; i < cdts.Length; ++i )
            {
                uponBMIndexs.Clear();
                PathCmpInfo pci = pcis[i];
                CollectDataType cdt = cdts[i];
                KData kd = kdd.GetData(cdt, false);
                BollinPoint bp = bpm.GetData(cdt, false);
                pci.paramMap["MayUpCount"] = (float)0;

                int minKVIndex = kdd.index;
                float minKV = kd.KValue;
                int maxMissCount = 0;
                int maxMissCountIndex = item.idGlobal;
                float maxMissCountKV = 0;

                float missheight = GraphDataManager.GetMissRelLength(cdt);
                float rdM = kd.RelateDistTo(bp.midValue);
                float rdD = kd.RelateDistTo(bp.downValue);
                float cM = rdM / missheight;
                float cD = rdD / missheight;
                if(cM >= -1 && cD <= 0)
                {
                    bool findBottomPt = false;
                    bool findLeftNearBolleanMidPt = false;
                    bool findValidPt = false;
                    int uponBooleanMidIndex = -1;

                    DataItem pItem = item.parent.GetPrevItem(item);
                    if (pItem == null)
                    {
                        continue;
                    }
                    StatisticUnitMap pSum = pItem.statisticInfo.allStatisticInfo[numIndex];
                    StatisticUnit pSu = pSum.statisticUnitMap[cdt];

                    int loopCount = tradeCountList.Count;
                    if (loopCount < pSu.missCount)
                        loopCount = pSu.missCount;

                    if (pSu.missCount == 0)
                        findValidPt = true;
                    bool hasResetLoop = false;
                    while(pItem != null && loopCount >= 0)
                    {
                        pSum = pItem.statisticInfo.allStatisticInfo[numIndex];
                        pSu = pSum.statisticUnitMap[cdt];
                        KDataMap pKDD = kddc.GetKDataDict(pItem);
                        KData pKD = pKDD.GetData(cdt, false);
                        BollinPoint pBP = kddc.GetBollinPointMap(pKDD).GetData(cdt, false);

                        // 统计最小K值的那一项
                        if(pKD.KValue < minKV)
                        {
                            minKV = pKD.KValue;
                            minKVIndex = pItem.idGlobal;
                        }
                        // 统计最大遗漏的那一项
                        if(pSu.missCount > maxMissCount)
                        {
                            maxMissCount = pSu.missCount;
                            maxMissCountIndex = pItem.idGlobal;
                            maxMissCountKV = pKD.KValue;
                        }

                        if (pSu.missCount > 0 && pKD.KValue < kd.KValue)
                            findValidPt = true;

                        float prdD = pKD.RelateDistTo(pBP.downValue) / missheight;
                        float prdB = pKD.RelateDistTo(pBP.midValue) / missheight;
                        if (Math.Abs(prdD) <= 1)
                            findBottomPt = true;
                        if (prdB <= 1)
                        {
                            findLeftNearBolleanMidPt = true;
                            if(prdB < 0)
                            {
                                uponBMIndexs.Add(pItem.idGlobal);
                            }
                        }
                        if (prdB < 0 && uponBooleanMidIndex == -1)
                        {
                            uponBooleanMidIndex = pItem.idGlobal;
                        }

                        pItem = pItem.parent.GetPrevItem(pItem);
                        --loopCount;

                        if (hasResetLoop == false && loopCount < 0 && pSu.missCount > 0)
                        {
                            loopCount = pSu.missCount;
                            hasResetLoop = true;
                        }

                        if (findBottomPt && findLeftNearBolleanMidPt && findValidPt)
                        {
                            break;
                        }
                    }

                    bool findMidEnd = false;
                    for (int k = 0; k < uponBMIndexs.Count; ++k)
                    {
                        if (uponBMIndexs[k] > maxMissCountIndex && uponBMIndexs[k] < item.idGlobal)
                        {
                            findMidEnd = true;
                            break;
                        }
                    }

                    //if ((minKVIndex == kdd.index && maxMissCount < 10) 
                    //    ||(maxMissCountIndex < item.idGlobal && kd.KValue < maxMissCountKV))
                    float distCount = (minKV - kd.KValue) / missheight;
                    if (/*findMidEnd || */(minKVIndex < item.idGlobal && distCount > 2)
                        || (minKVIndex == item.idGlobal))
                    {
                        pci.paramMap["MayUpCount"] = -(float)Math.Abs(cM);
                        continue;
                    }

                    if (findBottomPt && findLeftNearBolleanMidPt && findValidPt && findMidEnd == false)
                    {
                        //if(uponBooleanMidIndex == -1 || uponBooleanMidIndex >= tradeCountList.Count / 2)
                            pci.paramMap["MayUpCount"] = (float)Math.Abs(cM);
                    }

                    if((float)pci.paramMap["MayUpCount"] > 0)
                    {
                        mayUpPathsCount++;
                    }
                }
            }
        }

        void CalcPathAppearence(DataItem item, TradeDataOneStar trade, int numIndex)
        {
            DataItem prvItem = item.parent.GetPrevItem(item);
            StatisticUnitMap sumCUR = item.statisticInfo.allStatisticInfo[numIndex];
            StatisticUnitMap sumPRV = null;
            if (prvItem != null)
                sumPRV = prvItem.statisticInfo.allStatisticInfo[numIndex];
            CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
            float[] theoryAppRate = new float[] { 0, 0, 0 };
            float[] appearenceRateFastCUR = new float[] { 0, 0, 0, };
            float[] appearenceRateFastPRV = new float[] { 0, 0, 0, };
            float[] appearenceRateShortCUR = new float[] { 0, 0, 0, };
            float[] appearenceRateShortPRV = new float[] { 0, 0, 0, };
            int[] curMissCount = new int[] { 0, 0, 0, };
            int[] preMaxMissCount = new int[] { 0, 0, 0, };
            int[] preMaxMissCountID = new int[] { 0, 0, 0, };
            int[] underTheoryRateCount = new int[] { 0, 0, 0, };
            for (int i = 0; i < cdts.Length; ++i)
            {
                CollectDataType cdt = cdts[i];
                theoryAppRate[i] = GraphDataManager.GetTheoryProbability(cdt);
                curMissCount[i] = sumCUR.statisticUnitMap[cdt].missCount;
                preMaxMissCount[i] = sumCUR.statisticUnitMap[cdt].sample10Data.prevMaxMissCount;
                preMaxMissCountID[i] = sumCUR.statisticUnitMap[cdt].sample10Data.prevMaxMissCountIndex;
                appearenceRateFastCUR[i] = sumCUR.statisticUnitMap[cdt].sample5Data.appearProbability;
                underTheoryRateCount[i] = sumCUR.statisticUnitMap[cdt].sample5Data.underTheoryCount;
                appearenceRateShortCUR[i] = sumCUR.statisticUnitMap[cdt].sample10Data.appearProbability;
                if (sumPRV != null)
                {
                    appearenceRateFastPRV[i] = sumPRV.statisticUnitMap[cdt].sample5Data.appearProbability;
                    appearenceRateShortPRV[i] = sumPRV.statisticUnitMap[cdt].sample10Data.appearProbability;
                }
            }

            float[] count2BUs = new float[] { 0, 0, 0, };
            float[] count2BMs = new float[] { 0, 0, 0, };
            float[] count2BDs = new float[] { 0, 0, 0, };
            int[] count2LIM = new int[] { 0, 0, 0, };
            {
                CalcKValueDistToBolleanLine(item, numIndex, cdts, ref count2BUs, ref count2BMs, ref count2BDs, ref count2LIM);
            }

            trade.pathCmpInfos[numIndex].Clear();
            for (int i = 0; i < cdts.Length; ++i)
            {
                PathCmpInfo pci = new PathCmpInfo(i, sumCUR.statisticUnitMap[cdts[i]]);
                float diffPrevF = appearenceRateFastCUR[i] - appearenceRateFastPRV[i];
                float diffPrevS = appearenceRateShortCUR[i] - appearenceRateShortPRV[i];
                float diffTheoF = (appearenceRateFastCUR[i] - theoryAppRate[i]);
                float diffThroS = (appearenceRateShortCUR[i] - theoryAppRate[i]);
                pci.paramMap["curMissCount"] = curMissCount[i];
                pci.paramMap["maxMissCount"] = preMaxMissCount[i];
                pci.paramMap["maxMissCountID"] = preMaxMissCountID[i];
                //pci.paramMap["prvRateF"] = appearenceRateFastPRV[i];
                //pci.paramMap["theoryAppRate"] = theoryAppRate[i];
                pci.paramMap["underTheoRateCount"] = underTheoryRateCount[i];
                pci.paramMap["curRateF"] = appearenceRateFastCUR[i];
                pci.paramMap["detRateF"] = diffPrevF;
                pci.paramMap["isCurRateFBTheoRate"] = (diffTheoF >= 0);
                //pci.paramMap["prvRateS"] = appearenceRateShortPRV[i];
                pci.paramMap["curRateS"] = appearenceRateShortCUR[i];
                pci.paramMap["detRateS"] = diffPrevS;
                pci.paramMap["isCurRateSBTheoRate"] = (diffThroS >= 0);
                bool isAppRatePrefer = false;
                if (GlobalSetting.G_AppearenceCheckType == AppearenceCheckType.eUseFast)
                    isAppRatePrefer = diffPrevF > 0 || diffTheoF >= 0;
                else if (GlobalSetting.G_AppearenceCheckType == AppearenceCheckType.eUSeShort)
                    isAppRatePrefer = diffPrevS > 0 || diffThroS >= 0;
                else if(GlobalSetting.G_AppearenceCheckType == AppearenceCheckType.eUseFastAndShort)
                    isAppRatePrefer = (diffPrevF > 0 || diffTheoF >= 0) && (diffPrevS > 0 || diffThroS >= 0);
                pci.paramMap["isAppRatePrefer"] = isAppRatePrefer;

                //if (GlobalSetting.G_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE)
                //{
                //    pci.paramMap["count2LIM"] = count2LIM[i];
                //    pci.paramMap["count2BUs"] = count2BUs[i];
                //    pci.paramMap["count2BDs"] = count2BDs[i];
                //}
                pci.paramMap["count2BMs"] = count2BMs[i];
                pci.paramMap["count2BUs"] = count2BUs[i];

                trade.pathCmpInfos[numIndex].Add(pci);
            }
            CalcBolleanCommonData(item, trade, numIndex);

            int mayUpPathsCount = 0;
            //if (GlobalSetting.G_ENABLE_BOOLEAN_DOWN_UP_CHECK)
            //{
            //    CalcPathIfBecomeUp(item, trade, numIndex, ref mayUpPathsCount);
            //}

            //if (GlobalSetting.G_ENABLE_UPBOLLEAN_COUNT_STATISTIC)
            //{
            //    CalcPathUpBolleanCount(item, trade, numIndex);
            //}

            if (GlobalSetting.G_COLLECT_MACD_ANALYZE_DATA)
            {
                CalcPathMacdUp(item, trade, numIndex);
            }

            if(GlobalSetting.G_COLLECT_BOLLEAN_ANALYZE_DATA)
            {
                CalcBolleanCfg(item, trade, numIndex);
            }

            if(GlobalSetting.G_COLLECT_ANALYZE_TOOL_DATA)
            {
                AutoAnalyzeToolCheck(numIndex, trade);
            }

            int path0Index = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
            int selPathIndex = GraphDataManager.S_CDT_LIST.IndexOf(GlobalSetting.G_TRADE_SPEC_CDT) - path0Index;
            trade.pathCmpInfos[numIndex].Sort(
                (x, y) =>
                {
                    if(GlobalSetting.G_ONLY_TRADE_SPEC_CDT)
                    {
                        if (x.pathIndex == selPathIndex)
                            return -1;
                        return 1;
                    }

                    if(GlobalSetting.G_TRADE_IMMEDIATE_ON_MISS_COUNT0_PATH)
                    {
                        if ((int)x.paramMap["curMissCount"] < (int)y.paramMap["curMissCount"])
                            return -1;
                        if ((int)x.paramMap["curMissCount"] > (int)y.paramMap["curMissCount"])
                            return 1;
                        return 0;
                    }

                    //if (GlobalSetting.G_ENABLE_BOOLEAN_DOWN_UP_CHECK)
                    //{
                    //    if ((float)x.paramMap["MayUpCount"] > (float)y.paramMap["MayUpCount"])
                    //        return -1;
                    //    if ((float)x.paramMap["MayUpCount"] < (float)y.paramMap["MayUpCount"])
                    //        return 1;
                    //}

                    //if(GlobalSetting.G_ENABLE_UPBOLLEAN_COUNT_STATISTIC)
                    //{
                    //    if ((float)x.paramMap["UpBolleanCount"] > (float)y.paramMap["UpBolleanCount"])
                    //        return -1;
                    //    if ((float)x.paramMap["UpBolleanCount"] < (float)y.paramMap["UpBolleanCount"])
                    //        return 1;
                    //}

                    if(GlobalSetting.G_SEQ_PATH_BY_APPEARENCE_RATE)
                    {
                        bool isXPrefer = (bool)x.paramMap["isAppRatePrefer"];
                        bool isYPrefer = (bool)y.paramMap["isAppRatePrefer"];
                        if (isXPrefer && !isYPrefer)
                            return -1;
                        if (!isXPrefer && isYPrefer)
                            return 1;

                        if ((float)x.paramMap["detRateS"] >= 0 && (float)y.paramMap["detRateS"] < 0)
                            return -1;
                        if ((float)x.paramMap["detRateS"] < 0 && (float)y.paramMap["detRateS"] >= 0)
                            return 1;
                    }

                    if (GlobalSetting.G_ENABLE_BOLLEAN_CFG_CHECK &&
                        GlobalSetting.G_SEQ_PATH_BY_BOLLEAN_CFG)
                    {
                        bool isXBMUp = (int)x.paramMap["midKUCC"] > 0;
                        bool isYBMUp = (int)y.paramMap["midKUCC"] > 0;
                        bool isXNearBU = (float)x.paramMap["count2BUs"] <= 1;
                        bool isYNearBU = (float)y.paramMap["count2BUs"] <= 1;
                        //bool isXBMUp = ((int)x.paramMap["midKUC"] + (int)x.paramMap["midKHC"]) > 0;
                        //bool isYBMUp = ((int)y.paramMap["midKUC"] + (int)y.paramMap["midKHC"]) > 0;
                        bool isXPrefer = (bool)x.paramMap["isAppRatePrefer"];
                        bool isYPrefer = (bool)y.paramMap["isAppRatePrefer"];

                        if ((float)x.paramMap["count2BUs"] < (float)y.paramMap["count2BUs"])
                            return -1;
                        if ((float)x.paramMap["count2BUs"] > (float)y.paramMap["count2BUs"])
                            return 1;

                        if ((isXBMUp && isXPrefer && isXNearBU) && !(isYBMUp && isYPrefer && isYNearBU))
                            return -1;
                        if (!(isXBMUp && isXPrefer && isXNearBU) && (isYBMUp && isYPrefer && isYNearBU))
                            return 1;

                        if ((isXBMUp && isXPrefer) && !(isYBMUp && isYPrefer))
                            return -1;
                        if (!(isXBMUp && isXPrefer) && (isYBMUp && isYPrefer))
                            return 1;

                        if (isXPrefer && !isYPrefer)
                            return -1;
                        if (!isXPrefer && isYPrefer)
                            return 1;

                        if ((float)x.paramMap["detRateS"] >= 0 && (float)y.paramMap["detRateS"] < 0)
                            return -1;
                        if ((float)x.paramMap["detRateS"] < 0 && (float)y.paramMap["detRateS"] >= 0)
                            return 1;

                        // 计算x图里布林中轨持续向下的个数
                        int xMDC = (int)x.paramMap["midKDC"];
                        // 计算y图里布林中轨持续向下的个数
                        int yMDC = (int)y.paramMap["midKDC"];
                        // x没有向下，y向下了
                        if (xMDC == 0 && yMDC > 0)
                            return -1;
                        // x向下了，y没有向下
                        if (xMDC > 0 && yMDC == 0)
                            return 1;

                        if ((int)x.paramMap["aprC"] >= 2)
                            return -1;
                        if ((int)y.paramMap["aprC"] >= 2)
                            return 1;

                        if ((float)x.paramMap["curRateF"] > (float)y.paramMap["curRateF"])
                            return -1;
                        if ((float)x.paramMap["curRateF"] < (float)y.paramMap["curRateF"])
                            return 1;

                        /*
                        // 计算x图里布林中轨持续向下的个数
                        int xMDC = (int)x.paramMap["midKDC"];
                        // 计算y图里布林中轨持续向下的个数
                        int yMDC = (int)y.paramMap["midKDC"];
                        // x没有向下，y向下了
                        if (xMDC == 0 && yMDC > 0)
                        //if(xMDC <= 1 && yMDC > 1)
                            return -1;
                        // x向下了，y没有向下
                        if (xMDC > 0 && yMDC == 0)
                        //if(xMDC > 1 && yMDC <= 1)
                            return 1;
                        // 都没有向下
                        //if(xMDC == 0 && yMDC == 0)
                        if(xMDC <= 1 && yMDC <= 1)
                        {
                            bool isXAppRateHTheoryRate = (bool)x.paramMap["isCurRateFBTheoRate"];
                            bool isYAppRateHTheoryRate = (bool)y.paramMap["isCurRateFBTheoRate"];
                            // 如果x的出号率高于理论值，y的出号率没有高于理论值，那么就选x
                            if (isXAppRateHTheoryRate && !isYAppRateHTheoryRate)
                                return -1;
                            // 如果x的出号率没有高于理论值，y的出号率高于理论值，那么就选y
                            if (!isXAppRateHTheoryRate && isYAppRateHTheoryRate)
                                return 1;

                            // 如果x的k线在布林中轨之下的个数小于y的k线在布林中轨之下的个数，优先选x
                            if ((int)x.paramMap["dnMCC"] < (int)y.paramMap["dnMCC"])
                                return -1;
                            // 如果x的k线在布林中轨之下的个数大于y的k线在布林中轨之下的个数，优先选y
                            if ((int)x.paramMap["dnMCC"] > (int)y.paramMap["dnMCC"])
                                return 1;

                            int xUC = (int)x.paramMap["midKHC"] + (int)x.paramMap["midKUC"];
                            int yUC = (int)y.paramMap["midKHC"] + (int)y.paramMap["midKUC"];
                            if (xUC > yUC)
                                return -1;
                            if (xUC < yUC)
                                return 1;
                        }
                        
                        if ((int)x.paramMap["aprC"] >= 2)
                            return -1;
                        if ((int)y.paramMap["aprC"] >= 2)
                            return 1;

                        if ((float)x.paramMap["curRateF"] > (float)y.paramMap["curRateF"])
                            return -1;
                        if ((float)x.paramMap["curRateF"] < (float)y.paramMap["curRateF"])
                            return 1;
                        */
                    }

                    if (GlobalSetting.G_ENABLE_MACD_UP_CHECK &&
                        GlobalSetting.G_SEQ_PATH_BY_MACD_LINE)
                    {
                        float xDIF = (float)x.paramMap["DIF"];
                        float xDEA = (float)x.paramMap["DEA"];
                        float xBAR = (float)x.paramMap["BAR"];
                        int xmacdup = (int)x.paramMap["MacdUp"];

                        float yDIF = (float)y.paramMap["DIF"];
                        float yDEA = (float)y.paramMap["DEA"];
                        float yBAR = (float)y.paramMap["BAR"];
                        int ymacdup = (int)y.paramMap["MacdUp"];

                        bool isxup = xDIF > xDEA && xmacdup == 1;
                        bool isyup = yDIF > yDEA && ymacdup == 1;
                        bool isxFullUp = xDIF > xDEA && xmacdup == 2;
                        bool isyFullUp = yDIF > yDEA && ymacdup == 2;

                        if (isxFullUp && !isyFullUp)
                            return -1;
                        if (!isxFullUp && isyFullUp)
                            return 1;

                        if (isxup && !isyup)
                            return -1;
                        if (!isxup && isyup)
                            return 1;

                        if (xDIF > 0 && yDIF <= 0)
                            return -1;
                        if (xDIF <= 0 && yDIF > 0)
                            return 1;
                        return 0;
                    }

                    if (GlobalSetting.G_ENABLE_MACD_UP_CHECK &&
                        GlobalSetting.G_SEQ_PATH_BY_MACD_SIGNAL)
                    {
                        MacdSignal msX = (MacdSignal)x.paramMap["MacdSig"];
                        MacdSignal msY = (MacdSignal)y.paramMap["MacdSig"];
                        if (msX > msY)
                            return -1;
                        else if (msX < msY)
                            return 1;
                        return 0;
                    }

                    if (GlobalSetting.G_ENABLE_MACD_UP_CHECK &&
                        GlobalSetting.G_SEQ_PATH_BY_MACD_CFG)
                    {
                        int xCount = 0, yCount = 0;
                        bool XKUP = (float)x.paramMap["KGraph"] == 2.0f;
                        bool YKUP = (float)y.paramMap["KGraph"] == 2.0f;
                        bool isXKUp = (float)x.paramMap["KGraph"] >= 1.0f;
                        bool isYKUp = (float)y.paramMap["KGraph"] >= 1.0f;
                        bool isXMUp = (float)x.paramMap["MacdUp"] > 0;
                        bool isYMUp = (float)y.paramMap["MacdUp"] > 0;
                        MacdLineCfg xCfg = (MacdLineCfg)x.paramMap["MacdCfg"];
                        MacdLineCfg yCfg = (MacdLineCfg)y.paramMap["MacdCfg"];
                        bool isXGC = xCfg == MacdLineCfg.eGC || xCfg == MacdLineCfg.eGCFHES;
                        bool isYGC = yCfg == MacdLineCfg.eGC || yCfg == MacdLineCfg.eGCFHES;

                        // K线图是否提升
                        if ((float)x.paramMap["KGraph"] == 2) ++xCount;
                        // MACD图是否提升
                        if ((float)x.paramMap["MacdUp"] > 0) ++xCount;
                        // 当前5期内的出现率是否高于33%
                        if ((float)x.paramMap["curRateF"] > 33) ++xCount;
                        // 当期5期内的出现率是否比上期提升了
                        if ((float)x.paramMap["detRateF"] > 0) ++xCount;

                        if ((float)y.paramMap["KGraph"] == 2) ++yCount;
                        if ((float)y.paramMap["MacdUp"] > 0) ++yCount;
                        if ((float)y.paramMap["curRateF"] > 33) ++yCount;
                        if ((float)y.paramMap["detRateF"] > 0) ++yCount;

                        // 表现为提升的数据的量
                        x.paramMap["AnaCount"] = xCount;
                        y.paramMap["AnaCount"] = yCount;

                        if (isXGC && !isYGC)
                            return -1;
                        if (!isXGC && isYGC)
                            return 1;                            

                        if (xCount == 4 && yCount < 4)
                            return -1;
                        if (xCount < 4 && yCount == 4)
                            return 1;

                        if ((XKUP && isXMUp) && !(YKUP && isYMUp))
                            return -1;
                        if (!(XKUP && isXMUp) && (YKUP && isYMUp))
                            return 1;
                        if ((XKUP && isXMUp) && (YKUP && isYMUp))
                        {
                            if ((int)x.paramMap["KUP"] > 0 && (int)y.paramMap["KUP"] < 0)
                                return -1;
                            if ((int)x.paramMap["KUP"] < 0 && (int)y.paramMap["KUP"] > 0)
                                return 1;
                        }

                        if (isXKUp && isXMUp)
                        {
                            if (!(isYKUp && isYMUp))
                                return -1;
                        }
                        if (isYMUp && isYKUp)
                        {
                            if (!(isXKUp && isXMUp))
                                return 1;
                        }

                        if ((float)x.paramMap["KGraph"] > (float)y.paramMap["KGraph"] &&
                            isXMUp && !isYMUp)
                            return -1;
                        if ((float)x.paramMap["KGraph"] < (float)y.paramMap["KGraph"] &&
                            !isXMUp && isYMUp)
                            return 1;

                        if ((int)x.paramMap["MBAR"] > (int)y.paramMap["MBAR"])
                            return -1;
                        if ((int)x.paramMap["MBAR"] < (int)y.paramMap["MBAR"])
                            return 1;

                        if ((float)x.paramMap["curRateF"] > (float)y.paramMap["curRateF"] &&
                            (float)x.paramMap["detRateF"] >= (float)y.paramMap["detRateF"])
                            return -1;

                        if ((float)x.paramMap["curRateF"] < (float)y.paramMap["curRateF"] &&
                            (float)x.paramMap["detRateF"] <= (float)y.paramMap["detRateF"])
                            return 1;

                        if ((float)x.paramMap["MacdUp"] > (float)y.paramMap["MacdUp"])
                            return -1;
                        if ((float)x.paramMap["MacdUp"] < (float)y.paramMap["MacdUp"])
                            return 1;
                    }

                    if ((float)x.paramMap["curRateF"] > (float)y.paramMap["curRateF"])
                        return -1;
                    if ((float)x.paramMap["curRateF"] < (float)y.paramMap["curRateF"])
                        return 1;

                    if ((float)x.paramMap["detRateF"] > (float)y.paramMap["detRateF"])
                        return -1;
                    if ((float)x.paramMap["detRateF"] < (float)y.paramMap["detRateF"])
                        return 1;

                    //if (GlobalSetting.G_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE)
                    //{
                    //    if ((int)x.paramMap["count2LIM"] < (int)y.paramMap["count2LIM"])
                    //        return -1;
                    //    if ((int)x.paramMap["count2LIM"] > (int)y.paramMap["count2LIM"])
                    //        return 1;
                    //}

                    if ((int)x.paramMap["curMissCount"] < (int)y.paramMap["curMissCount"])
                        return -1;
                    if ((int)x.paramMap["curMissCount"] > (int)y.paramMap["curMissCount"])
                        return 1;
                    return 0;
                });

            //if (GlobalSetting.G_ENABLE_BOOLEAN_DOWN_UP_CHECK)
            //{
            //    if ((float)trade.pathCmpInfos[numIndex][0].paramMap["MayUpCount"] > 0 &&
            //        (float)trade.pathCmpInfos[numIndex][1].paramMap["MayUpCount"] > 0)
            //    {
            //        PathCmpInfo pci = trade.pathCmpInfos[numIndex][2];
            //        trade.pathCmpInfos[numIndex].RemoveAt(2);
            //        trade.pathCmpInfos[numIndex].Insert(0, pci);
            //    }
            //}

            CheckAndKeepSamePath(trade, numIndex, mayUpPathsCount);
        }

        CollectDataType GetPathCDT(int i)
        {
            switch(i)
            {
                case 0: return CollectDataType.ePath0;
                case 1: return CollectDataType.ePath1;
                case 2: return CollectDataType.ePath2;
            }
            return CollectDataType.eNone;
        }

        void AutoAnalyzeToolCheck(int numIndex, TradeDataBase trade)
        {
            int path0Index = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
            CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
            for( int i = 0; i < 3; ++i )
            {
                CollectDataType cdt = cdts[i];
                int selPathIndex = i;
                TradeDataOneStar osTrade = trade as TradeDataOneStar;
                PathCmpInfo pci = osTrade.FindInfoByPathIndex(numIndex, selPathIndex);
				pci.paramMap["DLNext"] = 0;
				pci.paramMap["DLPrev"] = 0;

                // 取这一路的通道线工具
                AutoAnalyzeTool.SingleAuxLineInfo sali = autoAnalyzeTool.GetSingleAuxLineInfo(numIndex, cdt);

                // 取下通道线
                if (GlobalSetting.G_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL)
                {
                    GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
                    KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
                    KDataMap kdd = kddc.GetKDataDict(trade.lastDateItem);
                    KData kd = kdd.GetData(cdt, false);
                    float missHeight = GraphDataManager.GetMissRelLength(cdt);
                    bool hasPrevKV, hasNextKV, hasPrevHitPt, hasNextHitPt;
                    float prevKV, nextKV, prevSlope, nextSlope;
                    float prevHitPtX, nextHitPtX, prevHitPtY, nextHitPtY;
                    // 计算当前期在下通道线上的K值
                    int testID = trade.lastDateItem.idGlobal + 1;
                    // 如果下支撑线存在
                    if (sali.downLineData.valid)
                    {
                        // 计算下后支撑线的参数
                        sali.downLineData.GetKValue(testID, kd.UpValue, -missHeight,
                            out hasPrevKV, out prevKV, out prevSlope, out hasPrevHitPt, out prevHitPtX, out prevHitPtY,
                            out hasNextKV, out nextKV, out nextSlope, out hasNextHitPt, out nextHitPtX, out nextHitPtY);
                        // 存在下后通道线
                        if (hasNextKV)
                        {
                            // 获取下后支撑线的最低值
                            float minKV = Math.Min(sali.downLineData.dataSharp.DownValue, sali.downLineData.dataNextSharp.DownValue);
                            // 计算当前K值在下后通道线上的垂直投影点与当前K值的距离
                            float willMissCount = kd.RelateDistTo(nextKV) / missHeight;
							pci.paramMap["DLNext"] = 1;
                            pci.paramMap["DLNextSlope"] = (float)nextSlope;
                            pci.paramMap["DLNextVDist"] = (float)willMissCount;
                            pci.paramMap["DLHasNextHitPt"] = hasNextHitPt ? 1 : 0;
                            if (hasNextHitPt)
                            {
                                pci.paramMap["DLNextHitPtXOF"] = nextHitPtX - testID;
                            }
                            float dlNextDist2Min = (kd.UpValue - minKV) / missHeight;
                            pci.paramMap["DLNextDist2Min"] = dlNextDist2Min;
                        }
                        // 下前通道线
                        if (hasPrevKV)
                        {
                            // 计算下前支撑线的参数
                            float minKV = Math.Min(sali.downLineData.dataSharp.DownValue, sali.downLineData.dataPrevSharp.DownValue);
                            // 计算当前K值在下前通道线上的垂直投影点与当前K值的距离
                            float willMissCount = kd.RelateDistTo(prevKV) / missHeight;
							pci.paramMap["DLPrev"] = 1;
                            pci.paramMap["DLPrevSlope"] = (float)prevSlope;
                            pci.paramMap["DLPrevVDist"] = (float)willMissCount;
                            pci.paramMap["DLHasPrevHitPt"] = hasPrevHitPt ? 1 : 0;
                            if (hasPrevHitPt)
                            {
                                pci.paramMap["DLPrevHitPtXOF"] = prevHitPtX - testID;
                            }
                            float dlPrevDist2Min = (kd.UpValue - minKV) / missHeight;
                            pci.paramMap["DLPrevDist2Min"] = dlPrevDist2Min;
                        }
                    }
                }
            }
        }


        bool _needChangePath = false;
        bool NeedChangePath
        {
            get { return _needChangePath; }
            set
            {
                _needChangePath = value;
            }
        }

        void CheckAndKeepSamePath(TradeDataOneStar trade, int numIndex, int mayUpPathsCount = 0)
        {
            if (GlobalSetting.G_ONLY_TRADE_SPEC_CDT)
                return;
            if (GlobalSetting.G_ENABLE_CheckAndKeepSamePath == false)
                return;

            if(GlobalSetting.G_CHANGE_PATH_ON_ALL_TRADE_MISS)
            {
                // 如果是标记重新选分路，那么就直接返回
                if(NeedChangePath)
                {
                    NeedChangePath = false;
                    return;
                }
            }
            if (CurrentTradeCountIndex != 0)
            {
                // 当前这次交易优先级最高的PathCmpInfo
                PathCmpInfo pciOpt0 = trade.pathCmpInfos[numIndex][0];

                // 如果不是在所有次数用完才切换分路
                if (GlobalSetting.G_CHANGE_PATH_ON_ALL_TRADE_MISS == false)
                {
                    //// 如果当前这一路是触到布林下轨且出现回补的，就交易这一路
                    //if (GlobalSetting.G_ENABLE_BOOLEAN_DOWN_UP_CHECK)
                    //{
                    //    if ((float)tmp.paramMap["MayUpCount"] > 0)
                    //        return;
                    //}

                    // 检测第0位的出号率是否比第1，2位的出号率的和还要大，如果是，直接交易第0位的号
                    if (GlobalSetting.G_ENABLE_MAX_APPEARENCE_FIRST)
                    {
                        if (pciOpt0.paramMap.ContainsKey("curRateF"))
                        {
                            float rate12 = (float)trade.pathCmpInfos[numIndex][1].paramMap["curRateF"] + (float)trade.pathCmpInfos[numIndex][2].paramMap["curRateF"];
                            float rate0 = (float)pciOpt0.paramMap["curRateF"];
                            if (rate0 > rate12)
                            {
                                return;
                            }
                        }
                    }

                    if (GlobalSetting.G_ENABLE_MACD_UP_CHECK &&
                        GlobalSetting.G_SEQ_PATH_BY_MACD_CFG)
                    {
                        // 如果当前表现为提升的类型等于大于3，那么就选择这一路
                        if ((int)pciOpt0.paramMap["AnaCount"] >= 3)
                            return;
                    }

                    //if (GlobalSetting.G_ENABLE_MACD_UP_CHECK && GlobalSetting.G_SEQ_PATH_BY_MACD_SIGNAL)
                    //{
                    //    MacdSignal msL = (MacdSignal)pciOpt0.paramMap["MacdSig"];
                    //    if (msL == MacdSignal.eFullUp)
                    //        return;
                    //}
                }

                TradeDataOneStar lastTrade = TradeDataManager.Instance.GetLatestTradeData() as TradeDataOneStar;
                if (lastTrade != null)
                {
                    // 找出上一次交易优先级最高的PathCmpInfo
                    PathCmpInfo lastPCI = lastTrade.pathCmpInfos[numIndex][0];
                    int lastTradePath = lastPCI.pathIndex;
                    // 如果2次交易不是选择的同一路
                    if (pciOpt0.pathIndex != lastTradePath)
                    {
                        // 找到上一次交易所选择的那一路在这次交易中的PathCmpInfo
                        int lastPathCurIndex = trade.FindIndex(numIndex, lastTradePath);
                        PathCmpInfo lastPathCurPCI = trade.pathCmpInfos[numIndex][lastPathCurIndex];

                        if(GlobalSetting.G_ENABLE_MACD_UP_CHECK && GlobalSetting.G_SEQ_PATH_BY_MACD_LINE)
                        {
                            int macdupL = (int)lastPathCurPCI.paramMap["MacdUp"];
                            if (macdupL == 0)
                                return;
                        }

                        if(GlobalSetting.G_ENABLE_MACD_UP_CHECK && GlobalSetting.G_SEQ_PATH_BY_MACD_SIGNAL)
                        {
                            MacdSignal msL = (MacdSignal)lastPathCurPCI.paramMap["MacdSig"];
                            if (msL < MacdSignal.eHalfDown)
                                return;
                        }

                        // 当上次选择的那一路在当前出号率很低的时候，就要重新选择分路了
                        if(GlobalSetting.G_CHANGE_PATH_ON_LOW_APPEARENCE_RATE)
                        {
                            // 如果5期内出现率等于0且15期内的出现率小于等于10%，那么就不选上次选的那路了
                            if ((float)lastPathCurPCI.paramMap["curRateF"] == 0.0f &&
                                (float)lastPathCurPCI.paramMap["curRateS"] <= 10.0f)
                            {
                                return;
                            }
                        }

                        // 如果是在全部交易都失败的时候，标记下次要重新选分路
                        if (GlobalSetting.G_CHANGE_PATH_ON_ALL_TRADE_MISS)
                        {
                            trade.pathCmpInfos[numIndex][0] = lastPathCurPCI;
                            trade.pathCmpInfos[numIndex][lastPathCurIndex] = pciOpt0;
                            if(CurrentTradeCountIndex == tradeCountList.Count - 1)
                                NeedChangePath = true;
                            return;
                        }

                        // 如果开启布林图形检测
                        if (GlobalSetting.G_ENABLE_BOLLEAN_CFG_CHECK)
                        {
                            // 如果上期选择的那路不是出现率提升或者高于理论出现率的，那么就不选择了
                            if ((bool)lastPathCurPCI.paramMap["isAppRatePrefer"] == false)
                                return;
                            // 如果上期选择的那路出现15期出现率变低，那么就不选择了
                            if ((float)lastPathCurPCI.paramMap["detRateS"] < 0)
                                return;

                            /*
                            // 如果上期选择的那路在这一期，布林中轨出现向下走了，那么就不再选择这一路
                            if ((int)lastPathCurPCI.paramMap["midKDC"] > 0)
                            {
                                return;
                            }
                            // 如果出号率低于理论概率，那么这一路也不再选择了
                            bool isHigherThanTheoAppRate = (bool)lastPathCurPCI.paramMap["isCurRateBTheoRate"];
                            if (!isHigherThanTheoAppRate)
                            {
                                return;
                            }
                            */
                        }

                        // 计算这一路的遗漏值
                        int curMissCount = -1;
                        if (lastPathCurPCI.paramMap.ContainsKey("curMissCount"))
                            curMissCount = (int)lastPathCurPCI.paramMap["curMissCount"];

                        //// 如果上期选择的那一路在这一期出现继续下行，那么就不再坚持选择往期的那一路
                        //if (GlobalSetting.G_ENABLE_BOOLEAN_DOWN_UP_CHECK)
                        //{
                        //    if ((float)lastPathCurPCI.paramMap["MayUpCount"] < 0 && curMissCount > 0)
                        //    {
                        //        return;
                        //    }
                        //}

                        if (GlobalSetting.G_ENABLE_MACD_UP_CHECK)
                        {
                            //// 如果上期选择的那一路在这一期是MACD柱下行或者k线下行，就不在坚持交易这一路了
                            //if ((float)lastPathCurPCI.paramMap["MacdUp"] < 0.0f ||
                            //    (float)lastPathCurPCI.paramMap["KGraph"] <= 0.0f)
                            //    return;

                            // 如果上期选择的那一路在这一期K值依然坚挺上升，我们就依旧选择这一路
                            if ((float)lastPathCurPCI.paramMap["KGraph"] == 2.0f)
                            {
                                trade.pathCmpInfos[numIndex][0] = lastPathCurPCI;
                                trade.pathCmpInfos[numIndex][lastPathCurIndex] = pciOpt0;
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }

                        // 取这一路的通道线工具
                        CollectDataType cdt = GetPathCDT(lastPathCurPCI.pathIndex);
                        AutoAnalyzeTool.SingleAuxLineInfo sali = autoAnalyzeTool.GetSingleAuxLineInfo(numIndex, cdt);
                        if(GlobalSetting.G_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL
                            && sali.downLineData.valid)
                        {
                            GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
                            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
                            KDataMap kdd = kddc.GetKDataDict(trade.lastDateItem);
                            KData kd = kdd.GetData(cdt, false); 
                            float missHeight = GraphDataManager.GetMissRelLength(cdt);

                            bool hasPrevKV, hasNextKV, hasPrevHitPt, hasNextHitPt;
                            float prevKV, nextKV, prevSlope, nextSlope;
                            float prevHitPtX, nextHitPtX, prevHitPtY, nextHitPtY;
                            // 计算当前期在下通道线上的K值
                            int testID = trade.lastDateItem.idGlobal + 1;
                            sali.downLineData.GetKValue(testID, kd.UpValue, -missHeight, 
                                out hasPrevKV, out prevKV, out prevSlope, out hasPrevHitPt, out prevHitPtX, out prevHitPtY,
                                out hasNextKV, out nextKV, out nextSlope, out hasNextHitPt, out nextHitPtX, out nextHitPtY);
                            // 下前通道线
                            if (hasPrevKV)
                            {
                                float minKV = Math.Min(sali.downLineData.dataSharp.DownValue, sali.downLineData.dataPrevSharp.DownValue);
                                // 通道线上的投影点的K值在K线上方超出2个遗漏点，表示k线下穿下通道线超过2个遗漏点了
                                float willMissCount = kd.RelateDistTo(prevKV) / missHeight;
                                if  (
                                        (
                                            willMissCount > 2
                                            || (/*prevSlope >= 0 &&*/ minKV - kd.KValue > 2)
                                            || !hasPrevHitPt
                                            || (
                                                    hasPrevHitPt
                                                    &&
                                                    (
                                                        (prevHitPtX < testID - 2)
                                                        || (prevHitPtX - testID) > (tradeCountList.Count - CurrentTradeCountIndex)
                                                    )
                                               )
                                        )
                                        &&
                                        curMissCount > 4
                                    )
                                    return;
                            }
                            if(hasNextKV)
                            {
                                float minKV = Math.Min(sali.downLineData.dataSharp.DownValue, sali.downLineData.dataNextSharp.DownValue);
                                // 通道线上的投影点的K值在K线上方超出2个遗漏点，表示k线下穿下通道线超过2个遗漏点了
                                float willMissCount = kd.RelateDistTo(nextKV) / missHeight;
                                if  (
                                        (
                                            willMissCount > 2
                                            || (/*nextSlope >= 0 &&*/ minKV - kd.KValue > 2)
                                            || !hasNextHitPt
                                            || (
                                                    hasNextHitPt
                                                    &&
                                                    (
                                                        nextHitPtX < testID - 2
                                                        || (nextHitPtX - testID) > (tradeCountList.Count - CurrentTradeCountIndex)
                                                    )
                                               )
                                         )
                                         &&
                                         curMissCount > 4
                                     )
                                {
                                    return;
                                }
                            }
                        }
                        
                        //// 过滤掉连续在布林线下轨附近的012路
                        //if(GlobalSetting.G_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE
                        //    && lastTrade.INDEX > 10
                        //    && curMissCount > 4)
                        //{
                        //    TradeDataOneStar t0 = GetTrade(lastTrade.INDEX - 0) as TradeDataOneStar;
                        //    TradeDataOneStar t1 = GetTrade(lastTrade.INDEX - 1) as TradeDataOneStar;
                        //    TradeDataOneStar t2 = GetTrade(lastTrade.INDEX - 2) as TradeDataOneStar;
                        //    if (t0 != null && t1 != null && t2 != null)
                        //    {
                        //        float c2bds0 = (float)GetPathInfo(t0, numIndex, lastTradePath).paramMap["count2BDs"];
                        //        float c2bds1 = (float)GetPathInfo(t1, numIndex, lastTradePath).paramMap["count2BDs"];
                        //        float c2bds2 = (float)GetPathInfo(t2, numIndex, lastTradePath).paramMap["count2BDs"];
                        //        // 连续3期这一路都走到布林线下轨了，那么就不再坚持选择这路了
                        //        if (Math.Abs(c2bds0) < 1 && Math.Abs(c2bds1) < 1 && Math.Abs(c2bds2) < 1)
                        //        {
                        //            return;
                        //        }
                        //    }
                        //}

                        //if ((int)lastPathCurPCI.paramMap["maxMissCount"] < MAX_MISS_COUNT_TOR &&
                        //    (int)lastPathCurPCI.paramMap["curMissCount"] < MAX_MISS_COUNT_TOR)
                        {
                            trade.pathCmpInfos[numIndex][0] = lastPathCurPCI;
                            trade.pathCmpInfos[numIndex][lastPathCurIndex] = pciOpt0;
                        }
                    }
                }
            }
        }

        PathCmpInfo GetPathInfo(TradeDataOneStar trade, int numIndex, int pathIndex)
        {
            List<PathCmpInfo> pcis = trade.pathCmpInfos[numIndex];
            for (int i = 0; i < pcis.Count; ++i)
            {
                if(pcis[i].pathIndex == pathIndex)
                {
                    return pcis[i];
                }
            }
            return null;
        }

        void CalcPathMissCountArea(DataItem item, TradeDataOneStar trade, int numIndex)
        {
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2 ,};
            float[] missCountAreas = new float[] { 0, 0, 0, };
            //float[] avgMissCountAreas = new float[] { 0, 0, 0, };
            int[] maxMissCount = new int[] { 0, 0, 0, };
            int[] missCount = new int[] { 0, 0, 0, };
            int[] maxMissCountID = new int[] { 0, 0, 0, };
            int MAX_MISS_COUNT_TOR = 4;

            for (int i = 0; i < cdts.Length; ++i )
            {
                StatisticUnit su = sum.statisticUnitMap[cdts[i]];
                maxMissCount[i] = su.sample5Data.prevMaxMissCount;
                missCount[i] = su.missCount;
                missCountAreas[i] = su.sample5Data.missCountArea;
                maxMissCountID[i] = su.sample5Data.prevMaxMissCountIndex;
            }

            //int validCount = 0;
            //int loop = LotteryStatisticInfo.FAST_COUNT;            
            //DataItem cItem = item;
            //while( cItem != null && loop > 0 )
            //{
            //    StatisticUnitMap csum = cItem.statisticInfo.allStatisticInfo[numIndex];
            //    for( int i = 0; i < cdts.Length; ++i )
            //    {
            //        int m = csum.statisticUnitMap[cdts[i]].missCount;
            //        if (maxMissCount[i] < m)
            //            maxMissCount[i] = m;
            //        avgMissCountAreas[i] = avgMissCountAreas[i] + csum.statisticUnitMap[cdts[i]].fastData.missCountArea;
            //    }
            //    cItem = cItem.parent.GetPrevItem(cItem);
            //    --loop;
            //    ++validCount;
            //}
            //for (int i = 0; i < cdts.Length; ++i)
            //{
            //    missCount[i] = sum.statisticUnitMap[cdts[i]].missCount;
            //    missCountAreas[i] = sum.statisticUnitMap[cdts[i]].fastData.missCountArea;
            //}
            //float total = missCountAreas[0] + missCountAreas[1] + missCountAreas[2];
            //avgMissCountAreas[0] = avgMissCountAreas[0] / validCount;
            //avgMissCountAreas[1] = avgMissCountAreas[1] / validCount;
            //avgMissCountAreas[2] = avgMissCountAreas[2] / validCount;

            //GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
            //KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            //KDataDict kdd = kddc.GetKDataDict(item);
            //MACDPointMap macdPM = kddc.GetMacdPointMap(kdd);

            trade.pathCmpInfos[numIndex].Clear();
            for (int i = 0; i < missCountAreas.Length; ++i)
            {
                PathCmpInfo pci = new PathCmpInfo(i, sum.statisticUnitMap[cdts[i]]);
                pci.paramMap["missCountAreas"] = missCountAreas[i];                
                //pci.paramMap["avgMissCountAreas"] = avgMissCountAreas[i];
                pci.paramMap["maxMissCount"] = maxMissCount[i];
                pci.paramMap["maxMissCountID"] = maxMissCountID[i];
                pci.paramMap["curMissCount"] = missCount[i];
                //MACDPoint mp = macdPM.GetData(cdts[i], false);
                //pci.paramMap["DEA"] = mp.DEA;
                //pci.paramMap["DIF"] = mp.DIF;
                //pci.paramMap["BAR"] = mp.BAR;
                //pci.paramMap["CFG"] = CalcMACDType(pci);
                trade.pathCmpInfos[numIndex].Add(pci);
            }

            trade.pathCmpInfos[numIndex].Sort(
                (x, y) =>
                {
                    //int xCFG = (int)x.paramMap["CFG"];
                    //int yCFG = (int)y.paramMap["CFG"];
                    //if (xCFG > 0 && yCFG == 0)
                    //    return -1;
                    //if (xCFG == 0 && yCFG > 0)
                    //    return 1;
                    //if(xCFG > 0 && yCFG > 0)
                    //{
                    //    if (xCFG > yCFG)
                    //        return -1;
                    //    if (xCFG < yCFG)
                    //        return 1;
                    //    if ((float)x.paramMap["DEA"] > (float)y.paramMap["DEA"])
                    //        return -1;
                    //    if ((float)x.paramMap["DEA"] < (float)y.paramMap["DEA"])
                    //        return 1;
                    //    if ((float)x.paramMap["BAR"] > (float)y.paramMap["BAR"])
                    //        return -1;
                    //    if ((float)x.paramMap["BAR"] < (float)y.paramMap["BAR"])
                    //        return 1;
                    //}

                    if ((float)x.paramMap["missCountAreas"] < (float)y.paramMap["missCountAreas"])
                        return -1;
                    else if ((float)x.paramMap["missCountAreas"] > (float)y.paramMap["missCountAreas"])
                        return 1;
                    else if ((int)x.paramMap["maxMissCount"] < (int)y.paramMap["maxMissCount"])
                        return -1;
                    else if ((int)x.paramMap["maxMissCount"] > (int)y.paramMap["maxMissCount"])
                        return 1;
                    return 0;
                });

            CheckAndKeepSamePath(trade, numIndex);
        }
        
        void CalcKValueDistToBolleanLine(DataItem item, int numIndex, CollectDataType[] cds, ref float[] count2BUs, ref float[] count2BMs, ref float[] count2BDs, ref int[] count2LIM)
        {
            GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataMap kdd = kddc.GetKDataDict(item);
            BollinPointMap bpm = kddc.GetBollinPointMap(kdd);
            for( int i = 0; i < cds.Length; ++i )
            {
                CalcKValueDistToBolleanLine(kdd, bpm, cds[i], ref count2BUs[i], ref count2BMs[i], ref count2BDs[i]);
                if (count2BUs[i] < 0)
                {
                    count2LIM[i] = (int)Math.Ceiling(Math.Abs(count2BUs[i]));
                }
                else if (count2BMs[i] < 0)
                {
                    count2LIM[i] = (int)Math.Ceiling(Math.Abs(count2BMs[i]));
                }
                else if (count2BDs[i] < 0)
                {
                    count2LIM[i] = (int)Math.Ceiling(Math.Abs(count2BDs[i]));
                }
                else
                {
                    count2LIM[i] = 0xFFFF;
                }
            }
        }

        void CalcKValueDistToBolleanLine(KDataMap kdd, BollinPointMap bpm, CollectDataType cdt, ref float count2BU, ref float count2BM, ref float count2BD)
        {
            KData kd = kdd.GetData(cdt, false);
            BollinPoint bp = bpm.GetData(cdt, false);
            float dist2bu = kd.RelateDistTo(bp.upValue);
            float dist2bm = kd.RelateDistTo(bp.midValue);
            float dist2bd = kd.RelateDistTo(bp.downValue);
            float missheight = GraphDataManager.GetMissRelLength(cdt);
            count2BU = dist2bu / missheight;
            count2BM = dist2bm / missheight;
            count2BD = dist2bd / missheight;
        }

        void CalcPaths(DataItem item, int numIndex, TradeDataOneStar trade)
        {
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            float[] pathValues = new float[] { 0, 0, 0, };
            pathValues[0] = sum.statisticUnitMap[CollectDataType.ePath0].sample5Data.appearProbabilityDiffWithTheory;
            pathValues[1] = sum.statisticUnitMap[CollectDataType.ePath1].sample5Data.appearProbabilityDiffWithTheory;
            pathValues[2] = sum.statisticUnitMap[CollectDataType.ePath2].sample5Data.appearProbabilityDiffWithTheory;
            trade.pathCmpInfos[numIndex].Clear();
            for (int i = 0; i < pathValues.Length; ++i)
            {
                PathCmpInfo pci = new PathCmpInfo(i, pathValues[i]);
                pci.avgPathValue = 0;
                trade.pathCmpInfos[numIndex].Add(pci);
            }
            int loop = 0;
            int lastID = historyTradeDatas.Count - 1;
            for (; loop < 5; ++loop)
            {
                if (lastID - loop < 0)
                    break;
                TradeDataOneStar prev = historyTradeDatas[lastID - loop] as TradeDataOneStar;
                prev.pathCmpInfos[numIndex].Sort((x, y) =>
                {
                    if (x.pathValue > y.pathValue)
                        return -1;
                    else if (x.pathValue < y.pathValue)
                        return 1;
                    return 0;
                });
                int bestID = prev.pathCmpInfos[numIndex][0].pathIndex;
                trade.pathCmpInfos[numIndex][bestID].avgPathValue += 1;
            }

            trade.pathCmpInfos[numIndex].Sort(
                (x, y) =>
            {
                if (x.pathValue > y.pathValue)
                    return -1;
                else if (x.pathValue < y.pathValue)
                    return 1;
                return 0;
            });
            trade.pathCmpInfos[numIndex][0].avgPathValue += 1;

            //float[] pathValues = new float[] { 0, 0, 0, };
            //int[] maxMissCounts = new int[3] { 0, 0, 0, };
            //int[] uponAvgLineCounts = new int[3] { 0, 0, 0 };
            //int[] maxMissID = new int[] { item.idGlobal, item.idGlobal, item.idGlobal, };
            //int[] minKValueID = new int[] { item.idGlobal, item.idGlobal, item.idGlobal, };
            //int[] vertCountFromCurToMinKValue = new int[] { 0, 0, 0, };
            //int[] vertCountFromCurToBollMidLine = new int[] { 0, 0, 0, };
            //int[] vertCountFromCurToBollDownLine = new int[] { 0, 0, 0, };
            //int[] belowAvgLineCounts = new int[3] { 0, 0, 0 };
            //KGraphConfig[] kgCfgs = new KGraphConfig[3] { KGraphConfig.eNone, KGraphConfig.eNone, KGraphConfig.eNone, };
            //GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
            //KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            //KDataDict kdd = kddc.GetKDataDict(item);
            //BollinPointMap bpm = kddc.GetBollinPointMap(kdd);

            //// 计算012路的K线图形态
            //kgCfgs[0] = CheckKGraphConfig(item, numIndex, kdd, bpm, CollectDataType.ePath0,
            //    ref belowAvgLineCounts[0], ref uponAvgLineCounts[0], ref maxMissCounts[0],
            //    ref maxMissID[0], ref minKValueID[0], ref vertCountFromCurToMinKValue[0], ref vertCountFromCurToBollMidLine[0], ref vertCountFromCurToBollDownLine[0]);
            //kgCfgs[1] = CheckKGraphConfig(item, numIndex, kdd, bpm, CollectDataType.ePath1,
            //    ref belowAvgLineCounts[1], ref uponAvgLineCounts[1], ref maxMissCounts[1],
            //    ref maxMissID[1], ref minKValueID[1], ref vertCountFromCurToMinKValue[1], ref vertCountFromCurToBollMidLine[1], ref vertCountFromCurToBollDownLine[1]);
            //kgCfgs[2] = CheckKGraphConfig(item, numIndex, kdd, bpm, CollectDataType.ePath2,
            //    ref belowAvgLineCounts[2], ref uponAvgLineCounts[2], ref maxMissCounts[2],
            //    ref maxMissID[2], ref minKValueID[2], ref vertCountFromCurToMinKValue[2], ref vertCountFromCurToBollMidLine[2], ref vertCountFromCurToBollDownLine[2]);

            //pathValues[0] = GetKGraphConfigValue(kgCfgs[0], belowAvgLineCounts[0], uponAvgLineCounts[0]);
            //pathValues[1] = GetKGraphConfigValue(kgCfgs[1], belowAvgLineCounts[1], uponAvgLineCounts[1]);
            //pathValues[2] = GetKGraphConfigValue(kgCfgs[2], belowAvgLineCounts[2], uponAvgLineCounts[2]);

            //trade.pathCmpInfos[numIndex].Clear();
            //for (int i = 0; i < pathValues.Length; ++i)
            //{
            //    int horzDist = item.idGlobal - maxMissID[i];
            //    PathCmpInfo pci = new PathCmpInfo(i, pathValues[i], horzDist, uponAvgLineCounts[i], vertCountFromCurToMinKValue[i], vertCountFromCurToBollMidLine[i], vertCountFromCurToBollDownLine[i]);
            //    pci.avgPathValue = pathValues[i];
            //    trade.pathCmpInfos[numIndex].Add(pci);
            //}
            //int valid_count = 1;
            //int lastID = historyTradeDatas.Count - 1;
            //while (lastID >= 0)
            //{
            //    ++valid_count;
            //    TradeDataOneStar ltrade = historyTradeDatas[lastID] as TradeDataOneStar;
            //    for (int i = 0; i < pathValues.Length; ++i)
            //    {
            //        PathCmpInfo lpci = ltrade.pathCmpInfos[numIndex][i];
            //        PathCmpInfo cpci = trade.pathCmpInfos[numIndex][lpci.pathIndex];
            //        cpci.avgPathValue += lpci.pathValue;
            //    }
            //    ++valid_count;
            //    --lastID;
            //    if (valid_count == 5)
            //    {
            //        break;
            //    }
            //}
            //for (int i = 0; i < pathValues.Length; ++i)
            //{
            //    trade.pathCmpInfos[numIndex][i].avgPathValue /= valid_count;
            //}

            //trade.pathCmpInfos[numIndex].Sort((x, y) =>
            //{
            //    if (x.maxVertDist < y.maxVertDist)
            //        return -1;
            //    else if(x.maxVertDist > y.maxVertDist)
            //        return 1;

            //    if (x.avgPathValue > y.avgPathValue)
            //        return -1;
            //    else if (x.avgPathValue < y.avgPathValue)
            //        return 1;

            //    if (x.pathValue > y.pathValue)
            //        return -1;
            //    else if (x.pathValue < y.pathValue)
            //        return 1;
            //    return 0;
            //});
        }

        void CalcPathValue(DataItem item, int numIndex, ref bool[] isPathStrongUp, out int[] uponAvgLineCounts, out float[] pathValues, out MACDLineWaveConfig[] mlCfgs, out MACDBarConfig[] mbCfgs, out KGraphConfig[] kgCfgs, out int[] maxMissCounts)
        {
            pathValues = new float[3] { 1, 1, 1 };
            float[] kValues = new float[3] { 1, 1, 1 };
            int[] belowAvgLineCounts = new int[3] { 0, 0, 0 };
            //int[] uponAvgLineCounts = new int[3] { 0, 0, 0 };
            uponAvgLineCounts = new int[3] { 0, 0, 0 };
            float[] proShort = new float[3] { 1, 1, 1, };
            int[] maxMissID = new int[] { 0, 0, 0, };
            int[] minKValueID = new int[] { 0, 0, 0, };
            int[] vertCountFromCurToMinKValue = new int[] { 0, 0, 0, };
            int[] vertCountFromCurToBollMidLine = new int[] { 0, 0, 0, };
            int[] vertCountFromCurToBollDownLine = new int[] { 0, 0, 0, };

            mlCfgs = new MACDLineWaveConfig[3] { MACDLineWaveConfig.eNone, MACDLineWaveConfig.eNone, MACDLineWaveConfig.eNone, };
            mbCfgs = new MACDBarConfig[3] { MACDBarConfig.eNone, MACDBarConfig.eNone, MACDBarConfig.eNone, };
            kgCfgs = new KGraphConfig[3] { KGraphConfig.eNone, KGraphConfig.eNone, KGraphConfig.eNone, };
            maxMissCounts = new int[3] { 0, 0, 0, };

            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            //int[] missCounts = new int[3] 
            //{
            //    sum.statisticUnitMap[CollectDataType.ePath0].missCount,
            //    sum.statisticUnitMap[CollectDataType.ePath1].missCount,
            //    sum.statisticUnitMap[CollectDataType.ePath2].missCount,
            //};
            GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataMap kdd = kddc.GetKDataDict(item);
            //// 5期均线
            //AvgPointMap apm5 = kddc.GetAvgPointMap(5, kdd);
            //// 10期均线
            //AvgPointMap apm10 = kddc.GetAvgPointMap(10, kdd);
            // 布林带数据
            BollinPointMap bpm = kddc.GetBollinPointMap(kdd);
            // MACD数据
            MACDPointMap mpm = kddc.GetMacdPointMap(kdd);
            MACDPoint mp0 = mpm.GetData(CollectDataType.ePath0, false);
            MACDPoint mp1 = mpm.GetData(CollectDataType.ePath1, false);
            MACDPoint mp2 = mpm.GetData(CollectDataType.ePath2, false);

            //float path0Avg5 = apm5.GetData(CollectDataType.ePath0, false).avgKValue;
            //float path1Avg5 = apm5.GetData(CollectDataType.ePath1, false).avgKValue;
            //float path2Avg5 = apm5.GetData(CollectDataType.ePath2, false).avgKValue;
            //float path0Avg10 = apm10.GetData(CollectDataType.ePath0, false).avgKValue;
            //float path1Avg10 = apm10.GetData(CollectDataType.ePath1, false).avgKValue;
            //float path2Avg10 = apm10.GetData(CollectDataType.ePath2, false).avgKValue;
            //float path0Bpm = bpm.GetData(CollectDataType.ePath0, false).midValue;
            //float path1Bpm = bpm.GetData(CollectDataType.ePath1, false).midValue;
            //float path2Bpm = bpm.GetData(CollectDataType.ePath2, false).midValue;
            {
                // 计算012路的K线图形态
                kgCfgs[0] = CheckKGraphConfig(item, numIndex, kdd, bpm, CollectDataType.ePath0, 
                    ref belowAvgLineCounts[0], ref uponAvgLineCounts[0], ref maxMissCounts[0], 
                    ref maxMissID[0], ref minKValueID[0], ref vertCountFromCurToMinKValue[0], ref vertCountFromCurToBollMidLine[0], ref vertCountFromCurToBollDownLine[0]);
                kgCfgs[1] = CheckKGraphConfig(item, numIndex, kdd, bpm, CollectDataType.ePath1, 
                    ref belowAvgLineCounts[1], ref uponAvgLineCounts[1], ref maxMissCounts[1],
                    ref maxMissID[1], ref minKValueID[1], ref vertCountFromCurToMinKValue[1], ref vertCountFromCurToBollMidLine[1], ref vertCountFromCurToBollDownLine[1]);
                kgCfgs[2] = CheckKGraphConfig(item, numIndex, kdd, bpm, CollectDataType.ePath2, 
                    ref belowAvgLineCounts[2], ref uponAvgLineCounts[2], ref maxMissCounts[2],
                    ref maxMissID[2], ref minKValueID[2], ref vertCountFromCurToMinKValue[2], ref vertCountFromCurToBollMidLine[2], ref vertCountFromCurToBollDownLine[2]);

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

                //proShort[0] = sum.statisticUnitMap[CollectDataType.ePath0].sample10Data.appearProbability;
                //proShort[1] = sum.statisticUnitMap[CollectDataType.ePath1].sample10Data.appearProbability;
                //proShort[2] = sum.statisticUnitMap[CollectDataType.ePath2].sample10Data.appearProbability;
                
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
            int[] uponAvgLineCounts;
            MACDLineWaveConfig[] lineCfgs;
            MACDBarConfig[] barCfgs;
            KGraphConfig[] kCfgs;
            int[] maxMissCounts;
            CalcPathValue(item, numIndex, ref isStrongUp, out uponAvgLineCounts, out pathValues, out lineCfgs, out barCfgs, out kCfgs, out maxMissCounts);

            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            StatisticUnit su0 = sum.statisticUnitMap[CollectDataType.ePath0];
            StatisticUnit su1 = sum.statisticUnitMap[CollectDataType.ePath1];
            StatisticUnit su2 = sum.statisticUnitMap[CollectDataType.ePath2];
            StatisticUnit[] sus = new StatisticUnit[] { su0, su1, su2, };

            //--------------------------
            trade.pathCmpInfos[numIndex].Clear();
            for (int i = 0; i < pathValues.Length; ++i)
            {
                PathCmpInfo pci = new PathCmpInfo(i, pathValues[i], uponAvgLineCounts[i], lineCfgs[i], barCfgs[i], kCfgs[i], sus[i], isStrongUp[i], maxMissCounts[i]);
                pci.avgPathValue = pathValues[i];
                trade.pathCmpInfos[numIndex].Add(pci);
            }

            int valid_count = 1;
            int lastID = historyTradeDatas.Count - 1;
            while (lastID >= 0)
            {
                ++valid_count;
                TradeDataOneStar ltrade = historyTradeDatas[lastID] as TradeDataOneStar;
                for (int i = 0; i < pathValues.Length; ++i)
                {
                    PathCmpInfo lpci = ltrade.pathCmpInfos[numIndex][i];
                    PathCmpInfo cpci = trade.pathCmpInfos[numIndex][lpci.pathIndex];
                    cpci.avgPathValue += lpci.pathValue;
                }
                ++valid_count;
                --lastID;
                if(valid_count == 5)
                {
                    break;
                }
            }
            for (int i = 0; i < pathValues.Length; ++i)
            {
                trade.pathCmpInfos[numIndex][i].avgPathValue /= valid_count;
            }

            trade.pathCmpInfos[numIndex].Sort((x, y) =>
            {
                if(onlyTradeOnStrongUpPath)
                {
                    if (x.isStrongUp && y.isStrongUp == false)
                        return -1;
                    else if (x.isStrongUp == false && y.isStrongUp)
                        return 1;
                }
                if (x.avgPathValue > y.avgPathValue)
                    return -1;
                else if (x.avgPathValue < y.avgPathValue)
                    return 1;

                if (x.pathValue > y.pathValue)
                    return -1;
                else if (x.pathValue < y.pathValue)
                    return 1;

                if (x.su.sample10Data.appearProbability > y.su.sample10Data.appearProbability)
                    return -1;
                else if (x.su.sample10Data.appearProbability < y.su.sample10Data.appearProbability)
                    return 1;
                
                if (x.su.sample30Data.appearProbability > y.su.sample30Data.appearProbability)
                    return -1;
                else if (x.su.sample30Data.appearProbability < y.su.sample30Data.appearProbability)
                    return 1;

                if (x.maxMissCount < y.maxMissCount)
                    return -1;
                else if (x.maxMissCount > y.maxMissCount)
                    return 1;

                return 0;
            });
            //--------------------------

            StatisticUnit curBestSU = null;
            float curBestV = 0;
            int curBestPath = -1;
            curBestPath = trade.pathCmpInfos[numIndex][0].pathIndex;
            curBestSU = trade.pathCmpInfos[numIndex][0].su;
            curBestV = trade.pathCmpInfos[numIndex][0].pathValue;

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
                    if(suBest.sample10Data.appearProbability < curBestSU.sample10Data.appearProbability)
                    {
                        bestPath = curBestPath;
                        bestNumIndex = numIndex;
                        maxV = curBestV;
                        isFinalPathStrongUp = isStrongUp[curBestPath];
                    }
                    else if(suBest.sample10Data.appearProbability == curBestSU.sample10Data.appearProbability)
                    {
                        if (suBest.sample30Data.appearProbability < curBestSU.sample30Data.appearProbability)
                        {
                            bestPath = curBestPath;
                            bestNumIndex = numIndex;
                            maxV = curBestV;
                            isFinalPathStrongUp = isStrongUp[curBestPath];
                        }
                    }
                }

                //CheckAndKeepSamePath(trade, numIndex);
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

        public static void FindAllNumberProbabilities(DataItem item, ref List<NumberCmpInfo> nums, int cycle)
        {
            nums.Clear();
            int realCount = item.idGlobal + 1;
            int MAX_COUNT = cycle;
            if (realCount > MAX_COUNT)
                realCount = MAX_COUNT;
            int countDown = realCount;
            float total = 5 * realCount;

            while (countDown > 0)
            {
                for (int i = 0; i < 5; ++i)
                {
                    StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[i];
                    int startIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum0);
                    int endIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum9);
                    for (int num = startIndex; num <= endIndex; ++num)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[num];
                        SByte number = (SByte)(num - startIndex);
                        NumberCmpInfo info = GetNumberCmpInfo(ref nums, number, true);
                        if (sum.statisticUnitMap[cdt].missCount == 0)
                        {
                            info.appearCount += 1;
                            info.rate = info.appearCount * 100 / total;
                            info.largerThanTheoryProbability = info.rate > GraphDataManager.GetTheoryProbability(cdt);
                        }
                    }
                }

                --countDown;
                item = item.parent.GetPrevItem(item);
            }
            nums.Sort(NumberCmpInfo.SortByAppearCount);
        }

        public static void FindSpecNumIndexPathsProbabilities(DataItem item, ref List<PathCmpInfo> paths, int numIndex, int cycle)
        {
            if (paths.Count == 0)
            {
                paths.Add(new PathCmpInfo(0, 0));
                paths.Add(new PathCmpInfo(1, 0));
                paths.Add(new PathCmpInfo(2, 0));
            }

            int realCount = item.idGlobal + 1;
            int MAX_COUNT = cycle;
            if (realCount > MAX_COUNT)
                realCount = MAX_COUNT;
            int countDown = realCount;
            float total = realCount;

            while (countDown > 0)
            {
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
                int startIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
                int endIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath2);
                for (int num = startIndex; num <= endIndex; ++num)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[num];
                    int pathIndex = num - startIndex;
                    PathCmpInfo info = paths[pathIndex];
                    if (sum.statisticUnitMap[cdt].missCount == 0)
                    {
                        info.pathValue += 1;
                    }
                }

                --countDown;
                item = item.parent.GetPrevItem(item);
            }

            for (int i = 0; i < 3; ++i)
            {
                paths[i].pathValue = paths[i].pathValue * 100 / total;
                paths[i].paramMap[cycle.ToString()] = paths[i].pathValue;
            }

        }

        public static void FindAllPathsProbabilities(DataItem item, ref List<PathCmpInfo> paths, int cycle)
        {
            paths.Clear();
            paths.Add(new PathCmpInfo(0, 0));
            paths.Add(new PathCmpInfo(1, 0));
            paths.Add(new PathCmpInfo(2, 0));

            int realCount = item.idGlobal + 1;
            int MAX_COUNT = cycle;
            if (realCount > MAX_COUNT)
                realCount = MAX_COUNT;
            int countDown = realCount;
            float total = 5 * realCount;
            
            while(countDown > 0)
            {
                for (int i = 0; i < 5; ++i)
                {
                    StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[i];
                    int startIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
                    int endIndex = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath2);
                    for (int num = startIndex; num <= endIndex; ++num)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[num];
                        int pathIndex = num - startIndex;
                        PathCmpInfo info = paths[pathIndex];
                        if (sum.statisticUnitMap[cdt].missCount == 0)
                        {
                            info.pathValue += 1;
                        }
                    }
                }

                --countDown;
                item = item.parent.GetPrevItem(item);
            }
            
            for(int i = 0; i < 3; ++i)
            {
                paths[i].pathValue = paths[i].pathValue * 100 / total;
            }
        }

        public static void FindAllNumberProbabilities(DataItem item, ref List<NumberCmpInfo> nums, bool collectByLongCount = true)
        {
            nums.Clear();
            int realCount = item.idGlobal + 1;
            int MAX_COUNT = collectByLongCount ? LotteryStatisticInfo.SAMPLE_COUNT_30 : LotteryStatisticInfo.SAMPLE_COUNT_10;
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
                    info.appearCount += collectByLongCount ? sum.statisticUnitMap[cdt].sample30Data.appearCount : sum.statisticUnitMap[cdt].sample10Data.appearCount;
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
            int MAX_COUNT = collectByLongCount ? LotteryStatisticInfo.SAMPLE_COUNT_30 : LotteryStatisticInfo.SAMPLE_COUNT_10;
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
                    info.appearCount += collectByLongCount ? sum.statisticUnitMap[cdt].sample30Data.appearCount : sum.statisticUnitMap[cdt].sample10Data.appearCount;
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
                    info.rate = sum.statisticUnitMap[cdt].sample30Data.appearProbability;
                    info.largerThanTheoryProbability = sum.statisticUnitMap[cdt].sample30Data.appearProbability > GraphDataManager.GetTheoryProbability(cdt);

                    if (nums.Count == 0)
                    {
                        nums.Add(info);
                    }
                    else
                    {
                        bool hasInsert = false;
                        for( int j = 0; j < nums.Count; ++j )
                        {
                            if(sum.statisticUnitMap[cdt].sample30Data.appearProbability > nums[j].rate)
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
    

}
