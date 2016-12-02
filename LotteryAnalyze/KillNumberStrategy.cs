﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
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
            funcList.Add("杀期号个位", new KillNumberByDateValue());
            //funcList.Add("和值杀号", new KillNumberByAndValue());
            funcList.Add("杀上期合值", new KillNumberByLastRearValue());
            //funcList.Add("跨度杀号", new KillNumberByCrossValue());
            funcList.Add("杀上期出的号", new KillNumberByReverseSelect());
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
            //int dateShiValue = Util.GetNumberByPos(item.id, 1);
            if (killList.Contains(dateGeValue) == false)
                killList.Add(dateGeValue);
            //if (killList.Contains(dateShiValue) == false)
            //    killList.Add(dateShiValue);
        }
    }

    //public class KillNumberByAndValue : KillNumberStrategy
    //{
    //    public static string GetTypeName()
    //    {
    //        return typeof(KillNumberByAndValue).ToString();
    //    }
    //    public override void KillNumber(DataItem item, ref List<int> killList)
    //    {

    //    }
    //}

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
}
