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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ColumnData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.andValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rearValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.crossValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.group6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.group3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.group1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(771, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(39, 21);
            this.toolStripMenuItem1.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnData,
            this.Number,
            this.andValue,
            this.rearValue,
            this.crossValue,
            this.group6,
            this.group3,
            this.group1});
            this.dataGridView1.Location = new System.Drawing.Point(0, 28);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(771, 266);
            this.dataGridView1.TabIndex = 1;
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 361);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnData;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn andValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn rearValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn crossValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn group6;
        private System.Windows.Forms.DataGridViewTextBoxColumn group3;
        private System.Windows.Forms.DataGridViewTextBoxColumn group1;
    }
}

