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
    public partial class GlobalSimTradeWindow : Form
    {
        static GlobalSimTradeWindow sInst;
        System.Windows.Forms.Timer updateTimer;

        public GlobalSimTradeWindow()
        {
            InitializeComponent();

            textBoxDayCountPerBatch.Text = BatchTradeSimulator.Instance.batch.ToString();
            textBoxStartMoney.Text = BatchTradeSimulator.Instance.startMoney.ToString();
            textBoxTradeCountLst.Text = TradeDataManager.Instance.GetTradeCountInfoStr();

            updateTimer = new Timer();
            updateTimer.Interval = 500;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            BatchTradeSimulator.Instance.Update();

            progressBarCurrent.Value = BatchTradeSimulator.Instance.GetBatchProgress();
            progressBarTotal.Value = BatchTradeSimulator.Instance.GetMainProgress();

            if (BatchTradeSimulator.Instance.HasFinished())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("结束任务!\n---------------------\n");
                sb.Append("[当前金额 - ");
                sb.Append(BatchTradeSimulator.Instance.currentMoney);
                sb.Append("] [最高金额 - ");
                sb.Append(BatchTradeSimulator.Instance.maxMoney);
                sb.Append("] [最低金额 - ");
                sb.Append(BatchTradeSimulator.Instance.minMoney);
                sb.Append("]\n---------------------\n");
                sb.Append("[总次数 - ");
                sb.Append(BatchTradeSimulator.Instance.totalCount);
                sb.Append("] [对次数 - ");
                sb.Append(BatchTradeSimulator.Instance.tradeRightCount);
                sb.Append("] [错次数 - ");
                sb.Append(BatchTradeSimulator.Instance.tradeWrongCount);
                sb.Append("] [忽略次数 - ");
                sb.Append(BatchTradeSimulator.Instance.untradeCount);
                sb.Append("]\n");

                textBoxCmd.Text = sb.ToString();
                sb = null;
            }
            else if (BatchTradeSimulator.Instance.HasJob())
            {
                DataItem fItem = DataManager.GetInst().GetFirstItem();
                DataItem lItem = DataManager.GetInst().GetLatestItem();
                DataItem cItem = TradeDataManager.Instance.GetLatestTradedDataItem();

                StringBuilder sb = new StringBuilder();
                sb.Append("[当前进度 - ");
                sb.Append(progressBarCurrent.Value);
                sb.Append("] [总进度 - ");
                sb.Append(progressBarTotal.Value);
                sb.Append("]\n---------------------\n");
                if (fItem != null && lItem != null && cItem != null)
                {
                    sb.Append("[首期 - ");
                    sb.Append(fItem.idTag);
                    sb.Append("] [末期 - ");
                    sb.Append(lItem.idTag);
                    sb.Append("] [当期 - ");
                    sb.Append(cItem.idTag);
                    sb.Append("]\n---------------------\n");
                }
                sb.Append("[当前金额 - ");
                sb.Append(BatchTradeSimulator.Instance.currentMoney);
                sb.Append("] [最高金额 - ");
                sb.Append(BatchTradeSimulator.Instance.maxMoney);
                sb.Append("] [最低金额 - ");
                sb.Append(BatchTradeSimulator.Instance.minMoney);
                sb.Append("]\n---------------------\n");
                sb.Append("[总次数 - ");
                sb.Append(BatchTradeSimulator.Instance.totalCount);
                sb.Append("] [对次数 - ");
                sb.Append(BatchTradeSimulator.Instance.tradeRightCount);
                sb.Append("] [错次数 - ");
                sb.Append(BatchTradeSimulator.Instance.tradeWrongCount);
                sb.Append("] [忽略次数 - ");
                sb.Append(BatchTradeSimulator.Instance.untradeCount);
                sb.Append("]\n");

                textBoxCmd.Text = sb.ToString();
                sb = null;
            }
            else
                textBoxCmd.Text = "";
        }

        public static void Open()
        {
            if (sInst == null)
                sInst = new GlobalSimTradeWindow();
            sInst.Show();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            BatchTradeSimulator.Instance.batch = int.Parse(textBoxDayCountPerBatch.Text);
            BatchTradeSimulator.Instance.startMoney = float.Parse(textBoxStartMoney.Text);
            TradeDataManager.Instance.SetTradeCountInfo(textBoxTradeCountLst.Text);

            textBoxCmd.ReadOnly = true;
            buttonPauseResume.Text = "暂停";
            BatchTradeSimulator.Instance.Start();
            textBoxDayCountPerBatch.Enabled = false;
            textBoxStartMoney.Enabled = false;
            textBoxTradeCountLst.Enabled = false;

            progressBarCurrent.Value = 0;
            progressBarTotal.Value = 0;
        }

        private void buttonPauseResume_Click(object sender, EventArgs e)
        {
            if (BatchTradeSimulator.Instance.IsPause())
            {
                BatchTradeSimulator.Instance.Resume();
                buttonPauseResume.Text = "暂停";
            }
            else
            {
                BatchTradeSimulator.Instance.Pause();
                buttonPauseResume.Text = "恢复";
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            BatchTradeSimulator.Instance.Stop();
            textBoxDayCountPerBatch.Enabled = true;
            textBoxStartMoney.Enabled = true;
            textBoxTradeCountLst.Enabled = true;
        }

        private void GlobalSimTradeWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            BatchTradeSimulator.Instance.Stop();
            sInst = null;
        }
    }
}
