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
        static GlobalSettingPanel sInst = null;

        public static void Open()
        {
            if (sInst == null)
                sInst = new GlobalSettingPanel();
            sInst.Show();
        }

        public static GlobalSettingPanel Instance
        {
            get { return sInst; }
        }

        public GlobalSettingPanel()
        {
            FormMain.AddWindow(this);

            InitializeComponent();

        }

        private void Tb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox ui = sender as ComboBox;
            FieldInfo fi = ui.Tag as FieldInfo;
            fi.SetValue(new GlobalSetting(), ui.SelectedIndex);
            GlobalSetting.SaveCfg(true);
        }

        private void Tb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ui = sender as CheckBox;
            FieldInfo fi = ui.Tag as FieldInfo;
            fi.SetValue(new GlobalSetting(), ui.Checked);
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
                fi.SetValue(new GlobalSetting(), t);
            }
            else if (fi.FieldType == typeof(float))
            {
                float t = 0;
                float.TryParse(ui.Text, out t);
                fi.SetValue(new GlobalSetting(), t);
            }
            else if (fi.FieldType == typeof(string))
            {
                fi.SetValue(new GlobalSetting(), ui.Text);
            }
            GlobalSetting.SaveCfg(true);
        }

        private void GlobalSettingPanel_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMain.RemoveWindow(this);
            sInst = null;
        }

        private void GlobalSettingPanel_Load(object sender, EventArgs e)
        {

            this.AutoScroll = true;
            //this.HScroll = true;
            this.VScroll = true;
            int w = this.Size.Width / 2 - 10;
            int h = 30;
            int lx = 10;
            int rx = w + 10;
            int y = 10;
            FieldInfo[] fis = typeof(GlobalSetting).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            for (int i = 0; i < fis.Length; ++i)
            {
                FieldInfo fi = fis[i];
                Parameter par = Attribute.GetCustomAttribute(fi, typeof(Parameter)) as Parameter;
                if (par != null)
                {
                    Label lb = new Label();
                    lb.Text = par.name;
                    lb.Size = new Size(w, h);
                    lb.Location = new Point(lx, y);
                    this.Controls.Add(lb);

                    object v = fi.GetValue(new GlobalSetting());
                    if (fi.FieldType == typeof(int) ||
                        fi.FieldType == typeof(float) ||
                        fi.FieldType == typeof(string))
                    {
                        TextBox tb = new TextBox();
                        tb.Text = v.ToString();
                        tb.Size = new Size(w, h);
                        tb.Location = new Point(rx, y-2);
                        this.Controls.Add(tb);
                        tb.Tag = fi;
                        tb.TextChanged += Tb_TextChanged;
                    }
                    else if (fi.FieldType == typeof(bool))
                    {
                        CheckBox tb = new CheckBox();
                        tb.Checked = (bool)v;
                        tb.Size = new Size(w, h);
                        tb.Location = new Point(rx, y-8);
                        this.Controls.Add(tb);
                        tb.Tag = fi;
                        tb.CheckedChanged += Tb_CheckedChanged;
                    }
                    else if (fi.FieldType.IsEnum)
                    {
                        ComboBox tb = new ComboBox();
                        tb.Size = new Size(w, h);
                        tb.Location = new Point(rx, y-2);
                        this.Controls.Add(tb);

                        string[] names = Enum.GetNames(fi.FieldType);
                        string txt = v.ToString();
                        tb.DataSource = names;
                        int selID = (int)v;
                        tb.SelectedIndex = selID;
                        tb.Tag = fi;
                        tb.SelectedIndexChanged += Tb_SelectedIndexChanged;
                    }

                    y += h;
                }
            }
        }
    }
}
