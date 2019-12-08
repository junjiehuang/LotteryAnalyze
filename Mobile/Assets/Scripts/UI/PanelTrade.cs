using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTrade : MonoBehaviour
{
    public class SingleTradeInfo
    {
        public string lastDataItemIdTag;
        public string targetDataItemIdTag;
        public float moneyLeft;
        public float cost;
        public float reward;
        public Dictionary<int, TradeNumbers> tradeDetail;

        string info;
        public string GetDetailInfo()
        {
            if (info == null)
            {
                info =
                    "目标期：" + lastDataItemIdTag + "\n" +
                    "结果期：" + targetDataItemIdTag + "\n";
                if (tradeDetail == null)
                {
                }
                else
                {
                    info += "交易详情：\n";
                    var itor = tradeDetail.GetEnumerator();
                    while (itor.MoveNext())
                    {
                        string tns = "";
                        itor.Current.Value.GetInfo(ref tns);
                        info += KDataDictContainer.C_TAGS[itor.Current.Key] + tns;
                    }
                }
            }
            return info;
        }
    }
    public List<SingleTradeInfo> allTradeInfos = new List<SingleTradeInfo>();

    [System.Serializable]
    public class UIMain
    {
        public Button btnClose;
        public Button btnToggleTradeSettingView;
        public Button btnStart;
        public Button btnPause;
        public Button btnStop;

        public Text txtBtnPause;
        public RectTransform rtProgressLocal;
        public RectTransform rtProgressGlobal;
    }

    [System.Serializable]
    public class UISetting
    {
        public Transform rtSettingView;
        public InputField inputStartDate;
        public InputField inputEndDate;
        public InputField inputStartMoney;

        public InputField inputTradeCountLst;

        public Dropdown dropdownTradeStratedy;

        public Toggle toggle1;
        public Toggle toggle2;
        public Toggle toggle3;
        public Toggle toggle4;
        public Toggle toggle5;
    }

    [System.Serializable]
    public class UITrade
    {
        public GraphBase graphTrade;
        public Slider sliderSelectTradeItem;
        public ScrollBar scrollZoom;
        public Button btnViewDataItem;
    }

    public UIMain uiMain;
    public UISetting uiSetting;
    public UITrade uiTrade;

    public float ProgreeBarMaxW;
    int startDate = -1, endDate = -1;
    public int StartDate
    {
        get { return startDate; }
        set { startDate = value; uiSetting.inputStartDate.text = value.ToString(); }
    }
    public int EndDate
    {
        get { return endDate; }
        set { endDate = value; uiSetting.inputEndDate.text = value.ToString(); }
    }

    bool needRepaint = false;
    GraphPainterTrade curPainter = new GraphPainterTrade();

    static PanelTrade sInst;
    public static PanelTrade Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;
    }

    void Init()
    {
        ProgreeBarMaxW = (uiMain.rtProgressGlobal.parent as RectTransform).rect.width;
        uiMain.rtProgressGlobal.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        uiMain.rtProgressLocal.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);

        uiMain.txtBtnPause = uiMain.btnPause.GetComponentInChildren<Text>();
        uiMain.btnClose.onClick.AddListener(() =>
        {
            LotteryManager.SetActive(gameObject, false);
        });

        uiMain.btnPause.onClick.AddListener(() =>
        {
            if (BatchTradeSimulator.Instance.IsSimulating)
            {
                if (BatchTradeSimulator.Instance.IsPause())
                {
                    DoResume();
                    uiMain.txtBtnPause.text = "暂停";
                }
                else
                {
                    DoPause();
                    uiMain.txtBtnPause.text = "恢复";
                }
            }
        });

        uiMain.btnStart.onClick.AddListener(() =>
        {
            if (BatchTradeSimulator.Instance.IsSimulating == false)
            {
                DoStart();
                uiMain.txtBtnPause.text = "暂停";
            }
        });

        uiMain.btnStop.onClick.AddListener(() =>
        {
            DoStop();
        });

        uiMain.btnToggleTradeSettingView.onClick.AddListener(() =>
        {
            uiSetting.rtSettingView.gameObject.SetActive(!uiSetting.rtSettingView.gameObject.activeSelf);
        });

        uiSetting.rtSettingView.gameObject.SetActive(false);
        uiSetting.inputStartMoney.text = BatchTradeSimulator.Instance.startMoney.ToString();

        uiSetting.toggle1.isOn = true;
        uiSetting.toggle2.isOn = true;
        uiSetting.toggle3.isOn = true;
        uiSetting.toggle4.isOn = true;
        uiSetting.toggle5.isOn = true;

        uiSetting.inputTradeCountLst.text = "1,2,4,8,16,32,64,128";

        uiSetting.dropdownTradeStratedy.AddOptions(TradeDataManager.STRATEGY_NAMES);
        uiSetting.dropdownTradeStratedy.value = 0;

        float ratio = GlobalSetting.G_TRADE_CANVAS_SCALE_X / GlobalSetting.G_TRADE_CANVAS_SCALE_X_MAX;
        uiTrade.scrollZoom.SetHandleRatio(0.2f);
        uiTrade.scrollZoom.SetProgress(ratio);
        uiTrade.scrollZoom.onScrollChange += () =>
        {
            GlobalSetting.G_TRADE_CANVAS_SCALE_X = uiTrade.scrollZoom.GetProgress() * GlobalSetting.G_TRADE_CANVAS_SCALE_X_MAX;
            if (GlobalSetting.G_TRADE_CANVAS_SCALE_X < 0.01f)
                GlobalSetting.G_TRADE_CANVAS_SCALE_X = 0.01f;
            curPainter.OnGraphZoom(curPainter.upPainter);
            NotifyRepaint();
        };

        uiTrade.sliderSelectTradeItem.onValueChanged.AddListener((v) =>
        {
            curPainter.OnScrollToData(uiTrade.sliderSelectTradeItem.value);
        });

        uiTrade.btnViewDataItem.onClick.AddListener(() =>
        {
            if (curPainter.selectedIndex >= 0 && allTradeInfos.Count > curPainter.selectedIndex)
            {
                SingleTradeInfo info = allTradeInfos[curPainter.selectedIndex];
                if (info != null)
                {
                    string idtag = info.lastDataItemIdTag;
                    int dateID = int.Parse(idtag.Split('-')[0]);
                    PanelDataView.Instance.ReadSpecDateData(idtag, dateID);
                    return;
                }
            }
        });

        TradeDataManager.Instance.evtOneTradeCompleted += OnOneTradeCompleted;
    }


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        DoTradeUpdate();
        curPainter.Update();
    }

    private void LateUpdate()
    {
        if (needRepaint)
        {
            uiTrade.graphTrade.SetVerticesDirty();
            needRepaint = false;
        }
    }

    public void NotifyRepaint()
    {
        needRepaint = true;
    }

    void DoStart()
    {
        BatchTradeSimulator.Instance.batch = GlobalSetting.G_DAYS_PER_BATCH;
        BatchTradeSimulator.Instance.startMoney = float.Parse(uiSetting.inputStartMoney.text);
        TradeDataManager.Instance.SetTradeCountInfo(uiSetting.inputTradeCountLst.text);
        
        uiMain.txtBtnPause.text = "暂停";
        BatchTradeSimulator.Instance.Start(ref startDate, ref endDate);

        uiSetting.inputStartDate.text = startDate.ToString();
        uiSetting.inputEndDate.text = endDate.ToString();

        allTradeInfos.Clear();
    }

    void DoPause()
    {
        BatchTradeSimulator.Instance.Pause();
    }

    void DoResume()
    {
        BatchTradeSimulator.Instance.Resume();
    }

    void DoStop()
    {
        BatchTradeSimulator.Instance.Stop();
        // 重置K线的起点值
        GraphDataManager.ResetCurKValueMap();
    }

    void SetProgress(float local, float globalV)
    {
        uiMain.rtProgressLocal.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ProgreeBarMaxW * local);
        uiMain.rtProgressGlobal.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ProgreeBarMaxW * globalV);
    }

    void DoTradeUpdate()
    {
        TradeDataManager.Instance.Update();
        BatchTradeSimulator.Instance.Update();

        if(BatchTradeSimulator.Instance.IsSimulating)
        {
            float vl = BatchTradeSimulator.Instance.GetBatchProgress() / 100.0f;
            float vg = BatchTradeSimulator.Instance.GetMainProgress() / 100.0f;
            SetProgress(vl, vg);
        }
        else
        {
            SetProgress(0, 0);
        }
    }

    void OnOneTradeCompleted(TradeDataBase _trade)
    {
        TradeDataOneStar trade = _trade as TradeDataOneStar;
        SingleTradeInfo info = new SingleTradeInfo();
        info.lastDataItemIdTag = trade.lastDateItem.idTag;
        info.targetDataItemIdTag = trade.targetLotteryItem.idTag;
        info.moneyLeft = trade.moneyAtferTrade;
        info.cost = trade.cost;
        info.reward = trade.reward;
        if (trade.tradeInfo.Count > 0)
        {
            info.tradeDetail = new Dictionary<int, TradeNumbers>();
            var itor = trade.tradeInfo.GetEnumerator();
            while(itor.MoveNext())
            {
                int numID = itor.Current.Key;
                TradeNumbers srcTN = itor.Current.Value;
                TradeNumbers tarTN = new TradeNumbers();
                tarTN.CopyFrom(srcTN);
                info.tradeDetail.Add(numID, tarTN);
            }
        }
        allTradeInfos.Add(info);
        NotifyRepaint();
    }
}
