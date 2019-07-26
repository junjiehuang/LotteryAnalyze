using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    public class GraphMissCount : GraphAppearence
    {
        public enum MissCountType
        {
            eMissCountValue,
            eMissCountAreaFast,
            eMissCountAreaShort,
            eMissCountAreaLong,
            eDisappearCountFast,
            eDisappearCountShort,
            eDisappearCountLong,
            eMissCountAreaMulti,
        }
        public static string[] MissCountTypeStrs = new string[]
        {
            "遗漏值",
            "统计5期的遗漏均线",
            "统计10期的遗漏均线",
            "统计30期的遗漏均线",
            "统计5期的遗漏数",
            "统计10期的遗漏数",
            "统计30期的遗漏数",
            "统计多周期的遗漏均线",
        };
        MissCountType _missCountType = MissCountType.eMissCountValue;
        public MissCountType missCountType
        {
            get
            {
                return _missCountType;
            }
            set
            {
                _missCountType = value;
                if (_missCountType == MissCountType.eDisappearCountFast)
                    maxMissCount = LotteryStatisticInfo.FAST_COUNT;
                else if (_missCountType == MissCountType.eDisappearCountShort)
                    maxMissCount = LotteryStatisticInfo.SHOR_COUNT;
                else if (_missCountType == MissCountType.eDisappearCountLong)
                    maxMissCount = LotteryStatisticInfo.LONG_COUNT;
                else if (_missCountType == MissCountType.eMissCountValue)
                    maxMissCount = 10;
                else
                    maxMissCountArea = 10;
            }
        }

        int maxMissCount = 10;
        float maxMissCountArea = 10;

        public GraphMissCount() : base()
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
                if (missCountType == MissCountType.eMissCountAreaMulti)
                    DrawMultiMissCountLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH, mouseRelPos);
                else
                    DrawSingleMissCountLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH, mouseRelPos);
            }
            else
            {
                var etor = cdtLineShowStates.GetEnumerator();
                while (etor.MoveNext())
                {
                    CollectDataType type = etor.Current.Key;
                    if (etor.Current.Value)
                    {
                        DrawSingleMissCountLine(g, numIndex, startIndex, endIndex, type, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH, mouseRelPos);
                    }
                }
            }

            g.DrawLine(grayDotLinePen, 0, mouseRelPos.Y, winW, mouseRelPos.Y);
            g.DrawLine(grayDotLinePen, 0, top, winW, top);
            g.DrawLine(grayDotLinePen, 0, bottom, winW, bottom);
            if (mouseRelPos.Y >= top && mouseRelPos.Y <= bottom)
            {
                float gridH = maxHeight / (missCountType != MissCountType.eMissCountValue ? maxMissCountArea : maxMissCount);
                float mouseMissCount = ((bottom - mouseRelPos.Y) / gridH);
                g.DrawString(mouseMissCount.ToString("f1"), tagFont, whiteBrush, winW - 60, mouseRelPos.Y);
            }

            if (hoverItem != null)
            {
                string info = "[" + hoverItem.idTag + "] [" + hoverItem.lotteryNumber + "] ";
                StatisticUnitMap sum = hoverItem.statisticInfo.allStatisticInfo[numIndex];
                switch (missCountType)
                {
                    case MissCountType.eMissCountValue:
                        {
                            info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].missCount +
                                "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].missCount +
                                "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].missCount + "]";
                        }
                        break;
                    case MissCountType.eMissCountAreaFast:
                        {
                            info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].fastData.missCountArea +
                                "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].fastData.missCountArea +
                                "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].fastData.missCountArea + "]";
                        }
                        break;
                    case MissCountType.eMissCountAreaShort:
                        {
                            info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].shortData.missCountArea +
                                "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].shortData.missCountArea +
                                "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].shortData.missCountArea + "]";
                        }
                        break;
                    case MissCountType.eMissCountAreaLong:
                        {
                            info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].longData.missCountArea +
                                "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].longData.missCountArea +
                                "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].longData.missCountArea + "]";
                        }
                        break;
                    case MissCountType.eDisappearCountFast:
                        {
                            info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].fastData.disappearCount +
                                "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].fastData.disappearCount +
                                "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].fastData.disappearCount + "]";
                        }
                        break;
                    case MissCountType.eDisappearCountShort:
                        {
                            info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].shortData.disappearCount +
                                "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].shortData.disappearCount +
                                "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].shortData.disappearCount + "]";
                        }
                        break;
                    case MissCountType.eDisappearCountLong:
                        {
                            info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].longData.disappearCount +
                                "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].longData.disappearCount +
                                "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].longData.disappearCount + "]";
                        }
                        break;
                }
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

        void DrawMultiMissCountLine(Graphics g, int numIndex, int startIndex, int endIndex, CollectDataType cdt, ref bool hasChoose, float bottom, float maxHeight, float halfGridW, float halfSize, float fullSize, int winH, Point mouseRelPos)
        {
            float gridH = gridH = maxHeight / maxMissCountArea;
            float prevY = 0;
            float prevX = 0;

            DataManager dm = DataManager.GetInst();
            Brush[] brushs = { redBrush, yellowBrush, whiteBrush, };
            Pen[] pens = { redPen, yellowPen, whitePen, };

            for (int j = 0; j < 3; ++j)
            {
                for (int i = startIndex; i < endIndex; ++i)
                {
                    DataItem item = dm.FindDataItem(i);
                    StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];

                    float CUR = 1;
                    float curMissCountArea = 0;
                    if (j == 0)
                        curMissCountArea = sum.statisticUnitMap[cdt].fastData.missCountArea;
                    else if (j == 1)
                        curMissCountArea = sum.statisticUnitMap[cdt].shortData.missCountArea;
                    else if (j == 2)
                        curMissCountArea = sum.statisticUnitMap[cdt].longData.missCountArea;

                    if (maxMissCountArea < curMissCountArea)
                        maxMissCountArea = curMissCountArea;
                    CUR = curMissCountArea;

                    float rH = CUR * gridH;
                    float rT = bottom - rH;
                    float x = i * gridScaleUp.X + halfGridW;
                    x = StandToCanvas(x, true, true);
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

        void DrawSingleMissCountLine(Graphics g, int numIndex, int startIndex, int endIndex, CollectDataType cdt, ref bool hasChoose, float bottom, float maxHeight, float halfGridW, float halfSize, float fullSize, int winH, Point mouseRelPos)
        {
            float gridH = 1;
            if (missCountType == MissCountType.eMissCountValue ||
                missCountType == MissCountType.eDisappearCountFast ||
                missCountType == MissCountType.eDisappearCountShort ||
                missCountType == MissCountType.eDisappearCountLong)
                gridH = maxHeight / maxMissCount;
            else
                gridH = maxHeight / maxMissCountArea;
            float prevY = 0;
            float prevX = 0;
            DataManager dm = DataManager.GetInst();
            Brush brush = GetBrush(cdt);
            Pen pen = GetLinePen(cdt);
            for (int i = startIndex; i < endIndex; ++i)
            {
                DataItem item = dm.FindDataItem(i);
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];

                float CUR = 1;
                if (missCountType == MissCountType.eMissCountValue)
                {
                    int curMissCount = sum.statisticUnitMap[cdt].missCount;
                    if (maxMissCount < curMissCount)
                        maxMissCount = curMissCount;
                    CUR = curMissCount;
                }
                else if (missCountType == MissCountType.eDisappearCountFast)
                {
                    CUR = sum.statisticUnitMap[cdt].fastData.disappearCount;
                }
                else if (missCountType == MissCountType.eDisappearCountShort)
                {
                    CUR = sum.statisticUnitMap[cdt].shortData.disappearCount;
                }
                else if (missCountType == MissCountType.eDisappearCountLong)
                {
                    CUR = sum.statisticUnitMap[cdt].longData.disappearCount;
                }
                else
                {
                    float curMissCountArea = 0;
                    if (missCountType == MissCountType.eMissCountAreaFast)
                        curMissCountArea = sum.statisticUnitMap[cdt].fastData.missCountArea;
                    else if (missCountType == MissCountType.eMissCountAreaShort)
                        curMissCountArea = sum.statisticUnitMap[cdt].shortData.missCountArea;
                    else if (missCountType == MissCountType.eMissCountAreaLong)
                        curMissCountArea = sum.statisticUnitMap[cdt].longData.missCountArea;

                    if (maxMissCountArea < curMissCountArea)
                        maxMissCountArea = curMissCountArea;
                    CUR = curMissCountArea;
                }

                float rH = CUR * gridH;
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
