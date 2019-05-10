namespace LotteryAnalyze.UI
{
    partial class TradeDebugWindow
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
            this.treeViewDebugNodes = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDataItemTag = new System.Windows.Forms.TextBox();
            this.buttonClearAllBPs = new System.Windows.Forms.Button();
            this.textBoxWrongCountBP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonStartDebug = new System.Windows.Forms.Button();
            this.buttonStep = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeViewDebugNodes
            // 
            this.treeViewDebugNodes.CheckBoxes = true;
            this.treeViewDebugNodes.FullRowSelect = true;
            this.treeViewDebugNodes.Location = new System.Drawing.Point(12, 50);
            this.treeViewDebugNodes.Name = "treeViewDebugNodes";
            this.treeViewDebugNodes.Size = new System.Drawing.Size(239, 223);
            this.treeViewDebugNodes.TabIndex = 0;
            this.treeViewDebugNodes.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDebugNodes_AfterCheck);
            this.treeViewDebugNodes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDebugNodes_AfterSelect);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "K线形态断点：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 285);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "奖号断点：";
            // 
            // textBoxDataItemTag
            // 
            this.textBoxDataItemTag.Location = new System.Drawing.Point(12, 301);
            this.textBoxDataItemTag.Name = "textBoxDataItemTag";
            this.textBoxDataItemTag.Size = new System.Drawing.Size(239, 21);
            this.textBoxDataItemTag.TabIndex = 3;
            this.textBoxDataItemTag.TextChanged += new System.EventHandler(this.textBoxDataItemTag_TextChanged);
            // 
            // buttonClearAllBPs
            // 
            this.buttonClearAllBPs.Location = new System.Drawing.Point(12, 6);
            this.buttonClearAllBPs.Name = "buttonClearAllBPs";
            this.buttonClearAllBPs.Size = new System.Drawing.Size(239, 23);
            this.buttonClearAllBPs.TabIndex = 4;
            this.buttonClearAllBPs.Text = "清空所有断点";
            this.buttonClearAllBPs.UseVisualStyleBackColor = true;
            this.buttonClearAllBPs.Click += new System.EventHandler(this.buttonClearAllBPs_Click);
            // 
            // textBoxWrongCountBP
            // 
            this.textBoxWrongCountBP.Location = new System.Drawing.Point(12, 347);
            this.textBoxWrongCountBP.Name = "textBoxWrongCountBP";
            this.textBoxWrongCountBP.Size = new System.Drawing.Size(239, 21);
            this.textBoxWrongCountBP.TabIndex = 6;
            this.textBoxWrongCountBP.TextChanged += new System.EventHandler(this.textBoxWrongCountBP_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 331);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "连错N期断点：";
            // 
            // buttonStartDebug
            // 
            this.buttonStartDebug.Location = new System.Drawing.Point(12, 375);
            this.buttonStartDebug.Name = "buttonStartDebug";
            this.buttonStartDebug.Size = new System.Drawing.Size(63, 23);
            this.buttonStartDebug.TabIndex = 7;
            this.buttonStartDebug.Text = "开始调试";
            this.buttonStartDebug.UseVisualStyleBackColor = true;
            this.buttonStartDebug.Click += new System.EventHandler(this.buttonStartDebug_Click);
            // 
            // buttonStep
            // 
            this.buttonStep.Location = new System.Drawing.Point(81, 375);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(63, 23);
            this.buttonStep.TabIndex = 8;
            this.buttonStep.Text = "单步";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
            // 
            // TradeDebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 405);
            this.Controls.Add(this.buttonStep);
            this.Controls.Add(this.buttonStartDebug);
            this.Controls.Add(this.textBoxWrongCountBP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonClearAllBPs);
            this.Controls.Add(this.textBoxDataItemTag);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeViewDebugNodes);
            this.Name = "TradeDebugWindow";
            this.Text = "TradeDebugWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TradeDebugWindow_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewDebugNodes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDataItemTag;
        private System.Windows.Forms.Button buttonClearAllBPs;
        private System.Windows.Forms.TextBox textBoxWrongCountBP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonStartDebug;
        private System.Windows.Forms.Button buttonStep;
    }
}