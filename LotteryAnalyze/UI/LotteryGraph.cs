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
        int cycleCount = 5;
        CollectDataType curDataType = CollectDataType.ePath0;


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

            comboBoxNumIndex.SelectedIndex = numberIndex + 1;
            comboBoxCollectionDataType.SelectedIndex = (int)curDataType;
        }


        void DrawCanvas(Graphics g)
        {
            g.Clear(Color.Black);

            Rectangle r = new Rectangle(0, 0, this.splitContainer1.Panel1.ClientSize.Width - 1, this.splitContainer1.Panel1.ClientSize.Height - 1);

            //Pen solidPen = GraphUtil.GetSolidPen(Color.Black);
            //g.DrawRectangle(solidPen, r);
            Pen linePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 2);
            g.DrawRectangle(linePen, r);
            canvas.DrawGraph(g, curDataType, this.ClientSize.Width, this.ClientSize.Height);

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
            canvas.CollectKDatas(numberIndex, curDataType, cycleCount);
            this.Update();
        }


        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawCanvas(g);
        }

        private void comboBoxNumIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            numberIndex = comboBoxNumIndex.SelectedIndex - 1;
        }

        private void comboBoxCollectionDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            curDataType = (CollectDataType)comboBoxCollectionDataType.SelectedIndex;
        }
    }
}
