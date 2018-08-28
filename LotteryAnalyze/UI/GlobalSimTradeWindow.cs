﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace LotteryAnalyze.UI
{
    public enum OperateOnNoMoney
    {
        eNone,
        ePauseTrade,
        eStopTrade,
    }

    public partial class GlobalSimTradeWindow : Form
    {
        static string[] OperateOnNoMoneyAR = {"无操作", "暂停交易模拟", "终止交易模拟", };

        static GlobalSimTradeWindow sInst;
        System.Windows.Forms.Timer updateTimer;
        int startDate = -1;
        int endDate = -1;
        bool stopTradeOnNoMoney = true;

        public void SaveCfg()
        {
            if (checkBoxSpecNumIndex.Checked == false)
                TradeDataManager.Instance.simSelNumIndex = -1;
            comboBoxSpecNumIndex.SelectedIndex = TradeDataManager.Instance.simSelNumIndex;

            SystemCfg.Instance.CFG.WriteInt("SimTrade", "batch", BatchTradeSimulator.Instance.batch);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "startMoney", (int)BatchTradeSimulator.Instance.startMoney);
            string tradeCountLstStr = TradeDataManager.Instance.GetTradeCountInfoStr();
            SystemCfg.Instance.CFG.WriteString("SimTrade", "tradeCountLst", tradeCountLstStr);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "strongUpStartIndex", TradeDataManager.Instance.strongUpStartTradeIndex);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "simSelNumIndex", TradeDataManager.Instance.simSelNumIndex);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "curTradeStrategy", (int)TradeDataManager.Instance.curTradeStrategy);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "forceTradeByMaxNumCount", TradeDataManager.Instance.forceTradeByMaxNumCount ? 1 : 0);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "maxNumCount", TradeDataManager.Instance.maxNumCount);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "specNumIndex", checkBoxSpecNumIndex.Checked ? 1 : 0);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "onlyTradeOnStrongUpPath", checkBoxOnTradeOnStrongUpPath.Checked ? 1 : 0);
            SystemCfg.Instance.CFG.WriteFloat("SimTrade", "riskControl", TradeDataManager.Instance.RiskControl);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "uponValue", TradeDataManager.Instance.uponValue);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "multiPathTradeCount", TradeDataManager.Instance.MultiTradePathCount);

            textBoxRiskControl.Text = TradeDataManager.Instance.RiskControl.ToString();
            textBoxMultiPathTradeCount.Text = TradeDataManager.Instance.MultiTradePathCount.ToString();
        }

        public void ReadCfg()
        {
            BatchTradeSimulator.Instance.batch = SystemCfg.Instance.CFG.ReadInt("SimTrade", "batch", 5);
            BatchTradeSimulator.Instance.startMoney = SystemCfg.Instance.CFG.ReadInt("SimTrade", "startMoney", 2000);
            string tradeCountLstStr = SystemCfg.Instance.CFG.ReadString("SimTrade", "tradeCountLst", "1,2,4,8,16,32,64,128,256,512");
            TradeDataManager.Instance.SetTradeCountInfo(tradeCountLstStr);
            TradeDataManager.Instance.strongUpStartTradeIndex = SystemCfg.Instance.CFG.ReadInt("SimTrade", "strongUpStartIndex", 5);
            TradeDataManager.Instance.simSelNumIndex = SystemCfg.Instance.CFG.ReadInt("SimTrade", "simSelNumIndex", -1);
            TradeDataManager.Instance.curTradeStrategy = (TradeDataManager.TradeStrategy)SystemCfg.Instance.CFG.ReadInt("SimTrade", "curTradeStrategy", 0);
            TradeDataManager.Instance.forceTradeByMaxNumCount = SystemCfg.Instance.CFG.ReadInt("SimTrade", "forceTradeByMaxNumCount", 0) == 1;
            TradeDataManager.Instance.maxNumCount = SystemCfg.Instance.CFG.ReadInt("SimTrade", "maxNumCount", 5);
            TradeDataManager.Instance.RiskControl = SystemCfg.Instance.CFG.ReadFloat("SimTrade", "riskControl", 1);
            TradeDataManager.Instance.uponValue = SystemCfg.Instance.CFG.ReadInt("SimTrade", "uponValue", 0);
            TradeDataManager.Instance.MultiTradePathCount = SystemCfg.Instance.CFG.ReadInt("SimTrade", "multiPathTradeCount", 3);

            checkBoxSpecNumIndex.Checked = SystemCfg.Instance.CFG.ReadInt("SimTrade", "specNumIndex", 0) == 1;
            checkBoxOnTradeOnStrongUpPath.Checked = SystemCfg.Instance.CFG.ReadInt("SimTrade", "onlyTradeOnStrongUpPath", 0) == 1;
            textBoxRiskControl.Text = TradeDataManager.Instance.RiskControl.ToString();
            textBoxMultiPathTradeCount.Text = TradeDataManager.Instance.MultiTradePathCount.ToString();
        }

        public GlobalSimTradeWindow()
        {
            InitializeComponent();

            ReadCfg();

            int preSelIndex;

            textBoxStrongUpStartIndex.Text = TradeDataManager.Instance.strongUpStartTradeIndex.ToString();

            textBoxDayCountPerBatch.Text = BatchTradeSimulator.Instance.batch.ToString();
            textBoxStartMoney.Text = BatchTradeSimulator.Instance.startMoney.ToString();
            textBoxTradeCountLst.Text = TradeDataManager.Instance.GetTradeCountInfoStr();

            preSelIndex = TradeDataManager.Instance.simSelNumIndex;
            comboBoxSpecNumIndex.DataSource = KDataDictContainer.C_TAGS;
            comboBoxSpecNumIndex.SelectedIndex = preSelIndex;
            TradeDataManager.Instance.simSelNumIndex = preSelIndex;
            //checkBoxSpecNumIndex.Checked = TradeDataManager.Instance.simSelNumIndex != -1;

            preSelIndex = (int)TradeDataManager.Instance.curTradeStrategy;
            comboBoxTradeStrategy.DataSource = TradeDataManager.STRATEGY_NAMES;
            comboBoxTradeStrategy.SelectedIndex = preSelIndex;
            TradeDataManager.Instance.curTradeStrategy = (TradeDataManager.TradeStrategy)(preSelIndex);

            checkBoxForceTradeByMaxNumCount.Checked = TradeDataManager.Instance.forceTradeByMaxNumCount;
            textBoxMaxNumCount.Text = TradeDataManager.Instance.maxNumCount.ToString();

            //checkBoxStopOnNoMoney.Checked = stopTradeOnNoMoney;
            comboBoxOnNoMoney.DataSource = OperateOnNoMoneyAR;
            comboBoxOnNoMoney.SelectedIndex = 1;

            trackBarRiskControl.Value = (int)(TradeDataManager.Instance.RiskControl * trackBarRiskControl.Maximum);

            updateTimer = new Timer();
            updateTimer.Interval = 500;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            TradeDataManager.Instance.tradeCompletedCallBack += OnTradeCompleted;
            TradeDataManager.Instance.longWrongTradeCallBack += OnLongWrongTrade;

            FormMain.AddWindow(this);
        }

        private void OnTradeCompleted()
        {
            if (TradeDataManager.Instance.currentMoney <= 0)
            {
                if (comboBoxOnNoMoney.SelectedIndex != 0)
                {
                    // 播放警告声
                    BeepUp.Beep(600, 1000);
                }

                if (comboBoxOnNoMoney.SelectedIndex == 1)
                    Pause();
                else if (comboBoxOnNoMoney.SelectedIndex == 2)
                    Stop();
            }
        }

        private void OnLongWrongTrade(LongWrongTradeInfo info)
        {
            TreeNode parNode = null;
            for (int i = 0; i < treeViewLongWrongTradeInfos.Nodes.Count; ++i)
            {
                TreeNode node = treeViewLongWrongTradeInfos.Nodes[i];
                if((int)(node.Tag) == info.count)
                {
                    parNode = node;
                    break;
                }
            }
            if(parNode == null)
            {
                parNode = new TreeNode();
                parNode.Tag = info.count;
                parNode.Text = info.count.ToString();

                bool hasAdd = false;
                for(int i = 0; i < treeViewLongWrongTradeInfos.Nodes.Count; ++i)
                {
                    int cc = (int)(treeViewLongWrongTradeInfos.Nodes[i].Tag);
                    if(cc < info.count)
                    {
                        treeViewLongWrongTradeInfos.Nodes.Insert(i, parNode);
                        hasAdd = true;
                        break;
                    }
                }
                if(!hasAdd)
                    treeViewLongWrongTradeInfos.Nodes.Add(parNode);
            }
            TreeNode cNode = new TreeNode();
            cNode.Text = info.startDataItemTag + "," + info.endDataItemTag + "," + info.tradeID;
            parNode.Nodes.Add(cNode);
            parNode.Text = info.count + "_" + parNode.Nodes.Count;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            BatchTradeSimulator.Instance.Update();

            progressBarCurrent.Value = BatchTradeSimulator.Instance.GetBatchProgress();
            progressBarTotal.Value = BatchTradeSimulator.Instance.GetMainProgress();

            string NL = "<br>";

            string missStr = "---------------------#NL#交易统计#NL#";
            List<int> keys = BatchTradeSimulator.Instance.tradeMissInfo.Keys.ToList<int>();
            keys.Sort();
            foreach ( int key in keys)
            {
                if(key >= TradeDataManager.Instance.tradeCountList.Count)
                    missStr += "<font color=\"blue\">连错期数 = " + key + ",\t 次数 = " + BatchTradeSimulator.Instance.tradeMissInfo[key] + "#NL#</font>";
                else
                    missStr += "<font color=\"black\">连错期数 = " + key + ",\t 次数 = " + BatchTradeSimulator.Instance.tradeMissInfo[key] + "#NL#</font>";
            }
            
            if (BatchTradeSimulator.Instance.HasFinished())
            {
                string info = "";
                info += ("结束任务!#NL#---------------------#NL#");
                info += ("[当前金额: (");
                info += (BatchTradeSimulator.Instance.currentMoney);
                info += (")]#NL#[最高金额: (");
                info += (BatchTradeSimulator.Instance.maxMoney);
                info += (")]#NL#[最低金额: (");
                info += (BatchTradeSimulator.Instance.minMoney);
                if(BatchTradeSimulator.Instance.currentMoney > BatchTradeSimulator.Instance.startMoney)
                {
                    info += (")]#NL#[盈利: (<font color=\"red\">");
                    info += (BatchTradeSimulator.Instance.currentMoney - BatchTradeSimulator.Instance.startMoney) + "</font>";
                }
                else if(BatchTradeSimulator.Instance.currentMoney < BatchTradeSimulator.Instance.startMoney)
                {
                    info += (")]#NL#[亏损: (<font color=\"green\">");
                    info += (-BatchTradeSimulator.Instance.currentMoney + BatchTradeSimulator.Instance.startMoney) + "</font>";
                }
                info += (")]#NL#---------------------#NL#");
                info += ("[总的次数: ");
                info += (BatchTradeSimulator.Instance.totalCount);
                info += ("]#NL#[对的次数: ");
                info += (BatchTradeSimulator.Instance.tradeRightCount);
                info += ("]#NL#[错的次数: ");
                info += (BatchTradeSimulator.Instance.tradeWrongCount);
                info += ("]#NL#[忽略次数: ");
                info += (BatchTradeSimulator.Instance.untradeCount);
                info += ("]#NL#");

                info = "<font size=2>" + info + missStr + "</font>";
                info = info.Replace("#NL#", NL);

                if (info.CompareTo(textBoxCmd.DocumentText) != 0)
                {
                    Point scrollpos = textBoxCmd.AutoScrollOffset;
                    textBoxCmd.DocumentText = info;
                    textBoxCmd.AutoScrollOffset = scrollpos;
                }
                //textBoxCmd.Text = info + missStr;
            }
            else if (BatchTradeSimulator.Instance.HasJob())
            {
                DataItem fItem = DataManager.GetInst().GetFirstItem();
                DataItem lItem = DataManager.GetInst().GetLatestItem();
                DataItem cItem = TradeDataManager.Instance.GetLatestTradedDataItem();

                string info = "";
                info += ("[当前进度: ");
                info += (progressBarCurrent.Value);
                info += ("%] [总进度: ");
                info += (progressBarTotal.Value);
                info += ("%]#NL#---------------------#NL#");
                if (fItem != null && lItem != null && cItem != null)
                {
                    info += ("[首期: ");
                    info += (fItem.idTag);
                    info += ("]#NL#[末期: ");
                    info += (lItem.idTag);
                    info += ("]#NL#[当期: ");
                    info += (cItem.idTag);
                    info += ("]#NL#---------------------#NL#");
                }
                info += ("[当前金额: (");
                info += (BatchTradeSimulator.Instance.currentMoney);
                info += (")]#NL#[最高金额: (");
                info += (BatchTradeSimulator.Instance.maxMoney);
                info += (")]#NL#[最低金额: (");
                info += (BatchTradeSimulator.Instance.minMoney);
                if (BatchTradeSimulator.Instance.currentMoney > BatchTradeSimulator.Instance.startMoney)
                {
                    info += (")]#NL#[盈利: (<font color=\"red\">");
                    info += (BatchTradeSimulator.Instance.currentMoney - BatchTradeSimulator.Instance.startMoney) + "</font>";
                }
                else if (BatchTradeSimulator.Instance.currentMoney < BatchTradeSimulator.Instance.startMoney)
                {
                    info += (")]#NL#[亏损: (<font color=\"green\">");
                    info += (-BatchTradeSimulator.Instance.currentMoney + BatchTradeSimulator.Instance.startMoney) + "</font>";
                }
                info += (")]#NL#---------------------#NL#");
                info += ("[总的次数: ");
                info += (BatchTradeSimulator.Instance.totalCount);
                info += ("]#NL#[对的次数: ");
                info += (BatchTradeSimulator.Instance.tradeRightCount);
                info += ("]#NL#[错的次数: ");
                info += (BatchTradeSimulator.Instance.tradeWrongCount);
                info += ("]#NL#[忽略次数: ");
                info += (BatchTradeSimulator.Instance.untradeCount);
                info += ("]#NL#");

                info = "<font size=2>" + info + missStr + "</font>";
                info = info.Replace("#NL#", NL);
                //textBoxCmd.Text = info;
                if (info.CompareTo(textBoxCmd.DocumentText) != 0)
                {
                    Point scrollpos = textBoxCmd.AutoScrollOffset;
                    textBoxCmd.DocumentText = info;
                    textBoxCmd.AutoScrollOffset = scrollpos;
                }
                info = null;
            }
            else
                textBoxCmd.Text = "";

            //if(TradeDataManager.Instance.currentMoney <= 0)
            //{
            //    if (comboBoxOnNoMoney.SelectedIndex == 1)
            //        Pause();
            //    else if(comboBoxOnNoMoney.SelectedIndex == 2)
            //        Stop();
            //}

            if (BatchTradeSimulator.Instance.IsPause())
            {
                buttonPauseResume.Text = "恢复";
                buttonPauseResume.BackColor = Color.LightGreen;
            }
            else
            {
                buttonPauseResume.Text = "暂停";
                buttonPauseResume.BackColor = Color.Yellow;
            }
        }

        public static void Open()
        {
            if (sInst == null)
                sInst = new GlobalSimTradeWindow();
            sInst.Show();
        }

        public static void SetSimStartDateAndEndDate(int _startDate, int _endDate)
        {
            if (sInst == null)
                Open();

            if(sInst != null)
            {
                sInst.startDate = _startDate;
                sInst.endDate = _endDate;
                sInst.textBoxStartDate.Text = _startDate.ToString();
                sInst.textBoxEndDate.Text = _endDate.ToString();
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            BatchTradeSimulator.Instance.batch = int.Parse(textBoxDayCountPerBatch.Text);
            BatchTradeSimulator.Instance.startMoney = float.Parse(textBoxStartMoney.Text);
            TradeDataManager.Instance.SetTradeCountInfo(textBoxTradeCountLst.Text);
            LotteryGraph.NotifyAllGraphsStartSimTrade();

            //textBoxCmd.ReadOnly = true;
            buttonPauseResume.Text = "暂停";
            BatchTradeSimulator.Instance.Start(ref startDate, ref endDate);
            textBoxDayCountPerBatch.Enabled = false;
            textBoxStartMoney.Enabled = false;
            textBoxTradeCountLst.Enabled = false;

            progressBarCurrent.Value = 0;
            progressBarTotal.Value = 0;

            textBoxStartDate.Text = startDate.ToString();
            textBoxEndDate.Text = endDate.ToString();

            SaveCfg();
        }

        private void buttonPauseResume_Click(object sender, EventArgs e)
        {
            if (BatchTradeSimulator.Instance.IsPause())
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        void Pause()
        {
            BatchTradeSimulator.Instance.Pause();
            //buttonPauseResume.Text = "恢复";
        }

        void Resume()
        {
            BatchTradeSimulator.Instance.Resume();
            //buttonPauseResume.Text = "暂停";
        }

        void Stop()
        {
            BatchTradeSimulator.Instance.Stop();
            textBoxDayCountPerBatch.Enabled = true;
            textBoxStartMoney.Enabled = true;
            textBoxTradeCountLst.Enabled = true;

            treeViewLongWrongTradeInfos.Nodes.Clear();
        }

        private void GlobalSimTradeWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            TradeDataManager.Instance.tradeCompletedCallBack -= OnTradeCompleted;
            TradeDataManager.Instance.longWrongTradeCallBack -= OnLongWrongTrade;

            FormMain.RemoveWindow(this);

            BatchTradeSimulator.Instance.Stop();
            sInst = null;
        }

        private void checkBoxSpecNumIndex_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSpecNumIndex.Checked)
                TradeDataManager.Instance.simSelNumIndex = comboBoxSpecNumIndex.SelectedIndex;
            else
                TradeDataManager.Instance.simSelNumIndex = -1;
        }

        private void comboBoxSpecNumIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBoxSpecNumIndex.Checked)
                TradeDataManager.Instance.simSelNumIndex = comboBoxSpecNumIndex.SelectedIndex;
            else
                TradeDataManager.Instance.simSelNumIndex = -1;
        }

        private void comboBoxTradeStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            TradeDataManager.Instance.curTradeStrategy = (TradeDataManager.TradeStrategy)comboBoxTradeStrategy.SelectedIndex;
        }

        private void checkBoxForceTradeByMaxNumCount_CheckedChanged(object sender, EventArgs e)
        {
            TradeDataManager.Instance.forceTradeByMaxNumCount = checkBoxForceTradeByMaxNumCount.Checked;
        }

        private void textBoxMaxNumCount_TextChanged(object sender, EventArgs e)
        {
            int value = 0;
            if (int.TryParse(textBoxMaxNumCount.Text, out value))
                TradeDataManager.Instance.maxNumCount = value;
        }

        private void checkBoxStopOnNoMoney_CheckedChanged(object sender, EventArgs e)
        {
            //stopTradeOnNoMoney = checkBoxStopOnNoMoney.Checked;
        }

        private void comboBoxOnNoMoney_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxStrongUpStartIndex_TextChanged(object sender, EventArgs e)
        {
            int v = TradeDataManager.Instance.strongUpStartTradeIndex;
            if(int.TryParse(textBoxStrongUpStartIndex.Text, out v))
                TradeDataManager.Instance.strongUpStartTradeIndex = v;
        }

        private void checkBoxOnTradeOnStrongUpPath_CheckedChanged(object sender, EventArgs e)
        {
            TradeDataManager.Instance.onlyTradeOnStrongUpPath = checkBoxOnTradeOnStrongUpPath.Checked;
        }

        private void trackBarRiskControl_Scroll(object sender, EventArgs e)
        {
            TradeDataManager.Instance.RiskControl = (float)trackBarRiskControl.Value / trackBarRiskControl.Maximum;
            textBoxRiskControl.Text = TradeDataManager.Instance.RiskControl.ToString();
        }

        private void buttonDebugSetting_Click(object sender, EventArgs e)
        {
            TradeDebugWindow.Open();
        }

        private void textBoxUponValue_TextChanged(object sender, EventArgs e)
        {
            int v = TradeDataManager.Instance.uponValue;
            if (int.TryParse(textBoxUponValue.Text, out v))
                TradeDataManager.Instance.uponValue = v;
        }

        private void treeViewLongWrongTradeInfos_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = treeViewLongWrongTradeInfos.SelectedNode;
            if(node != null && node.Tag == null)
            {
                string[] spt = node.Text.Split(',');
                if(spt.Length == 3)
                {
                    string lastTag = spt[1];
                    string tradeID = spt[2];
                    int tradeIndex = int.Parse(tradeID);
                    DataItem item = DataManager.GetInst().GetDataItemByIdTag(lastTag);
                    TradeDataBase trade = TradeDataManager.Instance.GetTrade(tradeIndex);
                    TradeDataBase firstTrade = TradeDataManager.Instance.GetFirstHistoryTradeData();
                    if (item!=null && trade!=null)
                    {
                        LotteryGraph.OnSelectDataItemOuter(item.idGlobal, trade.INDEX - firstTrade.INDEX);
                    }
                }
            }
        }

        private void textBoxMultiPathTradeCount_TextChanged(object sender, EventArgs e)
        {
            int v = TradeDataManager.Instance.MultiTradePathCount;
            if (int.TryParse(textBoxMultiPathTradeCount.Text, out v))
                TradeDataManager.Instance.MultiTradePathCount = v;
        }
    }
}
