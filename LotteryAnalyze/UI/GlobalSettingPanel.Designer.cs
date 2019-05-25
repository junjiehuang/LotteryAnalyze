namespace LotteryAnalyze.UI
{
    partial class GlobalSettingPanel
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
            this.components = new System.ComponentModel.Container();
            this.toolTipHandler = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // GlobalSettingPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 294);
            this.DoubleBuffered = true;
            this.Name = "GlobalSettingPanel";
            this.Text = "GlobalSettingPanel";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GlobalSettingPanel_FormClosed);
            this.Load += new System.EventHandler(this.GlobalSettingPanel_Load);
            this.ResizeEnd += new System.EventHandler(this.GlobalSettingPanel_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTipHandler;
    }
}