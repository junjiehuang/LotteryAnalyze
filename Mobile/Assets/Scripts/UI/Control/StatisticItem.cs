using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticItem : MonoBehaviour
{
    public Button btnExpand;
    public Text txtInfo;
    public Button btnView;
    public RectTransform rtInfo;

    [HideInInspector]
    public RectTransform rtSelf;
    [HideInInspector]
    

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
            txtInfo.text = item.Text;

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
            PanelStatisticCollect.Instance.RefreshView();
        });

        btnView.onClick.AddListener(() =>
        {
            if (item.Tag != null)
            {
                string idtag = item.Text;
                if (item.Text.Contains(","))
                {
                    idtag = item.Text.Split(',')[0];
                }
                PanelDataView.Instance.ReadSpecDateData(idtag, (int)item.Tag);
                PanelAnalyze.Instance.ChangeNumIndexAndCdtIndex(item.numIndex, item.cdtIndex);
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
        
    }

}


public class ItemWrapper
{
    public List<ItemWrapper> SubNodes = new List<ItemWrapper>();
    public bool Expand = false;
    public string Text;
    public object Tag;
    public int numIndex;
    public int cdtIndex;
    public bool isLeafNode;
    public bool showViewBtn;
    public int globalID = -1;
    public int barUpCount = 0;

    public ItemWrapper(string info)
    {
        Text = info;
        isLeafNode = false;
        showViewBtn = false;
    }

}