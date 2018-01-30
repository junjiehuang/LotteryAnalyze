//#define DEBUG_LOAD_DATA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Net;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    // 个位对数5期定胆
    // 0 - 1
    // 1 - 5
    // 2 - 3
    // 3 - 0
    // 4 - 7
    // 5 - 8
    // 6 - 2
    // 7 - 4
    // 8 - 3
    // 9 - 6

    // 十位对数3期定胆
    // 0 - 0
    // 1 - 8
    // 2 - 6
    // 3 - 4
    // 4 - 7
    // 5 - 9
    // 6 - 1
    // 7 - 2
    // 8 - 4
    // 9 - 6

    public enum GroupType
    {
        eGT1 = 1,
        eGT3 = 2,
        eGT6 = 3,
    }

    public enum TestResultType
    {
        eTRTFailed,
        eTRTSuccess,
        eTRTIgnore,
    }

    public class Util
    {
        public static bool ReadFile(int fileID, string filePath, ref OneDayDatas odd, ref int newDataIndex)
        {
            newDataIndex = -1;
            String line;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filePath, Encoding.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                sr.Close();
                return false;
            }
            while ((line = sr.ReadLine()) != null)
            {
                string[] strs = line.Split(' ');
                if (strs.Length > 1)
                {
                    if (string.IsNullOrEmpty(strs[0]) || string.IsNullOrEmpty(strs[1]) ||
                        !IsNumStr(strs[0]) || !IsNumStr(strs[1]))
                    {
#if DEBUG_LOAD_DATA
                        Console.WriteLine(strs[0] + " : " + strs[1]);
#endif
                        continue;
                    }
                    string tag = fileID + "-" + strs[0];
                    if (odd.FindItem(tag) != null)
                        continue;

                    DataItem item = new DataItem(strs[0], strs[1], fileID);
                    odd.AddItem(item);
                    if (newDataIndex == -1)
                        newDataIndex = item.id;
                }
            }
            sr.Close();
            return true;

        }

        public static bool ReadFile(int fileID, string filePath, ref OneDayDatas datas)
        {
            String line;
            StreamReader sr = null;
            datas = new OneDayDatas();
            datas.dateID = fileID;

            try
            {
                sr = new StreamReader(filePath, Encoding.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                sr.Close();
                return false;
            }
            while ((line = sr.ReadLine()) != null)
            {
                string[] strs = line.Split( ' ' );
                if (strs.Length > 1)
                {
                    if (string.IsNullOrEmpty(strs[0]) || string.IsNullOrEmpty(strs[1]) ||
                        !IsNumStr(strs[0]) || !IsNumStr(strs[1]))
                    {
#if DEBUG_LOAD_DATA
                        Console.WriteLine(strs[0] + " : " + strs[1]);
#endif
                        continue;
                    }

                    DataItem item = new DataItem(strs[0], strs[1], fileID);
                    datas.AddItem(item);
                    //item.parent = datas;
                    //item.id = datas.datas.Count;
                    //datas.datas.Add(item);
                }
            }
            sr.Close();
            return true;
        }
        public static bool IsNumStr(string str)
        {
            int v;
            if (int.TryParse(str, out v))
                return true;
            else
                return false;
        }

        public static int CharValue(char ch)
        {
            int value = ch - '0';
            return value;
        }

        public static int CalAndValue(string str)
        {
            int curId = str.Length - 1;
            int ge = CharValue(str[curId]); curId--;
            int shi = CharValue(str[curId]); curId--;
            int bai = CharValue(str[curId]); curId--;
            return ge + shi + bai;
        }

        public static int CalRearValue(string str)
        {
            int andValue = CalAndValue(str);
            int rearValue = andValue % 10;
            return rearValue;
        }

        public static int CalCrossValue(string str)
        {
            int curId = str.Length - 1;
            int ge = CharValue(str[curId]); curId--;
            int shi = CharValue(str[curId]); curId--;
            int bai = CharValue(str[curId]); curId--;
            int abs1 = Math.Abs(ge - shi);
            int abs2 = Math.Abs(ge - bai);
            int abs3 = Math.Abs(shi - bai);
            return Math.Max( abs3, Math.Max(abs1, abs2) );
        }



        public static GroupType GetGroupType(string str)
        {
            int curId = str.Length - 1;
            int ge = CharValue(str[curId]); curId--;
            int shi = CharValue(str[curId]); curId--;
            int bai = CharValue(str[curId]); curId--;
            if (ge == shi && ge == bai)
                return GroupType.eGT1;
            if (ge == shi || ge == bai || shi == bai)
                return GroupType.eGT3;
            return GroupType.eGT6;
        }

        // 获取整数srcNumber第index（从右向左数, index >= 0）位上的数字
        public static int GetNumberByPos(int srcNumber, int index)
        {
            string str = srcNumber.ToString();
            if (index >= str.Length)
                return 0;
            int realIndex = str.Length - index - 1;
            char ch = str[realIndex];
            int chValue = CharValue(ch);
            return chValue;
        }

        public static int GetCostByExceptAndValue(List<int> andValue, int ratio, GroupType gt)
        {
            int pairCount = AndValueSearchMap.GetPairCountExcept(andValue, gt);
            return pairCount * 2 * ratio;
        }

        public static int GetCost(int numCount, int ratio, GroupType gt)
        {
            int containsCount = 10 - numCount;
            if (gt == GroupType.eGT6)
            {
                switch (containsCount)
                {
                    case 3: return 2 * ratio;
                    case 4: return 8 * ratio;
                    case 5: return 20 * ratio;
                    case 6: return 40 * ratio;
                    case 7: return 70 * ratio;
                    case 8: return 112 * ratio;
                    case 9: return 168 * ratio;
                    case 10: return 240 * ratio;
                }
            }
            else if (gt == GroupType.eGT3)
            {
                switch (containsCount)
                {
                    case 2: return 4 * ratio;
                    case 3: return 12 * ratio;
                    case 4: return 24 * ratio;
                    case 5: return 40 * ratio;
                    case 6: return 60 * ratio;
                    case 7: return 84 * ratio;
                    case 8: return 112 * ratio;
                    case 9: return 144 * ratio;
                    case 10: return 180 * ratio;
                }
            }
            return 0;
        }

        public static int GetReward(GroupType groupID, int ratio)
        {
            switch (groupID)
            {
                case GroupType.eGT1: return 0;
                case GroupType.eGT3: return 576 * ratio;
                case GroupType.eGT6: return 288 * ratio;
            }
            return 0;
        }

        public static TestResultType SimBuyG6On5G3Out(DataItem item, int ratio)
        {
            item.simData.killType = KillType.eKTNone;
            item.simData.profit = DataManager.GetInst().curProfit;
            int g3Count = 0;
            TestResultType curResult = TestResultType.eTRTIgnore;
            DataItem last1 = DataManager.GetInst().GetPrevItem(item);
            if (last1 == null)
                return curResult;
            g3Count += last1.groupType == GroupType.eGT3 ? 1 : 0;
            DataItem last2 = DataManager.GetInst().GetPrevItem(last1);
            if (last2 == null)
                return curResult;
            g3Count += last2.groupType == GroupType.eGT3 ? 1 : 0;
            DataItem last3 = DataManager.GetInst().GetPrevItem(last2);
            if (last3 == null)
                return curResult;
            g3Count += last3.groupType == GroupType.eGT3 ? 1 : 0;
            DataItem last4 = DataManager.GetInst().GetPrevItem(last3);
            if (last4 == null)
                return curResult;
            g3Count += last4.groupType == GroupType.eGT3 ? 1 : 0;
            DataItem last5 = DataManager.GetInst().GetPrevItem(last4);
            if (last5 == null)
                return curResult;
            g3Count += last5.groupType == GroupType.eGT3 ? 1 : 0;
            if (g3Count >= 3)
            {
                curResult = SimBuyG6(item, ratio);
            }
            return curResult;
        }

        // 交叉匹配组三组六
        public static TestResultType SimCrossBuyG6G3(DataItem item, int ratio)
        {
            TestResultType curResult = TestResultType.eTRTIgnore;
            if (SimulationGroup3.curKillType == KillType.eKTGroup6)
            {
                curResult = SimBuyG6(item, ratio);
                if (curResult == TestResultType.eTRTFailed || item.groupType == GroupType.eGT1)
                {
                    SimulationGroup3.g6Round++;
                    if (SimulationGroup3.g6Round > 2 || item.groupType == GroupType.eGT1)
                    {
                        SimulationGroup3.curKillType = KillType.eKTGroup3;
                        SimulationGroup3.g3Round = 0;
                    }
                }
            }
            else if (SimulationGroup3.curKillType == KillType.eKTGroup3)
            {
                curResult = SimButG3OnG1Out(item, ratio);
                if (curResult == TestResultType.eTRTFailed)
                {
                    SimulationGroup3.g3Round++;
                    if (SimulationGroup3.g3Round > 2)
                    {
                        SimulationGroup3.curKillType = KillType.eKTGroup6;
                        SimulationGroup3.g6Round = 0;
                    }
                }
            }
            if (curResult == TestResultType.eTRTSuccess)
            {
                SimulationGroup3.g3Round = SimulationGroup3.g6Round = 0;
            }
            return curResult;
        }

        // 出豹子后匹配组三
        public static TestResultType SimButG3OnG1Out(DataItem item, int ratio)
        {
            DataItem prevItem = DataManager.GetInst().GetPrevItem(item);
            if (prevItem == null)
                return TestResultType.eTRTIgnore;
            if (prevItem.groupType == GroupType.eGT1 || SimulationGroup3.isCurKillGroup3 || SimulationGroup3.curKillType == KillType.eKTGroup3)
            {
                if (prevItem.groupType == GroupType.eGT1 )
                    SimulationGroup3.isCurKillGroup3 = true;
                item.simData.killType = KillType.eKTGroup3;
                List<int> killNums = new List<int>();
                KillNumberStrategyManager.GetInst().KillNumber(item, ref killNums);
                bool isRight = true;
                if (item.simData.killAndValue != null)
                {
                    item.simData.killList = "杀和值{";
                    for (int i = 0; i < item.simData.killAndValue.Count; ++i)
                    {
                        item.simData.killList += item.simData.killAndValue[i] + ",";
                    }
                    item.simData.killList += "}";
                    if (item.groupType == GroupType.eGT3)
                    {
                        for (int i = 0; i < item.simData.killAndValue.Count; ++i)
                        {
                            int killAndValue = item.simData.killAndValue[i];
                            // kill wrong number
                            if (item.andValue == killAndValue)
                            {
                                isRight = false;
                                break;
                            }
                        }
                    }
                    else
                        isRight = false;
                }
                else
                {
                    item.simData.killList = "杀号{";
                    for (int i = 0; i < killNums.Count; ++i)
                    {
                        item.simData.killList += killNums[i] + ",";
                    }
                    item.simData.killList += "}";
                    if (item.groupType == GroupType.eGT3)
                    {
                        for (int i = 0; i < killNums.Count; ++i)
                        {
                            byte killNum = (byte)killNums[i];
                            // kill wrong number
                            if (item.valuesOfLastThree.IndexOf(killNum) != -1)
                            {
                                isRight = false;
                                break;
                            }
                        }
                    }
                    else
                        isRight = false;
                }

                if (item.simData.killAndValue != null)
                    item.simData.cost = GetCostByExceptAndValue(item.simData.killAndValue, ratio, item.simData.killAndValueAtGroup);
                else
                    item.simData.cost = GetCost(killNums.Count, ratio, GroupType.eGT3);
                item.parent.simData.costTotal += item.simData.cost;
                item.parent.simData.predictCount++;
                DataManager.GetInst().simData.costTotal += item.simData.cost;
                DataManager.GetInst().simData.predictCount++;
                if (isRight)
                {
                    SimulationGroup3.isCurKillGroup3 = false;
                    item.simData.reward = GetReward(item.groupType, ratio);
                    item.parent.simData.rewardTotal += item.simData.reward;
                    item.parent.simData.rightCount++;
                    DataManager.GetInst().simData.rewardTotal += item.simData.reward;
                    DataManager.GetInst().simData.rightCount++;
                    Simulator.ResetRatio();
                    item.simData.predictResult = TestResultType.eTRTSuccess;
                }
                else
                {
                    Simulator.StepRatio();
                    item.simData.predictResult = TestResultType.eTRTFailed;
                }
                DataManager.GetInst().curProfit += -item.simData.cost + item.simData.reward;                
            }
            else
            {
                item.simData.predictResult = TestResultType.eTRTIgnore;
            }
            item.simData.profit = DataManager.GetInst().curProfit;
            return item.simData.predictResult;
        }

        // 匹配组六
        public static TestResultType SimBuyG6(DataItem item, int ratio)
        {
            List<int> killNums = new List<int>();
            item.simData.killType = KillType.eKTGroup6;

            KillNumberStrategyManager.GetInst().KillNumber(item, ref killNums);
            bool isRight = true;

            // 杀和值
            if (item.simData.killAndValue != null)
            {
                item.simData.killList = "杀和值{";
                for (int i = 0; i < item.simData.killAndValue.Count; ++i)
                {
                    item.simData.killList += item.simData.killAndValue[i] + ",";
                }
                item.simData.killList += "}";
                if (item.groupType == GroupType.eGT6)
                {
                    for (int i = 0; i < item.simData.killAndValue.Count; ++i)
                    {
                        int killAndValue = item.simData.killAndValue[i];
                        // kill wrong number
                        if (item.andValue == killAndValue)
                        {
                            isRight = false;
                            break;
                        }
                    }
                }
                else
                    isRight = false;
            }
            // 杀号
            else
            {
                item.simData.killList = "杀号{";
                for (int i = 0; i < killNums.Count; ++i)
                {
                    item.simData.killList += killNums[i] + ",";
                }
                item.simData.killList += "}";
                if (item.groupType == GroupType.eGT6)
                {
                    for (int i = 0; i < killNums.Count; ++i)
                    {
                        byte killNum = (byte)killNums[i];
                        // kill wrong number
                        if (item.valuesOfLastThree.IndexOf(killNum) != -1)
                        {
                            isRight = false;
                            break;
                        }
                    }
                }
                else
                    isRight = false;
            }
            
            item.simData.predictResult = isRight ? TestResultType.eTRTSuccess : TestResultType.eTRTFailed;
            item.simData.reward = 0;
            if (item.simData.killAndValue != null)
                item.simData.cost = GetCostByExceptAndValue(item.simData.killAndValue, ratio, item.simData.killAndValueAtGroup);
            else
                item.simData.cost = GetCost(killNums.Count, ratio, GroupType.eGT6);
            item.parent.simData.costTotal += item.simData.cost;
            item.parent.simData.predictCount++;
            DataManager.GetInst().simData.costTotal += item.simData.cost;
            DataManager.GetInst().simData.predictCount++;
            if (isRight)
            {
                item.simData.reward = GetReward(item.groupType, ratio);
                item.parent.simData.rewardTotal += item.simData.reward;
                item.parent.simData.rightCount++;
                DataManager.GetInst().simData.rewardTotal += item.simData.reward;
                DataManager.GetInst().simData.rightCount++;
                Simulator.ResetRatio();
            }
            else
            {
                Simulator.StepRatio();
            }
            DataManager.GetInst().curProfit += -item.simData.cost + item.simData.reward;
            item.simData.profit = DataManager.GetInst().curProfit;
            return item.simData.predictResult;
        }

        public static bool CollectPath012Info(List<SinglePath012MaxMissingCollector.MissingInfo> maxMissingInfo, OneDayDatas newAddOdd = null, int newAddIndex = -1)
        {
            DataManager dataMgr = DataManager.GetInst();
            if (dataMgr.indexs == null) return false;
            int count = dataMgr.indexs.Count;
            if (count == 0) return false;

            if (maxMissingInfo != null && newAddIndex == -1)
            {
                for (int i = 0; i < 5; ++i)
                {
                    maxMissingInfo[i].maxPath012MissingData[0] = maxMissingInfo[i].maxPath012MissingData[1] = maxMissingInfo[i].maxPath012MissingData[2] = 0;
                    maxMissingInfo[i].maxPath012MissingID[0] = maxMissingInfo[i].maxPath012MissingID[1] = maxMissingInfo[i].maxPath012MissingID[2] = -1;
                }
            }
            OneDayDatas curODD = dataMgr.allDatas[dataMgr.indexs[0]];
            if (newAddOdd != null)
                curODD = newAddOdd;
            DataItem curItem = null;
            if(curODD.datas.Count > 0)
                curItem = curODD.datas[0];
            if(newAddIndex != -1 && newAddIndex < curODD.datas.Count)
                curItem = curODD.datas[newAddIndex];
            curODD.CollectShortPath012Info();

            while(curItem != null)
            {
                if(curItem.parent != curODD)
                {
                    curODD = curItem.parent;
                    curODD.CollectShortPath012Info();
                }
                CollectPath012Info(curItem, maxMissingInfo);

                curItem = curItem.parent.GetNextItem(curItem);
            }

            //for (int i = 0; i < count; ++i)
            //{
            //    int oneDayID = DataManager.GetInst().indexs[i];
            //    OneDayDatas odd = DataManager.GetInst().allDatas[oneDayID];
            //    odd.CollectShortPath012Info();

            //    for (int j = 0; j < odd.datas.Count; ++j)
            //    {
            //        DataItem item = odd.datas[j];
            //        item.CollectShortPath012Info();

            //        DataItem prevItem = item.parent.GetPrevItem(item);
            //        if (prevItem != null)
            //        {
            //            for (int k = 0; k < 5; ++k)
            //            {
            //                for (int t = 0; t < 3; ++t)
            //                {
            //                    if (prevItem.path012OfEachSingle[k] == t)
            //                        item.simData.path012MissingInfo[k][t] = 0;
            //                    else
            //                        item.simData.path012MissingInfo[k][t] = prevItem.simData.path012MissingInfo[k][t] + 1;

            //                    if (prevItem.path012OfEachSingle[k] == t)
            //                        item.simData.path012CountInfoLong[k][t] = prevItem.simData.path012CountInfoLong[k][t] + 1;
            //                    else
            //                        item.simData.path012CountInfoLong[k][t] = prevItem.simData.path012CountInfoLong[k][t];

            //                    if (maxMissingInfo != null && item.simData.path012MissingInfo[k][t] > maxMissingInfo[k].maxPath012MissingData[t])
            //                    {
            //                        maxMissingInfo[k].maxPath012MissingData[t] = item.simData.path012MissingInfo[k][t];
            //                        maxMissingInfo[k].maxPath012MissingID[t] = item.idGlobal;
            //                    }
            //                }
            //                int totalCount = item.simData.path012CountInfoLong[k][0] + item.simData.path012CountInfoLong[k][1] + item.simData.path012CountInfoLong[k][2];
            //                if(totalCount > 0)
            //                {
            //                    item.simData.path012ProbabilityLong[k][0] = item.simData.path012CountInfoLong[k][0] * 100 / totalCount;
            //                    item.simData.path012ProbabilityLong[k][1] = item.simData.path012CountInfoLong[k][1] * 100 / totalCount;
            //                    item.simData.path012ProbabilityLong[k][2] = item.simData.path012CountInfoLong[k][2] * 100 / totalCount;
            //                }
            //            }
            //        }
            //    }
            //}
            return true;
        }
        static void CollectPath012Info(DataItem item, List<SinglePath012MaxMissingCollector.MissingInfo> maxMissingInfo)
        {
            DataItem prevItem = item.parent.GetPrevItem(item);
            if (prevItem != null)
            {
                for (int k = 0; k < 5; ++k)
                {
                    for (int t = 0; t < 3; ++t)
                    {
                        if (prevItem.path012OfEachSingle[k] == t)
                            item.simData.path012MissingInfo[k][t] = 0;
                        else
                            item.simData.path012MissingInfo[k][t] = prevItem.simData.path012MissingInfo[k][t] + 1;

                        if (prevItem.path012OfEachSingle[k] == t)
                            item.simData.path012CountInfoLong[k][t] = prevItem.simData.path012CountInfoLong[k][t] + 1;
                        else
                            item.simData.path012CountInfoLong[k][t] = prevItem.simData.path012CountInfoLong[k][t];

                        if (maxMissingInfo != null && item.simData.path012MissingInfo[k][t] > maxMissingInfo[k].maxPath012MissingData[t])
                        {
                            maxMissingInfo[k].maxPath012MissingData[t] = item.simData.path012MissingInfo[k][t];
                            maxMissingInfo[k].maxPath012MissingID[t] = item.idGlobal;
                        }
                    }
                    int totalCount = item.simData.path012CountInfoLong[k][0] + item.simData.path012CountInfoLong[k][1] + item.simData.path012CountInfoLong[k][2];
                    if (totalCount > 0)
                    {
                        item.simData.path012ProbabilityLong[k][0] = item.simData.path012CountInfoLong[k][0] * 100 / totalCount;
                        item.simData.path012ProbabilityLong[k][1] = item.simData.path012CountInfoLong[k][1] * 100 / totalCount;
                        item.simData.path012ProbabilityLong[k][2] = item.simData.path012CountInfoLong[k][2] * 100 / totalCount;
                    }
                }
            }
        }
    }


    public class AutoUpdateUtil
    {
        public delegate void OnCollecting(string info);
        public static OnCollecting sCallBackOnCollecting;

        enum ECType
        {
            UTF8,
            Default,
        };

        /// <summary>
        /// 自动获取当天数据
        /// </summary>
        public static int AutoFetchTodayData()
        {
            DateTime curDate = DateTime.Now;
            DateTime lastDate = curDate.AddDays(-1);
            FetchData(lastDate);

            string filename = combineFileName(curDate.Year, curDate.Month, curDate.Day);
            string url = "http://chart.cp.360.cn/kaijiang/ssccq?sb_spm=36335ab32b7a2ac5a4fa0881e40a5f6a";
            return FetchData(filename, url);
        }

        public static string combineDateString(int y, int m, int d)
        {
            string dateStr = y.ToString();
            if (m < 10)
                dateStr += "0";
            dateStr += m;
            if (d < 10)
                dateStr += "0";
            dateStr += d;
            return dateStr;
        }

        public static string combineFileName(int y, int m, int d)
        {
            string fileName = "..\\data\\" + y;
            if (m < 10)
                fileName += "0";
            fileName += m;
            if (d < 10)
                fileName += "0";
            fileName += d + ".txt";
            return fileName;
        }

        public static string combineUrlName(int y, int m, int d)
        {
            string url = "http://chart.cp.360.cn/kaijiang/kaijiang?lotId=255401&spanType=2&span=" + y + "-";
            if (m < 10)
                url += "0";
            url += m + "-";
            if (d < 10)
                url += "0";
            url += d + "_" + y + "-";
            if (m < 10)
                url += "0";
            url += m + "-";
            if (d < 10)
                url += "0";
            url += d;
            return url;
        }

        /// <summary>
        /// 获取指定日期的数据
        /// </summary>
        /// <param name="startYear"></param>
        /// <param name="startMonth"></param>
        /// <param name="startDay"></param>
        /// <param name="endYear"></param>
        /// <param name="endMonth"></param>
        /// <param name="endDay"></param>
        public static void FetchDatas(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
        {
            string filename = "";
            string url = "";
            DateTime startDate = new DateTime(startYear, startMonth, startDay);
            DateTime endDate = new DateTime(endYear, endMonth, endDay);
            int diff = DateTime.Compare(startDate, endDate);
            if (diff == 0)
            {
                filename = combineFileName(startDate.Year, startDate.Month, startDate.Day);
                url = combineUrlName(startDate.Year, startDate.Month, startDate.Day);
                FetchData(filename, url);
                sCallBackOnCollecting("fetch " + startDate.ToString() + "\r\n");
            }
            else
            {
                DateTime curDate = diff < 0 ? startDate : endDate;
                while (DateTime.Compare(curDate, endDate) < 1)
                {
                    filename = combineFileName(curDate.Year, curDate.Month, curDate.Day);
                    url = combineUrlName(curDate.Year, curDate.Month, curDate.Day);
                    FetchData(filename, url);
                    sCallBackOnCollecting("fetch " + curDate.ToString() + "\r\n");

                    curDate = curDate.AddDays(1);
                }
            }
        }

        public static int FetchData(DateTime date)
        {
            string filename = combineFileName(date.Year, date.Month, date.Day);
            string url = combineUrlName(date.Year, date.Month, date.Day);
            return FetchData(filename, url);
        }

        static HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
        static string strRegexR = @"(?<=<tr>)([\s\S]*?)(?=</tr>)"; //构造解析表格数据的正则表达式
        static string strRegexD = @"(?<=<td[^>]*>[\s]*?)([\S]*)(?=[\s]*?</td>)";
        static Regex regexR = new Regex(strRegexR);
        static Regex regexD = new Regex(strRegexD);

        public static int FetchData(string fileName, string webUrl)
        {
            int validCount = 0;
            // load web page
            ECType t = ECType.Default;
            WebRequest request = WebRequest.Create(webUrl);
            WebResponse response = request.GetResponse();
            StreamReader reader = null;
            switch (t)
            {
                case ECType.UTF8:
                    reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                    break;
                case ECType.Default:
                default:
                    reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                    break;
            }
            string strWebContent = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            response.Close();

            string lotteryData = "";
            htmlDocument.LoadHtml(strWebContent);
            HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectSingleNode("html/body").ChildNodes;
            foreach (HtmlNode wrapNode in collection)
            {
                if (wrapNode.Name == "div" && wrapNode.GetAttributeValue("class", "") == "wrap")
                {
                    foreach (HtmlNode histTabNode in wrapNode.ChildNodes)
                    {
                        if (histTabNode.GetAttributeValue("class", "") == "history-tab")
                        {
                            MatchCollection mcR = regexR.Matches(histTabNode.InnerHtml); //执行匹配
                            int totalCount = 120;
                            foreach (Match mr in mcR)
                            {
                                MatchCollection mcD = regexD.Matches(mr.Groups[0].ToString()); //执行匹配                                                                
                                for (int i = 0; i < mcD.Count; i++)
                                {
                                    if (totalCount > 0 && Util.IsNumStr(mcD[i].Value) && mcD[i].Value.Length == 3)
                                    {
                                        lotteryData += mcD[i].Value + " ";
                                        ++i;
                                        if (Util.IsNumStr(mcD[i].Value))
                                        {
                                            lotteryData += mcD[i].Value;
                                            ++validCount;
                                        }
                                        else
                                            lotteryData += "-";
                                        lotteryData += "\n";
                                        --totalCount;
                                        if (totalCount == 0)
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Console.WriteLine("=> " + fileName);
            //Console.WriteLine(lotteryData);

            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(lotteryData);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
            return validCount;
        }
    }
}
