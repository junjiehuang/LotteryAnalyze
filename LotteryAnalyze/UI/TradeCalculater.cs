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
        float reward = 9.8f;
        float cost = 1;
        int numCount = 4;


        public TradeCalculater()
        {
            InitializeComponent();

            textBoxCost.Text = cost.ToString();
            textBoxNumCount.Text = numCount.ToString();
            textBoxReward.Text = reward.ToString();
            textBoxTradeSlu.Text = "";
            textBoxResult.Text = "";
        }

        private void buttonStartCalc_Click(object sender, EventArgs e)
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
            for( int i = 0; i < slus.Length; ++i )
            {
                if (string.IsNullOrEmpty(slus[i]))
                    continue;
                int slu = int.Parse(slus[i]);
                totalCost += slu * numCount * cost;
                totalReward = slu * reward;
                profit = totalReward - totalCost;
                info += "[" + validIndex + "] "+ slus[i] + "\t(成本: " + totalCost.ToString("f2") + ")\t(奖金: " + totalReward.ToString("f2") + ")\t(获利: " + profit.ToString("f2") + ")\r\n";
                ++validIndex;
            }
            textBoxResult.Text = info;
        }
    }
}
