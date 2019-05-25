using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze.UI
{
    public partial class TradeCalculater : Form
    {
        public enum GenerateType
        {
            eMinCost,
            eFixedProfit,
            eFixedScaleCount,
        }
        public static string[] GenerateTypeStr = new string[]
        {
            "按总成本最小计算",
            "按每次中出有固定收益计算",
            "按以上次注数乘以指定倍率计算",
        };


        float reward = GlobalSetting.G_ONE_STARE_TRADE_REWARD;
        float cost = GlobalSetting.G_ONE_STARE_TRADE_COST;
        int numCount = 4;
        static TradeCalculater sInst;
        int planCount = 0;

        public static TradeCalculater Instance
        {
            get { return sInst; }
        }

        public static void Open()
        {
            if(sInst == null)
                sInst = new TradeCalculater();
            sInst.Show();
        }

        TradeCalculater()
        {
            sInst = this;
            InitializeComponent();

            textBoxCost.Text = cost.ToString();
            textBoxNumCount.Text = numCount.ToString();
            textBoxReward.Text = reward.ToString();
            textBoxTradeSlu.Text = "";
            textBoxResult.Text = "";

            textBoxParam.Text = "";
            comboBoxGenerateType.DataSource = GenerateTypeStr;
            comboBoxGenerateType.SelectedIndex = 0;

            FormMain.AddWindow(this);
        }

        private void buttonStartCalc_Click(object sender, EventArgs e)
        {
            CalcDetail();
        }

        private void TradeCalculater_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMain.RemoveWindow(this);
            sInst = null;
        }

        private void buttonCalcByCount_Click(object sender, EventArgs e)
        {
            GenerateType t = (GenerateType)comboBoxGenerateType.SelectedIndex;
            switch(t)
            {
                case GenerateType.eMinCost:
                    CalcByGenerateTypeMinCost();
                    break;
                case GenerateType.eFixedProfit:
                    CalcByGenerateTypeFixedProfit();
                    break;
                case GenerateType.eFixedScaleCount:
                    CalcByGenerateTypeFixedScaleCount();
                    break;
            }
        }

        private void textBoxCount_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBoxCount.Text, out planCount);
        }

        void CalcDetail()
        {
            reward = float.Parse(textBoxReward.Text);
            cost = float.Parse(textBoxCost.Text);
            numCount = int.Parse(textBoxNumCount.Text);
            float totalCost = 0;
            float totalReward = 0;
            float profit = 0;
            if (string.IsNullOrEmpty(textBoxTradeSlu.Text))
                return;
            string[] slus = textBoxTradeSlu.Text.Split(',');
            string info = "";
            int validIndex = 1;
            for (int i = 0; i < slus.Length; ++i)
            {
                if (string.IsNullOrEmpty(slus[i]))
                    continue;
                int slu = int.Parse(slus[i]);
                totalCost += slu * numCount * cost;
                totalReward = slu * reward;
                profit = totalReward - totalCost;
                info += "[" + validIndex + "] " + slus[i] + "\t(成本: " + totalCost.ToString("f2") + ")\t(奖金: " + totalReward.ToString("f2") + ")\t(获利: " + profit.ToString("f2") + ")\r\n";
                ++validIndex;
            }
            textBoxResult.Text = info;
        }
        void CalcByGenerateTypeMinCost()
        {
            if (planCount == 0)
                return;
            List<int> plans = new List<int>();
            if (string.IsNullOrEmpty(textBoxTradeSlu.Text))
                plans.Add(1);
            else
            {
                string[] slus = textBoxTradeSlu.Text.Split(',');
                for (int i = 0; i < slus.Length; ++i)
                {
                    if (string.IsNullOrEmpty(slus[i]))
                        continue;
                    int slu = int.Parse(slus[i]);
                    plans.Add(slu);
                }
            }
            string planStr = "";
            int validIndex = 1;
            float totalCost = 0;
            float totalReward = 0;
            float profit = 0;
            int noCalcID = plans.Count;
            float batchCost = cost * numCount;
            for (int i = 0; i < planCount; ++i)
            {
                if (i < noCalcID)
                {
                    totalCost += batchCost * plans[i];
                }
                else
                {
                    int dst = (int)Math.Ceiling(totalCost / (reward - batchCost));
                    plans.Add(dst);
                    totalCost += dst * batchCost;
                }
            }
            for (int i = 0; i < plans.Count; ++i)
            {
                planStr += plans[i];
                if (i < plans.Count - 1)
                    planStr += ",";
            }
            textBoxTradeSlu.Text = planStr;

            CalcDetail();
        }
        void CalcByGenerateTypeFixedProfit()
        {
            if (planCount == 0)
                return;
            List<int> plans = new List<int>();
            if (string.IsNullOrEmpty(textBoxTradeSlu.Text))
                plans.Add(1);
            else
            {
                string[] slus = textBoxTradeSlu.Text.Split(',');
                for (int i = 0; i < slus.Length; ++i)
                {
                    if (string.IsNullOrEmpty(slus[i]))
                        continue;
                    int slu = int.Parse(slus[i]);
                    plans.Add(slu);
                }
            }
            float fixedProfit = 0;
            float.TryParse(textBoxParam.Text, out fixedProfit);
            string planStr = "";
            int validIndex = 1;
            float totalCost = 0;
            float totalReward = 0;
            float profit = 0;
            int noCalcID = plans.Count;
            float batchCost = cost * numCount;
            for (int i = 0; i < planCount; ++i)
            {
                if (i < noCalcID)
                {
                    totalCost += batchCost * plans[i];
                }
                else
                {
                    int dst = (int)Math.Ceiling((totalCost + fixedProfit)  / (reward - batchCost));
                    plans.Add(dst);
                    totalCost += dst * batchCost;
                }
            }
            for (int i = 0; i < plans.Count; ++i)
            {
                planStr += plans[i];
                if (i < plans.Count - 1)
                    planStr += ",";
            }
            textBoxTradeSlu.Text = planStr;

            CalcDetail();
        }
        void CalcByGenerateTypeFixedScaleCount()
        {
            if (planCount == 0)
                return;
            List<int> plans = new List<int>();
            if (string.IsNullOrEmpty(textBoxTradeSlu.Text))
                plans.Add(1);
            else
            {
                string[] slus = textBoxTradeSlu.Text.Split(',');
                for (int i = 0; i < slus.Length; ++i)
                {
                    if (string.IsNullOrEmpty(slus[i]))
                        continue;
                    int slu = int.Parse(slus[i]);
                    plans.Add(slu);
                }
            }
            float scale = 1;
            float.TryParse(textBoxParam.Text, out scale);
            string planStr = "";
            int noCalcID = plans.Count;
            for (int i = 0; i < planCount; ++i)
            {
                if (i < noCalcID)
                {
                }
                else
                {
                    int dst = (int)Math.Ceiling(plans[i-1] * scale);
                    plans.Add(dst);
                }
            }
            for (int i = 0; i < plans.Count; ++i)
            {
                planStr += plans[i];
                if (i < plans.Count - 1)
                    planStr += ",";
            }
            textBoxTradeSlu.Text = planStr;

            CalcDetail();
        }
    }
}
