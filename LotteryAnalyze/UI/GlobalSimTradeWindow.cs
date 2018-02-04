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
                string info = "";
                info += ("结束任务!\r\n---------------------\r\n");
                info += ("[当前金额 - ");
                info += (BatchTradeSimulator.Instance.currentMoney);
                info += ("] [最高金额 - ");
                info += (BatchTradeSimulator.Instance.maxMoney);
                info += ("] [最低金额 - ");
                info += (BatchTradeSimulator.Instance.minMoney);
                info += ("]\r\n---------------------\r\n");
                info += ("[总次数 - ");
                info += (BatchTradeSimulator.Instance.totalCount);
                info += ("] [对次数 - ");
                info += (BatchTradeSimulator.Instance.tradeRightCount);
                info += ("] [错次数 - ");
                info += (BatchTradeSimulator.Instance.tradeWrongCount);
                info += ("] [忽略次数 - ");
                info += (BatchTradeSimulator.Instance.untradeCount);
                info += ("]\r\n");

                textBoxCmd.Text = info;
            }
            else if (BatchTradeSimulator.Instance.HasJob())
            {
                DataItem fItem = DataManager.GetInst().GetFirstItem();
                DataItem lItem = DataManager.GetInst().GetLatestItem();
                DataItem cItem = TradeDataManager.Instance.GetLatestTradedDataItem();

                string info = "";
                info += ("[当前进度 - ");
                info += (progressBarCurrent.Value);
                info += ("] [总进度 - ");
                info += (progressBarTotal.Value);
                info += ("]\r\n---------------------\r\n");
                if (fItem != null && lItem != null && cItem != null)
                {
                    info += ("[首期 - ");
                    info += (fItem.idTag);
                    info += ("] [末期 - ");
                    info += (lItem.idTag);
                    info += ("] [当期 - ");
                    info += (cItem.idTag);
                    info += ("]\r\n---------------------\r\n");
                }
                info += ("[当前金额 - ");
                info += (BatchTradeSimulator.Instance.currentMoney);
                info += ("] [最高金额 - ");
                info += (BatchTradeSimulator.Instance.maxMoney);
                info += ("] [最低金额 - ");
                info += (BatchTradeSimulator.Instance.minMoney);
                info += ("]\r\n---------------------\r\n");
                info += ("[总次数 - ");
                info += (BatchTradeSimulator.Instance.totalCount);
                info += ("] [对次数 - ");
                info += (BatchTradeSimulator.Instance.tradeRightCount);
                info += ("] [错次数 - ");
                info += (BatchTradeSimulator.Instance.tradeWrongCount);
                info += ("] [忽略次数 - ");
                info += (BatchTradeSimulator.Instance.untradeCount);
                info += ("]\r\n");

                textBoxCmd.Text = info;
                info = null;
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
