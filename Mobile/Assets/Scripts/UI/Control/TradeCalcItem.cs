using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TradeCalcItem : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public RectTransform rectTransform;

    public Text textID;
    public Text textCount;
    public Text textCost;
    public Text textReward;
    public Text textProfit;

    Image img;
    bool sel = false;

    private void Awake()
    {
        rectTransform = transform as RectTransform;

        img = GetComponent<Image>();
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

    public void OnPointerClick(PointerEventData eventData)
    {
        sel = !sel;
        img.color = sel ? Color.blue : Color.black;
    }
}
