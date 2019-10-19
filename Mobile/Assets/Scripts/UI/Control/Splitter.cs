using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Splitter : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool splitPanelByVertical = true;
    public float size = 10;

    SplitPanel splitPanel;
    bool isDragging = false;
    Image img;

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        if (splitPanelByVertical)
        {
            Vector2 pos = splitPanel.rtSplitter.anchoredPosition + new Vector2(0, eventData.delta.y);
            if (pos.y > 0)
                pos.y = 0;
            if (pos.y < size - splitPanel.rtSplitPanel.rect.height)
                pos.y = size - splitPanel.rtSplitPanel.rect.height;
            splitPanel.rtSplitter.anchoredPosition = pos;
        }
        else
        {
            Vector2 pos = splitPanel.rtSplitter.anchoredPosition + new Vector2(eventData.delta.x, 0);
            if (pos.x < 0)
                pos.x = 0;
            if (pos.x > splitPanel.rtSplitPanel.rect.width - size)
                pos.x = splitPanel.rtSplitPanel.rect.width - size;
            splitPanel.rtSplitter.anchoredPosition = pos;
        }
        splitPanel.OnSplitterChange();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void Awake()
    {
        splitPanel = transform.parent.GetComponent<SplitPanel>();
        img = gameObject.GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float showSize = size;
        if (splitPanelByVertical)
        {
            splitPanel.rtSplitter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, splitPanel.rtSplitPanel.rect.width);
            splitPanel.rtSplitter.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, showSize);
        }
        else
        {
            splitPanel.rtSplitter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, showSize);
            splitPanel.rtSplitter.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, splitPanel.rtSplitPanel.rect.height);
        }
        img.color = isDragging ? Color.green : Color.white;
    }
}
