using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace LotteryAnalyze
{
    public partial class FormMain : Form
    {
        static FormMain sInst;
        public static FormMain Instance
        {
            get { return sInst; }
        }
        System.Windows.Forms.Timer updateTimer;
        int lastFetchCount = -1;

        public FormMain()
        {
            sInst = this;
            InitializeComponent();

            Dictionary<string, KillNumberStrategy> funcList = KillNumberStrategyManager.GetInst().funcList;
            foreach (string key in funcList.Keys)
            {
                int rowID = dataGridViewKillNumberStrategy.Rows.Count;
                KillNumberStrategy strategy = funcList[key];
                object[] parms = new object[] { strategy.active, key, strategy.DESC(), };
                dataGridViewKillNumberStrategy.Rows.Add(parms);
                DataGridViewRow row = dataGridViewKillNumberStrategy.Rows[rowID];
                row.Tag = strategy;
            }
            List<CollectorBase> cl = StatisticsCollector.CollectorList;
            for (int i = 0; i < cl.Count; ++i)
            {
                CollectorBase cb = cl[i];
                object[] parms = new object[] { cb.enable, cb.GetDesc(), };
                dataGridViewCollectorOption.Rows.Add(parms);
                DataGridViewRow row = dataGridViewCollectorOption.Rows[i];
                row.Tag = cb;
            }
            for (int i = 0; i < DataGridViewColumnManager.COLUMNS.Count; ++i)
            {
                ColumnBase col = DataGridViewColumnManager.COLUMNS[i];
                if (col.forceActive)
                    continue;
                object[] parms = new object[] { col.active, col.GetColumnName(), };
                int rid = dataGridViewColSelector.Rows.Add(parms);
                DataGridViewRow row = dataGridViewColSelector.Rows[rid];
                row.Tag = col;
            }
#if ENABLE_GROUP_COLLECT
            comboBoxKillGroup.SelectedIndex = 2;
            SimulationGroup3.enableDoubleRatioIfFailed = checkBoxDoubleRatio.Checked;
            textBoxFirmRatio.Text = SimulationGroup3.firmRatio.ToString();
            textBoxMaxRatio.Text = SimulationGroup3.maxRatio.ToString();
            textBoxPath012ShortCount.Text = ColumnSimulateSingleBuyLottery.S_SHORT_COUNT.ToString();
#endif
            DataGridViewColumnManager.ReassignColumns(dataGridViewLotteryDatas);
        }

        public KillType GetCurSelectedKillType()
        {
            return (KillType)comboBoxKillGroup.SelectedIndex;
        }

        private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataManager dataMgr = DataManager.GetInst();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "data files|*.txt";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            openFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; ++i)
                {
                    string[] strs = openFileDialog.FileNames[i].Split('\\');
                    string fileName = strs[strs.Length - 1];
                    strs = fileName.Split('.');

                    int id = int.Parse(strs[0]);
                    if (dataMgr.mFileMetaInfo.ContainsKey(id) == false)
                    {
                        dataMgr.mFileMetaInfo.Add(id, openFileDialog.FileNames[i]);
                    }
                }

                RefreshFileList();
            }
        }

        void ClearAll()
        {
            DataManager dataMgr = DataManager.GetInst();
            dataMgr.mFileMetaInfo.Clear();
            dataMgr.indexs.Clear();
            dataMgr.allDatas.Clear();
            dataGridViewLotteryDatas.Rows.Clear();
            progressBar1.Value = progressBar1.Minimum;
            richTextBoxResult.Text = "";
            RefreshFileList();
        }

        void RefreshFileList()
        {
            DataManager dataMgr = DataManager.GetInst();
            listViewFileList.Items.Clear();
            foreach (int key in dataMgr.mFileMetaInfo.Keys)
            {
                ListViewItem item = new ListViewItem();
                item.Name = key.ToString();
                item.Text = key.ToString();
                item.Tag = key;
                listViewFileList.Items.Add(item);
            }
        }

        void RefreshDataView()
        {
            //int curID = 0;
            DataManager dataMgr = DataManager.GetInst();
            dataMgr.SetDataItemsGlobalID();

            dataGridViewLotteryDatas.Rows.Clear();
            foreach( int key in dataMgr.allDatas.Keys )
            {
                OneDayDatas data = dataMgr.allDatas[key];
                for (int i = 0; i < data.datas.Count; ++i)
                {
                    DataItem di = data.datas[i];
                    //di.idGlobal = curID++;
                    //string g6 = di.groupType == GroupType.eGT6 ? "组6" : "";
                    //string g3 = di.groupType == GroupType.eGT3 ? "组3" : "";
                    //string g1 = di.groupType == GroupType.eGT1 ? "豹子" : "";
                    //object[] objs = new object[] { di.idTag, di.lotteryNumber, di.andValue, di.rearValue, di.crossValue, g6, g3, g1, };
                    //dataGridViewLotteryDatas.Rows.Add(objs);
                    DataGridViewColumnManager.AddRow(di, dataGridViewLotteryDatas);
                }
            }
        }
        void RefreshDataViewOnColumnSelected()
        {
            int rowCount = dataGridViewLotteryDatas.Rows.Count;
            for (int i = 0; i < rowCount; ++i)
            {
                DataGridViewRow row = dataGridViewLotteryDatas.Rows[i];
                DataGridViewColumnManager.SetColumnText(row.Tag as DataItem, row);
            }
        }
        public void ResetResult()
        {
            //for (int i = 0; i < dataGridViewLotteryDatas.RowCount; ++i)
            //{
            //    DataGridViewRow row = dataGridViewLotteryDatas.Rows[i];
            //    for (int j = 0; j < 6; ++j)
            //    {
            //        int col = 8 + j;
            //        DataGridViewCell cell = row.Cells[col];
            //        cell.Value = "";
            //    }
            //}
            progressBar1.Value = progressBar1.Minimum;
        }
        public void RefreshResultItem(int itemIndex, DataItem item)
        {
            DataGridViewRow row = dataGridViewLotteryDatas.Rows[itemIndex];
            DataGridViewColumnManager.SetColumnText(item, row);
            //DataGridViewRow row = dataGridViewLotteryDatas.Rows[itemIndex];
            //int curCol = 8;
            //DataGridViewCell cell = row.Cells[curCol++];
            //string kt = "";
            //switch (item.simData.killType)
            //{
            //    case KillType.eKTGroup3: kt = "杀组三 "; break;
            //    case KillType.eKTGroup6: kt = "杀组六 "; break;
            //    case KillType.eKTNone: kt = "忽略 "; break;
            //}
            //cell.Value = kt + item.simData.killList;
            //DataGridViewCell c1 = row.Cells[curCol++];
            //c1.Value = item.simData.predictResult == TestResultType.eTRTSuccess ? "对" : "";
            //DataGridViewCell c2 = row.Cells[curCol++];
            //c2.Value = item.simData.predictResult == TestResultType.eTRTFailed ? "错" : "";
            //DataGridViewCell c3 = row.Cells[curCol++];
            //c3.Value = item.simData.cost;
            //DataGridViewCell c4 = row.Cells[curCol++];
            //c4.Value = item.simData.reward;
            //DataGridViewCell c5 = row.Cells[curCol++];
            //c5.Value = item.simData.profit;
            //progressBar1.Value = progressBar1.Minimum + (int)((float)(itemIndex+1) / (float)(dataGridViewLotteryDatas.RowCount) * (float)(progressBar1.Maximum - progressBar1.Minimum));
        }
        public void RefreshResultPanel()
        {
#if ENABLE_GROUP_COLLECT
            DataManager mgr = DataManager.GetInst();
            StringBuilder sb = new StringBuilder();
            sb.Append("准确率 : " + (float)mgr.simData.rightCount / (float)mgr.simData.predictCount * 100 + "%\n");
            Simulator.SortWrongInfos(true);
            for (int i = 0; i < SimulationGroup3.allWrongInfos.Count; ++i)
            {
                WrongInfo wi = SimulationGroup3.allWrongInfos[i];
                sb.Append("损失值\t: " + wi.costTotal + "\n");
                sb.Append("起始期号\t: " + wi.startTag + "\n");
                sb.Append("连错期数\t: " + wi.round + "\n\n");
            }
            richTextBoxResult.Text = sb.ToString();
#endif
        }
        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = "";
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.SelectedPath = System.Environment.CurrentDirectory;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog.SelectedPath;

                DirectoryInfo di = new DirectoryInfo(path);
                LoopSearchFolder(di);
                RefreshFileList();
            }
        }

        void LoopSearchFolder(DirectoryInfo parentDirInfo)
        {
            DataManager dataMgr = DataManager.GetInst();
            FileInfo[] files = parentDirInfo.GetFiles();
            DirectoryInfo[] dirs = parentDirInfo.GetDirectories();
            for (int i = 0; i < files.Length; ++i)
            {
                string[] strs = files[i].FullName.Split('\\');
                string fileName = strs[strs.Length - 1];
                strs = fileName.Split('.');

                int id = int.Parse(strs[0]);
                if (dataMgr.mFileMetaInfo.ContainsKey(id) == false)
                {
                    dataMgr.mFileMetaInfo.Add(id, files[i].FullName);
                }
            }
            for (int i = 0; i < dirs.Length; ++i)
            {
                LoopSearchFolder(dirs[i]);
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAll();
        }



        private void addToSimulatePoolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataManager dataMgr = DataManager.GetInst();
            dataMgr.ClearAllDatas();
            for (int i = 0; i < listViewFileList.SelectedItems.Count; ++i)
            {
                ListViewItem item = listViewFileList.SelectedItems[i];
                int key = (int)(item.Tag);
                dataMgr.LoadData(key);
            }
            Util.CollectPath012Info(null);
            RefreshDataView();
        }

        private void dataGridViewKillNumberStrategy_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridViewKillNumberStrategy.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            if (row.Tag != null && e.ColumnIndex == 0)
            {
                bool v = (bool)cell.Value;
                KillNumberStrategy strategy = row.Tag as KillNumberStrategy;
                strategy.active = v;
            }
        }

        private void buttonExecSimulate_Click(object sender, EventArgs e)
        {
            DataManager dataMgr = DataManager.GetInst();
            if (dataGridViewLotteryDatas.RowCount > 0 && dataMgr.allDatas.Count > 0)
            {
                Simulator.StartSimulate();
            }
        }

        private void checkBoxDoubleRatio_Click(object sender, EventArgs e)
        {
#if ENABLE_GROUP_COLLECT
            SimulationGroup3.enableDoubleRatioIfFailed = checkBoxDoubleRatio.Checked;
#endif
        }

        private void textBoxFirmRatio_TextChanged(object sender, EventArgs e)
        {
#if ENABLE_GROUP_COLLECT
            SimulationGroup3.firmRatio = int.Parse(textBoxFirmRatio.Text);
#endif
        }

        private void textBoxMaxRatio_TextChanged(object sender, EventArgs e)
        {
#if ENABLE_GROUP_COLLECT
            SimulationGroup3.maxRatio = int.Parse(textBoxMaxRatio.Text);
#endif
        }

        private void dataGridViewCollectorOption_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridViewCollectorOption.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            if (row.Tag != null && e.ColumnIndex == 0)
            {
                bool v = (bool)cell.Value;
                CollectorBase cb = row.Tag as CollectorBase;
                cb.enable = v;
            }
        }

        private void buttonCollector_Click(object sender, EventArgs e)
        {
            treeViewCollectorInfo.Nodes.Clear();
            StatisticsCollector.Collect();
            for (int i = 0; i < StatisticsCollector.CollectorList.Count; ++i)
            {
                CollectorBase cb = StatisticsCollector.CollectorList[i];
                if (cb.enable == false)
                    continue;
                TreeNode nodeParent = treeViewCollectorInfo.Nodes.Add(cb.GetDesc());
                cb.OutPutToTreeView(nodeParent);
            }
            RefreshDataView();
        }

        public void SelectDataItem(DataItem item)
        {
            if(item != null)
            {
                SelectDataItem(item.idGlobal);
            }
        }
        public void SelectDataItem(int id)
        {
            if (id != -1)
            {
                for (int i = 0; i < dataGridViewLotteryDatas.Rows.Count; ++i)
                {
                    if (i != id)
                        dataGridViewLotteryDatas.Rows[i].Selected = false;
                    else
                    {
                        dataGridViewLotteryDatas.Rows[i].Selected = true;
                        dataGridViewLotteryDatas.CurrentCell = dataGridViewLotteryDatas.Rows[i].Cells[0];
                    }
                }
            }
        }

        private void treeViewCollectorInfo_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = treeViewCollectorInfo.SelectedNode;
            if (node != null && node.Tag != null)
            {
                CollectTag tag = node.Tag as CollectTag;
                if (tag.itemIndex != -1)
                {
                    //dataGridViewLotteryDatas.Rows[tag.itemIndex].Selected = true;
                    dataGridViewLotteryDatas.FirstDisplayedScrollingRowIndex = tag.itemIndex;
                    int firstIndex = tag.itemIndex;
                    int lastIndex = tag.itemIndex + tag.continueCount - 1;
                    for (int i = 0; i < dataGridViewLotteryDatas.Rows.Count; ++i)
                    {
                        if (i < firstIndex || i > lastIndex)
                            dataGridViewLotteryDatas.Rows[i].Selected = false;
                        else
                        {
                            dataGridViewLotteryDatas.Rows[i].Selected = true;
                            dataGridViewLotteryDatas.CurrentCell = dataGridViewLotteryDatas.Rows[i].Cells[0];
                        }
                    }
                }
            }
        }

        private void dataGridViewColSelector_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridViewColSelector.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            if (row.Tag != null && e.ColumnIndex == 0)
            {
                bool v = (bool)cell.Value;
                ColumnBase cb = row.Tag as ColumnBase;
                cb.active = v;
                DataGridViewColumnManager.ReassignColumns(dataGridViewLotteryDatas);
                RefreshDataView();
            }
        }

        private void clearSimPoolDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataManager dataMgr = DataManager.GetInst();
            dataMgr.ClearAllDatas();
            dataGridViewLotteryDatas.Rows.Clear();
            progressBar1.Value = progressBar1.Minimum;
            richTextBoxResult.Text = "";

        }

        private void textBoxPath012ShortCount_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxPath012ShortCount.Text) == false)
                ColumnSimulateSingleBuyLottery.S_SHORT_COUNT = int.Parse(textBoxPath012ShortCount.Text);
        }

        private void openGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LotteryAnalyze.UI.LotteryGraph.Open(true);
        }

        private void getLatestDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshLatestData();
            if (updateTimer == null)
            {
                updateTimer = new System.Windows.Forms.Timer();
                updateTimer.Interval = 10000;
                updateTimer.Tick += UpdateTimer_Tick;
                updateTimer.Enabled = true;
                updateTimer.Start();
            }
            MessageBox.Show("更新数据完成！");
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            RefreshLatestData();
        }

        private void collectDatasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LotteryAnalyze.UI.CollectDataWindow win = new UI.CollectDataWindow();
            win.Show();
        }

        int GetFileIndex( int dateID )
        {
            for(int i = listViewFileList.Items.Count-1; i >= 0; --i)
            {
                int tag = (int)listViewFileList.Items[i].Tag;
                if (tag == dateID)
                    return i;
            }
            return -1;
        }

        void RefreshLatestData()
        {
            //Process p = Process.Start("AutoFetchDailyData.exe");
            //p.WaitForExit();//关键，等待外部程序退出后才能往下执行

            // 更新当天的数据
            int currentFetchCount = AutoUpdateUtil.AutoFetchTodayData();
            // 如果数据没变化，直接返回
            if (currentFetchCount == lastFetchCount)
                return;
            lastFetchCount = currentFetchCount;
            
            // 如果文件列表是空的，读取数据文件列表
            if (listViewFileList.Items.Count == 0)
            {
                DirectoryInfo di = new DirectoryInfo("..\\data");
                LoopSearchFolder(di);
                RefreshFileList();
            }

            DataManager dataMgr = DataManager.GetInst();

            // 检查今日的数据是否已经加载过
            DateTime curDate = DateTime.Now;
            string dateTag = AutoUpdateUtil.combineDateString(curDate.Year, curDate.Month, curDate.Day);
            string filePath = AutoUpdateUtil.combineFileName(curDate.Year, curDate.Month, curDate.Day);
            ListViewItem[] res = listViewFileList.Items.Find(dateTag, true);
            // 是新的数据文件，就加入到文件列表中
            if(res.Length == 0)
            {
                int id = int.Parse(dateTag);
                dataMgr.AddMetaInfo(id, filePath);

                ListViewItem item = new ListViewItem();
                item.Name = dateTag;
                item.Text = dateTag;
                item.Tag = id;
                listViewFileList.Items.Add(item);
            }

            int newAddItemIndex = -1, tmpAddItemIndex = -1;
            OneDayDatas newAddODD = null, tmpAddODD = null, firstODD = dataMgr.GetFirstOneDayDatas();

            int lastItemID = listViewFileList.Items.Count - 1;
            if (lastItemID > 0)
                --lastItemID;
            if (firstODD != null)
                lastItemID = GetFileIndex(firstODD.dateID);

            while (lastItemID != listViewFileList.Items.Count && lastItemID >= 0)
            {
                ListViewItem item = listViewFileList.Items[lastItemID];
                item.Focused = true;
                int key = (int)(item.Tag);
                dataMgr.LoadDataExt(key, ref tmpAddODD, ref tmpAddItemIndex);
                if(newAddItemIndex == -1 && tmpAddItemIndex != -1)
                {
                    newAddODD = tmpAddODD;
                    newAddItemIndex = tmpAddItemIndex;
                }
                ++lastItemID;
            }
            Util.CollectPath012Info(null, newAddODD, newAddItemIndex);
            RefreshDataView();

            GraphDataManager.Instance.CollectGraphData(GraphType.eKCurveGraph);
            LotteryAnalyze.UI.LotteryGraph.Open(false);
            LotteryAnalyze.UI.LotteryGraph.NotifyAllGraphsRefresh(newAddItemIndex != -1);
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer = null;
            }
        }

        private void globalSimTradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UI.GlobalSimTradeWindow.Open();
        }

        private void tradeCalculaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LotteryAnalyze.UI.TradeCalculater.Open();
        }
    }
}
