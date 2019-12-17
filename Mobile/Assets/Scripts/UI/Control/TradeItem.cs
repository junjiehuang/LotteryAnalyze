using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeItem : MonoBehaviour
{
    public Button btnExpand;
    public Text txtInfo;
    public Button btnView;
    public RectTransform rtInfo;

    [HideInInspector]
    public RectTransform rtSelf;

    Text btnText;
    ItemWrapper item;

    public string Text
    {
        get
        {
            return txtInfo.text;
        }
    }

    public void SetItem(ItemWrapper _item)
    {
        item = _item;
        if (item != null)
        {
            if (item.Expand)
                btnText.text = "-";
            else
                btnText.text = "+";
            
            if (item.isLeafNode == false)
            {
                txtInfo.text = "连错[" + item.Tag + "] " + item.SubNodes.Count + " / " + PanelTrade.Instance.totalValidTradeCount;
            }
            else
            {
                txtInfo.text = item.Text;
            }

            btnView.gameObject.SetActive(_item.showViewBtn);
            btnExpand.gameObject.SetActive(!item.isLeafNode);
        }
    }

    private void Awake()
    {
        rtSelf = transform as RectTransform;
        btnText = btnExpand.GetComponentInChildren<Text>();
        btnExpand.onClick.AddListener(() =>
        {
            item.Expand = !item.Expand;
            if (item.Expand)
                btnText.text = "-";
            else
                btnText.text = "+";
            PanelTrade.Instance.RefreshView();
            //PanelStatisticCollect.Instance.RefreshView();
        });

        btnView.onClick.AddListener(() =>
        {
            if (item.showViewBtn && item.Tag != null)
            {
                int tradeIndex = (int)item.Tag;
                PanelTrade.Instance.ScrollToData(tradeIndex);
                //string idtag = item.Text;
                //if (item.Text.Contains(","))
                //{
                //    idtag = item.Text.Split(',')[0];
                //}
                //PanelDataView.Instance.ReadSpecDateData(idtag, (int)item.Tag);
                //PanelAnalyze.Instance.ChangeNumIndexAndCdtIndex(item.numIndex, item.cdtIndex);
            }
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(item != null)
        {
            if(item.isLeafNode == false)
            {
                txtInfo.text = "连错[" + item.Tag + "] " + item.SubNodes.Count + " / " + PanelTrade.Instance.totalValidTradeCount;
            }
        }
    }
}
