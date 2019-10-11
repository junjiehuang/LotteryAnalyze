using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
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
            foreach (TradeDataManager.KGraphConfig key in kGraphCfgBPs.Keys)
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
}
