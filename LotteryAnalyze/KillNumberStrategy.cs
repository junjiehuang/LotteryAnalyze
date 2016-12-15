﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public class AndValueSearchMap
    {
        public class AndValuePairList
        {
            public int count = -1;
            public List<int[]> pairList;
            public AndValuePairList()
            {
                pairList = new List<int[]>();
            }
        }
        public class AndValueGroup
        {
            public int count = -1;
            public Dictionary<GroupType, AndValuePairList> pairList;
            public AndValueGroup()
            {
                pairList = new Dictionary<GroupType, AndValuePairList>();
            }
        }
        public static Dictionary<int, AndValueGroup> sAndValueSearchMap;
        public static int sPairTotal;
        public static int sPairGP1Total;
        public static int sPairGP3Total;
        public static int sPairGP6Total;

        static AndValueSearchMap()
        {
            sAndValueSearchMap = new Dictionary<int, AndValueGroup>();
            SetData(0, 1, 0, 0);
            SetData(1, 0, 1, 0);
            SetData(2, 0, 2, 0);
            SetData(3, 1, 1, 1);
            SetData(4, 0, 3, 1);
            SetData(5, 0, 3, 2);
            SetData(6, 1, 3, 3);
            SetData(7, 0, 4, 4);
            SetData(8, 0, 5, 5);
            SetData(9, 1, 4, 7);
            SetData(10, 0, 5, 8);
            SetData(11, 0, 5, 9);
            SetData(12, 1, 4, 10);
            SetData(13, 0, 5, 10);
            SetData(14, 0, 5, 10);
            SetData(15, 1, 4, 10);
            SetData(16, 0, 5, 9);
            SetData(17, 0, 5, 8);
            SetData(18, 1, 4, 7);
            SetData(19, 0, 5, 5);
            SetData(20, 0, 4, 4);
            SetData(21, 1, 3, 3);
            SetData(22, 0, 3, 2);
            SetData(23, 0, 3, 1);
            SetData(24, 1, 1, 1);
            SetData(25, 0, 2, 0);
            SetData(26, 0, 1, 0);
            SetData(27, 1, 0, 0);
            sPairTotal = 0;
            sPairGP1Total = 0;
            sPairGP3Total = 0;
            sPairGP6Total = 0;
            foreach (AndValueGroup avp in sAndValueSearchMap.Values)
            {
                sPairTotal += avp.count;
                sPairGP1Total += avp.pairList[GroupType.eGT1].count;
                sPairGP3Total += avp.pairList[GroupType.eGT3].count;
                sPairGP6Total += avp.pairList[GroupType.eGT6].count;
            }
        }
        static void SetData(int andValue, int GP1Count, int GP3Count, int GP6Count)
        {
            AndValueGroup avp = null;
            if (sAndValueSearchMap.ContainsKey(andValue))
                avp = sAndValueSearchMap[andValue];
            else
            {
                avp = new AndValueGroup();
                sAndValueSearchMap.Add(andValue, avp);
            }
            avp.count = GP1Count + GP3Count + GP6Count;
            AndValuePairList pl = null;
            if (avp.pairList.ContainsKey(GroupType.eGT1))
                pl = avp.pairList[GroupType.eGT1];
            else
            {
                pl = new AndValuePairList();
                avp.pairList.Add(GroupType.eGT1, pl);
            }
            pl.count = GP1Count;
            if (avp.pairList.ContainsKey(GroupType.eGT3))
                pl = avp.pairList[GroupType.eGT3];
            else
            {
                pl = new AndValuePairList();
                avp.pairList.Add(GroupType.eGT3, pl);
            }
            pl.count = GP3Count;
            if (avp.pairList.ContainsKey(GroupType.eGT6))
                pl = avp.pairList[GroupType.eGT6];
            else
            {
                pl = new AndValuePairList();
                avp.pairList.Add(GroupType.eGT6, pl);
            }
            pl.count = GP6Count;

        }
        static void AddData(int andValue, GroupType gt, int[] pairInfo)
        {
            AndValueGroup avp = null;
            if( sAndValueSearchMap.ContainsKey(andValue) )
                avp = sAndValueSearchMap[andValue];
            else
            {
                avp = new AndValueGroup();
                sAndValueSearchMap.Add(andValue, avp);
            }
            AndValuePairList pl = null;
            if (avp.pairList.ContainsKey(gt))
                pl = avp.pairList[gt];
            else
            {
                pl = new AndValuePairList();
                avp.pairList.Add(gt, pl);
            }
            pl.pairList.Add(pairInfo);
        }
        public static int GetPairCountExcept(int andValue, GroupType gt)
        {
            AndValueGroup avp = sAndValueSearchMap[andValue];
            switch (gt)
            {
                case GroupType.eGT1: return sPairGP1Total - avp.pairList[GroupType.eGT1].count;
                case GroupType.eGT3: return sPairGP3Total - avp.pairList[GroupType.eGT3].count;
                case GroupType.eGT6: return sPairGP6Total - avp.pairList[GroupType.eGT6].count;
            }
            return 0;
        }
    }


    public class KillNumberStrategy
    {
        public bool active = false;
        virtual public string DESC() { return ""; }
        virtual public void KillNumber(DataItem item, ref List<int> killList) {}
    }

    public class KillNumberStrategyManager
    {
        public Dictionary<string, KillNumberStrategy> funcList = new Dictionary<string, KillNumberStrategy>();

        KillNumberStrategyManager()
        {
            funcList.Add("杀上2期非重邻号", new KillNumberByLast2ReverseRepeatRelateNum());
            funcList.Add("杀上期非重邻号", new KillNumberByLastReverseRepeatRelateNum());
            funcList.Add("和值杀号", new KillNumberByAndValue());
            funcList.Add("杀上期合值", new KillNumberByLastRearValue());
            //funcList.Add("跨度杀号", new KillNumberByCrossValue());
            funcList.Add("杀上期出的号", new KillNumberByReverseSelect());
            funcList.Add("杀期号个位", new KillNumberByDateValue());
        }

        static KillNumberStrategyManager sInst = null;
        public static KillNumberStrategyManager GetInst()
        {
            if (sInst == null)
                sInst = new KillNumberStrategyManager();
            return sInst;
        }

        public void KillNumber(DataItem item, ref List<int> killList)
        {
            foreach (KillNumberStrategy strategy in funcList.Values)
            {
                if (strategy != null && strategy.active)
                {
                    strategy.KillNumber(item, ref killList);
                }
            }
        }
    }

    public class KillNumberByDateValue : KillNumberStrategy
    {
        static string sDesc = "把当期序号的个位数杀掉。";
        public override string DESC() { return sDesc; }

        public static string GetTypeName()
        {
            return typeof(KillNumberByDateValue).ToString();
        }
        public override void KillNumber(DataItem item, ref List<int> killList)
        {
            int dateGeValue = Util.GetNumberByPos(item.id, 0);
            if (killList.Contains(dateGeValue) == false)
                killList.Add(dateGeValue);
            //int dateShiValue = Util.GetNumberByPos(item.id, 1);
            //if (killList.Contains(dateShiValue) == false)
            //    killList.Add(dateShiValue);
        }
    }

    public class KillNumberByAndValue : KillNumberStrategy
    {
        public static string GetTypeName()
        {
            return typeof(KillNumberByAndValue).ToString();
        }
        public override void KillNumber(DataItem item, ref List<int> killList)
        {
            item.simData.killAndValue = -1;
            DataItem prevItem = DataManager.GetInst().GetPrevItem(item);
            if (prevItem == null)
                return;
            if (item.simData.killType == KillType.eKTGroup3)
            {
                item.simData.killAndValue = prevItem.andValue;
                item.simData.killAndValueAtGroup = GroupType.eGT3;
            }
            else if (item.simData.killType == KillType.eKTGroup6)
            {
                item.simData.killAndValue = prevItem.andValue;
                item.simData.killAndValueAtGroup = GroupType.eGT6;
            }
        }
    }

    public class KillNumberByLastRearValue : KillNumberStrategy
    {
        static string sDesc = "把上期出号的合值杀掉。";
        public override string DESC() { return sDesc; }

        public static string GetTypeName()
        {
            return typeof(KillNumberByLastRearValue).ToString();
        }
        public override void KillNumber(DataItem item, ref List<int> killList)
        {
            DataItem prevItem = DataManager.GetInst().GetPrevItem(item);
            if (prevItem == null)
                return;
            if (killList.Contains(prevItem.rearValue) == false)
                killList.Add(prevItem.rearValue);
        }
    }

    //public class KillNumberByCrossValue : KillNumberStrategy
    //{
    //    public static string GetTypeName()
    //    {
    //        return typeof(KillNumberByCrossValue).ToString();
    //    }
    //    public override void KillNumber(DataItem item, ref List<int> killList)
    //    {

    //    }
    //}

    public class KillNumberByReverseSelect : KillNumberStrategy
    {
        static string sDesc = "把上期出的各位数字杀掉。";
        public override string DESC() { return sDesc; }

        public static string GetTypeName()
        {
            return typeof(KillNumberByReverseSelect).ToString();
        }
        public override void KillNumber(DataItem item, ref List<int> killList)
        {
            DataItem prevItem = DataManager.GetInst().GetPrevItem(item);
            if (prevItem == null)
                return;
            int ge = prevItem.GetGeNumber();
            int shi = prevItem.GetShiNumber();
            int bai = prevItem.GetBaiNumber();
            if (killList.Contains(ge) == false)
                killList.Add(ge);
            if (killList.Contains(shi) == false)
                killList.Add(shi);
            if (killList.Contains(bai) == false)
                killList.Add(bai);
        }
    }

    public class KillNumberByLast2ReverseRepeatRelateNum : KillNumberStrategy
    {
        static string sDesc = "杀上2期的重邻号之外的号";
        public override string DESC() { return sDesc; }
        static List<int> sFullList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, };

        public static string GetTypeName()
        {
            return typeof(KillNumberByLast2ReverseRepeatRelateNum).ToString();
        }

        public override void KillNumber(DataItem item, ref List<int> killList)
        {
            DataItem prevItem = DataManager.GetInst().GetPrevItem(item);
            if (prevItem == null)
                return;
            killList.AddRange(sFullList);
            KillNumByItem(prevItem, ref killList);
            DataItem prevPrevItem = DataManager.GetInst().GetPrevItem(prevItem);
            if( prevPrevItem != null )
                KillNumByItem(prevPrevItem, ref killList);
        }
        void KillNum(ref List<int> killList, int num)
        {
            if (killList.Contains(num))
                killList.Remove(num);
            int preNum = num - 1;
            if (preNum < 0)
                preNum = 9;
            if (killList.Contains(preNum))
                killList.Remove(preNum);
            int nexNum = num + 1;
            if (nexNum > 9)
                nexNum = 0;
            if (killList.Contains(nexNum))
                killList.Remove(nexNum);
        }
        void KillNumByItem(DataItem item, ref List<int> killList)
        {
            int ge = item.GetGeNumber();
            int shi = item.GetShiNumber();
            int bai = item.GetBaiNumber();
            KillNum(ref killList, ge);
            KillNum(ref killList, shi);
            KillNum(ref killList, bai);
        }
    }

    public class KillNumberByLastReverseRepeatRelateNum : KillNumberStrategy
    {
        static string sDesc = "杀上期的重邻号之外的号";
        public override string DESC() { return sDesc; }
        static List<int> sFullList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, };

        public static string GetTypeName()
        {
            return typeof(KillNumberByLastReverseRepeatRelateNum).ToString();
        }

        public override void KillNumber(DataItem item, ref List<int> killList)
        {
            DataItem prevItem = DataManager.GetInst().GetPrevItem(item);
            if (prevItem == null)
                return;
            killList.AddRange(sFullList);
            KillNumByItem(prevItem, ref killList);
        }
        void KillNum(ref List<int> killList, int num)
        {
            if (killList.Contains(num))
                killList.Remove(num);
            int preNum = num - 1;
            if (preNum < 0)
                preNum = 9;
            if (killList.Contains(preNum))
                killList.Remove(preNum);
            int nexNum = num + 1;
            if (nexNum > 9)
                nexNum = 0;
            if (killList.Contains(nexNum))
                killList.Remove(nexNum);
        }
        void KillNumByItem(DataItem item, ref List<int> killList)
        {
            int ge = item.GetGeNumber();
            int shi = item.GetShiNumber();
            int bai = item.GetBaiNumber();
            KillNum(ref killList, ge);
            KillNum(ref killList, shi);
            KillNum(ref killList, bai);
        }
    }
}
