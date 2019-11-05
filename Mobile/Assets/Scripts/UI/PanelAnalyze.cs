using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PanelAnalyze : MonoBehaviour
{
    [System.Serializable]
    public class SettingCommon
    {
        public GameObject settingPanelCommon;

        public UnityEngine.UI.Dropdown dropdownNumIndex;
        public UnityEngine.UI.Dropdown dropdownCDT;
        public UnityEngine.UI.Button buttonPrev;
        public UnityEngine.UI.Button buttonNext;
    }

    [System.Serializable]
    public class SettingKGraph
    {
        public GameObject settingPanelKData;

        public UnityEngine.UI.InputField inputFieldScaleX;
        public UnityEngine.UI.InputField inputFieldScaleY;
        public UnityEngine.UI.Dropdown dropdownCycle;
        public UnityEngine.UI.Toggle toggleAuxline;
        public UnityEngine.UI.Toggle toggleKRuler;
        public UnityEngine.UI.Toggle toggleBoolean;
        public UnityEngine.UI.Toggle toggleMACD;
        public UnityEngine.UI.Toggle toggleAvgLine;
        public UnityEngine.UI.Toggle toggleAvg5;
        public UnityEngine.UI.Toggle toggleAvg10;
        public UnityEngine.UI.Toggle toggleAvg20;
        public UnityEngine.UI.Toggle toggleAvg30;
        public UnityEngine.UI.Toggle toggleAvg50;
        public UnityEngine.UI.Toggle toggleAvg100;
        public UnityEngine.UI.Button buttonAutoCalcBestPosPath;
        public UnityEngine.UI.Button buttonAddPosPath;
        public UnityEngine.UI.Button buttonRemovePosPath;
        public UnityEngine.UI.Button buttonRemoveAllPosPath;

        public UnityEngine.UI.Dropdown dropdownOperationType;
        public UnityEngine.UI.Button buttonDelSelAuxLine;
        public UnityEngine.UI.Button buttonDelAllAuxLine;

        public ListView listviewBestPosPath;
    }

    [System.Serializable]
    public class SettingBar
    {
        public GameObject settingPanelBar;

        public UnityEngine.UI.Dropdown dropdownStatisticType;
        public UnityEngine.UI.Dropdown dropdownStatisticRange;
        public UnityEngine.UI.InputField inputStatisticCustomRange;
        public UnityEngine.UI.Button buttonPrev;
        public UnityEngine.UI.Button buttonNext;
    }

    [System.Serializable]
    public class SettingMiss
    {
        public GameObject settingPanelMiss;

        public UnityEngine.UI.InputField inputFieldScaleX;
        public UnityEngine.UI.InputField inputFieldScaleY;
        public UnityEngine.UI.Toggle toggleShowSingleLine;
        public UnityEngine.UI.Dropdown dropdownMissType;
        public UnityEngine.UI.Dropdown dropdownAppearenceType;
        public ListView cdtShowStateView;
    }

    public class PosPathTag
    {
        public int numIndex;
        public int cdtIndex;

        public PosPathTag(int _numIndex, int _cdtIndex)
        {
            numIndex = _numIndex;
            cdtIndex = _cdtIndex;
        }
    }

    public UnityEngine.UI.Slider fastViewSlider;

    public GraphBase graphUp;
    public GraphBase graphDown;

    public SettingCommon settingCommon;
    public SettingKGraph settingKGraph;
    public SettingBar settingBar;
    public SettingMiss settingMiss;

    GraphPainterKData graghPainterKData = new GraphPainterKData();
    GraphPainterBar graphPainterBar = new GraphPainterBar();
    GraphPainterMiss graphPainterMiss = new GraphPainterMiss();
    public GraphPainterBase curGraphPainter;

    public int numIndex = 0;
    public CollectDataType cdt = CollectDataType.ePath0;
    public int curCycleIndex = 0;
    public int endShowDataItemIndex = -1;

    public int selectKDataIndex = -1;
    public int SelectKDataIndex
    {
        get { return selectKDataIndex; }
        set { selectKDataIndex = value; }
    }

    public int statisticType = 0;
    public int statisticRangeIndex = 0;

    int notifyGraphsUpdateFrame = -1;
    bool needUpdateScrollToSpecDataItem = true;

    [HideInInspector]
    public SplitPanel splitPanel;

    static PanelAnalyze sInst;
    public static PanelAnalyze Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;

        splitPanel = GetComponentInChildren<SplitPanel>();
        fastViewSlider.gameObject.SetActive(false);
        fastViewSlider.transform.parent.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        splitPanel.onSplitterChange += () => 
        {
            curGraphPainter.OnAutoAllign();
        };

        graphDown.ConvertPosToPainter += ConvertPosToPainter;
        graphUp.ConvertPosToPainter += ConvertPosToPainter;

        graghPainterKData.Start();
        graphPainterBar.Start();
        graphPainterMiss.Start();
        InitSettingPanelCommon();
        InitSettingPanelKData();
        InitSettingPanelBar();
        InitSettingPanelMiss();

        SetCurrentGraph(graghPainterKData);
    }

    void InitSettingPanelCommon()
    {
        settingCommon.settingPanelCommon.transform.parent.gameObject.SetActive(false);

        settingCommon.dropdownNumIndex.AddOptions(KDataDictContainer.C_TAGS);
        settingCommon.dropdownNumIndex.value = numIndex;
        settingCommon.dropdownNumIndex.onValueChanged.AddListener((v) =>
        {
            numIndex = settingCommon.dropdownNumIndex.value;
            OnBtnClickAutoAllign();
            NotifyUIRepaint();
        });

        settingCommon.dropdownCDT.AddOptions(GraphDataManager.S_CDT_TAG_LIST);
        settingCommon.dropdownCDT.value = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
        settingCommon.dropdownCDT.onValueChanged.AddListener((v) =>
        {
            cdt = GraphDataManager.S_CDT_LIST[settingCommon.dropdownCDT.value];
            OnBtnClickAutoAllign();
            NotifyUIRepaint();
        });

        settingCommon.buttonPrev.onClick.AddListener(() =>
        {
            if (SelectKDataIndex > 0)
            {
                --SelectKDataIndex;
                OnSelectedDataItemChanged();
            }
        });

        settingCommon.buttonNext.onClick.AddListener(() =>
        {
            if (SelectKDataIndex < DataManager.GetInst().GetAllDataItemCount() - 1)
            {
                ++SelectKDataIndex;
                OnSelectedDataItemChanged();
            }
        });

        settingCommon.settingPanelCommon.SetActive(true);
    }

    void InitSettingPanelKData()
    {
        graghPainterKData.canvasUpScale.x = graghPainterKData.canvasDownScale.x = GlobalSetting.G_KGRAPH_CANVAS_SCALE_X;
        settingKGraph.inputFieldScaleX.text = graghPainterKData.canvasUpScale.x.ToString();
        settingKGraph.inputFieldScaleX.onValueChanged.AddListener((str) => 
        {
            float.TryParse(settingKGraph.inputFieldScaleX.text, out graghPainterKData.canvasUpScale.x);
            if (graghPainterKData.canvasUpScale.x == 0)
            {
                graghPainterKData.canvasUpScale.x = 1;
                settingKGraph.inputFieldScaleX.text = graghPainterKData.canvasUpScale.x.ToString();
            }
            graghPainterKData.canvasDownScale.x = graghPainterKData.canvasUpScale.x;
            GlobalSetting.G_KGRAPH_CANVAS_SCALE_X = graghPainterKData.canvasDownScale.x;
            graghPainterKData.NeedRebuildAuxPoints = true;
            NotifyUIRepaint();
        });

        graghPainterKData.canvasUpScale.y = GlobalSetting.G_KGRAPH_CANVAS_SCALE_Y;
        settingKGraph.inputFieldScaleY.text = graghPainterKData.canvasUpScale.y.ToString();
        settingKGraph.inputFieldScaleY.onValueChanged.AddListener((str) =>
        {
            float.TryParse(settingKGraph.inputFieldScaleY.text, out graghPainterKData.canvasUpScale.y);
            if (graghPainterKData.canvasUpScale.y == 0)
            {
                graghPainterKData.canvasUpScale.y = 1;
                settingKGraph.inputFieldScaleY.text = graghPainterKData.canvasUpScale.y.ToString();
            }
            GlobalSetting.G_KGRAPH_CANVAS_SCALE_Y = graghPainterKData.canvasUpScale.y;
            graghPainterKData.NeedRebuildAuxPoints = true;
            NotifyUIRepaint();
        });

        settingKGraph.dropdownCycle.AddOptions(GraphDataManager.G_Circles_STRs);
        settingKGraph.dropdownCycle.value = curCycleIndex;
        settingKGraph.dropdownCycle.onValueChanged.AddListener((v) =>
        {
            curCycleIndex = settingKGraph.dropdownCycle.value;
            GraphDataManager.CurrentCircle = GraphDataManager.G_Circles[curCycleIndex];
            OnBtnClickAutoAllign();
            NotifyUIRepaint();
        });

        settingKGraph.toggleAuxline.isOn = graghPainterKData.enableAuxLine;
        settingKGraph.toggleAuxline.onValueChanged.AddListener((v) => 
        {
            graghPainterKData.enableAuxLine = settingKGraph.toggleAuxline.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleKRuler.isOn = graghPainterKData.enableKRuler;
        settingKGraph.toggleKRuler.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableKRuler = settingKGraph.toggleKRuler.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleBoolean.isOn = graghPainterKData.enableBollinBand;
        settingKGraph.toggleBoolean.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableBollinBand = settingKGraph.toggleBoolean.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleMACD.isOn = graghPainterKData.enableMACD;
        settingKGraph.toggleMACD.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableMACD = settingKGraph.toggleMACD.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleAvgLine.isOn = graghPainterKData.enableAvgLines;
        settingKGraph.toggleAvgLine.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableAvgLines = settingKGraph.toggleAvgLine.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleAvg5.isOn = graghPainterKData.enableAvg5;
        settingKGraph.toggleAvg5.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableAvg5 = settingKGraph.toggleAvg5.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleAvg10.isOn = graghPainterKData.enableAvg10;
        settingKGraph.toggleAvg10.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableAvg10 = settingKGraph.toggleAvg10.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleAvg20.isOn = graghPainterKData.enableAvg20;
        settingKGraph.toggleAvg20.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableAvg20 = settingKGraph.toggleAvg20.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleAvg30.isOn = graghPainterKData.enableAvg30;
        settingKGraph.toggleAvg30.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableAvg30 = settingKGraph.toggleAvg30.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleAvg50.isOn = graghPainterKData.enableAvg50;
        settingKGraph.toggleAvg50.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableAvg50 = settingKGraph.toggleAvg50.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.toggleAvg100.isOn = graghPainterKData.enableAvg100;
        settingKGraph.toggleAvg100.onValueChanged.AddListener((v) =>
        {
            graghPainterKData.enableAvg100 = settingKGraph.toggleAvg100.isOn;
            NotifyUIRepaint();
        });

        settingKGraph.listviewBestPosPath.onClickItem += OnClickItemBestPosPath;

        settingKGraph.buttonAutoCalcBestPosPath.onClick.AddListener(() =>
        {
            settingKGraph.listviewBestPosPath.ClearAllItems();
            List<TradeDataManager.NumCDT> results = TradeDataManager.Instance.CalcFavorits(null);
            for (int i = 0; i < results.Count; ++i)
            {
                AddPosPath(results[i].numID, results[i].cdtID);
            }
        });

        settingKGraph.buttonAddPosPath.onClick.AddListener(() =>
        {
            int cdtIndex = settingCommon.dropdownCDT.value;
            AddPosPath(numIndex, cdtIndex);
        });

        settingKGraph.buttonRemovePosPath.onClick.AddListener(() =>
        {
            settingKGraph.listviewBestPosPath.RemoveItem(settingKGraph.listviewBestPosPath.SelIndex);
        });

        settingKGraph.buttonRemoveAllPosPath.onClick.AddListener(() =>
        {
            settingKGraph.listviewBestPosPath.ClearAllItems();
        });

        settingKGraph.dropdownOperationType.AddOptions(GraphPainterKData.S_AUX_LINE_OPERATIONS);
        settingKGraph.dropdownOperationType.value = (int)graghPainterKData.auxLineOperation;
        settingKGraph.dropdownOperationType.onValueChanged.AddListener((i) =>
        {
            graghPainterKData.auxLineOperation = (AuxLineType)settingKGraph.dropdownOperationType.value;
        });

        settingKGraph.buttonDelSelAuxLine.onClick.AddListener(() =>
        {
            graghPainterKData.DelSelAuxLine();
            NotifyUIRepaint();
        });

        settingKGraph.buttonDelAllAuxLine.onClick.AddListener(() =>
        {
            graghPainterKData.DelAllAuxLine();
            NotifyUIRepaint();
        });

        settingKGraph.settingPanelKData.SetActive(false);
    }

    void InitSettingPanelBar()
    {
        settingBar.dropdownStatisticType.AddOptions(GraphDataContainerBarGraph.S_StatisticsType_STRS);
        settingBar.dropdownStatisticType.value = statisticType;
        settingBar.dropdownStatisticType.onValueChanged.AddListener((v) =>
        {
            statisticType = settingBar.dropdownStatisticType.value;
            GraphDataManager.BGDC.curStatisticsType = (GraphDataContainerBarGraph.StatisticsType)statisticType;
            OnSelectedDataItemChanged();
        });

        settingBar.dropdownStatisticRange.AddOptions(GraphDataContainerBarGraph.S_StatisticsRange_STRS);
        settingBar.dropdownStatisticRange.value = statisticRangeIndex;
        settingBar.dropdownStatisticRange.onValueChanged.AddListener((v) =>
        {
            statisticRangeIndex = settingBar.dropdownStatisticRange.value;
            GraphDataManager.BGDC.curStatisticsRange = (GraphDataContainerBarGraph.StatisticsRange)statisticRangeIndex;
            OnSelectedDataItemChanged();
        });

        settingBar.inputStatisticCustomRange.text = GraphDataManager.BGDC.customStatisticsRange.ToString();
        settingBar.inputStatisticCustomRange.onEndEdit.AddListener((str) =>
        {
            int range = 1;
            if(int.TryParse(settingBar.inputStatisticCustomRange.text, out range))
            {
                GraphDataManager.BGDC.customStatisticsRange = range;
            }
            else
            {
                GraphDataManager.BGDC.customStatisticsRange = 5;
            }
            if (GraphDataManager.BGDC.customStatisticsRange < 5)
                GraphDataManager.BGDC.customStatisticsRange = 5;
            settingBar.inputStatisticCustomRange.text = GraphDataManager.BGDC.customStatisticsRange.ToString();
            OnSelectedDataItemChanged();
        });

        settingBar.buttonPrev.onClick.AddListener(() => 
        {
            if (SelectKDataIndex > 0)
            {
                --SelectKDataIndex;
                OnSelectedDataItemChanged();
            }
        });

        settingBar.buttonNext.onClick.AddListener(() =>
        {
            if (SelectKDataIndex < DataManager.GetInst().GetAllDataItemCount() - 1)
            {
                ++SelectKDataIndex;
                OnSelectedDataItemChanged();
            }
        });

        settingBar.settingPanelBar.SetActive(false);
    }

    void InitSettingPanelMiss()
    {
        graphPainterMiss.canvasUpScale.x = graphPainterMiss.canvasDownScale.x = GlobalSetting.G_MISSGRAPH_CANVAS_SCALE_X;
        settingMiss.inputFieldScaleX.text = graphPainterMiss.canvasUpScale.x.ToString();
        settingMiss.inputFieldScaleX.onValueChanged.AddListener((str) =>
        {
            float.TryParse(settingMiss.inputFieldScaleX.text, out graphPainterMiss.canvasUpScale.x);
            if (graphPainterMiss.canvasUpScale.x == 0)
            {
                graphPainterMiss.canvasUpScale.x = 1;
                settingMiss.inputFieldScaleX.text = graphPainterMiss.canvasUpScale.x.ToString();
            }
            graphPainterMiss.canvasDownScale.x = graphPainterMiss.canvasUpScale.x;
            GlobalSetting.G_MISSGRAPH_CANVAS_SCALE_X = graphPainterMiss.canvasDownScale.x;
            NotifyUIRepaint();
        });

        graphPainterMiss.canvasUpScale.y = GlobalSetting.G_KGRAPH_CANVAS_SCALE_Y;
        settingMiss.inputFieldScaleY.text = graphPainterMiss.canvasUpScale.y.ToString();
        settingMiss.inputFieldScaleY.onValueChanged.AddListener((str) =>
        {
            float.TryParse(settingMiss.inputFieldScaleY.text, out graphPainterMiss.canvasUpScale.y);
            if (graphPainterMiss.canvasUpScale.y == 0)
            {
                graphPainterMiss.canvasUpScale.y = 1;
                settingMiss.inputFieldScaleY.text = graphPainterMiss.canvasUpScale.y.ToString();
            }
            GlobalSetting.G_KGRAPH_CANVAS_SCALE_Y = graphPainterMiss.canvasUpScale.y;
            NotifyUIRepaint();
        });

        settingMiss.toggleShowSingleLine.isOn = graphPainterMiss.onlyShowSelectCDTLine;
        settingMiss.toggleShowSingleLine.onValueChanged.AddListener((b) =>
        {
            graphPainterMiss.onlyShowSelectCDTLine = settingMiss.toggleShowSingleLine.isOn;
            NotifyUIRepaint();
        });

        settingMiss.dropdownMissType.AddOptions(GraphDataManager.S_MissCountTypeStrs);
        settingMiss.dropdownMissType.value = (int)graphPainterMiss.missCountType;
        settingMiss.dropdownMissType.onValueChanged.AddListener((i) => 
        {
            graphPainterMiss.missCountType = (MissCountType)(settingMiss.dropdownMissType.value);
            NotifyUIRepaint();
        });

        settingMiss.dropdownAppearenceType.AddOptions(GraphDataManager.S_AppearenceTypeStrs);
        settingMiss.dropdownAppearenceType.value = (int)graphPainterMiss.appearenceType;
        settingMiss.dropdownAppearenceType.onValueChanged.AddListener((i) =>
        {
            graphPainterMiss.appearenceType = (AppearenceType)(settingMiss.dropdownAppearenceType.value);
            NotifyUIRepaint();
        });

        for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
        {
            CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
            graphPainterMiss.cdtLineShowStates.Add(cdt, false);
            CdtShowStateItem item = settingMiss.cdtShowStateView.AddItem("", cdt) as CdtShowStateItem;
            item.label.text = GraphDataManager.S_CDT_TAG_LIST[i];
            item.toggleShow.isOn = false;
            item.imgColor.color = GraphDataManager.GetColorByCDT(cdt);
            item.toggleShow.onValueChanged.AddListener((b) =>
            {
                graphPainterMiss.cdtLineShowStates[cdt] = item.toggleShow.isOn;
                NotifyUIRepaint();
            });
        }

        settingMiss.settingPanelMiss.SetActive(false);
    }

    public void OnSelectedDataItemChanged()
    {
        GraphDataManager.BGDC.CurrentSelectItem = DataManager.GetInst().FindDataItem(SelectKDataIndex);
        GraphDataManager.Instance.CollectGraphData(GraphType.eBarGraph);
        NotifyUIRepaint();
    }

    // Update is called once per frame
    void Update()
    {
        curGraphPainter.Update();
    }

    public void SetSliderValue(float v)
    {
        needUpdateScrollToSpecDataItem = false;
        fastViewSlider.value = v;
    }

    public void NotifyUIRepaint()
    {
        notifyGraphsUpdateFrame = Time.frameCount;
    }

    public void LateUpdate()
    {
        if(notifyGraphsUpdateFrame > 0)
        {
            if (Time.frameCount > notifyGraphsUpdateFrame)
            {
                PanelAnalyze.Instance.graphUp.SetVerticesDirty();
                PanelAnalyze.Instance.graphDown.SetVerticesDirty();
                notifyGraphsUpdateFrame = -1;
            }
        }
    }

    public void SetCurrentGraph(GraphPainterBase g)
    {
        curGraphPainter = g;
        fastViewSlider.minValue = 0;
        if (curGraphPainter is GraphPainterKData)
        {
            fastViewSlider.maxValue = GraphDataManager.Instance.DataLength(GraphType.eKCurveGraph);
            settingKGraph.settingPanelKData.SetActive(true);
            settingBar.settingPanelBar.SetActive(false);
            settingMiss.settingPanelMiss.SetActive(false);
        }
        else if(curGraphPainter is GraphPainterBar)
        {
            int max = DataManager.GetInst().GetAllDataItemCount();
            fastViewSlider.maxValue = max;
            settingKGraph.settingPanelKData.SetActive(false);
            settingBar.settingPanelBar.SetActive(true);
            settingMiss.settingPanelMiss.SetActive(false);
        }
        else if(curGraphPainter is GraphPainterMiss)
        {
            int max = DataManager.GetInst().GetAllDataItemCount();
            fastViewSlider.maxValue = max;
            settingKGraph.settingPanelKData.SetActive(false);
            settingBar.settingPanelBar.SetActive(false);
            settingMiss.settingPanelMiss.SetActive(true);
        }
        fastViewSlider.value = Mathf.Clamp(fastViewSlider.value, fastViewSlider.minValue, fastViewSlider.maxValue);
    }

    #region call back

    public void OnBtnClickAutoAllign()
    {
        curGraphPainter.OnAutoAllign();
    }

    public void OnClickItemBestPosPath(int index)
    {
        object o = settingKGraph.listviewBestPosPath.SelectItemTag;
        if (o != null)
        {
            PosPathTag tag = o as PosPathTag;
            numIndex = tag.numIndex;
            cdt = GraphDataManager.S_CDT_LIST[tag.cdtIndex];
            settingCommon.dropdownCDT.value = tag.cdtIndex;
            settingCommon.dropdownNumIndex.value = numIndex;
            OnBtnClickAutoAllign();
        }
    }

    public void OnBtnClickRefreshData()
    {
        PanelDataView.Instance.RefreshLatestDataFromSpecDate();
        NotifyUIRepaint();
    }

    public void OnBtnClickClose()
    {
        gameObject.SetActive(false);
    }

    public void OnBtnClickFastViewSlider()
    {
        fastViewSlider.gameObject.SetActive(!fastViewSlider.gameObject.activeSelf);
        fastViewSlider.transform.parent.gameObject.SetActive(fastViewSlider.gameObject.activeSelf);
    }

    public void OnSliderFastViewChange(int v)
    {
        int index = (int)fastViewSlider.value;
        if (needUpdateScrollToSpecDataItem)
        {
            graghPainterKData.OnScrollToData(index);
            graphPainterBar.OnScrollToData(index);
            graphPainterMiss.OnScrollToData(index);
            curGraphPainter.OnAutoAllign();
        }
        else
        {
            if (graghPainterKData != curGraphPainter)
                graghPainterKData.OnScrollToData(index);
            if (graphPainterBar != curGraphPainter)
                graphPainterBar.OnScrollToData(index);
            if (graphPainterMiss != curGraphPainter)
                graphPainterMiss.OnScrollToData(index);
            needUpdateScrollToSpecDataItem = true;
        }
    }

    public void OnBtnClickSetting()
    {
        GameObject settingParent = settingCommon.settingPanelCommon.transform.parent.gameObject;
        settingParent.SetActive(!settingParent.activeSelf);
    }

    void AddPosPath(int numIndex, int cdtIndex)
    {
        string item = KDataDictContainer.C_TAGS[numIndex] + "_" + GraphDataManager.S_CDT_TAG_LIST[cdtIndex];
        if (settingKGraph.listviewBestPosPath.FindItem(item) == null)
        {
            settingKGraph.listviewBestPosPath.AddItem(item, new PosPathTag(numIndex, cdtIndex));
        }
    }

    public void OnBtnClickKGraph()
    {
        SetCurrentGraph(graghPainterKData);
        NotifyUIRepaint();
    }

    public void OnBtnClickBarGraph()
    {
        SetCurrentGraph(graphPainterBar);
        NotifyUIRepaint();
    }

    public void OnBtnClickMissGraph()
    {
        SetCurrentGraph(graphPainterMiss);
        NotifyUIRepaint();
    }

    #endregion

    #region util

    public Vector2 ConvertPosToPainter(Vector2 pointerPos, GraphBase graph)
    {
        Vector2 pos = pointerPos;
        if(graph == this.graphUp)
        {
            pos.y = pos.y - splitPanel.rtSplitter.rect.height - graphDown.rectTransform.rect.height;
        }
        return pos;
    }

    #endregion
}
