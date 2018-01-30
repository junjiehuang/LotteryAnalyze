namespace LotteryAnalyze.UI
{
    partial class LotteryGraph
    {
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
            this.tradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testAutoTradeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.contextMenuStripRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.delSelAuxLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.delAllLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelAddAuxLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxGridScaleH = new System.Windows.Forms.TextBox();
            this.textBoxGridScaleW = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControlView = new System.Windows.Forms.TabControl();
            this.tabPageKGraph = new System.Windows.Forms.TabPage();
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
            this.textBoxCycleLength = new System.Windows.Forms.TextBox();
            this.tabPageBarGraph = new System.Windows.Forms.TabPage();
            this.textBoxCustomCollectRange = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxCollectRange = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxBarCollectType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPageTrade = new System.Windows.Forms.TabPage();
            this.comboBoxCollectionDataType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxNumIndex = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelUp = new LotteryAnalyze.UI.ExtPanel();
            this.panelDown = new LotteryAnalyze.UI.ExtPanel();
            this.menuStripGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStripRightClick.SuspendLayout();
            this.tabControlView.SuspendLayout();
            this.tabPageKGraph.SuspendLayout();
            this.groupBoxAvgSettings.SuspendLayout();
            this.tabPageBarGraph.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripGraph
            // 
            this.menuStripGraph.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.operationToolStripMenuItem,
            this.设置ToolStripMenuItem});
            this.menuStripGraph.Location = new System.Drawing.Point(0, 0);
            this.menuStripGraph.Name = "menuStripGraph";
            this.menuStripGraph.Size = new System.Drawing.Size(696, 25);
            this.menuStripGraph.TabIndex = 0;
            this.menuStripGraph.Text = "menuStripGraph";
            // 
            // operationToolStripMenuItem
            // 
            this.operationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.autoAllignToolStripMenuItem,
            this.delAllAuxLinesToolStripMenuItem,
            this.tradeToolStripMenuItem,
            this.testAutoTradeToolStripMenuItem});
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
            // tradeToolStripMenuItem
            // 
            this.tradeToolStripMenuItem.Name = "tradeToolStripMenuItem";
            this.tradeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.tradeToolStripMenuItem.Text = "下单";
            this.tradeToolStripMenuItem.Click += new System.EventHandler(this.tradeToolStripMenuItem_Click);
            // 
            // testAutoTradeToolStripMenuItem
            // 
            this.testAutoTradeToolStripMenuItem.Name = "testAutoTradeToolStripMenuItem";
            this.testAutoTradeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.testAutoTradeToolStripMenuItem.Text = "测试大数据";
            this.testAutoTradeToolStripMenuItem.Click += new System.EventHandler(this.testAutoTradeToolStripMenuItem_Click);
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.设置ToolStripMenuItem.Text = "设置";
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
            this.splitContainer1.Panel2.Controls.Add(this.textBoxGridScaleH);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxGridScaleW);
            this.splitContainer1.Panel2.Controls.Add(this.label9);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.tabControlView);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxCollectionDataType);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxNumIndex);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(696, 481);
            this.splitContainer1.SplitterDistance = 495;
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
            this.splitContainer2.Size = new System.Drawing.Size(495, 481);
            this.splitContainer2.SplitterDistance = 294;
            this.splitContainer2.TabIndex = 0;
            // 
            // contextMenuStripRightClick
            // 
            this.contextMenuStripRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.delSelAuxLineToolStripMenuItem,
            this.delAllLinesToolStripMenuItem,
            this.cancelAddAuxLineToolStripMenuItem});
            this.contextMenuStripRightClick.Name = "contextMenuStripRightClick";
            this.contextMenuStripRightClick.Size = new System.Drawing.Size(173, 70);
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
            // textBoxGridScaleH
            // 
            this.textBoxGridScaleH.Location = new System.Drawing.Point(121, 4);
            this.textBoxGridScaleH.Name = "textBoxGridScaleH";
            this.textBoxGridScaleH.Size = new System.Drawing.Size(73, 21);
            this.textBoxGridScaleH.TabIndex = 10;
            this.textBoxGridScaleH.TextChanged += new System.EventHandler(this.textBoxGridScaleH_TextChanged);
            // 
            // textBoxGridScaleW
            // 
            this.textBoxGridScaleW.Location = new System.Drawing.Point(39, 4);
            this.textBoxGridScaleW.Name = "textBoxGridScaleW";
            this.textBoxGridScaleW.Size = new System.Drawing.Size(73, 21);
            this.textBoxGridScaleW.TabIndex = 9;
            this.textBoxGridScaleW.TextChanged += new System.EventHandler(this.textBoxGridScaleW_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 8;
            this.label9.Text = "缩放";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 122);
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
            this.tabControlView.Location = new System.Drawing.Point(6, 138);
            this.tabControlView.Name = "tabControlView";
            this.tabControlView.SelectedIndex = 0;
            this.tabControlView.Size = new System.Drawing.Size(188, 340);
            this.tabControlView.TabIndex = 6;
            this.tabControlView.SelectedIndexChanged += new System.EventHandler(this.tabControlView_SelectedIndexChanged);
            // 
            // tabPageKGraph
            // 
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
            this.tabPageKGraph.Controls.Add(this.textBoxCycleLength);
            this.tabPageKGraph.Location = new System.Drawing.Point(4, 22);
            this.tabPageKGraph.Name = "tabPageKGraph";
            this.tabPageKGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageKGraph.Size = new System.Drawing.Size(180, 314);
            this.tabPageKGraph.TabIndex = 0;
            this.tabPageKGraph.Text = "K线图";
            this.tabPageKGraph.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowAuxLines
            // 
            this.checkBoxShowAuxLines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowAuxLines.AutoSize = true;
            this.checkBoxShowAuxLines.Location = new System.Drawing.Point(5, 98);
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
            this.comboBoxOperations.Size = new System.Drawing.Size(112, 20);
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
            this.checkBoxShowAvgLines.Location = new System.Drawing.Point(5, 165);
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
            this.checkBoxMACD.Location = new System.Drawing.Point(5, 143);
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
            this.checkBoxBollinBand.Location = new System.Drawing.Point(5, 120);
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
            this.comboBoxAvgAlgorithm.Size = new System.Drawing.Size(112, 20);
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
            this.groupBoxAvgSettings.Location = new System.Drawing.Point(3, 180);
            this.groupBoxAvgSettings.Name = "groupBoxAvgSettings";
            this.groupBoxAvgSettings.Size = new System.Drawing.Size(174, 83);
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
            // textBoxCycleLength
            // 
            this.textBoxCycleLength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCycleLength.Enabled = false;
            this.textBoxCycleLength.Location = new System.Drawing.Point(65, 9);
            this.textBoxCycleLength.Name = "textBoxCycleLength";
            this.textBoxCycleLength.Size = new System.Drawing.Size(112, 21);
            this.textBoxCycleLength.TabIndex = 5;
            this.textBoxCycleLength.TextChanged += new System.EventHandler(this.textBoxCycleLength_TextChanged);
            // 
            // tabPageBarGraph
            // 
            this.tabPageBarGraph.Controls.Add(this.textBoxCustomCollectRange);
            this.tabPageBarGraph.Controls.Add(this.label7);
            this.tabPageBarGraph.Controls.Add(this.comboBoxCollectRange);
            this.tabPageBarGraph.Controls.Add(this.label6);
            this.tabPageBarGraph.Controls.Add(this.comboBoxBarCollectType);
            this.tabPageBarGraph.Controls.Add(this.label5);
            this.tabPageBarGraph.Location = new System.Drawing.Point(4, 22);
            this.tabPageBarGraph.Name = "tabPageBarGraph";
            this.tabPageBarGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBarGraph.Size = new System.Drawing.Size(180, 314);
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
            this.textBoxCustomCollectRange.Size = new System.Drawing.Size(152, 21);
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
            this.comboBoxCollectRange.Size = new System.Drawing.Size(152, 20);
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
            this.comboBoxBarCollectType.Size = new System.Drawing.Size(152, 20);
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
            this.tabPageTrade.Location = new System.Drawing.Point(4, 22);
            this.tabPageTrade.Name = "tabPageTrade";
            this.tabPageTrade.Size = new System.Drawing.Size(180, 314);
            this.tabPageTrade.TabIndex = 2;
            this.tabPageTrade.Text = "交易图";
            this.tabPageTrade.UseVisualStyleBackColor = true;
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
            this.comboBoxCollectionDataType.Location = new System.Drawing.Point(6, 90);
            this.comboBoxCollectionDataType.Name = "comboBoxCollectionDataType";
            this.comboBoxCollectionDataType.Size = new System.Drawing.Size(188, 20);
            this.comboBoxCollectionDataType.TabIndex = 3;
            this.comboBoxCollectionDataType.SelectedIndexChanged += new System.EventHandler(this.comboBoxCollectionDataType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 74);
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
            this.comboBoxNumIndex.Location = new System.Drawing.Point(6, 47);
            this.comboBoxNumIndex.Name = "comboBoxNumIndex";
            this.comboBoxNumIndex.Size = new System.Drawing.Size(188, 20);
            this.comboBoxNumIndex.TabIndex = 1;
            this.comboBoxNumIndex.SelectedIndexChanged += new System.EventHandler(this.comboBoxNumIndex_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择数字位：";
            // 
            // panelUp
            // 
            this.panelUp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelUp.ContextMenuStrip = this.contextMenuStripRightClick;
            this.panelUp.Location = new System.Drawing.Point(3, 4);
            this.panelUp.Name = "panelUp";
            this.panelUp.Size = new System.Drawing.Size(489, 287);
            this.panelUp.TabIndex = 0;
            this.panelUp.Paint += new System.Windows.Forms.PaintEventHandler(this.panelUp_Paint);
            this.panelUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelUp_MouseDown);
            this.panelUp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelUp_MouseMove);
            this.panelUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelUp_MouseUp);
            // 
            // panelDown
            // 
            this.panelDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDown.Location = new System.Drawing.Point(3, 3);
            this.panelDown.Name = "panelDown";
            this.panelDown.Size = new System.Drawing.Size(489, 177);
            this.panelDown.TabIndex = 0;
            this.panelDown.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDown_Paint);
            // 
            // LotteryGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 506);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStripGraph);
            this.DoubleBuffered = true;
            this.Name = "LotteryGraph";
            this.Text = "LotteryGraph";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LotteryGraph_FormClosed);
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
            this.contextMenuStripRightClick.ResumeLayout(false);
            this.tabControlView.ResumeLayout(false);
            this.tabPageKGraph.ResumeLayout(false);
            this.tabPageKGraph.PerformLayout();
            this.groupBoxAvgSettings.ResumeLayout(false);
            this.groupBoxAvgSettings.PerformLayout();
            this.tabPageBarGraph.ResumeLayout(false);
            this.tabPageBarGraph.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripGraph;
        private System.Windows.Forms.ToolStripMenuItem operationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxNumIndex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxCollectionDataType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCycleLength;
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
        private ExtPanel panelUp;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxGridScaleH;
        private System.Windows.Forms.TextBox textBoxGridScaleW;
        private System.Windows.Forms.Label label9;
        private ExtPanel panelDown;
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
        private System.Windows.Forms.ToolStripMenuItem tradeToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageTrade;
        private System.Windows.Forms.ToolStripMenuItem testAutoTradeToolStripMenuItem;
    }
}