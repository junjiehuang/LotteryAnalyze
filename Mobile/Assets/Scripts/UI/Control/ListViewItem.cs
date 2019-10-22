using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListViewItem : Text, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    public ListView listView;

    bool selected = false;

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        listView.OnListViewScroll(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        if(listView != null)
        {
            listView.OnListViewItemClick(this);
        }
    }

    void Awake()
    {
        
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
