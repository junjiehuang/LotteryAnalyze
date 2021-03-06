﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LotteryAnalyze;
using System;
using System.IO;
using UnityEngine.EventSystems;

public class GraphBase : UnityEngine.UI.MaskableGraphic, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    Text text;

    RectTransform meRT;
    RectTransform parentRT;
    Painter painter;
    GraphPainterBase graphPainter;
    bool hasDragged;

    Vector2 lastDragPos = Vector2.zero;

    List<Text> freeTxts = new List<Text>();
    List<Text> usingTxts = new List<Text>();

    public delegate Vector2 CallBackConvertPosToPainter(Vector2 pointerPos, GraphBase graph);
    public CallBackConvertPosToPainter ConvertPosToPainter;

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

    public void BeforeUpdate()
    {
        if (text)
        {
            text.text = "";
        }

        for(int i = 0; i < usingTxts.Count; ++i)
        {
            freeTxts.Add(usingTxts[i]);
            usingTxts[i].text = "";
        }
        usingTxts.Clear();
    }

    public void AppendText(string txt)
    {
        if (text)
        {
            text.text += txt;
        }
    }

    public void DrawText(string info, float x, float y, Color col)
    {
        y = y - meRT.rect.height;
        Text txt = null;
        if(freeTxts.Count > 0)
        {
            txt = freeTxts[0];
            freeTxts.RemoveAt(0);
        }
        else
        {
            GameObject go = GameObject.Instantiate(text.gameObject);
            go.transform.SetParent(text.transform.parent);
            txt = go.GetComponent<Text>();
        }
        usingTxts.Add(txt);
        txt.rectTransform.pivot = new Vector2(0, 1);
        txt.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, meRT.rect.width);
        txt.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, meRT.rect.height);
        txt.rectTransform.anchoredPosition = new Vector2(x, y);
        txt.color = col;
        txt.text = info;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (graphPainter != null)
        {
            Vector2 offset = eventData.delta;
            graphPainter.OnGraphDragging(offset, painter);
        }
        this.SetVerticesDirty();
        hasDragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        if (ConvertPosToPainter != null)
            pos = ConvertPosToPainter.Invoke(pos, this);

        if (hasDragged == false)
            graphPainter.OnPointerClick(pos, painter);
        hasDragged = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        if (ConvertPosToPainter != null)
            pos = ConvertPosToPainter.Invoke(pos, this);

        graphPainter.OnPointerDown(pos, painter);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        if (ConvertPosToPainter != null)
            pos = ConvertPosToPainter.Invoke(pos, this);

        graphPainter.OnPointerUp(pos, painter);
    }
}
