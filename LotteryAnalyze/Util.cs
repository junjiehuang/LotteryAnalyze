using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LotteryAnalyze
{
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
                string[] strs = line.Split( '\t' );
                if (strs.Length > 0)
                {
                    DataItem item = new DataItem();
                    item.id = int.Parse(strs[0]);
                    item.lotteryNumber = strs[1];
                    item.idTag = fileID.ToString() + "-" + strs[0];
                    item.andValue = Util.CalAndValue(item.lotteryNumber);
                    item.rearValue = Util.CalRearValue(item.lotteryNumber);
                    item.crossValue = Util.CalCrossValue(item.lotteryNumber);
                    item.groupType = Util.GetGroupType(item.lotteryNumber);
                    item.parent = datas;
                    item.GetValuesInThreePos();
                    datas.datas.Add(item);
                }
            }
            sr.Close();
            return true;
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

        public static TestResultType SimKillBlendGroup(DataItem item, int ratio)
        {
            TestResultType curResult = TestResultType.eTRTIgnore;
            if (Simulator.curKillType == KillType.eKTGroup6)
            {
                curResult = SimKillNumberAndCheckResult(item, ratio);
                if (curResult == TestResultType.eTRTFailed || item.groupType == GroupType.eGT1)
                {
                    Simulator.g6Round++;
                    if (Simulator.g6Round > 2 || item.groupType == GroupType.eGT1)
                    {
                        Simulator.curKillType = KillType.eKTGroup3;
                        Simulator.g3Round = 0;
                    }
                }
            }
            else if (Simulator.curKillType == KillType.eKTGroup3)
            {
                curResult = SimKillGroup3OnGroup1Out(item, ratio);
                if (curResult == TestResultType.eTRTFailed)
                {
                    Simulator.g3Round++;
                    if (Simulator.g3Round > 2)
                    {
                        Simulator.curKillType = KillType.eKTGroup6;
                        Simulator.g6Round = 0;
                    }
                }
            }
            if (curResult == TestResultType.eTRTSuccess)
            {
                Simulator.g3Round = Simulator.g6Round = 0;
            }
            return curResult;
        }

        public static TestResultType SimKillGroup3OnGroup1Out(DataItem item, int ratio)
        {
            DataItem prevItem = DataManager.GetInst().GetPrevItem(item);
            if (prevItem == null)
                return TestResultType.eTRTIgnore;
            if (prevItem.groupType == GroupType.eGT1 || Simulator.isCurKillGroup3 || Simulator.curKillType == KillType.eKTGroup3)
            {
                if (prevItem.groupType == GroupType.eGT1 )
                    Simulator.isCurKillGroup3 = true;
                item.simData.killType = KillType.eKTGroup3;
                item.simData.cost = GetCost(0, ratio, GroupType.eGT3);
                item.parent.simData.costTotal += item.simData.cost;
                item.parent.simData.predictCount++;
                DataManager.GetInst().simData.costTotal += item.simData.cost;
                DataManager.GetInst().simData.predictCount++;
                if (item.groupType == GroupType.eGT3)
                {
                    Simulator.isCurKillGroup3 = false;
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

        public static TestResultType SimKillNumberAndCheckResult(DataItem item, int ratio)
        {
            List<int> killNums = new List<int>();
            KillNumberStrategyManager.GetInst().KillNumber(item, ref killNums);
            item.simData.killList = "";
            for (int i = 0; i < killNums.Count; ++i)
            {
                item.simData.killList += killNums[i] + ",";
            }
            bool isRight = true;
            if (item.groupType == GroupType.eGT6)
            {
                for (int i = 0; i < killNums.Count; ++i)
                {
                    int killNum = killNums[i];
                    // kill wrong number
                    if (item.valuesInThreePos.IndexOf(killNum) != -1)
                    {
                        isRight = false;
                        break;
                    }
                }
            }
            else
                isRight = false;

            item.simData.killType = KillType.eKTGroup6;
            item.simData.predictResult = isRight ? TestResultType.eTRTSuccess : TestResultType.eTRTFailed;
            item.simData.reward = 0;
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
    }
}
