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
        btnImgs = new Image[btns.Length];
        btnTxts = new Text[btns.Length];
        btnIndexMap.Clear();
        for (int i = 0; i < btns.Length; ++i)
        {
            Button btn = btns[i];
            btn.gameObject.SetActive(false);
            btnTxts[i] = btn.GetComponentInChildren<Text>();
            btnImgs[i] = btn.GetComponent<Image>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => 
            {
                OnClickBtn(btn);
            });
            btnIndexMap.Add(btn, i);
        }        
        scrollBar.onScrollChange += OnScrollChange;

        trPanelControl = transform.Find("PanelControl");

        btnSelCurPage.onClick.AddListener(OnBtnClickSelCurPage);
        btnInvSelCurPage.onClick.AddListener(OnBtnClickInvSelCurPage);
        btnSelAll.onClick.AddListener(OnBtnClickSelAll);
        btnUnSelAll.onClick.AddListener(OnBtnClickUnSelAll);
        btnSimSelDates.onClick.AddListener(OnBtnClickSimSelDates);
        btnSimAllDates.onClick.AddListener(OnBtnClickSimAllDates);
        btnRefreshFromSelDate.onClick.AddListener(OnBtnClickRefreshFromSelDate);
    }

    public Button btnSelCurPage;
    public Button btnInvSelCurPage;
    public Button btnSelAll;
    public Button btnUnSelAll;

    public Button btnSimSelDates;
    public Button btnSimAllDates;

    public Button btnRefreshFromSelDate;

    Transform trPanelControl;
    ScrollBar scrollBar;
    Button[] btns;
    Text[] btnTxts;
    Image[] btnImgs;
    Dictionary<Button, int> btnIndexMap = new Dictionary<Button, int>();
    List<int> selectedDateID = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        scrollBar.gameObject.SetActive(false);
        trPanelControl.gameObject.SetActive(false);
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
                int dateID = dataMgr.fileKeys[curID];
                btnTxts[i].text = dateID.ToString();
                if (selectedDateID.Contains(dateID))
                    btnImgs[i].color = Color.green;
                else
                    btnImgs[i].color = Color.white;
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
        int btnID = btnIndexMap[btn];
        Image img = btnImgs[btnID];
        int dateID = int.Parse(btnTxts[btnID].text);
        if (selectedDateID.Contains(dateID))
        {
            selectedDateID.Remove(dateID);
            img.color = Color.white;
        }
        else
        {
            selectedDateID.Add(dateID);
            img.color = Color.green;
        }
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
        scrollBar.gameObject.SetActive(DataManager.GetInst().fileKeys.Count > btns.Length);
        trPanelControl.gameObject.SetActive(DataManager.GetInst().fileKeys.Count > 0);
    }

    public void OnBtnClickClose()
    {
        gameObject.SetActive(false);
    }
    
    public void OnBtnClickSelCurPage()
    {
        for(int i = 0; i < btns.Length; ++i)
        {
            if(btns[i].gameObject.activeSelf)
            {
                int dateID = int.Parse(btnTxts[i].text);
                if (selectedDateID.Contains(dateID) == false)
                    selectedDateID.Add(dateID);
                btnImgs[i].color = Color.green;
            }
        }
    }
    public void OnBtnClickInvSelCurPage()
    {
        for (int i = 0; i < btns.Length; ++i)
        {
            if (btns[i].gameObject.activeSelf)
            {
                int dateID = int.Parse(btnTxts[i].text);
                if (selectedDateID.Contains(dateID))
                {
                    selectedDateID.Remove(dateID);
                    btnImgs[i].color = Color.white;
                }
                else
                {
                    selectedDateID.Add(dateID);
                    btnImgs[i].color = Color.green;
                }
            }
        }
    }
    public void OnBtnClickSelAll()
    {
        DataManager dataMgr = DataManager.GetInst();
        selectedDateID.Clear();
        selectedDateID.AddRange(dataMgr.fileKeys);
        for (int i = 0; i < btns.Length; ++i)
        {
            if (btns[i].gameObject.activeSelf)
            {
                btnImgs[i].color = Color.green;
            }
        }
    }
    public void OnBtnClickUnSelAll()
    {
        selectedDateID.Clear();
        for (int i = 0; i < btns.Length; ++i)
        {
            if (btns[i].gameObject.activeSelf)
            {
                btnImgs[i].color = Color.white;
            }
        }
    }
    public void OnBtnClickSimSelDates()
    {

    }
    public void OnBtnClickSimAllDates()
    {

    }
    public void OnBtnClickRefreshFromSelDate()
    {

    }

    #endregion

}
