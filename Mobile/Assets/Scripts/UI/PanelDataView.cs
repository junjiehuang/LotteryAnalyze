﻿using LotteryAnalyze;
using System;
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

        btnReadSelDateData.onClick.AddListener(OnBtnClickReadSelDateData);
        btnWriteSelDateData.onClick.AddListener(OnBtnClickWriteSelDateData);
        btnCloseSelDateData.onClick.AddListener(OnBtnClickCloseSelDateData);

        btnPrevBatchReadAndAnalyze.onClick.AddListener(OnBtnClickPrevBatchReadAndAnalyze);
        btnNextBatchReadAndAnalyze.onClick.AddListener(OnBtnClickNextBatchReadAndAnalyze);
    }

    public Button btnSelCurPage;
    public Button btnInvSelCurPage;
    public Button btnSelAll;
    public Button btnUnSelAll;

    public Button btnSimSelDates;
    public Button btnSimAllDates;

    public Button btnRefreshFromSelDate;

    public Button btnReadSelDateData;
    public Button btnWriteSelDateData;
    public Button btnCloseSelDateData;

    public Button btnPrevBatchReadAndAnalyze;
    public Button btnNextBatchReadAndAnalyze;

    public ScrollRect scrollRectDataView;
    public RectTransform rtDataEditContent;
    public GameObject editDataItem;

    List<DateDateEditItem> freeEditDataItems = new List<DateDateEditItem>();
    List<DateDateEditItem> showEditDataItems = new List<DateDateEditItem>();

    Transform trPanelControl;
    ScrollBar scrollBar;
    Button[] btns;
    Text[] btnTxts;
    Image[] btnImgs;
    Dictionary<Button, int> btnIndexMap = new Dictionary<Button, int>();
    List<int> selectedDateID = new List<int>();

    int lastFetchCount = -1;
    float lastUpdateTime;
    int startRefreshDateIndex = -1;

    string editFilePath = "";
    int editFileID = -1;

    // Start is called before the first frame update
    void Start()
    {
        scrollBar.gameObject.SetActive(false);
        trPanelControl.gameObject.SetActive(false);
        scrollRectDataView.gameObject.SetActive(false);
        DateDateEditItem prefabItem = editDataItem.GetComponent<DateDateEditItem>();
        freeEditDataItems.Add(prefabItem);
        editDataItem.SetActive(false);
        rtDataEditContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 120 * prefabItem.rectTransform.rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoopSearchFolder(DirectoryInfo parentDirInfo)
    {
        //Debug.Log("LoopSearchFolder : " + parentDirInfo.FullName);
        DataManager dataMgr = DataManager.GetInst();
        FileInfo[] files = parentDirInfo.GetFiles();
        DirectoryInfo[] dirs = parentDirInfo.GetDirectories();
        for (int i = 0; i < files.Length; ++i)
        {
            string fullFileName = files[i].FullName.Replace('\\', '/');
            //Debug.Log("Try to split file : " + fullFileName);
            string[] strs = fullFileName.Split('/');
            string fileName = strs[strs.Length - 1];
            strs = fileName.Split('.');

            if (strs.Length > 0)
            {
                int id = -1;
                if (int.TryParse(strs[0], out id))
                {
                    if (dataMgr.mFileMetaInfo.ContainsKey(id) == false)
                    {
                        dataMgr.mFileMetaInfo.Add(id, files[i].FullName);
                    }
                }
                else
                {
                    Debug.LogError("Invalid file : " + strs[0]);
                }
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
        int pages = dataMgr.fileKeys.Count / btns.Length;
        if (btns.Length * pages < dataMgr.fileKeys.Count)
            ++pages;
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

    void TickAutoRefreshLatestData()
    {

    }

    void RefreshLatestData(bool forceUpdate)
    {
        DataManager dataMgr = DataManager.GetInst();
        // 如果当前不是在获取最新数据，返回
        if (GlobalSetting.IsCurrentFetchingLatestData == false)
            return;

        if (forceUpdate == false)
        {
            // 如果不是自动获取，返回
            if (GlobalSetting.G_AUTO_REFRESH_LATEST_DATA == false)
                return;

            // 间隔时间满足才执行刷新
            if ((Time.time - lastUpdateTime) < GlobalSetting.G_AUTO_REFRESH_LATEST_DATA_INTERVAL)
                return;
            lastUpdateTime = Time.time;
        }

        // 更新当天的数据
        int currentFetchCount = AutoUpdateUtil.AutoFetchTodayData();
        // 如果数据没变化，直接返回
        if (currentFetchCount == lastFetchCount && forceUpdate == false)
            return;
        lastFetchCount = currentFetchCount;

        // 如果文件列表是空的，读取数据文件列表
        if (dataMgr.fileKeys.Count == 0)
        {
            OnBtnClickImportData();
        }

        DateTime startCollectDate = DateTime.Now.AddDays(-GlobalSetting.G_DAYS_PER_BATCH);
        if (dataMgr.fileKeys.Count > 0)
        {
            int startDateKey = dataMgr.fileKeys[dataMgr.fileKeys.Count - 1];
            AutoUpdateUtil.DateKeyToDateTime(startDateKey);
        }
        bool hasFetchNewData = false;
        string error = "";
        DateTime curDate = DateTime.Now;
        for (DateTime cd = startCollectDate; cd.CompareTo(curDate) <= 0; cd = cd.AddDays(1))
        {
            string dateTag = AutoUpdateUtil.combineDateString(cd.Year, cd.Month, cd.Day);
            int dateKey = int.Parse(dateTag);
            
            if (DataManager.GetInst().fileKeys.Contains(dateKey) == false)
            {
                AutoUpdateUtil.FetchData(cd, ref error);
                string filePath = AutoUpdateUtil.combineFileName(cd.Year, cd.Month, cd.Day);
                dataMgr.AddMetaInfo(dateKey, filePath);
                hasFetchNewData = true;
            }
        }

        if(hasFetchNewData)
        {
            OnBtnClickImportData();
        }
        
        int newAddItemIndex = -1, tmpAddItemIndex = -1;
        OneDayDatas newAddODD = null, tmpAddODD = null;

        int lastItemID = dataMgr.fileKeys.Count - 1;
        if (lastItemID > 0)
            lastItemID -= GlobalSetting.G_DAYS_PER_BATCH;
        if (startRefreshDateIndex >= 0)
            lastItemID = startRefreshDateIndex;
        if (lastItemID < 0)
            startRefreshDateIndex = 0;
        
        while (lastItemID != dataMgr.fileKeys.Count && lastItemID >= 0)
        {
            int key = dataMgr.fileKeys[lastItemID];
            dataMgr.LoadDataExt(key, ref tmpAddODD, ref tmpAddItemIndex);
            if (newAddItemIndex == -1 && tmpAddItemIndex != -1)
            {
                newAddODD = tmpAddODD;
                newAddItemIndex = tmpAddItemIndex;
            }
            ++lastItemID;
        }
        dataMgr.SetDataItemsGlobalID();

        if (GlobalSetting.G_ENABLE_SHOW_KCURVE_PREDICT)
        {
            DataItem tailItem = dataMgr.GetLatestItem();
            if (tailItem != null)
            {
                dataMgr.GenerateFakeODD(tailItem.lotteryNumber);
                dataMgr.SetDataItemsGlobalID();
            }
        }

        Util.CollectPath012Info(null, newAddODD, newAddItemIndex);
        GraphDataManager.ResetCurKValueMap();
        GraphDataManager.Instance.CollectGraphData(GraphType.eKCurveGraph);

        LotteryManager.SetActive(PanelAnalyze.Instance.gameObject, true);
        if(PanelAnalyze.Instance.SelectKDataIndex == -1 ||
           PanelAnalyze.Instance.SelectKDataIndex >= dataMgr.GetAllDataItemCount())
        {
            PanelAnalyze.Instance.SelectKDataIndex = dataMgr.GetAllDataItemCount() - 1;
            PanelAnalyze.Instance.OnSelectedDataItemChanged();
        }
        PanelAnalyze.Instance.NotifyUIRepaint();
    }

    // 读取指定日期到当前最新的所有数据
    public void RefreshLatestDataFromSpecDate()
    {
        if(startRefreshDateIndex == -1)
        {
            if (selectedDateID.Count > 0)
            {
                int lastDateKey = selectedDateID[selectedDateID.Count - 1];
                startRefreshDateIndex = DataManager.GetInst().fileKeys.IndexOf(lastDateKey);
            }
        }
        GlobalSetting.IsCurrentFetchingLatestData = true;
        DataManager.GetInst().ClearAllDatas();
        RefreshLatestData(true);
    }


    int BatchStartIndex = -1;
    public void ReadBatchData(bool prevBatch)
    {
        GlobalSetting.IsCurrentFetchingLatestData = false;

        DataManager dataMgr = DataManager.GetInst();

        // 如果文件列表是空的，读取数据文件列表
        if (dataMgr.fileKeys.Count == 0)
        {
            OnBtnClickImportData();
        }


        if (BatchStartIndex < 0)
        {
            if (selectedDateID.Count > 0)
            {
                int lastDateKey = selectedDateID[selectedDateID.Count - 1];
                BatchStartIndex = DataManager.GetInst().fileKeys.IndexOf(lastDateKey);
            }
            else
                BatchStartIndex = 0;
        }
        int EndIndex = BatchStartIndex;
        if (prevBatch)
        {
            if (BatchStartIndex == 0)
                return;
            else
            {
                EndIndex = BatchStartIndex;
                BatchStartIndex = EndIndex - GlobalSetting.G_DAYS_PER_BATCH;
                if (BatchStartIndex < 0)
                    BatchStartIndex = 0;
            }
        }
        else
        {
            if (BatchStartIndex == dataMgr.fileKeys.Count - 1)
                return;
            EndIndex = BatchStartIndex + GlobalSetting.G_DAYS_PER_BATCH;
            if (EndIndex >= dataMgr.fileKeys.Count)
                EndIndex = dataMgr.fileKeys.Count - 1;
        }

        if (BatchStartIndex != EndIndex)
        {
            dataMgr.ClearAllDatas();
            for (int i = BatchStartIndex; i <= EndIndex; ++i)
            {
                int key = dataMgr.fileKeys[i];
                dataMgr.LoadData(key);
            }
            dataMgr.SetDataItemsGlobalID();

            Util.CollectPath012Info(null);
            GraphDataManager.ResetCurKValueMap();
            GraphDataManager.Instance.CollectGraphData(GraphType.eKCurveGraph);

            LotteryManager.SetActive(PanelAnalyze.Instance.gameObject, true);
            if (PanelAnalyze.Instance.SelectKDataIndex == -1 ||
               PanelAnalyze.Instance.SelectKDataIndex >= dataMgr.GetAllDataItemCount())
            {
                PanelAnalyze.Instance.SelectKDataIndex = dataMgr.GetAllDataItemCount() - 1;
                PanelAnalyze.Instance.OnSelectedDataItemChanged();
            }
            PanelAnalyze.Instance.NotifyUIRepaint();
        }
        BatchStartIndex = EndIndex;
    }

    public void ReadSpecDateData(string idtag, int dateID)
    {
        GlobalSetting.IsCurrentFetchingLatestData = false;
        DataManager dataMgr = DataManager.GetInst();
        // 如果文件列表是空的，读取数据文件列表
        if (dataMgr.fileKeys.Count == 0)
        {
            OnBtnClickImportData();
        }
        int startID = DataManager.GetInst().fileKeys.IndexOf(dateID);
        int endID = startID + 1;
        if (startID > 0)
            --startID;
        if (endID >= DataManager.GetInst().fileKeys.Count)
            endID = DataManager.GetInst().fileKeys.Count - 1;
        dataMgr.ClearAllDatas();
        for (int i = startID; i <= endID; ++i)
        {
            int key = dataMgr.fileKeys[i];
            dataMgr.LoadData(key);
        }
        dataMgr.SetDataItemsGlobalID();

        Util.CollectPath012Info(null);
        GraphDataManager.ResetCurKValueMap();
        GraphDataManager.Instance.CollectGraphData(GraphType.eKCurveGraph);

        LotteryManager.SetActive(PanelAnalyze.Instance.gameObject, true);
        PanelAnalyze.Instance.SelectKDataIndex = DataManager.GetInst().GetDataItemByIdTag(idtag).idGlobal;
        PanelAnalyze.Instance.OnSelectedDataItemChanged();
        PanelAnalyze.Instance.NotifyUIRepaint();
    }


    #region call backs

    void OnBtnClickReadSelDateData()
    {
        if (selectedDateID.Count == 1)
        {
            freeEditDataItems.AddRange(showEditDataItems);
            showEditDataItems.Clear();

            editFileID = selectedDateID[0];
            editFilePath = DataManager.GetInst().GetFilePath(editFileID);
            OneDayDatas odd = null;
            Util.ReadFile(editFileID, editFilePath, ref odd);
            scrollRectDataView.gameObject.SetActive(true);

            for (int i = 0; i < 120; ++i)
            {
                DateDateEditItem editItem = null;
                if(freeEditDataItems.Count > 0)
                {
                    editItem = freeEditDataItems[0];
                    freeEditDataItems.RemoveAt(0);
                }
                else
                {
                    GameObject go = GameObject.Instantiate(editDataItem);
                    editItem = go.GetComponent<DateDateEditItem>();
                }
                showEditDataItems.Add(editItem);
                editItem.gameObject.SetActive(true);
                if (i < odd.datas.Count)
                {
                    DataItem item = odd.datas[i];
                    editItem.SetData(item);
                }
                else
                {
                    editItem.label.text = odd.dateID + "-" + AutoUpdateUtil.GetHundredIndexString(i + 1);
                    editItem.inputField.text = "-";
                }
                editItem.rectTransform.SetParent(rtDataEditContent);
                editItem.rectTransform.anchoredPosition = new Vector2(0, -editItem.rectTransform.rect.height * i);
            }
            
        }
    }

    void OnBtnClickWriteSelDateData()
    {
        if(editFileID != -1 && !string.IsNullOrEmpty(editFilePath))
        {
            string content = "";
            for(int i = 0; i < 120; ++i)
            {
                content += AutoUpdateUtil.GetHundredIndexString(i + 1) + " ";
                DateDateEditItem item = showEditDataItems[i];
                content += item.inputField.text + "\n";
            }

            FileStream fs = new FileStream(editFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(content);
            //sw.Write(strWebContent);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }
    }

    void OnBtnClickCloseSelDateData()
    {
        freeEditDataItems.AddRange(showEditDataItems);
        showEditDataItems.Clear();
        for(int i = 0; i < freeEditDataItems.Count; ++i)
        {
            freeEditDataItems[i].gameObject.SetActive(false);
        }
    }

    void OnBtnClickPrevBatchReadAndAnalyze()
    {
        ReadBatchData(true);
    }

    void OnBtnClickNextBatchReadAndAnalyze()
    {
        ReadBatchData(false);
    }

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
        if (!Directory.Exists(LotteryAnalyze.AutoUpdateUtil.LOTTERY_DATA_PATH))
        {
            Directory.CreateDirectory(LotteryAnalyze.AutoUpdateUtil.LOTTERY_DATA_PATH);
            Debug.Log("Create Lottery Data Folder : " + LotteryAnalyze.AutoUpdateUtil.LOTTERY_DATA_PATH);
        }

        DirectoryInfo di = new DirectoryInfo(LotteryAnalyze.AutoUpdateUtil.LOTTERY_DATA_PATH);
        LoopSearchFolder(di);
        float ratio = Mathf.Clamp01((float)(btns.Length) / (float)DataManager.GetInst().fileKeys.Count);
        if (ratio < 0.1f)
            ratio = 0.1f;
        scrollBar.SetHandleRatio(ratio);
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
        if(selectedDateID.Count > 0)
        {
            int firstIndex = selectedDateID[0];
            int lastIndex = selectedDateID[selectedDateID.Count - 1];
            PanelTrade.Instance.StartDate = firstIndex;// DataManager.GetInst().fileKeys.IndexOf(firstIndex);
            PanelTrade.Instance.EndDate = lastIndex;// DataManager.GetInst().fileKeys.IndexOf(lastIndex);
            LotteryManager.SetActive(PanelTrade.Instance.gameObject, true);
        }
    }
    public void OnBtnClickSimAllDates()
    {
        PanelTrade.Instance.StartDate = -1;
        PanelTrade.Instance.EndDate = -1;
        LotteryManager.SetActive(PanelTrade.Instance.gameObject, true);
    }

    // 从选择的最后一个日期作为起始日期，读取截至到目前最新的所有数据
    public void OnBtnClickRefreshFromSelDate()
    {
        GlobalSetting.IsCurrentFetchingLatestData = true;
        DataManager.GetInst().ClearAllDatas();

        startRefreshDateIndex = -1;
        if(selectedDateID.Count > 0)
        {
            int lastDateKey = selectedDateID[selectedDateID.Count - 1];
            startRefreshDateIndex = DataManager.GetInst().fileKeys.IndexOf(lastDateKey);
        }
        RefreshLatestData(true);
    }

    #endregion



}
