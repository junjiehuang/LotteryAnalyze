using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    public partial class Form1 : Form
    {
        DataManager dataMgr = new DataManager();
        int curPage = -1;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "data files|*.txt";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                dataMgr.indexs.Clear();

                List<string> names = new List<string>();
                List<string> paths = new List<string>();
                int count = 0;
                for (int i = 0; i < openFileDialog.FileNames.Length; ++i)
                {
                    string[] strs = openFileDialog.FileNames[i].Split('\\');
                    string path = "";
                    for (int j = 0; j < strs.Length - 1; ++j)
                    {
                        path += strs[j] + "\\";
                    }
                    string fileName = strs[strs.Length - 1];
                    strs = fileName.Split('.');
                    paths.Add(path);
                    names.Add(strs[0]);
                    ++count;

                    int id = int.Parse(strs[0]);
                    dataMgr.indexs.Add(id);
                }

                dataMgr.indexs.Sort();
                dataMgr.LoadAllDatas(ref names, ref paths);
                curPage = 0;

                RefreshUI();
            }
        }

        void RefreshUI()
        {
            dataGridView1.Rows.Clear();
            int key = dataMgr.indexs[curPage];
            OneDayDatas data = dataMgr.allDatas[key];
            for (int i = 0; i < data.datas.Count; ++i)
            {
                DataItem di = data.datas[i];
                int andValue = Util.CalAndValue(di.lotteryNumber);
                int rearValue = Util.CalRearValue(di.lotteryNumber);
                int crossValue = Util.CalCrossValue(di.lotteryNumber);
                int groupType = Util.GetGroupType(di.lotteryNumber);
                string g6 = groupType == 3 ? "组6" : "";
                string g3 = groupType == 2 ? "组3" : "";
                string g1 = groupType == 1 ? "豹子" : "";
                object[] objs = new object[] { di.id, di.lotteryNumber, andValue, rearValue, crossValue, g6, g3, g1, };
                dataGridView1.Rows.Add( objs );
            }
        }
    }
}
