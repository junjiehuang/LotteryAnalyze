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

    public class PathCmpInfo
    {
        public Dictionary<string, object> paramMap = new Dictionary<string, object>();
        public int pathIndex;
        public StatisticUnit su;

        public float pathValue;
        public int uponBMCount;
        public TradeDataManager.MACDLineWaveConfig macdLineCfg;
        public TradeDataManager.MACDBarConfig macdBarCfg;
        public TradeDataManager.KGraphConfig kGraphCfg;
        
        public bool isStrongUp;
        public int maxMissCount;

        public float avgPathValue = 0;

        public int horzDist = 0;
        public int vertDistToMinKValue = 0;
        public int vertDistToBML = 0;
        public int vertDistToBDL = 0;
        public int maxVertDist = 0;

        public PathCmpInfo(int _id, StatisticUnit _su)
        {
            pathIndex = _id;
            su = _su;
        }

        public PathCmpInfo(int id, float v, int _uponBMCount, 
            TradeDataManager.MACDLineWaveConfig lineCFG,
            TradeDataManager.MACDBarConfig barCFG, 
            TradeDataManager.KGraphConfig kCFG,            
            StatisticUnit _su,
            bool _isStrongUp, 
            int  _maxMissCount)
        {
            pathIndex = id;
            pathValue = v;
            macdLineCfg = lineCFG;
            macdBarCfg = barCFG;
            kGraphCfg = kCFG;
            uponBMCount = _uponBMCount;
            su = _su;
            isStrongUp = _isStrongUp;
            maxMissCount = _maxMissCount;
        }

        public PathCmpInfo(int id, float v, int _uponBMCount)
        {
            pathIndex = id;
            pathValue = v;
            uponBMCount = _uponBMCount;
        }

        public PathCmpInfo(int id, float v)
        {
            pathIndex = id;
            pathValue = v;
        }

        public PathCmpInfo(int id, float v, int _horzDist, int _uponBMCount, int _vertDistToMinKValue, int _vertDistToBML, int _vertDistToBDL)
        {
            pathIndex = id;
            pathValue = v;
            horzDist = _horzDist;
            uponBMCount = _uponBMCount;
            vertDistToMinKValue = _vertDistToMinKValue;
            vertDistToBML = _vertDistToBML;
            vertDistToBDL = _vertDistToBDL;
            //maxVertDist = Math.Abs(vertDistToMaxMissV);
            //if (vertDistToBML < 0 && maxVertDist < -vertDistToBML)
            //    maxVertDist = Math.Abs(vertDistToBML);
            if (vertDistToBML < 0)
            {
                maxVertDist = vertDistToBML;
                if (vertDistToMinKValue < 0 && vertDistToMinKValue > vertDistToBML)
                    maxVertDist = vertDistToMinKValue;
            }
            else if (vertDistToBML > 0)
            {
                if (vertDistToMinKValue < 0)
                {
                    maxVertDist = -vertDistToMinKValue;
                    if (vertDistToBDL < 0 && vertDistToBDL > vertDistToMinKValue)
                        maxVertDist = -vertDistToBDL;
                }
                else
                    maxVertDist = 0xFFFFFF;
            }
            else
                maxVertDist = 0;
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
                int index = TradeDataManager.Instance.tradeCountList.IndexOf(tradeCount);

                info += "[" + index + ", "+ tradeCount+"] {";
                for (int i = 0; i < tradeNumbers.Count; ++i)
                {
                    info += tradeNumbers[i].ToString();
                    if (i != tradeNumbers.Count - 1)
                        info += ",";
                }
                info += "}\n";
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
        public void AddProbabilityNumber(NumberCmpInfo nci)
        {
            tradeNumbers.Add(nci);
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
        public void RemoveNumber(SByte number)
        {
            for (int i = 0; i < tradeNumbers.Count; ++i)
            {
                if (tradeNumbers[i].number == number)
                {
                    tradeNumbers.RemoveAt(i);
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
        static Type IT = typeof(int);
        static Type FT = typeof(float);
        static Type BT = typeof(bool);

        public static float SingleTradeCost = 1;
        public static float SingleTradeReward = 9.8f;

        public Dictionary<int, TradeNumbers> tradeInfo = new Dictionary<int, TradeNumbers>();

#if TRADE_DBG
        public List<List<PathCmpInfo>> pathCmpInfos = new List<List<PathCmpInfo>>();
        public int FindIndex(int numindex, int pathIndex)
        {
            if (numindex < 0 || numindex >= pathCmpInfos.Count)
                return -1;
            if (pathIndex < 0 || pathIndex >= pathCmpInfos[numindex].Count)
                return -1;
            for( int i = 0; i < pathCmpInfos[numindex].Count; ++ i )
            {
                if (pathCmpInfos[numindex][i].pathIndex == pathIndex)
                    return i;
            }
            return -1;
        }
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
            List<PathCmpInfo> pcis = null;
            PathCmpInfo pci = null;
            for (int i = 0; i < pathCmpInfos.Count; ++i)
            {
                pcis = pathCmpInfos[i];
                for (int j = 0; j < pcis.Count; ++j)
                {
                    pci = pcis[j];
                    dbgtxt += "[" + pci.pathIndex + "] ";
                    var etor = pci.paramMap.GetEnumerator();
                    while(etor.MoveNext())
                    {
                        dbgtxt += etor.Current.Key + "=" + etor.Current.Value + ", ";
                    }
                    dbgtxt += "\n";
                    //dbgtxt += "[" + pci.pathIndex + " = " + pci.pathValue;
                    //dbgtxt += ", AvgP = " + pci.avgPathValue.ToString();
                    //if (pci.kGraphCfg != TradeDataManager.KGraphConfig.eNone)
                    //    dbgtxt += ", K = " + pci.kGraphCfg.ToString();
                    //if (pci.macdLineCfg != TradeDataManager.MACDLineWaveConfig.eNone)
                    //    dbgtxt += ", L = " + pci.macdLineCfg.ToString();
                    //if (pci.macdBarCfg != TradeDataManager.MACDBarConfig.eNone)
                    //    dbgtxt += ", B = " + pci.macdBarCfg.ToString();
                    //dbgtxt += ", HD = " + pci.horzDist.ToString();
                    //dbgtxt += ", UBMC = " + pci.uponBMCount.ToString();
                    //dbgtxt += ", VD2MKV = " + pci.vertDistToMinKValue.ToString();
                    //dbgtxt += ", VD2BML = " + pci.vertDistToBML.ToString();
                    //dbgtxt += ", VD2BDL = " + pci.vertDistToBDL.ToString();
                    //dbgtxt += ", MaxVD = " + pci.maxVertDist.ToString();
                    //dbgtxt += "]\n";

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
            // 选择某个数值位012路顶低区间在交易次数范围内的号码
            eSinglePositionPathOnArea,
            // 选择某个数值位012路遗漏图面积最小的那一路的号码
            eSinglePositionSmallestMissCountArea,
            // 根据012路出号概率来决定选中那一路的号码
            eSinglePositionPathByAppearencePosibility,
            // 只要哪个数字位的最优012路满足就进行交易
            eMultiNumPath,

            // 选择某个数字位连续N期出号概率最高的几个数
            eSingleMostPosibilityNums,
            // 所有数字位都选择连续N期出号概率最高的几个数
            eMultiMostPosibilityNums,
            // 选择某个数字位短期长期概率最好的几个数
            eSingleShortLongMostPosibilityNums,
            // 选择几率最大的012路
            eSingleMostPosibilityPath,

            // 单个数字位按所有排列顺序权重叠加筛选
            sSinglePositionCondictionsSuperposition,
        }
        public static string[] STRATEGY_NAMES = 
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
            "eSingleMostPosibilityNums",
            "eMultiMostPosibilityNums",
            "eSingleShortLongMostPosibilityNums",
            "eSingleMostPosibilityPath",

            "sSinglePositionCondictionsSuperposition",
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
            autoAnalyzeTool.Analyze(item.idGlobal, KGRAPH_LOOP_COUNT);

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
                case TradeStrategy.sSinglePositionCondictionsSuperposition:
                    TradeSinglePositionCondictionsSuperposition(item, trade);
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

            CalcPathAppearence(item, trade, bestNumIndex);

            TradeNumbers tn = new TradeNumbers();
            tn.tradeCount = tradeCount;
            trade.tradeInfo.Add(bestNumIndex, tn);
            FindOverTheoryProbabilityNums(item, bestNumIndex, ref maxProbilityNums);
            PathCmpInfo pci0 = trade.pathCmpInfos[bestNumIndex][0];
            PathCmpInfo pci1 = trade.pathCmpInfos[bestNumIndex][1];
            tn.SelPath012Number(pci0.pathIndex, tradeCount, ref maxProbilityNums);
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
            byte ac0 = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[CollectDataType.ePath0].shortData.appearCount;
            byte ac1 = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[CollectDataType.ePath1].shortData.appearCount;
            byte ac2 = item.statisticInfo.allStatisticInfo[numID].statisticUnitMap[CollectDataType.ePath2].shortData.appearCount;
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

        //class NumberValue
        //{
        //    public int number;
        //    public float value; 

        //    public NumberValue(int num, float v)
        //    {
        //        number = num;
        //        value = v;
        //    }
        //}
        void TradeSinglePositionCondictionsSuperposition(DataItem item, TradeDataOneStar trade)
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
                StaticData sd = su.fastData;
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
                if (suA.shortData.appearProbability > suB.shortData.appearProbability)
                {
                    curBestPath = indexA;
                    curBestV = pathValueA;
                    curBeshSU = suA;
                }
                else if (suA.shortData.appearProbability < suB.shortData.appearProbability)
                {
                    curBestPath = indexB;
                    curBestV = pathValueB;
                    curBeshSU = suB;
                }
                else
                {
                    if (suA.longData.appearProbability > suB.longData.appearProbability)
                    {
                        curBestPath = indexA;
                        curBestV = pathValueA;
                        curBeshSU = suA;
                    }
                    else if (suA.longData.appearProbability < suB.longData.appearProbability)
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

        public static KGraphConfig CheckKGraphConfig(DataItem di, int numIndex, KDataDict item, BollinPointMap bpm, CollectDataType cdt, 
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
            KDataDict curItem = item;
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
            minKValueID = minID;
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
                if (x.su.shortData.appearProbability > y.su.shortData.appearProbability)
                    return -1;
                else if (x.su.shortData.appearProbability < y.su.shortData.appearProbability)
                    return 1;
                else
                {
                    if (x.su.longData.appearProbability > y.su.longData.appearProbability)
                        return -1;
                    else if (x.su.longData.appearProbability < y.su.longData.appearProbability)
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
            KGraphDataContainer kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataDict kdd = kddc.GetKDataDict(item);
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

        void CalcPathAppearence(DataItem item, TradeDataOneStar trade, int numIndex)
        {
            DataItem prvItem = item.parent.GetPrevItem(item);
            StatisticUnitMap sumCUR = item.statisticInfo.allStatisticInfo[numIndex];
            StatisticUnitMap sumPRV = null;
            if (prvItem != null)
                sumPRV = prvItem.statisticInfo.allStatisticInfo[numIndex];
            CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2, };
            float[] appearenceRateFastCUR = new float[] { 0, 0, 0, };
            float[] appearenceRateFastPRV = new float[] { 0, 0, 0, };
            //float[] appearenceRateShortCUR = new float[] { 0, 0, 0, };
            //float[] appearenceRateShortPRV = new float[] { 0, 0, 0, };
            //int[] maxMissCount = new int[] { 0, 0, 0, };
            //int[] curMissCount = new int[] { 0, 0, 0, };
            for (int i = 0; i < cdts.Length; ++i)
            {
                CollectDataType cdt = cdts[i];
                //maxMissCount[i] = sumCUR.statisticUnitMap[cdt].fastData.prevMaxMissCount;
                //curMissCount[i] = sumCUR.statisticUnitMap[cdt].missCount;
                appearenceRateFastCUR[i] = sumCUR.statisticUnitMap[cdt].fastData.appearProbability;
                //appearenceRateShortCUR[i] = sumCUR.statisticUnitMap[cdt].shortData.appearProbability;
                if (sumPRV != null)
                {
                    appearenceRateFastPRV[i] = sumPRV.statisticUnitMap[cdt].fastData.appearProbability;
                    //appearenceRateShortPRV[i] = sumPRV.statisticUnitMap[cdt].shortData.appearProbability;
                }
            }

            float[] count2BUs = new float[] { 0, 0, 0, };
            float[] count2BMs = new float[] { 0, 0, 0, };
            float[] count2BDs = new float[] { 0, 0, 0, };
            int[] count2LIM = new int[] { 0, 0, 0, };
            CalcKValueDistToBolleanLine(item, numIndex, cdts, ref count2BUs, ref count2BMs, ref count2BDs, ref count2LIM);

            trade.pathCmpInfos[numIndex].Clear();
            for (int i = 0; i < cdts.Length; ++i)
            {
                PathCmpInfo pci = new PathCmpInfo(i, sumCUR.statisticUnitMap[cdts[i]]);
                //pci.paramMap["maxMissCount"] = maxMissCount[i];
                //pci.paramMap["curMissCount"] = curMissCount[i];
                //pci.paramMap["prvRateF"] = appearenceRateFastPRV[i];
                pci.paramMap["curRateF"] = appearenceRateFastCUR[i];
                pci.paramMap["detRateF"] = appearenceRateFastCUR[i] - appearenceRateFastPRV[i];
                //pci.paramMap["prvRateS"] = appearenceRateShortPRV[i];
                //pci.paramMap["curRateS"] = appearenceRateShortCUR[i];
                //pci.paramMap["detRateS"] = appearenceRateShortCUR[i] - appearenceRateShortPRV[i];
                pci.paramMap["count2LIM"] = count2LIM[i];
                pci.paramMap["count2BUs"] = count2BUs[i];
                pci.paramMap["count2BMs"] = count2BMs[i];
                pci.paramMap["count2BDs"] = count2BDs[i];
                trade.pathCmpInfos[numIndex].Add(pci);
            }

            trade.pathCmpInfos[numIndex].Sort(
                (x, y) =>
                {
                    if ((float)x.paramMap["curRateF"] > (float)y.paramMap["curRateF"])
                        return -1;
                    if ((float)x.paramMap["curRateF"] < (float)y.paramMap["curRateF"])
                        return 1;
                    if ((float)x.paramMap["detRateF"] > 0 && (float)y.paramMap["detRateF"] <= 0)
                        return -1;
                    if ((float)x.paramMap["detRateF"] <= 0 && (float)y.paramMap["detRateF"] > 0)
                        return 1;

                    if ((int)x.paramMap["count2LIM"] < (int)y.paramMap["count2LIM"])
                        return -1;
                    if ((int)x.paramMap["count2LIM"] > (int)y.paramMap["count2LIM"])
                        return 1;
                    return 0;
                });

            CheckAndKeepSamePath(trade,numIndex);
        }

        void CheckAndKeepSamePath(TradeDataOneStar trade, int numIndex)
        {
            if (CurrentTradeCountIndex != 0)
            {
                PathCmpInfo tmp = trade.pathCmpInfos[numIndex][0];
                
                TradeDataOneStar lastTrade = TradeDataManager.Instance.GetLatestTradeData() as TradeDataOneStar;
                if (lastTrade != null)
                {
                    PathCmpInfo lastPCI = lastTrade.pathCmpInfos[numIndex][0];
                    int lastTradePath = lastPCI.pathIndex;
                    if (trade.pathCmpInfos[numIndex][0].pathIndex != lastTradePath)
                    {
                        int lastPathCurIndex = trade.FindIndex(numIndex, lastTradePath);
                        PathCmpInfo lastPathCurPCI = trade.pathCmpInfos[numIndex][lastPathCurIndex];

                        if (lastPathCurPCI.paramMap.ContainsKey("count2LIM"))
                        {
                            int count = (int)lastPathCurPCI.paramMap["count2LIM"];
                            if (count + CurrentTradeCountIndex > tradeCountList.Count)
                                return;
                        }
                        //if ((int)lastPathCurPCI.paramMap["maxMissCount"] < MAX_MISS_COUNT_TOR &&
                        //    (int)lastPathCurPCI.paramMap["curMissCount"] < MAX_MISS_COUNT_TOR)
                        {
                            trade.pathCmpInfos[numIndex][0] = lastPathCurPCI;
                            trade.pathCmpInfos[numIndex][lastPathCurIndex] = tmp;
                        }
                    }
                }
            }
        }

        void CalcPathMissCountArea(DataItem item, TradeDataOneStar trade, int numIndex)
        {
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
            CollectDataType[] cdts = new CollectDataType[] { CollectDataType.ePath0, CollectDataType.ePath1, CollectDataType.ePath2 ,};
            float[] missCountAreas = new float[] { 0, 0, 0, };
            //float[] avgMissCountAreas = new float[] { 0, 0, 0, };
            int[] maxMissCount = new int[] { 0, 0, 0, };
            int[] missCount = new int[] { 0, 0, 0, };
            int MAX_MISS_COUNT_TOR = 4;

            for (int i = 0; i < cdts.Length; ++i )
            {
                StatisticUnit su = sum.statisticUnitMap[cdts[i]];
                maxMissCount[i] = su.fastData.prevMaxMissCount;
                missCount[i] = su.missCount;
                missCountAreas[i] = su.fastData.missCountArea;
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

            //KGraphDataContainer kgdc = GraphDataManager.KGDC;
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
                pci.paramMap["missCount"] = missCount[i];
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
            KGraphDataContainer kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataDict kdd = kddc.GetKDataDict(item);
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

        void CalcKValueDistToBolleanLine(KDataDict kdd, BollinPointMap bpm, CollectDataType cdt, ref float count2BU, ref float count2BM, ref float count2BD)
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
            pathValues[0] = sum.statisticUnitMap[CollectDataType.ePath0].fastData.appearProbabilityDiffWithTheory;
            pathValues[1] = sum.statisticUnitMap[CollectDataType.ePath1].fastData.appearProbabilityDiffWithTheory;
            pathValues[2] = sum.statisticUnitMap[CollectDataType.ePath2].fastData.appearProbabilityDiffWithTheory;
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
            //KGraphDataContainer kgdc = GraphDataManager.KGDC;
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
            KGraphDataContainer kgdc = GraphDataManager.KGDC;
            KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
            KDataDict kdd = kddc.GetKDataDict(item);
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

                //proShort[0] = sum.statisticUnitMap[CollectDataType.ePath0].shortData.appearProbability;
                //proShort[1] = sum.statisticUnitMap[CollectDataType.ePath1].shortData.appearProbability;
                //proShort[2] = sum.statisticUnitMap[CollectDataType.ePath2].shortData.appearProbability;
                
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

                if (x.su.shortData.appearProbability > y.su.shortData.appearProbability)
                    return -1;
                else if (x.su.shortData.appearProbability < y.su.shortData.appearProbability)
                    return 1;
                
                if (x.su.longData.appearProbability > y.su.longData.appearProbability)
                    return -1;
                else if (x.su.longData.appearProbability < y.su.longData.appearProbability)
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
                    if(suBest.shortData.appearProbability < curBestSU.shortData.appearProbability)
                    {
                        bestPath = curBestPath;
                        bestNumIndex = numIndex;
                        maxV = curBestV;
                        isFinalPathStrongUp = isStrongUp[curBestPath];
                    }
                    else if(suBest.shortData.appearProbability == curBestSU.shortData.appearProbability)
                    {
                        if (suBest.longData.appearProbability < curBestSU.longData.appearProbability)
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
                    info.appearCount += collectByLongCount ? sum.statisticUnitMap[cdt].longData.appearCount : sum.statisticUnitMap[cdt].shortData.appearCount;
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
                    info.appearCount += collectByLongCount ? sum.statisticUnitMap[cdt].longData.appearCount : sum.statisticUnitMap[cdt].shortData.appearCount;
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
                    info.rate = sum.statisticUnitMap[cdt].longData.appearProbability;
                    info.largerThanTheoryProbability = sum.statisticUnitMap[cdt].longData.appearProbability > GraphDataManager.GetTheoryProbability(cdt);

                    if (nums.Count == 0)
                    {
                        nums.Add(info);
                    }
                    else
                    {
                        bool hasInsert = false;
                        for( int j = 0; j < nums.Count; ++j )
                        {
                            if(sum.statisticUnitMap[cdt].longData.appearProbability > nums[j].rate)
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
        

        //public Dictionary<int, int> missCountInfos = new Dictionary<int, int>();
        public Dictionary<CollectDataType, Dictionary<int, int>> missCountInfos = new Dictionary<CollectDataType, Dictionary<int, int>>();
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

            for (int i = 0; i < 5; ++i)
            {
                StatisticUnitMap sum = trade.lastDateItem.statisticInfo.allStatisticInfo[i];
                for( int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j )
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    StatisticUnit su = sum.statisticUnitMap[cdt];
                    Dictionary<int, int> lst = null;
                    if (missCountInfos.ContainsKey(cdt))
                        lst = missCountInfos[cdt];
                    else
                    {
                        lst = new Dictionary<int, int>();
                        missCountInfos[cdt] = lst;
                    }

                    if(lst.ContainsKey(su.missCount))
                    {
                        lst[su.missCount] = lst[su.missCount] + 1;
                    }
                    else
                    {
                        lst[su.missCount] = 1;
                    }
                }
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
            missCountInfos.Clear();
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
        public bool IsSimulating
        {
            get { return state != SimState.eNone; }
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
