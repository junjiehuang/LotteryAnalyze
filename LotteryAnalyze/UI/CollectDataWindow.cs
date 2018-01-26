using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace LotteryAnalyze.UI
{
    public partial class CollectDataWindow : Form
    {
        System.Windows.Forms.Timer updateTimer;
        List<DateTime> jobLst = new List<DateTime>();

        public CollectDataWindow()
        {
            InitializeComponent();

            DateTime curDate = DateTime.Now;
            textBoxEndY.Text = curDate.Year.ToString();
            textBoxStartY.Text = curDate.Year.ToString();
            textBoxEndM.Text = curDate.Month.ToString();
            textBoxStartM.Text = curDate.Month.ToString();
            textBoxEndD.Text = curDate.Day.ToString();
            textBoxStartD.Text = curDate.Day.ToString();

            dateTimePickerStartDate.Value = curDate;
            dateTimePickerEndDate.Value = curDate;

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 500;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Enabled = true;
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (jobLst.Count == 0)
                return;
            DateTime date = jobLst[0];
            jobLst.RemoveAt(0);
            AutoUpdateUtil.FetchData(date);
            textBoxCmd.Text += date.ToString() + "\r\n";
            if (jobLst.Count == 0)
            {
                textBoxCmd.Text += "Exec Completed!";
                MessageBox.Show("Collect Completed!");
            }
        }

        private void buttonCollect_Click(object sender, EventArgs e)
        {
            int sy, sm, sd, ey, em, ed;
            sy = int.Parse(textBoxStartY.Text);
            sm = int.Parse(textBoxStartM.Text);
            sd = int.Parse(textBoxStartD.Text);
            ey = int.Parse(textBoxEndY.Text);
            em = int.Parse(textBoxEndM.Text);
            ed = int.Parse(textBoxEndD.Text);

            textBoxCmd.Text = "";
            jobLst.Clear();
            DateTime startDate = new DateTime(sy, sm, sd);
            DateTime endDate = new DateTime(ey, em, ed);
            if (startDate == endDate)
                jobLst.Add(startDate);
            else
            {
                int diff = DateTime.Compare(startDate, endDate);
                DateTime curDate = diff < 0 ? startDate : endDate;
                while (DateTime.Compare(curDate, endDate) < 1)
                {
                    jobLst.Add(curDate);
                    curDate = curDate.AddDays(1);
                }
            }
        }

        private void dateTimePickerStartDate_ValueChanged(object sender, EventArgs e)
        {
            textBoxStartY.Text = dateTimePickerStartDate.Value.Year.ToString();
            textBoxStartM.Text = dateTimePickerStartDate.Value.Month.ToString();
            textBoxStartD.Text = dateTimePickerStartDate.Value.Day.ToString();
        }

        private void dateTimePickerEndDate_ValueChanged(object sender, EventArgs e)
        {
            textBoxEndY.Text = dateTimePickerEndDate.Value.Year.ToString();
            textBoxEndM.Text = dateTimePickerEndDate.Value.Month.ToString();
            textBoxEndD.Text = dateTimePickerEndDate.Value.Day.ToString();
        }

        private void CollectDataWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void CollectDataWindow_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
