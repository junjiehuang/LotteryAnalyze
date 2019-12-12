using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterTrade : GraphPainterBase
{
    public int selectedIndex = 0;
    bool canScroll = true;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        canvasUpScale.x = GlobalSetting.G_TRADE_CANVAS_SCALE_X;
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
        float xv = g.CanvasToStand(pos.x, true);
        selectedIndex = (int)(xv / canvasUpScale.x);
        if (selectedIndex < 0)
            selectedIndex = 0;
        if (selectedIndex >= PanelTrade.Instance.allTradeInfos.Count)
            selectedIndex = PanelTrade.Instance.allTradeInfos.Count - 1;

        PanelTrade.Instance.NotifyRepaint();

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
        if (selectedIndex >= 0)
        {
            float anchorXold = g.StandToCanvas(selectedIndex * canvasUpScale.x, true);
            float anchorXnew = g.StandToCanvas(selectedIndex * GlobalSetting.G_TRADE_CANVAS_SCALE_X, true);
            canvasUpOffset.x += anchorXold - anchorXnew;

            g.BeforeDraw(canvasUpOffset);
            float startX = g.CanvasToStand(0, true);
            float sv = startX / GlobalSetting.G_TRADE_CANVAS_SCALE_X;
            canScroll = false;
            PanelTrade.Instance.uiTrade.sliderSelectTradeItem.value = sv;
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

        PanelTrade.Instance.NotifyRepaint();

    }

    public void ScrollLatestItemToMiddle(Painter g, RectTransform rtCanvas)
    {
        float curCanvasX = g.StandToCanvas(PanelTrade.Instance.allTradeInfos.Count * canvasUpScale.x, true);
        canvasUpOffset.x += rtCanvas.rect.width * 0.5f - curCanvasX;
        PanelTrade.Instance.NotifyRepaint();
    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {
        base.DrawUpPanel(g, rtCanvas);

        float HistoryMaxMoney = BatchTradeSimulator.Instance.maxMoney;
        float StartMoney = BatchTradeSimulator.Instance.startMoney;
        float HistoryMinMoney = BatchTradeSimulator.Instance.minMoney;
        float MinMoney = HistoryMinMoney;
        if (MinMoney > 0)
            MinMoney = 0;
        float MoneyGap = HistoryMaxMoney - MinMoney;

        float gridSize = canvasUpScale.x - 1;
        if (gridSize < GlobalSetting.G_TRADE_CANVAS_MIN_GRID_SIZE)
            gridSize = GlobalSetting.G_TRADE_CANVAS_MIN_GRID_SIZE;
        float gridHalfSize = gridSize * 0.5f;
        float winW = rtCanvas.rect.width;
        float winH = rtCanvas.rect.height;
        float yMin = winH * 0.1f;
        float yMax = winH - yMin;
        float H = yMax - yMin;
        float sx = g.StandToCanvas(0, true);
        float ex = g.StandToCanvas(PanelTrade.Instance.allTradeInfos.Count * canvasUpScale.x, true);
        g.DrawLineInCanvasSpace(0, yMin, winW, yMin, Color.white, 2);
        g.DrawLineInCanvasSpace(sx, 0, sx, winH, Color.white, 2);
        //g.DrawLineInCanvasSpace(0, yMax, winW, yMax, Color.white, 2);
        if (MinMoney < 0)
        {
            float zy = -MinMoney / MoneyGap * H + yMin;
            g.DrawLineInCanvasSpace(0, zy, winW, zy, Color.white, 2);
        }

        float prevY = 0;
        float prevX = g.StandToCanvas(0, true);
        int Count = PanelTrade.Instance.allTradeInfos.Count;
        for (int i = 0; i < Count; ++i)
        {
            PanelTrade.SingleTradeInfo info = PanelTrade.Instance.allTradeInfos[i];
            if(i == selectedIndex)
            {
                float profitCur = info.reward - info.cost;
                string strProfitCur = profitCur > 0 ? ("盈利：" + profitCur) : (profitCur < 0 ? ("亏损：" + profitCur) : " 没交易");
                float profitTotal = info.moneyLeft - StartMoney;
                string strProfitTotal = profitTotal > 0 ? ("盈利：" + profitTotal) : (profitTotal < 0 ? ("亏损：" + profitTotal) : " 没交易");
                string txt = "[" + i + ", " + info.tradeID + "] 初始：" + StartMoney + " 最高：" + HistoryMaxMoney + " 最低：" + HistoryMinMoney + " 当前：" + info.moneyLeft + "\n" +
                    "总" + strProfitTotal + " 当前" + strProfitCur + "\n" + 
                    info.GetDetailInfo();
                PanelTrade.Instance.uiTrade.graphTrade.AppendText(txt);
            }
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
            if (i == selectedIndex)
            {
                g.DrawLineInCanvasSpace(x + canvasUpScale.x, 0, x + canvasUpScale.x, winH, Color.yellow);
                g.DrawLineInCanvasSpace(x, 0, x, winH, Color.yellow);
            }
            prevX = x + gridHalfSize;
            prevY = y + gridHalfSize;
        }
    }
}
