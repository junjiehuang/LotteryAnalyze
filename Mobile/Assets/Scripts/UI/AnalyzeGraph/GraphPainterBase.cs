using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterBase
{
    public Painter upPainter = new Painter();
    public Painter downPainter = new Painter();

    public Vector2 canvasUpScale = new Vector2(5, 10);
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
        PanelAnalyze.Instance.graphUp.SetVerticesDirty();
        PanelAnalyze.Instance.graphDown.SetVerticesDirty();
    }

    public virtual void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {

    }

    public virtual void DrawDownPanel(Painter g, RectTransform rtCanvas)
    {

    }
}
