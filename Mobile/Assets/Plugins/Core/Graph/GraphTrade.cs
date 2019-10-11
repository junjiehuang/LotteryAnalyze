using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

#if WIN
using System.Windows.Forms;

namespace LotteryAnalyze
{
    // 交易图
    public class GraphTrade : GraphBase
    {
        public bool autoAllign = false;

        Pen grayDotLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);
        Pen redLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 1);
        Pen cyanLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Cyan, 1);
        Pen whiteLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);
        Pen moneyLvLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);

        Font tipsFont = new Font(FontFamily.GenericMonospace, 9);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush cyanBrush = new SolidBrush(Color.Cyan);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush greenBrush = new SolidBrush(Color.Green);
        int selectTradeIndex = -1;
        static float[] TRADE_LVS = new float[] { 0, 0.5f, 1.0f, 1.5f, 2.0f, };

        public GraphTrade()
        {
            //gridScaleW = 10;
            gridScaleUp.X = gridScaleDown.X = 10;
        }

        public override void MoveGraph(float dx, float dy)
        {
            canvasOffset.X += dx;
            canvasOffset.Y += dy;
            if (canvasOffset.X < 0)
                canvasOffset.X = 0;
        }
        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return true;
        }
        public override void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            bool isMouseAtRight = mouseRelPos.X > winW * 0.6f;
            g.DrawLine(grayDotLinePen, 0, mouseRelPos.Y, winW, mouseRelPos.Y);

            float halfSize = gridScaleUp.X * 0.2f;
            if (halfSize < 4)
                halfSize = 4;
            float fullSize = halfSize * 2;
            float halfWinH = winH * 0.5f;
            float halfGridW = gridScaleUp.X * 0.5f;

            TradeDataManager tdm = TradeDataManager.Instance;
            float zeroY = StandToCanvas(0, false, true);
            float startMoneyY = StandToCanvas(BatchTradeSimulator.Instance.startMoney * gridScaleUp.Y, false, true);
            float maxMoneyY = StandToCanvas(BatchTradeSimulator.Instance.maxMoney * gridScaleUp.Y, false, true);
            float minMoneyY = StandToCanvas(BatchTradeSimulator.Instance.minMoney * gridScaleUp.Y, false, true);
            g.FillRectangle(redBrush, winW - 10, maxMoneyY, 10, startMoneyY - maxMoneyY);
            g.FillRectangle(cyanBrush, winW - 10, startMoneyY, 10, zeroY - startMoneyY);
            if (BatchTradeSimulator.Instance.minMoney < BatchTradeSimulator.Instance.startMoney)
                g.FillRectangle(greenBrush, winW - 10, startMoneyY, 10, minMoneyY - startMoneyY);

            moneyLvLinePen.Color = Color.Red;
            g.DrawLine(moneyLvLinePen, 0, maxMoneyY, winW, maxMoneyY);
            if (isMouseAtRight)
                g.DrawString(BatchTradeSimulator.Instance.maxMoney.ToString("f0"), tipsFont, whiteBrush, winW - 65, maxMoneyY);
            moneyLvLinePen.Color = Color.Green;
            g.DrawLine(moneyLvLinePen, 0, minMoneyY, winW, minMoneyY);
            if (isMouseAtRight)
                g.DrawString(BatchTradeSimulator.Instance.minMoney.ToString("f0"), tipsFont, whiteBrush, winW - 65, minMoneyY);

            for (int i = 0; i < TRADE_LVS.Length; ++i)
            {
                float lv = TRADE_LVS[i];
                float money = tdm.startMoney * lv;
                float y = money * gridScaleUp.Y;
                float relY = StandToCanvas(y, false, true);
                if (lv < 0)
                    moneyLvLinePen.Color = Color.Gray;
                else if (lv == 0)
                    moneyLvLinePen.Color = Color.Gray;
                else if (lv == 1)
                    moneyLvLinePen.Color = Color.Orange;
                else if (lv > 1)
                    moneyLvLinePen.Color = Color.White;
                else
                    moneyLvLinePen.Color = Color.Cyan;
                g.DrawLine(moneyLvLinePen, 0, relY, winW, relY);
                if (isMouseAtRight)
                    g.DrawString(money.ToString("f0"), tipsFont, whiteBrush, winW - 65, relY);
            }

            float curMouseY = CanvasToStand(mouseRelPos.Y, false, true);
            curMouseY /= gridScaleUp.Y;
            g.DrawString(curMouseY.ToString("f0"), tipsFont, whiteBrush, winW - 65, mouseRelPos.Y);

            if (tdm.historyTradeDatas.Count == 0)
            {
                float y = tdm.startMoney * gridScaleUp.Y;
                float relY = StandToCanvas(y, false, true);
                if (relY < 0 || relY > winH)
                {
                    canvasOffset.Y = y + winH * 0.5f;
                }
                return;
            }

            //float maxGap = Math.Max(Math.Abs(tdm.maxValue), Math.Abs(tdm.minValue)) * 2;
            float maxGap = Math.Max(Math.Abs(BatchTradeSimulator.Instance.maxMoney), Math.Abs(BatchTradeSimulator.Instance.minMoney)) * 1.5f;
            gridScaleUp.Y = winH / maxGap * 0.9f;
            int startIndex = (int)(canvasOffset.X / gridScaleUp.X) - 1;
            if (startIndex < 0)
                startIndex = 0;
            int endIndex = (int)((canvasOffset.X + winW) / gridScaleUp.X) + 1;
            if (endIndex > tdm.historyTradeDatas.Count)
                endIndex = tdm.historyTradeDatas.Count;

            // 自动对齐
            if (autoAllign)
            {
                float endY = tdm.historyTradeDatas[endIndex - 1].moneyAtferTrade * gridScaleUp.Y;
                float relEY = StandToCanvas(endY, false, true);
                bool isEYOut = relEY < 0 || relEY > winH;
                if (isEYOut)
                    canvasOffset.Y = endY + winH * 0.5f;
                autoAllign = false;
            }

            int selIndex = -1;
            for (int i = startIndex; i < endIndex; ++i)
            {
                TradeDataBase tdb = tdm.historyTradeDatas[i];
                float cx = i * gridScaleUp.X + halfGridW;
                float px = cx - gridScaleUp.X;
                float py = tdb.moneyBeforeTrade * gridScaleUp.Y;
                float cy = tdb.moneyAtferTrade * gridScaleUp.Y;
                cx = StandToCanvas(cx, true, true);
                px = StandToCanvas(px, true, true);
                cy = StandToCanvas(cy, false, true);
                py = StandToCanvas(py, false, true);
                Pen pen = (tdb.cost == 0 ? whiteLinePen : (tdb.reward > tdb.cost ? redLinePen : cyanLinePen));
                if (i == 0)
                    g.DrawRectangle(whiteLinePen, px - halfSize, py - halfSize, fullSize, fullSize);
                g.DrawRectangle(pen, cx - halfSize, cy - halfSize, fullSize, fullSize);
                g.DrawLine(pen, px, py, cx, cy);

                if (mouseRelPos.X >= cx - halfGridW && mouseRelPos.X <= cx + halfGridW && selIndex == -1)
                {
                    selIndex = i;
                    g.DrawLine(grayDotLinePen, cx - halfGridW, 0, cx - halfGridW, winH);
                    g.DrawLine(grayDotLinePen, cx + halfGridW, 0, cx + halfGridW, winH);
                    string info = tdb.GetTips() + "\n" +
                        "[对:" + tdm.rightCount +
                        "] [错:" + tdm.wrongCount +
                        "] [放弃:" + tdm.untradeCount +
                        "]\n[最高:" + tdm.maxValue +
                        "] [最低:" + tdm.minValue + "]\n";
#if TRADE_DBG
                    info += tdb.GetDbgInfo();
#endif
                    g.DrawString(info, tipsFont, whiteBrush, 5, 5);
                }

                if (selectTradeIndex == i)
                {
                    g.DrawLine(whiteLinePen, cx - halfGridW, 0, cx - halfGridW, winH);
                    g.DrawLine(whiteLinePen, cx + halfGridW, 0, cx + halfGridW, winH);
                }

                if (i == TradeDataManager.Instance.historyTradeDatas.Count - 1)
                {
                    g.DrawLine(whiteLinePen, cx, cy, winW, cy);
                    //if(isMouseAtRight)
                    if (tdb.moneyAtferTrade <= 0)
                        g.DrawString(tdb.moneyAtferTrade.ToString("f0"), tipsFont, cyanBrush, winW - 130, cy);
                    else
                        g.DrawString(tdb.moneyAtferTrade.ToString("f0"), tipsFont, whiteBrush, winW - 130, cy);
                }
            }
        }
        public override void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            int selID = -1;
            TradeDataManager tdm = TradeDataManager.Instance;
            if (tdm.waitingTradeDatas.Count > 0)
            {
                float y = winH - 2 * gridScaleDown.X;

                for (int i = 0; i < tdm.waitingTradeDatas.Count; ++i)
                {
                    float x = i * gridScaleDown.X;
                    g.DrawRectangle(whiteLinePen, x, y, gridScaleDown.X, gridScaleDown.X);

                    if (selID == -1 && mouseRelPos.X >= x && mouseRelPos.X <= x + gridScaleDown.X)
                    {
                        selID = i;
                        g.DrawLine(grayDotLinePen, x, 0, x, winH);
                        g.DrawLine(grayDotLinePen, x + gridScaleDown.X, 0, x + gridScaleDown.X, winH);

                        string info = tdm.waitingTradeDatas[i].GetTips();
#if TRADE_DBG
                        info += "\n" + tdm.waitingTradeDatas[i].GetDbgInfo();
#endif
                        g.DrawString(info, tipsFont, whiteBrush, 5, 5);
                    }
                }
            }
        }

        public void GetViewItemIndexInfo(ref int startIndex, ref int maxIndex)
        {
            startIndex = (int)(canvasOffset.X / gridScaleUp.X) - 1;
            if (startIndex < 0)
                startIndex = 0;

            maxIndex = TradeDataManager.Instance.historyTradeDatas.Count;
        }

        public override void ScrollToData(int index, int winW, int winH, bool needSelect, int xOffset = 0, bool needScrollToData = true)
        {
            if (needSelect)
                selectTradeIndex = index;
            else
                selectTradeIndex = -1;
            if (needScrollToData)
            {
                canvasOffset.X = index * gridScaleUp.X + xOffset;// (index + 1) * gridScaleW + xOffset;
            }
            autoAllign = true;
        }

        public int SelectTradeData(Point mouseRelPos)
        {
            TradeDataManager tdm = TradeDataManager.Instance;
            selectTradeIndex = -1;
            Point standMousePos = CanvasToStand(mouseRelPos, true);
            int mouseHoverID = (int)(standMousePos.X / gridScaleUp.X);
            if (mouseHoverID >= tdm.historyTradeDatas.Count)
                mouseHoverID = -1;
            selectTradeIndex = mouseHoverID;
            return selectTradeIndex;
        }
        public void UnselectTradeData()
        {
            selectTradeIndex = -1;
        }
    }
}
#endif