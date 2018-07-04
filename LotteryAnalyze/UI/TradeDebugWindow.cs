using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze.UI
{
    public partial class TradeDebugWindow : Form
    {
        static TradeDebugWindow sInst = null;
        public static void Open()
        {
            if (sInst == null)
                sInst = new TradeDebugWindow();
            sInst.Show();
        }

        public TradeDebugWindow()
        {
            InitializeComponent();

            Data2UI();
            FormMain.AddWindow(this);
        }

        void Data2UI()
        {
            treeViewDebugNodes.Nodes.Clear();
            for (TradeDataManager.KGraphConfig i = TradeDataManager.KGraphConfig.eNone; i < TradeDataManager.KGraphConfig.eMAX; ++i)
            {
                TreeNode node = new TreeNode(i.ToString());
                node.Checked = TradeDataManager.Instance.debugInfo.kGraphCfgBPs[i];
                node.Tag = i;
                treeViewDebugNodes.Nodes.Add(node);
            }
            textBoxDataItemTag.Text = TradeDataManager.Instance.debugInfo.dataItemTagBP;
            textBoxWrongCountBP.Text = TradeDataManager.Instance.debugInfo.wrongCountBP.ToString();
        }

        private void treeViewDebugNodes_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeViewDebugNodes_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node != null)
            {
                TradeDataManager.KGraphConfig cfg = (TradeDataManager.KGraphConfig)(node.Tag);
                TradeDataManager.Instance.debugInfo.kGraphCfgBPs[cfg] = node.Checked;
            }
        }

        private void TradeDebugWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            sInst = null;
            FormMain.RemoveWindow(this);
        }

        private void textBoxDataItemTag_TextChanged(object sender, EventArgs e)
        {
            TradeDataManager.Instance.debugInfo.dataItemTagBP = textBoxDataItemTag.Text;
        }        

        private void textBoxWrongCountBP_TextChanged(object sender, EventArgs e)
        {
            TradeDataManager.Instance.debugInfo.wrongCountBP = int.Parse(textBoxWrongCountBP.Text);
        }

        private void buttonClearAllBPs_Click(object sender, EventArgs e)
        {
            TradeDataManager.Instance.debugInfo.ClearAllBreakPoints();
            Data2UI();
        }
    }
}
