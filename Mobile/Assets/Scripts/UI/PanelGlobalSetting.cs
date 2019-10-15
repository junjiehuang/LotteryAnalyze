using LotteryAnalyze;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PanelGlobalSetting : MonoBehaviour
{
    static PanelGlobalSetting sInst;
    public static PanelGlobalSetting Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;
    }

    public class NodeWrapper
    {
        public string nodeName;
        public FieldInfo paramFieldInfo;
        public bool isExpand;
        public List<NodeWrapper> subNodes;
    }

    bool isGenerating = false;
    List<NodeWrapper> allParameterNodes = new List<NodeWrapper>();
    Dictionary<string, NodeWrapper> nodeSearchInfo = new Dictionary<string, NodeWrapper>();
    bool hasGenerateWin = false;
    Vector2 scrollPos = Vector2.zero;


    // Start is called before the first frame update
    void Start()
    {
        CollectParameterNodeInfos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        DrawUI();
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
                if (parInfos.Length > 1)
                {
                    NodeWrapper parentNode = null;
                    for (int t = 0; t < parInfos.Length; ++t)
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
                                else if (parentNode.subNodes.Contains(nodeWrap) == false)
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

    void DrawUI()
    {
        int w = Screen.width / 2 - 10;
        int h = 30;
        int lx = 10;
        int rx = w + 10;
        int y = 10;
        
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Close", GUILayout.Width(Screen.width)))
        {
            gameObject.SetActive(false);
        }
        GUILayout.EndHorizontal();

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width));
        for (int i = 0; i < allParameterNodes.Count; ++i)
        {
            NodeWrapper node = allParameterNodes[i];
            GenerateNode(node, w, h, ref lx, rx, ref y);
        }
        GUILayout.EndScrollView();
    }

    void GenerateNode(NodeWrapper node, int w, int h, ref int lx, int rx, ref int y)
    {
        if (node.paramFieldInfo != null)
        {
            FieldInfo fi = node.paramFieldInfo;

            GUILayout.BeginHorizontal();
            GUILayout.Label(node.nodeName);
            //Label lb = new Label();
            //lb.Text = node.nodeName;
            //lb.Size = new Size(w - lx, h);
            //lb.Location = new Point(lx, y);
            //toolTipHandler.SetToolTip(lb, fi.Name);
            //this.Controls.Add(lb);

            //int rightSize = w - 20;
            object v = fi.GetValue(GlobalSetting.Instance);
            if (fi.FieldType == typeof(int) ||
                fi.FieldType == typeof(float) ||
                fi.FieldType == typeof(string))
            {
                string str = GUILayout.TextField(v.ToString());
                if(str != v.ToString())
                {
                    if (fi.FieldType == typeof(int))
                    {
                        int t = 0;
                        int.TryParse(str, out t);
                        fi.SetValue(GlobalSetting.Instance, t);
                    }
                    else if (fi.FieldType == typeof(float))
                    {
                        float t = 0;
                        float.TryParse(str, out t);
                        fi.SetValue(GlobalSetting.Instance, t);
                    }
                    else if (fi.FieldType == typeof(string))
                    {
                        fi.SetValue(GlobalSetting.Instance, str);
                    }
                    GlobalSetting.SaveCfg(true);
                }
                //TextBox tb = new TextBox();
                //tb.Text = v.ToString();
                //tb.Size = new Size(rightSize, h);
                //tb.Location = new Point(rx, y - 2);
                //this.Controls.Add(tb);
                //tb.Tag = fi;
                //tb.TextChanged += Tb_TextChanged;
                //toolTipHandler.SetToolTip(tb, fi.Name);
            }
            else if (fi.FieldType == typeof(bool))
            {
                bool res = GUILayout.Toggle((bool)v, "");
                if(res != (bool)v)
                {
                    fi.SetValue(GlobalSetting.Instance, res);
                    GlobalSetting.SaveCfg(true);
                }
                //CheckBox tb = new CheckBox();
                //tb.Checked = (bool)v;
                //tb.Size = new Size(rightSize, h);
                //tb.Location = new Point(rx, y - 8);
                //this.Controls.Add(tb);
                //tb.Tag = fi;
                //tb.CheckedChanged += Tb_CheckedChanged;
                //toolTipHandler.SetToolTip(tb, fi.Name);
            }
            else if (fi.FieldType.IsEnum)
            {
                string[] names = Enum.GetNames(fi.FieldType);
                int selID = (int)v;
                int res = GUILayout.SelectionGrid(selID, names, 1);
                if(res != selID)
                {
                    fi.SetValue(GlobalSetting.Instance, res);
                    GlobalSetting.SaveCfg(true);
                }

                //ComboBox tb = new ComboBox();
                //tb.Size = new Size(rightSize, h);
                //tb.Location = new Point(rx, y - 2);
                //this.Controls.Add(tb);

                //string[] names = Enum.GetNames(fi.FieldType);
                //string txt = v.ToString();
                //tb.DataSource = names;
                //int selID = (int)v;
                //tb.SelectedIndex = selID;
                //tb.Tag = fi;
                //tb.SelectedIndexChanged += Tb_SelectedIndexChanged;
                //toolTipHandler.SetToolTip(tb, fi.Name);
            }
            y += h;

            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            if(GUILayout.Button((node.isExpand ? "- " : "+ ") + node.nodeName))
            {
                node.isExpand = !node.isExpand;
            }
            GUILayout.EndHorizontal();
            //Button btn = new Button();
            //btn.BackColor = Color.Black;
            //btn.ForeColor = Color.Yellow;
            //btn.TextAlign = ContentAlignment.MiddleLeft;
            //btn.Text = (node.isExpand ? "- " : "+ ") + node.nodeName;
            //btn.Size = new Size(this.Size.Width - lx - 20, h);
            //btn.Location = new Point(lx, y);
            //btn.Tag = node;
            //btn.Click += Tb_ExpandBtnClick;
            //this.Controls.Add(btn);
            y += h + 5;

            if (node.isExpand)
            {
                lx += 30;

                for (int t = 0; t < node.subNodes.Count; ++t)
                {
                    GenerateNode(node.subNodes[t], w, h, ref lx, rx, ref y);
                }

                lx -= 30;
            }
        }
    }
}
