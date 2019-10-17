using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ScrollBar : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void OnScrollChange();
    
    public bool isVertScroll;
    public OnScrollChange onScrollChange;
    RectTransform rtHandle;
    RectTransform rtBar;
    bool isDraging = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        //rtHandle.anchoredPosition = eventData.position;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rtBar, Input.mousePosition, Camera.main, out pos);

        Debug.Log(pos);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        isDraging = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        isDraging = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        isDraging = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        isDraging = false;
    }

    void Awake()
    {
        rtHandle = transform.GetChild(0) as RectTransform;
        rtBar = transform as RectTransform;
        rtHandle.anchoredPosition = Vector2.zero;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GetProgress());
    }

    public void SetHandleRatio(float ratio)
    {
        if(isVertScroll)
        {
            rtHandle.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtBar.rect.height * ratio);
        }
        else
        {
            rtHandle.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtBar.rect.width * ratio);
        }
    }

    public float GetProgress()
    {
        float v = 0;
        if(isVertScroll)
        {
            if (rtBar.rect.height != rtHandle.rect.height)
                v = -rtHandle.anchoredPosition.y / (rtBar.rect.height - rtHandle.rect.height);
        }
        else
        {
            if (rtBar.rect.width != rtHandle.rect.width)
                v = rtHandle.anchoredPosition.x / (rtBar.rect.width - rtHandle.rect.width);
        }
        return v;
    }
}
