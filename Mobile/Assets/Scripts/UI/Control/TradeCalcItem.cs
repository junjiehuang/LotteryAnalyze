using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeCalcItem : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rectTransform;

    public Text textID;
    public Text textCount;
    public Text textCost;
    public Text textReward;
    public Text textProfit;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInfo(string id, string count, string cost, string reward, string profit)
    {
        textID.text = id;
        textCount.text = count;
        textCost.text = cost;
        textReward.text = reward;
        textProfit.text = profit;
    }
}
