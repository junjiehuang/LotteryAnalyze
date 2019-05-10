using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace LotteryAnalyze.UI
{
    public enum OperateOnNoMoney
    {
        eNone,
        ePauseTrade,
        eStopTrade,
    }

    public partial class GlobalSimTradeWindow : Form, UpdaterBase
    {
        static string[] OperateOnNoMoneyAR = {"无操作", "暂停交易模拟", "终止交易模拟", };

        static GlobalSimTradeWindow sInst;
        
        int startDate = -1;
        int endDate = -1;
        bool stopTradeOnNoMoney = true;

        System.Windows.Forms.Timer updateTimer;
        int updateInterval = GlobalSetting.G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL;
        double updateCountDown = 0;

        Dictionary<int, List<string>> tradeWrongCountTagsMap = new Dictionary<int, List<string>>();
        Dictionary<int, TreeNode> tradeWrongCountTreeNodeMap = new Dictionary<int, TreeNode>();
        Dictionary<string, TreeNode> tagTreeNodeMap = new Dictionary<string, TreeNode>();
        bool needRefreshTree = false;

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
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "killLastNumber", checkBoxKillLastNumber.Checked ? 1 : 0);
            SystemCfg.Instance.CFG.WriteInt("SimTrade", "procOnNegMoney", comboBoxOnNoMoney.SelectedIndex);

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
            checkBoxKillLastNumber.Checked = SystemCfg.Instance.CFG.ReadInt("SimTrade", "killLastNumber", 0) == 1;

            textBoxRiskControl.Text = TradeDataManager.Instance.RiskControl.ToString();
            textBoxMultiPathTradeCount.Text = TradeDataManager.Instance.MultiTradePathCount.ToString();

            int procOnNegMoney = SystemCfg.Instance.CFG.ReadInt("SimTrade", "procOnNegMoney", 0);
            comboBoxOnNoMoney.SelectedIndex = procOnNegMoney;
        }

        GlobalSimTradeWindow()
        {
            InitializeComponent();

            //采用双缓冲技术的控件必需的设置
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            comboBoxOnNoMoney.DataSource = OperateOnNoMoneyAR;
            comboBoxSpecNumIndex.DataSource = KDataDictContainer.C_TAGS;
            comboBoxTradeStrategy.DataSource = TradeDataManager.STRATEGY_NAMES;

            ReadCfg();

            int preSelIndex;

            textBoxStrongUpStartIndex.Text = TradeDataManager.Instance.strongUpStartTradeIndex.ToString();
            textBoxDayCountPerBatch.Text = BatchTradeSimulator.Instance.batch.ToString();
            textBoxStartMoney.Text = BatchTradeSimulator.Instance.startMoney.ToString();
            textBoxTradeCountLst.Text = TradeDataManager.Instance.GetTradeCountInfoStr();
            preSelIndex = TradeDataManager.Instance.simSelNumIndex;            
            comboBoxSpecNumIndex.SelectedIndex = preSelIndex;
            TradeDataManager.Instance.simSelNumIndex = preSelIndex;
            //checkBoxSpecNumIndex.Checked = TradeDataManager.Instance.simSelNumIndex != -1;
            preSelIndex = (int)TradeDataManager.Instance.curTradeStrategy;            
            comboBoxTradeStrategy.SelectedIndex = preSelIndex;
            TradeDataManager.Instance.curTradeStrategy = (TradeDataManager.TradeStrategy)(preSelIndex);
            checkBoxForceTradeByMaxNumCount.Checked = TradeDataManager.Instance.forceTradeByMaxNumCount;
            textBoxMaxNumCount.Text = TradeDataManager.Instance.maxNumCount.ToString();
            checkBoxKillLastNumber.Checked = TradeDataManager.Instance.killLastNumber;
            //checkBoxStopOnNoMoney.Checked = stopTradeOnNoMoney;            
            //comboBoxOnNoMoney.SelectedIndex = 1;
            trackBarRiskControl.Value = (int)(TradeDataManager.Instance.RiskControl * trackBarRiskControl.Maximum);
            
            updateTimer = new Timer();
            updateTimer.Interval = 10;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            textBoxRefreshTime.Text = GlobalSetting.G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL.ToString();

            TradeDataManager.Instance.tradeCompletedCallBack += OnTradeCompleted;
            TradeDataManager.Instance.longWrongTradeCallBack += OnLongWrongTrade;

            FormMain.AddWindow(this);
            Program.AddUpdater(this);
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
                    DoStop();
            }
        }


        private void OnLongWrongTrade(LongWrongTradeInfo info)
        {
            needRefreshTree = true;
            List<string> lst = null;
            if (tradeWrongCountTagsMap.ContainsKey(info.count))
            {
                lst = tradeWrongCountTagsMap[info.count];
            }
            else
            {
                lst = new List<string>();
                tradeWrongCountTagsMap[info.count] = lst;
            }
            string txt = info.startDataItemTag + "," + info.endDataItemTag + "," + info.tradeID;
            lst.Add(txt);

            //TreeNode parNode = null;
            //for (int i = 0; i < treeViewLongWrongTradeInfos.Nodes.Count; ++i)
            //{
            //    TreeNode node = treeViewLongWrongTradeInfos.Nodes[i];
            //    if((int)(node.Tag) == info.count)
            //    {
            //        parNode = node;
            //        break;
            //    }
            //}
            //if(parNode == null)
            //{
            //    parNode = new TreeNode();
            //    parNode.Tag = info.count;
            //    parNode.Text = info.count.ToString();

            //    bool hasAdd = false;
            //    for(int i = 0; i < treeViewLongWrongTradeInfos.Nodes.Count; ++i)
            //    {
            //        int cc = (int)(treeViewLongWrongTradeInfos.Nodes[i].Tag);
            //        if(cc < info.count)
            //        {
            //            treeViewLongWrongTradeInfos.Nodes.Insert(i, parNode);
            //            hasAdd = true;
            //            break;
            //        }
            //    }
            //    if(!hasAdd)
            //        treeViewLongWrongTradeInfos.Nodes.Add(parNode);
            //}
            //TreeNode cNode = new TreeNode();
            //cNode.Text = info.startDataItemTag + "," + info.endDataItemTag + "," + info.tradeID;
            //parNode.Nodes.Add(cNode);
            //parNode.Text = info.count + "_" + parNode.Nodes.Count;
        }

        public virtual void OnUpdate()
        {
            //progressBarCurrent.Value = BatchTradeSimulator.Instance.GetBatchProgress();
            //progressBarTotal.Value = BatchTradeSimulator.Instance.GetMainProgress();

            if (GlobalSetting.G_UPDATE_IN_MAIN_THREAD)
            {
                if (updateCountDown <= 0)
                {
                    UpdateImpl();
                    //UpdateTimer_Tick(null, null);
                    updateCountDown = (double)GlobalSetting.G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL / 1000.0;
                }
                else
                {
                    updateCountDown -= Program.DeltaTime;
                }
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (GlobalSetting.G_UPDATE_IN_MAIN_THREAD == false)
            {
                BatchTradeSimulator.Instance.Update();

                UpdateImpl();
            }
        }

        private void UpdateImpl()
        {
            progressBarCurrent.Value = BatchTradeSimulator.Instance.GetBatchProgress();
            progressBarTotal.Value = BatchTradeSimulator.Instance.GetMainProgress();

            string NL = "<br>";

            string missStr = "---------------------#NL#交易统计#NL#";
            List<int> keys = BatchTradeSimulator.Instance.tradeMissInfo.Keys.ToList<int>();
            keys.Sort();
            foreach (int key in keys)
            {
                if (key >= TradeDataManager.Instance.tradeCountList.Count)
                    missStr += "<font color=\"blue\">连错期数 = " + key + ",\t 次数 = " + BatchTradeSimulator.Instance.tradeMissInfo[key] + "#NL#</font>";
                else
                    missStr += "<font color=\"black\">连错期数 = " + key + ",\t 次数 = " + BatchTradeSimulator.Instance.tradeMissInfo[key] + "#NL#</font>";
            }

            string info = "";
            if (BatchTradeSimulator.Instance.HasFinished())
            {
                info += ("结束任务!#NL#---------------------#NL#");

                info += ("[当前金额: (");
                if (BatchTradeSimulator.Instance.currentMoney > 0)
                    info += "+";
                info += (int)(BatchTradeSimulator.Instance.currentMoney);
                info += (")]#NL#[最高金额: (");
                if (BatchTradeSimulator.Instance.maxMoney > 0)
                    info += "+";
                info += (int)(BatchTradeSimulator.Instance.maxMoney);
                info += (")]#NL#[最低金额: (");
                if (BatchTradeSimulator.Instance.minMoney > 0)
                    info += "+";
                info += (int)(BatchTradeSimulator.Instance.minMoney);
                int delta = (int)(BatchTradeSimulator.Instance.currentMoney - BatchTradeSimulator.Instance.startMoney);
                if (delta > 0)
                    info += (")]#NL#[盈利: (<font color=\"red\">+");
                else if (delta < 0)
                    info += (")]#NL#[亏损: (<font color=\"green\">");
                else
                    info += (")]#NL#[平衡: (<font color=\"white\">");
                info += delta + "</font>";

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
            }
            else if (BatchTradeSimulator.Instance.HasJob())
            {
                DataItem fItem = DataManager.GetInst().GetFirstItem();
                DataItem lItem = DataManager.GetInst().GetLatestItem();
                DataItem cItem = TradeDataManager.Instance.GetLatestTradedDataItem();

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
                if (BatchTradeSimulator.Instance.currentMoney > 0)
                    info += "+";
                info += (int)(BatchTradeSimulator.Instance.currentMoney);
                info += (")]#NL#[最高金额: (");
                if (BatchTradeSimulator.Instance.maxMoney > 0)
                    info += "+";
                info += (int)(BatchTradeSimulator.Instance.maxMoney);
                info += (")]#NL#[最低金额: (");
                if (BatchTradeSimulator.Instance.minMoney > 0)
                    info += "+";
                info += (int)(BatchTradeSimulator.Instance.minMoney);
                int delta = (int)(BatchTradeSimulator.Instance.currentMoney - BatchTradeSimulator.Instance.startMoney);
                if (delta > 0)
                    info += (")]#NL#[盈利: (<font color=\"red\">+");
                else if (delta < 0)
                    info += (")]#NL#[亏损: (<font color=\"green\">");
                else
                    info += (")]#NL#[平衡: (<font color=\"white\">");
                info += delta + "</font>";

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
            }

            try
            {
                if (info.CompareTo(textBoxCmd.DocumentText) != 0)
                {
                    Point scrollpos = textBoxCmd.AutoScrollOffset;
                    textBoxCmd.DocumentText = info;
                    textBoxCmd.AutoScrollOffset = scrollpos;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
            info = null;

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

            RefreshTree();
            this.Invalidate(true);

        }

        void RefreshTree()
        {
            if (needRefreshTree == false)
                return;
            
            foreach (int key in tradeWrongCountTagsMap.Keys)
            {
                List<string> tags = tradeWrongCountTagsMap[key];
                TreeNode pN = null;
                if(tradeWrongCountTreeNodeMap.ContainsKey(key))
                {
                    pN = tradeWrongCountTreeNodeMap[key];
                }
                else
                {
                    pN = new TreeNode();
                    bool insert = false;
                    for( int j = 0; j < treeViewLongWrongTradeInfos.Nodes.Count; ++j)
                    {
                        if((int)(treeViewLongWrongTradeInfos.Nodes[j].Tag) < key)
                        {
                            treeViewLongWrongTradeInfos.Nodes.Insert(j, pN);
                            insert = true;
                            break;
                        }
                    }
                    if(insert == false)
                        treeViewLongWrongTradeInfos.Nodes.Add(pN);
                    tradeWrongCountTreeNodeMap[key] = pN;
                    pN.Tag = key;
                }
                pN.Text = key + "-" + tags.Count;

                for ( int i = 0; i < tags.Count; ++i )
                {
                    if (tagTreeNodeMap.ContainsKey(tags[i]))
                        continue;
                    TreeNode pS = new TreeNode(tags[i]);
                    pN.Nodes.Add(pS);
                    tagTreeNodeMap[tags[i]] = pS;
                }
            }
            needRefreshTree = false;
        }

        public static void Open()
        {
            if (sInst == null)
                sInst = new GlobalSimTradeWindow();
            sInst.Show();
        }

        public static GlobalSimTradeWindow Instance
        {
            get { return sInst; }
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
            DoStart();
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
            DoStop();
        }

        public void DoStart()
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
            tradeWrongCountTagsMap.Clear();
            treeViewLongWrongTradeInfos.Nodes.Clear();
            tradeWrongCountTreeNodeMap.Clear();
            tagTreeNodeMap.Clear();

            SaveCfg();
        }

        void Pause()
        {
            BatchTradeSimulator.Instance.Pause();
            //buttonPauseResume.Text = "恢复";
        }

        void Resume()
        {
            BatchTradeSimulator.Instance.Resume();
        }

        public void DoStop()
        {
            BatchTradeSimulator.Instance.Stop();
            textBoxDayCountPerBatch.Enabled = true;
            textBoxStartMoney.Enabled = true;
            textBoxTradeCountLst.Enabled = true;

            // 重置K线的起点值
            KGraphDataContainer.ResetCurKValueMap();
        }

        private void GlobalSimTradeWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
            }
            TradeDataManager.Instance.tradeCompletedCallBack -= OnTradeCompleted;
            TradeDataManager.Instance.longWrongTradeCallBack -= OnLongWrongTrade;

            FormMain.RemoveWindow(this);
            Program.RemoveUpdater(this);

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

        private void checkBoxKillLastNumber_CheckedChanged(object sender, EventArgs e)
        {
            TradeDataManager.Instance.killLastNumber = checkBoxKillLastNumber.Checked;
        }

        private void exportResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = "..\\tools\\模拟结果.xml";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "sim result files|*.xml";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.InitialDirectory = "..\\tools\\";// System.Environment.CurrentDirectory;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
            }
            else
            {
                return;
            }

            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            string info = "<root>\n";

            string tradeCountLstStr = TradeDataManager.Instance.GetTradeCountInfoStr();
            info += "\t<TradeStrategy>\n";
            info += "\t\t<batch>" + BatchTradeSimulator.Instance.batch + "</batch>\n";
            info += "\t\t<startMoney>" + BatchTradeSimulator.Instance.startMoney + "</startMoney>\n";
            info += "\t\t<tradeCountLstStr>" + tradeCountLstStr + "</tradeCountLstStr>\n";
            info += "\t\t<strongUpStartIndex>" + TradeDataManager.Instance.strongUpStartTradeIndex + "</strongUpStartIndex>\n";
            info += "\t\t<simSelNumIndex>" + TradeDataManager.Instance.simSelNumIndex + "</simSelNumIndex>\n";
            info += "\t\t<curTradeStrategy>" + TradeDataManager.Instance.curTradeStrategy + "</curTradeStrategy>\n";
            info += "\t\t<forceTradeByMaxNumCount>" + TradeDataManager.Instance.forceTradeByMaxNumCount + "</forceTradeByMaxNumCount>\n";
            info += "\t\t<maxNumCount>" + TradeDataManager.Instance.maxNumCount + "</maxNumCount>\n";
            info += "\t\t<specNumIndex>" + checkBoxSpecNumIndex.Checked + "</specNumIndex>\n";
            info += "\t\t<onlyTradeOnStrongUpPath>" + checkBoxOnTradeOnStrongUpPath.Checked + "</onlyTradeOnStrongUpPath>\n";
            info += "\t\t<riskControl>" + TradeDataManager.Instance.RiskControl + "</riskControl>\n";
            info += "\t\t<uponValue>" + TradeDataManager.Instance.uponValue + "</uponValue>\n";
            info += "\t\t<MultiTradePathCount>" + TradeDataManager.Instance.MultiTradePathCount + "</MultiTradePathCount>\n";
            info += "\t\t<killLastNumber>" + checkBoxKillLastNumber.Checked + "</killLastNumber>\n";
            info += "\t\t<procOnNegMoney>" + comboBoxOnNoMoney.SelectedIndex + "</procOnNegMoney>\n";
            info += "\t</TradeStrategy>\n";

            info += "\t<Simple>\n";
            info += "\t\t<StartMoney>" + BatchTradeSimulator.Instance.startMoney + "</StartMoney>\n";
            info += "\t\t<CurrentMoney>" + BatchTradeSimulator.Instance.currentMoney + "</CurrentMoney>\n";
            info += "\t\t<MaxMoney>" + BatchTradeSimulator.Instance.maxMoney + "</MaxMoney>\n";
            info += "\t\t<MinMoney>" + BatchTradeSimulator.Instance.minMoney + "</MinMoney>\n";
            float delta = BatchTradeSimulator.Instance.currentMoney - BatchTradeSimulator.Instance.startMoney;
            if (delta > 0)
            {
                info += "\t\t<EarnMoney>" + delta + "</EarnMoney>\n";
            }
            else if (BatchTradeSimulator.Instance.currentMoney < BatchTradeSimulator.Instance.startMoney)
            {
                info += "\t\t<LostMoney>" + delta + "</LostMoney>\n";
            }
            info += "\t\t<TradeTotalCount>" + BatchTradeSimulator.Instance.totalCount + "</TradeTotalCount>\n";
            info += "\t\t<TradeRightCount>" + BatchTradeSimulator.Instance.tradeRightCount + "</TradeRightCount>\n";
            info += "\t\t<TradeWrongCount>" + BatchTradeSimulator.Instance.tradeWrongCount + "</TradeWrongCount>\n";
            info += "\t\t<UnTradeCount>" + BatchTradeSimulator.Instance.untradeCount + "</UnTradeCount>\n";
            info += "\t</Simple>\n";

            info += GlobalSetting.WriteSettingToXMLString();

            List<int> keys = BatchTradeSimulator.Instance.tradeMissInfo.Keys.ToList<int>();
            keys.Sort();
            info += "\t<TradeBeief>\n";
            foreach (int key in keys)
            {
                info += "\t\t<MissCount count=\"" + key + "\">" + BatchTradeSimulator.Instance.tradeMissInfo[key] + "</MissCount>\n";
            }
            info += "\t</TradeBeief>\n";

            info += "\t<LongMissTradeInfos>\n";
            //foreach(TreeNode node in treeViewLongWrongTradeInfos.Nodes)
            List<int> tagKeys = new List<int>();
            tagKeys.AddRange(tradeWrongCountTagsMap.Keys);
            tagKeys.Sort((x, y) =>
            {
                if (x > y)
                    return -1;
                return 1;
            });
            foreach ( int key in tagKeys)
            {
                List<string> tags = tradeWrongCountTagsMap[key];

                info += "\t\t<trade name=\"" + key + "-" + tags.Count + "\">\n";

                //foreach(TreeNode subNode in node.Nodes)
                for( int i = 0; i < tags.Count; ++i )
                {
                    info += "\t\t\t<detail>" + tags[i] + "</detail>\n";
                }

                info += "\t\t</trade>\n";
            }
            info += "\t</LongMissTradeInfos>\n";
            //开始写入
            sw.Write(info);
            info = "";

            info += "\t<LongMissTradeDatas>\n";
            var etor = TradeDataManager.Instance.longWrongTradeInfo.GetEnumerator();
            while(etor.MoveNext())
            {
                //开始写入
                sw.Write(info);
                info = "";

                List<LongWrongTradeInfo> lst = etor.Current.Value;
                info += "\t\t<TradeMissCount name=\"" + etor.Current.Key + "\">\n";
                for( int i = 0; i < lst.Count; ++i )
                {
                    LongWrongTradeInfo ti = lst[i];
                    info += "\t\t\t<TradeDatas startDataItemTag=\"" + ti.startDataItemTag + "\" endDataItemTag=\"" + ti.endDataItemTag + "\">\n";

                    if (ti.tradeDatas != null)
                    {
                        for (int j = 0; j < ti.tradeDatas.Count; ++j)
                        {
                            info += ti.tradeDatas[j];
                        }
                    }
                    info += "\t\t\t</TradeDatas>\n";

                    //开始写入
                    sw.Write(info);
                    info = "";
                }
                info += "\t\t</TradeMissCount>\n";
            }
            info += "\t</LongMissTradeDatas>\n";

            info += "</root>";

            //开始写入
            sw.Write(info);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        private void textBoxRefreshTime_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(textBoxRefreshTime.Text, out v) == false)
                GlobalSetting.G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = 50;
            else
                GlobalSetting.G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = v;
            if (GlobalSetting.G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL < 1)
                GlobalSetting.G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = 1;
            textBoxRefreshTime.Text = GlobalSetting.G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL.ToString();
            this.Invalidate(true);
        }

        public static void StartDebug()
        {
            if (sInst != null)
            {
                sInst.DoStop();
                sInst.DoStart();
            }
        }

    }
}
