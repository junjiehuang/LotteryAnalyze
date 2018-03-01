namespace LotteryAnalyze.UI
{
    partial class TradeCalculater
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
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxNumCount = new System.Windows.Forms.TextBox();
            this.textBoxReward = new System.Windows.Forms.TextBox();
            this.textBoxCost = new System.Windows.Forms.TextBox();
            this.textBoxTradeSlu = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.buttonStartCalc = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "投注方案：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "每笔注数：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(179, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "单注奖金：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(339, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "单注成本：";
            // 
            // textBoxNumCount
            // 
            this.textBoxNumCount.Location = new System.Drawing.Point(83, 6);
            this.textBoxNumCount.Name = "textBoxNumCount";
            this.textBoxNumCount.Size = new System.Drawing.Size(75, 21);
            this.textBoxNumCount.TabIndex = 4;
            // 
            // textBoxReward
            // 
            this.textBoxReward.Location = new System.Drawing.Point(250, 6);
            this.textBoxReward.Name = "textBoxReward";
            this.textBoxReward.Size = new System.Drawing.Size(75, 21);
            this.textBoxReward.TabIndex = 5;
            // 
            // textBoxCost
            // 
            this.textBoxCost.Location = new System.Drawing.Point(410, 6);
            this.textBoxCost.Name = "textBoxCost";
            this.textBoxCost.Size = new System.Drawing.Size(75, 21);
            this.textBoxCost.TabIndex = 6;
            // 
            // textBoxTradeSlu
            // 
            this.textBoxTradeSlu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTradeSlu.Location = new System.Drawing.Point(83, 41);
            this.textBoxTradeSlu.Multiline = true;
            this.textBoxTradeSlu.Name = "textBoxTradeSlu";
            this.textBoxTradeSlu.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxTradeSlu.Size = new System.Drawing.Size(402, 46);
            this.textBoxTradeSlu.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "计算结果：";
            // 
            // textBoxResult
            // 
            this.textBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResult.Location = new System.Drawing.Point(83, 94);
            this.textBoxResult.Multiline = true;
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResult.Size = new System.Drawing.Size(402, 284);
            this.textBoxResult.TabIndex = 9;
            // 
            // buttonStartCalc
            // 
            this.buttonStartCalc.Location = new System.Drawing.Point(16, 386);
            this.buttonStartCalc.Name = "buttonStartCalc";
            this.buttonStartCalc.Size = new System.Drawing.Size(75, 23);
            this.buttonStartCalc.TabIndex = 10;
            this.buttonStartCalc.Text = "开始计算";
            this.buttonStartCalc.UseVisualStyleBackColor = true;
            this.buttonStartCalc.Click += new System.EventHandler(this.buttonStartCalc_Click);
            // 
            // TradeCalculater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 421);
            this.Controls.Add(this.buttonStartCalc);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxTradeSlu);
            this.Controls.Add(this.textBoxCost);
            this.Controls.Add(this.textBoxReward);
            this.Controls.Add(this.textBoxNumCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TradeCalculater";
            this.Text = "TradeCalculater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxNumCount;
        private System.Windows.Forms.TextBox textBoxReward;
        private System.Windows.Forms.TextBox textBoxCost;
        private System.Windows.Forms.TextBox textBoxTradeSlu;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.Button buttonStartCalc;
    }
}