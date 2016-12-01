using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LotteryAnalyze
{
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

        public static int GetGroupType(string str)
        {
            int curId = str.Length - 1;
            int ge = CharValue(str[curId]); curId--;
            int shi = CharValue(str[curId]); curId--;
            int bai = CharValue(str[curId]); curId--;
            if (ge == shi && ge == bai)
                return 1;
            if (ge == shi || ge == bai || shi == bai)
                return 2;
            return 3;
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

        public static int GetCost(int numCount, int ratio)
        {
            int containsCount = 10 - numCount;
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
            return 0;
        }

        public static int GetReward(int groupID, int ratio)
        {
            switch (groupID)
            {
                case 1: return 0;
                case 2: return 576 * ratio;
                case 3: return 288 * ratio;
            }
            return 0;
        }

        public static bool SimKillNumberAndCheckResult(DataItem item, int ratio)
        {
            List<int> killNums = new List<int>();
            KillNumberStrategyManager.GetInst().KillNumber(item, ref killNums);
            bool isRight = (item.groupType == 3);
            if (isRight)
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
            item.simData.isPredictRight = isRight;
            item.simData.reward = 0;
            item.simData.cost = GetCost(killNums.Count, ratio);
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
            }
            DataManager.GetInst().curProfit += -item.simData.cost + item.simData.reward;
            item.simData.profit = DataManager.GetInst().curProfit;
            return isRight;
        }
    }
}
