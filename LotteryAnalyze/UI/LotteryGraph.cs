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
        }

        private void LotteryGraph_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //Rectangle r = new Rectangle(0, 0, this.ClientSize.Width-1, this.ClientSize.Height-1);
            //g.DrawRectangle(Pens.Red, r);
            canvas.DrawGraph(g, curDataType, this.ClientSize.Width, this.ClientSize.Height);
        }

        private void LotteryGraph_FormClosed(object sender, FormClosedEventArgs e)
        {
            sInst = null;
        }
    }
}
