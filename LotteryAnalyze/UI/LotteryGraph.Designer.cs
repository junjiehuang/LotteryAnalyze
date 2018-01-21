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
            this.menuStripGraph = new System.Windows.Forms.MenuStrip();
            this.operationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new LotteryAnalyze.UI.ExtSplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControlView = new System.Windows.Forms.TabControl();
            this.tabPageKGraph = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxCycleLength = new System.Windows.Forms.TextBox();
            this.tabPageBarGraph = new System.Windows.Forms.TabPage();
            this.comboBoxCollectionDataType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxNumIndex = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxBarCollectType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxCollectRange = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCustomCollectRange = new System.Windows.Forms.TextBox();
            this.menuStripGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlView.SuspendLayout();
            this.tabPageKGraph.SuspendLayout();
            this.tabPageBarGraph.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripGraph
            // 
            this.menuStripGraph.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.operationToolStripMenuItem});
            this.menuStripGraph.Location = new System.Drawing.Point(0, 0);
            this.menuStripGraph.Name = "menuStripGraph";
            this.menuStripGraph.Size = new System.Drawing.Size(647, 25);
            this.menuStripGraph.TabIndex = 0;
            this.menuStripGraph.Text = "menuStripGraph";
            // 
            // operationToolStripMenuItem
            // 
            this.operationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.operationToolStripMenuItem.Name = "operationToolStripMenuItem";
            this.operationToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.operationToolStripMenuItem.Text = "操作";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.refreshToolStripMenuItem.Text = "刷新";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            this.splitContainer1.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel1_MouseDown);
            this.splitContainer1.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel1_MouseMove);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.tabControlView);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxCollectionDataType);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxNumIndex);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(647, 363);
            this.splitContainer1.SplitterDistance = 466;
            this.splitContainer1.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 92);
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
            this.tabControlView.Location = new System.Drawing.Point(6, 113);
            this.tabControlView.Name = "tabControlView";
            this.tabControlView.SelectedIndex = 0;
            this.tabControlView.Size = new System.Drawing.Size(168, 247);
            this.tabControlView.TabIndex = 6;
            this.tabControlView.SelectedIndexChanged += new System.EventHandler(this.tabControlView_SelectedIndexChanged);
            // 
            // tabPageKGraph
            // 
            this.tabPageKGraph.Controls.Add(this.label3);
            this.tabPageKGraph.Controls.Add(this.textBoxCycleLength);
            this.tabPageKGraph.Location = new System.Drawing.Point(4, 22);
            this.tabPageKGraph.Name = "tabPageKGraph";
            this.tabPageKGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageKGraph.Size = new System.Drawing.Size(160, 221);
            this.tabPageKGraph.TabIndex = 0;
            this.tabPageKGraph.Text = "K线图";
            this.tabPageKGraph.UseVisualStyleBackColor = true;
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
            this.textBoxCycleLength.Location = new System.Drawing.Point(1, 28);
            this.textBoxCycleLength.Name = "textBoxCycleLength";
            this.textBoxCycleLength.Size = new System.Drawing.Size(159, 21);
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
            this.tabPageBarGraph.Size = new System.Drawing.Size(160, 221);
            this.tabPageBarGraph.TabIndex = 1;
            this.tabPageBarGraph.Text = "柱状图";
            this.tabPageBarGraph.UseVisualStyleBackColor = true;
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
            this.comboBoxCollectionDataType.Location = new System.Drawing.Point(6, 63);
            this.comboBoxCollectionDataType.Name = "comboBoxCollectionDataType";
            this.comboBoxCollectionDataType.Size = new System.Drawing.Size(168, 20);
            this.comboBoxCollectionDataType.TabIndex = 3;
            this.comboBoxCollectionDataType.SelectedIndexChanged += new System.EventHandler(this.comboBoxCollectionDataType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
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
            this.comboBoxNumIndex.Location = new System.Drawing.Point(6, 20);
            this.comboBoxNumIndex.Name = "comboBoxNumIndex";
            this.comboBoxNumIndex.Size = new System.Drawing.Size(168, 20);
            this.comboBoxNumIndex.TabIndex = 1;
            this.comboBoxNumIndex.SelectedIndexChanged += new System.EventHandler(this.comboBoxNumIndex_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择数字位：";
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
            // comboBoxBarCollectType
            // 
            this.comboBoxBarCollectType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxBarCollectType.FormattingEnabled = true;
            this.comboBoxBarCollectType.Location = new System.Drawing.Point(3, 23);
            this.comboBoxBarCollectType.Name = "comboBoxBarCollectType";
            this.comboBoxBarCollectType.Size = new System.Drawing.Size(154, 20);
            this.comboBoxBarCollectType.TabIndex = 1;
            this.comboBoxBarCollectType.SelectedIndexChanged += new System.EventHandler(this.comboBoxBarCollectType_SelectedIndexChanged);
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
            // comboBoxCollectRange
            // 
            this.comboBoxCollectRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxCollectRange.FormattingEnabled = true;
            this.comboBoxCollectRange.Location = new System.Drawing.Point(3, 61);
            this.comboBoxCollectRange.Name = "comboBoxCollectRange";
            this.comboBoxCollectRange.Size = new System.Drawing.Size(154, 20);
            this.comboBoxCollectRange.TabIndex = 3;
            this.comboBoxCollectRange.SelectedIndexChanged += new System.EventHandler(this.comboBoxCollectRange_SelectedIndexChanged);
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
            // textBoxCustomCollectRange
            // 
            this.textBoxCustomCollectRange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCustomCollectRange.Location = new System.Drawing.Point(3, 100);
            this.textBoxCustomCollectRange.Name = "textBoxCustomCollectRange";
            this.textBoxCustomCollectRange.Size = new System.Drawing.Size(154, 21);
            this.textBoxCustomCollectRange.TabIndex = 5;
            this.textBoxCustomCollectRange.TextChanged += new System.EventHandler(this.textBoxCustomCollectRange_TextChanged);
            // 
            // LotteryGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 388);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStripGraph);
            this.DoubleBuffered = true;
            this.Name = "LotteryGraph";
            this.Text = "LotteryGraph";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LotteryGraph_FormClosed);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LotteryGraph_Paint);
            this.menuStripGraph.ResumeLayout(false);
            this.menuStripGraph.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlView.ResumeLayout(false);
            this.tabPageKGraph.ResumeLayout(false);
            this.tabPageKGraph.PerformLayout();
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
        private ExtSplitContainer splitContainer1;
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
    }
}