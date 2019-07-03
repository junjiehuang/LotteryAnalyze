namespace LotteryAnalyze.UI
{
    partial class AnalyzeInfoCollectWindow
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
            this.treeViewMissCount = new System.Windows.Forms.TreeView();
            this.buttonCollectMissCount = new System.Windows.Forms.Button();
            this.progressBarMain = new System.Windows.Forms.ProgressBar();
            this.progressBarSub = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxProgress = new System.Windows.Forms.TextBox();
            this.buttonExport = new System.Windows.Forms.Button();
            this.comboBoxDataAnalyseType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonImportData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeViewMissCount
            // 
            this.treeViewMissCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewMissCount.Location = new System.Drawing.Point(2, 3);
            this.treeViewMissCount.Name = "treeViewMissCount";
            this.treeViewMissCount.Size = new System.Drawing.Size(506, 158);
            this.treeViewMissCount.TabIndex = 0;
            this.treeViewMissCount.DoubleClick += new System.EventHandler(this.treeViewMissCount_DoubleClick);
            // 
            // buttonCollectMissCount
            // 
            this.buttonCollectMissCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCollectMissCount.Location = new System.Drawing.Point(2, 195);
            this.buttonCollectMissCount.Name = "buttonCollectMissCount";
            this.buttonCollectMissCount.Size = new System.Drawing.Size(81, 23);
            this.buttonCollectMissCount.TabIndex = 1;
            this.buttonCollectMissCount.Text = "开始统计";
            this.buttonCollectMissCount.UseVisualStyleBackColor = true;
            this.buttonCollectMissCount.Click += new System.EventHandler(this.buttonCollectMissCount_Click);
            // 
            // progressBarMain
            // 
            this.progressBarMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarMain.Location = new System.Drawing.Point(2, 269);
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(506, 21);
            this.progressBarMain.TabIndex = 2;
            // 
            // progressBarSub
            // 
            this.progressBarSub.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarSub.Location = new System.Drawing.Point(2, 247);
            this.progressBarSub.Name = "progressBarSub";
            this.progressBarSub.Size = new System.Drawing.Size(506, 21);
            this.progressBarSub.Step = 1;
            this.progressBarSub.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 226);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "进度：";
            // 
            // textBoxProgress
            // 
            this.textBoxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProgress.Location = new System.Drawing.Point(47, 222);
            this.textBoxProgress.Name = "textBoxProgress";
            this.textBoxProgress.ReadOnly = true;
            this.textBoxProgress.Size = new System.Drawing.Size(461, 21);
            this.textBoxProgress.TabIndex = 5;
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExport.Location = new System.Drawing.Point(176, 195);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(102, 23);
            this.buttonExport.TabIndex = 6;
            this.buttonExport.Text = "导出统计信息";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // comboBoxDataAnalyseType
            // 
            this.comboBoxDataAnalyseType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDataAnalyseType.FormattingEnabled = true;
            this.comboBoxDataAnalyseType.Location = new System.Drawing.Point(98, 163);
            this.comboBoxDataAnalyseType.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.comboBoxDataAnalyseType.Name = "comboBoxDataAnalyseType";
            this.comboBoxDataAnalyseType.Size = new System.Drawing.Size(402, 20);
            this.comboBoxDataAnalyseType.TabIndex = 7;
            this.comboBoxDataAnalyseType.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataAnalyseType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 166);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "统计类型:";
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.Location = new System.Drawing.Point(89, 195);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(81, 23);
            this.buttonStop.TabIndex = 9;
            this.buttonStop.Text = "停止统计";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonImportData
            // 
            this.buttonImportData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonImportData.Location = new System.Drawing.Point(284, 195);
            this.buttonImportData.Name = "buttonImportData";
            this.buttonImportData.Size = new System.Drawing.Size(102, 23);
            this.buttonImportData.TabIndex = 10;
            this.buttonImportData.Text = "导入统计信息";
            this.buttonImportData.UseVisualStyleBackColor = true;
            this.buttonImportData.Click += new System.EventHandler(this.buttonImportData_Click);
            // 
            // AnalyzeInfoCollectWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 291);
            this.Controls.Add(this.buttonImportData);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxDataAnalyseType);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.textBoxProgress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBarSub);
            this.Controls.Add(this.progressBarMain);
            this.Controls.Add(this.buttonCollectMissCount);
            this.Controls.Add(this.treeViewMissCount);
            this.Name = "AnalyzeInfoCollectWindow";
            this.Text = "AnalyzeInfoCollectWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AnalyzeInfoCollectWindow_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewMissCount;
        private System.Windows.Forms.Button buttonCollectMissCount;
        private System.Windows.Forms.ProgressBar progressBarMain;
        private System.Windows.Forms.ProgressBar progressBarSub;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxProgress;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.ComboBox comboBoxDataAnalyseType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonImportData;
    }
}