﻿//#define FIX_DISIGNER

namespace LotteryAnalyze.UI
{
    partial class LotteryGraph
    {
//        void CreateUpAndDownPanel()
//        {
//#if FIX_DISIGNER
//            //this.panelUp = new System.Windows.Forms.Panel();
//#else
//            if(this.panelUp != null)
//            {
//                this.panelUp.Dispose();
//                this.panelUp = null;
//            }
//            this.panelUp = new ExtPanel();
//#endif
//#if FIX_DISIGNER
//            //this.panelDown = new System.Windows.Forms.Panel();
//#else
//            if (this.panelDown != null)
//            {
//                this.panelDown.Dispose();
//                this.panelDown = null;
//            }
//            this.panelDown = new ExtPanel();
//#endif
//        }


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStripGraph = new System.Windows.Forms.MenuStrip();
            this.operationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoAllignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.delAllAuxLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadNextBatchDatasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshLatestDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tradeSimFromFirstToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tradeSimFromLatestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseSimTradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeSimTradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simTradeOneStepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopSimTradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllTradeDatasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.globalSimTradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tradeCalculaterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panelUp = new CustomUI.DoubleBufferPanel();
            this.contextMenuStripRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.delSelAuxLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.delAllLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelAddAuxLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modifyAuxLineColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoAddAuxLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.setBreakPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonVertExpand = new System.Windows.Forms.Button();
            this.buttonHorzExpand = new System.Windows.Forms.Button();
            this.panelDown = new CustomUI.DoubleBufferPanel();
            this.textBoxRefreshTimeLength = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.buttonClearFavoriteCharts = new System.Windows.Forms.Button();
            this.buttonAutoCalcFavotiteCharts = new System.Windows.Forms.Button();
            this.buttonAddFavoriteChart = new System.Windows.Forms.Button();
            this.listBoxFavoriteCharts = new System.Windows.Forms.ListBox();
            this.textBoxGridScaleH = new System.Windows.Forms.TextBox();
            this.textBoxGridScaleW = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControlView = new System.Windows.Forms.TabControl();
            this.tabPageKGraph = new System.Windows.Forms.TabPage();
            this.comboBoxKDataCircle = new System.Windows.Forms.ComboBox();
            this.checkBoxKRuler = new System.Windows.Forms.CheckBox();
            this.trackBarKData = new System.Windows.Forms.TrackBar();
            this.checkBoxShowAuxLines = new System.Windows.Forms.CheckBox();
            this.comboBoxOperations = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxShowAvgLines = new System.Windows.Forms.CheckBox();
            this.checkBoxMACD = new System.Windows.Forms.CheckBox();
            this.checkBoxBollinBand = new System.Windows.Forms.CheckBox();
            this.comboBoxAvgAlgorithm = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBoxAvgSettings = new System.Windows.Forms.GroupBox();
            this.buttonAvg100 = new System.Windows.Forms.Button();
            this.checkBoxAvg100 = new System.Windows.Forms.CheckBox();
            this.buttonAvg50 = new System.Windows.Forms.Button();
            this.checkBoxAvg50 = new System.Windows.Forms.CheckBox();
            this.buttonAvg30 = new System.Windows.Forms.Button();
            this.checkBoxAvg30 = new System.Windows.Forms.CheckBox();
            this.buttonAvg20 = new System.Windows.Forms.Button();
            this.checkBoxAvg20 = new System.Windows.Forms.CheckBox();
            this.buttonAvg10 = new System.Windows.Forms.Button();
            this.checkBoxAvg10 = new System.Windows.Forms.CheckBox();
            this.buttonAvg5 = new System.Windows.Forms.Button();
            this.checkBoxAvg5 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPageBarGraph = new System.Windows.Forms.TabPage();
            this.textBoxCustomCollectRange = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxCollectRange = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxBarCollectType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPageTrade = new System.Windows.Forms.TabPage();
            this.comboBoxTradeStrategy = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnSetAsStartTrade = new System.Windows.Forms.Button();
            this.textBoxStartDataItem = new System.Windows.Forms.TextBox();
            this.trackBarTradeData = new System.Windows.Forms.TrackBar();
            this.textBoxStartMoney = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.comboBoxTradeNumIndex = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBoxTradeSpecNumIndex = new System.Windows.Forms.CheckBox();
            this.buttonCommitTradeCount = new System.Windows.Forms.Button();
            this.textBoxDefaultCount = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxMultiCount = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPageAppearence = new System.Windows.Forms.TabPage();
            this.trackBarAppearRate = new System.Windows.Forms.TrackBar();
            this.label18 = new System.Windows.Forms.Label();
            this.comboBoxAppearenceType = new System.Windows.Forms.ComboBox();
            this.groupBoxCDTShowSetting = new System.Windows.Forms.GroupBox();
            this.checkBoxShowSingleLine = new System.Windows.Forms.CheckBox();
            this.tabPageMissCount = new System.Windows.Forms.TabPage();
            this.trackBarMissCount = new System.Windows.Forms.TrackBar();
            this.label16 = new System.Windows.Forms.Label();
            this.comboBoxMissCountType = new System.Windows.Forms.ComboBox();
            this.groupBoxMissCountCDTShowSetting = new System.Windows.Forms.GroupBox();
            this.checkBoxMissCountShowSingleLine = new System.Windows.Forms.CheckBox();
            this.comboBoxCollectionDataType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxNumIndex = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarGraphBar = new System.Windows.Forms.TrackBar();
            this.buttonPrevItem = new System.Windows.Forms.Button();
            this.buttonNextItem = new System.Windows.Forms.Button();
            this.menuStripGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panelUp.SuspendLayout();
            this.contextMenuStripRightClick.SuspendLayout();
            this.tabControlView.SuspendLayout();
            this.tabPageKGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarKData)).BeginInit();
            this.groupBoxAvgSettings.SuspendLayout();
            this.tabPageBarGraph.SuspendLayout();
            this.tabPageTrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTradeData)).BeginInit();
            this.tabPageAppearence.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAppearRate)).BeginInit();
            this.tabPageMissCount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMissCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGraphBar)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStripGraph
            // 
            this.menuStripGraph.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.operationToolStripMenuItem,
            this.设置ToolStripMenuItem});
            this.menuStripGraph.Location = new System.Drawing.Point(0, 0);
            this.menuStripGraph.Name = "menuStripGraph";
            this.menuStripGraph.Size = new System.Drawing.Size(921, 25);
            this.menuStripGraph.TabIndex = 0;
            this.menuStripGraph.Text = "menuStripGraph";
            // 
            // operationToolStripMenuItem
            // 
            this.operationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.autoAllignToolStripMenuItem,
            this.delAllAuxLinesToolStripMenuItem,
            this.loadNextBatchDatasToolStripMenuItem,
            this.refreshLatestDataToolStripMenuItem});
            this.operationToolStripMenuItem.Name = "operationToolStripMenuItem";
            this.operationToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.operationToolStripMenuItem.Text = "操作";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.refreshToolStripMenuItem.Text = "刷新";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // autoAllignToolStripMenuItem
            // 
            this.autoAllignToolStripMenuItem.Name = "autoAllignToolStripMenuItem";
            this.autoAllignToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.autoAllignToolStripMenuItem.Text = "对齐";
            this.autoAllignToolStripMenuItem.Click += new System.EventHandler(this.autoAllignToolStripMenuItem_Click);
            // 
            // delAllAuxLinesToolStripMenuItem
            // 
            this.delAllAuxLinesToolStripMenuItem.Name = "delAllAuxLinesToolStripMenuItem";
            this.delAllAuxLinesToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.delAllAuxLinesToolStripMenuItem.Text = "清除所有辅助线";
            this.delAllAuxLinesToolStripMenuItem.Click += new System.EventHandler(this.delAllAuxLinesToolStripMenuItem_Click);
            // 
            // loadNextBatchDatasToolStripMenuItem
            // 
            this.loadNextBatchDatasToolStripMenuItem.Name = "loadNextBatchDatasToolStripMenuItem";
            this.loadNextBatchDatasToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.loadNextBatchDatasToolStripMenuItem.Text = "读取下一批数据";
            this.loadNextBatchDatasToolStripMenuItem.Click += new System.EventHandler(this.loadNextBatchDatasToolStripMenuItem_Click);
            // 
            // refreshLatestDataToolStripMenuItem
            // 
            this.refreshLatestDataToolStripMenuItem.Name = "refreshLatestDataToolStripMenuItem";
            this.refreshLatestDataToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.refreshLatestDataToolStripMenuItem.Text = "获取最新数据";
            this.refreshLatestDataToolStripMenuItem.Click += new System.EventHandler(this.refreshLatestDataToolStripMenuItem_Click);
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tradeSimFromFirstToolStripMenuItem,
            this.tradeSimFromLatestToolStripMenuItem,
            this.pauseSimTradeToolStripMenuItem,
            this.resumeSimTradeToolStripMenuItem,
            this.simTradeOneStepToolStripMenuItem,
            this.stopSimTradeToolStripMenuItem,
            this.toolStripSeparator1,
            this.tradeToolStripMenuItem,
            this.clearAllTradeDatasToolStripMenuItem,
            this.toolStripSeparator2,
            this.globalSimTradeToolStripMenuItem,
            this.tradeCalculaterToolStripMenuItem});
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.设置ToolStripMenuItem.Text = "交易";
            // 
            // tradeSimFromFirstToolStripMenuItem
            // 
            this.tradeSimFromFirstToolStripMenuItem.Name = "tradeSimFromFirstToolStripMenuItem";
            this.tradeSimFromFirstToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.tradeSimFromFirstToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.tradeSimFromFirstToolStripMenuItem.Text = "从第一期开始模拟";
            this.tradeSimFromFirstToolStripMenuItem.Click += new System.EventHandler(this.tradeSimFromFirstToolStripMenuItem_Click);
            // 
            // tradeSimFromLatestToolStripMenuItem
            // 
            this.tradeSimFromLatestToolStripMenuItem.Name = "tradeSimFromLatestToolStripMenuItem";
            this.tradeSimFromLatestToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.tradeSimFromLatestToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.tradeSimFromLatestToolStripMenuItem.Text = "从最后期开始模拟";
            this.tradeSimFromLatestToolStripMenuItem.Click += new System.EventHandler(this.tradeSimFromLatestToolStripMenuItem_Click);
            // 
            // pauseSimTradeToolStripMenuItem
            // 
            this.pauseSimTradeToolStripMenuItem.Name = "pauseSimTradeToolStripMenuItem";
            this.pauseSimTradeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.pauseSimTradeToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.pauseSimTradeToolStripMenuItem.Text = "暂停模拟";
            this.pauseSimTradeToolStripMenuItem.Click += new System.EventHandler(this.pauseSimTradeToolStripMenuItem_Click);
            // 
            // resumeSimTradeToolStripMenuItem
            // 
            this.resumeSimTradeToolStripMenuItem.Name = "resumeSimTradeToolStripMenuItem";
            this.resumeSimTradeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.resumeSimTradeToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.resumeSimTradeToolStripMenuItem.Text = "恢复模拟";
            this.resumeSimTradeToolStripMenuItem.Click += new System.EventHandler(this.resumeSimTradeToolStripMenuItem_Click);
            // 
            // simTradeOneStepToolStripMenuItem
            // 
            this.simTradeOneStepToolStripMenuItem.Name = "simTradeOneStepToolStripMenuItem";
            this.simTradeOneStepToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.simTradeOneStepToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.simTradeOneStepToolStripMenuItem.Text = "模拟交易一次";
            this.simTradeOneStepToolStripMenuItem.Click += new System.EventHandler(this.simTradeOneStepToolStripMenuItem_Click);
            // 
            // stopSimTradeToolStripMenuItem
            // 
            this.stopSimTradeToolStripMenuItem.Name = "stopSimTradeToolStripMenuItem";
            this.stopSimTradeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.stopSimTradeToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.stopSimTradeToolStripMenuItem.Text = "停止模拟";
            this.stopSimTradeToolStripMenuItem.Click += new System.EventHandler(this.stopSimTradeToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(238, 6);
            // 
            // tradeToolStripMenuItem
            // 
            this.tradeToolStripMenuItem.Name = "tradeToolStripMenuItem";
            this.tradeToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.tradeToolStripMenuItem.Text = "下单";
            this.tradeToolStripMenuItem.Click += new System.EventHandler(this.tradeToolStripMenuItem_Click);
            // 
            // clearAllTradeDatasToolStripMenuItem
            // 
            this.clearAllTradeDatasToolStripMenuItem.Name = "clearAllTradeDatasToolStripMenuItem";
            this.clearAllTradeDatasToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.A)));
            this.clearAllTradeDatasToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.clearAllTradeDatasToolStripMenuItem.Text = "清空所有交易数据";
            this.clearAllTradeDatasToolStripMenuItem.Click += new System.EventHandler(this.clearAllTradeDatasToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(238, 6);
            // 
            // globalSimTradeToolStripMenuItem
            // 
            this.globalSimTradeToolStripMenuItem.Name = "globalSimTradeToolStripMenuItem";
            this.globalSimTradeToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.globalSimTradeToolStripMenuItem.Text = "模拟交易所有历史数据";
            this.globalSimTradeToolStripMenuItem.Click += new System.EventHandler(this.globalSimTradeToolStripMenuItem_Click);
            // 
            // tradeCalculaterToolStripMenuItem
            // 
            this.tradeCalculaterToolStripMenuItem.Name = "tradeCalculaterToolStripMenuItem";
            this.tradeCalculaterToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.tradeCalculaterToolStripMenuItem.Text = "交易计算器";
            this.tradeCalculaterToolStripMenuItem.Click += new System.EventHandler(this.tradeCalculaterToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBoxRefreshTimeLength);
            this.splitContainer1.Panel2.Controls.Add(this.label17);
            this.splitContainer1.Panel2.Controls.Add(this.buttonClearFavoriteCharts);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAutoCalcFavotiteCharts);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAddFavoriteChart);
            this.splitContainer1.Panel2.Controls.Add(this.listBoxFavoriteCharts);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxGridScaleH);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxGridScaleW);
            this.splitContainer1.Panel2.Controls.Add(this.label9);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.tabControlView);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxCollectionDataType);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxNumIndex);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(921, 523);
            this.splitContainer1.SplitterDistance = 586;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panelUp);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panelDown);
            this.splitContainer2.Size = new System.Drawing.Size(586, 523);
            this.splitContainer2.SplitterDistance = 319;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.splitContainer2_SplitterMoving);
            // 
            // panelUp
            // 
            this.panelUp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelUp.ContextMenuStrip = this.contextMenuStripRightClick;
            this.panelUp.Controls.Add(this.buttonVertExpand);
            this.panelUp.Controls.Add(this.buttonHorzExpand);
            this.panelUp.Location = new System.Drawing.Point(3, 4);
            this.panelUp.Name = "panelUp";
            this.panelUp.Size = new System.Drawing.Size(580, 312);
            this.panelUp.TabIndex = 0;
            this.panelUp.Paint += new System.Windows.Forms.PaintEventHandler(this.panelUp_Paint);
            this.panelUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelUp_MouseDown);
            this.panelUp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelUp_MouseMove);
            this.panelUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelUp_MouseUp);
            this.panelUp.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
            // 
            // contextMenuStripRightClick
            // 
            this.contextMenuStripRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.delSelAuxLineToolStripMenuItem,
            this.delAllLinesToolStripMenuItem,
            this.cancelAddAuxLineToolStripMenuItem,
            this.modifyAuxLineColorToolStripMenuItem,
            this.autoAddAuxLineToolStripMenuItem,
            this.toolStripSeparator3,
            this.setBreakPointToolStripMenuItem});
            this.contextMenuStripRightClick.Name = "contextMenuStripRightClick";
            this.contextMenuStripRightClick.Size = new System.Drawing.Size(173, 142);
            // 
            // delSelAuxLineToolStripMenuItem
            // 
            this.delSelAuxLineToolStripMenuItem.Name = "delSelAuxLineToolStripMenuItem";
            this.delSelAuxLineToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.delSelAuxLineToolStripMenuItem.Text = "删除选中的辅助线";
            this.delSelAuxLineToolStripMenuItem.Click += new System.EventHandler(this.delSelAuxLineToolStripMenuItem_Click);
            // 
            // delAllLinesToolStripMenuItem
            // 
            this.delAllLinesToolStripMenuItem.Name = "delAllLinesToolStripMenuItem";
            this.delAllLinesToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.delAllLinesToolStripMenuItem.Text = "删除所有辅助线";
            this.delAllLinesToolStripMenuItem.Click += new System.EventHandler(this.delAllAuxLinesToolStripMenuItem_Click);
            // 
            // cancelAddAuxLineToolStripMenuItem
            // 
            this.cancelAddAuxLineToolStripMenuItem.Name = "cancelAddAuxLineToolStripMenuItem";
            this.cancelAddAuxLineToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.cancelAddAuxLineToolStripMenuItem.Text = "取消当前辅助线";
            this.cancelAddAuxLineToolStripMenuItem.Click += new System.EventHandler(this.cancelAddAuxLineToolStripMenuItem_Click);
            // 
            // modifyAuxLineColorToolStripMenuItem
            // 
            this.modifyAuxLineColorToolStripMenuItem.Name = "modifyAuxLineColorToolStripMenuItem";
            this.modifyAuxLineColorToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.modifyAuxLineColorToolStripMenuItem.Text = "修改辅助线颜色";
            this.modifyAuxLineColorToolStripMenuItem.Click += new System.EventHandler(this.modifyAuxLineColorToolStripMenuItem_Click);
            // 
            // autoAddAuxLineToolStripMenuItem
            // 
            this.autoAddAuxLineToolStripMenuItem.Name = "autoAddAuxLineToolStripMenuItem";
            this.autoAddAuxLineToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.autoAddAuxLineToolStripMenuItem.Text = "自动添加辅助线";
            this.autoAddAuxLineToolStripMenuItem.Click += new System.EventHandler(this.autoAddAuxLineToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(169, 6);
            // 
            // setBreakPointToolStripMenuItem
            // 
            this.setBreakPointToolStripMenuItem.Name = "setBreakPointToolStripMenuItem";
            this.setBreakPointToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.setBreakPointToolStripMenuItem.Text = "设置断点";
            this.setBreakPointToolStripMenuItem.Click += new System.EventHandler(this.setBreakPointToolStripMenuItem_Click);
            // 
            // buttonVertExpand
            // 
            this.buttonVertExpand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonVertExpand.Location = new System.Drawing.Point(0, 297);
            this.buttonVertExpand.Name = "buttonVertExpand";
            this.buttonVertExpand.Size = new System.Drawing.Size(580, 15);
            this.buttonVertExpand.TabIndex = 1;
            this.buttonVertExpand.UseVisualStyleBackColor = true;
            this.buttonVertExpand.Click += new System.EventHandler(this.buttonVertExpand_Click);
            // 
            // buttonHorzExpand
            // 
            this.buttonHorzExpand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonHorzExpand.BackColor = System.Drawing.SystemColors.Control;
            this.buttonHorzExpand.FlatAppearance.BorderColor = System.Drawing.Color.Fuchsia;
            this.buttonHorzExpand.FlatAppearance.BorderSize = 0;
            this.buttonHorzExpand.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Blue;
            this.buttonHorzExpand.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.buttonHorzExpand.Location = new System.Drawing.Point(565, 0);
            this.buttonHorzExpand.Name = "buttonHorzExpand";
            this.buttonHorzExpand.Size = new System.Drawing.Size(15, 312);
            this.buttonHorzExpand.TabIndex = 0;
            this.buttonHorzExpand.UseVisualStyleBackColor = false;
            this.buttonHorzExpand.Click += new System.EventHandler(this.buttonHorzExpand_Click);
            // 
            // panelDown
            // 
            this.panelDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDown.Location = new System.Drawing.Point(3, 3);
            this.panelDown.Name = "panelDown";
            this.panelDown.Size = new System.Drawing.Size(580, 194);
            this.panelDown.TabIndex = 0;
            this.panelDown.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDown_Paint);
            this.panelDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelDown_MouseDown);
            this.panelDown.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelDown_MouseMove);
            this.panelDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelDown_MouseUp);
            this.panelDown.Resize += new System.EventHandler(this.panelDown_Resize);
            // 
            // textBoxRefreshTimeLength
            // 
            this.textBoxRefreshTimeLength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRefreshTimeLength.Location = new System.Drawing.Point(87, 25);
            this.textBoxRefreshTimeLength.Name = "textBoxRefreshTimeLength";
            this.textBoxRefreshTimeLength.Size = new System.Drawing.Size(241, 21);
            this.textBoxRefreshTimeLength.TabIndex = 15;
            this.textBoxRefreshTimeLength.TextChanged += new System.EventHandler(this.textBoxRefreshTimeLength_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(5, 28);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(65, 12);
            this.label17.TabIndex = 14;
            this.label17.Text = "刷新时间：";
            // 
            // buttonClearFavoriteCharts
            // 
            this.buttonClearFavoriteCharts.Location = new System.Drawing.Point(7, 130);
            this.buttonClearFavoriteCharts.Name = "buttonClearFavoriteCharts";
            this.buttonClearFavoriteCharts.Size = new System.Drawing.Size(75, 23);
            this.buttonClearFavoriteCharts.TabIndex = 13;
            this.buttonClearFavoriteCharts.Text = "清空";
            this.buttonClearFavoriteCharts.UseVisualStyleBackColor = true;
            this.buttonClearFavoriteCharts.Click += new System.EventHandler(this.buttonClearFavoriteCharts_Click);
            //
            // buttonAutoCalcFavotiteCharts
            //
            this.buttonAutoCalcFavotiteCharts.Location = new System.Drawing.Point(7, 155);
            this.buttonAutoCalcFavotiteCharts.Name = "buttonAutoCalcFavotiteCharts";
            this.buttonAutoCalcFavotiteCharts.Size = new System.Drawing.Size(75, 23);
            this.buttonAutoCalcFavotiteCharts.TabIndex = 14;
            this.buttonAutoCalcFavotiteCharts.Text = "自动筛选";
            this.buttonAutoCalcFavotiteCharts.UseVisualStyleBackColor = true;
            this.buttonAutoCalcFavotiteCharts.Click += new System.EventHandler(this.buttonAutoCalcFavotiteCharts_Click);
            // 
            // buttonAddFavoriteChart
            // 
            this.buttonAddFavoriteChart.Location = new System.Drawing.Point(7, 105);
            this.buttonAddFavoriteChart.Name = "buttonAddFavoriteChart";
            this.buttonAddFavoriteChart.Size = new System.Drawing.Size(75, 23);
            this.buttonAddFavoriteChart.TabIndex = 12;
            this.buttonAddFavoriteChart.Text = "添加";
            this.buttonAddFavoriteChart.UseVisualStyleBackColor = true;
            this.buttonAddFavoriteChart.Click += new System.EventHandler(this.buttonAddFavoriteChart_Click);
            // 
            // listBoxFavoriteCharts
            // 
            this.listBoxFavoriteCharts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxFavoriteCharts.FormattingEnabled = true;
            this.listBoxFavoriteCharts.ItemHeight = 12;
            this.listBoxFavoriteCharts.Location = new System.Drawing.Point(87, 105);
            this.listBoxFavoriteCharts.Name = "listBoxFavoriteCharts";
            this.listBoxFavoriteCharts.Size = new System.Drawing.Size(241, 76);
            this.listBoxFavoriteCharts.TabIndex = 11;
            this.listBoxFavoriteCharts.SelectedIndexChanged += new System.EventHandler(this.listBoxFavoriteCharts_SelectedIndexChanged);
            this.listBoxFavoriteCharts.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.listBoxFavoriteCharts_PreviewKeyDown);
            // 
            // textBoxGridScaleH
            // 
            this.textBoxGridScaleH.Location = new System.Drawing.Point(209, 1);
            this.textBoxGridScaleH.Name = "textBoxGridScaleH";
            this.textBoxGridScaleH.Size = new System.Drawing.Size(66, 21);
            this.textBoxGridScaleH.TabIndex = 10;
            this.textBoxGridScaleH.TextChanged += new System.EventHandler(this.textBoxGridScaleH_TextChanged);
            // 
            // textBoxGridScaleW
            // 
            this.textBoxGridScaleW.Location = new System.Drawing.Point(129, 1);
            this.textBoxGridScaleW.Name = "textBoxGridScaleW";
            this.textBoxGridScaleW.Size = new System.Drawing.Size(67, 21);
            this.textBoxGridScaleW.TabIndex = 9;
            this.textBoxGridScaleW.TextChanged += new System.EventHandler(this.textBoxGridScaleW_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 4);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(119, 12);
            this.label9.TabIndex = 8;
            this.label9.Text = "视图缩放 (宽，高)：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "切换视图：";
            // 
            // tabControlView
            // 
            this.tabControlView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlView.Controls.Add(this.tabPageKGraph);
            this.tabControlView.Controls.Add(this.tabPageBarGraph);
            this.tabControlView.Controls.Add(this.tabPageTrade);
            this.tabControlView.Controls.Add(this.tabPageAppearence);
            this.tabControlView.Controls.Add(this.tabPageMissCount);
            this.tabControlView.Location = new System.Drawing.Point(2, 205);
            this.tabControlView.Name = "tabControlView";
            this.tabControlView.SelectedIndex = 0;
            this.tabControlView.Size = new System.Drawing.Size(326, 315);
            this.tabControlView.TabIndex = 6;
            this.tabControlView.SelectedIndexChanged += new System.EventHandler(this.tabControlView_SelectedIndexChanged);
            // 
            // tabPageKGraph
            // 
            this.tabPageKGraph.Controls.Add(this.comboBoxKDataCircle);
            this.tabPageKGraph.Controls.Add(this.checkBoxKRuler);
            this.tabPageKGraph.Controls.Add(this.trackBarKData);
            this.tabPageKGraph.Controls.Add(this.checkBoxShowAuxLines);
            this.tabPageKGraph.Controls.Add(this.comboBoxOperations);
            this.tabPageKGraph.Controls.Add(this.label10);
            this.tabPageKGraph.Controls.Add(this.checkBoxShowAvgLines);
            this.tabPageKGraph.Controls.Add(this.checkBoxMACD);
            this.tabPageKGraph.Controls.Add(this.checkBoxBollinBand);
            this.tabPageKGraph.Controls.Add(this.comboBoxAvgAlgorithm);
            this.tabPageKGraph.Controls.Add(this.label8);
            this.tabPageKGraph.Controls.Add(this.groupBoxAvgSettings);
            this.tabPageKGraph.Controls.Add(this.label3);
            this.tabPageKGraph.Location = new System.Drawing.Point(4, 22);
            this.tabPageKGraph.Name = "tabPageKGraph";
            this.tabPageKGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageKGraph.Size = new System.Drawing.Size(318, 289);
            this.tabPageKGraph.TabIndex = 0;
            this.tabPageKGraph.Text = "K线图";
            this.tabPageKGraph.UseVisualStyleBackColor = true;
            // 
            // comboBoxKDataCircle
            // 
            this.comboBoxKDataCircle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxKDataCircle.FormattingEnabled = true;
            this.comboBoxKDataCircle.Location = new System.Drawing.Point(65, 7);
            this.comboBoxKDataCircle.Margin = new System.Windows.Forms.Padding(8);
            this.comboBoxKDataCircle.Name = "comboBoxKDataCircle";
            this.comboBoxKDataCircle.Size = new System.Drawing.Size(243, 20);
            this.comboBoxKDataCircle.TabIndex = 18;
            this.comboBoxKDataCircle.SelectedIndexChanged += new System.EventHandler(this.comboBoxKDataCircle_SelectedIndexChanged);
            // 
            // checkBoxKRuler
            // 
            this.checkBoxKRuler.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxKRuler.AutoSize = true;
            this.checkBoxKRuler.Location = new System.Drawing.Point(105, 95);
            this.checkBoxKRuler.Name = "checkBoxKRuler";
            this.checkBoxKRuler.Size = new System.Drawing.Size(66, 16);
            this.checkBoxKRuler.TabIndex = 17;
            this.checkBoxKRuler.Text = "K线标尺";
            this.checkBoxKRuler.UseVisualStyleBackColor = true;
            this.checkBoxKRuler.CheckedChanged += new System.EventHandler(this.checkBoxKRuler_CheckedChanged);
            // 
            // trackBarKData
            // 
            this.trackBarKData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarKData.AutoSize = false;
            this.trackBarKData.Location = new System.Drawing.Point(5, 248);
            this.trackBarKData.Name = "trackBarKData";
            this.trackBarKData.Size = new System.Drawing.Size(305, 36);
            this.trackBarKData.TabIndex = 16;
            this.trackBarKData.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarKData.Scroll += new System.EventHandler(this.trackBarKData_Scroll);
            // 
            // checkBoxShowAuxLines
            // 
            this.checkBoxShowAuxLines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowAuxLines.AutoSize = true;
            this.checkBoxShowAuxLines.Location = new System.Drawing.Point(5, 95);
            this.checkBoxShowAuxLines.Name = "checkBoxShowAuxLines";
            this.checkBoxShowAuxLines.Size = new System.Drawing.Size(60, 16);
            this.checkBoxShowAuxLines.TabIndex = 15;
            this.checkBoxShowAuxLines.Text = "辅助线";
            this.checkBoxShowAuxLines.UseVisualStyleBackColor = true;
            this.checkBoxShowAuxLines.CheckedChanged += new System.EventHandler(this.checkBoxShowAuxLines_CheckedChanged);
            // 
            // comboBoxOperations
            // 
            this.comboBoxOperations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxOperations.FormattingEnabled = true;
            this.comboBoxOperations.Items.AddRange(new object[] {
            "0路",
            "1路",
            "2路"});
            this.comboBoxOperations.Location = new System.Drawing.Point(65, 66);
            this.comboBoxOperations.Name = "comboBoxOperations";
            this.comboBoxOperations.Size = new System.Drawing.Size(243, 20);
            this.comboBoxOperations.TabIndex = 14;
            this.comboBoxOperations.SelectedIndexChanged += new System.EventHandler(this.comboBoxOperations_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 69);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 13;
            this.label10.Text = "操作方式：";
            // 
            // checkBoxShowAvgLines
            // 
            this.checkBoxShowAvgLines.AutoSize = true;
            this.checkBoxShowAvgLines.Location = new System.Drawing.Point(5, 139);
            this.checkBoxShowAvgLines.Name = "checkBoxShowAvgLines";
            this.checkBoxShowAvgLines.Size = new System.Drawing.Size(72, 16);
            this.checkBoxShowAvgLines.TabIndex = 12;
            this.checkBoxShowAvgLines.Text = "均线指标";
            this.checkBoxShowAvgLines.UseVisualStyleBackColor = true;
            this.checkBoxShowAvgLines.CheckedChanged += new System.EventHandler(this.checkBoxShowAvgLines_CheckedChanged);
            // 
            // checkBoxMACD
            // 
            this.checkBoxMACD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxMACD.AutoSize = true;
            this.checkBoxMACD.Location = new System.Drawing.Point(105, 117);
            this.checkBoxMACD.Name = "checkBoxMACD";
            this.checkBoxMACD.Size = new System.Drawing.Size(72, 16);
            this.checkBoxMACD.TabIndex = 10;
            this.checkBoxMACD.Text = "MACD指标";
            this.checkBoxMACD.UseVisualStyleBackColor = true;
            this.checkBoxMACD.CheckedChanged += new System.EventHandler(this.checkBoxMACD_CheckedChanged);
            // 
            // checkBoxBollinBand
            // 
            this.checkBoxBollinBand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxBollinBand.AutoSize = true;
            this.checkBoxBollinBand.Location = new System.Drawing.Point(5, 117);
            this.checkBoxBollinBand.Name = "checkBoxBollinBand";
            this.checkBoxBollinBand.Size = new System.Drawing.Size(72, 16);
            this.checkBoxBollinBand.TabIndex = 9;
            this.checkBoxBollinBand.Text = "布林指标";
            this.checkBoxBollinBand.UseVisualStyleBackColor = true;
            this.checkBoxBollinBand.CheckedChanged += new System.EventHandler(this.checkBoxBollinBand_CheckedChanged);
            // 
            // comboBoxAvgAlgorithm
            // 
            this.comboBoxAvgAlgorithm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAvgAlgorithm.FormattingEnabled = true;
            this.comboBoxAvgAlgorithm.Items.AddRange(new object[] {
            "0路",
            "1路",
            "2路"});
            this.comboBoxAvgAlgorithm.Location = new System.Drawing.Point(65, 37);
            this.comboBoxAvgAlgorithm.Name = "comboBoxAvgAlgorithm";
            this.comboBoxAvgAlgorithm.Size = new System.Drawing.Size(243, 20);
            this.comboBoxAvgAlgorithm.TabIndex = 8;
            this.comboBoxAvgAlgorithm.SelectedIndexChanged += new System.EventHandler(this.comboBoxAvgAlgorithm_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "均线计算：";
            // 
            // groupBoxAvgSettings
            // 
            this.groupBoxAvgSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAvgSettings.Controls.Add(this.buttonAvg100);
            this.groupBoxAvgSettings.Controls.Add(this.checkBoxAvg100);
            this.groupBoxAvgSettings.Controls.Add(this.buttonAvg50);
            this.groupBoxAvgSettings.Controls.Add(this.checkBoxAvg50);
            this.groupBoxAvgSettings.Controls.Add(this.buttonAvg30);
            this.groupBoxAvgSettings.Controls.Add(this.checkBoxAvg30);
            this.groupBoxAvgSettings.Controls.Add(this.buttonAvg20);
            this.groupBoxAvgSettings.Controls.Add(this.checkBoxAvg20);
            this.groupBoxAvgSettings.Controls.Add(this.buttonAvg10);
            this.groupBoxAvgSettings.Controls.Add(this.checkBoxAvg10);
            this.groupBoxAvgSettings.Controls.Add(this.buttonAvg5);
            this.groupBoxAvgSettings.Controls.Add(this.checkBoxAvg5);
            this.groupBoxAvgSettings.Location = new System.Drawing.Point(3, 153);
            this.groupBoxAvgSettings.Name = "groupBoxAvgSettings";
            this.groupBoxAvgSettings.Size = new System.Drawing.Size(305, 83);
            this.groupBoxAvgSettings.TabIndex = 6;
            this.groupBoxAvgSettings.TabStop = false;
            // 
            // buttonAvg100
            // 
            this.buttonAvg100.BackColor = System.Drawing.Color.Red;
            this.buttonAvg100.Location = new System.Drawing.Point(140, 57);
            this.buttonAvg100.Name = "buttonAvg100";
            this.buttonAvg100.Size = new System.Drawing.Size(34, 23);
            this.buttonAvg100.TabIndex = 11;
            this.buttonAvg100.UseVisualStyleBackColor = false;
            // 
            // checkBoxAvg100
            // 
            this.checkBoxAvg100.AutoSize = true;
            this.checkBoxAvg100.Location = new System.Drawing.Point(102, 61);
            this.checkBoxAvg100.Name = "checkBoxAvg100";
            this.checkBoxAvg100.Size = new System.Drawing.Size(42, 16);
            this.checkBoxAvg100.TabIndex = 10;
            this.checkBoxAvg100.Text = "100";
            this.checkBoxAvg100.UseVisualStyleBackColor = true;
            this.checkBoxAvg100.CheckedChanged += new System.EventHandler(this.checkBoxAvg100_CheckedChanged);
            // 
            // buttonAvg50
            // 
            this.buttonAvg50.BackColor = System.Drawing.Color.Red;
            this.buttonAvg50.Location = new System.Drawing.Point(42, 57);
            this.buttonAvg50.Name = "buttonAvg50";
            this.buttonAvg50.Size = new System.Drawing.Size(34, 23);
            this.buttonAvg50.TabIndex = 9;
            this.buttonAvg50.UseVisualStyleBackColor = false;
            // 
            // checkBoxAvg50
            // 
            this.checkBoxAvg50.AutoSize = true;
            this.checkBoxAvg50.Location = new System.Drawing.Point(6, 61);
            this.checkBoxAvg50.Name = "checkBoxAvg50";
            this.checkBoxAvg50.Size = new System.Drawing.Size(36, 16);
            this.checkBoxAvg50.TabIndex = 8;
            this.checkBoxAvg50.Text = "50";
            this.checkBoxAvg50.UseVisualStyleBackColor = true;
            this.checkBoxAvg50.CheckedChanged += new System.EventHandler(this.checkBoxAvg50_CheckedChanged);
            // 
            // buttonAvg30
            // 
            this.buttonAvg30.BackColor = System.Drawing.Color.Red;
            this.buttonAvg30.Location = new System.Drawing.Point(140, 33);
            this.buttonAvg30.Name = "buttonAvg30";
            this.buttonAvg30.Size = new System.Drawing.Size(34, 23);
            this.buttonAvg30.TabIndex = 7;
            this.buttonAvg30.UseVisualStyleBackColor = false;
            // 
            // checkBoxAvg30
            // 
            this.checkBoxAvg30.AutoSize = true;
            this.checkBoxAvg30.Location = new System.Drawing.Point(102, 37);
            this.checkBoxAvg30.Name = "checkBoxAvg30";
            this.checkBoxAvg30.Size = new System.Drawing.Size(36, 16);
            this.checkBoxAvg30.TabIndex = 6;
            this.checkBoxAvg30.Text = "30";
            this.checkBoxAvg30.UseVisualStyleBackColor = true;
            this.checkBoxAvg30.CheckedChanged += new System.EventHandler(this.checkBoxAvg30_CheckedChanged);
            // 
            // buttonAvg20
            // 
            this.buttonAvg20.BackColor = System.Drawing.Color.Red;
            this.buttonAvg20.Location = new System.Drawing.Point(42, 33);
            this.buttonAvg20.Name = "buttonAvg20";
            this.buttonAvg20.Size = new System.Drawing.Size(34, 23);
            this.buttonAvg20.TabIndex = 5;
            this.buttonAvg20.UseVisualStyleBackColor = false;
            // 
            // checkBoxAvg20
            // 
            this.checkBoxAvg20.AutoSize = true;
            this.checkBoxAvg20.Location = new System.Drawing.Point(6, 37);
            this.checkBoxAvg20.Name = "checkBoxAvg20";
            this.checkBoxAvg20.Size = new System.Drawing.Size(36, 16);
            this.checkBoxAvg20.TabIndex = 4;
            this.checkBoxAvg20.Text = "20";
            this.checkBoxAvg20.UseVisualStyleBackColor = true;
            this.checkBoxAvg20.CheckedChanged += new System.EventHandler(this.checkBoxAvg20_CheckedChanged);
            // 
            // buttonAvg10
            // 
            this.buttonAvg10.BackColor = System.Drawing.Color.Red;
            this.buttonAvg10.Location = new System.Drawing.Point(140, 10);
            this.buttonAvg10.Name = "buttonAvg10";
            this.buttonAvg10.Size = new System.Drawing.Size(34, 23);
            this.buttonAvg10.TabIndex = 3;
            this.buttonAvg10.UseVisualStyleBackColor = false;
            // 
            // checkBoxAvg10
            // 
            this.checkBoxAvg10.AutoSize = true;
            this.checkBoxAvg10.Location = new System.Drawing.Point(102, 14);
            this.checkBoxAvg10.Name = "checkBoxAvg10";
            this.checkBoxAvg10.Size = new System.Drawing.Size(36, 16);
            this.checkBoxAvg10.TabIndex = 2;
            this.checkBoxAvg10.Text = "10";
            this.checkBoxAvg10.UseVisualStyleBackColor = true;
            this.checkBoxAvg10.CheckedChanged += new System.EventHandler(this.checkBoxAvg10_CheckedChanged);
            // 
            // buttonAvg5
            // 
            this.buttonAvg5.BackColor = System.Drawing.Color.Red;
            this.buttonAvg5.Location = new System.Drawing.Point(42, 10);
            this.buttonAvg5.Name = "buttonAvg5";
            this.buttonAvg5.Size = new System.Drawing.Size(34, 23);
            this.buttonAvg5.TabIndex = 1;
            this.buttonAvg5.UseVisualStyleBackColor = false;
            // 
            // checkBoxAvg5
            // 
            this.checkBoxAvg5.AutoSize = true;
            this.checkBoxAvg5.Location = new System.Drawing.Point(6, 14);
            this.checkBoxAvg5.Name = "checkBoxAvg5";
            this.checkBoxAvg5.Size = new System.Drawing.Size(30, 16);
            this.checkBoxAvg5.TabIndex = 0;
            this.checkBoxAvg5.Text = "5";
            this.checkBoxAvg5.UseVisualStyleBackColor = true;
            this.checkBoxAvg5.CheckedChanged += new System.EventHandler(this.checkBoxAvg5_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "K值周期：";
            // 
            // tabPageBarGraph
            // 
            this.tabPageBarGraph.Controls.Add(this.buttonNextItem);
            this.tabPageBarGraph.Controls.Add(this.buttonPrevItem);
            this.tabPageBarGraph.Controls.Add(this.trackBarGraphBar);
            this.tabPageBarGraph.Controls.Add(this.textBoxCustomCollectRange);
            this.tabPageBarGraph.Controls.Add(this.label7);
            this.tabPageBarGraph.Controls.Add(this.comboBoxCollectRange);
            this.tabPageBarGraph.Controls.Add(this.label6);
            this.tabPageBarGraph.Controls.Add(this.comboBoxBarCollectType);
            this.tabPageBarGraph.Controls.Add(this.label5);
            this.tabPageBarGraph.Location = new System.Drawing.Point(4, 22);
            this.tabPageBarGraph.Name = "tabPageBarGraph";
            this.tabPageBarGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBarGraph.Size = new System.Drawing.Size(318, 289);
            this.tabPageBarGraph.TabIndex = 1;
            this.tabPageBarGraph.Text = "柱状图";
            this.tabPageBarGraph.UseVisualStyleBackColor = true;
            // 
            // textBoxCustomCollectRange
            // 
            this.textBoxCustomCollectRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCustomCollectRange.Location = new System.Drawing.Point(3, 100);
            this.textBoxCustomCollectRange.Name = "textBoxCustomCollectRange";
            this.textBoxCustomCollectRange.Size = new System.Drawing.Size(309, 21);
            this.textBoxCustomCollectRange.TabIndex = 5;
            this.textBoxCustomCollectRange.TextChanged += new System.EventHandler(this.textBoxCustomCollectRange_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 84);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "自定义统计范围：";
            // 
            // comboBoxCollectRange
            // 
            this.comboBoxCollectRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCollectRange.FormattingEnabled = true;
            this.comboBoxCollectRange.Location = new System.Drawing.Point(3, 61);
            this.comboBoxCollectRange.Name = "comboBoxCollectRange";
            this.comboBoxCollectRange.Size = new System.Drawing.Size(309, 20);
            this.comboBoxCollectRange.TabIndex = 3;
            this.comboBoxCollectRange.SelectedIndexChanged += new System.EventHandler(this.comboBoxCollectRange_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "统计范围：";
            // 
            // comboBoxBarCollectType
            // 
            this.comboBoxBarCollectType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxBarCollectType.FormattingEnabled = true;
            this.comboBoxBarCollectType.Location = new System.Drawing.Point(3, 23);
            this.comboBoxBarCollectType.Name = "comboBoxBarCollectType";
            this.comboBoxBarCollectType.Size = new System.Drawing.Size(309, 20);
            this.comboBoxBarCollectType.TabIndex = 1;
            this.comboBoxBarCollectType.SelectedIndexChanged += new System.EventHandler(this.comboBoxBarCollectType_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "统计类型：";
            // 
            // tabPageTrade
            // 
            this.tabPageTrade.Controls.Add(this.comboBoxTradeStrategy);
            this.tabPageTrade.Controls.Add(this.label15);
            this.tabPageTrade.Controls.Add(this.btnSetAsStartTrade);
            this.tabPageTrade.Controls.Add(this.textBoxStartDataItem);
            this.tabPageTrade.Controls.Add(this.trackBarTradeData);
            this.tabPageTrade.Controls.Add(this.textBoxStartMoney);
            this.tabPageTrade.Controls.Add(this.label14);
            this.tabPageTrade.Controls.Add(this.comboBoxTradeNumIndex);
            this.tabPageTrade.Controls.Add(this.label13);
            this.tabPageTrade.Controls.Add(this.checkBoxTradeSpecNumIndex);
            this.tabPageTrade.Controls.Add(this.buttonCommitTradeCount);
            this.tabPageTrade.Controls.Add(this.textBoxDefaultCount);
            this.tabPageTrade.Controls.Add(this.label12);
            this.tabPageTrade.Controls.Add(this.textBoxMultiCount);
            this.tabPageTrade.Controls.Add(this.label11);
            this.tabPageTrade.Location = new System.Drawing.Point(4, 22);
            this.tabPageTrade.Name = "tabPageTrade";
            this.tabPageTrade.Size = new System.Drawing.Size(318, 289);
            this.tabPageTrade.TabIndex = 2;
            this.tabPageTrade.Text = "交易图";
            this.tabPageTrade.UseVisualStyleBackColor = true;
            // 
            // comboBoxTradeStrategy
            // 
            this.comboBoxTradeStrategy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTradeStrategy.FormattingEnabled = true;
            this.comboBoxTradeStrategy.Location = new System.Drawing.Point(79, 133);
            this.comboBoxTradeStrategy.Name = "comboBoxTradeStrategy";
            this.comboBoxTradeStrategy.Size = new System.Drawing.Size(234, 20);
            this.comboBoxTradeStrategy.TabIndex = 14;
            this.comboBoxTradeStrategy.SelectedIndexChanged += new System.EventHandler(this.comboBoxTradeStrategy_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 136);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(65, 12);
            this.label15.TabIndex = 13;
            this.label15.Text = "交易策略：";
            // 
            // btnSetAsStartTrade
            // 
            this.btnSetAsStartTrade.Location = new System.Drawing.Point(1, 190);
            this.btnSetAsStartTrade.Name = "btnSetAsStartTrade";
            this.btnSetAsStartTrade.Size = new System.Drawing.Size(75, 23);
            this.btnSetAsStartTrade.TabIndex = 12;
            this.btnSetAsStartTrade.Text = "设为起始期";
            this.btnSetAsStartTrade.UseVisualStyleBackColor = true;
            this.btnSetAsStartTrade.Click += new System.EventHandler(this.btnSetAsStartTrade_Click);
            // 
            // textBoxStartDataItem
            // 
            this.textBoxStartDataItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStartDataItem.Location = new System.Drawing.Point(79, 191);
            this.textBoxStartDataItem.Name = "textBoxStartDataItem";
            this.textBoxStartDataItem.Size = new System.Drawing.Size(234, 21);
            this.textBoxStartDataItem.TabIndex = 11;
            // 
            // trackBarTradeData
            // 
            this.trackBarTradeData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarTradeData.Location = new System.Drawing.Point(3, 241);
            this.trackBarTradeData.Name = "trackBarTradeData";
            this.trackBarTradeData.Size = new System.Drawing.Size(310, 45);
            this.trackBarTradeData.TabIndex = 10;
            this.trackBarTradeData.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarTradeData.Scroll += new System.EventHandler(this.trackBarTradeData_Scroll);
            // 
            // textBoxStartMoney
            // 
            this.textBoxStartMoney.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStartMoney.Location = new System.Drawing.Point(79, 105);
            this.textBoxStartMoney.Name = "textBoxStartMoney";
            this.textBoxStartMoney.Size = new System.Drawing.Size(234, 21);
            this.textBoxStartMoney.TabIndex = 9;
            this.textBoxStartMoney.TextChanged += new System.EventHandler(this.textBoxStartMoney_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 108);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 8;
            this.label14.Text = "初始金额：";
            // 
            // comboBoxTradeNumIndex
            // 
            this.comboBoxTradeNumIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTradeNumIndex.FormattingEnabled = true;
            this.comboBoxTradeNumIndex.Location = new System.Drawing.Point(79, 79);
            this.comboBoxTradeNumIndex.Name = "comboBoxTradeNumIndex";
            this.comboBoxTradeNumIndex.Size = new System.Drawing.Size(234, 20);
            this.comboBoxTradeNumIndex.TabIndex = 7;
            this.comboBoxTradeNumIndex.SelectedIndexChanged += new System.EventHandler(this.comboBoxTradeNumIndex_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 82);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(77, 12);
            this.label13.TabIndex = 6;
            this.label13.Text = "投注数字位：";
            // 
            // checkBoxTradeSpecNumIndex
            // 
            this.checkBoxTradeSpecNumIndex.AutoSize = true;
            this.checkBoxTradeSpecNumIndex.Location = new System.Drawing.Point(3, 57);
            this.checkBoxTradeSpecNumIndex.Name = "checkBoxTradeSpecNumIndex";
            this.checkBoxTradeSpecNumIndex.Size = new System.Drawing.Size(132, 16);
            this.checkBoxTradeSpecNumIndex.TabIndex = 5;
            this.checkBoxTradeSpecNumIndex.Text = "是否投注指定数字位";
            this.checkBoxTradeSpecNumIndex.UseVisualStyleBackColor = true;
            this.checkBoxTradeSpecNumIndex.Click += new System.EventHandler(this.checkBoxTradeSpecNumIndex_Click);
            // 
            // buttonCommitTradeCount
            // 
            this.buttonCommitTradeCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCommitTradeCount.Location = new System.Drawing.Point(1, 159);
            this.buttonCommitTradeCount.Name = "buttonCommitTradeCount";
            this.buttonCommitTradeCount.Size = new System.Drawing.Size(312, 23);
            this.buttonCommitTradeCount.TabIndex = 4;
            this.buttonCommitTradeCount.Text = "提交交易设定";
            this.buttonCommitTradeCount.UseVisualStyleBackColor = true;
            this.buttonCommitTradeCount.Click += new System.EventHandler(this.buttonCommitTradeCount_Click);
            // 
            // textBoxDefaultCount
            // 
            this.textBoxDefaultCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDefaultCount.Location = new System.Drawing.Point(79, 31);
            this.textBoxDefaultCount.Name = "textBoxDefaultCount";
            this.textBoxDefaultCount.Size = new System.Drawing.Size(234, 21);
            this.textBoxDefaultCount.TabIndex = 3;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 34);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 2;
            this.label12.Text = "固定倍率：";
            // 
            // textBoxMultiCount
            // 
            this.textBoxMultiCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMultiCount.Location = new System.Drawing.Point(79, 4);
            this.textBoxMultiCount.Name = "textBoxMultiCount";
            this.textBoxMultiCount.Size = new System.Drawing.Size(234, 21);
            this.textBoxMultiCount.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 7);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "倍投设置：";
            // 
            // tabPageAppearence
            // 
            this.tabPageAppearence.Controls.Add(this.trackBarAppearRate);
            this.tabPageAppearence.Controls.Add(this.label18);
            this.tabPageAppearence.Controls.Add(this.comboBoxAppearenceType);
            this.tabPageAppearence.Controls.Add(this.groupBoxCDTShowSetting);
            this.tabPageAppearence.Controls.Add(this.checkBoxShowSingleLine);
            this.tabPageAppearence.Location = new System.Drawing.Point(4, 22);
            this.tabPageAppearence.Name = "tabPageAppearence";
            this.tabPageAppearence.Size = new System.Drawing.Size(318, 289);
            this.tabPageAppearence.TabIndex = 3;
            this.tabPageAppearence.Text = "出号率图";
            this.tabPageAppearence.UseVisualStyleBackColor = true;
            // 
            // trackBarAppearRate
            // 
            this.trackBarAppearRate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarAppearRate.AutoSize = false;
            this.trackBarAppearRate.Location = new System.Drawing.Point(4, 250);
            this.trackBarAppearRate.Name = "trackBarAppearRate";
            this.trackBarAppearRate.Size = new System.Drawing.Size(309, 36);
            this.trackBarAppearRate.TabIndex = 17;
            this.trackBarAppearRate.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarAppearRate.Scroll += new System.EventHandler(this.trackBarAppearRate_Scroll);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(2, 23);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(65, 12);
            this.label18.TabIndex = 8;
            this.label18.Text = "统计周期：";
            // 
            // comboBoxAppearenceType
            // 
            this.comboBoxAppearenceType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAppearenceType.FormattingEnabled = true;
            this.comboBoxAppearenceType.Location = new System.Drawing.Point(113, 20);
            this.comboBoxAppearenceType.Name = "comboBoxAppearenceType";
            this.comboBoxAppearenceType.Size = new System.Drawing.Size(200, 20);
            this.comboBoxAppearenceType.TabIndex = 7;
            this.comboBoxAppearenceType.SelectedIndexChanged += new System.EventHandler(this.comboBoxAppearenceType_SelectedIndexChanged);
            // 
            // groupBoxCDTShowSetting
            // 
            this.groupBoxCDTShowSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCDTShowSetting.Location = new System.Drawing.Point(4, 46);
            this.groupBoxCDTShowSetting.Name = "groupBoxCDTShowSetting";
            this.groupBoxCDTShowSetting.Size = new System.Drawing.Size(310, 200);
            this.groupBoxCDTShowSetting.TabIndex = 1;
            this.groupBoxCDTShowSetting.TabStop = false;
            this.groupBoxCDTShowSetting.Text = "曲线显示选项";
            // 
            // checkBoxShowSingleLine
            // 
            this.checkBoxShowSingleLine.AutoSize = true;
            this.checkBoxShowSingleLine.Location = new System.Drawing.Point(4, 4);
            this.checkBoxShowSingleLine.Name = "checkBoxShowSingleLine";
            this.checkBoxShowSingleLine.Size = new System.Drawing.Size(108, 16);
            this.checkBoxShowSingleLine.TabIndex = 0;
            this.checkBoxShowSingleLine.Text = "只显示单条曲线";
            this.checkBoxShowSingleLine.UseVisualStyleBackColor = true;
            this.checkBoxShowSingleLine.CheckedChanged += new System.EventHandler(this.checkBoxShowSingleLine_CheckedChanged);
            // 
            // tabPageMissCount
            // 
            this.tabPageMissCount.Controls.Add(this.trackBarMissCount);
            this.tabPageMissCount.Controls.Add(this.label16);
            this.tabPageMissCount.Controls.Add(this.comboBoxMissCountType);
            this.tabPageMissCount.Controls.Add(this.groupBoxMissCountCDTShowSetting);
            this.tabPageMissCount.Controls.Add(this.checkBoxMissCountShowSingleLine);
            this.tabPageMissCount.Location = new System.Drawing.Point(4, 22);
            this.tabPageMissCount.Name = "tabPageMissCount";
            this.tabPageMissCount.Size = new System.Drawing.Size(318, 289);
            this.tabPageMissCount.TabIndex = 4;
            this.tabPageMissCount.Text = "遗漏图";
            this.tabPageMissCount.UseVisualStyleBackColor = true;
            // 
            // trackBarMissCount
            // 
            this.trackBarMissCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarMissCount.AutoSize = false;
            this.trackBarMissCount.Location = new System.Drawing.Point(4, 250);
            this.trackBarMissCount.Name = "trackBarMissCount";
            this.trackBarMissCount.Size = new System.Drawing.Size(305, 36);
            this.trackBarMissCount.TabIndex = 17;
            this.trackBarMissCount.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarMissCount.Scroll += new System.EventHandler(this.trackBarMissCount_Scroll);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 23);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 6;
            this.label16.Text = "遗漏类型：";
            // 
            // comboBoxMissCountType
            // 
            this.comboBoxMissCountType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxMissCountType.FormattingEnabled = true;
            this.comboBoxMissCountType.Location = new System.Drawing.Point(115, 20);
            this.comboBoxMissCountType.Name = "comboBoxMissCountType";
            this.comboBoxMissCountType.Size = new System.Drawing.Size(200, 20);
            this.comboBoxMissCountType.TabIndex = 5;
            this.comboBoxMissCountType.SelectedIndexChanged += new System.EventHandler(this.comboBoxMissCountType_SelectedIndexChanged);
            // 
            // groupBoxMissCountCDTShowSetting
            // 
            this.groupBoxMissCountCDTShowSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMissCountCDTShowSetting.Location = new System.Drawing.Point(4, 46);
            this.groupBoxMissCountCDTShowSetting.Name = "groupBoxMissCountCDTShowSetting";
            this.groupBoxMissCountCDTShowSetting.Size = new System.Drawing.Size(310, 200);
            this.groupBoxMissCountCDTShowSetting.TabIndex = 3;
            this.groupBoxMissCountCDTShowSetting.TabStop = false;
            this.groupBoxMissCountCDTShowSetting.Text = "曲线显示选项";
            // 
            // checkBoxMissCountShowSingleLine
            // 
            this.checkBoxMissCountShowSingleLine.AutoSize = true;
            this.checkBoxMissCountShowSingleLine.Location = new System.Drawing.Point(4, 4);
            this.checkBoxMissCountShowSingleLine.Name = "checkBoxMissCountShowSingleLine";
            this.checkBoxMissCountShowSingleLine.Size = new System.Drawing.Size(108, 16);
            this.checkBoxMissCountShowSingleLine.TabIndex = 2;
            this.checkBoxMissCountShowSingleLine.Text = "只显示单条曲线";
            this.checkBoxMissCountShowSingleLine.UseVisualStyleBackColor = true;
            this.checkBoxMissCountShowSingleLine.CheckedChanged += new System.EventHandler(this.checkBoxMissCountShowSingleLine_CheckedChanged);
            // 
            // comboBoxCollectionDataType
            // 
            this.comboBoxCollectionDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCollectionDataType.FormattingEnabled = true;
            this.comboBoxCollectionDataType.Items.AddRange(new object[] {
            "0路",
            "1路",
            "2路"});
            this.comboBoxCollectionDataType.Location = new System.Drawing.Point(87, 78);
            this.comboBoxCollectionDataType.Name = "comboBoxCollectionDataType";
            this.comboBoxCollectionDataType.Size = new System.Drawing.Size(241, 20);
            this.comboBoxCollectionDataType.TabIndex = 3;
            this.comboBoxCollectionDataType.SelectedIndexChanged += new System.EventHandler(this.comboBoxCollectionDataType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "选择数据类型：";
            // 
            // comboBoxNumIndex
            // 
            this.comboBoxNumIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxNumIndex.AutoCompleteCustomSource.AddRange(new string[] {
            "万位",
            "千位",
            "百位",
            "十位",
            "个位"});
            this.comboBoxNumIndex.FormattingEnabled = true;
            this.comboBoxNumIndex.Items.AddRange(new object[] {
            "万位",
            "千位",
            "百位",
            "十位",
            "个位"});
            this.comboBoxNumIndex.Location = new System.Drawing.Point(87, 52);
            this.comboBoxNumIndex.Name = "comboBoxNumIndex";
            this.comboBoxNumIndex.Size = new System.Drawing.Size(241, 20);
            this.comboBoxNumIndex.TabIndex = 1;
            this.comboBoxNumIndex.SelectedIndexChanged += new System.EventHandler(this.comboBoxNumIndex_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择数字位：";
            // 
            // trackBarGraphBar
            // 
            this.trackBarGraphBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarGraphBar.AutoSize = false;
            this.trackBarGraphBar.Location = new System.Drawing.Point(3, 250);
            this.trackBarGraphBar.Name = "trackBarGraphBar";
            this.trackBarGraphBar.Size = new System.Drawing.Size(309, 36);
            this.trackBarGraphBar.TabIndex = 17;
            this.trackBarGraphBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarGraphBar.Scroll += new System.EventHandler(this.trackBarGraphBar_Scroll);
            // 
            // buttonPrevItem
            // 
            this.buttonPrevItem.Location = new System.Drawing.Point(3, 128);
            this.buttonPrevItem.Name = "buttonPrevItem";
            this.buttonPrevItem.Size = new System.Drawing.Size(75, 23);
            this.buttonPrevItem.TabIndex = 18;
            this.buttonPrevItem.Text = "上一期";
            this.buttonPrevItem.UseVisualStyleBackColor = true;
            this.buttonPrevItem.Click += new System.EventHandler(this.buttonPrevItem_Click);
            // 
            // buttonNextItem
            // 
            this.buttonNextItem.Location = new System.Drawing.Point(85, 128);
            this.buttonNextItem.Name = "buttonNextItem";
            this.buttonNextItem.Size = new System.Drawing.Size(75, 23);
            this.buttonNextItem.TabIndex = 19;
            this.buttonNextItem.Text = "下一期";
            this.buttonNextItem.UseVisualStyleBackColor = true;
            this.buttonNextItem.Click += new System.EventHandler(this.buttonNextItem_Click);
            // 
            // LotteryGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 548);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStripGraph);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "LotteryGraph";
            this.Text = "LotteryGraph";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LotteryGraph_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
            this.menuStripGraph.ResumeLayout(false);
            this.menuStripGraph.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panelUp.ResumeLayout(false);
            this.contextMenuStripRightClick.ResumeLayout(false);
            this.tabControlView.ResumeLayout(false);
            this.tabPageKGraph.ResumeLayout(false);
            this.tabPageKGraph.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarKData)).EndInit();
            this.groupBoxAvgSettings.ResumeLayout(false);
            this.groupBoxAvgSettings.PerformLayout();
            this.tabPageBarGraph.ResumeLayout(false);
            this.tabPageBarGraph.PerformLayout();
            this.tabPageTrade.ResumeLayout(false);
            this.tabPageTrade.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTradeData)).EndInit();
            this.tabPageAppearence.ResumeLayout(false);
            this.tabPageAppearence.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAppearRate)).EndInit();
            this.tabPageMissCount.ResumeLayout(false);
            this.tabPageMissCount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMissCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGraphBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ListBoxFavoriteCharts_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripGraph;
        private System.Windows.Forms.ToolStripMenuItem operationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxNumIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxCollectionDataType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tabControlView;
        private System.Windows.Forms.TabPage tabPageKGraph;
        private System.Windows.Forms.TabPage tabPageBarGraph;
        private System.Windows.Forms.ComboBox comboBoxCollectRange;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxBarCollectType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxCustomCollectRange;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxAvgAlgorithm;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBoxAvgSettings;
        private System.Windows.Forms.Button buttonAvg100;
        private System.Windows.Forms.CheckBox checkBoxAvg100;
        private System.Windows.Forms.Button buttonAvg50;
        private System.Windows.Forms.CheckBox checkBoxAvg50;
        private System.Windows.Forms.Button buttonAvg30;
        private System.Windows.Forms.CheckBox checkBoxAvg30;
        private System.Windows.Forms.Button buttonAvg20;
        private System.Windows.Forms.CheckBox checkBoxAvg20;
        private System.Windows.Forms.Button buttonAvg10;
        private System.Windows.Forms.CheckBox checkBoxAvg10;
        private System.Windows.Forms.Button buttonAvg5;
        private System.Windows.Forms.CheckBox checkBoxAvg5;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckBox checkBoxMACD;
        private System.Windows.Forms.CheckBox checkBoxBollinBand;
        private CustomUI.DoubleBufferPanel panelUp;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxGridScaleH;
        private System.Windows.Forms.TextBox textBoxGridScaleW;
        private System.Windows.Forms.Label label9;
        private CustomUI.DoubleBufferPanel panelDown;
        private System.Windows.Forms.ToolStripMenuItem autoAllignToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxOperations;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox checkBoxShowAvgLines;
        private System.Windows.Forms.ToolStripMenuItem delAllAuxLinesToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxShowAuxLines;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRightClick;
        private System.Windows.Forms.ToolStripMenuItem delSelAuxLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem delAllLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelAddAuxLineToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageTrade;
        private System.Windows.Forms.ToolStripMenuItem tradeSimFromFirstToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tradeSimFromLatestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseSimTradeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resumeSimTradeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopSimTradeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tradeToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxTradeNumIndex;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBoxTradeSpecNumIndex;
        private System.Windows.Forms.Button buttonCommitTradeCount;
        private System.Windows.Forms.TextBox textBoxDefaultCount;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxMultiCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ToolStripMenuItem clearAllTradeDatasToolStripMenuItem;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxStartMoney;
        private System.Windows.Forms.TrackBar trackBarKData;
        private System.Windows.Forms.TrackBar trackBarTradeData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem globalSimTradeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modifyAuxLineColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoAddAuxLineToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxKRuler;
        private System.Windows.Forms.ToolStripMenuItem tradeCalculaterToolStripMenuItem;
        private System.Windows.Forms.ListBox listBoxFavoriteCharts;
        private System.Windows.Forms.Button buttonClearFavoriteCharts;
        private System.Windows.Forms.Button buttonAutoCalcFavotiteCharts;
        private System.Windows.Forms.Button buttonAddFavoriteChart;
        private System.Windows.Forms.Button btnSetAsStartTrade;
        private System.Windows.Forms.TextBox textBoxStartDataItem;
        private System.Windows.Forms.Button buttonHorzExpand;
        private System.Windows.Forms.Button buttonVertExpand;
        private System.Windows.Forms.ToolStripMenuItem simTradeOneStepToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxTradeStrategy;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TabPage tabPageAppearence;
        private System.Windows.Forms.GroupBox groupBoxCDTShowSetting;
        private System.Windows.Forms.CheckBox checkBoxShowSingleLine;
        private System.Windows.Forms.TabPage tabPageMissCount;
        private System.Windows.Forms.GroupBox groupBoxMissCountCDTShowSetting;
        private System.Windows.Forms.CheckBox checkBoxMissCountShowSingleLine;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboBoxMissCountType;
        private System.Windows.Forms.TextBox textBoxRefreshTimeLength;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox comboBoxAppearenceType;
        private System.Windows.Forms.ToolStripMenuItem loadNextBatchDatasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshLatestDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem setBreakPointToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxKDataCircle;
        private System.Windows.Forms.TrackBar trackBarAppearRate;
        private System.Windows.Forms.TrackBar trackBarMissCount;
        private System.Windows.Forms.TrackBar trackBarGraphBar;
        private System.Windows.Forms.Button buttonNextItem;
        private System.Windows.Forms.Button buttonPrevItem;
    }
}