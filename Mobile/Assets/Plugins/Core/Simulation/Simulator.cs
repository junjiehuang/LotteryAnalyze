using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    #region simulation

    public enum SimState
    {
        eNotStart = 0,
        eSimulating,
        eFinished,
    }

    public class WrongInfo
    {
        public long costTotal;
        public string startTag;
        public int round;

        public WrongInfo()
        {
            costTotal = 0;
            round = 0;
            startTag = "";
        }
        public void CopyFrom(WrongInfo other)
        {
            costTotal = other.costTotal;
            startTag = other.startTag;
            round = other.round;
        }
    }

    public enum KillType
    {
        // 只匹配组三
        eKTGroup3 = 0,
        // 只匹配组六
        eKTGroup6,
        // 交叉匹配
        eKTBlend,
        // 根据组三形态匹配组六
        eKTGroup6OnGroup3,
        // 不做
        eKTNone,
    }

    public enum SimType
    {
        eGroup3,
        eGroup2,
    }

    public abstract class SimulationBase
    {
        public virtual void SortWrongInfos(bool byRound) { }
        public virtual void StepRatio() { }
        public virtual void ResetRatio() { }
        public virtual void StartSimulate() { }
        public virtual void UpdateSimulate() { }
    }

#if ENABLE_GROUP_COLLECT
    public class SimulationGroup3 : SimulationBase
    {
        static SimState curState = SimState.eNotStart;
        static int curSimIndex = -1;
        static int curItemIndex = -1;
        static WrongInfo curCal = null;
        public static bool isCurKillGroup3 = false;
        public static List<WrongInfo> allWrongInfos = new List<WrongInfo>();
        public static KillType killType = KillType.eKTGroup3;
        public static int g3Round = 0;
        public static int g6Round = 0;
        public static KillType curKillType = KillType.eKTGroup6;

        const float G1SCORE = 1000.0f / 10.0f;
        const float G3SCORE = 1000.0f / 270.0f;
        const float G6SCORE = 1000.0f / 720.0f;

        static int curRatio = 1;
        public static bool enableDoubleRatioIfFailed = true;
        public static int firmRatio = 10;
        public static int maxRatio = 32;

        public override void SortWrongInfos(bool byRound)
        {
            if (byRound)
            {
                allWrongInfos.Sort((x, y) =>
                {
                    if (x.round > y.round)
                        return -1;
                    return 1;
                });
            }
            else
            {
                allWrongInfos.Sort((x, y) =>
                {
                    if (x.costTotal > y.costTotal)
                        return -1;
                    return 1;
                });
            }
        }

        public override void StepRatio()
        {
            if (enableDoubleRatioIfFailed)
            {
                curRatio *= 2;
                if (maxRatio > 0 && curRatio > maxRatio)
                    curRatio = maxRatio;
            }
            else
            {
                curRatio = firmRatio;
            }
        }
        public override void ResetRatio()
        {
            curRatio = 1;
            if (!enableDoubleRatioIfFailed)
                curRatio = firmRatio;
        }
        public override void StartSimulate()
        {
            DataManager mgr = DataManager.GetInst();
            mgr.simData.Reset();
            for (int i = 0; i < mgr.indexs.Count; ++i)
            {
                int curFileID = mgr.indexs[i];
                OneDayDatas odd = mgr.allDatas[curFileID];
                if (odd != null)
                {
                    odd.simData.Reset();
                    for (int j = 0; j < odd.datas.Count; ++j)
                    {
                        odd.datas[j].simData.Reset();
                    }
                }
            }
            curItemIndex = 0;
            curSimIndex = 0;
            mgr.curProfit = 0;
            ResetRatio();
            Program.mainForm.ResetResult();
            curState = SimState.eSimulating;
            allWrongInfos.Clear();
            g3Round = 0;
            g6Round = 0;
            killType = Program.mainForm.GetCurSelectedKillType();
            if (killType == KillType.eKTBlend)
                curKillType = KillType.eKTGroup6;
            else
                curKillType = KillType.eKTNone;
        }
        public override void UpdateSimulate()
        {
            DataManager mgr = DataManager.GetInst();
            if (curState == SimState.eSimulating)
            {
                if (curSimIndex < mgr.indexs.Count)
                {
                    int curFileID = mgr.indexs[curSimIndex];
                    OneDayDatas odd = mgr.allDatas[curFileID];
                    if (odd != null)
                    {
                        for (int i = 0; i < odd.datas.Count; ++i)
                        {
                            DataItem item = odd.datas[i];
                            item.simData.g1Score = mgr.simData.g1Score;
                            item.simData.g3Score = mgr.simData.g3Score;
                            item.simData.g6Score = mgr.simData.g6Score;
                            switch (item.groupType)
                            {
                                case GroupType.eGT1: mgr.simData.g1Score += G1SCORE; break;
                                case GroupType.eGT3: mgr.simData.g3Score += G3SCORE; break;
                                case GroupType.eGT6: mgr.simData.g6Score += G6SCORE; break;
                            }

                            TestResultType curResult = TestResultType.eTRTIgnore;
                            if (killType == KillType.eKTGroup3)
                                curResult = Util.SimButG3OnG1Out(item, curRatio);
                            else if (killType == KillType.eKTGroup6)
                                curResult = Util.SimBuyG6(item, curRatio);
                            else if (killType == KillType.eKTBlend)
                                curResult = Util.SimCrossBuyG6G3(item, curRatio);
                            else if (killType == KillType.eKTGroup6OnGroup3)
                                curResult = Util.SimBuyG6On5G3Out(item, curRatio);

                            Program.mainForm.RefreshResultItem(curItemIndex, item);
                            ++curItemIndex;

                            if (curCal == null && curResult != TestResultType.eTRTIgnore)
                            {
                                curCal = new WrongInfo();
                                curCal.costTotal = item.simData.cost;
                                curCal.startTag = item.idTag;
                                curCal.round = 1;
                                allWrongInfos.Add(curCal);
                                if (curResult == TestResultType.eTRTSuccess)
                                    curCal = null;
                            }
                            else if (curCal != null)
                            {
                                if (curResult == TestResultType.eTRTFailed)
                                {
                                    curCal.round++;
                                    curCal.costTotal += item.simData.cost;
                                }
                                else if (curResult == TestResultType.eTRTSuccess)
                                {
                                    curCal.round++;
                                    curCal.costTotal += item.simData.cost;
                                    curCal = null;
                                }
                            }
                        }
                    }
                    ++curSimIndex;
                }
                if (curSimIndex == mgr.indexs.Count)
                {
                    curCal = null;
                    curState = SimState.eFinished;
                    Program.mainForm.RefreshResultPanel();
                }
            }
        }
    }


    public class SimulationGroup2 : SimulationBase
    {
        public override void SortWrongInfos(bool byRound)
        {

        }
        public override void StepRatio()
        {
        }
        public override void ResetRatio()
        {
        }
        public override void StartSimulate()
        {
        }
        public override void UpdateSimulate()
        {
        }
    }
#endif

    public class Simulator
    {
        static SimulationBase curSim = null;
        static Dictionary<SimType, SimulationBase> simDict = null;

        static Simulator()
        {
#if ENABLE_GROUP_COLLECT
            simDict = new Dictionary<SimType, SimulationBase>();
            simDict.Add(SimType.eGroup3, new SimulationGroup3());
            simDict.Add(SimType.eGroup2, new SimulationGroup2());
            curSim = simDict[SimType.eGroup3];
#endif
        }

        public static void SortWrongInfos(bool byRound)
        {
            if (curSim != null)
                curSim.SortWrongInfos(byRound);
        }

        public static void StepRatio()
        {
            if (curSim != null)
                curSim.StepRatio();
        }

        public static void ResetRatio()
        {
            if (curSim != null)
                curSim.ResetRatio();
        }

        public static void StartSimulate()
        {
            if (curSim != null)
                curSim.StartSimulate();
        }

        public static void UpdateSimulate()
        {
            if (curSim != null)
                curSim.UpdateSimulate();
        }
    }

    #endregion
}
