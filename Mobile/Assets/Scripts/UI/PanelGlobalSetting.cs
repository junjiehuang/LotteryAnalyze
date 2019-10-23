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

    GUIStyle labelStyle = null;
    GUIStyle textFieldStyle = null;
    GUIStyle toggleStyle = null;
    GUIStyle selectionGridStyle = null;
    GUIStyle buttonStyle = null;
    GUIStyle scrollViewStyle = null;
    GUIStyle tabBtnStyle = null;

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
        int WIDTH = Screen.width - 50;
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 30;

            textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.fontSize = 30;

            toggleStyle = new GUIStyle(GUI.skin.toggle);
            toggleStyle.fontSize = 30;

            selectionGridStyle = new GUIStyle(GUI.skin.button);
            selectionGridStyle.fontSize = 30;

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 30;

            tabBtnStyle = new GUIStyle(GUI.skin.button);
            tabBtnStyle.fontSize = 32;
            tabBtnStyle.alignment = TextAnchor.MiddleLeft;
            tabBtnStyle.normal.textColor = Color.green;
            tabBtnStyle.hover.textColor = Color.yellow;

            scrollViewStyle = new GUIStyle(GUI.skin.scrollView);
            scrollViewStyle.fontSize = 30;
        }


        int w = WIDTH / 2 - 10;
        int h = 30;
        int lx = 10;
        int rx = w + 10;
        int y = 10;

        GUILayout.BeginVertical();
        GUILayout.Space(20);
        GUILayout.EndVertical();
        
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("关闭", buttonStyle, GUILayout.Width(WIDTH)))
        {
            gameObject.SetActive(false);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Space(20);
        GUILayout.EndVertical();

        scrollPos = GUILayout.BeginScrollView(scrollPos, scrollViewStyle, GUILayout.Width(WIDTH));
        for (int i = 0; i < allParameterNodes.Count; ++i)
        {
            NodeWrapper node = allParameterNodes[i];
            GenerateNode(node, w, h, ref lx, rx, ref y);
        }
        GUILayout.EndScrollView();
    }

    void GenerateNode(NodeWrapper node, int w, int h, ref int lx, int rx, ref int y)
    {
        float halfScreenWidth = Screen.width * 0.5f;
        if (node.paramFieldInfo != null)
        {
            FieldInfo fi = node.paramFieldInfo;

            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            GUILayout.Label(node.nodeName, labelStyle, GUILayout.Width(halfScreenWidth - 50));

            object v = fi.GetValue(GlobalSetting.Instance);
            if (fi.FieldType == typeof(int) ||
                fi.FieldType == typeof(float) ||
                fi.FieldType == typeof(string))
            {
                string str = GUILayout.TextField(v.ToString(), textFieldStyle);
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
            }
            else if (fi.FieldType == typeof(bool))
            {
                bool res = GUILayout.Toggle((bool)v, "", toggleStyle);
                if(res != (bool)v)
                {
                    fi.SetValue(GlobalSetting.Instance, res);
                    GlobalSetting.SaveCfg(true);
                }
            }
            else if (fi.FieldType.IsEnum)
            {
                string[] names = Enum.GetNames(fi.FieldType);
                int selID = (int)v;
                int res = GUILayout.SelectionGrid(selID, names, 1, selectionGridStyle);
                if(res != selID)
                {
                    fi.SetValue(GlobalSetting.Instance, res);
                    GlobalSetting.SaveCfg(true);
                }
            }
            y += h;

            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            if(GUILayout.Button((node.isExpand ? "- " : "+ ") + node.nodeName, tabBtnStyle))
            {
                node.isExpand = !node.isExpand;
            }
            GUILayout.EndHorizontal();
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
