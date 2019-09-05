using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    // 一星交易数据
    public class TradeDataOneStar : TradeDataBase
    {
        static Type IT = typeof(int);
        static Type FT = typeof(float);
        static Type BT = typeof(bool);

        public static float SingleTradeCost
        {
            get { return GlobalSetting.G_ONE_STARE_TRADE_COST; }
        }
        public static float SingleTradeReward
        {
            get { return GlobalSetting.G_ONE_STARE_TRADE_REWARD; }
        }

        public Dictionary<int, TradeNumbers> tradeInfo = new Dictionary<int, TradeNumbers>();

#if TRADE_DBG
        public List<List<PathCmpInfo>> pathCmpInfos = new List<List<PathCmpInfo>>();
        public int FindIndex(int numindex, int pathIndex)
        {
            if (numindex < 0 || numindex >= pathCmpInfos.Count)
                return -1;
            if (pathIndex < 0 || pathIndex >= pathCmpInfos[numindex].Count)
                return -1;
            for (int i = 0; i < pathCmpInfos[numindex].Count; ++i)
            {
                if (pathCmpInfos[numindex][i].pathIndex == pathIndex)
                    return i;
            }
            return -1;
        }
        public PathCmpInfo FindInfoByPathIndex(int numindex, int pathIndex)
        {
            if (numindex < 0 || numindex >= pathCmpInfos.Count)
                return null;
            if (pathIndex < 0 || pathIndex >= pathCmpInfos[numindex].Count)
                return null;
            for (int i = 0; i < pathCmpInfos[numindex].Count; ++i)
            {
                if (pathCmpInfos[numindex][i].pathIndex == pathIndex)
                    return pathCmpInfos[numindex][i];
            }
            return null;
        }
#endif

        public override string GetTradeXML()
        {
            string info = "";
            info += "\t\t\t\t<TradeData reward=\"" + reward + "\" cost=\"" + cost + "\" moneyBeforeTrade=\"" + moneyBeforeTrade +
                "\" moneyAtferTrade=\"" + moneyAtferTrade + "\">\n";
            info += "\t\t\t\t\t<TradeNumsInfo>\n";
            var etor = tradeInfo.GetEnumerator();
            while (etor.MoveNext())
            {
                TradeNumbers tn = etor.Current.Value;
                info += "\t\t\t\t\t\t<TradeNums numIndex=\"" + etor.Current.Key + "\" tradeCount=\"" + tn.tradeCount + "\">\n";
                info += "\t\t\t\t\t\t\t<TradeNumInfos>\n";
                for (int i = 0; i < tn.tradeNumbers.Count; ++i)
                {
                    NumberCmpInfo nci = tn.tradeNumbers[i];
                    info += "\t\t\t\t\t\t\t\t<TradeNum number=\"" + nci.number +
                        "\" rate=\"" + nci.rate +
                        "\" appearCount=\"" + nci.appearCount + "\"/>\n";
                }
                info += "\t\t\t\t\t\t\t</TradeNumInfos>\n";
                info += "\t\t\t\t\t\t</TradeNums>\n";
            }
            info += "\t\t\t\t\t</TradeNumsInfo>\n";
            info += "\t\t\t\t</TradeData>\n";
            return info;
        }

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
                    dbgtxt += "[" + pci.pathIndex + "] [" + pci.pathValue + "] ";
                    var etor = pci.paramMap.GetEnumerator();
                    int count = 0;
                    while (etor.MoveNext())
                    {
                        dbgtxt += etor.Current.Key + "=" + etor.Current.Value + ", ";
                        ++count;
                        if (count % 5 == 0)
                        {
                            dbgtxt += "\n\t";
                        }
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
            if (tradeInfo.ContainsKey(numIndex) == false)
            {
                TradeNumbers tn = new TradeNumbers();
                tradeInfo.Add(numIndex, tn);
                tn.tradeCount = tradeCount;

                for (int i = 0; i < selNums.Count; ++i)
                {
                    int index = NumberCmpInfo.FindIndex(nums, selNums[i], false);
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
            if (tradeStatus == TradeStatus.eWaiting)
            {
                if (lastDateItem != null)
                {
                    targetLotteryItem = lastDateItem.parent.GetNextItem(lastDateItem);
                    if (targetLotteryItem != null)
                    {
                        reward = 0;
                        cost = 0;
                        foreach (int numIndex in tradeInfo.Keys)
                        {
                            TradeNumbers tns = tradeInfo[numIndex];
                            SByte dstValue = targetLotteryItem.GetNumberByIndex(numIndex);
                            if (tns.ContainsNumber(dstValue))
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
            if (tips.Length == 0 && targetLotteryItem != null)
            {
                tips += targetLotteryItem.idGlobal + " [期号：" + targetLotteryItem.idTag + "] [号码：" + targetLotteryItem.lotteryNumber + "]\n";
                foreach (int key in tradeInfo.Keys)
                {
                    TradeNumbers tn = tradeInfo[key];
                    if (tn.tradeNumbers.Count > 0)
                    {
                        tips += TradeDataBase.NUM_TAGS[key];
                        tn.GetInfo(ref tips);
                    }
                }
                tips += "[成本：" + cost + "] [奖金：" + reward + "] [剩余：" + moneyAtferTrade + "]";
            }
            // 等待开奖
            else if (targetLotteryItem == null)
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
            foreach (int key in tradeInfo.Keys)
            {
                if (tradeInfo[key].tradeCount > 0 && tradeInfo[key].tradeNumbers.Count > 0)
                {
                    numIndex = key;
                    pathIndex = tradeInfo[key].tradeNumbers[0].number % 3;
                    break;
                }
            }
        }
    }
}
