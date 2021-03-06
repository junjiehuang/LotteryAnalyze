﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterBase
{
    public Painter upPainter = new Painter("PainterUp");
    public Painter downPainter = new Painter("PainterDown");

    public Vector2 canvasUpScale = new Vector2(5, 20);
    public Vector2 canvasDownScale = new Vector2(5, 10);
    public Vector2 canvasUpOffset;
    public Vector2 canvasDownOffset;

    public virtual void Reset()
    {
        canvasUpOffset.Set(0, 0);
        canvasDownOffset.Set(0, 0);
    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        PanelAnalyze.Instance.graphUp.SetPainter(upPainter, this);
        PanelAnalyze.Instance.graphDown.SetPainter(downPainter, this);
        upPainter.BeforeDraw(canvasUpOffset);
        downPainter.BeforeDraw(canvasDownOffset);

        PanelAnalyze.Instance.graphUp.BeforeUpdate();
        PanelAnalyze.Instance.graphDown.BeforeUpdate();
        DrawUpPanel(upPainter, PanelAnalyze.Instance.graphUp.rectTransform);
        DrawDownPanel(downPainter, PanelAnalyze.Instance.graphDown.rectTransform);
    }

    public virtual void OnGraphDragging(Vector2 offset, Painter painter)
    {
        if(painter == upPainter)
        {
            canvasUpOffset += offset;
            canvasDownOffset.x = canvasUpOffset.x;
        }
        else
        {
            canvasDownOffset += offset;
            canvasUpOffset.x = canvasDownOffset.x;
        }
        upPainter.BeforeDraw(canvasUpOffset);
        downPainter.BeforeDraw(canvasDownOffset);
        float v = 0;
        if (canvasUpOffset.x < 0)
            v = -canvasUpOffset.x / canvasUpScale.x;
        PanelAnalyze.Instance.NotifyUIRepaint();
        PanelAnalyze.Instance.SetSliderValue(v);
    }

    public virtual void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {

    }

    public virtual void DrawDownPanel(Painter g, RectTransform rtCanvas)
    {

    }

    public virtual void OnScrollToData(float dataIndex)
    {
        canvasUpOffset.x = -dataIndex * canvasUpScale.x;
        canvasDownOffset.x = canvasUpOffset.x;

        PanelAnalyze.Instance.NotifyUIRepaint();
    }

    public virtual void OnPointerClick(Vector2 pos, Painter g)
    {

    }

    public virtual void OnPointerDown(Vector2 pos, Painter g)
    {

    }

    public virtual void OnPointerUp(Vector2 pos, Painter g)
    {

    }

    public virtual void OnAutoAllign()
    {

    }
}
