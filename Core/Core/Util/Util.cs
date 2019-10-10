//#define DEBUG_LOAD_DATA
#define USE_163_URL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Configuration;
using System.IO.Compression;

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
                        newDataIndex = item.idInOneDay;
                }
            }
            sr.Close();
            return odd.datas.Count > 0;
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
                if (sr != null)
                {
                    sr.Close();
                }
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
            return datas.datas.Count > 0;
        }
        public static bool IsNumStr(string str)
        {
            int v;
            if (int.TryParse(str, out v))
                return true;
            else
                return false;
        }

        public static SByte CharValue(char ch)
        {
            SByte value = (SByte)(ch - '0');
            return value;
        }

        public static SByte CalAndValue(string str)
        {
            int curId = str.Length - 1;
            SByte ge = CharValue(str[curId]); curId--;
            SByte shi = CharValue(str[curId]); curId--;
            SByte bai = CharValue(str[curId]); curId--;
            return (SByte)(ge + shi + bai);
        }

        public static SByte CalRearValue(string str)
        {
            SByte andValue = CalAndValue(str);
            SByte rearValue = (SByte)(andValue % 10);
            return rearValue;
        }

        public static SByte CalCrossValue(string str)
        {
            int curId = str.Length - 1;
            SByte ge = CharValue(str[curId]); curId--;
            SByte shi = CharValue(str[curId]); curId--;
            SByte bai = CharValue(str[curId]); curId--;
            SByte abs1 = (SByte)Math.Abs(ge - shi);
            SByte abs2 = (SByte)Math.Abs(ge - bai);
            SByte abs3 = (SByte)Math.Abs(shi - bai);
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

        public static int GetCostByExceptAndValue(List<SByte> andValue, int ratio, GroupType gt)
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

#if ENABLE_GROUP_COLLECT
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
                            SByte killNum = (SByte)killNums[i];
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
                        SByte killNum = (SByte)killNums[i];
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
#endif


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
            //curODD.CollectOneDayLotteryInfo();

            while(curItem != null)
            {
                if(curItem.parent != curODD)
                {
                    curODD = curItem.parent;
                    //curODD.CollectOneDayLotteryInfo();
                }
                CollectPath012Info(curItem, maxMissingInfo);

                curItem = curItem.parent.GetNextItem(curItem);
            }
            return true;
        }
        static void CollectPath012Info(DataItem item, List<SinglePath012MaxMissingCollector.MissingInfo> maxMissingInfo)
        {
            item.statisticInfo.Collect();

            if (maxMissingInfo != null)
            {
                for (int i = 0; i < 5; ++i)
                {
                    StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[i];
                    if (sum.statisticUnitMap[CollectDataType.ePath0].missCount > maxMissingInfo[i].maxPath012MissingData[0])
                    {
                        maxMissingInfo[i].maxPath012MissingData[0] = sum.statisticUnitMap[CollectDataType.ePath0].missCount;
                        maxMissingInfo[i].maxPath012MissingID[0] = item.idGlobal;
                    }
                    if (sum.statisticUnitMap[CollectDataType.ePath1].missCount > maxMissingInfo[i].maxPath012MissingData[1])
                    {
                        maxMissingInfo[i].maxPath012MissingData[1] = sum.statisticUnitMap[CollectDataType.ePath1].missCount;
                        maxMissingInfo[i].maxPath012MissingID[1] = item.idGlobal;
                    }
                    if (sum.statisticUnitMap[CollectDataType.ePath2].missCount > maxMissingInfo[i].maxPath012MissingData[2])
                    {
                        maxMissingInfo[i].maxPath012MissingData[2] = sum.statisticUnitMap[CollectDataType.ePath2].missCount;
                        maxMissingInfo[i].maxPath012MissingID[2] = item.idGlobal;
                    }
                }
            }
        }
    }

    public class AutoUpdateUtil
    {
        static string S_OLD_CURRENT_URL = "http://chart.cp.360.cn/kaijiang/ssccq?sb_spm=36335ab32b7a2ac5a4fa0881e40a5f6a";
        static string S_NEW_CURRENT_URL = "https://chart.cp.360.cn/zst/getchartdata?sb_spm=53c826cb603073977966b7e3425b731b&lotId=255401&chartType=x5";

        static string S_OLD_DATE_URL = "http://chart.cp.360.cn/kaijiang/kaijiang?lotId=255401&spanType=2&span=";
        static string S_NEW_DATE_URL = "https://chart.cp.360.cn/zst/ssccq?lotId=255401&chartType=x5&spanType=2&span=";

        static string S_163_NEW_CURRENT_URL = "http://caipiao.163.com/award/cqssc/";
        static string S_163_NEW_DATE_URL = "http://caipiao.163.com/award/cqssc/";

        static string S_CAIBOW_DATE_URL = "https://www.caibow.com/kj/cqssc/";

        public delegate void OnCollecting(string info);
        public static OnCollecting sCallBackOnCollecting;

        enum ECType
        {
            UTF8,
            Default,
            GBK,
        };

        // 数据源
        public enum DataSourceType
        {
            e360 = 0,
            e163 = 1,
            eCaiBow = 2,

            eMax,
        }

        //public static DataSourceType CURRENT_DATA_SOURCE = DataSourceType.e163;

        /// <summary>
        /// 自动获取当天数据
        /// </summary>
        public static int AutoFetchTodayData()
        {
            string error = "";
            DateTime curDate = DateTime.Now;
            DateTime lastDate = curDate.AddDays(-1);
            FetchData(lastDate, ref error);
            return FetchData(curDate, ref error);
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

        public static string DATA_PATH_FOLDER = "..\\data\\";

        public static string combineFileName(int y, int m, int d)
        {
            string fileName = DATA_PATH_FOLDER + y;
            if (m < 10)
                fileName += "0";
            fileName += m;
            if (d < 10)
                fileName += "0";
            fileName += d + ".txt";
            return fileName;
        }

        public static string combineUrlName(int y, int m, int d, bool newUrl)
        {
            string url = string.Empty;
            if (GlobalSetting.G_DATA_SOURCE_TYPE == DataSourceType.e163)
            {
                url = S_163_NEW_DATE_URL + y;
                if (m < 10)
                    url += "0";
                url += m;
                if (d < 10)
                    url += "0";
                url += d + ".html";
            }
            else if(GlobalSetting.G_DATA_SOURCE_TYPE == DataSourceType.eCaiBow)
            {
                url = S_CAIBOW_DATE_URL + y + "-";
                if (m < 10)
                    url += "0";
                url += m + "-";
                if (d < 10)
                    url += "0";
                url += d + ".html";
            }
            else
            {
                url = S_NEW_DATE_URL;
                if (!newUrl)
                    url = S_OLD_DATE_URL;
                url += y + "-";
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
            }
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
            string error = "";
            string filename = "";
            string url = "";
            DateTime startDate = new DateTime(startYear, startMonth, startDay);
            DateTime endDate = new DateTime(endYear, endMonth, endDay);
            int diff = DateTime.Compare(startDate, endDate);
            if (diff == 0)
            {
                //filename = combineFileName(startDate.Year, startDate.Month, startDate.Day);
                //url = combineUrlName(startDate.Year, startDate.Month, startDate.Day);
                //FetchData(filename, url, ref error);
                FetchData(startDate, ref error);
                sCallBackOnCollecting("fetch " + startDate.ToString() + "\r\n");
            }
            else
            {
                DateTime curDate = diff < 0 ? startDate : endDate;
                while (DateTime.Compare(curDate, endDate) < 1)
                {
                    //filename = combineFileName(curDate.Year, curDate.Month, curDate.Day);
                    //url = combineUrlName(curDate.Year, curDate.Month, curDate.Day);
                    //FetchData(filename, url, ref error);
                    FetchData(curDate, ref error);
                    sCallBackOnCollecting("fetch " + curDate.ToString() + "\r\n");

                    curDate = curDate.AddDays(1);
                }
            }
        }

        static int CombineFileID(int y, int m, int d)
        {
            string url = y.ToString();
            if (m < 10)
                url += "0";
            url += m;
            if (d < 10)
                url += "0";
            url += d;
            return int.Parse(url);
        }

        public static int FetchData(DateTime date, ref string error)
        {
            int dataCount = 0;
            string filename = combineFileName(date.Year, date.Month, date.Day);
            int fid = CombineFileID(date.Year, date.Month, date.Day);
            if (GlobalSetting.G_DATA_SOURCE_TYPE == DataSourceType.e163)
            {
                string url = combineUrlName(date.Year, date.Month, date.Day, true);
                dataCount = FetchData(true, filename, url, ref error, fid);
            }
            else if(GlobalSetting.G_DATA_SOURCE_TYPE == DataSourceType.eCaiBow)
            {
                string url = combineUrlName(date.Year, date.Month, date.Day, true);
                dataCount = FetchData(true, filename, url, ref error, fid);
            }
            else
            {             
                // 先按旧版的网页数据拉取
                string url = combineUrlName(date.Year, date.Month, date.Day, false);
                dataCount = FetchData(false, filename, url, ref error, fid);
                // 如果拉取到的数据是空的，就按照新版的网页数据来拉取
                if (dataCount == 0)
                {
                    url = combineUrlName(date.Year, date.Month, date.Day, true);
                    dataCount = FetchData(true, filename, url, ref error, fid);
                }
            }
            return dataCount;
        }

        static HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
        static string strRegexR = @"(?<=<tr>)([\s\S]*?)(?=</tr>)"; //构造解析表格数据的正则表达式
        static string strRegexD = @"(?<=<td[^>]*>[\s]*?)([\S]*)(?=[\s]*?</td>)";
        static string strRegexDate = @"(?<=<td class='tdbg_1' >[\s]*?)([\S]*)(?=[\s]*?</td>)";
        static string strRegexNumber = @"(?<=<td[^>]*><strong class='num'>[\s]*?)([\S]*)(?=[\s]*?</strong></td>)";
        static Regex regexR = new Regex(strRegexR);
        static Regex regexD = new Regex(strRegexD);
        static Regex regexDate = new Regex(strRegexDate);
        static Regex regexNumber = new Regex(strRegexNumber);

        public static int FetchData(bool newUrl, string fileName, string webUrl, ref string error, int fileID)
        {
            Console.WriteLine("Fetch weburl : " + webUrl);

            int validCount = 0;
            // load web page
            ECType t = ECType.Default;
            if (GlobalSetting.G_DATA_SOURCE_TYPE == DataSourceType.e163 ||
                GlobalSetting.G_DATA_SOURCE_TYPE == DataSourceType.eCaiBow)
            {
                t = ECType.UTF8;
            }
            WebRequest request = WebRequest.Create(webUrl);
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch(Exception e)
            {
                error = "Request url <" + webUrl + "> failed! info = " + e.ToString();
                Console.WriteLine(error);
                return 0;
            }

            if (response == null)
                return 0;

            bool isZipCont = response.ContentEncoding == "gzip";
            //先把响应流以gzip形式解码，然后再读取。结果success.......
            Stream stm = null;
            if (isZipCont)
                stm = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
            else
                stm = response.GetResponseStream();

            StreamReader reader = null;
            switch (t)
            {
                case ECType.UTF8:
                    reader = new StreamReader(stm, Encoding.UTF8);
                    break;
                case ECType.GBK:
                    reader = new StreamReader(stm, Encoding.GetEncoding("GBK"));
                    break;
                case ECType.Default:
                default:
                    reader = new StreamReader(stm, Encoding.Default);
                    break;
            }

            string strWebContent = "";
            try
            {
                strWebContent = reader.ReadToEnd();
            }
            catch(Exception e)
            {
                Console.WriteLine("FetchData failed on read web stream - " + e.ToString());
            }
            reader.Close();
            reader.Dispose();
            response.Close();

            string lotteryData = "";

            if (GlobalSetting.G_DATA_SOURCE_TYPE == DataSourceType.e163)
            {
                AnalyzeBy163(strWebContent, ref lotteryData, ref validCount);
            }
            else if(GlobalSetting.G_DATA_SOURCE_TYPE == DataSourceType.eCaiBow)
            {
                AnalyzeByCaiBow(strWebContent, ref lotteryData, ref validCount);
            }
            else
            {
                AnalyzeBy360(strWebContent, ref lotteryData, ref validCount, newUrl);
            }

            //Console.WriteLine("=> " + fileName);
            //Console.WriteLine(lotteryData);


            // check old data
            OneDayDatas data = null;
            if (Util.ReadFile(fileID, fileName, ref data))
            {
                if (data.datas.Count > validCount)
                    return data.datas.Count;
            }
            //


            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(lotteryData);
            //sw.Write(strWebContent);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
            return validCount;
        }

        static void AnalyzeBy163(string strWebContent, ref string lotteryData, ref int validCount)
        {
            string[] lst = new string[120];
            string startStr = "data-win-number=";
            string endStr = "</td>";
            int index = strWebContent.IndexOf(startStr);
            while (index != -1)
            {
                int endIndex = strWebContent.IndexOf(endStr);
                if (endIndex == -1)
                    break;
                string subStr = strWebContent.Substring(index, endIndex - index);
                string[] strs = subStr.Split('\'');
                if (strs.Length > 2)
                {
                    string numStr = strs[1].Replace(" ", "");
                    string dateStr = null;
                    strs = strs[2].Split('>');
                    if (strs.Length > 1)
                    {
                        dateStr = strs[strs.Length - 1];
                    }

                    int dateID = int.Parse(dateStr) - 1;
                    string info = dateStr + " " + numStr;
                    if (string.IsNullOrEmpty(numStr))
                        info += "-\n";
                    else
                        info += "\n";
                    lst[dateID] = info;
                }
                strWebContent = strWebContent.Substring(endIndex);
                index = strWebContent.IndexOf(startStr);
                if (index == -1)
                    break;
                strWebContent = strWebContent.Substring(index);
                index = 0;
            }

            validCount = 0;
            for (int i = 0; i < lst.Length; ++i)
            {
                if (string.IsNullOrEmpty(lst[i]))
                {
                    index = i + 1;
                    if (index < 10)
                        lotteryData += "00";
                    else if (index < 100)
                        lotteryData += "0";
                    lotteryData += index.ToString() + " -\n";
                }
                else
                {
                    lotteryData += lst[i];
                    ++validCount;
                }
            }
        }

        static void AnalyzeByCaiBow(string strWebContent, ref string lotteryData, ref int validCount)
        {
            int index = 0;
            string[] lst = new string[120];
            string startIndexStr = "<span class=\"w_15 mr_1 lh_34 ta_cen ds_bl fl\">";
            int startIndexStrL = startIndexStr.Length;
            string endStr = "</span>";
            string startNumStr = "<span class=\"ds_ib mr5\">";
            int startNumStrL = startNumStr.Length;

            int startIndex = strWebContent.IndexOf(startIndexStr);
            while(startIndex != -1)
            {
                string subStr = strWebContent.Substring(startIndex + startIndexStrL);
                int nextStartIndex = subStr.IndexOf(startIndexStr);

                if (subStr[0] >= '0' && subStr[0] <= '9')
                {
                    if (nextStartIndex != -1)
                    {
                        subStr = strWebContent.Substring(startIndex + startIndexStrL, nextStartIndex);
                        strWebContent = strWebContent.Substring(startIndex + startIndexStrL + nextStartIndex);
                    }

                    string indexStr = subStr.Substring(0, 3);
                    index = int.Parse(indexStr);
                    if (index > 0)
                    {
                        index = index - 1;
                        string numStr = "";
                        int tid = subStr.IndexOf(startNumStr);
                        if (tid != -1)
                        {
                            while (tid != -1)
                            {
                                numStr += subStr[tid + startNumStrL];
                                subStr = subStr.Substring(tid + startNumStrL + 1);
                                tid = subStr.IndexOf(startNumStr);
                            }
                            lst[index] = indexStr + " " + numStr + "\n";
                        }
                        else
                        {
                            lst[index] = indexStr + " -\n";
                        }
                    }
                }
                else
                {
                    strWebContent = strWebContent.Substring(startIndex + startIndexStrL + nextStartIndex);
                }
                startIndex = strWebContent.IndexOf(startIndexStr);

                if (nextStartIndex == -1)
                    break;
            }

            index = 0;
            validCount = 0;
            for (int i = 0; i < lst.Length; ++i)
            {
                if (string.IsNullOrEmpty(lst[i]))
                {
                    index = i + 1;
                    if (index < 10)
                        lotteryData += "00";
                    else if (index < 100)
                        lotteryData += "0";
                    lotteryData += index.ToString() + " -\n";
                }
                else
                {
                    lotteryData += lst[i];
                    ++validCount;
                }
            }

        }

        static void AnalyzeBy360(string strWebContent, ref string lotteryData, ref int validCount, bool newUrl)
        {
            if (newUrl)
            {
                // 匹配日期
                MatchCollection mcDates = regexDate.Matches(strWebContent);
                // 匹配号码
                MatchCollection mcNumbers = regexNumber.Matches(strWebContent);
                if (mcDates.Count != mcNumbers.Count)
                {
                    throw new Exception("日期和号码数量对不上！");
                }
                validCount = mcDates.Count;
                if (validCount > 0)
                {
                    string[] lst = new string[120];

                    for (int i = 0; i < validCount; ++i)
                    {
                        string dateStr = mcDates[i].Groups[0].ToString().Split('-')[1];
                        int dateID = int.Parse(dateStr);
                        string numStr = mcNumbers[i].Groups[0].ToString();

                        lst[dateID - 1] = dateStr + " " + numStr + "\n";
                    }

                    for (int i = 0; i < lst.Length; ++i)
                    {
                        if (string.IsNullOrEmpty(lst[i]))
                        {
                            int index = i + 1;
                            if (index < 10)
                                lotteryData += "00";
                            else if (index < 100)
                                lotteryData += "0";
                            lotteryData += index.ToString() + " -\n";
                        }
                        else
                        {
                            lotteryData += lst[i];
                        }
                    }
                }
            }
            else
            {
                htmlDocument.LoadHtml(strWebContent);
                HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectSingleNode("html/body").ChildNodes;
                foreach (HtmlNode wrapNode in collection)
                {
                    if (wrapNode.Name != "div" || wrapNode.GetAttributeValue("class", "") != "wrap")
                        continue;
                    foreach (HtmlNode histTabNode in wrapNode.ChildNodes)
                    {
                        if (histTabNode.GetAttributeValue("class", "") != "history-tab")
                            continue;
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
    }


    // 枚举窗体
    public static class WindowsEnumerator
    {
        private delegate bool EnumWindowsProc(IntPtr windowHandle, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumChildWindows(IntPtr hWndStart, EnumWindowsProc callback, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        private static List<IntPtr> handles = new List<IntPtr>();
        private static string targetName;
        public static List<IntPtr> GetWindowHandles(string target)
        {
            targetName = target;
            EnumWindows(EnumWindowsCallback, IntPtr.Zero);
            return handles;
        }
        private static bool EnumWindowsCallback(IntPtr HWND, IntPtr includeChildren)
        {
            StringBuilder name = new StringBuilder(GetWindowTextLength(HWND) + 1);
            GetWindowText(HWND, name, name.Capacity);
            if (name.ToString() == targetName)
                handles.Add(HWND);
            EnumChildWindows(HWND, EnumWindowsCallback, IntPtr.Zero);
            return true;
        }
    }

    public class WindowUtil
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // hWnd是句柄，factor是透明度0~255
        public static bool MakeWindowTransparent(IntPtr hWnd, byte factor)
        {
            const int GWL_EXSTYLE = (-20);
            const uint WS_EX_LAYERED = 0x00080000;
            int Cur_STYLE = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, (uint)(Cur_STYLE | WS_EX_LAYERED));
            const uint LWA_COLORKEY = 1;
            const uint LWA_ALPHA = 2;
            const uint WHITE = 0xffffff;
            return SetLayeredWindowAttributes(hWnd, WHITE, factor, LWA_COLORKEY | LWA_ALPHA);
        }

        // 所有标题为Form1的都调整（不包括自己，因为此时自己还没有显示，
        // 当初始化完毕后连自己都会调整）
        // 其实这个WindowsEnumerator只是枚举窗口句柄
        // 如果只要某个特定窗口，获取他的窗体句柄就好了
        // MakeWindowTransparent(句柄, 透明度);
        public static void MakeWindowTransparent(string windowName, byte alpha)
        {
            foreach (var item in WindowsEnumerator.GetWindowHandles(windowName))
                MakeWindowTransparent(item, alpha); // 0~255 128是50%透明度
        }
    }
    
    /// <summary>
    /// ini文件类
    /// </summary>
    public class IniFile
    {
        private string m_FileName;

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileInt(
            string lpAppName,
            string lpKeyName,
            int nDefault,
            string lpFileName
            );

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName
            );

        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName
            );

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="aFileName">Ini文件路径</param>
        public IniFile(string aFileName)
        {
            this.m_FileName = aFileName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IniFile()
        { }

        /// <summary>
        /// [扩展]读Int数值
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public int ReadInt(string section, string name, int def)
        {
            return GetPrivateProfileInt(section, name, def, this.m_FileName);
        }

        public float ReadFloat(string section, string name, float def)
        {
            StringBuilder vRetSb = new StringBuilder(2048);
            GetPrivateProfileString(section, name, def.ToString(), vRetSb, 2048, this.m_FileName);
            float res = def;
            if (float.TryParse(vRetSb.ToString(), out res) == false)
                res = def;
            return res;
        }

        public bool ReadBool(string section, string name, bool def)
        {
            StringBuilder vRetSb = new StringBuilder(2048);
            GetPrivateProfileString(section, name, def.ToString(), vRetSb, 2048, this.m_FileName);
            bool res = def;
            if (bool.TryParse(vRetSb.ToString(), out res) == false)
                res = def;
            return res;
        }

        /// <summary>
        /// [扩展]读取string字符串
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public string ReadString(string section, string name, string def)
        {
            StringBuilder vRetSb = new StringBuilder(2048);
            GetPrivateProfileString(section, name, def, vRetSb, 2048, this.m_FileName);
            return vRetSb.ToString();
        }

        /// <summary>
        /// [扩展]写入Int数值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="Ival">写入值</param>
        public void WriteInt(string section, string name, int Ival)
        {

            WritePrivateProfileString(section, name, Ival.ToString(), this.m_FileName);
        }

        public void WriteFloat(string section, string name, float Ival)
        {

            WritePrivateProfileString(section, name, Ival.ToString(), this.m_FileName);
        }

        public void WriteBool(string section, string name, bool val)
        {
            WritePrivateProfileString(section, name, val.ToString(), this.m_FileName);
        }

        /// <summary>
        /// [扩展]写入String字符串，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="strVal">写入值</param>
        public void WriteString(string section, string name, string strVal)
        {
            WritePrivateProfileString(section, name, strVal, this.m_FileName);
        }

        /// <summary>
        /// 删除指定的 节
        /// </summary>
        /// <param name="section"></param>
        public void DeleteSection(string section)
        {
            WritePrivateProfileString(section, null, null, this.m_FileName);
        }

        /// <summary>
        /// 删除全部 节
        /// </summary>
        public void DeleteAllSection()
        {
            WritePrivateProfileString(null, null, null, this.m_FileName);
        }

        /// <summary>
        /// 读取指定 节-键 的值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string IniReadValue(string section, string name)
        {
            StringBuilder strSb = new StringBuilder(256);
            GetPrivateProfileString(section, name, "", strSb, 256, this.m_FileName);
            return strSb.ToString();
        }

        /// <summary>
        /// 写入指定值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void IniWriteValue(string section, string name, string value)
        {
            WritePrivateProfileString(section, name, value, this.m_FileName);
        }
    }

    public class SystemCfg
    {
        static SystemCfg sInst;
        public static SystemCfg Instance
        {
            get
            {
                if (sInst == null)
                    sInst = new SystemCfg();
                return sInst;
            }
        }

        IniFile cfg = new IniFile(Environment.CurrentDirectory + "\\setting.ini");

        SystemCfg()
        {

        }

        public IniFile CFG
        {
            get { return cfg; }
        }
    }

    // 声明  
    public class BeepUp
    {
        /// <param name="iFrequency">声音频率（从37Hz到32767Hz）。在windows95中忽略</param>  
        /// <param name="iDuration">声音的持续时间，以毫秒为单位。</param>  
        [DllImport("Kernel32.dll")] //引入命名空间 using System.Runtime.InteropServices;  
        public static extern bool Beep(int frequency, int duration);
    }


    public interface UpdaterBase
    {
        void OnUpdate();
    }

}
