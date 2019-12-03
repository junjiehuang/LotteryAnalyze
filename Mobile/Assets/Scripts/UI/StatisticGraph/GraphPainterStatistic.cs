using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterStatistic : GraphPainterBase
{
    public int selectedIndex = 0;
    int maxMissCount = 0;
    int maxBarUpCount = 0;
    bool canScroll = true;

    public override void Update()
    {
        canvasUpScale.x = GlobalSetting.G_STATISTIC_CANVAS_SCALE_X;
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

        PanelStatisticCollect.Instance.slider.value = v;
        PanelStatisticCollect.Instance.NotifyRepaint();
    }

    public override void OnPointerClick(Vector2 pos, Painter g)
    {
        base.OnPointerClick(pos, g);

        float xv = g.CanvasToStand(pos.x, true);
        selectedIndex = (int)( xv / canvasUpScale.x );
        if (selectedIndex < 0)
            selectedIndex = 0;
        if (selectedIndex > PanelStatisticCollect.Instance.TotalCount)
            selectedIndex = PanelStatisticCollect.Instance.TotalCount;

        PanelStatisticCollect.Instance.NotifyRepaint();
    }

    public override void OnPointerDown(Vector2 pos, Painter g)
    {
        base.OnPointerDown(pos, g);
    }

    public override void OnPointerUp(Vector2 pos, Painter g)
    {
        base.OnPointerUp(pos, g);
    }

    public void OnGraphZoom(Painter g)
    {
        if(selectedIndex >= 0)
        {
            float anchorXold = g.StandToCanvas(selectedIndex * canvasUpScale.x, true);
            float anchorXnew = g.StandToCanvas(selectedIndex * GlobalSetting.G_STATISTIC_CANVAS_SCALE_X, true);
            canvasUpOffset.x += anchorXold - anchorXnew;

            g.BeforeDraw(canvasUpOffset);
            float startX = g.CanvasToStand(0, true);
            float sv = startX / GlobalSetting.G_STATISTIC_CANVAS_SCALE_X;
            canScroll = false;
            PanelStatisticCollect.Instance.slider.value = sv;
        }
    }

    public override void OnScrollToData(float dataIndex)
    {
        if (canScroll)
        {
            canvasUpOffset.x = -dataIndex * canvasUpScale.x;
            canvasDownOffset.x = canvasUpOffset.x;
        }
        canScroll = true;

        PanelStatisticCollect.Instance.NotifyRepaint();
    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {
        base.DrawUpPanel(g, rtCanvas);
        
        float gridSize = canvasUpScale.x - 1;
        if (gridSize < GlobalSetting.G_STATISTIC_CANVAS_MIN_GRID_SIZE)
            gridSize = GlobalSetting.G_STATISTIC_CANVAS_MIN_GRID_SIZE;
        float winW = rtCanvas.rect.width;
        float winH = rtCanvas.rect.height;
        float yMin = winH * 0.1f;
        float yMax = winH - yMin;
        float H = yMax - yMin;
        float sx = g.StandToCanvas(0, true);
        float ex = g.StandToCanvas(PanelStatisticCollect.Instance.TotalCount * canvasUpScale.x, true);
        g.DrawFillRectInCanvasSpace(sx, yMin, ex - sx, yMax - yMin, new Color(0.1f,0.1f,0.1f));
        g.DrawLineInCanvasSpace(0, yMin, winW, yMin, Color.white);
        g.DrawLineInCanvasSpace(sx, 0, sx, winH, Color.white);

        float MAX = PanelStatisticCollect.Instance.MaxMissCount;
        float yT = 0, yB = 0;
        
        var itorGID = PanelStatisticCollect.Instance.graphDataItems.GetEnumerator();
        while(itorGID.MoveNext())
        {
            int gid = itorGID.Current.Key;
            float x = g.StandToCanvas(gid * canvasUpScale.x, true);
            if (x < 0 || x > winW)
                continue;
            List<ItemWrapper> lst = itorGID.Current.Value;
            for(int i = 0; i < lst.Count; ++i)
            {
                ItemWrapper iw = lst[i];
                if (iw.cdtIndex > 2)
                    continue;
                yT = (float)(iw.missCount) / MAX * H;
                yB = (float)(iw.barUpCount) / MAX * H;
                g.DrawFillRectInCanvasSpace(x, yMin, gridSize, yT, Color.cyan);
                g.DrawFillRectInCanvasSpace(x, yMin, gridSize, yB, Color.red);
                if (gid == selectedIndex)
                {
                    string info = KDataDictContainer.C_TAGS[iw.numIndex] + ", " +
                        GraphDataManager.S_CDT_TAG_LIST[iw.cdtIndex] + ", " +
                        iw.missCount + ", " + iw.Text + "\n";
                    PanelStatisticCollect.Instance.graphStatistic.AppendText(info);
                }

                if (maxMissCount < iw.missCount)
                    maxMissCount = iw.missCount;
                if (maxBarUpCount < iw.barUpCount)
                    maxBarUpCount = iw.barUpCount;
            }
        }

        if (selectedIndex >= 0)
        {
            float x = g.StandToCanvas(selectedIndex * canvasUpScale.x, true);
            g.DrawLineInCanvasSpace(x, 0, x, winH, Color.yellow);
            g.DrawLineInCanvasSpace(x + canvasUpScale.x, 0, x + canvasUpScale.x, winH, Color.yellow);
        }

        yT = (float)(maxMissCount) / MAX * H + yMin;
        yB = (float)(maxBarUpCount) / MAX * H + yMin;
        g.DrawLineInCanvasSpace(0, yT, winW, yT, Color.cyan);
        g.DrawLineInCanvasSpace(0, yB, winW, yB, Color.red);
    }
}
