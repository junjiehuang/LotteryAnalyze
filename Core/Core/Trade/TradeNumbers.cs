using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class TradeNumbers
    {
        public List<NumberCmpInfo> tradeNumbers = new List<NumberCmpInfo>();
        public int tradeCount = 0;

        public void GetInfo(ref string info)
        {
            if (tradeNumbers.Count > 0)
            {
                int index = TradeDataManager.Instance.tradeCountList.IndexOf(tradeCount);

                info += "[" + index + ", " + tradeCount + "] {";
                for (int i = 0; i < tradeNumbers.Count; ++i)
                {
                    info += tradeNumbers[i].ToString();
                    if (i != tradeNumbers.Count - 1)
                        info += ",";
                }
                info += "}\n";
            }
        }
        public void SelPath012Number(int path, int tradeCount, ref List<NumberCmpInfo> nums)
        {
            this.tradeCount = tradeCount;
            for (int i = 0; i < nums.Count; ++i)
            {
                if (nums[i].number % 3 == path && ContainsNumber(nums[i].number) == false)
                {
                    tradeNumbers.Add(nums[i]);
                }
            }
        }
        public bool ContainsNumber(SByte number)
        {
            for (int i = 0; i < tradeNumbers.Count; ++i)
            {
                if (tradeNumbers[i].number == number)
                    return true;
            }
            return false;
        }
        public void SetMaxProbabilityNumber(int tradeCount, ref List<NumberCmpInfo> nums, bool needGetLessProbabilityNum, int maxNumCount)
        {
            this.tradeCount = tradeCount;
            int count = 0;
            for (int i = 0; i < nums.Count; ++i)
            {
                if (nums[i].largerThanTheoryProbability || needGetLessProbabilityNum)
                {
                    ++count;
                    tradeNumbers.Add(nums[i]);
                    if (count == maxNumCount)
                        break;
                }
            }
        }
        public void SetHotNumber(int tradeCount, ref List<NumberCmpInfo> nums)
        {
            int count = 0;
            for (int i = 0; i < nums.Count; ++i)
            {
                if (nums[i].rate > 0.5f)
                {
                    tradeNumbers.Add(nums[i]);
                    ++count;
                }
            }
            this.tradeCount = tradeNumbers.Count > 0 ? tradeCount : 0;
        }
        public void AddProbabilityNumber(NumberCmpInfo nci)
        {
            tradeNumbers.Add(nci);
        }
        public void AddProbabilityNumber(ref List<NumberCmpInfo> nums, int num)
        {
            for (int i = 0; i < nums.Count; ++i)
            {
                if (nums[i].number == num && tradeNumbers.Contains(nums[i]) == false)
                {
                    tradeNumbers.Add(nums[i]);
                    break;
                }
            }
        }
        public void RemoveNumber(SByte number)
        {
            for (int i = 0; i < tradeNumbers.Count; ++i)
            {
                if (tradeNumbers[i].number == number)
                {
                    tradeNumbers.RemoveAt(i);
                }
            }
        }
    }
}
