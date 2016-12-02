namespace LotteryAnalyze
{
    partial class Form1
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
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewLotteryDatas = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listViewFileList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToSimulatePoolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlSimulator = new System.Windows.Forms.TabControl();
            this.tabPageKillNumberStrategySetting = new System.Windows.Forms.TabPage();
            this.dataGridViewKillNumberStrategy = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageSimulator = new System.Windows.Forms.TabPage();
            this.richTextBoxResult = new System.Windows.Forms.RichTextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonExecSimulate = new System.Windows.Forms.Button();
            this.ColumnData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.andValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rearValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.crossValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.group6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.group3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.group1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.killNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.right = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wrong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reward = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.profit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLotteryDatas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStripListView.SuspendLayout();
            this.tabControlSimulator.SuspendLayout();
            this.tabPageKillNumberStrategySetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKillNumberStrategy)).BeginInit();
            this.tabPageSimulator.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(771, 25);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFilesToolStripMenuItem,
            this.addFolderToolStripMenuItem,
            this.clearToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(39, 21);
            this.toolStripMenuItem1.Text = "File";
            // 
            // addFilesToolStripMenuItem
            // 
            this.addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
            this.addFilesToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.addFilesToolStripMenuItem.Text = "添加文件";
            this.addFilesToolStripMenuItem.Click += new System.EventHandler(this.addFilesToolStripMenuItem_Click);
            // 
            // addFolderToolStripMenuItem
            // 
            this.addFolderToolStripMenuItem.Name = "addFolderToolStripMenuItem";
            this.addFolderToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.addFolderToolStripMenuItem.Text = "添加文件夹";
            this.addFolderToolStripMenuItem.Click += new System.EventHandler(this.addFolderToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.clearToolStripMenuItem.Text = "清空数据";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // dataGridViewLotteryDatas
            // 
            this.dataGridViewLotteryDatas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewLotteryDatas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLotteryDatas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnData,
            this.Number,
            this.andValue,
            this.rearValue,
            this.crossValue,
            this.group6,
            this.group3,
            this.group1,
            this.killNum,
            this.right,
            this.wrong,
            this.cost,
            this.reward,
            this.profit});
            this.dataGridViewLotteryDatas.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewLotteryDatas.Name = "dataGridViewLotteryDatas";
            this.dataGridViewLotteryDatas.RowTemplate.Height = 23;
            this.dataGridViewLotteryDatas.Size = new System.Drawing.Size(584, 181);
            this.dataGridViewLotteryDatas.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControlSimulator);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(771, 387);
            this.splitContainer1.SplitterDistance = 187;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listViewFileList);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dataGridViewLotteryDatas);
            this.splitContainer2.Size = new System.Drawing.Size(771, 187);
            this.splitContainer2.SplitterDistance = 177;
            this.splitContainer2.TabIndex = 0;
            // 
            // listViewFileList
            // 
            this.listViewFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewFileList.ContextMenuStrip = this.contextMenuStripListView;
            this.listViewFileList.FullRowSelect = true;
            this.listViewFileList.GridLines = true;
            this.listViewFileList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewFileList.Location = new System.Drawing.Point(4, 4);
            this.listViewFileList.Name = "listViewFileList";
            this.listViewFileList.Size = new System.Drawing.Size(170, 180);
            this.listViewFileList.TabIndex = 0;
            this.listViewFileList.UseCompatibleStateImageBehavior = false;
            this.listViewFileList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 200;
            // 
            // contextMenuStripListView
            // 
            this.contextMenuStripListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToSimulatePoolToolStripMenuItem});
            this.contextMenuStripListView.Name = "contextMenuStripListView";
            this.contextMenuStripListView.Size = new System.Drawing.Size(137, 26);
            // 
            // addToSimulatePoolToolStripMenuItem
            // 
            this.addToSimulatePoolToolStripMenuItem.Name = "addToSimulatePoolToolStripMenuItem";
            this.addToSimulatePoolToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.addToSimulatePoolToolStripMenuItem.Text = "放入模拟池";
            this.addToSimulatePoolToolStripMenuItem.Click += new System.EventHandler(this.addToSimulatePoolToolStripMenuItem_Click);
            // 
            // tabControlSimulator
            // 
            this.tabControlSimulator.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlSimulator.Controls.Add(this.tabPageKillNumberStrategySetting);
            this.tabControlSimulator.Controls.Add(this.tabPageSimulator);
            this.tabControlSimulator.Location = new System.Drawing.Point(4, 4);
            this.tabControlSimulator.Name = "tabControlSimulator";
            this.tabControlSimulator.SelectedIndex = 0;
            this.tabControlSimulator.Size = new System.Drawing.Size(764, 192);
            this.tabControlSimulator.TabIndex = 0;
            // 
            // tabPageKillNumberStrategySetting
            // 
            this.tabPageKillNumberStrategySetting.Controls.Add(this.dataGridViewKillNumberStrategy);
            this.tabPageKillNumberStrategySetting.Location = new System.Drawing.Point(4, 22);
            this.tabPageKillNumberStrategySetting.Name = "tabPageKillNumberStrategySetting";
            this.tabPageKillNumberStrategySetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageKillNumberStrategySetting.Size = new System.Drawing.Size(756, 166);
            this.tabPageKillNumberStrategySetting.TabIndex = 0;
            this.tabPageKillNumberStrategySetting.Text = "杀号策略";
            this.tabPageKillNumberStrategySetting.UseVisualStyleBackColor = true;
            // 
            // dataGridViewKillNumberStrategy
            // 
            this.dataGridViewKillNumberStrategy.AllowUserToAddRows = false;
            this.dataGridViewKillNumberStrategy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewKillNumberStrategy.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewKillNumberStrategy.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.name,
            this.desc});
            this.dataGridViewKillNumberStrategy.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewKillNumberStrategy.Name = "dataGridViewKillNumberStrategy";
            this.dataGridViewKillNumberStrategy.RowTemplate.Height = 23;
            this.dataGridViewKillNumberStrategy.Size = new System.Drawing.Size(753, 169);
            this.dataGridViewKillNumberStrategy.TabIndex = 0;
            this.dataGridViewKillNumberStrategy.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewKillNumberStrategy_CellEndEdit);
            // 
            // id
            // 
            this.id.HeaderText = "序号";
            this.id.Name = "id";
            // 
            // name
            // 
            this.name.HeaderText = "策略名称";
            this.name.Name = "name";
            // 
            // desc
            // 
            this.desc.HeaderText = "描述";
            this.desc.Name = "desc";
            this.desc.Width = 1000;
            // 
            // tabPageSimulator
            // 
            this.tabPageSimulator.Controls.Add(this.richTextBoxResult);
            this.tabPageSimulator.Controls.Add(this.progressBar1);
            this.tabPageSimulator.Controls.Add(this.buttonExecSimulate);
            this.tabPageSimulator.Location = new System.Drawing.Point(4, 22);
            this.tabPageSimulator.Name = "tabPageSimulator";
            this.tabPageSimulator.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSimulator.Size = new System.Drawing.Size(756, 166);
            this.tabPageSimulator.TabIndex = 1;
            this.tabPageSimulator.Text = "模拟器";
            this.tabPageSimulator.UseVisualStyleBackColor = true;
            // 
            // richTextBoxResult
            // 
            this.richTextBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxResult.Location = new System.Drawing.Point(0, 53);
            this.richTextBoxResult.Name = "richTextBoxResult";
            this.richTextBoxResult.Size = new System.Drawing.Size(756, 111);
            this.richTextBoxResult.TabIndex = 2;
            this.richTextBoxResult.Text = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(0, 29);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(756, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // buttonExecSimulate
            // 
            this.buttonExecSimulate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExecSimulate.Location = new System.Drawing.Point(0, 4);
            this.buttonExecSimulate.Name = "buttonExecSimulate";
            this.buttonExecSimulate.Size = new System.Drawing.Size(756, 23);
            this.buttonExecSimulate.TabIndex = 0;
            this.buttonExecSimulate.Text = "开始模拟";
            this.buttonExecSimulate.UseVisualStyleBackColor = true;
            this.buttonExecSimulate.Click += new System.EventHandler(this.buttonExecSimulate_Click);
            // 
            // ColumnData
            // 
            this.ColumnData.HeaderText = "id";
            this.ColumnData.Name = "ColumnData";
            this.ColumnData.Width = 60;
            // 
            // Number
            // 
            this.Number.HeaderText = "number";
            this.Number.Name = "Number";
            this.Number.Width = 60;
            // 
            // andValue
            // 
            this.andValue.HeaderText = "和值";
            this.andValue.Name = "andValue";
            this.andValue.Width = 60;
            // 
            // rearValue
            // 
            this.rearValue.HeaderText = "合值";
            this.rearValue.Name = "rearValue";
            this.rearValue.Width = 60;
            // 
            // crossValue
            // 
            this.crossValue.HeaderText = "跨度";
            this.crossValue.Name = "crossValue";
            this.crossValue.Width = 60;
            // 
            // group6
            // 
            this.group6.HeaderText = "组六";
            this.group6.Name = "group6";
            this.group6.Width = 60;
            // 
            // group3
            // 
            this.group3.HeaderText = "组三";
            this.group3.Name = "group3";
            this.group3.Width = 60;
            // 
            // group1
            // 
            this.group1.HeaderText = "豹子";
            this.group1.Name = "group1";
            this.group1.Width = 60;
            // 
            // killNum
            // 
            this.killNum.HeaderText = "杀号";
            this.killNum.Name = "killNum";
            // 
            // right
            // 
            this.right.HeaderText = "对";
            this.right.Name = "right";
            this.right.ReadOnly = true;
            this.right.Width = 50;
            // 
            // wrong
            // 
            this.wrong.HeaderText = "错";
            this.wrong.Name = "wrong";
            this.wrong.ReadOnly = true;
            this.wrong.Width = 50;
            // 
            // cost
            // 
            this.cost.HeaderText = "花费";
            this.cost.Name = "cost";
            this.cost.ReadOnly = true;
            // 
            // reward
            // 
            this.reward.HeaderText = "奖励";
            this.reward.Name = "reward";
            this.reward.ReadOnly = true;
            // 
            // profit
            // 
            this.profit.HeaderText = "收益";
            this.profit.Name = "profit";
            this.profit.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 415);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLotteryDatas)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStripListView.ResumeLayout(false);
            this.tabControlSimulator.ResumeLayout(false);
            this.tabPageKillNumberStrategySetting.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKillNumberStrategy)).EndInit();
            this.tabPageSimulator.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.DataGridView dataGridViewLotteryDatas;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addFilesToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView listViewFileList;
        private System.Windows.Forms.ToolStripMenuItem addFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripListView;
        private System.Windows.Forms.ToolStripMenuItem addToSimulatePoolToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControlSimulator;
        private System.Windows.Forms.TabPage tabPageKillNumberStrategySetting;
        private System.Windows.Forms.DataGridView dataGridViewKillNumberStrategy;
        private System.Windows.Forms.DataGridViewCheckBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn desc;
        private System.Windows.Forms.TabPage tabPageSimulator;
        private System.Windows.Forms.RichTextBox richTextBoxResult;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonExecSimulate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnData;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn andValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn rearValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn crossValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn group6;
        private System.Windows.Forms.DataGridViewTextBoxColumn group3;
        private System.Windows.Forms.DataGridViewTextBoxColumn group1;
        private System.Windows.Forms.DataGridViewTextBoxColumn killNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn right;
        private System.Windows.Forms.DataGridViewTextBoxColumn wrong;
        private System.Windows.Forms.DataGridViewTextBoxColumn cost;
        private System.Windows.Forms.DataGridViewTextBoxColumn reward;
        private System.Windows.Forms.DataGridViewTextBoxColumn profit;
    }
}

