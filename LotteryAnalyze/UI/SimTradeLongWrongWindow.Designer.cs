namespace LotteryAnalyze.UI
{
    partial class SimTradeLongWrongWindow
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
            this.treeViewLongWrongInfo = new System.Windows.Forms.TreeView();
            this.buttonLoadLongWrongInfo = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStep = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonClearEarnNode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeViewLongWrongInfo
            // 
            this.treeViewLongWrongInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewLongWrongInfo.Location = new System.Drawing.Point(2, 3);
            this.treeViewLongWrongInfo.Name = "treeViewLongWrongInfo";
            this.treeViewLongWrongInfo.Size = new System.Drawing.Size(489, 257);
            this.treeViewLongWrongInfo.TabIndex = 0;
            // 
            // buttonLoadLongWrongInfo
            // 
            this.buttonLoadLongWrongInfo.Location = new System.Drawing.Point(2, 266);
            this.buttonLoadLongWrongInfo.Name = "buttonLoadLongWrongInfo";
            this.buttonLoadLongWrongInfo.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadLongWrongInfo.TabIndex = 1;
            this.buttonLoadLongWrongInfo.Text = "读取配置";
            this.buttonLoadLongWrongInfo.UseVisualStyleBackColor = true;
            this.buttonLoadLongWrongInfo.Click += new System.EventHandler(this.buttonLoadLongWrongInfo_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(83, 266);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "开始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStep
            // 
            this.buttonStep.Location = new System.Drawing.Point(164, 266);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(75, 23);
            this.buttonStep.TabIndex = 3;
            this.buttonStep.Text = "单步";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(245, 266);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "结束";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Location = new System.Drawing.Point(326, 266);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(75, 23);
            this.buttonPause.TabIndex = 5;
            this.buttonPause.Text = "暂停";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonClearEarnNode
            // 
            this.buttonClearEarnNode.Location = new System.Drawing.Point(407, 266);
            this.buttonClearEarnNode.Name = "buttonClearEarnNode";
            this.buttonClearEarnNode.Size = new System.Drawing.Size(84, 23);
            this.buttonClearEarnNode.TabIndex = 6;
            this.buttonClearEarnNode.Text = "清除通过的";
            this.buttonClearEarnNode.UseVisualStyleBackColor = true;
            this.buttonClearEarnNode.Click += new System.EventHandler(this.buttonClearEarnNode_Click);
            // 
            // SimTradeLongWrongWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 296);
            this.Controls.Add(this.buttonClearEarnNode);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStep);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonLoadLongWrongInfo);
            this.Controls.Add(this.treeViewLongWrongInfo);
            this.Name = "SimTradeLongWrongWindow";
            this.Text = "SimTradeLongWrongWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SimTradeLongWrongWindow_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewLongWrongInfo;
        private System.Windows.Forms.Button buttonLoadLongWrongInfo;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStep;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonClearEarnNode;
    }
}