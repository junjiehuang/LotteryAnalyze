using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

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

        private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "data files|*.txt";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            openFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; ++i)
                {
                    string[] strs = openFileDialog.FileNames[i].Split('\\');
                    string fileName = strs[strs.Length - 1];
                    strs = fileName.Split('.');

                    int id = int.Parse(strs[0]);
                    if (dataMgr.mFileMetaInfo.ContainsKey(id) == false)
                    {
                        dataMgr.mFileMetaInfo.Add(id, openFileDialog.FileNames[i]);
                    }
                }

                RefreshFileList();
            }
        }

        void RefreshFileList()
        {
            listViewFileList.Items.Clear();
            foreach (int key in dataMgr.mFileMetaInfo.Keys)
            {
                ListViewItem item = new ListViewItem();
                item.Name = key.ToString();
                item.Text = key.ToString();
                item.Tag = key;
                listViewFileList.Items.Add(item);
            }
        }

        void RefreshDataView()
        {
            dataGridViewLotteryDatas.Rows.Clear();
            foreach( int key in dataMgr.allDatas.Keys )
            {
                OneDayDatas data = dataMgr.allDatas[key];
                for (int i = 0; i < data.datas.Count; ++i)
                {
                    DataItem di = data.datas[i];
                    string g6 = di.groupType == 3 ? "组6" : "";
                    string g3 = di.groupType == 2 ? "组3" : "";
                    string g1 = di.groupType == 1 ? "豹子" : "";
                    object[] objs = new object[] { di.idTag, di.lotteryNumber, di.andValue, di.rearValue, di.crossValue, g6, g3, g1, };
                    dataGridViewLotteryDatas.Rows.Add(objs);
                }
            }
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = "";
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.SelectedPath = System.Environment.CurrentDirectory;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog.SelectedPath;

                DirectoryInfo di = new DirectoryInfo(path);
                LoopSearchFolder(di);
                RefreshFileList();
            }
        }

        void LoopSearchFolder(DirectoryInfo parentDirInfo)
        {
            FileInfo[] files = parentDirInfo.GetFiles();
            DirectoryInfo[] dirs = parentDirInfo.GetDirectories();
            for (int i = 0; i < files.Length; ++i)
            {
                string[] strs = files[i].FullName.Split('\\');
                string fileName = strs[strs.Length - 1];
                strs = fileName.Split('.');

                int id = int.Parse(strs[0]);
                if (dataMgr.mFileMetaInfo.ContainsKey(id) == false)
                {
                    dataMgr.mFileMetaInfo.Add(id, files[i].FullName);
                }
            }
            for (int i = 0; i < dirs.Length; ++i)
            {
                LoopSearchFolder(dirs[i]);
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataMgr.mFileMetaInfo.Clear();
            dataMgr.indexs.Clear();
            dataMgr.allDatas.Clear();
        }

        private void addToSimulatePoolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataMgr.ClearAllDatas();
            for (int i = 0; i < listViewFileList.SelectedItems.Count; ++i)
            {
                ListViewItem item = listViewFileList.SelectedItems[i];
                int key = (int)(item.Tag);
                dataMgr.LoadData(key);
            }
            RefreshDataView();
        }
    }
}
