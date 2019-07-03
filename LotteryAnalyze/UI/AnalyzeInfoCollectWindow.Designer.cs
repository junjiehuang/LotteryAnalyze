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
            this.SuspendLayout();
            // 
            // treeViewMissCount
            // 
            this.treeViewMissCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewMissCount.Location = new System.Drawing.Point(5, 8);
            this.treeViewMissCount.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.treeViewMissCount.Name = "treeViewMissCount";
            this.treeViewMissCount.Size = new System.Drawing.Size(1349, 388);
            this.treeViewMissCount.TabIndex = 0;
            // 
            // buttonCollectMissCount
            // 
            this.buttonCollectMissCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCollectMissCount.Location = new System.Drawing.Point(5, 487);
            this.buttonCollectMissCount.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.buttonCollectMissCount.Name = "buttonCollectMissCount";
            this.buttonCollectMissCount.Size = new System.Drawing.Size(203, 58);
            this.buttonCollectMissCount.TabIndex = 1;
            this.buttonCollectMissCount.Text = "开始统计";
            this.buttonCollectMissCount.UseVisualStyleBackColor = true;
            this.buttonCollectMissCount.Click += new System.EventHandler(this.buttonCollectMissCount_Click);
            // 
            // progressBarMain
            // 
            this.progressBarMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarMain.Location = new System.Drawing.Point(5, 672);
            this.progressBarMain.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(1355, 52);
            this.progressBarMain.TabIndex = 2;
            // 
            // progressBarSub
            // 
            this.progressBarSub.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarSub.Location = new System.Drawing.Point(5, 617);
            this.progressBarSub.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.progressBarSub.Name = "progressBarSub";
            this.progressBarSub.Size = new System.Drawing.Size(1355, 52);
            this.progressBarSub.Step = 1;
            this.progressBarSub.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 565);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 30);
            this.label1.TabIndex = 4;
            this.label1.Text = "进度：";
            // 
            // textBoxProgress
            // 
            this.textBoxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProgress.Location = new System.Drawing.Point(118, 555);
            this.textBoxProgress.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.textBoxProgress.Name = "textBoxProgress";
            this.textBoxProgress.ReadOnly = true;
            this.textBoxProgress.Size = new System.Drawing.Size(1236, 42);
            this.textBoxProgress.TabIndex = 5;
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExport.Location = new System.Drawing.Point(245, 487);
            this.buttonExport.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(255, 58);
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
            this.comboBoxDataAnalyseType.Location = new System.Drawing.Point(245, 407);
            this.comboBoxDataAnalyseType.Name = "comboBoxDataAnalyseType";
            this.comboBoxDataAnalyseType.Size = new System.Drawing.Size(1109, 38);
            this.comboBoxDataAnalyseType.TabIndex = 7;
            this.comboBoxDataAnalyseType.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataAnalyseType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 415);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 30);
            this.label2.TabIndex = 8;
            this.label2.Text = "统计类型:";
            // 
            // AnalyzeInfoCollectWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 727);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxDataAnalyseType);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.textBoxProgress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBarSub);
            this.Controls.Add(this.progressBarMain);
            this.Controls.Add(this.buttonCollectMissCount);
            this.Controls.Add(this.treeViewMissCount);
            this.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
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
    }
}