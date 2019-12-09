using LotteryAnalyze;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class PanelStatisticCollect : MonoBehaviour
{
    public Dropdown dropdownStatisticType;
    public Button btnClose;
    public Button btnToggleBrief;

    public GameObject goBrief;
    public GraphBase graphStatistic;
    public ScrollBar scrollBar;
    public Slider slider;
    public Button btnViewData;

    GraphPainterStatistic curPainter = new GraphPainterStatistic();

    public Button btnStart;
    public Button btnStop;
    public Button btnExport;
    public Button btnImport;

    public Text txtInfo;

    public Image imgL;
    public Image imgG;

    public RectTransform rtContent;
    public RectTransform rtPrefab;


    static PanelStatisticCollect sInst;
    public static PanelStatisticCollect Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;

        Init();
    }

    public enum DataAnalyseType
    {
        // 所有的遗漏统计数据
        eDAT_MissCountTotal,
        // 统计K线触碰到布林上轨后的遗漏值
        eDAT_MissCountOnTouchBolleanUp,
        // 开出长遗漏后继续开出长遗漏的信息
        eDAT_ContinueLongMissCount,
        // 统计K线触碰到布林下轨后的遗漏值
        eDAT_MissCountOnTouchBulleanDown,
    }
    static List<string> DataAnalyseTypeStrs = new List<string>()
    {
            "所有的遗漏数据",
            "K线触碰到布林上轨后的遗漏值",
            "连续开出长遗漏的统计信息",
            "统计K线触碰到布林下轨后的遗漏值",
    };
    static List<string> ExportDataPaths = new List<string>()
    {
            "遗漏统计结果.xml",
            "从布林上轨连续遗漏的统计结果.xml",
            "连续开出长遗漏的统计信息.xml",
            "统计K线触碰到布林下轨后的遗漏值.xml",
    };


    float ITEM_W = 0;
    float ITEM_H = 0;
    float FULL_SIZE = 0;
    DataAnalyseType currentDataAnalyseType = DataAnalyseType.eDAT_MissCountTotal;

    List<int> dateIDLst = new List<int>();
    double updateInterval = 0.05;
    double updateCountDown = 0;
    int startIndex = -1;
    int endIndex = -1;
    int batchCount = 10;
    string lastDataItemTag = "";
    bool hasFinished = true;
    int allDataItemCount = 0;
    int CALC_PER_COUNT = 100;
    bool isStop = false;

    bool needRepaint = false;

    enum ProcStatus
    {
        eNotStart,
        eStart,
        ePrepBatch,
        eDoBatch,
        eCompleted,
    }
    ProcStatus status = ProcStatus.eNotStart;

    List<Dictionary<CollectDataType, Dictionary<int, int>>> allCDTMissCountNumMap = new List<Dictionary<CollectDataType, Dictionary<int, int>>>();
    DataItem cItem;

    Dictionary<CollectDataType, Dictionary<int, StatisticItem>> cdtMissCountTreeNodeMap = null;
    Dictionary<CollectDataType, Dictionary<int, int>> cdtMissCountNumMap = null;
    Dictionary<int, StatisticItem> missCountTreeNodeMap = null;
    Dictionary<int, int> missCountNumMap = null;
    StatisticItem numNode = null;
    StatisticItem cdtNode = null;
    StatisticItem missCountNode = null;
    StatisticUnitMap sum = null;
    StatisticUnit su = null;

    List<Dictionary<CollectDataType, int>> numberPathMissCount = new List<Dictionary<CollectDataType, int>>();
    List<Dictionary<CollectDataType, Dictionary<int, List<string>>>> overMissCountAndTouchBolleanUpInfos = new List<Dictionary<CollectDataType, Dictionary<int, List<string>>>>();

    List<Dictionary<CollectDataType, Dictionary<int, List<string>>>> overMissCountAndTouchBolleanDownInfos = new List<Dictionary<CollectDataType, Dictionary<int, List<string>>>>();


    List<Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>>> continueMissCountInfos = new List<Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>>>();
    List<Dictionary<CollectDataType, int>> lastLongMissCountInfo = new List<Dictionary<CollectDataType, int>>();
    List<Dictionary<CollectDataType, int>> currentLongMissCountInfo = new List<Dictionary<CollectDataType, int>>();


    List<StatisticItem> statisticItems = new List<StatisticItem>();
    List<StatisticItem> freePool = new List<StatisticItem>();
    List<ItemWrapper> treeInfos = new List<ItemWrapper>();

    public Dictionary<int, Dictionary<int, Dictionary<int, List<ItemWrapper>>>> allBarUpInfo = new Dictionary<int, Dictionary<int, Dictionary<int, List<ItemWrapper>>>>();

    public int TotalStartID = 0;
    public int TotalCount = 0;
    public int MaxMissCount = 0;

    public Dictionary<int, List<ItemWrapper>> graphDataItems = new Dictionary<int, List<ItemWrapper>>();

    void SetProgress(float pL, float pG)
    {
        imgL.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FULL_SIZE * pL);
        imgG.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FULL_SIZE * pG);
    }

    private void Init()
    {
        dropdownStatisticType.AddOptions(DataAnalyseTypeStrs);
        dropdownStatisticType.value = 0;
        dropdownStatisticType.onValueChanged.AddListener((v) =>
        {
            TotalStartID = 0;
            currentDataAnalyseType = (DataAnalyseType)dropdownStatisticType.value;

            treeInfos.Clear();
            RefreshView();
        });

        btnToggleBrief.onClick.AddListener(() =>
        {
            goBrief.SetActive(!goBrief.activeSelf);
        });

        btnClose.onClick.AddListener(() =>
        {
            LotteryManager.SetActive(gameObject, false);
        });

        btnStart.onClick.AddListener(() =>
        {
            DoStartCollect();
        });

        btnStop.onClick.AddListener(() =>
        {
            isStop = true;
        });

        btnExport.onClick.AddListener(() =>
        {
            switch (currentDataAnalyseType)
            {
                case DataAnalyseType.eDAT_MissCountTotal:
                    ExportForMissCountTotal();
                    break;
                case DataAnalyseType.eDAT_MissCountOnTouchBolleanUp:
                    ExportForMissCountOnTouchBolleanUp();
                    break;
                case DataAnalyseType.eDAT_ContinueLongMissCount:
                    ExportForContinueLongMissCount();
                    break;
                case DataAnalyseType.eDAT_MissCountOnTouchBulleanDown:
                    ExportForMissCountOnTouchBulleanDown();
                    break;
            }
        });

        btnImport.onClick.AddListener(() =>
        {
            switch (currentDataAnalyseType)
            {
                case DataAnalyseType.eDAT_MissCountOnTouchBolleanUp:
                    ImportFromMissCountOnTouchBolleanUp();
                    break;
                case DataAnalyseType.eDAT_MissCountTotal:
                    ImportFromMissCountTotal();
                    break;
                case DataAnalyseType.eDAT_ContinueLongMissCount:
                    ImportFromContinueLongMissCount();
                    break;
                case DataAnalyseType.eDAT_MissCountOnTouchBulleanDown:
                    ImportMissCountOnTouchBulleanDown();
                    break;
            }
        });

        float ratio = GlobalSetting.G_STATISTIC_CANVAS_SCALE_X / GlobalSetting.G_STATISTIC_CANVAS_SCALE_X_MAX;
        scrollBar.SetHandleRatio(0.2f);
        scrollBar.SetProgress(ratio);
        scrollBar.onScrollChange += () =>
        {
            GlobalSetting.G_STATISTIC_CANVAS_SCALE_X = scrollBar.GetProgress() * GlobalSetting.G_STATISTIC_CANVAS_SCALE_X_MAX;
            if (GlobalSetting.G_STATISTIC_CANVAS_SCALE_X < 0.01f)
                GlobalSetting.G_STATISTIC_CANVAS_SCALE_X = 0.01f;
            curPainter.OnGraphZoom(curPainter.upPainter);
            NotifyRepaint();
        };

        slider.onValueChanged.AddListener((v) =>
        {
            curPainter.OnScrollToData(slider.value);
        });

        btnViewData.onClick.AddListener(() =>
        {
            if(curPainter.selectedIndex >= 0)
            {
                if(graphDataItems.ContainsKey(curPainter.selectedIndex))
                {
                    List<ItemWrapper> lst = graphDataItems[curPainter.selectedIndex];
                    for (int i = 0; i < lst.Count; ++i)
                    {
                        ItemWrapper item = lst[i];
                        if (item.Tag != null && item.cdtIndex < 3)
                        {
                            string idtag = item.Text;
                            if (item.Text.Contains(","))
                            {
                                idtag = item.Text.Split(',')[0];
                            }
                            PanelDataView.Instance.ReadSpecDateData(idtag, (int)item.Tag);
                            PanelAnalyze.Instance.ChangeNumIndexAndCdtIndex(item.numIndex, item.cdtIndex);
                            return;
                        }
                    }
                }
            }
        });
    }

    public Vector2 ConvertPosToPainter(Vector2 pointerPos, GraphBase graph)
    {
        Vector2 pos = pointerPos;
        return pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        goBrief.SetActive(false);

        FULL_SIZE = (imgL.rectTransform.parent as RectTransform).rect.width;
        ITEM_W = rtPrefab.rect.width;
        ITEM_H = rtPrefab.rect.height;
        if (ITEM_W == 0)
            ITEM_W = FULL_SIZE;

        imgL.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        imgG.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        txtInfo.text = "";

        rtPrefab.gameObject.SetActive(false);
        freePool.Add(rtPrefab.GetComponent<StatisticItem>());

        graphStatistic.ConvertPosToPainter += ConvertPosToPainter;
    }

    // Update is called once per frame
    void Update()
    {
        DoUpdate();

        curPainter.Update();
    }

    private void LateUpdate()
    {
        if(needRepaint)
        {
            graphStatistic.SetVerticesDirty();
            needRepaint = false;
        }
    }

    void DoUpdate()
    {
        if (isStop)
            return;
        switch (status)
        {
            case ProcStatus.eStart:
            case ProcStatus.ePrepBatch:
                {
                    DoPrepBatch();
                    //if (updateCountDown <= 0)
                    //{
                    //    DoPrepBatch();
                    //    updateCountDown = updateInterval;
                    //}
                    //else
                    //{
                    //    updateCountDown -= Time.deltaTime;
                    //}
                }
                break;
            case ProcStatus.eDoBatch:
                DoBatch();
                break;
        }
    }

    void DoPrepBatch()
    {
        if(TotalCount > 0 && TotalStartID == 0)
        {
            TotalStartID = TotalCount;
        }
        DataManager dataMgr = DataManager.GetInst();
        endIndex = startIndex + batchCount;
        if (endIndex >= dateIDLst.Count)
            endIndex = dateIDLst.Count - 1;
        dataMgr.ClearAllDatas();
        for (int i = startIndex; i <= endIndex; ++i)
        {
            int key = dateIDLst[i];
            dataMgr.LoadData(key);
        }
        dataMgr.SetDataItemsGlobalID();
        if (dataMgr.GetAllDataItemCount() > 0)
        {
            Util.CollectPath012Info(null);
            if (currentDataAnalyseType == DataAnalyseType.eDAT_MissCountOnTouchBolleanUp ||
                currentDataAnalyseType == DataAnalyseType.eDAT_ContinueLongMissCount ||
                currentDataAnalyseType == DataAnalyseType.eDAT_MissCountOnTouchBulleanDown)
            {
                GraphDataManager.Instance.CollectGraphData(GraphType.eKCurveGraph);
            }

            cItem = dataMgr.GetFirstItem();

            DataItem tailItem = dataMgr.GetLatestRealItem();
            if(TotalStartID > 0 && tailItem != null)
            {
                TotalStartID -= tailItem.parent.datas.Count;
            }
            TotalCount = TotalStartID + dataMgr.GetAllDataItemCount();
            if (string.IsNullOrEmpty(lastDataItemTag) == false)
            {
                DataItem lastItem = dataMgr.GetDataItemByIdTag(lastDataItemTag);
                if (lastItem != null)
                    cItem = lastItem.parent.GetNextItem(lastItem);
            }
            allDataItemCount = dataMgr.GetAllDataItemCount();
            status = ProcStatus.eDoBatch;
        }
        else
            status = ProcStatus.ePrepBatch;
        SetProgress(0, (float)(endIndex + 1) / dateIDLst.Count);
    }

    void RefreshProgress()
    {
        float pG = 0, pL = 0;
        if (dateIDLst == null || dateIDLst.Count == 0)
        {
            pG = 1.0f;
        }
        else
        {
            pG = (float)(endIndex + 1) / dateIDLst.Count;
        }
        if (cItem != null)
        {
            pL = (float)(cItem.idGlobal + 1) / allDataItemCount;
            txtInfo.text = "当前" + cItem.idGlobal + "/" + allDataItemCount + ", 总(" + startIndex + "-" + endIndex + ")/" + dateIDLst.Count;
        }
        else
        {
            pL = 1.0f;
        }
        SetProgress(pL, pG);
    }

    void DoBatch()
    {
        switch (currentDataAnalyseType)
        {
            case DataAnalyseType.eDAT_MissCountTotal:
                AnalyseForMissCountTotal();
                break;
            case DataAnalyseType.eDAT_MissCountOnTouchBolleanUp:
                AnalyseForMissCountOnTouchBolleanUp();
                break;
            case DataAnalyseType.eDAT_ContinueLongMissCount:
                AnalyseContinueLongMissCount();
                break;
            case DataAnalyseType.eDAT_MissCountOnTouchBulleanDown:
                AnalyseMissCountOnTouchBulleanDown();
                break;
        }
    }

    void AnalyseForMissCountTotal()
    {
        int loop = CALC_PER_COUNT;
        while (cItem != null && loop-- > 0)
        {
            if (cItem != null)
            {
                for (int i = 0; i < 5; ++i)
                {
                    cdtMissCountNumMap = allCDTMissCountNumMap[i];
                    sum = cItem.statisticInfo.allStatisticInfo[i];

                    for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                        missCountNumMap = cdtMissCountNumMap[cdt];
                        su = sum.statisticUnitMap[cdt];

                        if (su.missCount == 0)
                        {
                            int lastMissCount = numberPathMissCount[i][cdt];
                            numberPathMissCount[i][cdt] = 0;
                            if (missCountNumMap.ContainsKey(lastMissCount))
                                missCountNumMap[lastMissCount] = missCountNumMap[lastMissCount] + 1;
                            else
                                missCountNumMap[lastMissCount] = 1;
                        }
                        else
                        {
                            numberPathMissCount[i][cdt] = su.missCount;
                        }
                    }
                }
                lastDataItemTag = cItem.idTag;
                cItem = cItem.parent.GetNextItem(cItem);

                if (cItem == null)
                {
                    if (endIndex == dateIDLst.Count - 1)
                    {
                        hasFinished = true;
                        lastDataItemTag = "";
                        status = ProcStatus.eCompleted;
                    }
                    else
                    {
                        startIndex = endIndex;
                        status = ProcStatus.ePrepBatch;
                    }

                    RefreshProgress();
                    return;
                }

                RefreshProgress();
            }
        }
    }

    void AnalyseForMissCountOnTouchBolleanUp()
    {
        int loop = CALC_PER_COUNT;
        while (cItem != null && loop-- > 0)
        {
            if (cItem != null)
            {
                for (int i = 0; i < 5; ++i)
                {
                    KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(i);

                    sum = cItem.statisticInfo.allStatisticInfo[i];

                    for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                        su = sum.statisticUnitMap[cdt];

                        if (su.missCount == 0)
                        {
                            int lastMissCount = numberPathMissCount[i][cdt];

                            // 判断当前这一期是否在触及布林上轨
                            bool isCurrentHitBooleanUp = CheckIsFullUp(i, cdt, cItem);

                            // 上次记录的遗漏值如果超过指定的遗漏值,就记录之
                            if (lastMissCount > GlobalSetting.G_OVER_SPEC_MISS_COUNT)
                            {
                                Dictionary<CollectDataType, Dictionary<int, List<string>>> numInfo = overMissCountAndTouchBolleanUpInfos[i];
                                Dictionary<int, List<string>> cdtMissCountInfo = null;
                                numInfo.TryGetValue(cdt, out cdtMissCountInfo);
                                if (cdtMissCountInfo == null)
                                {
                                    cdtMissCountInfo = new Dictionary<int, List<string>>();
                                    numInfo[cdt] = cdtMissCountInfo;
                                }
                                List<string> missInfo = null;
                                cdtMissCountInfo.TryGetValue(lastMissCount, out missInfo);
                                if (missInfo == null)
                                {
                                    missInfo = new List<string>();
                                    cdtMissCountInfo[lastMissCount] = missInfo;
                                }

                                string info = cItem.idTag;
                                int curID = cItem.idGlobal;
                                MACDPointMap mpmC = kddc.GetMacdPointMap(curID);
                                --curID;
                                MACDPointMap mpmP = kddc.GetMacdPointMap(curID);
                                int barGoUpCount = 0;
                                while(mpmC != null && mpmP != null)
                                {
                                    MACDPoint mpC = mpmC.GetData(cdt, false);
                                    MACDPoint mpP = mpmP.GetData(cdt, false);
                                    if (mpC.BAR > mpP.BAR)
                                        ++barGoUpCount;
                                    else
                                        break;
                                    --curID;
                                    mpmC = mpmP;
                                    mpmP = kddc.GetMacdPointMap(curID);
                                }
                                int globalID = TotalStartID + cItem.idGlobal;
                                info += "," + barGoUpCount + "," + globalID;
                                missInfo.Add(info);
                            }

                            // 当前触到布林中轨,就设置下次开始记录的遗漏值
                            if (isCurrentHitBooleanUp)
                            {
                                numberPathMissCount[i][cdt] = 0;
                            }
                            // 否则就不需要记录了
                            else
                            {
                                numberPathMissCount[i][cdt] = -1;
                            }
                        }
                        // 只有当上次记录的值>=0时才记录
                        else if (numberPathMissCount[i][cdt] >= 0)
                        {
                            numberPathMissCount[i][cdt] = su.missCount;
                        }
                    }
                }
                lastDataItemTag = cItem.idTag;
                cItem = cItem.parent.GetNextItem(cItem);

                if (cItem == null)
                {
                    if (endIndex == dateIDLst.Count - 1)
                    {
                        hasFinished = true;
                        lastDataItemTag = "";
                        status = ProcStatus.eCompleted;
                    }
                    else
                    {
                        startIndex = endIndex;
                        status = ProcStatus.ePrepBatch;
                    }
                    RefreshProgress();
                    return;
                }
                RefreshProgress();
            }
        }
    }

    void AnalyseMissCountOnTouchBulleanDown()
    {
        int loop = CALC_PER_COUNT;
        while (cItem != null && loop-- > 0)
        {
            if (cItem != null)
            {
                for (int i = 0; i < 5; ++i)
                {
                    KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(i);

                    sum = cItem.statisticInfo.allStatisticInfo[i];

                    for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                        su = sum.statisticUnitMap[cdt];

                        if (su.missCount == 0)
                        {
                            int lastMissCount = numberPathMissCount[i][cdt];

                            // 判断当前这一期是否在触及布林下轨
                            bool isCurrentHitBooleanDown = CheckIsTouchBolleanDown(i, cdt, cItem);

                            // 上次记录的遗漏值如果超过指定的遗漏值,就记录之
                            if (lastMissCount > GlobalSetting.G_OVER_SPEC_MISS_COUNT)
                            {
                                Dictionary<CollectDataType, Dictionary<int, List<string>>> numInfo = overMissCountAndTouchBolleanDownInfos[i];
                                Dictionary<int, List<string>> cdtMissCountInfo = null;
                                numInfo.TryGetValue(cdt, out cdtMissCountInfo);
                                if (cdtMissCountInfo == null)
                                {
                                    cdtMissCountInfo = new Dictionary<int, List<string>>();
                                    numInfo[cdt] = cdtMissCountInfo;
                                }
                                List<string> missInfo = null;
                                cdtMissCountInfo.TryGetValue(lastMissCount, out missInfo);
                                if (missInfo == null)
                                {
                                    missInfo = new List<string>();
                                    cdtMissCountInfo[lastMissCount] = missInfo;
                                }

                                string info = cItem.idTag;
                                int curID = cItem.idGlobal;
                                MACDPointMap mpmC = kddc.GetMacdPointMap(curID);
                                --curID;
                                MACDPointMap mpmP = kddc.GetMacdPointMap(curID);
                                int barGoUpCount = 0;
                                while (mpmC != null && mpmP != null)
                                {
                                    MACDPoint mpC = mpmC.GetData(cdt, false);
                                    MACDPoint mpP = mpmP.GetData(cdt, false);
                                    if (mpC.BAR > mpP.BAR)
                                        ++barGoUpCount;
                                    else
                                        break;
                                    --curID;
                                    mpmC = mpmP;
                                    mpmP = kddc.GetMacdPointMap(curID);
                                }
                                int globalID = TotalStartID + cItem.idGlobal;
                                info += "," + barGoUpCount + "," + globalID;
                                missInfo.Add(info);
                            }

                            // 当前触到布林中轨,就设置下次开始记录的遗漏值
                            if (isCurrentHitBooleanDown)
                            {
                                numberPathMissCount[i][cdt] = 0;
                            }
                            // 否则就不需要记录了
                            else
                            {
                                numberPathMissCount[i][cdt] = -1;
                            }
                        }
                        // 只有当上次记录的值>=0时才记录
                        else if (numberPathMissCount[i][cdt] >= 0)
                        {
                            numberPathMissCount[i][cdt] = su.missCount;
                        }
                    }
                }
                lastDataItemTag = cItem.idTag;
                cItem = cItem.parent.GetNextItem(cItem);

                if (cItem == null)
                {
                    if (endIndex == dateIDLst.Count - 1)
                    {
                        hasFinished = true;
                        lastDataItemTag = "";
                        status = ProcStatus.eCompleted;
                    }
                    else
                    {
                        startIndex = endIndex;
                        status = ProcStatus.ePrepBatch;
                    }
                    RefreshProgress();
                    return;
                }
                RefreshProgress();
            }
        }

    }

    bool CheckIsFullUp(int numIndex, CollectDataType cdt, DataItem testItem)
    {
        DataItem prevItem = testItem.parent.GetPrevItem(testItem);
        if (prevItem == null)
            return false;

        KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
        KDataMap kddCur = kddc.GetKDataDict(testItem);
        KData kdCur = kddCur.GetData(cdt, false);
        BollinPoint bpCur = kddc.GetBollinPointMap(kddCur).GetData(cdt, false);
        MACDPoint macdCur = kddc.GetMacdPointMap(kddCur).GetData(cdt, false);
        bool isCurTouchBU = kdCur.RelateDistTo(bpCur.upValue) <= 0;

        KDataMap kddPrv = kddc.GetKDataDict(prevItem);
        KData kdPrv = kddCur.GetData(cdt, false);
        BollinPoint bpPrv = kddc.GetBollinPointMap(kddPrv).GetData(cdt, false);
        MACDPoint macdPrv = kddc.GetMacdPointMap(kddPrv).GetData(cdt, false);
        bool isPrvTouchBU = kdPrv.RelateDistTo(bpPrv.upValue) <= 0;

        return isCurTouchBU && isPrvTouchBU && macdCur.BAR > macdPrv.BAR && macdCur.DIF > macdPrv.DIF;
    }

    bool CheckIsTouchBolleanDown(int numIndex, CollectDataType cdt, DataItem testItem)
    {
        KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
        KDataMap kddCur = kddc.GetKDataDict(testItem);
        KData kdCur = kddCur.GetData(cdt, false);
        BollinPoint bpCur = kddc.GetBollinPointMap(kddCur).GetData(cdt, false);
        MACDPoint macdCur = kddc.GetMacdPointMap(kddCur).GetData(cdt, false);
        bool isCurTouchBD = kdCur.RelateDistTo(bpCur.downValue) >= 0;
        return isCurTouchBD;
    }

    void AnalyseContinueLongMissCount()
    {
        int loop = CALC_PER_COUNT;
        while (cItem != null && loop-- > 0)
        {
            if (cItem != null)
            {
                for (int i = 0; i < 5; ++i)
                {
                    sum = cItem.statisticInfo.allStatisticInfo[i];

                    for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                    {
                        CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                        su = sum.statisticUnitMap[cdt];

                        if (su.missCount == 0)
                        {
                            int currentLongMissCount = currentLongMissCountInfo[i][cdt];
                            int lastLongMissCount = lastLongMissCountInfo[i][cdt];
                            if (lastLongMissCount == 0)
                            {
                                if (currentLongMissCount >= GlobalSetting.G_MISS_COUNT_FIRST)
                                    lastLongMissCountInfo[i][cdt] = currentLongMissCount;
                                currentLongMissCountInfo[i][cdt] = 0;
                            }
                            else
                            {
                                if (currentLongMissCount >= GlobalSetting.G_MISS_COUNT_SECOND)
                                {
                                    // record
                                    RecordContinueLongMissCount(i, cdt, lastLongMissCount, currentLongMissCount, cItem.idTag);
                                }

                                if (currentLongMissCount >= GlobalSetting.G_MISS_COUNT_FIRST)
                                {
                                    lastLongMissCountInfo[i][cdt] = currentLongMissCount;
                                }
                                else
                                {
                                    lastLongMissCountInfo[i][cdt] = 0;
                                }
                                currentLongMissCountInfo[i][cdt] = 0;
                            }
                        }
                        // 只有当上次记录的值>=0时才记录
                        else
                        {
                            currentLongMissCountInfo[i][cdt] = su.missCount;
                        }
                    }
                }
                lastDataItemTag = cItem.idTag;
                cItem = cItem.parent.GetNextItem(cItem);

                if (cItem == null)
                {
                    if (endIndex == dateIDLst.Count - 1)
                    {
                        hasFinished = true;
                        lastDataItemTag = "";
                        status = ProcStatus.eCompleted;
                    }
                    else
                    {
                        startIndex = endIndex;
                        status = ProcStatus.ePrepBatch;
                    }
                    RefreshProgress();
                    return;
                }
                RefreshProgress();
            }
        }
    }

    void RecordContinueLongMissCount(int numIndex, CollectDataType cdt, int lastMissCount, int curMissCount, string idTag)
    {
        Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>> numInfo = continueMissCountInfos[numIndex];
        Dictionary<int, Dictionary<int, List<string>>> cdtInfo = null;
        if (numInfo.ContainsKey(cdt))
            cdtInfo = numInfo[cdt];
        else
        {
            cdtInfo = new Dictionary<int, Dictionary<int, List<string>>>();
            numInfo[cdt] = cdtInfo;
        }
        Dictionary<int, List<string>> lastInfo = null;
        if (cdtInfo.ContainsKey(lastMissCount))
            lastInfo = cdtInfo[lastMissCount];
        else
        {
            lastInfo = new Dictionary<int, List<string>>();
            cdtInfo[lastMissCount] = lastInfo;
        }

        List<string> curInfo = null;
        if (lastInfo.ContainsKey(curMissCount))
            curInfo = lastInfo[curMissCount];
        else
        {
            curInfo = new List<string>();
            lastInfo[curMissCount] = curInfo;
        }

        curInfo.Add(idTag);
    }

    void DoStartCollect()
    {
        DataManager dataMgr = DataManager.GetInst();
        if(dataMgr.fileKeys.Count == 0)
        {
            PanelDataView.Instance.OnBtnClickImportData();
        }
        TotalCount = 0;
        TotalStartID = 0;

        isStop = false;
        status = ProcStatus.eStart;
        dateIDLst.Clear();
        dateIDLst.AddRange(dataMgr.mFileMetaInfo.Keys);
        startIndex = 0;
        allCDTMissCountNumMap.Clear();
        hasFinished = false;
        numberPathMissCount.Clear();
        overMissCountAndTouchBolleanUpInfos.Clear();
        overMissCountAndTouchBolleanDownInfos.Clear();
        continueMissCountInfos.Clear();
        lastLongMissCountInfo.Clear();
        currentLongMissCountInfo.Clear();
        for (int i = 0; i < 5; ++i)
        {
            Dictionary<CollectDataType, Dictionary<int, int>> dct = new Dictionary<CollectDataType, Dictionary<int, int>>();
            allCDTMissCountNumMap.Add(dct);
            Dictionary<CollectDataType, int> cdtMissCountMap = new Dictionary<CollectDataType, int>();
            numberPathMissCount.Add(cdtMissCountMap);
            overMissCountAndTouchBolleanUpInfos.Add(new Dictionary<CollectDataType, Dictionary<int, List<string>>>());
            overMissCountAndTouchBolleanDownInfos.Add(new Dictionary<CollectDataType, Dictionary<int, List<string>>>());
            continueMissCountInfos.Add(new Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>>());
            Dictionary<CollectDataType, int> lastLongMissCount = new Dictionary<CollectDataType, int>();
            Dictionary<CollectDataType, int> currentLongMissCount = new Dictionary<CollectDataType, int>();
            lastLongMissCountInfo.Add(lastLongMissCount);
            currentLongMissCountInfo.Add(currentLongMissCount);

            for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                dct.Add(cdt, new Dictionary<int, int>());
                cdtMissCountMap.Add(cdt, 0);
                lastLongMissCount.Add(cdt, 0);
                currentLongMissCount.Add(cdt, 0);
            }
        }
    }

    void ExportForMissCountTotal()
    {
        string fileName = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "../" + ExportDataPaths[(int)currentDataAnalyseType];
        FileStream fs = new FileStream(fileName, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);

        string info = "<root type=\"" + currentDataAnalyseType.ToString() + "\">\n";
        sw.Write(info);

        for (int i = 0; i < 5; ++i)
        {
            info = "\t<Num pos=\"" + KDataDictContainer.C_TAGS[i] + "\">\n";
            sw.Write(info);
            cdtMissCountNumMap = allCDTMissCountNumMap[i];

            for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];

                info = "\t\t<CDT name=\"" + cdt + "\">\n";
                sw.Write(info);
                missCountNumMap = cdtMissCountNumMap[cdt];

                foreach (int key in missCountNumMap.Keys)
                {
                    info = "\t\t\t<MissCount miss=\"" + key + "\" count=\"" + missCountNumMap[key] + "\"/>\n";
                    sw.Write(info);
                }

                info = "\t\t</CDT>\n";
                sw.Write(info);
            }

            info = "\t</Num>\n";
            sw.Write(info);
        }

        info = "</root>";
        sw.Write(info);

        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
        fs.Close();

        PanelMessageBox.Instance.Show("成功导出: " + fileName, null, null);
    }

    void ExportForMissCountOnTouchBolleanUp()
    {
        string fileName = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "../" + ExportDataPaths[(int)currentDataAnalyseType];
        FileStream fs = new FileStream(fileName, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);

        string info = "<root type=\"" + currentDataAnalyseType.ToString() + "\" TotalCount=\"" + TotalCount + "\">\n";
        sw.Write(info);

        for (int i = 0; i < 5; ++i)
        {
            info = "\t<Num pos=\"" + KDataDictContainer.C_TAGS[i] + "\">\n";
            sw.Write(info);

            Dictionary<CollectDataType, Dictionary<int, List<string>>> cdtMissInfo = overMissCountAndTouchBolleanUpInfos[i];

            for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];

                if (cdtMissInfo.ContainsKey(cdt) == false)
                    continue;
                Dictionary<int, List<string>> countInfo = cdtMissInfo[cdt];

                info = "\t\t<CDT name=\"" + cdt + "\">\n";
                sw.Write(info);
                foreach (int key in countInfo.Keys)
                {
                    info = "\t\t\t<Count count=\"" + key + "\">\n";
                    sw.Write(info);

                    List<string> missInfo = countInfo[key];
                    for (int jj = 0; jj < missInfo.Count; ++jj)
                    {
                        info = "\t\t\t\t<MissInfo item=\"" + missInfo[jj] + "\"/>\n";
                        sw.Write(info);
                    }

                    info = "\t\t\t</Count>\n";
                    sw.Write(info);
                }

                info = "\t\t</CDT>\n";
                sw.Write(info);
            }

            info = "\t</Num>\n";
            sw.Write(info);
        }

        info = "</root>";
        sw.Write(info);

        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
        fs.Close();

        PanelMessageBox.Instance.Show("成功导出: " + fileName, null, null);
    }

    void ExportForContinueLongMissCount()
    {
        string fileName = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "../" + ExportDataPaths[(int)currentDataAnalyseType];
        FileStream fs = new FileStream(fileName, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);

        string info = "<root type=\"" + currentDataAnalyseType.ToString() + "\">\n";
        sw.Write(info);

        for (int i = 0; i < 5; ++i)
        {
            info = "\t<Num pos=\"" + KDataDictContainer.C_TAGS[i] + "\">\n";
            sw.Write(info);

            Dictionary<CollectDataType, Dictionary<int, Dictionary<int, List<string>>>> numInfo = continueMissCountInfos[i];
            for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                Dictionary<int, Dictionary<int, List<string>>> cdtInfo = null;
                if (numInfo.ContainsKey(cdt) == false)
                    continue;
                cdtInfo = numInfo[cdt];

                info = "\t\t<CDT name=\"" + cdt + "\">\n";
                sw.Write(info);

                List<int> mcFirstLst = new List<int>();
                mcFirstLst.AddRange(cdtInfo.Keys);
                mcFirstLst.Sort((x, y) => { if (x < y) return -1; return 1; });
                for (int fi = 0; fi < mcFirstLst.Count; ++fi)
                {
                    int firstMissCount = mcFirstLst[fi];
                    Dictionary<int, List<string>> curMissCountInfo = cdtInfo[firstMissCount];
                    info = "\t\t\t<FirstMissCount count=\"" + firstMissCount + "\">\n";
                    sw.Write(info);

                    List<int> mcSecondLst = new List<int>();
                    mcSecondLst.AddRange(curMissCountInfo.Keys);
                    mcSecondLst.Sort((x, y) => { if (x < y) return -1; return 1; });
                    for (int si = 0; si < mcSecondLst.Count; ++si)
                    {
                        int secMissCount = mcSecondLst[si];
                        List<string> tags = curMissCountInfo[secMissCount];
                        info = "\t\t\t\t<SecondMissCount count=\"" + secMissCount + "\">\n";
                        sw.Write(info);

                        for (int t = 0; t < tags.Count; ++t)
                        {
                            info = "\t\t\t\t\t<MissCountInfo>" + tags[t] + "</MissCountInfo>\n";
                            sw.Write(info);
                        }

                        info = "\t\t\t\t</SecondMissCount>\n";
                        sw.Write(info);
                    }

                    info = "\t\t\t</FirstMissCount>\n";
                    sw.Write(info);
                }

                info = "\t\t</CDT>\n";
                sw.Write(info);
            }

            info = "\t</Num>\n";
            sw.Write(info);
        }

        info = "</root>";
        sw.Write(info);

        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
        fs.Close();

        PanelMessageBox.Instance.Show("成功导出: " + fileName, null, null);
    }

    void ExportForMissCountOnTouchBulleanDown()
    {
        string fileName = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "../" + ExportDataPaths[(int)currentDataAnalyseType];
        FileStream fs = new FileStream(fileName, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);

        string info = "<root type=\"" + currentDataAnalyseType.ToString() + "\" TotalCount=\"" + TotalCount + "\">\n";
        sw.Write(info);

        for (int i = 0; i < 5; ++i)
        {
            info = "\t<Num pos=\"" + KDataDictContainer.C_TAGS[i] + "\">\n";
            sw.Write(info);

            Dictionary<CollectDataType, Dictionary<int, List<string>>> cdtMissInfo = overMissCountAndTouchBolleanDownInfos[i];

            for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];

                if (cdtMissInfo.ContainsKey(cdt) == false)
                    continue;
                Dictionary<int, List<string>> countInfo = cdtMissInfo[cdt];

                info = "\t\t<CDT name=\"" + cdt + "\">\n";
                sw.Write(info);
                foreach (int key in countInfo.Keys)
                {
                    info = "\t\t\t<Count count=\"" + key + "\">\n";
                    sw.Write(info);

                    List<string> missInfo = countInfo[key];
                    for (int jj = 0; jj < missInfo.Count; ++jj)
                    {
                        info = "\t\t\t\t<MissInfo item=\"" + missInfo[jj] + "\"/>\n";
                        sw.Write(info);
                    }

                    info = "\t\t\t</Count>\n";
                    sw.Write(info);
                }

                info = "\t\t</CDT>\n";
                sw.Write(info);
            }

            info = "\t</Num>\n";
            sw.Write(info);
        }

        info = "</root>";
        sw.Write(info);

        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
        fs.Close();

        PanelMessageBox.Instance.Show("成功导出: " + fileName, null, null);
    }

    void RecycleAllItems()
    {
        for (int i = 0; i < statisticItems.Count; ++i)
        {
            statisticItems[i].SetItem(null);
            statisticItems[i].gameObject.SetActive(false);
            freePool.Add(statisticItems[i]);
        }
        statisticItems.Clear();
    }

    StatisticItem CreateItem(ItemWrapper item)
    {
        StatisticItem si = null;
        if (freePool.Count > 0)
        {
            si = freePool[freePool.Count - 1];
            freePool.RemoveAt(freePool.Count - 1);
        }
        else
        {
            GameObject go = GameObject.Instantiate(rtPrefab.gameObject);
            si = go.GetComponent<StatisticItem>();
        }
        si.gameObject.SetActive(true);
        si.rtSelf.SetParent(rtContent, true);
        statisticItems.Add(si);
        si.SetItem(item);
        si.rtSelf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ITEM_W);
        si.rtSelf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ITEM_H);
        return si;
    }

    void ImportFromMissCountTotal()
    {
        string fileName = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "../" + ExportDataPaths[(int)currentDataAnalyseType];
        XmlDocument x = new XmlDocument();
        try
        {
            x.Load(fileName);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            PanelMessageBox.Instance.Show("读取" + fileName + "失败 - " + e.ToString(), null, null);
            return;
        }

        XmlNode pNode = x.SelectSingleNode("root");
        if (pNode == null)
            return;
        treeInfos.Clear();
        foreach (XmlNode numNode in pNode.ChildNodes)
        {
            int numIndex = KDataDictContainer.C_TAGS.IndexOf(numNode.Attributes[0].Value);
            ItemWrapper tnNum = new ItemWrapper(numNode.Attributes[0].Value);
            treeInfos.Add(tnNum);

            foreach (XmlNode cdtNode in numNode.ChildNodes)
            {
                int cdtIndex = GraphDataManager.GetCdtIndexByEnumStr(cdtNode.Attributes[0].Value);
                ItemWrapper tnCDT = new ItemWrapper(cdtNode.Attributes[0].Value);
                tnNum.SubNodes.Add(tnCDT);

                foreach(XmlNode missCountNode in cdtNode.ChildNodes)
                {
                    string miss = missCountNode.Attributes[0].Value;
                    string count = missCountNode.Attributes[1].Value;

                    ItemWrapper iwMC = new ItemWrapper(miss + " - " + count);
                    iwMC.Tag = int.Parse(miss);
                    iwMC.showViewBtn = false;
                    iwMC.isLeafNode = true;
                    tnCDT.SubNodes.Add(iwMC);
                }

                tnCDT.SubNodes.Sort((a, b) =>
                {
                    if ((int)a.Tag > (int)b.Tag)
                        return -1;
                    return 1;
                });
            }
        }
        RefreshView();
    }

    void ImportFromMissCountOnTouchBolleanUp()
    {
        string fileName = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "../" + ExportDataPaths[(int)currentDataAnalyseType];

        XmlDocument x = new XmlDocument();
        try
        {
            x.Load(fileName);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            PanelMessageBox.Instance.Show("读取" + fileName + "失败 - " + e.ToString(), null, null);
            return;
        }

        XmlNode pNode = x.SelectSingleNode("root");
        if (pNode == null)
            return;

        MaxMissCount = 0;

        TotalCount = int.Parse(pNode.Attributes["TotalCount"].Value);

        treeInfos.Clear();
        allBarUpInfo.Clear();
        graphDataItems.Clear();

        ItemWrapper iwBolleanUpLongMiss = new ItemWrapper("遗漏信息");
        treeInfos.Add(iwBolleanUpLongMiss);

        // numIndex node
        foreach (XmlNode numNode in pNode.ChildNodes)
        {
            int numIndex = KDataDictContainer.C_TAGS.IndexOf(numNode.Attributes[0].Value);
            ItemWrapper tnNum = new ItemWrapper(numNode.Attributes[0].Value);
            iwBolleanUpLongMiss.SubNodes.Add(tnNum);

            Dictionary<int, Dictionary<int, List<ItemWrapper>>> numBarUpInfo = new Dictionary<int, Dictionary<int, List<ItemWrapper>>>();
            allBarUpInfo[numIndex] = numBarUpInfo;

            // collectDataType node
            foreach (XmlNode cdtNode in numNode.ChildNodes)
            {
                int cdtIndex = GraphDataManager.GetCdtIndexByEnumStr(cdtNode.Attributes[0].Value);
                ItemWrapper tnCDT = new ItemWrapper(cdtNode.Attributes[0].Value);
                tnNum.SubNodes.Add(tnCDT);

                Dictionary<int, List<ItemWrapper>> cdtBarUpInfo = new Dictionary<int, List<ItemWrapper>>();
                numBarUpInfo[cdtIndex] = cdtBarUpInfo;

                // missCount node
                foreach (XmlNode coundNode in cdtNode.ChildNodes)
                {
                    ItemWrapper tnCount = new ItemWrapper(coundNode.Attributes[0].Value);
                    tnCDT.SubNodes.Add(tnCount);

                    int missCount = int.Parse(coundNode.Attributes[0].Value);
                    if (MaxMissCount < missCount)
                        MaxMissCount = missCount;

                    foreach (XmlNode missNode in coundNode.ChildNodes)
                    {
                        ItemWrapper tnMiss = new ItemWrapper(missNode.Attributes[0].Value);
                        tnCount.SubNodes.Add(tnMiss);

                        string[] strs = tnMiss.Text.Split(',');
                        int date = int.Parse(strs[0].Split('-')[0]);
                        int barGoUpCount = 0;
                        int globalID = -1;
                        if(strs.Length > 1)
                            barGoUpCount = int.Parse(strs[1]);
                        if (strs.Length > 2)
                            globalID = int.Parse(strs[2]);
                        tnMiss.Tag = date;
                        tnMiss.globalID = globalID;
                        tnMiss.numIndex = numIndex;
                        tnMiss.cdtIndex = cdtIndex;
                        tnMiss.showViewBtn = true;
                        tnMiss.isLeafNode = true;
                        tnMiss.barUpCount = barGoUpCount;
                        tnMiss.missCount = missCount;

                        List<ItemWrapper> lst = null;
                        if (cdtBarUpInfo.ContainsKey(barGoUpCount))
                            lst = cdtBarUpInfo[barGoUpCount];
                        else
                        { 
                            lst = new List<ItemWrapper>();
                            cdtBarUpInfo[barGoUpCount] = lst;
                        }
                        lst.Add(tnMiss);

                        List<ItemWrapper> lstData = null;
                        if (graphDataItems.ContainsKey(globalID))
                            lstData = graphDataItems[globalID];
                        else
                        {
                            lstData = new List<ItemWrapper>();
                            graphDataItems[globalID] = lstData;
                        }
                        lstData.Add(tnMiss);
                    }

                    tnCDT.SubNodes.Sort((a, b) =>
                    {
                        int va = int.Parse(a.Text);
                        int vb = int.Parse(b.Text);
                        if (va < vb)
                            return 1;
                        return -1;
                    });
                }
            }
        }

        ItemWrapper iwBarUpCount = new ItemWrapper("长遗漏柱值提升期数统计信息");
        treeInfos.Add(iwBarUpCount);
        foreach(int numIndex in allBarUpInfo.Keys)
        {
            ItemWrapper iwNum = new ItemWrapper(KDataDictContainer.C_TAGS[numIndex]);
            iwBarUpCount.SubNodes.Add(iwNum);

            Dictionary<int, Dictionary<int, List<ItemWrapper>>> cdtInfo = allBarUpInfo[numIndex];
            foreach(int cdtIndex in cdtInfo.Keys)
            {
                ItemWrapper iwCDT = new ItemWrapper(GraphDataManager.S_CDT_TAG_LIST[cdtIndex]);
                iwNum.SubNodes.Add(iwCDT);

                Dictionary<int, List<ItemWrapper>> info = cdtInfo[cdtIndex];
                foreach (int key in info.Keys)
                {
                    List<ItemWrapper> lst = info[key];

                    ItemWrapper iwKeyNode = new ItemWrapper(key + " - " + lst.Count);
                    iwCDT.SubNodes.Add(iwKeyNode);
                    iwKeyNode.Tag = key;

                    for(int i = 0; i < lst.Count; ++i)
                    {
                        ItemWrapper iwNode = new ItemWrapper(lst[i].Text);
                        iwKeyNode.SubNodes.Add(iwNode);
                        iwNode.showViewBtn = true;
                        iwNode.isLeafNode = true;
                        iwNode.Tag = lst[i].Tag;
                        iwNode.numIndex = lst[i].numIndex;
                        iwNode.cdtIndex = lst[i].cdtIndex;
                    }
                }

                iwCDT.SubNodes.Sort((a, b) =>
                {
                    if ((int)a.Tag > (int)b.Tag) return -1;
                    return 1;
                });
            }
        }

        slider.minValue = 0;
        slider.maxValue = TotalCount;
        slider.value = 0;
        RefreshView();
    }

    void ImportFromContinueLongMissCount()
    {
        string fileName = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "../" + ExportDataPaths[(int)currentDataAnalyseType];

        XmlDocument x = new XmlDocument();
        try
        {
            x.Load(fileName);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            PanelMessageBox.Instance.Show("读取" + fileName + "失败 - " + e.ToString(), null, null);
            return;
        }

        XmlNode pNode = x.SelectSingleNode("root");
        if (pNode == null)
            return;

        treeInfos.Clear();
        foreach (XmlNode numNode in pNode.ChildNodes)
        {
            int numIndex = KDataDictContainer.C_TAGS.IndexOf(numNode.Attributes[0].Value);
            ItemWrapper tnNum = new ItemWrapper(numNode.Attributes[0].Value);
            treeInfos.Add(tnNum);

            foreach (XmlNode cdtNode in numNode.ChildNodes)
            {
                int cdtIndex = GraphDataManager.GetCdtIndexByEnumStr(cdtNode.Attributes[0].Value);
                ItemWrapper tnCDT = new ItemWrapper(cdtNode.Attributes[0].Value);
                tnNum.SubNodes.Add(tnCDT);

                foreach (XmlNode firstNode in cdtNode.ChildNodes)
                {
                    ItemWrapper tnFirst = new ItemWrapper(firstNode.Attributes[0].Value);
                    tnCDT.SubNodes.Add(tnFirst);

                    foreach (XmlNode secNode in firstNode.ChildNodes)
                    {
                        ItemWrapper tnSec = new ItemWrapper(secNode.Attributes[0].Value);
                        tnFirst.SubNodes.Add(tnSec);

                        foreach (XmlNode missNode in secNode.ChildNodes)
                        {
                            ItemWrapper tnMiss = new ItemWrapper(missNode.InnerXml);
                            tnSec.SubNodes.Add(tnMiss);

                            int date = int.Parse(tnMiss.Text.Split('-')[0]);
                            tnMiss.Tag = date;
                            tnMiss.numIndex = numIndex;
                            tnMiss.cdtIndex = cdtIndex;
                            tnMiss.showViewBtn = true;
                            tnMiss.isLeafNode = true;
                        }

                        tnFirst.SubNodes.Sort((a, b) =>
                        {
                            int va = int.Parse(a.Text);
                            int vb = int.Parse(b.Text);
                            if (va < vb)
                                return 1;
                            return -1;
                        });
                    }

                    tnCDT.SubNodes.Sort((a, b) =>
                    {
                        int va = int.Parse(a.Text);
                        int vb = int.Parse(b.Text);
                        if (va < vb)
                            return 1;
                        return -1;
                    });
                }
            }
        }
        RefreshView();
    }

    void ImportMissCountOnTouchBulleanDown()
    {
        string fileName = LotteryAnalyze.AutoUpdateUtil.DATA_PATH_FOLDER + "../" + ExportDataPaths[(int)currentDataAnalyseType];

        XmlDocument x = new XmlDocument();
        try
        {
            x.Load(fileName);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            PanelMessageBox.Instance.Show("读取" + fileName + "失败 - " + e.ToString(), null, null);
            return;
        }

        XmlNode pNode = x.SelectSingleNode("root");
        if (pNode == null)
            return;

        MaxMissCount = 0;

        TotalCount = int.Parse(pNode.Attributes["TotalCount"].Value);

        treeInfos.Clear();
        allBarUpInfo.Clear();
        graphDataItems.Clear();

        ItemWrapper iwBolleanUpLongMiss = new ItemWrapper("遗漏信息");
        treeInfos.Add(iwBolleanUpLongMiss);

        // numIndex node
        foreach (XmlNode numNode in pNode.ChildNodes)
        {
            int numIndex = KDataDictContainer.C_TAGS.IndexOf(numNode.Attributes[0].Value);
            ItemWrapper tnNum = new ItemWrapper(numNode.Attributes[0].Value);
            iwBolleanUpLongMiss.SubNodes.Add(tnNum);

            Dictionary<int, Dictionary<int, List<ItemWrapper>>> numBarUpInfo = new Dictionary<int, Dictionary<int, List<ItemWrapper>>>();
            allBarUpInfo[numIndex] = numBarUpInfo;

            // collectDataType node
            foreach (XmlNode cdtNode in numNode.ChildNodes)
            {
                int cdtIndex = GraphDataManager.GetCdtIndexByEnumStr(cdtNode.Attributes[0].Value);
                ItemWrapper tnCDT = new ItemWrapper(cdtNode.Attributes[0].Value);
                tnNum.SubNodes.Add(tnCDT);

                Dictionary<int, List<ItemWrapper>> cdtBarUpInfo = new Dictionary<int, List<ItemWrapper>>();
                numBarUpInfo[cdtIndex] = cdtBarUpInfo;

                // missCount node
                foreach (XmlNode coundNode in cdtNode.ChildNodes)
                {
                    ItemWrapper tnCount = new ItemWrapper(coundNode.Attributes[0].Value);
                    tnCDT.SubNodes.Add(tnCount);

                    int missCount = int.Parse(coundNode.Attributes[0].Value);
                    if (MaxMissCount < missCount)
                        MaxMissCount = missCount;

                    foreach (XmlNode missNode in coundNode.ChildNodes)
                    {
                        ItemWrapper tnMiss = new ItemWrapper(missNode.Attributes[0].Value);
                        tnCount.SubNodes.Add(tnMiss);

                        string[] strs = tnMiss.Text.Split(',');
                        int date = int.Parse(strs[0].Split('-')[0]);
                        int barGoUpCount = 0;
                        int globalID = -1;
                        if (strs.Length > 1)
                            barGoUpCount = int.Parse(strs[1]);
                        if (strs.Length > 2)
                            globalID = int.Parse(strs[2]);
                        tnMiss.Tag = date;
                        tnMiss.globalID = globalID;
                        tnMiss.numIndex = numIndex;
                        tnMiss.cdtIndex = cdtIndex;
                        tnMiss.showViewBtn = true;
                        tnMiss.isLeafNode = true;
                        tnMiss.barUpCount = barGoUpCount;
                        tnMiss.missCount = missCount;

                        List<ItemWrapper> lst = null;
                        if (cdtBarUpInfo.ContainsKey(barGoUpCount))
                            lst = cdtBarUpInfo[barGoUpCount];
                        else
                        {
                            lst = new List<ItemWrapper>();
                            cdtBarUpInfo[barGoUpCount] = lst;
                        }
                        lst.Add(tnMiss);

                        List<ItemWrapper> lstData = null;
                        if (graphDataItems.ContainsKey(globalID))
                            lstData = graphDataItems[globalID];
                        else
                        {
                            lstData = new List<ItemWrapper>();
                            graphDataItems[globalID] = lstData;
                        }
                        lstData.Add(tnMiss);
                    }

                    tnCDT.SubNodes.Sort((a, b) =>
                    {
                        int va = int.Parse(a.Text);
                        int vb = int.Parse(b.Text);
                        if (va < vb)
                            return 1;
                        return -1;
                    });
                }
            }
        }

        ItemWrapper iwBarUpCount = new ItemWrapper("长遗漏柱值提升期数统计信息");
        treeInfos.Add(iwBarUpCount);
        foreach (int numIndex in allBarUpInfo.Keys)
        {
            ItemWrapper iwNum = new ItemWrapper(KDataDictContainer.C_TAGS[numIndex]);
            iwBarUpCount.SubNodes.Add(iwNum);

            Dictionary<int, Dictionary<int, List<ItemWrapper>>> cdtInfo = allBarUpInfo[numIndex];
            foreach (int cdtIndex in cdtInfo.Keys)
            {
                ItemWrapper iwCDT = new ItemWrapper(GraphDataManager.S_CDT_TAG_LIST[cdtIndex]);
                iwNum.SubNodes.Add(iwCDT);

                Dictionary<int, List<ItemWrapper>> info = cdtInfo[cdtIndex];
                foreach (int key in info.Keys)
                {
                    List<ItemWrapper> lst = info[key];

                    ItemWrapper iwKeyNode = new ItemWrapper(key + " - " + lst.Count);
                    iwCDT.SubNodes.Add(iwKeyNode);
                    iwKeyNode.Tag = key;

                    for (int i = 0; i < lst.Count; ++i)
                    {
                        ItemWrapper iwNode = new ItemWrapper(lst[i].Text);
                        iwKeyNode.SubNodes.Add(iwNode);
                        iwNode.showViewBtn = true;
                        iwNode.isLeafNode = true;
                        iwNode.Tag = lst[i].Tag;
                        iwNode.numIndex = lst[i].numIndex;
                        iwNode.cdtIndex = lst[i].cdtIndex;
                    }
                }

                iwCDT.SubNodes.Sort((a, b) =>
                {
                    if ((int)a.Tag > (int)b.Tag) return -1;
                    return 1;
                });
            }
        }

        slider.minValue = 0;
        slider.maxValue = TotalCount;
        slider.value = 0;
        RefreshView();

    }

    public void RefreshView()
    {
        RecycleAllItems();

        float height = 0;
        int layer = 0;
        for(int i = 0; i < treeInfos.Count; ++i)
        {
            RefreshItem(treeInfos[i], ref height, ref layer);
        }
        rtContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void RefreshItem(ItemWrapper item, ref float height, ref int layer)
    {
        StatisticItem si = CreateItem(item);
        si.rtSelf.anchoredPosition = new Vector2(0, -height);
        si.rtInfo.anchoredPosition = new Vector2(layer * ITEM_H, 0);
        height += ITEM_H;

        if (item.Expand)
        {
            ++layer;
            for (int i = 0; i < item.SubNodes.Count; ++i)
            {
                RefreshItem(item.SubNodes[i], ref height, ref layer);
            }
            --layer;
        }
    }

    public void NotifyRepaint()
    {
        needRepaint = true;
    }
}
