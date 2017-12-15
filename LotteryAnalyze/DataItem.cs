using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    #region data manage
    public struct SimData
    {
        public string killList;
        public TestResultType predictResult;
        public long cost;
        public long reward;
        public long costTotal;
        public long rewardTotal;
        public long predictCount;
        public long rightCount;
        public long profit;
        public KillType killType;

        public List<int> killAndValue;
        public GroupType killAndValueAtGroup;

        public float g6Score;
        public float g3Score;
        public float g1Score;

        public void Reset()
        {
            predictResult = TestResultType.eTRTIgnore;
            cost = reward = costTotal = rewardTotal = predictCount = rightCount = profit = 0;
            killList = null;

            killAndValue = null;
            killAndValueAtGroup = GroupType.eGT6;

            g6Score = g3Score = g1Score = 0.0f;
        }
    }

    public class DataItem
    {
        public OneDayDatas parent = null;

        public int idGlobal = -1;
        public int id;
        public string idTag;
        public string lotteryNumber;
        public int andValue;
        public int rearValue;
        public int crossValue;
        public GroupType groupType;
        public List<int> valuesOfLastThree = new List<int>();

        public SimData simData;

        public DataItem()
        {
        }

        public int GetGeNumber()
        {
            int value = Util.CharValue(lotteryNumber[4]);
            return value;
        }
        public int GetShiNumber()
        {
            int value = Util.CharValue(lotteryNumber[3]);
            return value;
        }
        public int GetBaiNumber()
        {
            int value = Util.CharValue(lotteryNumber[2]);
            return value;
        }
        public int GetQianNumber()
        {
            int value = Util.CharValue(lotteryNumber[1]);
            return value;
        }
        public int GetWanNumber()
        {
            int value = Util.CharValue(lotteryNumber[0]);
            return value;
        }
        public void GetValuesInThreePos()
        {
            if (valuesOfLastThree.Count == 0)
            {
                valuesOfLastThree.Add(GetBaiNumber());
                valuesOfLastThree.Add(GetShiNumber());
                valuesOfLastThree.Add(GetGeNumber());
            }
        }
    }

    public class OneDayDatas
    {
        public int dateID = 0;
        public List<DataItem> datas = new List<DataItem>();
        public SimData simData;


        public OneDayDatas()
        {
        }

        public DataItem GetTailItem()
        {
            if (datas.Count > 0)
                return datas[datas.Count-1];
            return null;
        }
        public DataItem GetFirstItem()
        {
            if (datas.Count > 0)
                return datas[0];
            return null;
        }
        public DataItem GetPrevItem(DataItem curItem)
        {
            int curID = curItem.id - 1;
            if (curID > 0)
                return datas[curID-1];
            else
            {
                OneDayDatas prevODD = DataManager.GetInst().GetPrevOneDayDatas(this);
                if (prevODD != null)
                    return prevODD.GetTailItem();
            }
            return null;
        }
        public DataItem GetNextItem(DataItem curItem)
        {
            int curID = curItem.id-1;
            if (curID < datas.Count - 1)
                return datas[curID + 1];
            else
            {
                OneDayDatas nextODD = DataManager.GetInst().GetNextOneDayDatas(this);
                if (nextODD != null)
                    return nextODD.GetFirstItem();
            }
            return null;
        }
    }

    public class DataManager
    {
        public Dictionary<int, OneDayDatas> allDatas = new Dictionary<int, OneDayDatas>();
        public List<int> indexs = new List<int>();
        public Dictionary<int, string> mFileMetaInfo = new Dictionary<int, string>();
        public SimData simData;
        public long curProfit = 0;

        DataManager()
        {
        }

        static DataManager sInst = null;
        public static DataManager GetInst()
        {
            if (sInst == null)
                sInst = new DataManager();
            return sInst;
        }

        public void ClearAllDatas()
        {
            allDatas.Clear();
            indexs.Clear();
        }

        public void LoadAllDatas(ref List<int> selectIDs)
        {
            for (int i = 0; i < selectIDs.Count; ++i)
            {
                int key = selectIDs[i];
                LoadData(key);
            }
        }
        public void LoadData(int key)
        {
            OneDayDatas data = null;
            string fullPath = mFileMetaInfo[key];
            if (Util.ReadFile(key, fullPath, ref data))
            {
                allDatas.Add(key, data);
                if (indexs.IndexOf(key) == -1)
                {
                    indexs.Add(key);
                    indexs.Sort();
                }
            }
        }
        public OneDayDatas GetPrevOneDayDatas(OneDayDatas curData)
        {
            int index = indexs.IndexOf(curData.dateID);
            if (index > 0)
            {
                --index;
                int newDateID = indexs[index];
                return allDatas[newDateID];
            }
            return null;
        }
        public OneDayDatas GetNextOneDayDatas(OneDayDatas curData)
        {
            int index = indexs.IndexOf(curData.dateID);
            if (index < indexs.Count-1)
            {
                ++index;
                int newDateID = indexs[index];
                return allDatas[newDateID];
            }
            return null;
        }
        public DataItem GetPrevItem(DataItem curItem)
        {
            DataItem prevItem = curItem.parent.GetPrevItem(curItem);
            if (prevItem != null)
                return prevItem;
            OneDayDatas prevODD = GetPrevOneDayDatas(curItem.parent);
            if (prevODD != null)
                return prevODD.GetTailItem();
            return null;
        }
        public DataItem GetNextItem(DataItem curItem)
        {
            DataItem nextItem = curItem.parent.GetNextItem(curItem);
            if (nextItem != null)
                return nextItem;
            OneDayDatas nextODD = GetNextOneDayDatas(curItem.parent);
            if (nextODD != null)
                return nextODD.GetFirstItem();
            return null;
        }
    }

    #endregion

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


    public class Simulator
    {
        static SimulationBase curSim = null;
        static Dictionary<SimType, SimulationBase> simDict = null;

        static Simulator()
        {
            simDict = new Dictionary<SimType, SimulationBase>();
            simDict.Add(SimType.eGroup3, new SimulationGroup3());
            simDict.Add(SimType.eGroup2, new SimulationGroup2());
            curSim = simDict[SimType.eGroup3];
        }

        public static void SortWrongInfos(bool byRound)
        {
            curSim.SortWrongInfos(byRound);
        }

        public static void StepRatio()
        {
            curSim.StepRatio();
        }

        public static void ResetRatio()
        {
            curSim.ResetRatio();
        }

        public static void StartSimulate()
        {
            curSim.StartSimulate();
        }

        public static void UpdateSimulate()
        {
            curSim.UpdateSimulate();
        }
    }

    #endregion
}
