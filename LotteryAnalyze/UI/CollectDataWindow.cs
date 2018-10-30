using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using static LotteryAnalyze.AutoUpdateUtil;

namespace LotteryAnalyze.UI
{
    public partial class CollectDataWindow : Form
    {
        System.Windows.Forms.Timer updateTimer;
        List<DateTime> jobLst = new List<DateTime>();
        List<DateTime> jobUnFinishLst = new List<DateTime>();
        int curJobIndex = -1;

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

            List<string> dataArr = new List<string>();
            for(DataSourceType i = DataSourceType.e360; i < DataSourceType.eMax; ++i)
            {
                dataArr.Add(i.ToString());
            }
            comboBoxDataSource.DataSource = dataArr;
            comboBoxDataSource.SelectedIndex = (int)GlobalSetting.G_DATA_SOURCE_TYPE;

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 30;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Enabled = true;
            updateTimer.Start();

            FormMain.AddWindow(this);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (jobLst.Count == 0 || curJobIndex == -1)
                return;
            DateTime date = jobLst[curJobIndex];
            ++curJobIndex;
            progressBarCollectDatas.Value = curJobIndex * 100 / jobLst.Count;
            string error = "";
            int lotteryCount = AutoUpdateUtil.FetchData(date, ref error);
            if (lotteryCount < 120)
            {
                jobUnFinishLst.Add(date);
                if (string.IsNullOrEmpty(error))
                    textBoxCmd.Text = date.ToString() + "--------------> " + lotteryCount + "\r\n" + textBoxCmd.Text;
                else
                    textBoxCmd.Text = date.ToString() + "--------------> " + error + "\r\n" + textBoxCmd.Text;
            }
            else
                textBoxCmd.Text = date.ToString() + "\r\n" + textBoxCmd.Text;
            textBoxCmd.ScrollToCaret();
            if (jobLst.Count == curJobIndex)
            {
                textBoxCmd.Text += "收集完毕!";
                progressBarCollectDatas.Value = 100;
                jobLst.Clear();
                curJobIndex = -1;
                MessageBox.Show("收集完毕!");
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
            if (jobLst.Count > 0)
                curJobIndex = 0;
            progressBarCollectDatas.Value = 0;
            jobUnFinishLst.Clear();
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
            FormMain.RemoveWindow(this);
            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer = null;
            }
        }

        private void CollectDataWindow_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonReFetchFailedDatas_Click(object sender, EventArgs e)
        {
            jobLst.Clear();
            jobLst.AddRange(jobUnFinishLst);
            if (jobLst.Count > 0)
                curJobIndex = 0;
            progressBarCollectDatas.Value = 0;
            jobUnFinishLst.Clear();
            textBoxCmd.Text = "";
        }

        private void comboBoxDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            GlobalSetting.G_DATA_SOURCE_TYPE = (DataSourceType)comboBoxDataSource.SelectedIndex;
        }
    }
}
