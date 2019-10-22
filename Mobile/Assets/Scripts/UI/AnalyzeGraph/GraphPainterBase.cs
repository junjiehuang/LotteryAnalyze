using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterBase
{
    public Painter upPainter = new Painter();
    public Painter downPainter = new Painter();

    public Vector2 canvasUpScale = new Vector2(5, 20);
    public Vector2 canvasDownScale = new Vector2(5, 10);
    public Vector2 canvasUpOffset;
    public Vector2 canvasDownOffset;

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
        float v = 0;
        if (canvasUpOffset.x < 0)
            v = -canvasUpOffset.x / canvasUpScale.x;
        PanelAnalyze.Instance.SetSliderValue(v);
        PanelAnalyze.Instance.NotifyUIRepaint();
    }

    public virtual void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {

    }

    public virtual void DrawDownPanel(Painter g, RectTransform rtCanvas)
    {

    }

    public virtual void OnScrollToData(int dataIndex)
    {
        canvasUpOffset.x = -dataIndex * canvasUpScale.x;
        canvasDownOffset.x = canvasUpOffset.x;

        PanelAnalyze.Instance.NotifyUIRepaint();
    }

    public virtual void OnPointerClick(Vector2 pos, Painter g)
    {

    }
}
