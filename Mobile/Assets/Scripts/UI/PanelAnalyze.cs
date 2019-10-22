using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PanelAnalyze : MonoBehaviour
{
    [System.Serializable]
    public class SettingKGraph
    {
        public GameObject settingPanelKData;
        public UnityEngine.UI.InputField inputFieldScaleX;
        public UnityEngine.UI.InputField inputFieldScaleY;
        public UnityEngine.UI.Dropdown dropdownNumIndex;
        public UnityEngine.UI.Dropdown dropdownCDT;
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

        public ListView listviewBestPosPath;
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

    public SettingKGraph settingKGraph;

    GraphPainterKData graghPainterKData = new GraphPainterKData();
    public GraphPainterBase curGraphPainter;

    public int numIndex = 0;
    public CollectDataType cdt = CollectDataType.ePath0;
    public int curCycleIndex = 0;
    public int endShowDataItemIndex = -1;

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
        SetCurrentGraph(graghPainterKData);
        graghPainterKData.Start();
        InitSettingPanelKData();
    }

    void InitSettingPanelKData()
    {
        settingKGraph.inputFieldScaleX.text = graghPainterKData.canvasUpScale.x.ToString();
        settingKGraph.inputFieldScaleX.onValueChanged.AddListener((str) => 
        {
            float.TryParse(settingKGraph.inputFieldScaleX.text, out graghPainterKData.canvasUpScale.x);
            graghPainterKData.canvasDownScale.x = graghPainterKData.canvasUpScale.x;
            NotifyUIRepaint();
        });
        settingKGraph.inputFieldScaleY.text = graghPainterKData.canvasUpScale.y.ToString();
        settingKGraph.inputFieldScaleY.onValueChanged.AddListener((str) =>
        {
            float.TryParse(settingKGraph.inputFieldScaleY.text, out graghPainterKData.canvasUpScale.y);
            NotifyUIRepaint();
        });

        settingKGraph.dropdownNumIndex.AddOptions(KDataDictContainer.C_TAGS);
        settingKGraph.dropdownNumIndex.value = numIndex;
        settingKGraph.dropdownNumIndex.onValueChanged.AddListener((v) => 
        {
            numIndex = settingKGraph.dropdownNumIndex.value;
            NotifyUIRepaint();
        });

        settingKGraph.dropdownCDT.AddOptions(GraphDataManager.S_CDT_TAG_LIST);
        settingKGraph.dropdownCDT.value = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
        settingKGraph.dropdownCDT.onValueChanged.AddListener((v) =>
        {
            cdt = GraphDataManager.S_CDT_LIST[settingKGraph.dropdownCDT.value];
            NotifyUIRepaint();
        });

        settingKGraph.dropdownCycle.AddOptions(GraphDataManager.G_Circles_STRs);
        settingKGraph.dropdownCycle.value = curCycleIndex;
        settingKGraph.dropdownCycle.onValueChanged.AddListener((v) =>
        {
            curCycleIndex = settingKGraph.dropdownCycle.value;
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

        settingKGraph.settingPanelKData.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        curGraphPainter.Update();
    }

    public void SetSliderValue(float v)
    {
        fastViewSlider.value = v;
    }

    public void NotifyUIRepaint()
    {
        PanelAnalyze.Instance.graphUp.SetVerticesDirty();
        PanelAnalyze.Instance.graphDown.SetVerticesDirty();
    }

    public void SetCurrentGraph(GraphPainterBase g)
    {
        curGraphPainter = g;
        if(curGraphPainter is GraphPainterKData)
        {
            fastViewSlider.minValue = 0;
            fastViewSlider.maxValue = GraphDataManager.Instance.DataLength(GraphType.eKCurveGraph);
        }
        fastViewSlider.value = Mathf.Clamp(fastViewSlider.value, fastViewSlider.minValue, fastViewSlider.maxValue);
    }

    #region call back

    public void OnClickItemBestPosPath(int index)
    {
        object o = settingKGraph.listviewBestPosPath.SelectItemTag;
        if (o != null)
        {
            PosPathTag tag = o as PosPathTag;
            numIndex = tag.numIndex;
            settingKGraph.dropdownCDT.value = tag.cdtIndex;
            settingKGraph.dropdownNumIndex.value = numIndex;
            NotifyUIRepaint();
        }
    }

    public void OnBtnClickRefreshData()
    {

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
        curGraphPainter.OnScrollToData((int)fastViewSlider.value);
    }

    public void OnBtnClickSetting()
    {
        if(curGraphPainter is GraphPainterKData)
        {
            settingKGraph.settingPanelKData.SetActive(!settingKGraph.settingPanelKData.activeSelf);
        }
    }

    public void OnBtnClickAutoCalcBestPosPath()
    {
        settingKGraph.listviewBestPosPath.ClearAllItems();
        List<TradeDataManager.NumCDT> results = TradeDataManager.Instance.CalcFavorits(null);
        for (int i = 0; i < results.Count; ++i)
        {
            AddPosPath(results[i].numID, results[i].cdtID);
        }
    }

    void AddPosPath(int numIndex, int cdtIndex)
    {
        string item = KDataDictContainer.C_TAGS[numIndex] + "_" + GraphDataManager.S_CDT_TAG_LIST[cdtIndex];
        if (settingKGraph.listviewBestPosPath.FindItem(item) == null)
        {
            settingKGraph.listviewBestPosPath.AddItem(item, new PosPathTag(numIndex, cdtIndex));
        }
    }

    public void OnBtnClickAddPosPath()
    {
        int cdtIndex = settingKGraph.dropdownCDT.value;
        AddPosPath(numIndex, cdtIndex);
    }

    public void OnBtnClickDelSelPosPath()
    {
        settingKGraph.listviewBestPosPath.RemoveItem(settingKGraph.listviewBestPosPath.SelIndex);
    }

    public void OnBtnClickClearAllPosPath()
    {
        settingKGraph.listviewBestPosPath.ClearAllItems();
    }

    #endregion
}
