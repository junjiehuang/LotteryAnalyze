using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class AutoAnalyzeTool
    {

        public enum CalcType
        {
            eUpLine,
            eDownLine,
        }

        public class AuxLineData
        {
            public bool valid = false;
            public KData dataSharp = null;
            public KData dataPrevSharp = null;
            public KData dataNextSharp = null;

            public List<KData> candictDatas = new List<KData>();

            public void Reset()
            {
                valid = false;
                dataSharp = null;
                dataPrevSharp = null;
                dataNextSharp = null;
                candictDatas.Clear();
            }
            public void CheckValid()
            {
                if (dataSharp != null && (dataPrevSharp != null || dataNextSharp != null))
                    valid = true;
                else
                    valid = false;
            }
            public void CalcSecondPoint(bool bGetSmallSecPt)
            {
                if (dataSharp == null || candictDatas.Count == 0)
                    return;
                float prevKV = 0, nextKV = 0;

                if (bGetSmallSecPt)
                {
                    for (int i = 0; i < candictDatas.Count; ++i)
                    {
                        KData testD = candictDatas[i];
                        if (testD == dataSharp || testD == null)
                            continue;
                        float k = (dataSharp.KValue - testD.KValue) / (dataSharp.index - testD.index);
                        if (testD.index < dataSharp.index)
                        {
                            if (k < prevKV || dataPrevSharp == null)
                            {
                                prevKV = k;
                                dataPrevSharp = testD;
                            }
                        }
                        else
                        {
                            if (k > nextKV || dataNextSharp == null)
                            {
                                nextKV = k;
                                dataNextSharp = testD;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < candictDatas.Count; ++i)
                    {
                        KData testD = candictDatas[i];
                        if (testD == dataSharp || testD == null)
                            continue;
                        float k = (dataSharp.KValue - testD.KValue) / (dataSharp.index - testD.index);
                        if (testD.index < dataSharp.index)
                        {
                            if (k > prevKV || dataPrevSharp == null)
                            {
                                prevKV = k;
                                dataPrevSharp = testD;
                            }
                        }
                        else
                        {
                            if (k < nextKV || dataNextSharp == null)
                            {
                                nextKV = k;
                                dataNextSharp = testD;
                            }
                        }
                    }
                }
            }


            public void GetKValue(int x, float y, float k,
                out bool hasPrev, out float prevKV, out float prevSlope,
                out bool hasPrevHitPt, out float prevHitPtX, out float prevHitPtY,
                out bool hasNext, out float nextKV, out float nextSlope,
                out bool hasNextHitPt, out float nextHitPtX, out float nextHitPtY)
            {
                hasPrev = hasPrevHitPt = (dataPrevSharp != null && dataSharp != null);
                hasNext = hasNextHitPt = (dataNextSharp != null && dataSharp != null);
                prevKV = nextKV = prevHitPtX = nextHitPtX = prevHitPtY = nextHitPtY = 0;
                prevSlope = nextSlope = 0;
                float b = y - k * x;
                if (hasPrev)
                {
                    float x2 = dataPrevSharp.index;
                    float x1 = dataSharp.index;
                    float y2 = dataPrevSharp.KValue;
                    float y1 = dataSharp.KValue;
                    float K = (y2 - y1) / (x2 - x1);
                    float B = (y1 * x2 - y2 * x1) / (x2 - x1);
                    prevSlope = K;
                    prevKV = (x * prevSlope) + ((y1 * x2 - y2 * x1) / (x2 - x1));

                    if (k != K)
                    {
                        hasPrevHitPt = true;
                        prevHitPtX = (b - B) / (K - k);
                        prevHitPtY = k * prevHitPtX + b;
                    }
                    else
                    {
                        hasPrevHitPt = false;
                    }
                }
                if (hasNext)
                {
                    float x2 = dataNextSharp.index;
                    float x1 = dataSharp.index;
                    float y2 = dataNextSharp.KValue;
                    float y1 = dataSharp.KValue;
                    float K = (y2 - y1) / (x2 - x1);
                    float B = (y1 * x2 - y2 * x1) / (x2 - x1);
                    nextSlope = K;
                    nextKV = (x * nextSlope) + ((y1 * x2 - y2 * x1) / (x2 - x1));

                    if (k != K)
                    {
                        hasNextHitPt = true;
                        nextHitPtX = (b - B) / (K - k);
                        nextHitPtY = k * nextHitPtX + b;
                    }
                    else
                    {
                        hasNextHitPt = false;
                    }
                }
            }
        }

        public class SingleAuxLineInfo
        {
            public AuxLineData upLineData = new AuxLineData();
            public AuxLineData downLineData = new AuxLineData();

            public void Reset()
            {
                upLineData.Reset();
                downLineData.Reset();
            }

            public void CheckData(KData kd, int endKDIndex, int numID)
            {
                if (kd.index == endKDIndex)
                    return;
                // 取得下一个K值
                KData nxtData = kd.GetNextKData();
                //KData prvData = kd.GetPrevKData();

                // 当前K值上升
                if (kd.HitValue > kd.MissValue)
                {
                    // 下一个K值是下降，表明当前K值可能是一个波峰
                    if (nxtData != null && nxtData.HitValue < nxtData.MissValue)
                    {
                        // 当前没有波峰
                        if (upLineData.dataSharp == null)
                        {
                            // 记录该K值为波峰
                            upLineData.dataSharp = kd;
                        }
                        // 如果当前K值高于波峰的K值
                        else if (kd.KValue > upLineData.dataSharp.KValue)
                        {
                            // 把当前的波峰K值放到候选列表中
                            upLineData.candictDatas.Add(upLineData.dataSharp);
                            // 设置当前K值为波峰
                            upLineData.dataSharp = kd;
                        }
                        // 把当前的K值放到候选列表中
                        else
                            upLineData.candictDatas.Add(kd);
                    }
                }
                // 当前K值下降
                else if (kd.HitValue < kd.MissValue)
                {
                    // 下一个K值上升，表明当前K值可能是一个波谷
                    if (nxtData != null && nxtData.HitValue > nxtData.MissValue)
                    {
                        // 当前没有波谷
                        if (downLineData.dataSharp == null)
                        {
                            // 记录该K值为波谷
                            downLineData.dataSharp = kd;
                        }
                        // 如果当前K值低于波谷的K值
                        else if (kd.KValue < downLineData.dataSharp.KValue)
                        {
                            // 把当前的波谷K值放到候选列表中
                            downLineData.candictDatas.Add(downLineData.dataSharp);
                            // 设置当前K值为波谷
                            downLineData.dataSharp = kd;
                        }
                        // 把当前的K值放到候选列表中
                        else
                            downLineData.candictDatas.Add(kd);
                    }
                }
            }

            public void CheckDataFast(KDataDictContainer kddc, CollectDataType cdt, int numIndex, int maxKDataID, ref int curKDataID, ref int loopCount, ref int prevMissCount)
            {
                if (maxKDataID == curKDataID)
                {
                    --loopCount;
                    --curKDataID;
                    prevMissCount = 0;
                    return;
                }
                KDataMap kddC = kddc.dataLst[curKDataID];
                KData kdCurrent = kddC.GetData(cdt, false);
                KData kdNext = kdCurrent.GetNextKData();
                int missCount = kdCurrent.parent.startItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt].missCount;

                // 当前K值上升
                if (kdCurrent.HitValue > kdCurrent.MissValue)
                {
                    // 下一个K值是下降，表明当前K值可能是一个波峰
                    if (kdNext != null && kdNext.HitValue < kdNext.MissValue && prevMissCount > 1)
                    {
                        // 当前没有波峰
                        if (upLineData.dataSharp == null)
                        {
                            // 记录该K值为波峰
                            upLineData.dataSharp = kdCurrent;
                        }
                        // 如果当前K值高于波峰的K值
                        else if (kdCurrent.KValue > upLineData.dataSharp.KValue)
                        {
                            // 把当前的波峰K值放到候选列表中
                            upLineData.candictDatas.Add(upLineData.dataSharp);
                            // 设置当前K值为波峰
                            upLineData.dataSharp = kdCurrent;
                        }
                        // 把当前的K值放到候选列表中
                        else
                            upLineData.candictDatas.Add(kdCurrent);
                    }
                }
                // 当前K值下降
                else if (kdCurrent.HitValue < kdCurrent.MissValue)
                {
                    // 下一个K值上升，表明当前K值可能是一个波谷
                    if (kdNext != null && kdNext.HitValue > kdNext.MissValue && missCount > 1)
                    {
                        // 当前没有波谷
                        if (downLineData.dataSharp == null)
                        {
                            // 记录该K值为波谷
                            downLineData.dataSharp = kdCurrent;
                        }
                        // 如果当前K值低于波谷的K值
                        else if (kdCurrent.KValue < downLineData.dataSharp.KValue)
                        {
                            // 把当前的波谷K值放到候选列表中
                            downLineData.candictDatas.Add(downLineData.dataSharp);
                            // 设置当前K值为波谷
                            downLineData.dataSharp = kdCurrent;
                        }
                        // 把当前的K值放到候选列表中
                        else
                            downLineData.candictDatas.Add(kdCurrent);
                    }
                }
                if (missCount > 0)
                {
                    curKDataID -= missCount;
                    loopCount -= missCount;
                }
                else
                {
                    --loopCount;
                    --curKDataID;
                }
                prevMissCount = missCount;
            }

            public void CheckValid()
            {
                upLineData.CalcSecondPoint(true);
                downLineData.CalcSecondPoint(false);
                upLineData.CheckValid();
                downLineData.CheckValid();
            }
        }

        List<Dictionary<CollectDataType, SingleAuxLineInfo>> allAuxInfo = new List<Dictionary<CollectDataType, SingleAuxLineInfo>>();

        public SingleAuxLineInfo GetSingleAuxLineInfo(int numID, CollectDataType cdt)
        {
            return allAuxInfo[numID][cdt];
        }

        public AutoAnalyzeTool()
        {
            allAuxInfo.Clear();
            for (int i = 0; i < 5; ++i)
            {
                Dictionary<CollectDataType, SingleAuxLineInfo> dict = new Dictionary<CollectDataType, SingleAuxLineInfo>();
                for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    dict.Add(cdt, new SingleAuxLineInfo());
                }
                allAuxInfo.Add(dict);
            }
        }

        void Reset()
        {
            for (int i = 0; i < allAuxInfo.Count; ++i)
            {
                Dictionary<CollectDataType, SingleAuxLineInfo> dict = allAuxInfo[i];
                foreach (SingleAuxLineInfo sali in dict.Values)
                {
                    sali.Reset();
                }
            }
        }

        void FinalCheckValid()
        {
            for (int i = 0; i < allAuxInfo.Count; ++i)
            {
                Dictionary<CollectDataType, SingleAuxLineInfo> dict = allAuxInfo[i];
                foreach (SingleAuxLineInfo sali in dict.Values)
                {
                    sali.CheckValid();
                }
            }
        }

        void ProcessCheck(int curKDataIndex, int loopCount)
        {
            // 遍历每个数字位
            for (int numID = 0; numID < 5; ++numID)
            {
                int loop = loopCount;
                int kdID = curKDataIndex;
                // 往前回溯loopCount个K值
                while (kdID >= 0 && loop >= 0)
                {
                    // 取K值映射表
                    KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numID);
                    KDataMap kdd = kddc.dataLst[kdID];
                    // 遍历每类数据的K值
                    foreach (CollectDataType cdt in kdd.dataDict.Keys)
                    {
                        KData kd = kdd.GetData(cdt, false);
                        SingleAuxLineInfo sali = GetSingleAuxLineInfo(numID, cdt);
                        sali.CheckData(kd, curKDataIndex, numID);
                    }

                    --kdID;
                    --loop;
                }
            }
        }

        void ProcessCheckFast(int curKDataIndex, int loopCount)
        {
            // 遍历每个数字位
            for (int numID = 0; numID < 5; ++numID)
            {
                // 取K值映射表
                KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numID);
                if (kddc.dataLst.Count <= curKDataIndex)
                    continue;

                for (int cdtID = 0; cdtID < GraphDataManager.S_CDT_LIST.Count; ++cdtID)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[cdtID];
                    int loop = loopCount;
                    int kdID = curKDataIndex;
                    int prevMissCount = 0;
                    SingleAuxLineInfo sali = GetSingleAuxLineInfo(numID, cdt);

                    while (loop >= 0 && kdID >= 0)
                    {
                        sali.CheckDataFast(kddc, cdt, numID, curKDataIndex, ref kdID, ref loop, ref prevMissCount);
                    }
                }
            }
        }

        public void Analyze(int curKDataIndex)
        {
            if (curKDataIndex < 2)
                return;
            Reset();
            ProcessCheckFast(curKDataIndex, GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT);
            FinalCheckValid();
        }

        // 计算第dataIndex个数据在压力线或者支撑线上的K值
        public bool CalculateValue(int numIndex, int dataIndex, CollectDataType cdt, CalcType ct, bool prevLine, ref float value)
        {
            SingleAuxLineInfo sali = allAuxInfo[numIndex][cdt];
            if (ct == CalcType.eUpLine)
            {
                if (sali.upLineData.valid == false)
                    return false;
                float x1 = sali.upLineData.dataSharp.index, y1 = sali.upLineData.dataSharp.KValue, x2, y2;
                if (prevLine)
                {
                    if (sali.upLineData.dataPrevSharp == null)
                        return false;
                    x2 = sali.upLineData.dataPrevSharp.index;
                    y2 = sali.upLineData.dataPrevSharp.KValue;
                }
                else
                {
                    if (sali.upLineData.dataNextSharp == null)
                        return false;
                    x2 = sali.upLineData.dataNextSharp.index;
                    y2 = sali.upLineData.dataNextSharp.KValue;
                }
                float k = (y1 - y2) / (x1 - x2);
                float b = (x1 * y2 - y1 * x2) / (x1 - x2);
                value = k * dataIndex + b;
                return true;
            }
            else if (ct == CalcType.eDownLine)
            {
                if (sali.downLineData.valid == false)
                    return false;
                float x1 = sali.downLineData.dataSharp.index, y1 = sali.downLineData.dataSharp.KValue, x2, y2;
                if (prevLine)
                {
                    if (sali.downLineData.dataPrevSharp == null)
                        return false;
                    x2 = sali.downLineData.dataPrevSharp.index;
                    y2 = sali.downLineData.dataPrevSharp.KValue;
                }
                else
                {
                    if (sali.downLineData.dataNextSharp == null)
                        return false;
                    x2 = sali.downLineData.dataNextSharp.index;
                    y2 = sali.downLineData.dataNextSharp.KValue;
                }
                float k = (y1 - y2) / (x1 - x2);
                float b = (x1 * y2 - y1 * x2) / (x1 - x2);
                value = k * dataIndex + b;
                return true;
            }
            return false;
        }
    }
}
