namespace LotteryAnalyze.UI
{
    partial class CollectDataWindow
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
            this.textBoxStartY = new System.Windows.Forms.TextBox();
            this.textBoxStartM = new System.Windows.Forms.TextBox();
            this.textBoxStartD = new System.Windows.Forms.TextBox();
            this.textBoxEndD = new System.Windows.Forms.TextBox();
            this.textBoxEndM = new System.Windows.Forms.TextBox();
            this.textBoxEndY = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerStartDate = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEndDate = new System.Windows.Forms.DateTimePicker();
            this.buttonCollect = new System.Windows.Forms.Button();
            this.textBoxCmd = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "起始日期";
            // 
            // textBoxStartY
            // 
            this.textBoxStartY.Location = new System.Drawing.Point(82, 24);
            this.textBoxStartY.Name = "textBoxStartY";
            this.textBoxStartY.Size = new System.Drawing.Size(78, 21);
            this.textBoxStartY.TabIndex = 1;
            // 
            // textBoxStartM
            // 
            this.textBoxStartM.Location = new System.Drawing.Point(186, 24);
            this.textBoxStartM.Name = "textBoxStartM";
            this.textBoxStartM.Size = new System.Drawing.Size(78, 21);
            this.textBoxStartM.TabIndex = 2;
            // 
            // textBoxStartD
            // 
            this.textBoxStartD.Location = new System.Drawing.Point(293, 24);
            this.textBoxStartD.Name = "textBoxStartD";
            this.textBoxStartD.Size = new System.Drawing.Size(78, 21);
            this.textBoxStartD.TabIndex = 3;
            // 
            // textBoxEndD
            // 
            this.textBoxEndD.Location = new System.Drawing.Point(293, 72);
            this.textBoxEndD.Name = "textBoxEndD";
            this.textBoxEndD.Size = new System.Drawing.Size(78, 21);
            this.textBoxEndD.TabIndex = 7;
            // 
            // textBoxEndM
            // 
            this.textBoxEndM.Location = new System.Drawing.Point(186, 72);
            this.textBoxEndM.Name = "textBoxEndM";
            this.textBoxEndM.Size = new System.Drawing.Size(78, 21);
            this.textBoxEndM.TabIndex = 6;
            // 
            // textBoxEndY
            // 
            this.textBoxEndY.Location = new System.Drawing.Point(82, 72);
            this.textBoxEndY.Name = "textBoxEndY";
            this.textBoxEndY.Size = new System.Drawing.Size(78, 21);
            this.textBoxEndY.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "结束日期";
            // 
            // dateTimePickerStartDate
            // 
            this.dateTimePickerStartDate.Location = new System.Drawing.Point(394, 24);
            this.dateTimePickerStartDate.Name = "dateTimePickerStartDate";
            this.dateTimePickerStartDate.Size = new System.Drawing.Size(200, 21);
            this.dateTimePickerStartDate.TabIndex = 8;
            this.dateTimePickerStartDate.ValueChanged += new System.EventHandler(this.dateTimePickerStartDate_ValueChanged);
            // 
            // dateTimePickerEndDate
            // 
            this.dateTimePickerEndDate.Location = new System.Drawing.Point(394, 72);
            this.dateTimePickerEndDate.Name = "dateTimePickerEndDate";
            this.dateTimePickerEndDate.Size = new System.Drawing.Size(200, 21);
            this.dateTimePickerEndDate.TabIndex = 9;
            this.dateTimePickerEndDate.ValueChanged += new System.EventHandler(this.dateTimePickerEndDate_ValueChanged);
            // 
            // buttonCollect
            // 
            this.buttonCollect.Location = new System.Drawing.Point(5, 104);
            this.buttonCollect.Name = "buttonCollect";
            this.buttonCollect.Size = new System.Drawing.Size(75, 23);
            this.buttonCollect.TabIndex = 10;
            this.buttonCollect.Text = "搜集数据";
            this.buttonCollect.UseVisualStyleBackColor = true;
            this.buttonCollect.Click += new System.EventHandler(this.buttonCollect_Click);
            // 
            // textBoxCmd
            // 
            this.textBoxCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCmd.Location = new System.Drawing.Point(5, 134);
            this.textBoxCmd.Multiline = true;
            this.textBoxCmd.Name = "textBoxCmd";
            this.textBoxCmd.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxCmd.Size = new System.Drawing.Size(589, 211);
            this.textBoxCmd.TabIndex = 11;
            // 
            // CollectDataWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 346);
            this.Controls.Add(this.textBoxCmd);
            this.Controls.Add(this.buttonCollect);
            this.Controls.Add(this.dateTimePickerEndDate);
            this.Controls.Add(this.dateTimePickerStartDate);
            this.Controls.Add(this.textBoxEndD);
            this.Controls.Add(this.textBoxEndM);
            this.Controls.Add(this.textBoxEndY);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxStartD);
            this.Controls.Add(this.textBoxStartM);
            this.Controls.Add(this.textBoxStartY);
            this.Controls.Add(this.label1);
            this.Name = "CollectDataWindow";
            this.Text = "CollectDataWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CollectDataWindow_FormClosed);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CollectDataWindow_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStartY;
        private System.Windows.Forms.TextBox textBoxStartM;
        private System.Windows.Forms.TextBox textBoxStartD;
        private System.Windows.Forms.TextBox textBoxEndD;
        private System.Windows.Forms.TextBox textBoxEndM;
        private System.Windows.Forms.TextBox textBoxEndY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerStartDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerEndDate;
        private System.Windows.Forms.Button buttonCollect;
        private System.Windows.Forms.TextBox textBoxCmd;
    }
}