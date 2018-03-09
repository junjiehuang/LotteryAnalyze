using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LotteryAnalyze;

namespace LotteryAnalyze.UI
{
    public partial class TradeWindow : Form
    {
        List<CheckBox> cbw = new List<CheckBox>();
        List<CheckBox> cbq = new List<CheckBox>();
        List<CheckBox> cbb = new List<CheckBox>();
        List<CheckBox> cbs = new List<CheckBox>();
        List<CheckBox> cbg = new List<CheckBox>();

        List<SByte> wSels = new List<SByte>();
        List<SByte> qSels = new List<SByte>();
        List<SByte> bSels = new List<SByte>();
        List<SByte> sSels = new List<SByte>();
        List<SByte> gSels = new List<SByte>();

        GraphManager graphMgr = null;


        public TradeWindow(object _graphMgr)
        {
            graphMgr = _graphMgr as GraphManager;
            InitializeComponent();
            cbw.Add(checkBoxW0); cbw.Add(checkBoxW1); cbw.Add(checkBoxW2); cbw.Add(checkBoxW3); cbw.Add(checkBoxW4);
            cbw.Add(checkBoxW5); cbw.Add(checkBoxW6); cbw.Add(checkBoxW7); cbw.Add(checkBoxW8); cbw.Add(checkBoxW9);

            cbq.Add(checkBoxQ0); cbq.Add(checkBoxQ1); cbq.Add(checkBoxQ2); cbq.Add(checkBoxQ3); cbq.Add(checkBoxQ4);
            cbq.Add(checkBoxQ5); cbq.Add(checkBoxQ6); cbq.Add(checkBoxQ7); cbq.Add(checkBoxQ8); cbq.Add(checkBoxQ9);

            cbb.Add(checkBoxB0); cbb.Add(checkBoxB1); cbb.Add(checkBoxB2); cbb.Add(checkBoxB3); cbb.Add(checkBoxB4);
            cbb.Add(checkBoxB5); cbb.Add(checkBoxB6); cbb.Add(checkBoxB7); cbb.Add(checkBoxB8); cbb.Add(checkBoxB9);

            cbs.Add(checkBoxS0); cbs.Add(checkBoxS1); cbs.Add(checkBoxS2); cbs.Add(checkBoxS3); cbs.Add(checkBoxS4);
            cbs.Add(checkBoxS5); cbs.Add(checkBoxS6); cbs.Add(checkBoxS7); cbs.Add(checkBoxS8); cbs.Add(checkBoxS9);

            cbg.Add(checkBoxG0); cbg.Add(checkBoxG1); cbg.Add(checkBoxG2); cbg.Add(checkBoxG3); cbg.Add(checkBoxG4);
            cbg.Add(checkBoxG5); cbg.Add(checkBoxG6); cbg.Add(checkBoxG7); cbg.Add(checkBoxG8); cbg.Add(checkBoxG9);
        }

        private void buttonWClear_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < cbw.Count; ++i )
            {
                cbw[i].Checked = false;
            }
        }

        private void buttonQClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbq.Count; ++i)
            {
                cbq[i].Checked = false;
            }
        }

        private void buttonBClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbb.Count; ++i)
            {
                cbb[i].Checked = false;
            }
        }

        private void buttonSClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbs.Count; ++i)
            {
                cbs[i].Checked = false;
            }
        }

        private void buttonGClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cbg.Count; ++i)
            {
                cbg[i].Checked = false;
            }
        }

        void CheckSelectNum(ref List<CheckBox> cbLst, ref List<SByte> numLst)
        {
            numLst.Clear();
            for(int i = 0; i < cbLst.Count; ++i)
            {
                if(cbLst[i].Checked)
                {
                    numLst.Add((SByte)i);
                }
            }
        }

        string GetListInfoString(List<SByte> lst)
        {
            string info = "";
            for (int i = 0; i < lst.Count; ++i)
            {
                info += lst[i] + ", ";
            }
            return info;
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            string buyInfo = "";
            string info = "确定购买？";
            string caption = "提示";
            //DialogResult dr = MessageBox.Show(info,caption,MessageBoxButtons.OKCancel);
            //if(dr == DialogResult.OK)
            {
                DataItem lastItem = DataManager.GetInst().GetLatestItem();
                if (graphMgr.endShowDataItemIndex >= 0)
                    lastItem = DataManager.GetInst().FindDataItem(graphMgr.endShowDataItemIndex);
                
                int tradeCount = 1;
                int.TryParse(textBoxLotteryCount.Text, out tradeCount);
                wSels.Clear(); CheckSelectNum(ref cbw, ref wSels);
                qSels.Clear(); CheckSelectNum(ref cbq, ref qSels);
                bSels.Clear(); CheckSelectNum(ref cbb, ref bSels);
                sSels.Clear(); CheckSelectNum(ref cbs, ref sSels);
                gSels.Clear(); CheckSelectNum(ref cbg, ref gSels);
                if (wSels.Count > 0)
                    buyInfo += "万位 ： " + GetListInfoString(wSels) + "\n";
                if (qSels.Count > 0)
                    buyInfo += "千位 ： " + GetListInfoString(qSels) + "\n";
                if (bSels.Count > 0)
                    buyInfo += "百位 ： " + GetListInfoString(bSels) + "\n";
                if (sSels.Count > 0)
                    buyInfo += "十位 ： " + GetListInfoString(sSels) + "\n";
                if (gSels.Count > 0)
                    buyInfo += "个位 ： " + GetListInfoString(gSels) + "\n";
                buyInfo += "数量 ： " + tradeCount + "\n" + info;

                DialogResult dr = MessageBox.Show(buyInfo, caption, MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    bool hasSelNum = wSels.Count > 0 || qSels.Count > 0 || bSels.Count > 0 || sSels.Count > 0 || gSels.Count > 0;
                    if (tradeCount >= 0 && hasSelNum)
                    {
                        TradeDataOneStar trade = TradeDataManager.Instance.NewTrade(TradeType.eOneStar) as TradeDataOneStar;
                        trade.lastDateItem = lastItem;
                        if (wSels.Count > 0)
                        {
                            List<NumberCmpInfo> nci = new List<NumberCmpInfo>();
                            TradeDataManager.FindOverTheoryProbabilityNums(lastItem, 0, ref nci);
                            trade.AddSelNum(0, ref wSels, tradeCount, ref nci);
                        }
                        if (qSels.Count > 0)
                        {
                            List<NumberCmpInfo> nci = new List<NumberCmpInfo>();
                            TradeDataManager.FindOverTheoryProbabilityNums(lastItem, 1, ref nci);
                            trade.AddSelNum(1, ref qSels, tradeCount, ref nci);
                        }
                        if (bSels.Count > 0)
                        {
                            List<NumberCmpInfo> nci = new List<NumberCmpInfo>();
                            TradeDataManager.FindOverTheoryProbabilityNums(lastItem, 2, ref nci);
                            trade.AddSelNum(2, ref bSels, tradeCount, ref nci);
                        }
                        if (sSels.Count > 0)
                        {
                            List<NumberCmpInfo> nci = new List<NumberCmpInfo>();
                            TradeDataManager.FindOverTheoryProbabilityNums(lastItem, 3, ref nci);
                            trade.AddSelNum(3, ref sSels, tradeCount, ref nci);
                        }
                        if (gSels.Count > 0)
                        {
                            List<NumberCmpInfo> nci = new List<NumberCmpInfo>();
                            TradeDataManager.FindOverTheoryProbabilityNums(lastItem, 4, ref nci);
                            trade.AddSelNum(4, ref gSels, tradeCount, ref nci);
                        }
                    }
                }
            }
        }
    }
}
