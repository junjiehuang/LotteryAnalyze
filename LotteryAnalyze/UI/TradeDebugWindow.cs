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

            for( TradeDataManager.KGraphConfig i = TradeDataManager.KGraphConfig.eNone;
                i < TradeDataManager.KGraphConfig.eMAX; ++i )
            {
                TreeNode node = new TreeNode(i.ToString());
                node.Checked = TradeDataManager.Instance.debugNodes[i];
                node.Tag = i;
                treeViewDebugNodes.Nodes.Add(node);
            }
            FormMain.AddWindow(this);
        }

        private void treeViewDebugNodes_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeViewDebugNodes_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            //if (sender is TreeView)
            //    node = (sender as TreeView).SelectedNode;
            //else if (sender is TreeNode)
            //    node = sender as TreeNode;
            if (node != null)
            {
                TradeDataManager.KGraphConfig cfg = (TradeDataManager.KGraphConfig)(node.Tag);
                TradeDataManager.Instance.debugNodes[cfg] = node.Checked;
            }
        }

        private void TradeDebugWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            sInst = null;
            FormMain.RemoveWindow(this);
        }
    }
}
