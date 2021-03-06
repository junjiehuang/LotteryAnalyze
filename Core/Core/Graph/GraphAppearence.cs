﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    // 出号率曲线图
    public class GraphAppearence : GraphBase
    {
        public bool autoAllign = false;
        public bool onlyShowSelectCDTLine = true;
        public Dictionary<CollectDataType, bool> cdtLineShowStates = new Dictionary<CollectDataType, bool>();

        protected SolidBrush redBrush = new SolidBrush(Color.Red);
        protected SolidBrush whiteBrush = new SolidBrush(Color.White);
        protected SolidBrush greenBrush = new SolidBrush(Color.Green);
        protected SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
        protected Pen grayDotLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);
        protected Pen redPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 1);
        protected Pen yellowPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Yellow, 1);
        protected Pen whitePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);
        protected Font tagFont = new Font(FontFamily.GenericSerif, 12);
        protected Dictionary<Color, Brush> brushes = new Dictionary<Color, Brush>();
        protected Dictionary<Color, Pen> pens = new Dictionary<Color, Pen>();
        protected DataItem hoverItem;
        protected int selectDataIndex;

        public enum AppearenceType
        {
            eAppearenceFast,
            eAppearenceShort,
            eAppearenceLong,
            eAppearCountFast,
            eAppearCountShort,
            eAppearCountLong,
            eAppearenceMulti,
        }
        public static string[] AppearenceTypeStrs = new string[]
        {
            "统计5期的出号率",
            "统计10期的出号率",
            "统计30期的出号率",
            "统计5期的出号个数",
            "统计10期的出号个数",
            "统计30期的出号个数",
            "多周期的出号率曲线",
        };
        AppearenceType appearenceType = AppearenceType.eAppearenceFast;
        public AppearenceType AppearenceCycleType
        {
            get { return appearenceType; }
            set { appearenceType = value; }
        }


        public GraphAppearence()
        {
            selectDataIndex = -1;
            //gridScaleW = 10;
            gridScaleUp.X = gridScaleDown.X = 10;

            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                cdtLineShowStates.Add(GraphDataManager.S_CDT_LIST[i], false);
            }
        }

        public void GetViewItemIndexInfo(ref int startIndex, ref int maxIndex)
        {
            startIndex = (int)(canvasOffset.X / gridScaleUp.X) - 1;
            if (startIndex < 0)
                startIndex = 0;
            maxIndex = GraphDataManager.KGDC.DataLength();
        }

        public bool GetCDTLineShowState(CollectDataType cdt)
        {
            return cdtLineShowStates[cdt];
        }
        public void SetCDTLineShowState(CollectDataType cdt, bool v)
        {
            cdtLineShowStates[cdt] = v;
        }

        protected Brush GetBrush(CollectDataType cdt)
        {
            Color col = GraphDataManager.GetCdtColor(cdt);
            return GetBrush(col);
        }
        protected Brush GetBrush(Color col)
        {
            if (brushes.ContainsKey(col))
                return brushes[col];
            else
            {
                SolidBrush brush = new SolidBrush(col);
                brushes.Add(col, brush);
                return brush;
            }
        }
        protected Pen GetLinePen(CollectDataType cdt)
        {
            Color col = GraphDataManager.GetCdtColor(cdt);
            return GetLinePen(col);
        }
        protected Pen GetLinePen(Color col)
        {
            if (pens.ContainsKey(col))
                return pens[col];
            else
            {
                Pen pen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, col, 2);
                pens.Add(col, pen);
                return pen;
            }
        }

        public int SelectDataItem(Point mouseRelPos)
        {
            DataManager dm = DataManager.GetInst();
            selectDataIndex = -1;
            Point standMousePos = CanvasToStand(mouseRelPos, true);
            int mouseHoverID = (int)(standMousePos.X / gridScaleUp.X);
            if (mouseHoverID >= dm.GetAllDataItemCount())
                mouseHoverID = -1;
            selectDataIndex = mouseHoverID;
            return selectDataIndex;
        }
        public void UnselectDataItem()
        {
            selectDataIndex = -1;
        }

        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return true;
        }
        public override void MoveGraph(float dx, float dy)
        {
            canvasOffset.X += dx;
            canvasOffset.Y += dy;
            if (canvasOffset.X < 0)
                canvasOffset.X = 0;
        }

        public override void ResetGraphPosition()
        {

        }

        public override void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            hoverItem = null;
            bool isMouseAtRight = mouseRelPos.X > winW * 0.6f;
            float halfSize = gridScaleUp.X * 0.2f;
            if (halfSize < 1)
                halfSize = 1;
            float fullSize = halfSize * 2;
            float halfWinH = winH * 0.5f;
            float halfGridW = gridScaleUp.X * 0.5f;
            TradeDataManager tdm = TradeDataManager.Instance;
            DataManager dm = DataManager.GetInst();

            int startIndex = (int)(canvasOffset.X / gridScaleUp.X) - 1;
            if (startIndex < 0)
                startIndex = 0;
            int endIndex = (int)((canvasOffset.X + winW) / gridScaleUp.X) + 1;
            if (endIndex > dm.GetAllDataItemCount())
                endIndex = dm.GetAllDataItemCount();

            float top = winH * 0.1f;
            float bottom = winH * 0.9f;
            float maxHeight = bottom - top;
            float prevY = 0;
            float prevX = 0;
            bool hasChoose = false;

            if (onlyShowSelectCDTLine)
            {
                if (appearenceType == AppearenceType.eAppearenceMulti)
                {
                    DrawMultiCDTLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH, mouseRelPos);
                }
                else
                {
                    DrawSingleCDTLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH, mouseRelPos);
                }
                float tp = GraphDataManager.GetTheoryProbability(cdt);
                float tY = bottom - (bottom - top) * tp / 100;
                g.DrawLine(GetLinePen(cdt), 0, tY, winW, tY);
            }
            else
            {
                var etor = cdtLineShowStates.GetEnumerator();
                while (etor.MoveNext())
                {
                    CollectDataType type = etor.Current.Key;
                    if (etor.Current.Value)
                    {
                        DrawSingleCDTLine(g, numIndex, startIndex, endIndex, type, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH, mouseRelPos);
                    }
                }
            }

            g.DrawLine(grayDotLinePen, 0, mouseRelPos.Y, winW, mouseRelPos.Y);
            g.DrawLine(grayDotLinePen, 0, top, winW, top);
            g.DrawLine(grayDotLinePen, 0, bottom, winW, bottom);
            if (mouseRelPos.Y >= top && mouseRelPos.Y <= bottom)
            {
                float v = 0;
                if (appearenceType < AppearenceType.eAppearCountFast)
                    v = (bottom - mouseRelPos.Y) / (bottom - top) * 100.0f;
                else if (appearenceType == AppearenceType.eAppearCountFast)
                    v = (bottom - mouseRelPos.Y) / (bottom - top) * LotteryStatisticInfo.SAMPLE_COUNT_5;
                else if (appearenceType == AppearenceType.eAppearCountShort)
                    v = (bottom - mouseRelPos.Y) / (bottom - top) * LotteryStatisticInfo.SAMPLE_COUNT_10;
                else if (appearenceType == AppearenceType.eAppearCountLong)
                    v = (bottom - mouseRelPos.Y) / (bottom - top) * LotteryStatisticInfo.SAMPLE_COUNT_30;
                g.DrawString(v.ToString("f2") + "%", tagFont, whiteBrush, winW - 60, mouseRelPos.Y);
            }

            if (hoverItem != null)
            {
                StatisticUnit su = hoverItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt];
                float apprate = 0;
                int underTheoRateCount = 0;
                switch (appearenceType)
                {
                    case AppearenceType.eAppearCountFast:
                        apprate = (su.sample5Data.appearCount * 100 / LotteryStatisticInfo.SAMPLE_COUNT_5);
                        underTheoRateCount = su.sample5Data.underTheoryCount;
                        break;
                    case AppearenceType.eAppearCountShort:
                        apprate = (su.sample10Data.appearCount * 100 / LotteryStatisticInfo.SAMPLE_COUNT_10);
                        underTheoRateCount = su.sample10Data.underTheoryCount;
                        break;
                    case AppearenceType.eAppearCountLong:
                        apprate = (su.sample30Data.appearCount * 100 / LotteryStatisticInfo.SAMPLE_COUNT_30);
                        underTheoRateCount = su.sample30Data.underTheoryCount;
                        break;
                    case AppearenceType.eAppearenceFast:
                        apprate = su.sample5Data.appearProbability;
                        underTheoRateCount = su.sample5Data.underTheoryCount;
                        break;
                    case AppearenceType.eAppearenceShort:
                        apprate = su.sample10Data.appearProbability;
                        underTheoRateCount = su.sample10Data.underTheoryCount;
                        break;
                    case AppearenceType.eAppearenceLong:
                        apprate = su.sample30Data.appearProbability;
                        underTheoRateCount = su.sample30Data.underTheoryCount;
                        break;
                }
                string info = "[" + hoverItem.idTag + "] [" + hoverItem.lotteryNumber + "] [出号率 = " + apprate + "] [连续低于理论概率期数 = " + underTheoRateCount + "]";
                g.DrawString(info, tagFont, whiteBrush, 5, 5);
            }

            if (selectDataIndex != -1)
            {
                Pen pen = GetLinePen(Color.Yellow);
                float left = selectDataIndex * gridScaleUp.X;
                left = StandToCanvas(left, true, true);
                float right = left + gridScaleUp.X;
                g.DrawLine(pen, left, 0, left, winH);
                g.DrawLine(pen, right, 0, right, winH);
            }


        }
        public override void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {

        }
        public override void ScrollToData(int index, int winW, int winH, bool needSelect, int xOffset = 0, bool needScrollToData = true)
        {
            if (needSelect)
                selectDataIndex = index;
            else
                selectDataIndex = -1;
            if (needScrollToData)
            {
                canvasOffset.X = index * gridScaleUp.X + xOffset;
            }
            autoAllign = true;
        }

        void DrawMultiCDTLine(Graphics g, int numIndex, int startIndex, int endIndex, CollectDataType cdt, ref bool hasChoose, float bottom, float maxHeight, float halfGridW, float halfSize, float fullSize, int winH, Point mouseRelPos)
        {
            Brush[] brushs = { redBrush, yellowBrush, whiteBrush, };
            Pen[] pens = { redPen, yellowPen, whitePen, };

            float prevY = 0;
            float prevX = 0;
            DataManager dm = DataManager.GetInst();

            for (int j = 0; j < 3; ++j)
            {

                for (int i = startIndex; i < endIndex; ++i)
                {
                    DataItem item = dm.FindDataItem(i);
                    StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
                    float apr = 0;
                    if(j == 0)
                        apr = sum.statisticUnitMap[cdt].sample5Data.appearProbability;
                    else if (j == 1)
                        apr = sum.statisticUnitMap[cdt].sample10Data.appearProbability;
                    else
                        apr = sum.statisticUnitMap[cdt].sample30Data.appearProbability;

                    float x = i * gridScaleUp.X + halfGridW;
                    x = StandToCanvas(x, true, true);
                    
                    float rH = apr / 100 * maxHeight;
                    float rT = bottom - rH;

                    g.FillRectangle(brushs[j], x - halfSize, rT - halfSize, fullSize, fullSize);
                    if (i > startIndex)
                    {
                        g.DrawLine(pens[j], x, rT, prevX, prevY);
                    }
                    prevX = x;
                    prevY = rT;

                    float left = x - halfGridW;
                    float right = x + halfGridW;
                    if (hasChoose == false && mouseRelPos.X > left && mouseRelPos.X < right)
                    {
                        hoverItem = item;
                        g.DrawLine(grayDotLinePen, left, 0, left, winH);
                        g.DrawLine(grayDotLinePen, right, 0, right, winH);
                        hasChoose = true;
                    }
                }
            }
        }

        void DrawSingleCDTLine(Graphics g, int numIndex, int startIndex, int endIndex, CollectDataType cdt, ref bool hasChoose, float bottom, float maxHeight, float halfGridW, float halfSize, float fullSize, int winH, Point mouseRelPos)
        {
            float prevY = 0;
            float prevX = 0;
            DataManager dm = DataManager.GetInst();
            Brush brush = GetBrush(cdt);
            Pen pen = GetLinePen(cdt);
            for (int i = startIndex; i < endIndex; ++i)
            {
                DataItem item = dm.FindDataItem(i);
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];
                float apr = 0;
                switch (appearenceType)
                {
                    case AppearenceType.eAppearenceFast:
                        apr = sum.statisticUnitMap[cdt].sample5Data.appearProbability;
                        break;
                    case AppearenceType.eAppearenceShort:
                        apr = sum.statisticUnitMap[cdt].sample10Data.appearProbability;
                        break;
                    case AppearenceType.eAppearenceLong:
                        apr = sum.statisticUnitMap[cdt].sample30Data.appearProbability;
                        break;
                    case AppearenceType.eAppearCountFast:
                        apr = sum.statisticUnitMap[cdt].sample5Data.appearCount * 100 / LotteryStatisticInfo.SAMPLE_COUNT_5;
                        break;
                    case AppearenceType.eAppearCountShort:
                        apr = sum.statisticUnitMap[cdt].sample10Data.appearCount * 100 / LotteryStatisticInfo.SAMPLE_COUNT_10;
                        break;
                    case AppearenceType.eAppearCountLong:
                        apr = sum.statisticUnitMap[cdt].sample30Data.appearCount * 100 / LotteryStatisticInfo.SAMPLE_COUNT_30;
                        break;
                }
                float rH = apr / 100 * maxHeight;
                float rT = bottom - rH;
                float x = i * gridScaleUp.X + halfGridW;
                x = StandToCanvas(x, true, true);
                g.FillRectangle(brush, x - halfSize, rT - halfSize, fullSize, fullSize);
                if (i > startIndex)
                {
                    g.DrawLine(pen, x, rT, prevX, prevY);
                }
                prevX = x;
                prevY = rT;

                float left = x - halfGridW;
                float right = x + halfGridW;
                if (hasChoose == false && mouseRelPos.X > left && mouseRelPos.X < right)
                {
                    hoverItem = item;
                    g.DrawLine(grayDotLinePen, left, 0, left, winH);
                    g.DrawLine(grayDotLinePen, right, 0, right, winH);
                    hasChoose = true;
                }
            }
        }
    }
}
