using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterMiss : GraphPainterBase
{
    public bool onlyShowSelectCDTLine = true;
    public MissCountType missCountType = MissCountType.eMissCountValue;
    public AppearenceType appearenceType = AppearenceType.eAppearenceFast;
    public Dictionary<CollectDataType, bool> cdtLineShowStates = new Dictionary<CollectDataType, bool>();

    float DataMaxWidth = 0;
    int maxMissCount = 10;
    float maxMissCountArea = 10;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {
        int numIndex = PanelAnalyze.Instance.numIndex;
        CollectDataType cdt = PanelAnalyze.Instance.cdt;
        float winH = rtCanvas.rect.height;
        float winW = rtCanvas.rect.width;
        float halfSize = canvasUpScale.x * 0.2f;
        if (halfSize < 1)
            halfSize = 1;
        float fullSize = halfSize * 2;
        float halfWinH = winH * 0.5f;
        float halfGridW = canvasUpScale.x * 0.5f;
        DataManager dm = DataManager.GetInst();

        DataMaxWidth = canvasUpScale.x * dm.GetAllDataItemCount();

        int startIndex = 0;
        if (canvasUpOffset.x < 0)
        {
            startIndex = (int)(-canvasUpOffset.x / canvasUpScale.x) - 1;
            if (startIndex < 0)
                startIndex = 0;
        }
        if (startIndex > dm.GetAllDataItemCount())
            startIndex = dm.GetAllDataItemCount() - 1;
        int endIndex = startIndex + (int)(winW / canvasUpScale.x) + 1;
        if (endIndex > dm.GetAllDataItemCount())
            endIndex = dm.GetAllDataItemCount() - 1;

        float bottom = winH * 0.1f;
        float top = winH * 0.9f;
        if (winH - top < 60)
            top = winH - 60;
        float maxHeight = Mathf.Abs( bottom - top );
        float prevY = 0;
        float prevX = 0;
        bool hasChoose = false;

        if (onlyShowSelectCDTLine)
        {
            if (missCountType == MissCountType.eMissCountAreaMulti)
                DrawMultiMissCountLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH);
            else
                DrawSingleMissCountLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH);
        }
        else
        {
            var etor = cdtLineShowStates.GetEnumerator();
            while (etor.MoveNext())
            {
                CollectDataType type = etor.Current.Key;
                if (etor.Current.Value)
                {
                    DrawSingleMissCountLine(g, numIndex, startIndex, endIndex, type, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH);
                }
            }
        }
        
        g.DrawLineInCanvasSpace(0, top, winW, top, Color.gray);
        g.DrawLineInCanvasSpace(0, bottom, winW, bottom, Color.gray);

        DataItem hoverItem = GraphDataManager.BGDC.CurrentSelectItem;

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
                        info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].sample5Data.missCountArea +
                            "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].sample5Data.missCountArea +
                            "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].sample5Data.missCountArea + "]";
                    }
                    break;
                case MissCountType.eMissCountAreaShort:
                    {
                        info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].sample10Data.missCountArea +
                            "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].sample10Data.missCountArea +
                            "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].sample10Data.missCountArea + "]";
                    }
                    break;
                case MissCountType.eMissCountAreaLong:
                    {
                        info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].sample30Data.missCountArea +
                            "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].sample30Data.missCountArea +
                            "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].sample30Data.missCountArea + "]";
                    }
                    break;
                case MissCountType.eDisappearCountFast:
                    {
                        info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].sample5Data.disappearCount +
                            "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].sample5Data.disappearCount +
                            "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].sample5Data.disappearCount + "]";
                    }
                    break;
                case MissCountType.eDisappearCountShort:
                    {
                        info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].sample10Data.disappearCount +
                            "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].sample10Data.disappearCount +
                            "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].sample10Data.disappearCount + "]";
                    }
                    break;
                case MissCountType.eDisappearCountLong:
                    {
                        info += "[0 - " + sum.statisticUnitMap[CollectDataType.ePath0].sample30Data.disappearCount +
                            "] [1 - " + sum.statisticUnitMap[CollectDataType.ePath1].sample30Data.disappearCount +
                            "] [2 - " + sum.statisticUnitMap[CollectDataType.ePath2].sample30Data.disappearCount + "]";
                    }
                    break;
            }
            PanelAnalyze.Instance.graphUp.AppendText(info);
        }

        int selectDataIndex = PanelAnalyze.Instance.SelectKDataIndex;
        if (selectDataIndex >= 0)
        {
            float left = selectDataIndex * canvasUpScale.x;
            left = g.StandToCanvas(left, true);
            float right = left + canvasUpScale.x;
            g.DrawLineInCanvasSpace(left, 0, left, winH, Color.yellow);
            g.DrawLineInCanvasSpace(right, 0, right, winH, Color.yellow);
        }

    }

    public override void DrawDownPanel(Painter g, RectTransform rtCanvas)
    {
        //base.DrawDownPanel(g, rtCanvas);
        int numIndex = PanelAnalyze.Instance.numIndex;
        CollectDataType cdt = PanelAnalyze.Instance.cdt;
        float winH = rtCanvas.rect.height;
        float winW = rtCanvas.rect.width;
        float halfSize = canvasUpScale.x * 0.2f;
        if (halfSize < 1)
            halfSize = 1;
        float fullSize = halfSize * 2;
        float halfWinH = winH * 0.5f;
        float halfGridW = canvasUpScale.x * 0.5f;
        DataManager dm = DataManager.GetInst();

        DataMaxWidth = canvasUpScale.x * dm.GetAllDataItemCount();

        int startIndex = 0;
        if (canvasUpOffset.x < 0)
        {
            startIndex = (int)(-canvasUpOffset.x / canvasUpScale.x) - 1;
            if (startIndex < 0)
                startIndex = 0;
        }
        if (startIndex > dm.GetAllDataItemCount())
            startIndex = dm.GetAllDataItemCount() - 1;
        int endIndex = startIndex + (int)(winW / canvasUpScale.x) + 1;
        if (endIndex > dm.GetAllDataItemCount())
            endIndex = dm.GetAllDataItemCount() - 1;

        float bottom = winH * 0.1f;
        float top = winH * 0.9f;
        if (winH - top < 60)
            top = winH - 60;
        float maxHeight = Mathf.Abs(bottom - top);
        float prevY = 0;
        float prevX = 0;
        bool hasChoose = false;

        if (onlyShowSelectCDTLine)
        {
            if (appearenceType == AppearenceType.eAppearenceMulti)
            {
                DrawMultiCDTLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH);
            }
            else
            {
                DrawSingleCDTLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH);
            }
            float tp = GraphDataManager.GetTheoryProbability(cdt);
            float tY = bottom - (bottom - top) * tp / 100;
            Color col = GraphDataManager.GetColorByCDT(cdt);
            g.DrawLineInCanvasSpace(0, tY, winW, tY, col);
        }
        else
        {
            var etor = cdtLineShowStates.GetEnumerator();
            while (etor.MoveNext())
            {
                CollectDataType type = etor.Current.Key;
                if (etor.Current.Value)
                {
                    DrawSingleCDTLine(g, numIndex, startIndex, endIndex, type, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH);
                }
            }
        }

        g.DrawLineInCanvasSpace(0, top, winW, top, Color.gray);
        g.DrawLineInCanvasSpace(0, bottom, winW, bottom, Color.gray);
        //g.DrawLine(grayDotLinePen, 0, mouseRelPos.Y, winW, mouseRelPos.Y);
        //g.DrawLine(grayDotLinePen, 0, top, winW, top);
        //g.DrawLine(grayDotLinePen, 0, bottom, winW, bottom);
        //if (mouseRelPos.Y >= top && mouseRelPos.Y <= bottom)
        //{
        //    float v = 0;
        //    if (appearenceType < AppearenceType.eAppearCountFast)
        //        v = (bottom - mouseRelPos.Y) / (bottom - top) * 100.0f;
        //    else if (appearenceType == AppearenceType.eAppearCountFast)
        //        v = (bottom - mouseRelPos.Y) / (bottom - top) * LotteryStatisticInfo.SAMPLE_COUNT_5;
        //    else if (appearenceType == AppearenceType.eAppearCountShort)
        //        v = (bottom - mouseRelPos.Y) / (bottom - top) * LotteryStatisticInfo.SAMPLE_COUNT_10;
        //    else if (appearenceType == AppearenceType.eAppearCountLong)
        //        v = (bottom - mouseRelPos.Y) / (bottom - top) * LotteryStatisticInfo.SAMPLE_COUNT_30;
        //    g.DrawString(v.ToString("f2") + "%", tagFont, whiteBrush, winW - 60, mouseRelPos.Y);
        //}

        DataItem hoverItem = GraphDataManager.BGDC.CurrentSelectItem;
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
            //g.DrawString(info, tagFont, whiteBrush, 5, 5);
            PanelAnalyze.Instance.graphDown.AppendText(info);
        }

        //if (selectDataIndex != -1)
        //{
        //    Pen pen = GetLinePen(Color.Yellow);
        //    float left = selectDataIndex * gridScaleUp.X;
        //    left = StandToCanvas(left, true, true);
        //    float right = left + gridScaleUp.X;
        //    g.DrawLine(pen, left, 0, left, winH);
        //    g.DrawLine(pen, right, 0, right, winH);
        //}
        int selectDataIndex = PanelAnalyze.Instance.SelectKDataIndex;
        if (selectDataIndex >= 0)
        {
            float left = selectDataIndex * canvasUpScale.x;
            left = g.StandToCanvas(left, true);
            float right = left + canvasUpScale.x;
            g.DrawLineInCanvasSpace(left, 0, left, winH, Color.yellow);
            g.DrawLineInCanvasSpace(right, 0, right, winH, Color.yellow);
        }

    }

    public override void OnAutoAllign()
    {
        base.OnAutoAllign();
    }

    public override void OnGraphDragging(Vector2 offset, Painter painter)
    {
        base.OnGraphDragging(offset, painter);
    }

    public override void OnPointerClick(Vector2 pos, Painter g)
    {
        base.OnPointerClick(pos, g);
        if (g == upPainter)
        {
            float x = upPainter.CanvasToStand(pos.x, true);
            if (x >= 0)// && x <= DataMaxWidth)
            {
                PanelAnalyze.Instance.SelectKDataIndex = Mathf.FloorToInt(x / canvasUpScale.x);
                if (PanelAnalyze.Instance.SelectKDataIndex >= DataManager.GetInst().GetAllDataItemCount())
                    PanelAnalyze.Instance.SelectKDataIndex = DataManager.GetInst().GetAllDataItemCount() - 1;
            }
            else
            {
                PanelAnalyze.Instance.SelectKDataIndex = -1;
            }
        }
        else
        {
            float x = downPainter.CanvasToStand(pos.x, true);
            if (x >= 0)// && x <= DataMaxWidth)
            {
                PanelAnalyze.Instance.SelectKDataIndex = Mathf.FloorToInt(x / canvasDownScale.x);
                if (PanelAnalyze.Instance.SelectKDataIndex >= DataManager.GetInst().GetAllDataItemCount())
                    PanelAnalyze.Instance.SelectKDataIndex = DataManager.GetInst().GetAllDataItemCount() - 1;
            }
            else
            {
                PanelAnalyze.Instance.SelectKDataIndex = -1;
            }
        }
        PanelAnalyze.Instance.NotifyUIRepaint();

        if (PanelAnalyze.Instance.SelectKDataIndex == -1)
            GraphDataManager.BGDC.CurrentSelectItem = DataManager.GetInst().GetLatestItem();
        else
            GraphDataManager.BGDC.CurrentSelectItem = DataManager.GetInst().FindDataItem(PanelAnalyze.Instance.SelectKDataIndex);
        GraphDataManager.Instance.CollectGraphData(GraphType.eBarGraph);
    }

    public override void OnScrollToData(int dataIndex)
    {
        base.OnScrollToData(dataIndex);
    }

    void DrawMultiMissCountLine(Painter g, int numIndex, int startIndex, int endIndex, CollectDataType cdt, ref bool hasChoose, float bottom, float maxHeight, float halfGridW, float halfSize, float fullSize, float winH)
    {
        float gridH = gridH = maxHeight / maxMissCountArea;
        float prevY = 0;
        float prevX = 0;

        DataManager dm = DataManager.GetInst();
        Color[] cols = { Color.red, Color.yellow, Color.white, };

        for (int j = 0; j < 3; ++j)
        {
            for (int i = startIndex; i < endIndex; ++i)
            {
                DataItem item = dm.FindDataItem(i);
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];

                float CUR = 1;
                float curMissCountArea = 0;
                if (j == 0)
                    curMissCountArea = sum.statisticUnitMap[cdt].sample5Data.missCountArea;
                else if (j == 1)
                    curMissCountArea = sum.statisticUnitMap[cdt].sample10Data.missCountArea;
                else if (j == 2)
                    curMissCountArea = sum.statisticUnitMap[cdt].sample30Data.missCountArea;

                if (maxMissCountArea < curMissCountArea)
                    maxMissCountArea = curMissCountArea;
                CUR = curMissCountArea;

                float rH = CUR * gridH;
                float rT = bottom + rH;
                float x = i * canvasUpScale.x + halfGridW;
                x = g.StandToCanvas(x, true);
                g.DrawFillRectInCanvasSpace(x - halfSize, rT - halfSize, fullSize, fullSize, cols[i]);
                if (i > startIndex)
                {
                    g.DrawLineInCanvasSpace(x, rT, prevX, prevY, cols[i]);
                }
                prevX = x;
                prevY = rT;

                float left = x - halfGridW;
                float right = x + halfGridW;
                if (PanelAnalyze.Instance.SelectKDataIndex == i)
                {
                    g.DrawLineInCanvasSpace(left, 0, left, winH, Color.gray);
                    g.DrawLineInCanvasSpace(right, 0, right, winH, Color.gray);
                }
            }
        }
    }

    void DrawSingleMissCountLine(Painter g, int numIndex, int startIndex, int endIndex, CollectDataType cdt, ref bool hasChoose, float bottom, float maxHeight, float halfGridW, float halfSize, float fullSize, float winH)
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
        UnityEngine.Color col = GraphDataManager.GetColorByCDT(cdt);
        for (int i = startIndex; i < endIndex; ++i)
        {
            DataItem item = dm.FindDataItem(i);
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[numIndex];

            float CUR = 1;
            if (missCountType == MissCountType.eMissCountValue)
            {
                int curMissCount = sum.statisticUnitMap[cdt].missCount;
                if (maxMissCount < curMissCount)
                {
                    maxMissCount = curMissCount;
                }
                CUR = curMissCount;
            }
            else if (missCountType == MissCountType.eDisappearCountFast)
            {
                CUR = sum.statisticUnitMap[cdt].sample5Data.disappearCount;
            }
            else if (missCountType == MissCountType.eDisappearCountShort)
            {
                CUR = sum.statisticUnitMap[cdt].sample10Data.disappearCount;
            }
            else if (missCountType == MissCountType.eDisappearCountLong)
            {
                CUR = sum.statisticUnitMap[cdt].sample30Data.disappearCount;
            }
            else
            {
                float curMissCountArea = 0;
                if (missCountType == MissCountType.eMissCountAreaFast)
                    curMissCountArea = sum.statisticUnitMap[cdt].sample5Data.missCountArea;
                else if (missCountType == MissCountType.eMissCountAreaShort)
                    curMissCountArea = sum.statisticUnitMap[cdt].sample10Data.missCountArea;
                else if (missCountType == MissCountType.eMissCountAreaLong)
                    curMissCountArea = sum.statisticUnitMap[cdt].sample30Data.missCountArea;

                if (maxMissCountArea < curMissCountArea)
                    maxMissCountArea = curMissCountArea;
                CUR = curMissCountArea;
            }

            float rH = CUR * gridH;
            float rT = bottom + rH;
            float x = i * canvasUpScale.x + halfGridW;
            x = g.StandToCanvas(x, true);
            g.DrawFillRectInCanvasSpace(x - halfSize, rT - halfSize, fullSize, fullSize, col);
            if (i > startIndex)
            {
                g.DrawLineInCanvasSpace(x, rT, prevX, prevY, col);
            }
            prevX = x;
            prevY = rT;

            float left = x - halfGridW;
            float right = x + halfGridW;
            //if (hasChoose == false && mouseRelPos.X > left && mouseRelPos.X < right)
            //{
            //    hoverItem = item;
            //    g.DrawLine(grayDotLinePen, left, 0, left, winH);
            //    g.DrawLine(grayDotLinePen, right, 0, right, winH);
            //    hasChoose = true;
            //}
            if(PanelAnalyze.Instance.SelectKDataIndex == i)
            {
                g.DrawLineInCanvasSpace(left, 0, left, winH, Color.gray);
                g.DrawLineInCanvasSpace(right, 0, right, winH, Color.gray);
            }
        }
    }

    void DrawMultiCDTLine(Painter g, int numIndex, int startIndex, int endIndex, CollectDataType cdt, ref bool hasChoose, float bottom, float maxHeight, float halfGridW, float halfSize, float fullSize, float winH)
    {
        //Brush[] brushs = { redBrush, yellowBrush, whiteBrush, };
        //Pen[] pens = { redPen, yellowPen, whitePen, };
        Color[] cols = { Color.red, Color.yellow, Color.white, };

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
                if (j == 0)
                    apr = sum.statisticUnitMap[cdt].sample5Data.appearProbability;
                else if (j == 1)
                    apr = sum.statisticUnitMap[cdt].sample10Data.appearProbability;
                else
                    apr = sum.statisticUnitMap[cdt].sample30Data.appearProbability;

                float x = i * canvasDownScale.x + halfGridW;
                x = g.StandToCanvas(x, true);

                float rH = apr / 100 * maxHeight;
                float rT = bottom + rH;

                g.DrawFillRectInCanvasSpace(x - halfSize, rT - halfSize, fullSize, fullSize, cols[j]);
                if (i > startIndex)
                {
                    g.DrawLineInCanvasSpace(x, rT, prevX, prevY, cols[j]);
                }
                prevX = x;
                prevY = rT;

                //float left = x - halfGridW;
                //float right = x + halfGridW;
                //if (hasChoose == false && mouseRelPos.X > left && mouseRelPos.X < right)
                //{
                //    hoverItem = item;
                //    g.DrawLine(grayDotLinePen, left, 0, left, winH);
                //    g.DrawLine(grayDotLinePen, right, 0, right, winH);
                //    hasChoose = true;
                //}
            }
        }
    }

    void DrawSingleCDTLine(Painter g, int numIndex, int startIndex, int endIndex, CollectDataType cdt, ref bool hasChoose, float bottom, float maxHeight, float halfGridW, float halfSize, float fullSize, float winH)
    {
        float prevY = 0;
        float prevX = 0;
        DataManager dm = DataManager.GetInst();
        Color col = GraphDataManager.GetColorByCDT(cdt);
        //Brush brush = GetBrush(cdt);
        //Pen pen = GetLinePen(cdt);
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
            float rT = bottom + rH;
            float x = i * canvasDownScale.x + halfGridW;
            x = g.StandToCanvas(x, true);
            g.DrawFillRectInCanvasSpace(x - halfSize, rT - halfSize, fullSize, fullSize, col);
            if (i > startIndex)
            {
                g.DrawLineInCanvasSpace(x, rT, prevX, prevY, col);
            }
            prevX = x;
            prevY = rT;

            //float left = x - halfGridW;
            //float right = x + halfGridW;
            //if (hasChoose == false && mouseRelPos.X > left && mouseRelPos.X < right)
            //{
            //    hoverItem = item;
            //    g.DrawLine(grayDotLinePen, left, 0, left, winH);
            //    g.DrawLine(grayDotLinePen, right, 0, right, winH);
            //    hasChoose = true;
            //}
        }
    }

}
