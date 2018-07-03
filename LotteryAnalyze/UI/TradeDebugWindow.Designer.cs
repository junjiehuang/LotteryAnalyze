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
            this.SuspendLayout();
            // 
            // treeViewDebugNodes
            // 
            this.treeViewDebugNodes.CheckBoxes = true;
            this.treeViewDebugNodes.FullRowSelect = true;
            this.treeViewDebugNodes.Location = new System.Drawing.Point(12, 12);
            this.treeViewDebugNodes.Name = "treeViewDebugNodes";
            this.treeViewDebugNodes.Size = new System.Drawing.Size(349, 223);
            this.treeViewDebugNodes.TabIndex = 0;
            this.treeViewDebugNodes.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDebugNodes_AfterCheck);
            this.treeViewDebugNodes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDebugNodes_AfterSelect);
            // 
            // TradeDebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 261);
            this.Controls.Add(this.treeViewDebugNodes);
            this.Name = "TradeDebugWindow";
            this.Text = "TradeDebugWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TradeDebugWindow_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewDebugNodes;
    }
}