using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PanelDataView : MonoBehaviour
{
    static PanelDataView sInst;
    public static PanelDataView Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;
        scrollBar = transform.Find("ScrollBar").gameObject.GetComponent<ScrollBar>();
        Transform txtView = transform.Find("PanelListView");
        btns = txtView.gameObject.GetComponentsInChildren<Button>();
        btnTxts = new Text[btns.Length];
        btnTxtDict.Clear();
        for (int i = 0; i < btns.Length; ++i)
        {
            Button btn = btns[i];
            btn.gameObject.SetActive(false);
            btnTxts[i] = btn.GetComponentInChildren<Text>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => 
            {
                OnClickBtn(btn);
            });
            btnTxtDict.Add(btn, btnTxts[i]);
        }        
        scrollBar.onScrollChange += OnScrollChange;
    }

    


    public ScrollBar scrollBar;
    public Button[] btns;
    public Text[] btnTxts;
    public Dictionary<Button, Text> btnTxtDict = new Dictionary<Button, Text>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoopSearchFolder(DirectoryInfo parentDirInfo)
    {
        DataManager dataMgr = DataManager.GetInst();
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
        dataMgr.GenFileKeys();
    }

    void RefreshFileList()
    {
        DataManager dataMgr = DataManager.GetInst();
        int pages = dataMgr.fileKeys.Count / btns.Length + 1;
        int curPage = (int)(scrollBar.GetProgress() * pages);
        int startID = curPage * btns.Length;
        int endID = startID + btns.Length - 1;
        if (endID >= dataMgr.fileKeys.Count)
            endID = dataMgr.fileKeys.Count - 1;
        int curID = startID;
        for(int i = 0; i < btns.Length; ++i)
        {
            if(curID <= endID && curID < dataMgr.fileKeys.Count)
            {
                btns[i].gameObject.SetActive(true);
                btnTxts[i].text = dataMgr.fileKeys[curID].ToString();
            }
            else
            {
                btns[i].gameObject.SetActive(false);
            }
            ++curID;
        }
    }


    #region call backs

    void OnClickBtn(Button btn)
    {
        Debug.Log("Click Btn " + btnTxtDict[btn].text);
    }

    public void OnScrollChange()
    {
        RefreshFileList();
    }

    public void OnBtnClickImportData()
    {
        DirectoryInfo di = new DirectoryInfo(LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER);
        LoopSearchFolder(di);
        scrollBar.SetHandleRatio(Mathf.Clamp01((float)(btns.Length) / (float)DataManager.GetInst().fileKeys.Count));
        RefreshFileList();
    }

    public void OnBtnClickClose()
    {
        gameObject.SetActive(false);
    }

    #endregion

}
