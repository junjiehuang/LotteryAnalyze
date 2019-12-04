using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTrade : MonoBehaviour
{
    [System.Serializable]
    public class UIMain
    {
        public Button btnClose;
        public Button btnToggleTradeSettingView;
        public Button btnStart;
        public Button btnPause;
        public Button btnStop;
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

    static PanelTrade sInst;
    public static PanelTrade Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;

        Init();
    }

    void Init()
    {
        uiMain.btnClose.onClick.AddListener(() =>
        {
            LotteryManager.SetActive(gameObject, false);
        });

        uiMain.btnPause.onClick.AddListener(() =>
        {

        });

        uiMain.btnStart.onClick.AddListener(() =>
        {

        });

        uiMain.btnStop.onClick.AddListener(() =>
        {

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
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
