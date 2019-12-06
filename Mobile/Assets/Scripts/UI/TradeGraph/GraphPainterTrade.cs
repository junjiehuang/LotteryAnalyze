using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterTrade : GraphPainterBase
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        canvasUpScale.x = GlobalSetting.G_STATISTIC_CANVAS_SCALE_X;
        PanelTrade.Instance.uiTrade.graphTrade.SetPainter(upPainter, this);
        upPainter.BeforeDraw(canvasUpOffset);
        PanelTrade.Instance.uiTrade.graphTrade.BeforeUpdate();
        DrawUpPanel(upPainter, PanelTrade.Instance.uiTrade.graphTrade.rectTransform);

    }

    public override void OnGraphDragging(Vector2 offset, Painter painter)
    {
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

        PanelTrade.Instance.uiTrade.sliderSelectTradeItem.value = v;
        PanelTrade.Instance.NotifyRepaint();

    }

    public override void OnPointerClick(Vector2 pos, Painter g)
    {

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

    }

    public override void OnScrollToData(float dataIndex)
    {

    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {
        base.DrawUpPanel(g, rtCanvas);

        float MaxMoney = BatchTradeSimulator.Instance.maxMoney;
        float MinMoney = BatchTradeSimulator.Instance.minMoney;
        if (MinMoney > 0)
            MinMoney = 0;
        float MoneyGap = MaxMoney - MinMoney;

        float gridSize = canvasUpScale.x - 1;
        if (gridSize < GlobalSetting.G_STATISTIC_CANVAS_MIN_GRID_SIZE)
            gridSize = GlobalSetting.G_STATISTIC_CANVAS_MIN_GRID_SIZE;
        float gridHalfSize = gridSize * 0.5f;
        float winW = rtCanvas.rect.width;
        float winH = rtCanvas.rect.height;
        float yMin = winH * 0.1f;
        float yMax = winH - yMin;
        float H = yMax - yMin;
        float sx = g.StandToCanvas(0, true);
        float ex = g.StandToCanvas(PanelTrade.Instance.allTradeInfos.Count * canvasUpScale.x, true);
        g.DrawLineInCanvasSpace(0, yMin, winW, yMin, Color.white);
        g.DrawLineInCanvasSpace(sx, 0, sx, winH, Color.white);
        if(MinMoney < 0)
        {
            float zy = -MinMoney / MoneyGap * H + yMin;
            g.DrawLineInCanvasSpace(0, zy, winW, zy, Color.white);
        }

        float prevY = 0;
        float prevX = 0;
        for(int i = 0; i < PanelTrade.Instance.allTradeInfos.Count; ++i)
        {
            PanelTrade.SingleTradeInfo info = PanelTrade.Instance.allTradeInfos[i];
            float x = g.StandToCanvas(i * canvasUpScale.x, true);
            if (x < 0)
                continue;
            if (x > winW)
                return;
            float y = (info.moneyLeft - MinMoney) / MoneyGap * H + yMin;
            Color c = info.cost == 0 ? Color.white : (info.reward > info.cost ? Color.red : Color.cyan);
            g.DrawRectInCanvasSpace(x, y, gridSize, gridSize, c, 2);
            if(i > 0)
            {
                g.DrawLineInCanvasSpace(prevX, prevY, x + gridHalfSize, y + gridHalfSize, Color.white);
            }
            prevX = x + gridHalfSize;
            prevY = y + gridHalfSize;
        }
    }
}
