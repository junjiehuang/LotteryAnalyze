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
    public partial class GlobalSimTradeWindow : Form
    {
        static GlobalSimTradeWindow sInst;

        public GlobalSimTradeWindow()
        {
            InitializeComponent();
        }

        public static void Open()
        {
            if (sInst == null)
                sInst = new GlobalSimTradeWindow();
            sInst.Show();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {

        }

        private void buttonPauseResume_Click(object sender, EventArgs e)
        {

        }

        private void buttonStop_Click(object sender, EventArgs e)
        {

        }

        private void GlobalSimTradeWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            sInst = null;
        }
    }
}
