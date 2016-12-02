﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
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

        public void Reset()
        {
            predictResult = TestResultType.eTRTIgnore;
            cost = reward = costTotal = rewardTotal = predictCount = rightCount = profit = 0;
            killList = null;
        }
    }

    public class DataItem
    {
        public OneDayDatas parent = null;

        public int id;
        public string idTag;
        public string lotteryNumber;
        public int andValue;
        public int rearValue;
        public int crossValue;
        public GroupType groupType;
        public List<int> valuesInThreePos = new List<int>();

        public SimData simData;

        public DataItem()
        {
        }

        public int GetGeNumber()
        {
            int value = Util.CharValue(lotteryNumber[lotteryNumber.Length - 1]);
            return value;
        }
        public int GetShiNumber()
        {
            int value = Util.CharValue(lotteryNumber[lotteryNumber.Length - 2]);
            return value;
        }
        public int GetBaiNumber()
        {
            int value = Util.CharValue(lotteryNumber[lotteryNumber.Length - 3]);
            return value;
        }
        public void GetValuesInThreePos()
        {
            if (valuesInThreePos.Count == 0)
            {
                valuesInThreePos.Add(GetBaiNumber());
                valuesInThreePos.Add(GetShiNumber());
                valuesInThreePos.Add(GetGeNumber());
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
            return datas[datas.Count-1];
        }

        public DataItem GetPrevItem(DataItem curItem)
        {
            if (curItem.id > 1)
                return datas[curItem.id - 2];
            else
            {
                OneDayDatas prevODD = DataManager.GetInst().GetPrevOneDayDatas(this);
                if (prevODD != null)
                    return GetTailItem();
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
    }

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

    public class Simulator
    {
        static SimState curState = SimState.eNotStart;
        static int curSimIndex = -1;
        static int curItemIndex = -1;
        static int curRatio = 1;
        static bool enableDoubleRatioIfFailed = true;

        public static WrongInfo maxCost = new WrongInfo();
        public static WrongInfo maxRound = new WrongInfo();
        static WrongInfo curCal = new WrongInfo();
        static bool startCount = false;

        public static bool isCurKillGroup3 = false;
        static bool testSimKillGroup3OnGroup1Out = true;

        public static void StepRatio()
        {
            if (enableDoubleRatioIfFailed)
                curRatio *= 2;
        }
        public static void ResetRatio()
        {
            curRatio = 1;
        }

        public static void StartSimulate()
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
            startCount = true;
            ResetRatio();
            Program.mainForm.ResetResult();
            curState = SimState.eSimulating;
        }

        public static void UpdateSimulate()
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
                            TestResultType curResult = TestResultType.eTRTIgnore;
                            if (testSimKillGroup3OnGroup1Out)
                                curResult = Util.SimKillGroup2OnGroup1Out(item, curRatio);
                            else
                                curResult = Util.SimKillNumberAndCheckResult(item, curRatio);
                            Program.mainForm.RefreshResultItem(curItemIndex, item);
                            ++curItemIndex;

                            if (startCount && curResult == TestResultType.eTRTFailed )
                            {
                                startCount = false;
                                curCal.costTotal = item.simData.cost;
                                curCal.startTag = item.idTag;
                                curCal.round = 1;
                            }
                            else
                            {
                                DataItem prev = DataManager.GetInst().GetPrevItem(item);
                                if (prev != null)
                                {
                                    if (prev.simData.predictResult == TestResultType.eTRTSuccess && curResult == TestResultType.eTRTFailed)
                                    {
                                        curCal.costTotal = item.simData.cost;
                                        curCal.startTag = item.idTag;
                                        curCal.round = 1;
                                    }
                                    else if (prev.simData.predictResult == TestResultType.eTRTFailed && item.simData.predictResult == TestResultType.eTRTSuccess)
                                    {
                                        if (maxCost.costTotal < curCal.costTotal)
                                            maxCost.CopyFrom(curCal);
                                        if (maxRound.round < curCal.round)
                                            maxRound.CopyFrom(curCal);
                                    }
                                    else if (prev.simData.predictResult == TestResultType.eTRTFailed && item.simData.predictResult == TestResultType.eTRTFailed)
                                    {
                                        curCal.round++;
                                        curCal.costTotal += item.simData.cost;
                                    }
                                }
                            }
                        }
                    }
                    ++curSimIndex;
                }
                if (curSimIndex == mgr.indexs.Count)
                {
                    if (maxCost.costTotal < curCal.costTotal)
                        maxCost.CopyFrom(curCal);
                    if (maxRound.round < curCal.round)
                        maxRound.CopyFrom(curCal);
                    curState = SimState.eFinished;
                    Program.mainForm.RefreshResultPanel();
                }
            }
        }
    }
}
