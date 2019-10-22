using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListView : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Color itemNormalColor = Color.black;
    public Color itemSelColor = Color.red;

    public RectTransform rtContent;
    public RectTransform rtItem;

    public List<ListViewItem> freeItems = new List<ListViewItem>();

    List<ListViewItem> items = new List<ListViewItem>();
    List<object> tags = new List<object>();

    public delegate void OnClickItem(int index);
    public OnClickItem onClickItem;

    int selIndex = -1;

    public ListViewItem SelectItem
    {
        get
        {
            if (selIndex >= 0 && selIndex < items.Count)
                return items[selIndex];
            return null;
        }
        set
        {
            selIndex = items.IndexOf(value);
        }
    }

    public int SelIndex
    {
        get
        {
            return selIndex;
        }
        set
        {
            selIndex = value;
        }
    }

    public object SelectItemTag
    {
        get
        {
            if (selIndex >= 0 && selIndex < tags.Count)
                return tags[selIndex];
            return null;
        }
    }

    public ListViewItem FindItem(string name)
    {
        for(int i = 0; i < items.Count; ++i)
        {
            if (items[i].text == name)
                return items[i];
        }
        return null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnListViewScroll(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    void Awake()
    {
        rtContent = transform.Find("Contents") as RectTransform;
        rtItem = rtContent.GetChild(0) as RectTransform;
        rtItem.gameObject.SetActive(false);
        freeItems.Add(rtItem.GetComponent<ListViewItem>());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(string itemInfo, object tag)
    {
        ListViewItem item = null;
        if(freeItems.Count > 0)
        {
            item = freeItems[0];
            freeItems.RemoveAt(0);
        }
        else
        {
            GameObject go = GameObject.Instantiate(rtItem.gameObject);
            item = go.GetComponent<ListViewItem>();
        }
        item.listView = this;
        item.gameObject.SetActive(true);
        item.transform.SetParent(rtContent, true);
        RectTransform rt = item.rectTransform;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtItem.rect.width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtItem.rect.height);
        item.text = itemInfo;
        items.Add(item);
        tags.Add(tag);
        AdjustItemPos();
    }

    public void RemoveItem(int index)
    {
        if(index >= 0 && index < items.Count)
        {
            ListViewItem item = items[index];
            item.gameObject.SetActive(false);
            freeItems.Add(item);
            tags.RemoveAt(index);
            items.RemoveAt(index);

            AdjustItemPos();
        }        
    }

    public void ClearAllItems()
    {
        for(int i = 0; i < items.Count; ++i)
        {
            freeItems.Add(items[i]);
            items[i].gameObject.SetActive(false);
        }
        items.Clear();
        tags.Clear();
    }

    void AdjustItemPos()
    {
        for(int i = 0; i < items.Count; ++i)
        {
            Text item = items[i];
            Vector2 pos = new Vector2(0, -rtItem.rect.height * i);
            item.rectTransform.SetSiblingIndex(i);
            item.rectTransform.anchoredPosition = pos;
        }
    }

    public void OnListViewItemClick(ListViewItem item)
    {
        item.color = itemSelColor;
        if(SelectItem != null && SelectItem != item)
        {
            SelectItem.color = itemNormalColor;
        }
        selIndex = items.IndexOf(item);
        onClickItem.Invoke(selIndex);
    }

    public void OnListViewScroll(Vector2 offset)
    {
        Vector2 pos = rtContent.anchoredPosition;
        pos.y += offset.y;
        if (pos.y < 0)
            pos.y = 0;
        rtContent.anchoredPosition = pos;
    }
}
