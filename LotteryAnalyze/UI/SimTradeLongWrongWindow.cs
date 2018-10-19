using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace LotteryAnalyze.UI
{
    public partial class SimTradeLongWrongWindow : Form, UpdaterBase
    {
        static SimTradeLongWrongWindow sInst;
        TreeNode curParentNode = null;
        TreeNode curSubNode = null;
        bool isPause = false;
        double updateInterval = 0.5;
        double updateCountDown = 0;


        public static void Open()
        {
            if (sInst == null)
                sInst = new SimTradeLongWrongWindow();
            sInst.Show();
        }


        SimTradeLongWrongWindow()
        {
            InitializeComponent();

            //采用双缓冲技术的控件必需的设置
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            buttonPause.Text = isPause ? "恢复" : "暂停";

            FormMain.AddWindow(this);
            Program.AddUpdater(this);
        }

        void LoadPatch()
        {
            string fileName = "..\\tools\\模拟结果.xml";
            XmlDocument x = new XmlDocument();
            try
            {
                x.Load(fileName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

            XmlNode pNode = x.SelectSingleNode("root/LongMissTradeInfos");
            if (pNode == null)
                return;

            treeViewLongWrongInfo.Nodes.Clear();
            foreach ( XmlNode subNode in pNode.ChildNodes )
            {
                string name = subNode.Name;
                foreach (XmlAttribute att in subNode.Attributes)
                {
                    if (att.Name == "name")
                        name = att.Value;
                }
                TreeNode pTN = new TreeNode(name);
                treeViewLongWrongInfo.Nodes.Add(pTN);
                foreach (XmlNode node in subNode.ChildNodes)
                {
                    TreeNode sTN = new TreeNode(node.InnerXml);
                    string[] strs = node.InnerXml.Split(',');
                    strs = strs[0].Split('-');
                    sTN.Tag = strs[0];
                    pTN.Nodes.Add(sTN);
                }
            }
        }

        void Start()
        {
            if (treeViewLongWrongInfo.Nodes.Count == 0)
                return;
            curParentNode = treeViewLongWrongInfo.Nodes[0];
            curSubNode = curParentNode.Nodes[0];
            treeViewLongWrongInfo.SelectedNode = curSubNode;
            isPause = false;
            buttonPause.Text = isPause ? "恢复" : "暂停";
            PrepareSim();
        }

        void PrepareSim()
        {
            treeViewLongWrongInfo.Focus();
            int index = FormMain.Instance.GetDateIndex((string)curSubNode.Tag);
            if (index >= 0)
            {
                int startDate = FormMain.Instance.GetDateTag(index);
                int endDate = FormMain.Instance.GetDateTag(index + 1);
                if (endDate == -1)
                    endDate = startDate;
                if (startDate != -1)
                {
                    GlobalSimTradeWindow.Open();
                    GlobalSimTradeWindow.Instance.DoStop();

                    BatchTradeSimulator.Instance.Stop();
                    GlobalSimTradeWindow.SetSimStartDateAndEndDate(startDate, endDate);
                    LotteryGraph.Open(false);

                    GlobalSimTradeWindow.Instance.DoStart();
                    updateCountDown = updateInterval;
                }
            }
        }

        void Stop()
        {
            GlobalSimTradeWindow.Instance.DoStop();
            BatchTradeSimulator.Instance.Stop();
            isPause = false;
            buttonPause.Text = isPause ? "恢复" : "暂停";
        }

        void Step()
        {
            if(curSubNode == curParentNode.LastNode)
            {
                int pindex = treeViewLongWrongInfo.Nodes.IndexOf(curParentNode);
                if (pindex == treeViewLongWrongInfo.Nodes.Count-1)
                {
                    curParentNode = null;
                    curSubNode = null;
                    treeViewLongWrongInfo.SelectedNode = null;
                    return;
                }
                else
                {
                    curParentNode = treeViewLongWrongInfo.Nodes[pindex + 1];
                    curSubNode = curParentNode.FirstNode;
                }
            }
            else
            {
                curSubNode = curSubNode.NextNode;
            }
            treeViewLongWrongInfo.SelectedNode = curSubNode;

            PrepareSim();
            this.Invalidate(true);
        }

        public void OnUpdate()
        {
            if (updateCountDown <= 0)
            {
                updateCountDown = updateInterval;
                if (BatchTradeSimulator.Instance.HasFinished())
                {
                    if(isPause == false)
                        Step();
                }
            }
            else
            {
                updateCountDown -= Program.DeltaTime;
            }
        }

        private void SimTradeLongWrongWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMain.RemoveWindow(this);
            Program.RemoveUpdater(this);
            sInst = null;
        }

        private void buttonLoadLongWrongInfo_Click(object sender, EventArgs e)
        {
            LoadPatch();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            isPause = !isPause;
            buttonPause.Text = isPause ? "恢复" : "暂停";
        }

        private void buttonStep_Click(object sender, EventArgs e)
        {
            if(BatchTradeSimulator.Instance.IsSimulating == false)
            {
                Start();
                isPause = true;
                buttonPause.Text = isPause ? "恢复" : "暂停";
            }
            else if (BatchTradeSimulator.Instance.HasFinished())
            {
                if (curSubNode == null)
                {
                    Start();
                    isPause = true;
                    buttonPause.Text = isPause ? "恢复" : "暂停";
                }
                else
                {
                    Step();
                }
            }
        }
    }
}
