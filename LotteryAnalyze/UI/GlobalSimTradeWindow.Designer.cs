namespace LotteryAnalyze.UI
{
    partial class GlobalSimTradeWindow
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxStartMoney = new System.Windows.Forms.TextBox();
            this.textBoxTradeCountLst = new System.Windows.Forms.TextBox();
            this.textBoxDayCountPerBatch = new System.Windows.Forms.TextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonPauseResume = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.progressBarCurrent = new System.Windows.Forms.ProgressBar();
            this.progressBarTotal = new System.Windows.Forms.ProgressBar();
            this.checkBoxSpecNumIndex = new System.Windows.Forms.CheckBox();
            this.comboBoxSpecNumIndex = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxStartDate = new System.Windows.Forms.TextBox();
            this.textBoxEndDate = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxTradeStrategy = new System.Windows.Forms.ComboBox();
            this.checkBoxForceTradeByMaxNumCount = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxUponValue = new System.Windows.Forms.TextBox();
            this.textBoxRiskControl = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.trackBarRiskControl = new System.Windows.Forms.TrackBar();
            this.checkBoxOnTradeOnStrongUpPath = new System.Windows.Forms.CheckBox();
            this.textBoxStrongUpStartIndex = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBoxOnNoMoney = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxMaxNumCount = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeViewLongWrongTradeInfos = new System.Windows.Forms.TreeView();
            this.buttonDebugSetting = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxCmd = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRiskControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "起始金额：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "投注方式：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "每批几天：";
            // 
            // textBoxStartMoney
            // 
            this.textBoxStartMoney.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStartMoney.Location = new System.Drawing.Point(74, 5);
            this.textBoxStartMoney.Name = "textBoxStartMoney";
            this.textBoxStartMoney.Size = new System.Drawing.Size(186, 21);
            this.textBoxStartMoney.TabIndex = 3;
            // 
            // textBoxTradeCountLst
            // 
            this.textBoxTradeCountLst.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTradeCountLst.Location = new System.Drawing.Point(74, 27);
            this.textBoxTradeCountLst.Name = "textBoxTradeCountLst";
            this.textBoxTradeCountLst.Size = new System.Drawing.Size(186, 21);
            this.textBoxTradeCountLst.TabIndex = 4;
            // 
            // textBoxDayCountPerBatch
            // 
            this.textBoxDayCountPerBatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDayCountPerBatch.Location = new System.Drawing.Point(74, 71);
            this.textBoxDayCountPerBatch.Name = "textBoxDayCountPerBatch";
            this.textBoxDayCountPerBatch.Size = new System.Drawing.Size(186, 21);
            this.textBoxDayCountPerBatch.TabIndex = 5;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(3, 3);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 6;
            this.buttonStart.Text = "开始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonPauseResume
            // 
            this.buttonPauseResume.Location = new System.Drawing.Point(84, 3);
            this.buttonPauseResume.Name = "buttonPauseResume";
            this.buttonPauseResume.Size = new System.Drawing.Size(75, 23);
            this.buttonPauseResume.TabIndex = 7;
            this.buttonPauseResume.Text = "暂停/恢复";
            this.buttonPauseResume.UseVisualStyleBackColor = true;
            this.buttonPauseResume.Click += new System.EventHandler(this.buttonPauseResume_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(165, 3);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 8;
            this.buttonStop.Text = "结束";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // progressBarCurrent
            // 
            this.progressBarCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarCurrent.Location = new System.Drawing.Point(75, 387);
            this.progressBarCurrent.Name = "progressBarCurrent";
            this.progressBarCurrent.Size = new System.Drawing.Size(634, 23);
            this.progressBarCurrent.TabIndex = 11;
            // 
            // progressBarTotal
            // 
            this.progressBarTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarTotal.Location = new System.Drawing.Point(75, 417);
            this.progressBarTotal.Name = "progressBarTotal";
            this.progressBarTotal.Size = new System.Drawing.Size(634, 23);
            this.progressBarTotal.TabIndex = 12;
            // 
            // checkBoxSpecNumIndex
            // 
            this.checkBoxSpecNumIndex.AutoSize = true;
            this.checkBoxSpecNumIndex.Location = new System.Drawing.Point(3, 316);
            this.checkBoxSpecNumIndex.Name = "checkBoxSpecNumIndex";
            this.checkBoxSpecNumIndex.Size = new System.Drawing.Size(108, 16);
            this.checkBoxSpecNumIndex.TabIndex = 13;
            this.checkBoxSpecNumIndex.Text = "是否指定数字位";
            this.checkBoxSpecNumIndex.UseVisualStyleBackColor = true;
            this.checkBoxSpecNumIndex.CheckedChanged += new System.EventHandler(this.checkBoxSpecNumIndex_CheckedChanged);
            // 
            // comboBoxSpecNumIndex
            // 
            this.comboBoxSpecNumIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSpecNumIndex.FormattingEnabled = true;
            this.comboBoxSpecNumIndex.Location = new System.Drawing.Point(74, 94);
            this.comboBoxSpecNumIndex.Name = "comboBoxSpecNumIndex";
            this.comboBoxSpecNumIndex.Size = new System.Drawing.Size(186, 20);
            this.comboBoxSpecNumIndex.TabIndex = 14;
            this.comboBoxSpecNumIndex.SelectedIndexChanged += new System.EventHandler(this.comboBoxSpecNumIndex_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "数字位：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "起始日期：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 217);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 17;
            this.label6.Text = "结束日期：";
            // 
            // textBoxStartDate
            // 
            this.textBoxStartDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStartDate.Location = new System.Drawing.Point(74, 191);
            this.textBoxStartDate.Name = "textBoxStartDate";
            this.textBoxStartDate.ReadOnly = true;
            this.textBoxStartDate.Size = new System.Drawing.Size(186, 21);
            this.textBoxStartDate.TabIndex = 18;
            // 
            // textBoxEndDate
            // 
            this.textBoxEndDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEndDate.Location = new System.Drawing.Point(74, 214);
            this.textBoxEndDate.Name = "textBoxEndDate";
            this.textBoxEndDate.ReadOnly = true;
            this.textBoxEndDate.Size = new System.Drawing.Size(186, 21);
            this.textBoxEndDate.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 143);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 21;
            this.label7.Text = "交易策略：";
            // 
            // comboBoxTradeStrategy
            // 
            this.comboBoxTradeStrategy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTradeStrategy.FormattingEnabled = true;
            this.comboBoxTradeStrategy.Location = new System.Drawing.Point(74, 138);
            this.comboBoxTradeStrategy.Name = "comboBoxTradeStrategy";
            this.comboBoxTradeStrategy.Size = new System.Drawing.Size(186, 20);
            this.comboBoxTradeStrategy.TabIndex = 20;
            this.comboBoxTradeStrategy.SelectedIndexChanged += new System.EventHandler(this.comboBoxTradeStrategy_SelectedIndexChanged);
            // 
            // checkBoxForceTradeByMaxNumCount
            // 
            this.checkBoxForceTradeByMaxNumCount.AutoSize = true;
            this.checkBoxForceTradeByMaxNumCount.Location = new System.Drawing.Point(3, 338);
            this.checkBoxForceTradeByMaxNumCount.Name = "checkBoxForceTradeByMaxNumCount";
            this.checkBoxForceTradeByMaxNumCount.Size = new System.Drawing.Size(144, 16);
            this.checkBoxForceTradeByMaxNumCount.TabIndex = 23;
            this.checkBoxForceTradeByMaxNumCount.Text = "是否使用最大交易个数";
            this.checkBoxForceTradeByMaxNumCount.UseVisualStyleBackColor = true;
            this.checkBoxForceTradeByMaxNumCount.CheckedChanged += new System.EventHandler(this.checkBoxForceTradeByMaxNumCount_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(5, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label14);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxUponValue);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxRiskControl);
            this.splitContainer1.Panel1.Controls.Add(this.label13);
            this.splitContainer1.Panel1.Controls.Add(this.trackBarRiskControl);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxOnTradeOnStrongUpPath);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxStrongUpStartIndex);
            this.splitContainer1.Panel1.Controls.Add(this.label12);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxOnNoMoney);
            this.splitContainer1.Panel1.Controls.Add(this.label11);
            this.splitContainer1.Panel1.Controls.Add(this.label8);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxSpecNumIndex);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxSpecNumIndex);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxDayCountPerBatch);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxTradeCountLst);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxMaxNumCount);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxStartMoney);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxForceTradeByMaxNumCount);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxEndDate);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxTradeStrategy);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxStartDate);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.buttonDebugSetting);
            this.splitContainer1.Panel2.Controls.Add(this.buttonStop);
            this.splitContainer1.Panel2.Controls.Add(this.buttonPauseResume);
            this.splitContainer1.Panel2.Controls.Add(this.buttonStart);
            this.splitContainer1.Size = new System.Drawing.Size(704, 377);
            this.splitContainer1.SplitterDistance = 263;
            this.splitContainer1.TabIndex = 24;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 165);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 34;
            this.label14.Text = "评分起点：";
            // 
            // textBoxUponValue
            // 
            this.textBoxUponValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUponValue.Location = new System.Drawing.Point(74, 163);
            this.textBoxUponValue.Name = "textBoxUponValue";
            this.textBoxUponValue.Size = new System.Drawing.Size(186, 21);
            this.textBoxUponValue.TabIndex = 35;
            this.textBoxUponValue.TextChanged += new System.EventHandler(this.textBoxUponValue_TextChanged);
            // 
            // textBoxRiskControl
            // 
            this.textBoxRiskControl.Location = new System.Drawing.Point(74, 283);
            this.textBoxRiskControl.Name = "textBoxRiskControl";
            this.textBoxRiskControl.ReadOnly = true;
            this.textBoxRiskControl.Size = new System.Drawing.Size(129, 21);
            this.textBoxRiskControl.TabIndex = 33;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 261);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 32;
            this.label13.Text = "风险控制：";
            // 
            // trackBarRiskControl
            // 
            this.trackBarRiskControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarRiskControl.AutoSize = false;
            this.trackBarRiskControl.Location = new System.Drawing.Point(74, 258);
            this.trackBarRiskControl.Maximum = 100;
            this.trackBarRiskControl.Name = "trackBarRiskControl";
            this.trackBarRiskControl.Size = new System.Drawing.Size(186, 23);
            this.trackBarRiskControl.TabIndex = 31;
            this.trackBarRiskControl.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarRiskControl.Scroll += new System.EventHandler(this.trackBarRiskControl_Scroll);
            // 
            // checkBoxOnTradeOnStrongUpPath
            // 
            this.checkBoxOnTradeOnStrongUpPath.AutoSize = true;
            this.checkBoxOnTradeOnStrongUpPath.Location = new System.Drawing.Point(3, 360);
            this.checkBoxOnTradeOnStrongUpPath.Name = "checkBoxOnTradeOnStrongUpPath";
            this.checkBoxOnTradeOnStrongUpPath.Size = new System.Drawing.Size(144, 16);
            this.checkBoxOnTradeOnStrongUpPath.TabIndex = 30;
            this.checkBoxOnTradeOnStrongUpPath.Text = "是否只在强上升时交易";
            this.checkBoxOnTradeOnStrongUpPath.UseVisualStyleBackColor = true;
            this.checkBoxOnTradeOnStrongUpPath.CheckedChanged += new System.EventHandler(this.checkBoxOnTradeOnStrongUpPath_CheckedChanged);
            // 
            // textBoxStrongUpStartIndex
            // 
            this.textBoxStrongUpStartIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStrongUpStartIndex.Location = new System.Drawing.Point(74, 49);
            this.textBoxStrongUpStartIndex.Name = "textBoxStrongUpStartIndex";
            this.textBoxStrongUpStartIndex.Size = new System.Drawing.Size(186, 21);
            this.textBoxStrongUpStartIndex.TabIndex = 29;
            this.textBoxStrongUpStartIndex.TextChanged += new System.EventHandler(this.textBoxStrongUpStartIndex_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 52);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 28;
            this.label12.Text = "强交易线：";
            // 
            // comboBoxOnNoMoney
            // 
            this.comboBoxOnNoMoney.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxOnNoMoney.FormattingEnabled = true;
            this.comboBoxOnNoMoney.Location = new System.Drawing.Point(74, 237);
            this.comboBoxOnNoMoney.Name = "comboBoxOnNoMoney";
            this.comboBoxOnNoMoney.Size = new System.Drawing.Size(186, 20);
            this.comboBoxOnNoMoney.TabIndex = 26;
            this.comboBoxOnNoMoney.SelectedIndexChanged += new System.EventHandler(this.comboBoxOnNoMoney_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 242);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 27;
            this.label11.Text = "崩盘处理：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 118);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 24;
            this.label8.Text = "最多个数：";
            // 
            // textBoxMaxNumCount
            // 
            this.textBoxMaxNumCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMaxNumCount.Location = new System.Drawing.Point(74, 116);
            this.textBoxMaxNumCount.Name = "textBoxMaxNumCount";
            this.textBoxMaxNumCount.Size = new System.Drawing.Size(186, 21);
            this.textBoxMaxNumCount.TabIndex = 25;
            this.textBoxMaxNumCount.TextChanged += new System.EventHandler(this.textBoxMaxNumCount_TextChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 27);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.textBoxCmd);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.treeViewLongWrongTradeInfos);
            this.splitContainer2.Size = new System.Drawing.Size(434, 349);
            this.splitContainer2.SplitterDistance = 217;
            this.splitContainer2.TabIndex = 13;
            // 
            // treeViewLongWrongTradeInfos
            // 
            this.treeViewLongWrongTradeInfos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewLongWrongTradeInfos.Location = new System.Drawing.Point(3, 3);
            this.treeViewLongWrongTradeInfos.Name = "treeViewLongWrongTradeInfos";
            this.treeViewLongWrongTradeInfos.Size = new System.Drawing.Size(207, 344);
            this.treeViewLongWrongTradeInfos.TabIndex = 12;
            this.treeViewLongWrongTradeInfos.DoubleClick += new System.EventHandler(this.treeViewLongWrongTradeInfos_DoubleClick);
            // 
            // buttonDebugSetting
            // 
            this.buttonDebugSetting.Location = new System.Drawing.Point(246, 3);
            this.buttonDebugSetting.Name = "buttonDebugSetting";
            this.buttonDebugSetting.Size = new System.Drawing.Size(75, 23);
            this.buttonDebugSetting.TabIndex = 11;
            this.buttonDebugSetting.Text = "调试设置";
            this.buttonDebugSetting.UseVisualStyleBackColor = true;
            this.buttonDebugSetting.Click += new System.EventHandler(this.buttonDebugSetting_Click);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 392);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "当前进度：";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 422);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 27;
            this.label10.Text = "总进度：";
            // 
            // textBoxCmd
            // 
            this.textBoxCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCmd.Location = new System.Drawing.Point(0, 3);
            this.textBoxCmd.MinimumSize = new System.Drawing.Size(20, 20);
            this.textBoxCmd.Name = "textBoxCmd";
            this.textBoxCmd.Size = new System.Drawing.Size(214, 343);
            this.textBoxCmd.TabIndex = 0;
            // 
            // GlobalSimTradeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 445);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.progressBarTotal);
            this.Controls.Add(this.progressBarCurrent);
            this.Name = "GlobalSimTradeWindow";
            this.Text = "GlobalSimTradeWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GlobalSimTradeWindow_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRiskControl)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxStartMoney;
        private System.Windows.Forms.TextBox textBoxTradeCountLst;
        private System.Windows.Forms.TextBox textBoxDayCountPerBatch;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonPauseResume;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.ProgressBar progressBarCurrent;
        private System.Windows.Forms.ProgressBar progressBarTotal;
        private System.Windows.Forms.CheckBox checkBoxSpecNumIndex;
        private System.Windows.Forms.ComboBox comboBoxSpecNumIndex;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxStartDate;
        private System.Windows.Forms.TextBox textBoxEndDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxTradeStrategy;
        private System.Windows.Forms.CheckBox checkBoxForceTradeByMaxNumCount;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxMaxNumCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboBoxOnNoMoney;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxStrongUpStartIndex;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox checkBoxOnTradeOnStrongUpPath;
        private System.Windows.Forms.TrackBar trackBarRiskControl;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxRiskControl;
        private System.Windows.Forms.Button buttonDebugSetting;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxUponValue;
        private System.Windows.Forms.TreeView treeViewLongWrongTradeInfos;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.WebBrowser textBoxCmd;
    }
}