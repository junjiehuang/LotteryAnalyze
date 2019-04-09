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
            this.SuspendLayout();
            // 
            // treeViewMissCount
            // 
            this.treeViewMissCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewMissCount.Location = new System.Drawing.Point(2, 3);
            this.treeViewMissCount.Name = "treeViewMissCount";
            this.treeViewMissCount.Size = new System.Drawing.Size(546, 239);
            this.treeViewMissCount.TabIndex = 0;
            // 
            // buttonCollectMissCount
            // 
            this.buttonCollectMissCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCollectMissCount.Location = new System.Drawing.Point(2, 244);
            this.buttonCollectMissCount.Name = "buttonCollectMissCount";
            this.buttonCollectMissCount.Size = new System.Drawing.Size(97, 23);
            this.buttonCollectMissCount.TabIndex = 1;
            this.buttonCollectMissCount.Text = "统计所有数据遗漏值";
            this.buttonCollectMissCount.UseVisualStyleBackColor = true;
            this.buttonCollectMissCount.Click += new System.EventHandler(this.buttonCollectMissCount_Click);
            // 
            // progressBarMain
            // 
            this.progressBarMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarMain.Location = new System.Drawing.Point(2, 318);
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(546, 21);
            this.progressBarMain.TabIndex = 2;
            // 
            // progressBarSub
            // 
            this.progressBarSub.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarSub.Location = new System.Drawing.Point(2, 296);
            this.progressBarSub.Name = "progressBarSub";
            this.progressBarSub.Size = new System.Drawing.Size(546, 21);
            this.progressBarSub.Step = 1;
            this.progressBarSub.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 275);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "进度：";
            // 
            // textBoxProgress
            // 
            this.textBoxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProgress.Location = new System.Drawing.Point(47, 271);
            this.textBoxProgress.Name = "textBoxProgress";
            this.textBoxProgress.ReadOnly = true;
            this.textBoxProgress.Size = new System.Drawing.Size(501, 21);
            this.textBoxProgress.TabIndex = 5;
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExport.Location = new System.Drawing.Point(105, 244);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(102, 23);
            this.buttonExport.TabIndex = 6;
            this.buttonExport.Text = "导出统计信息";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // AnalyzeInfoCollectWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 340);
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
    }
}