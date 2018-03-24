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
            this.textBoxCmd = new System.Windows.Forms.TextBox();
            this.progressBarCurrent = new System.Windows.Forms.ProgressBar();
            this.progressBarTotal = new System.Windows.Forms.ProgressBar();
            this.checkBoxSpecNumIndex = new System.Windows.Forms.CheckBox();
            this.comboBoxSpecNumIndex = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxStartDate = new System.Windows.Forms.TextBox();
            this.textBoxEndDate = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "起始金额：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "投注方式：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "每批几天：";
            // 
            // textBoxStartMoney
            // 
            this.textBoxStartMoney.Location = new System.Drawing.Point(84, 10);
            this.textBoxStartMoney.Name = "textBoxStartMoney";
            this.textBoxStartMoney.Size = new System.Drawing.Size(472, 21);
            this.textBoxStartMoney.TabIndex = 3;
            // 
            // textBoxTradeCountLst
            // 
            this.textBoxTradeCountLst.Location = new System.Drawing.Point(84, 34);
            this.textBoxTradeCountLst.Name = "textBoxTradeCountLst";
            this.textBoxTradeCountLst.Size = new System.Drawing.Size(472, 21);
            this.textBoxTradeCountLst.TabIndex = 4;
            // 
            // textBoxDayCountPerBatch
            // 
            this.textBoxDayCountPerBatch.Location = new System.Drawing.Point(84, 58);
            this.textBoxDayCountPerBatch.Name = "textBoxDayCountPerBatch";
            this.textBoxDayCountPerBatch.Size = new System.Drawing.Size(472, 21);
            this.textBoxDayCountPerBatch.TabIndex = 5;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(15, 142);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 6;
            this.buttonStart.Text = "开始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonPauseResume
            // 
            this.buttonPauseResume.Location = new System.Drawing.Point(107, 142);
            this.buttonPauseResume.Name = "buttonPauseResume";
            this.buttonPauseResume.Size = new System.Drawing.Size(75, 23);
            this.buttonPauseResume.TabIndex = 7;
            this.buttonPauseResume.Text = "暂停/恢复";
            this.buttonPauseResume.UseVisualStyleBackColor = true;
            this.buttonPauseResume.Click += new System.EventHandler(this.buttonPauseResume_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(198, 142);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 8;
            this.buttonStop.Text = "结束";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textBoxCmd
            // 
            this.textBoxCmd.AcceptsReturn = true;
            this.textBoxCmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCmd.Location = new System.Drawing.Point(15, 169);
            this.textBoxCmd.Multiline = true;
            this.textBoxCmd.Name = "textBoxCmd";
            this.textBoxCmd.Size = new System.Drawing.Size(541, 162);
            this.textBoxCmd.TabIndex = 10;
            // 
            // progressBarCurrent
            // 
            this.progressBarCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarCurrent.Location = new System.Drawing.Point(15, 337);
            this.progressBarCurrent.Name = "progressBarCurrent";
            this.progressBarCurrent.Size = new System.Drawing.Size(541, 23);
            this.progressBarCurrent.TabIndex = 11;
            // 
            // progressBarTotal
            // 
            this.progressBarTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarTotal.Location = new System.Drawing.Point(15, 367);
            this.progressBarTotal.Name = "progressBarTotal";
            this.progressBarTotal.Size = new System.Drawing.Size(541, 23);
            this.progressBarTotal.TabIndex = 12;
            // 
            // checkBoxSpecNumIndex
            // 
            this.checkBoxSpecNumIndex.AutoSize = true;
            this.checkBoxSpecNumIndex.Location = new System.Drawing.Point(15, 90);
            this.checkBoxSpecNumIndex.Name = "checkBoxSpecNumIndex";
            this.checkBoxSpecNumIndex.Size = new System.Drawing.Size(108, 16);
            this.checkBoxSpecNumIndex.TabIndex = 13;
            this.checkBoxSpecNumIndex.Text = "是否指定数字位";
            this.checkBoxSpecNumIndex.UseVisualStyleBackColor = true;
            this.checkBoxSpecNumIndex.CheckedChanged += new System.EventHandler(this.checkBoxSpecNumIndex_CheckedChanged);
            // 
            // comboBoxSpecNumIndex
            // 
            this.comboBoxSpecNumIndex.FormattingEnabled = true;
            this.comboBoxSpecNumIndex.Location = new System.Drawing.Point(234, 88);
            this.comboBoxSpecNumIndex.Name = "comboBoxSpecNumIndex";
            this.comboBoxSpecNumIndex.Size = new System.Drawing.Size(121, 20);
            this.comboBoxSpecNumIndex.TabIndex = 14;
            this.comboBoxSpecNumIndex.SelectedIndexChanged += new System.EventHandler(this.comboBoxSpecNumIndex_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(175, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "数字位：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "起始日期：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(270, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 17;
            this.label6.Text = "结束日期：";
            // 
            // textBoxStartDate
            // 
            this.textBoxStartDate.Location = new System.Drawing.Point(84, 113);
            this.textBoxStartDate.Name = "textBoxStartDate";
            this.textBoxStartDate.ReadOnly = true;
            this.textBoxStartDate.Size = new System.Drawing.Size(152, 21);
            this.textBoxStartDate.TabIndex = 18;
            // 
            // textBoxEndDate
            // 
            this.textBoxEndDate.Location = new System.Drawing.Point(341, 112);
            this.textBoxEndDate.Name = "textBoxEndDate";
            this.textBoxEndDate.ReadOnly = true;
            this.textBoxEndDate.Size = new System.Drawing.Size(152, 21);
            this.textBoxEndDate.TabIndex = 19;
            // 
            // GlobalSimTradeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 394);
            this.Controls.Add(this.textBoxEndDate);
            this.Controls.Add(this.textBoxStartDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBoxSpecNumIndex);
            this.Controls.Add(this.checkBoxSpecNumIndex);
            this.Controls.Add(this.progressBarTotal);
            this.Controls.Add(this.progressBarCurrent);
            this.Controls.Add(this.textBoxCmd);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonPauseResume);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.textBoxDayCountPerBatch);
            this.Controls.Add(this.textBoxTradeCountLst);
            this.Controls.Add(this.textBoxStartMoney);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GlobalSimTradeWindow";
            this.Text = "GlobalSimTradeWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GlobalSimTradeWindow_FormClosed);
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
        private System.Windows.Forms.TextBox textBoxCmd;
        private System.Windows.Forms.ProgressBar progressBarCurrent;
        private System.Windows.Forms.ProgressBar progressBarTotal;
        private System.Windows.Forms.CheckBox checkBoxSpecNumIndex;
        private System.Windows.Forms.ComboBox comboBoxSpecNumIndex;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxStartDate;
        private System.Windows.Forms.TextBox textBoxEndDate;
    }
}