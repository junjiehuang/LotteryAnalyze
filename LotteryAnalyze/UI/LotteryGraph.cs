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
    public partial class LotteryGraph : Form
    {
        static List<LotteryGraph> instLst = new List<LotteryGraph>();

        GraphManager graphMgr = new GraphManager();
        int numberIndex = 0;        
        int curCDTIndex = 0;
        Point currentPoint = new Point();
        Point mouseRelPos = new Point();

        public static void Open()
        {
            LotteryGraph graphInst = new LotteryGraph();
            graphInst.Show();
        }
        
        public LotteryGraph()
        {
            instLst.Add(this);
            InitializeComponent();

            //采用双缓冲技术的控件必需的设置
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            graphMgr.SetCurrentGraph(GraphType.eKCurveGraph);

            comboBoxCollectionDataType.DataSource = GraphDataManager.S_CDT_TAG_LIST;
            comboBoxCollectionDataType.SelectedIndex = curCDTIndex;
            comboBoxNumIndex.SelectedIndex = numberIndex;
            textBoxCycleLength.Text = GraphDataManager.KGDC.CycleLength.ToString();

            comboBoxBarCollectType.DataSource = BarGraphDataContianer.S_StatisticsType_STRS;
            comboBoxCollectRange.DataSource = BarGraphDataContianer.S_StatisticsRange_STRS;
            comboBoxBarCollectType.SelectedIndex = (int)GraphDataManager.BGDC.curStatisticsType;
            comboBoxCollectRange.SelectedIndex = (int)GraphDataManager.BGDC.curStatisticsRange;
            textBoxCustomCollectRange.Text = GraphDataManager.BGDC.customStatisticsRange.ToString();

            comboBoxAvgAlgorithm.DataSource = KGraphDataContainer.S_AVG_ALGORITHM_STRS;
            comboBoxAvgAlgorithm.SelectedIndex = (int)KGraphDataContainer.S_AVG_ALGORITHM;

            RefreshUI();
        }

        void RefreshUI()
        {
            CheckBox[] cbs = new CheckBox[] { checkBoxAvg5, checkBoxAvg10, checkBoxAvg20, checkBoxAvg30, checkBoxAvg50, checkBoxAvg100, };
            Button[] btns = new Button[] { buttonAvg5, buttonAvg10, buttonAvg20, buttonAvg30, buttonAvg50, buttonAvg100, };
            for (int i = 0; i < KGraphDataContainer.S_AVG_LINE_SETTINGS.Count; ++i)
            {
                cbs[i].Enabled = KGraphDataContainer.S_AVG_LINE_SETTINGS[i].enable;
                cbs[i].Text = KGraphDataContainer.S_AVG_LINE_SETTINGS[i].tag;
                btns[i].BackColor = KGraphDataContainer.S_AVG_LINE_SETTINGS[i].color;
                cbs[i].Tag = KGraphDataContainer.S_AVG_LINE_SETTINGS[i];
            }
        }

        static void NotifyOtherGraphRefreshUI(LotteryGraph sender)
        {
            for( int i = 0; i < instLst.Count; ++i )
            {
                if(instLst[i] != sender)
                {
                    instLst[i].RefreshUI();
                }
            }
        } 

        void DrawCanvas(Graphics g)
        {
            g.Clear(Color.Black);

            CollectDataType cdt = GraphDataManager.S_CDT_LIST[curCDTIndex];
            graphMgr.DrawGraph(g, numberIndex, cdt, this.splitContainer1.Panel1.ClientSize.Width, this.splitContainer1.Panel1.ClientSize.Height, mouseRelPos);

            Rectangle r = new Rectangle(1, 1, this.splitContainer1.Panel1.ClientSize.Width - 2, this.splitContainer1.Panel1.ClientSize.Height - 2);
            Pen linePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 2);
            g.DrawRectangle(linePen, r);

            g.Flush();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014) // 禁掉清除背景消息
                return;
            base.WndProc(ref m);

        }

        private void LotteryGraph_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawCanvas(g);
        }

        private void LotteryGraph_FormClosed(object sender, FormClosedEventArgs e)
        {
            instLst.Remove(this);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            this.Invalidate(true);//触发Paint事件
        }


        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawCanvas(g);
        }

        private void comboBoxNumIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            numberIndex = comboBoxNumIndex.SelectedIndex;
            this.Invalidate(true);//触发Paint事件
        }

        private void comboBoxCollectionDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            curCDTIndex = comboBoxCollectionDataType.SelectedIndex;
            this.Invalidate(true);//触发Paint事件
        }

        private void textBoxCycleLength_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCycleLength.Text) == false)
            {
                int value = 5;
                if (int.TryParse(textBoxCycleLength.Text, out value))
                {
                    GraphDataManager.KGDC.CycleLength = value;
                    GraphDataManager.KGDC.CollectGraphData();
                    this.Invalidate(true);//触发Paint事件
                }
            }
        }

        private void splitContainer1_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseRelPos = e.Location;// this.splitContainer1.Panel1.PointToClient(e.Location);
            if (graphMgr.NeedRefreshCanvasOnMouseMove(mouseRelPos))
                this.Invalidate(true);//触发Paint事件
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.Location.X - currentPoint.X;
                currentPoint = e.Location;
                bool moveLeft = dx < 0;
                for( int i = Math.Abs(dx); i > 0; i -= 5 )
                {
                    graphMgr.MoveLeftRight(moveLeft);
                }
            }
        }

        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                currentPoint = e.Location;
        }

        private void tabControlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.SetCurrentGraph((GraphType)(tabControlView.SelectedIndex+1));
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            this.Invalidate(true);
        }

        private void comboBoxBarCollectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphDataManager.BGDC.curStatisticsType = (BarGraphDataContianer.StatisticsType)comboBoxBarCollectType.SelectedIndex;
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            this.Invalidate(true);
        }

        private void comboBoxCollectRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphDataManager.BGDC.curStatisticsRange = (BarGraphDataContianer.StatisticsRange)comboBoxCollectRange.SelectedIndex;
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            this.Invalidate(true);
        }

        private void textBoxCustomCollectRange_TextChanged(object sender, EventArgs e)
        {
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            this.Invalidate(true);
        }

        private void comboBoxAvgAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.S_AVG_ALGORITHM = (AvgAlgorithm)comboBoxAvgAlgorithm.SelectedIndex;
            NotifyOtherGraphRefreshUI(this);
            GraphDataManager.KGDC.CollectAvgDatas();
        }

        private void checkBoxAvg5_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg5.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg5.Checked;
            NotifyOtherGraphRefreshUI(this);
        }

        private void checkBoxAvg10_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg10.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg10.Checked;
            NotifyOtherGraphRefreshUI(this);
        }

        private void checkBoxAvg20_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg20.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg20.Checked;
            NotifyOtherGraphRefreshUI(this);
        }

        private void checkBoxAvg30_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg30.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg30.Checked;
            NotifyOtherGraphRefreshUI(this);
        }

        private void checkBoxAvg50_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg50.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg50.Checked;
            NotifyOtherGraphRefreshUI(this);
        }

        private void checkBoxAvg100_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg100.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg100.Checked;
            NotifyOtherGraphRefreshUI(this);
        }
    }
}
