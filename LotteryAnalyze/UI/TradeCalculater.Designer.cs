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
            this.textBoxCount = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonCalcByCount = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxParam = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxGenerateType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 33);
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
            this.textBoxTradeSlu.Location = new System.Drawing.Point(83, 30);
            this.textBoxTradeSlu.Multiline = true;
            this.textBoxTradeSlu.Name = "textBoxTradeSlu";
            this.textBoxTradeSlu.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxTradeSlu.Size = new System.Drawing.Size(429, 46);
            this.textBoxTradeSlu.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "计算结果：";
            // 
            // textBoxResult
            // 
            this.textBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResult.Location = new System.Drawing.Point(83, 78);
            this.textBoxResult.Multiline = true;
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxResult.Size = new System.Drawing.Size(429, 282);
            this.textBoxResult.TabIndex = 9;
            // 
            // buttonStartCalc
            // 
            this.buttonStartCalc.Location = new System.Drawing.Point(16, 364);
            this.buttonStartCalc.Name = "buttonStartCalc";
            this.buttonStartCalc.Size = new System.Drawing.Size(142, 23);
            this.buttonStartCalc.TabIndex = 10;
            this.buttonStartCalc.Text = "按给定的投注方案计算";
            this.buttonStartCalc.UseVisualStyleBackColor = true;
            this.buttonStartCalc.Click += new System.EventHandler(this.buttonStartCalc_Click);
            // 
            // textBoxCount
            // 
            this.textBoxCount.Location = new System.Drawing.Point(83, 394);
            this.textBoxCount.Name = "textBoxCount";
            this.textBoxCount.Size = new System.Drawing.Size(75, 21);
            this.textBoxCount.TabIndex = 11;
            this.textBoxCount.TextChanged += new System.EventHandler(this.textBoxCount_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 397);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "投注次数：";
            // 
            // buttonCalcByCount
            // 
            this.buttonCalcByCount.Location = new System.Drawing.Point(164, 364);
            this.buttonCalcByCount.Name = "buttonCalcByCount";
            this.buttonCalcByCount.Size = new System.Drawing.Size(210, 23);
            this.buttonCalcByCount.TabIndex = 13;
            this.buttonCalcByCount.Text = "按指定条件生成投注方案并计算";
            this.buttonCalcByCount.UseVisualStyleBackColor = true;
            this.buttonCalcByCount.Click += new System.EventHandler(this.buttonCalcByCount_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(163, 397);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "参数：";
            // 
            // textBoxParam
            // 
            this.textBoxParam.Location = new System.Drawing.Point(210, 394);
            this.textBoxParam.Name = "textBoxParam";
            this.textBoxParam.Size = new System.Drawing.Size(81, 21);
            this.textBoxParam.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 423);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "生成类型：";
            // 
            // comboBoxGenerateType
            // 
            this.comboBoxGenerateType.FormattingEnabled = true;
            this.comboBoxGenerateType.Location = new System.Drawing.Point(83, 420);
            this.comboBoxGenerateType.Name = "comboBoxGenerateType";
            this.comboBoxGenerateType.Size = new System.Drawing.Size(429, 20);
            this.comboBoxGenerateType.TabIndex = 17;
            // 
            // TradeCalculater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 446);
            this.Controls.Add(this.comboBoxGenerateType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxParam);
            this.Controls.Add(this.buttonCalcByCount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxCount);
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
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TradeCalculater_FormClosed);
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
        private System.Windows.Forms.TextBox textBoxCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonCalcByCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxParam;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxGenerateType;
    }
}