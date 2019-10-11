using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class BollinDataContainer
    {
        public static bool ENABLE = true;
        public List<BollinPointMap> bollinMapLst = new List<BollinPointMap>();

        BollinPointMap CreateBollinPointMap(KDataMap kdd)
        {
            BollinPointMap bpm = new BollinPointMap();
            bpm.index = bollinMapLst.Count;
            bpm.parent = this;
            bpm.kdd = kdd;
            bollinMapLst.Add(bpm);
            return bpm;
        }

        public void Process(List<KDataMap> srcData, AvgDataContainer avgContainer)
        {
            const float scale = 2;
            bollinMapLst.Clear();
            if (ENABLE == false)
            {
                return;
            }
            for (int i = 0; i < srcData.Count; ++i)
            {
                KDataMap kdd = srcData[i];
                AvgPointMap apm = avgContainer.avgPointMapLst[i];

                BollinPointMap bpm = CreateBollinPointMap(srcData[i]);
                BollinPointMap prevBPM = null;
                if (bpm.index > 0)
                {
                    prevBPM = bpm.GetPrevBPM();
                }
                for (int t = 0; t < GraphDataManager.S_CDT_LIST.Count; ++t)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[t];
                    AvgPoint ap = apm.apMap[cdt];
                    KData kd = kdd.dataDict[cdt];
                    float missHeight = GraphDataManager.GetMissRelLength(cdt);

                    float MA = ap.avgKValue;
                    int startIndex = i - avgContainer.cycle + 1;
                    if (startIndex < 0)
                        startIndex = 0;
                    float SD = 0;
                    int N = 0;
                    for (int k = startIndex; k <= i; ++k)
                    {
                        KData ckd = srcData[k].dataDict[cdt];
                        SD += (ckd.KValue - MA) * (ckd.KValue - MA);
                        ++N;
                    }
                    SD = (float)Math.Sqrt(SD / N);
                    BollinPoint bp = bpm.GetData(cdt, true);
                    bp.midValue = MA;
                    bp.upValue = MA + scale * SD;
                    bp.downValue = MA - scale * SD;

#if RECORD_BOLLEAN_MID_COUNTS
                    if (GlobalSetting.G_CALC_BOOLEAN_ANALYSE_DATA)
                    {
                        CalcBolleanMidCount(bp, bpm, prevBPM, cdt, missHeight, kd);
                        CalcBolleanMidDirectionCount(bp, bpm, prevBPM, cdt, missHeight, kd);
                        CalcBolleanDownCount(bp, bpm, prevBPM, cdt, missHeight, kd);
                    }
#endif
                }
            }
        }

#if RECORD_BOLLEAN_MID_COUNTS
        void CalcBolleanMidCount(BollinPoint bp, BollinPointMap bpm, BollinPointMap prevBPM, CollectDataType cdt, float missHeight, KData kd)
        {
            const float TOR = 0.5f;
            if (bpm.index == 0)
            {
                bp.underBolleanMidCount = bp.uponBolleanMidCount = bp.underBolleanMidCountContinue = bp.uponBolleanMidCountContinue = 0;
                bp.onBolleanMidCount = bp.onBolleanMidCountContinue = 1;
                bp.distFromBolleanMidToKD = 0.0f;
            }
            else
            {
                BollinPoint pBP = prevBPM.GetData(cdt, false);
                string tt = (kd.RelateDistTo(bp.midValue) / missHeight).ToString("f1");
                bp.distFromBolleanMidToKD = float.Parse(tt);
                // 当前K值在布林中轨之上
                if (bp.distFromBolleanMidToKD < -TOR)
                {
                    bp.underBolleanMidCount = 0;
                    // 如果之前在布林中轨下方的个数大于0，意味着出现反转信号，
                    // 那么需要重置在中轨及以上的个数
                    if (pBP.underBolleanMidCount > 0)
                    {
                        bp.onBolleanMidCount = pBP.onBolleanMidCountContinue;
                        bp.uponBolleanMidCount = 1;
                    }
                    // 中轨之上的个数增加
                    else
                    {
                        bp.onBolleanMidCount = pBP.onBolleanMidCount;
                        bp.uponBolleanMidCount = pBP.uponBolleanMidCount + 1;
                    }

                    if (pBP.uponBolleanMidCountContinue > 0)
                    {
                        bp.uponBolleanMidCountContinue = pBP.uponBolleanMidCountContinue + 1;
                    }
                    else
                    {
                        bp.uponBolleanMidCountContinue = 1;
                    }
                    bp.onBolleanMidCountContinue = 0;
                    bp.underBolleanMidCountContinue = 0;
                }
                // 当前K值在布林中轨之下
                else if (bp.distFromBolleanMidToKD > TOR)
                {
                    bp.uponBolleanMidCount = 0;
                    // 如果之前在布林中轨上方的个数大于0，意味着出现反转信号，
                    // 那么需要重置在中轨及以下的个数
                    if (pBP.uponBolleanMidCount > 0)
                    {
                        bp.onBolleanMidCount = pBP.onBolleanMidCountContinue;
                        bp.underBolleanMidCount = 1;
                    }
                    // 中轨之下的个数增加
                    else
                    {
                        bp.onBolleanMidCount = pBP.onBolleanMidCount;
                        bp.underBolleanMidCount = pBP.underBolleanMidCount + 1;
                    }

                    if (pBP.underBolleanMidCountContinue > 0)
                    {
                        bp.underBolleanMidCountContinue = pBP.underBolleanMidCountContinue + 1;
                    }
                    else
                    {
                        bp.underBolleanMidCountContinue = 1;
                    }
                    bp.onBolleanMidCountContinue = 0;
                    bp.uponBolleanMidCountContinue = 0;
                }
                // 当前K值在布林中轨
                else
                {
                    bp.onBolleanMidCount = pBP.onBolleanMidCount + 1;
                    bp.underBolleanMidCount = pBP.underBolleanMidCount;
                    bp.uponBolleanMidCount = pBP.uponBolleanMidCount;

                    if (pBP.onBolleanMidCountContinue > 0)
                    {
                        bp.onBolleanMidCountContinue = pBP.onBolleanMidCountContinue + 1;
                    }
                    else
                    {
                        bp.onBolleanMidCountContinue = 1;
                    }
                    bp.underBolleanMidCountContinue = 0;
                    bp.uponBolleanMidCountContinue = 0;
                }
            }
        }
        void CalcBolleanMidDirectionCount(BollinPoint bp, BollinPointMap bpm, BollinPointMap prevBPM, CollectDataType cdt, float missHeight, KData kd)
        {
            const float TOR = 0.00001f;
            if (bpm.index == 0)
            {
                bp.bolleanMidKeepDownCount = bp.bolleanMidKeepUpCount = bp.bolleanMidKeepDownCountContinue = bp.bolleanMidKeepUpCountContinue = 0;
                bp.bolleanMidKeepHorzCount = bp.bolleanMidKeepHorzCountContinue = 1;
            }
            else
            {
                BollinPoint pBP = prevBPM.GetData(cdt, false);
                float delta = bp.midValue - pBP.midValue;
                // turn up
                if (delta > TOR)
                {
                    bp.bolleanMidKeepDownCount = 0;
                    if (pBP.bolleanMidKeepDownCount > 0)
                    {
                        bp.bolleanMidKeepHorzCount = pBP.bolleanMidKeepHorzCountContinue;
                        bp.bolleanMidKeepUpCount = 1;
                    }
                    else
                    {
                        bp.bolleanMidKeepHorzCount = pBP.bolleanMidKeepHorzCount;
                        bp.bolleanMidKeepUpCount = pBP.bolleanMidKeepUpCount + 1;
                    }

                    if (pBP.bolleanMidKeepUpCountContinue > 0)
                    {
                        bp.bolleanMidKeepUpCountContinue = pBP.bolleanMidKeepUpCountContinue + 1;
                    }
                    else
                    {
                        bp.bolleanMidKeepUpCountContinue = 1;
                    }
                    bp.bolleanMidKeepHorzCountContinue = 0;
                    bp.bolleanMidKeepDownCountContinue = 0;
                }
                // turn down
                else if (delta < -TOR)
                {
                    bp.bolleanMidKeepUpCount = 0;
                    if (pBP.bolleanMidKeepUpCount > 0)
                    {
                        bp.bolleanMidKeepHorzCount = pBP.bolleanMidKeepHorzCountContinue;
                        bp.bolleanMidKeepDownCount = 1;
                    }
                    // 中轨之下的个数增加
                    else
                    {
                        bp.bolleanMidKeepHorzCount = pBP.bolleanMidKeepHorzCount;
                        bp.bolleanMidKeepDownCount = pBP.bolleanMidKeepDownCount + 1;
                    }

                    if (pBP.bolleanMidKeepDownCountContinue > 0)
                    {
                        bp.bolleanMidKeepDownCountContinue = pBP.bolleanMidKeepDownCountContinue + 1;
                    }
                    else
                    {
                        bp.bolleanMidKeepDownCountContinue = 1;
                    }
                    bp.bolleanMidKeepHorzCountContinue = 0;
                    bp.bolleanMidKeepUpCountContinue = 0;
                }
                else
                {
                    bp.bolleanMidKeepHorzCount = pBP.bolleanMidKeepHorzCount + 1;
                    bp.bolleanMidKeepDownCount = pBP.bolleanMidKeepDownCount;
                    bp.bolleanMidKeepUpCount = pBP.bolleanMidKeepUpCount;

                    if (pBP.bolleanMidKeepHorzCountContinue > 0)
                    {
                        bp.bolleanMidKeepHorzCountContinue = pBP.bolleanMidKeepHorzCountContinue + 1;
                    }
                    else
                    {
                        bp.bolleanMidKeepHorzCountContinue = 1;
                    }
                    bp.bolleanMidKeepDownCountContinue = 0;
                    bp.bolleanMidKeepUpCountContinue = 0;
                }
            }
        }
        void CalcBolleanDownCount(BollinPoint bp, BollinPointMap bpm, BollinPointMap prevBPM, CollectDataType cdt, float missHeight, KData kd)
        {
            const float TOR = 0.5f;
            string tt = (kd.RelateDistTo(bp.downValue) / missHeight).ToString("f1");
            float distFromBolleanDownToKD = float.Parse(tt);

            if (kd.MissValue == 0)
            {
                bp.onBolleanDownCountContinue = 0;
            }
            else if (distFromBolleanDownToKD >= -TOR)
            {
                if (prevBPM == null)
                    bp.onBolleanDownCountContinue = 1;
                else
                {
                    int preC = prevBPM.bpMap[cdt].onBolleanDownCountContinue;
                    bp.onBolleanDownCountContinue = preC + 1;
                }
            }
            else
            {
                //if (kd.MissValue > 0)
                {
                    if (prevBPM != null)
                    {
                        BollinPoint preBP = prevBPM.GetData(cdt, false);
                        KData preKD = kd.GetPrevKData();
                        if (preKD.HitValue > 0)
                            bp.onBolleanDownCountContinue = 0;
                        else
                        {
                            if (preBP.onBolleanDownCountContinue > 0)
                                bp.onBolleanDownCountContinue = preBP.onBolleanDownCountContinue + 1;
                            else
                                bp.onBolleanDownCountContinue = 0;
                        }
                    }
                    else
                        bp.onBolleanDownCountContinue = 0;
                }
                //else
                //    bp.onBolleanDownCountContinue = 0;
            }
        }
#endif
    }
}
