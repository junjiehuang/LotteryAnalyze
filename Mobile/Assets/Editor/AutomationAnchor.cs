#define StepUse
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text.RegularExpressions;

public class AutomationAnchor : Editor
{
    #region 路径获取
    [MenuItem("UITool/指定目标路径,并且打印出来")]
    public static void GetUIPath()
    {
        string path = GetParentName(Selection.transforms[0]);
        string[] pathArray = path.Split(';');
        path = "";
        for (int i = pathArray.Length - 3; i >= 0; i--)
        {
            path = path + pathArray[i] + "/";
        }
        path = path + Selection.transforms[0].name;
        Debug.Log(path);
    }

    private static string GetParentName(Transform obj, string rootName = "Canvas")
    {
        string a = "";
        string parentName = obj.parent.name;
        if (parentName != rootName)
        {
            a = parentName + ";" + GetParentName(obj.parent);
        }
        else
        {
            a = rootName;
        }
        return a;
    }

    #endregion


    #region 自动锚定
    [MenuItem("UITool/自动锚定（从自身遍历查询所有节点进行锚定）")]
    public static void DoAutomationAnchor()
    {
        Transform[] transforms = Selection.transforms;
        for (int i = 0; i < transforms.Length; i++)
        {
            UIAutomationAnchor(transforms[i]);

            ChildrenAutomationAnchor(transforms[i]);
        }
    }

    [MenuItem("UITool/自动锚定（自身进行锚定）")]
    public static void SingleDoAutomationAnchor()
    {
        Transform[] transforms = Selection.transforms;

        for (int i = 0; i < transforms.Length; i++)
        {
            UIAutomationAnchor(transforms[i]);
        }
    }

    /// <summary>
    /// 寻找所有的子物体,并且自动锚定
    /// </summary>
    /// <param name="obj"></param>
    private static void ChildrenAutomationAnchor(Transform obj)
    {
        int childCount = obj.childCount;


        for (int i = 0; i < childCount; i++)
        {
            Transform temp = obj.GetChild(i);

            UIAutomationAnchor(temp);

            if (!IsHaveSpecialComponent(temp))
            {
                ChildrenAutomationAnchor(temp);
            }
        }
    }

    /// <summary>
    /// 是否包含特殊的组件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static bool IsHaveSpecialComponent(Transform obj)
    {
        if (obj.GetComponent<ScrollRect>()!= null)
        {
            return true;
        }

        if (obj.GetComponent<Dropdown>() != null)
        {
            return true;
        }

        if (obj.GetComponent<LayoutGroup>() != null)
        {
            return true;
        }
        if (obj.GetComponent<Slider>() != null)
        {
            return true;
        }
        if (obj.GetComponent<Scrollbar>() != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 记录当前的坐标位置,把锚点还原
    /// </summary>
    /// <param name="rec"></param>
    private static void ResetAnchorPosition(RectTransform rec)
    {
        if (rec == null)
            return;
        float width = rec.rect.width;
        float height = rec.rect.height;
        Vector3 cueerntPosition = rec.transform.position;
        rec.anchorMin = new Vector2(0.5f, 0.5f);
        rec.anchorMax = new Vector2(0.5f, 0.5f);
        rec.position = cueerntPosition;
        rec.sizeDelta = new Vector2(width, height);
    }

    /// <summary>
    /// 获父物体上的RectTransform,注意判空
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static RectTransform GetParentUI(Transform obj)
    {
        Transform parentUI = obj.transform.parent;
        if (parentUI)
            return parentUI.transform.GetComponent<RectTransform>();
        return null;
    }

    /// <summary>
    /// 获取自身上的RectTransform
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static RectTransform GetUIRectTransform(Transform obj)
    {
        if (obj)
            return obj.transform.GetComponent<RectTransform>();
        return null;
    }

    /// <summary>
    /// UI的自动锚定的具体使用
    /// </summary>
    /// <param name="obj"></param>
    private static void UIAutomationAnchor(Transform obj)
    {
        RectTransform selfRectTransform = GetUIRectTransform(obj);
        RectTransform parentRectTransform = GetParentUI(obj);
        if (selfRectTransform == null || parentRectTransform == null)
            return;
        //父物体的宽高
        float parentWidth = parentRectTransform.rect.width;
        float parentHeight = parentRectTransform.rect.height;

        //自身UI的宽高
        float selfWidth = selfRectTransform.rect.width;
        float selfHeight = selfRectTransform.rect.height;
        ResetAnchorPosition(selfRectTransform);
        //计算四个角的坐标
        Vector3 leftLower = new Vector3(selfRectTransform.anchoredPosition3D.x - selfRectTransform.pivot.x * selfWidth,
                                selfRectTransform.anchoredPosition3D.y - selfRectTransform.pivot.y * selfHeight,
                                selfRectTransform.anchoredPosition3D.z);
        Vector3 leftUp = new Vector3(leftLower.x, leftLower.y + selfHeight, leftLower.z);
        Vector3 rightLower = new Vector3(leftLower.x + selfWidth, leftLower.y, leftLower.z);
        Vector3 rightUP = new Vector3(leftLower.x + selfWidth, leftLower.y + selfHeight, leftLower.z);
        float min_x = 0;
        float max_x = 0;
        float min_y = 0;
        float max_y = 0;
        List<float> xyList = new List<float>() { min_x, max_x, min_y, max_y };
        //计算ui的min和max
        min_x = (leftLower.x >= 0) ? (leftLower.x + parentWidth / 2) / parentWidth : ((parentWidth / 2 + leftLower.x) / parentWidth);
        max_x = (rightLower.x >= 0) ? (rightLower.x + parentWidth / 2) / parentWidth : ((parentWidth / 2 + rightLower.x) / parentWidth);
        min_y = (rightLower.y >= 0) ? (rightLower.y + parentHeight / 2) / parentHeight : ((parentHeight / 2 + rightLower.y) / parentHeight);
        max_y = (rightUP.y >= 0) ? (rightUP.y + parentHeight / 2) / parentHeight : ((parentHeight / 2 + rightUP.y) / parentHeight);
        //设置ui的min和max
        selfRectTransform.anchorMin = new Vector2(min_x, min_y);
        selfRectTransform.anchorMax = new Vector2(max_x, max_y);

        //重置ui的偏移,保证UI大小不变
        selfRectTransform.offsetMin = Vector2.zero;
        selfRectTransform.offsetMax = Vector2.zero;

    }

    #endregion

    #region 字体
    [MenuItem("UITool/检查msyh,Arial的斜体和加粗修改成正常字体")]
    public static void AutomationChangeFont()
    {
        string[] fullPathArray = //new string[1] { "Assets/Test" };

            new string[] 
            {
                "Assets/AssetBundle/UI/Prefabs",
                "Assets/AssetBundle/UI/StdPrefab",
                "Assets/AssetBundle/FX_Effects/FX_Prefab",
                "Assets/AssetBundle/Levels/SceneItems/Prefabs",
                "Assets/AssetBundle/race/Prefabs",
                "Assets/Resources/Prefabs",
            };


        for (int i = 0; i < fullPathArray.Length; i++)
        {
            CheckFStyle(fullPathArray[i]);

            CheckFont(fullPathArray[i]);
        }
        Debug.LogError("修改字体完毕");
    }

    //改变字体的格式（Arial）
    private static void CheckFont(string fullPath)
    {
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                string path = files[i].FullName;
                int idx = path.IndexOf("Assets");
                path = path.Remove(0, idx);
                /*1AssetImporter的信息*/
                AssetImporter importer = AssetImporter.GetAtPath(path);
                /*2资源类型名字*/
                Type assetsType = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (assetsType.ToString() == "UnityEngine.GameObject")
                {
                    //Debug.LogError("有");
                    GameObject obj = AssetDatabase.LoadAssetAtPath(path, assetsType) as GameObject;
                    Text[] allText = obj.GetComponentsInChildren<Text>(true);
                    bool needLoading = false;
                    for (int j = 0; j < allText.Length; j++)
                    {
                        if (allText[j].font == null)
                            continue;

                        if (allText[j].font.name == "Arial"
                            || allText[j].font.name == "EurostileNextLTPro-Regular_0"
                            )
                        {
                           Debug.Log(obj.name + ";" + allText[j].name);
                           needLoading = true;
                        }
                    }
                    if (needLoading)
                    {
                        string text = File.ReadAllText(files[i].FullName);

                        var fileID = "12800000";

                        var guid = "e0a5fecf3a3d1144893ef035a6b2b9d3";

                        var type = 3;

                        var pattern = "m_Font: {fileID: [0-9]+, guid: [0-9a-z]{32}, type: [0-9]+}";

                        var replacement = "m_Font: {fileID: " + fileID + ", guid: " + guid + ", type: " + type + "}";

                        var contents = Regex.Replace(text, pattern, replacement);

                        File.WriteAllText(path, contents);

                        ////字体风格
                        //for (int k = 1; k < 3; k++)
                        //{
                        //    string key = "m_FontStyle: " + k;
                        //
                        //    if (contents.Contains(key))
                        //    {
                        //        contents = contents.Replace(key, "m_FontStyle: 0");
                        //
                        //        FileStream fs = new FileStream(files[i].FullName, FileMode.Open);
                        //
                        //        fs.SetLength(0);
                        //
                        //        byte[] needByte = System.Text.Encoding.Default.GetBytes(contents);
                        //
                        //        fs.Write(needByte, 0, contents.Length);
                        //
                        //        fs.Close();
                        //
                        //        //Debug.LogError("需要字符串替换");
                        //    }
                        //}
                    }
                }
            }
        }

        AssetDatabase.Refresh();
    }

    //改变字体的风格（Arial）
    private static void CheckFStyle(string fullPath)
    {
           if (Directory.Exists(fullPath))
           {
               DirectoryInfo direction = new DirectoryInfo(fullPath);
               FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
               for (int i = 0; i < files.Length; i++)
               {
                   if (files[i].Name.EndsWith(".meta"))
                   {
                       continue;
                   }
                   string path = files[i].FullName;
                   int idx = path.IndexOf("Assets");
                   path = path.Remove(0, idx);
                   /*1AssetImporter的信息*/
                   AssetImporter importer = AssetImporter.GetAtPath(path);
                   /*2资源类型名字*/
                   Type assetsType = AssetDatabase.GetMainAssetTypeAtPath(path);
                   if (assetsType.ToString() == "UnityEngine.GameObject")
                   {
                       //Debug.LogError("有");
                       GameObject obj = AssetDatabase.LoadAssetAtPath(path, assetsType) as GameObject;
                       Text[] allText = obj.GetComponentsInChildren<Text>(true);
                       bool needLoading = false;
                       for (int j = 0; j < allText.Length; j++)
                       {
                        if (allText[j].font == null)
                            continue;
                        if (allText[j].font.name == "Arial" ||
                            allText[j].font.name == "msyh")
                           {
                               if (allText[j].fontStyle != FontStyle.Normal)
                               {
                                   //allText[j].fontStyle = FontStyle.Normal;
                                   Debug.Log(obj.name + ";" + allText[j].name + ", fontStyle = " + allText[j].fontStyle);
                                   needLoading = true;
                               }
                           }
                       }
                       if (needLoading)
                       {
                           string text = File.ReadAllText(files[i].FullName);
                           //FontStyle.Normal
                           for (int k = 1; k < 3; k++)
                           {
                               string key = "m_FontStyle: " + k;
                               if (text.Contains(key))
                               {
                                   text = text.Replace(key, "m_FontStyle: 0");
                                   FileStream fs = new FileStream(files[i].FullName, FileMode.Open);
                                   fs.SetLength(0);
                                   byte[] needByte = System.Text.Encoding.Default.GetBytes(text);
                                   fs.Write(needByte, 0, text.Length);
                                   fs.Close();
                                   //Debug.LogError("需要字符串替换");
                               }
                           }
                       }
                   }
               }
           }
    }

    #endregion

    [MenuItem("UITool/删除所有的PlayerPrefs数据")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

}
