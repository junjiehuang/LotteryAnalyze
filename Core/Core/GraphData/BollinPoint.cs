//#define TRADE_DBG
//#define RECORD_BOLLEAN_MID_COUNTS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class BollinPoint
    {
        public BollinPointMap parent = null;
        //// 标准差
        //public float standardDeviation = 0;
        // 上轨值
        public float upValue = 0;
        // 中轨值
        public float midValue = 0;
        // 下轨值
        public float downValue = 0;

#if RECORD_BOLLEAN_MID_COUNTS
        // K值开在布林中轨之下的个数
        public int underBolleanMidCount = 0;
        // K值开在布林中轨之上的个数
        public int uponBolleanMidCount = 0;
        // K值开在布林中轨的个数
        public int onBolleanMidCount = 0;

        // K值连续开在布林中轨之下的个数
        public int underBolleanMidCountContinue = 0;
        // K值连续开在布林中轨的个数
        public int onBolleanMidCountContinue = 0;
        // K值连续开在布林中轨之上的个数
        public int uponBolleanMidCountContinue = 0;
        // 布林中轨到对应K值的距离单位
        public float distFromBolleanMidToKD = 0;

        // K值连续开在布林下轨的个数
        public int onBolleanDownCountContinue = 0;

        // 布林中轨保持向上的个数
        public int bolleanMidKeepUpCount = 0;
        // 布林中轨保持水平的个数
        public int bolleanMidKeepHorzCount = 0;
        // 布林中轨保持下降的个数
        public int bolleanMidKeepDownCount = 0;

        // 布林中轨持续向上的个数
        public int bolleanMidKeepUpCountContinue = 0;
        // 布林中轨持续走平的个数
        public int bolleanMidKeepHorzCountContinue = 0;
        // 布林中轨持续向下的个数
        public int bolleanMidKeepDownCountContinue = 0;
#endif

        public int Index
        {
            get
            {
                return parent.index;
            }
        }
    }
}
