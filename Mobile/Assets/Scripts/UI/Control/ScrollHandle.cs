using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollHandle : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    ScrollBar bar;
    RectTransform rtBar;
    RectTransform rt;
    bool isDraging = false;

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        if (bar.isVertScroll)
        {
            rt.anchoredPosition += new Vector2(0, eventData.delta.y);
            if (rt.anchoredPosition.y > 0)
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
            if (rt.anchoredPosition.y < -(rtBar.rect.height - rt.rect.height))
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -(rtBar.rect.height - rt.rect.height));
        }
        else
        {
            rt.anchoredPosition += new Vector2(eventData.delta.x, 0);
            if (rt.anchoredPosition.x < 0)
                rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
            if (rt.anchoredPosition.x > rtBar.rect.width - rt.rect.width)
                rt.anchoredPosition = new Vector2(rtBar.rect.width - rt.rect.width, rt.anchoredPosition.y);
        }
        bar.onScrollChange();
        //Debug.Log(rt.anchoredPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        isDraging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
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
        bar = transform.parent.gameObject.GetComponent<ScrollBar>();
        rt = transform as RectTransform;
        rtBar = bar.transform as RectTransform;
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
