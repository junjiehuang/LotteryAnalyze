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
        static LotteryGraph sInst = null;

        Graph canvas = new Graph();
        int numberIndex = 0;        
        int curCDTIndex = 0;
        Point currentPoint = new Point();


        public static void Open()
        {
            if (sInst == null)
                sInst = new LotteryGraph();
            sInst.Show();
        }

        public static void ShutDown()
        {
            if (sInst != null)
                sInst.Close();
            sInst = null;
        }

        public LotteryGraph()
        {
            InitializeComponent();

            IList<string> list = new List<string>();
            for (int i = 0; i < GraphDataManager.S_CDTSTRS.Length; ++i)
                list.Add(GraphDataManager.S_CDTSTRS[i]);
            comboBoxCollectionDataType.DataSource = list;
            comboBoxCollectionDataType.SelectedIndex = curCDTIndex;

            comboBoxNumIndex.SelectedIndex = numberIndex;            
            textBoxCycleLength.Text = GraphDataManager.Instance.CycleLength.ToString();
        }


        void DrawCanvas(Graphics g)
        {
            g.Clear(Color.Black);

            CollectDataType cdt = GraphDataManager.S_CDTS[curCDTIndex];
            canvas.DrawGraph(g, numberIndex, cdt, this.ClientSize.Width, this.ClientSize.Height);

            Rectangle r = new Rectangle(0, 0, this.splitContainer1.Panel1.ClientSize.Width - 1, this.splitContainer1.Panel1.ClientSize.Height - 1);
            Pen linePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 2);
            g.DrawRectangle(linePen, r);

            g.Flush();
        }

        private void LotteryGraph_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawCanvas(g);
        }

        private void LotteryGraph_FormClosed(object sender, FormClosedEventArgs e)
        {
            sInst = null;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphDataManager.Instance.CollectGraphData();
            this.Refresh();
        }


        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawCanvas(g);
        }

        private void comboBoxNumIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            numberIndex = comboBoxNumIndex.SelectedIndex;
            this.Refresh();
        }

        private void comboBoxCollectionDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            curCDTIndex = comboBoxCollectionDataType.SelectedIndex;
            this.Refresh();
        }

        private void textBoxCycleLength_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCycleLength.Text) == false)
            {
                GraphDataManager.Instance.CycleLength = int.Parse(textBoxCycleLength.Text);
            }
        }

        private void splitContainer1_Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.Location.X - currentPoint.X;
                currentPoint = e.Location;
                bool moveLeft = dx < 0;
                bool moveSuccess = false;
                for( int i = Math.Abs(dx); i > 0; i -= 5 )
                {
                    if (canvas.MoveLeftRight(moveLeft))
                        moveSuccess = true;
                }
                if(moveSuccess)
                    this.Refresh();
            }
        }

        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                currentPoint = e.Location;
        }
    }
}
