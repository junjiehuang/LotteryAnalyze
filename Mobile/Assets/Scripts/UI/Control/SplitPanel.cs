using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitPanel : MonoBehaviour
{
    public RectTransform subLT;
    public RectTransform subRB;

    public float splitRatio = 0.5f;
    Splitter splitter;

    [HideInInspector]
    public RectTransform rtSplitter;
    [HideInInspector]
    public RectTransform rtSplitPanel;

    public Vector2 panelLTSize = Vector2.zero;
    public Vector2 panelRBSize = Vector2.zero;
    
    private void Awake()
    {
        rtSplitPanel = transform as RectTransform;
        splitter = GetComponentInChildren<Splitter>();
        rtSplitter = splitter.transform as RectTransform;
        SetSplitterPosByRatio();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SetSplitterPosByRatio()
    {
        Vector2 pos = Vector2.zero;
        if(splitter.splitPanelByVertical)
        {
            pos.x = 0;
            pos.y = (splitter.size - rtSplitPanel.rect.height) * splitRatio;
        }
        else
        {
            pos.y = 0;
            pos.x = (rtSplitPanel.rect.width - splitter.size) * splitRatio;
        }
        rtSplitter.anchoredPosition = pos;
        AdjustSubPanels();
    }

    public void OnSplitterChange()
    {
        if(splitter.splitPanelByVertical)
        {
            splitRatio = rtSplitter.anchoredPosition.y / (rtSplitter.rect.height - rtSplitPanel.rect.height);

            float upPanelSize = (rtSplitPanel.rect.height - rtSplitter.rect.height) * splitRatio;
        }
        else
        {
            splitRatio = rtSplitter.anchoredPosition.x / (rtSplitPanel.rect.width - rtSplitter.rect.width);
        }
        AdjustSubPanels();
    }

    public void AdjustSubPanels()
    {
        if (splitter.splitPanelByVertical)
        {
            float upPanelSize = (rtSplitPanel.rect.height - rtSplitter.rect.height) * splitRatio;
            subLT.anchoredPosition = Vector2.zero;
            subLT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtSplitPanel.rect.width);
            subLT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, upPanelSize);

            subRB.anchoredPosition = new Vector2(0, -(upPanelSize + splitter.size));
            subRB.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtSplitPanel.rect.width);
            subRB.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtSplitPanel.rect.height - upPanelSize - splitter.size);
        }
        else
        {
            float upPanelSize = (rtSplitPanel.rect.width - rtSplitter.rect.width) * splitRatio;
            subLT.anchoredPosition = Vector2.zero;
            subLT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, upPanelSize);
            subLT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtSplitPanel.rect.height);

            subRB.anchoredPosition = new Vector2(upPanelSize + splitter.size, 0);
            subRB.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtSplitPanel.rect.width - upPanelSize - splitter.size);
            subRB.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtSplitPanel.rect.height);
        }
        panelLTSize = subLT.rect.size;
        panelRBSize = subRB.rect.size;
    }


}
