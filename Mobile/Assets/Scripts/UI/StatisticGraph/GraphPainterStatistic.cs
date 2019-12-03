using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterStatistic : GraphPainterBase
{

    public override void Update()
    {
        PanelStatisticCollect.Instance.graphStatistic.SetPainter(upPainter, this);
        upPainter.BeforeDraw(canvasUpOffset);
        PanelStatisticCollect.Instance.graphStatistic.BeforeUpdate();
        DrawUpPanel(upPainter, PanelStatisticCollect.Instance.graphStatistic.rectTransform);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void OnGraphDragging(Vector2 offset, Painter painter)
    {
        offset.y = 0;
        if (painter == upPainter)
        {
            canvasUpOffset += offset;
            canvasDownOffset.x = canvasUpOffset.x;
        }
        upPainter.BeforeDraw(canvasUpOffset);
        downPainter.BeforeDraw(canvasDownOffset);
        float v = 0;
        if (canvasUpOffset.x < 0)
            v = -canvasUpOffset.x / canvasUpScale.x;
        PanelStatisticCollect.Instance.NotifyRepaint();
    }

    public override void OnPointerClick(Vector2 pos, Painter g)
    {
        base.OnPointerClick(pos, g);
    }

    public override void OnPointerDown(Vector2 pos, Painter g)
    {
        base.OnPointerDown(pos, g);
    }

    public override void OnPointerUp(Vector2 pos, Painter g)
    {
        base.OnPointerUp(pos, g);
    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {
        base.DrawUpPanel(g, rtCanvas);

        float gridHalfSize = canvasUpScale.x * 0.8f;
        float gridSize = gridHalfSize * 2;
        float winW = rtCanvas.rect.width;
        float winH = rtCanvas.rect.height;
        float yMin = winH * 0.1f;
        float yMax = winH - yMin;
        float H = yMax - yMin;
        float sx = g.StandToCanvas(0, true);
        float ex = g.StandToCanvas(PanelStatisticCollect.Instance.TotalCount * canvasUpScale.x, true);
        g.DrawFillRectInCanvasSpace(sx, yMin, ex - sx, yMax, Color.gray);
        g.DrawLineInCanvasSpace(0, yMin, winW, yMin, Color.white);
        g.DrawLineInCanvasSpace(sx, 0, sx, winH, Color.white);

        float MAX = PanelStatisticCollect.Instance.MaxMissCount;
        var itorNumID = PanelStatisticCollect.Instance.allBarUpInfo.GetEnumerator();
        while (itorNumID.MoveNext())
        {
            var itorCDT = itorNumID.Current.Value.GetEnumerator();
            while(itorCDT.MoveNext())
            {
                var itorMissCount = itorCDT.Current.Value.GetEnumerator();
                int missCount = itorMissCount.Current.Key;
                while(itorMissCount.MoveNext())
                {
                    List<ItemWrapper> lst = itorMissCount.Current.Value;
                    for(int i = 0; i < lst.Count; ++i)
                    {
                        float x = g.StandToCanvas(lst[i].globalID * canvasUpScale.x, true);
                        if(x >= 0 && x <= winW)
                        {
                            float yT = (float)(missCount) / MAX * H;
                            float yB = (float)(lst[i].barUpCount) / MAX * H;

                            g.DrawFillRectInCanvasSpace(x - gridHalfSize, yMin, gridSize, yT, Color.cyan);
                            g.DrawFillRectInCanvasSpace(x - gridHalfSize, yMin, gridSize, yB, Color.red);
                        }
                    }
                }
            }
        }
    }
}
