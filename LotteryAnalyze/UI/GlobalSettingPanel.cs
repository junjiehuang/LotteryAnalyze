using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace LotteryAnalyze.UI
{


    public partial class GlobalSettingPanel : Form
    {
        public class NodeWrapper
        {
            public string nodeName;
            public FieldInfo paramFieldInfo;
            public bool isExpand;
            public List<NodeWrapper> subNodes;
        }

        static GlobalSettingPanel sInst = null;
        bool isGenerating = false;
        Size lastSize = new Size();
        List<NodeWrapper> allParameterNodes = new List<NodeWrapper>();
        Dictionary<string, NodeWrapper> nodeSearchInfo = new Dictionary<string, NodeWrapper>();
        bool hasGenerateWin = false;

        public static void Open()
        {
            try
            {
                if (sInst == null)
                    sInst = new GlobalSettingPanel();
                sInst.Show();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static GlobalSettingPanel Instance
        {
            get { return sInst; }
        }

        public GlobalSettingPanel()
        {
            FormMain.AddWindow(this);

            InitializeComponent();

            //采用双缓冲技术的控件必需的设置
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        private void Tb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox ui = sender as ComboBox;
            FieldInfo fi = ui.Tag as FieldInfo;
            fi.SetValue(GlobalSetting.Instance, ui.SelectedIndex);
            GlobalSetting.SaveCfg(true);
        }

        private void Tb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ui = sender as CheckBox;
            FieldInfo fi = ui.Tag as FieldInfo;
            fi.SetValue(GlobalSetting.Instance, ui.Checked);
            GlobalSetting.SaveCfg(true);
        }

        private void Tb_TextChanged(object sender, EventArgs e)
        {
            TextBox ui = sender as TextBox;
            FieldInfo fi = ui.Tag as FieldInfo;
            if (fi.FieldType == typeof(int))
            {
                int t = 0;
                int.TryParse(ui.Text, out t);
                fi.SetValue(GlobalSetting.Instance, t);
            }
            else if (fi.FieldType == typeof(float))
            {
                float t = 0;
                float.TryParse(ui.Text, out t);
                fi.SetValue(GlobalSetting.Instance, t);
            }
            else if (fi.FieldType == typeof(string))
            {
                fi.SetValue(GlobalSetting.Instance, ui.Text);
            }
            GlobalSetting.SaveCfg(true);
        }

        private void Tb_ExpandBtnClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            NodeWrapper node = btn.Tag as NodeWrapper;
            node.isExpand = !node.isExpand;
            GenerateControls(true);
        }

        private void GlobalSettingPanel_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMain.RemoveWindow(this);
            sInst = null;
        }

        private void GlobalSettingPanel_Load(object sender, EventArgs e)
        {
            CollectParameterNodeInfos();
            GenerateControls();
            lastSize = this.Size;
            hasGenerateWin = true;
        }

        void CollectParameterNodeInfos()
        {
            allParameterNodes.Clear();
            nodeSearchInfo.Clear();

            FieldInfo[] fis = typeof(GlobalSetting).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            for (int i = 0; i < fis.Length; ++i)
            {
                FieldInfo fi = fis[i];
                Parameter par = Attribute.GetCustomAttribute(fi, typeof(Parameter)) as Parameter;
                if (par != null)
                {
                    string[] parInfos = par.name.Split('/');
                    if(parInfos.Length > 1)
                    {
                        NodeWrapper parentNode = null;
                        for(int t = 0; t < parInfos.Length; ++t)
                        {
                            string nodeName = parInfos[t];
                            NodeWrapper nodeWrap = null;
                            if (nodeSearchInfo.ContainsKey(nodeName) == false)
                            {
                                nodeWrap = new NodeWrapper();
                                nodeWrap.nodeName = nodeName;
                                nodeWrap.isExpand = true;

                                if (t < parInfos.Length - 1)
                                {
                                    nodeWrap.subNodes = new List<NodeWrapper>();
                                    nodeSearchInfo[nodeName] = nodeWrap;
                                    if (parentNode == null)
                                        allParameterNodes.Add(nodeWrap);
                                    else if(parentNode.subNodes.Contains(nodeWrap) == false)
                                        parentNode.subNodes.Add(nodeWrap);
                                }
                            }
                            else
                            {
                                nodeWrap = nodeSearchInfo[nodeName];
                            }

                            if (t < parInfos.Length - 1)
                            {
                                parentNode = nodeWrap;
                            }
                            else
                            {
                                parentNode.subNodes.Add(nodeWrap);
                                nodeWrap.paramFieldInfo = fi;
                            }
                        }
                    }
                    else
                    {
                        NodeWrapper nodeWrap = new NodeWrapper();
                        nodeWrap.nodeName = par.name;
                        nodeWrap.isExpand = true;
                        nodeWrap.paramFieldInfo = fi;
                        nodeSearchInfo[par.name] = nodeWrap;
                        allParameterNodes.Add(nodeWrap);
                    }
                }
            }
        }
       

        void GenerateControls(bool forceGenerate = false)
        {
            if (isGenerating)
                return;
            if (forceGenerate == false &&
                lastSize.Width == this.Size.Width && 
                lastSize.Height == this.Size.Height)
                return;

            //this.SuspendLayout();
            isGenerating = true;
            for(int i = 0; i < this.Controls.Count; ++i)
            {
                this.Controls[i].Dispose();
            }
            this.Controls.Clear();
            this.AutoScroll = true;
            this.VScroll = true;
            int w = this.Size.Width / 2 - 10;
            int h = 30;
            int lx = 10;
            int rx = w + 10;
            int y = 10;
            for(int i = 0; i < allParameterNodes.Count; ++i)
            {
                NodeWrapper node = allParameterNodes[i];
                GenerateNode(node, w, h, ref lx, rx, ref y);
            }
            isGenerating = false;
            lastSize = this.Size;

            //this.ResumeLayout(false);
            //this.PerformLayout();
        }

        void GenerateNode(NodeWrapper node, int w, int h, ref int lx, int rx, ref int y)
        {
            if(node.paramFieldInfo != null)
            {
                FieldInfo fi = node.paramFieldInfo;
                Label lb = new Label();
                lb.Text = node.nodeName;
                lb.Size = new Size(w - lx, h);
                lb.Location = new Point(lx, y);
                toolTipHandler.SetToolTip(lb, fi.Name);
                this.Controls.Add(lb);

                int rightSize = w - 20;
                object v = fi.GetValue( GlobalSetting.Instance );
                if (fi.FieldType == typeof(int) ||
                    fi.FieldType == typeof(float) ||
                    fi.FieldType == typeof(string))
                {
                    TextBox tb = new TextBox();
                    tb.Text = v.ToString();
                    tb.Size = new Size(rightSize, h);
                    tb.Location = new Point(rx, y - 2);
                    this.Controls.Add(tb);
                    tb.Tag = fi;
                    tb.TextChanged += Tb_TextChanged;
                    toolTipHandler.SetToolTip(tb, fi.Name);
                }
                else if (fi.FieldType == typeof(bool))
                {
                    CheckBox tb = new CheckBox();
                    tb.Checked = (bool)v;
                    tb.Size = new Size(rightSize, h);
                    tb.Location = new Point(rx, y - 8);
                    this.Controls.Add(tb);
                    tb.Tag = fi;
                    tb.CheckedChanged += Tb_CheckedChanged;
                    toolTipHandler.SetToolTip(tb, fi.Name);
                }
                else if (fi.FieldType.IsEnum)
                {
                    ComboBox tb = new ComboBox();
                    tb.Size = new Size(rightSize, h);
                    tb.Location = new Point(rx, y - 2);
                    this.Controls.Add(tb);

                    string[] names = Enum.GetNames(fi.FieldType);
                    string txt = v.ToString();
                    tb.DataSource = names;
                    int selID = (int)v;
                    tb.SelectedIndex = selID;
                    tb.Tag = fi;
                    tb.SelectedIndexChanged += Tb_SelectedIndexChanged;
                    toolTipHandler.SetToolTip(tb, fi.Name);
                }
                y += h;
            }
            else
            {
                Button btn = new Button();
                btn.BackColor = Color.Black;
                btn.ForeColor = Color.Yellow;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Text = (node.isExpand ? "- " : "+ ") + node.nodeName;
                btn.Size = new Size(this.Size.Width - lx - 20, h);
                btn.Location = new Point(lx, y);
                btn.Tag = node;
                btn.Click += Tb_ExpandBtnClick;
                this.Controls.Add(btn);
                y += h + 5;

                if(node.isExpand)
                {
                    lx += 30;

                    for(int t = 0; t < node.subNodes.Count; ++t)
                    {
                        GenerateNode(node.subNodes[t], w, h, ref lx, rx, ref y);
                    }

                    lx -= 30;
                }
            }
        }

        private void GlobalSettingPanel_ResizeEnd(object sender, EventArgs e)
        {
            GenerateControls();
        }


        
        #region do window maxmise
        const int WM_SYSCOMMAND = 0x112;
        const int SC_CLOSE = 0xF060;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (hasGenerateWin == false)
                return;

            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    if (m.WParam.ToInt32() == SC_MAXIMIZE)
                    {
                        GenerateControls();
                        return;
                    }
                    break;
            }
        }
        #endregion
        

    }
}
