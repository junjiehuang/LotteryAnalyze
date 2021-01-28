using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTrade : MonoBehaviour
{
    public class SingleTradeInfo
    {
        public int tradeID;
        public string lastDataItemIdTag;
        public string lastDataItemNums;
        public string targetDataItemIdTag;
        public string targetDataItemNums;
        public string extraInfo;
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
                    "目标期: " + lastDataItemIdTag + " 出号: " + lastDataItemNums + "\n" +
                    "结果期: " + targetDataItemIdTag + " 出号: " + targetDataItemNums + "\n";
                if (string.IsNullOrEmpty(extraInfo) == false)
                {
                    info += "判断数据 : \n" + extraInfo + "\n";
                }

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

        public bool GetTradeNumIndexAndPathIndex(ref int numID, ref int pathID)
        {
            if(tradeDetail.Count > 0)
            {
                var itor = tradeDetail.GetEnumerator();
                while(itor.MoveNext())
                {
                    TradeNumbers tn = itor.Current.Value;
                    if(tn.tradeCount > 0)
                    {
                        numID = itor.Current.Key;
                        pathID = tn.tradeNumbers[0].number % 3;
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public List<SingleTradeInfo> allTradeInfos = new List<SingleTradeInfo>();

    float ITEM_W = 0;
    float ITEM_H = 0;
    // 统计连续交易失败的次数
    List<TradeItem> statisticItems = new List<TradeItem>();
    List<TradeItem> freePool = new List<TradeItem>();
    List<ItemWrapper> treeInfos = new List<ItemWrapper>();
    Dictionary<int, int> tradeMissCountIndexMap = new Dictionary<int, int>();
    //Dictionary<int, int> continueWrongTradeInfos = new Dictionary<int, int>();
    // 当前连续交易失败的次数
    public int currentMissTradeCount = 0;
    // 总有效交易的次数
    public int totalValidTradeCount = 0;
    // 总忽略交易的次数
    public int totalIgnoreTradeCount = 0;

    [System.Serializable]
    public class UIMain
    {
        public Button btnClose;
        public Button btnToggleTradeSettingView;
        public Button btnStart;
        public Button btnPause;
        public Button btnStop;
        public Button btnViewBrief;

        public Text txtBtnPause;
        public RectTransform rtProgressLocal;
        public RectTransform rtProgressGlobal;
        public GameObject briefView;
        //public Text txtBriefView;

        public RectTransform rtContent;
        public RectTransform rtPrefab;

        public GameObject tradeInfoBG;
        public Button toggleTradeInfoBG;
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
        ITEM_W = uiMain.rtPrefab.rect.width;
        ITEM_H = uiMain.rtPrefab.rect.height;
        if(ITEM_W == 0)
        {
            ITEM_W = (uiMain.briefView.transform as RectTransform).rect.width;
        }

        uiMain.rtPrefab.gameObject.SetActive(false);
        freePool.Add(uiMain.rtPrefab.GetComponent<TradeItem>());

        //uiMain.txtBriefView.text = "";
        uiMain.briefView.SetActive(false);
        uiMain.btnViewBrief.onClick.AddListener(() =>
        {
            uiMain.briefView.SetActive(!uiMain.briefView.activeSelf);
        });

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

        uiSetting.toggle1.isOn = GlobalSetting.G_SIM_SEL_NUM_AT_POS_0;
        uiSetting.toggle1.onValueChanged.AddListener((v) =>
        {
            GlobalSetting.G_SIM_SEL_NUM_AT_POS_0 = uiSetting.toggle1.isOn;
        });
        uiSetting.toggle2.isOn = GlobalSetting.G_SIM_SEL_NUM_AT_POS_1;
        uiSetting.toggle2.onValueChanged.AddListener((v) =>
        {
            GlobalSetting.G_SIM_SEL_NUM_AT_POS_1 = uiSetting.toggle2.isOn;
        });
        uiSetting.toggle3.isOn = GlobalSetting.G_SIM_SEL_NUM_AT_POS_2;
        uiSetting.toggle3.onValueChanged.AddListener((v) =>
        {
            GlobalSetting.G_SIM_SEL_NUM_AT_POS_2 = uiSetting.toggle3.isOn;
        });
        uiSetting.toggle4.isOn = GlobalSetting.G_SIM_SEL_NUM_AT_POS_3;
        uiSetting.toggle4.onValueChanged.AddListener((v) =>
        {
            GlobalSetting.G_SIM_SEL_NUM_AT_POS_3 = uiSetting.toggle4.isOn;
        });
        uiSetting.toggle5.isOn = GlobalSetting.G_SIM_SEL_NUM_AT_POS_4;
        uiSetting.toggle5.onValueChanged.AddListener((v) =>
        {
            GlobalSetting.G_SIM_SEL_NUM_AT_POS_4 = uiSetting.toggle5.isOn;
        });

        uiSetting.inputTradeCountLst.text = GlobalSetting.G_SIM_TRADE_PLANS;// "1,2,4,8,16,32,64,128";
        uiSetting.inputTradeCountLst.onEndEdit.AddListener((str) =>
        {
            GlobalSetting.G_SIM_TRADE_PLANS = uiSetting.inputTradeCountLst.text;
        });

        uiSetting.dropdownTradeStratedy.AddOptions(TradeDataManager.STRATEGY_NAMES);
        uiSetting.dropdownTradeStratedy.value = (int)GlobalSetting.G_SIM_STRETAGY;
        uiSetting.dropdownTradeStratedy.onValueChanged.AddListener((v) =>
        {
            GlobalSetting.G_SIM_STRETAGY = (TradeDataManager.TradeStrategy)(uiSetting.dropdownTradeStratedy.value);
        });

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
                    string idtag = info.targetDataItemIdTag;
                    int dateID = int.Parse(idtag.Split('-')[0]);
                    PanelDataView.Instance.ReadSpecDateData(idtag, dateID);
                    int numID = 0, pathID = 0;
                    if(info.GetTradeNumIndexAndPathIndex(ref numID, ref pathID))
                    {
                        PanelAnalyze.Instance.ChangeNumIndexAndCdtIndex(numID, pathID);
                    }
                    return;
                }
            }
        });

        if(uiMain.tradeInfoBG != null)
        {
            uiMain.tradeInfoBG.SetActive(false);
        }
        if(uiMain.toggleTradeInfoBG != null)
        {
            uiMain.toggleTradeInfoBG.onClick.AddListener(() => {
                if(uiMain.tradeInfoBG != null)
                {
                    uiMain.tradeInfoBG.SetActive(!uiMain.tradeInfoBG.activeSelf);
                }
            });
        }

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
        curPainter.Reset();

        BatchTradeSimulator.Instance.batch = GlobalSetting.G_DAYS_PER_BATCH;
        BatchTradeSimulator.Instance.startMoney = float.Parse(uiSetting.inputStartMoney.text);
        TradeDataManager.Instance.curTradeStrategy = GlobalSetting.G_SIM_STRETAGY;
        TradeDataManager.Instance.SetTradeCountInfo(uiSetting.inputTradeCountLst.text);
        
        uiMain.txtBtnPause.text = "暂停";
        BatchTradeSimulator.Instance.Start(ref startDate, ref endDate);

        uiSetting.inputStartDate.text = startDate.ToString();
        uiSetting.inputEndDate.text = endDate.ToString();

        allTradeInfos.Clear();
        //continueWrongTradeInfos.Clear();
        treeInfos.Clear();
        tradeMissCountIndexMap.Clear();
        currentMissTradeCount = 0;
        totalIgnoreTradeCount = 0;
        totalValidTradeCount = 0;

        NotifyRepaint();
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

        allTradeInfos.Clear();
        //continueWrongTradeInfos.Clear();
        treeInfos.Clear();
        tradeMissCountIndexMap.Clear();
        currentMissTradeCount = 0;
        totalIgnoreTradeCount = 0;
        totalValidTradeCount = 0;

        NotifyRepaint();
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
        info.tradeID = trade.INDEX;
        info.lastDataItemIdTag = trade.lastDateItem.idTag;
        info.lastDataItemNums = trade.lastDateItem.lotteryNumber;
        info.targetDataItemIdTag = trade.targetLotteryItem.idTag;
        info.targetDataItemNums = trade.targetLotteryItem.lotteryNumber;
        info.moneyLeft = trade.moneyAtferTrade;
        info.cost = trade.cost;
        info.reward = trade.reward;
        info.extraInfo = trade.GetDbgInfo();
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

        uiTrade.sliderSelectTradeItem.minValue = 0;
        uiTrade.sliderSelectTradeItem.maxValue = allTradeInfos.Count - 1;
        uiTrade.sliderSelectTradeItem.value = uiTrade.sliderSelectTradeItem.maxValue;

        curPainter.selectedIndex = allTradeInfos.Count - 1;
        curPainter.ScrollLatestItemToMiddle(curPainter.upPainter, uiTrade.graphTrade.rectTransform);

        if(trade.reward > 0)
        {
            RecordMissWrongTradeInfo();
            ++totalValidTradeCount;
        }
        else if(trade.cost > 0)
        {
            ++currentMissTradeCount;
            ++totalValidTradeCount;
        }
        else
        {
            ++totalIgnoreTradeCount;
        }
        RefreshBriefView();
        //NotifyRepaint();
    }

    void RecordMissWrongTradeInfo()
    {
        //if(continueWrongTradeInfos.ContainsKey(currentMissTradeCount))
        //{
        //    continueWrongTradeInfos[currentMissTradeCount] = continueWrongTradeInfos[currentMissTradeCount] + 1;
        //}
        //else
        //{
        //    continueWrongTradeInfos.Add(currentMissTradeCount, 1);
        //}

        ItemWrapper iwParent = null;
        if (tradeMissCountIndexMap.ContainsKey(currentMissTradeCount) == false)
        {
            iwParent = new ItemWrapper(currentMissTradeCount.ToString());
            iwParent.Tag = currentMissTradeCount;
            iwParent.showViewBtn = false;
            iwParent.Expand = false;
            treeInfos.Add(iwParent);

            treeInfos.Sort((a, b) => 
            {
                if ((int)a.Tag > (int)b.Tag)
                    return -1;
                return 1;
            });
            tradeMissCountIndexMap.Clear();
            for(int i = 0; i < treeInfos.Count; ++i)
            {
                tradeMissCountIndexMap.Add((int)treeInfos[i].Tag, i);
            }
        }
        else
        {
            iwParent = treeInfos[tradeMissCountIndexMap[currentMissTradeCount]];
        }

        int tradeID = allTradeInfos.Count - 1;
        ItemWrapper iwChild = new ItemWrapper(tradeID.ToString());
        iwChild.isLeafNode = true;
        iwChild.showViewBtn = true;
        iwChild.Tag = tradeID;
        iwParent.SubNodes.Add(iwChild);

        currentMissTradeCount = 0;
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

    TradeItem CreateItem(ItemWrapper item)
    {
        TradeItem si = null;
        if (freePool.Count > 0)
        {
            si = freePool[freePool.Count - 1];
            freePool.RemoveAt(freePool.Count - 1);
        }
        else
        {
            GameObject go = GameObject.Instantiate(uiMain.rtPrefab.gameObject);
            si = go.GetComponent<TradeItem>();
        }
        si.gameObject.SetActive(true);
        si.rtSelf.SetParent(uiMain.rtContent, true);
        statisticItems.Add(si);
        si.SetItem(item);
        si.rtSelf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ITEM_W);
        si.rtSelf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ITEM_H);
        return si;
    }

    public void RefreshView()
    {
        RecycleAllItems();

        float height = 0;
        int layer = 0;
        for (int i = 0; i < treeInfos.Count; ++i)
        {
            RefreshItem(treeInfos[i], ref height, ref layer);
        }
        uiMain.rtContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void RefreshItem(ItemWrapper item, ref float height, ref int layer)
    {
        TradeItem si = CreateItem(item);
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


    //List<int> keys = new List<int>();
    void RefreshBriefView()
    {
        RefreshView();
        //string info = "";
        //keys.Clear();
        //foreach(int key in continueWrongTradeInfos.Keys)
        //{
        //    keys.Add(key);
        //}
        //keys.Sort((a, b) =>
        //{
        //    if (a > b) return -1;
        //    return 1;
        //});
        //for(int i = 0; i < keys.Count; ++i)
        //{
        //    info += keys[i] + "\t" + continueWrongTradeInfos[keys[i]] + "/" + totalValidTradeCount + "\n";
        //}
        //uiMain.txtBriefView.text = info;
    }

    public void ScrollToData(int tradeIndex)
    {
        curPainter.selectedIndex = tradeIndex;
        curPainter.OnScrollToData(tradeIndex);
    }
}
