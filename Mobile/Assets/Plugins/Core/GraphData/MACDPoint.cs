#define TRADE_DBG

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class MACDLimitValue
    {
        public float MaxValue = 0;
        public float MinValue = 0;
    }
    public class MACDPoint
    {
        public MACDPointMap parent;
        // 快线
        public float DIF = 0;
        // 慢线
        public float DEA = 0;
        // 动能柱
        public float BAR = 0;
#if TRADE_DBG
        public byte WAVE_CFG = 0;
        public int MAX_DIF_INDEX = -1;
        public int MIN_DIF_INDEX = -1;
        public int LEFT_DIF_INDEX = -1;
        public byte BAR_CFG = 0;
        public int MAX_BAR_INDEX = -1;
        public int MIN_BAR_INDEX = -1;
        public byte KGRAPH_CFG = 0;
        public bool IS_STRONG_UP = false;
        public string LAST_DATA_TAG = "";
#endif
    }
}
