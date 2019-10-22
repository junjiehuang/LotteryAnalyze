using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LotteryAnalyze;
using System;
using System.IO;
using UnityEngine.EventSystems;

public class GraphBase : UnityEngine.UI.MaskableGraphic, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Text text;

    RectTransform meRT;
    RectTransform parentRT;
    Painter painter;
    GraphPainterBase graphPainter;

    public void SetPainter(Painter p, GraphPainterBase gp)
    {
        painter = p;
        graphPainter = gp;
    }

    protected override void Awake()
    {
        meRT = transform as RectTransform;
        parentRT = transform.parent as RectTransform;
        meRT.anchorMin = Vector2.zero;
        meRT.anchorMax = Vector2.zero;
        meRT.pivot = Vector2.zero;
        text = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        meRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRT.rect.width);
        meRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentRT.rect.height);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (painter != null)
        {
            vh.AddUIVertexStream(painter.Verts, painter.Tris);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(graphPainter != null)
        {
            graphPainter.OnGraphDragging(eventData.delta, painter);
        }
        this.SetVerticesDirty();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void BeforeUpdate()
    {
        if (text)
        {
            text.text = "";
        }
    }

    public void AppendText(string txt)
    {
        if (text)
        {
            text.text += txt;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        graphPainter.OnPointerClick(eventData.position, painter);
    }
}
