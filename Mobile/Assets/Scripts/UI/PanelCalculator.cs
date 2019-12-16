using LotteryAnalyze;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCalculator : MonoBehaviour
{
    static PanelCalculator sInst;
    public static PanelCalculator Instance
    {
        get { return sInst; }
    }
    private void Awake()
    {
        sInst = this;
    }




    public InputField inputfieldCountPerTrade;
    public InputField inputfieldReward;
    public InputField inputfieldCost;

    public InputField inputfieldTradeCount;
    public InputField inputfieldGenParam;
    public Dropdown dropdownGenType;

    public Button btnCalcByPlan;
    public Button btnGenPlanAndCalc;
    public Button btnClose;

    public InputField inputfieldTradePlan;

    public GameObject itemPrefab;

    public RectTransform rtContent;

    List<TradeCalcItem> inuseItems = new List<TradeCalcItem>();
    List<TradeCalcItem> freeItems = new List<TradeCalcItem>();
    RectTransform rtItemPrefab;

    int numberCountPerTrade = 3;
    float reward = 9.95f;
    float cost = 1f;

    int planCount = 5;
    int genParam = 0;
    GenerateType genType = GenerateType.eMinCost;


    // Start is called before the first frame update
    void Start()
    {
        inputfieldTradePlan.text = GlobalSetting.G_SIM_TRADE_PLANS;
        inputfieldTradePlan.onEndEdit.AddListener((str) =>
        {
            GlobalSetting.G_SIM_TRADE_PLANS = inputfieldTradePlan.text;
        });

        inputfieldCountPerTrade.text = numberCountPerTrade.ToString();
        inputfieldReward.text = reward.ToString();
        inputfieldCost.text = cost.ToString();
        inputfieldTradeCount.text = planCount.ToString();
        inputfieldGenParam.text = genParam.ToString();

        dropdownGenType.AddOptions(TradeDataManager.S_GenerateTypeStr);
        dropdownGenType.value = (int)genType;
        dropdownGenType.onValueChanged.AddListener((v) =>
        {
            genType = (GenerateType)dropdownGenType.value;
        });

        btnCalcByPlan.onClick.AddListener(OnBtnClickCalcByPlan);
        btnGenPlanAndCalc.onClick.AddListener(OnBtnClickGenPlanAndCalc);
        btnClose.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        itemPrefab.SetActive(false);
        freeItems.Add(itemPrefab.GetComponent<TradeCalcItem>());
        rtContent = itemPrefab.transform.parent as RectTransform;
        rtItemPrefab = itemPrefab.transform as RectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClearItems()
    {
        for(int i = 0; i < inuseItems.Count; ++i)
        {
            freeItems.Add(inuseItems[i]);
            inuseItems[i].gameObject.SetActive(false);
        }
        inuseItems.Clear();
    }

    void AddItem(int id, int count, float cost, float reward, float profit)
    {
        TradeCalcItem item = null;
        if(freeItems.Count > 0)
        {
            item = freeItems[0];
            freeItems.RemoveAt(0);
        }
        else
        {
            GameObject go = GameObject.Instantiate(itemPrefab);
            item = go.GetComponent<TradeCalcItem>();
        }
        item.gameObject.SetActive(true);
        float itemHeight = rtItemPrefab.rect.height;
        float itemWidth = rtItemPrefab.rect.width;
        item.rectTransform.SetParent(rtContent);
        item.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemWidth);
        item.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);
        item.rectTransform.anchoredPosition = new Vector2(0, -inuseItems.Count * itemHeight);
        item.SetInfo(id.ToString(), count.ToString(), cost.ToString(), reward.ToString(), profit.ToString());
        inuseItems.Add(item);
        rtContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inuseItems.Count * itemHeight);
    }

    void OnBtnClickCalcByPlan()
    {
        CalcDetail();
    }

    void OnBtnClickGenPlanAndCalc()
    {
        switch (genType)
        {
            case GenerateType.eMinCost:
                CalcByGenerateTypeMinCost();
                break;
            case GenerateType.eFixedProfit:
                CalcByGenerateTypeFixedProfit();
                break;
            case GenerateType.eFixedScaleCount:
                CalcByGenerateTypeFixedScaleCount();
                break;
        }
    }

    void CalcDetail()
    {
        ClearItems();
        reward = float.Parse(inputfieldReward.text);
        cost = float.Parse(inputfieldCost.text);
        numberCountPerTrade = int.Parse(inputfieldCountPerTrade.text);
        float totalCost = 0;
        float totalReward = 0;
        float profit = 0;
        if (string.IsNullOrEmpty(inputfieldTradePlan.text))
            return;
        string[] slus = inputfieldTradePlan.text.Split(',');
        //string info = "";
        int validIndex = 1;
        for (int i = 0; i < slus.Length; ++i)
        {
            if (string.IsNullOrEmpty(slus[i]))
                continue;
            int slu = int.Parse(slus[i]);
            totalCost += slu * numberCountPerTrade * cost;
            totalReward = slu * reward;
            profit = totalReward - totalCost;
            AddItem(i + 1, slu, totalCost, totalReward, profit);
            //info += "[" + validIndex + "] " + slus[i] + "\t(成本: " + totalCost.ToString("f2") + ")\t(奖金: " + totalReward.ToString("f2") + ")\t(获利: " + profit.ToString("f2") + ")\r\n";
            ++validIndex;
        }
        //textBoxResult.Text = info;
    }

    void CalcByGenerateTypeMinCost()
    {
        int.TryParse(inputfieldTradeCount.text, out planCount);
        if (planCount == 0)
            return;
        List<int> plans = new List<int>();
        if (string.IsNullOrEmpty(inputfieldTradePlan.text))
            plans.Add(1);
        else
        {
            string[] slus = inputfieldTradePlan.text.Split(',');
            for (int i = 0; i < slus.Length; ++i)
            {
                if (string.IsNullOrEmpty(slus[i]))
                    continue;
                int slu = int.Parse(slus[i]);
                plans.Add(slu);
            }
        }
        string planStr = "";
        int validIndex = 1;
        float totalCost = 0;
        float totalReward = 0;
        float profit = 0;
        int noCalcID = plans.Count;
        float batchCost = cost * numberCountPerTrade;
        for (int i = 0; i < planCount; ++i)
        {
            if (i < noCalcID)
            {
                totalCost += batchCost * plans[i];
            }
            else
            {
                int dst = (int)Math.Ceiling(totalCost / (reward - batchCost));
                plans.Add(dst);
                totalCost += dst * batchCost;
            }
        }
        for (int i = 0; i < plans.Count; ++i)
        {
            planStr += plans[i];
            if (i < plans.Count - 1)
                planStr += ",";
        }
        inputfieldTradePlan.text = planStr;

        CalcDetail();
    }

    void CalcByGenerateTypeFixedProfit()
    {
        int.TryParse(inputfieldTradeCount.text, out planCount);
        if (planCount == 0)
            return;
        List<int> plans = new List<int>();
        if (string.IsNullOrEmpty(inputfieldTradePlan.text))
            plans.Add(1);
        else
        {
            string[] slus = inputfieldTradePlan.text.Split(',');
            for (int i = 0; i < slus.Length; ++i)
            {
                if (string.IsNullOrEmpty(slus[i]))
                    continue;
                int slu = int.Parse(slus[i]);
                plans.Add(slu);
            }
        }
        float fixedProfit = 0;
        float.TryParse(inputfieldGenParam.text, out fixedProfit);
        string planStr = "";
        int validIndex = 1;
        float totalCost = 0;
        float totalReward = 0;
        float profit = 0;
        int noCalcID = plans.Count;
        float batchCost = cost * numberCountPerTrade;
        for (int i = 0; i < planCount; ++i)
        {
            if (i < noCalcID)
            {
                totalCost += batchCost * plans[i];
            }
            else
            {
                int dst = (int)Math.Ceiling((totalCost + fixedProfit) / (reward - batchCost));
                plans.Add(dst);
                totalCost += dst * batchCost;
            }
        }
        for (int i = 0; i < plans.Count; ++i)
        {
            planStr += plans[i];
            if (i < plans.Count - 1)
                planStr += ",";
        }
        inputfieldTradePlan.text = planStr;

        CalcDetail();
    }

    void CalcByGenerateTypeFixedScaleCount()
    {
        int.TryParse(inputfieldTradeCount.text, out planCount);
        if (planCount == 0)
            return;
        List<int> plans = new List<int>();
        if (string.IsNullOrEmpty(inputfieldTradePlan.text))
            plans.Add(1);
        else
        {
            string[] slus = inputfieldTradePlan.text.Split(',');
            for (int i = 0; i < slus.Length; ++i)
            {
                if (string.IsNullOrEmpty(slus[i]))
                    continue;
                int slu = int.Parse(slus[i]);
                plans.Add(slu);
            }
        }
        float scale = 1;
        float.TryParse(inputfieldGenParam.text, out scale);
        string planStr = "";
        int noCalcID = plans.Count;
        for (int i = 0; i < planCount; ++i)
        {
            if (i < noCalcID)
            {
            }
            else
            {
                int dst = (int)Math.Ceiling(plans[i - 1] * scale);
                plans.Add(dst);
            }
        }
        for (int i = 0; i < plans.Count; ++i)
        {
            planStr += plans[i];
            if (i < plans.Count - 1)
                planStr += ",";
        }
        inputfieldTradePlan.text = planStr;

        CalcDetail();
    }
}
