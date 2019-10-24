using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterKData : GraphPainterBase
{
    public bool enableMACD = true;
    public bool enableBollinBand = true;
    public bool enableKRuler = true;
    public bool enableAvgLines = true;
    public bool enableAuxLine = true;

    public bool enableAvg5 = true;
    public bool enableAvg10 = true;
    public bool enableAvg20 = true;
    public bool enableAvg30 = true;
    public bool enableAvg50 = true;
    public bool enableAvg100 = true;

    int startItemIndex;
    bool autoAllign = false;
    int selectKDataIndex = -1;
    List<int> predict_results = new List<int>();

    float lastValue = 0;
    bool findPrevPt = false;
    Vector2 prevPt = Vector2.zero;

    float DownGraphYOffset;
    float DataMaxWidth = 0;

    public override void Start()
    {
        base.Start();

        canvasUpOffset.y = PanelAnalyze.Instance.splitPanel.panelLTSize.y * 0.5f;
        canvasDownOffset.y = PanelAnalyze.Instance.splitPanel.panelRBSize.y * 0.5f;
    }

    public override void Update()
    {
        base.Update();

        //upPainter.DrawFillRect(5, 5, 100, 200, Color.blue);
        //upPainter.DrawFillRectInCanvasSpace(5, 200, 100, 30, Color.white);
        //downPainter.DrawFillRect(50, 5, 100, 200, Color.green);
        //upPainter.DrawLine(5, 5, 100, 200, Color.white);
    }

    public override void OnPointerClick(Vector2 pos, Painter g)
    {
        base.OnPointerClick(pos, g);
        if(g == upPainter)
        {
            float x = upPainter.CanvasToStand(pos.x, true);
            if(x >= 0 && x <= DataMaxWidth)
            {
                selectKDataIndex = Mathf.FloorToInt(x / canvasUpScale.x);
            }
            else
            {
                selectKDataIndex = -1;
            }
        }
        else
        {
            float x = downPainter.CanvasToStand(pos.x, true);
            if (x >= 0 && x <= DataMaxWidth)
            {
                selectKDataIndex = Mathf.FloorToInt(x / canvasDownScale.x);
            }
            else
            {
                selectKDataIndex = -1;
            }
        }
        PanelAnalyze.Instance.NotifyUIRepaint();
    }

    bool CheckAvgShow(AvgDataContainer adc)
    {
        switch(adc.cycle)
        {
            case 5:return enableAvg5;
            case 10:return enableAvg10;
            case 20:return enableAvg20;
            case 30:return enableAvg30;
            case 50:return enableAvg50;
            case 100:return enableAvg100;
        }
        return false;
    }

    public override void OnAutoAllign()
    {
        base.OnAutoAllign();

        int numIndex = PanelAnalyze.Instance.numIndex;
        CollectDataType cdt = PanelAnalyze.Instance.cdt;

        KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
        int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
        float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];
        if (kddc != null)
        {
            if (kddc.dataLst.Count > 0)
            {
                int startIndex = 0;
                if (canvasUpOffset.x < 0)
                {
                    startIndex = (int)(-canvasUpOffset.x / canvasUpScale.x) - 1;
                    if (startIndex < 0)
                        startIndex = 0;
                }

                KDataMap kdDict = kddc.dataLst[startIndex];
                KData data = kdDict.GetData(cdt, false);
                float y = data.KValue * canvasUpScale.y;
                float canvasY = upPainter.StandToCanvas(y, false);
                canvasUpOffset.y += PanelAnalyze.Instance.graphUp.rectTransform.rect.height * 0.5f - canvasY;
                PanelAnalyze.Instance.NotifyUIRepaint();
            }
        }
    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {
        DataMaxWidth = 0;
        float MaxLen = 99999;
        g.DrawLine(-MaxLen, 0, MaxLen, 0, Color.gray);
        g.DrawLine(0, -MaxLen, 0, MaxLen, Color.gray);

        int numIndex = PanelAnalyze.Instance.numIndex;
        CollectDataType cdt = PanelAnalyze.Instance.cdt;

        KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
        int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
        float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];
        if (kddc != null)
        {
            if (kddc.dataLst.Count > 0)
            {
                DataMaxWidth = kddc.dataLst.Count * canvasUpScale.x;
                int startIndex = 0;
                if (canvasUpOffset.x < 0)
                {
                    startIndex = (int)(-canvasUpOffset.x / canvasUpScale.x) - 1;
                    if (startIndex < 0)
                        startIndex = 0;
                }
                int endIndex = startIndex + (int)(rtCanvas.rect.width / canvasUpScale.x) + 1;
                if (PanelAnalyze.Instance.endShowDataItemIndex != -1)
                {
                    if (endIndex > PanelAnalyze.Instance.endShowDataItemIndex + 1)
                        endIndex = PanelAnalyze.Instance.endShowDataItemIndex + 1;
                }
                if (endIndex > kddc.dataLst.Count)
                    endIndex = kddc.dataLst.Count;

                startItemIndex = startIndex;

                // 画K线图
                for (int i = startIndex; i < endIndex; ++i)
                {
                    KDataMap kdDict = kddc.dataLst[i];
                    KData data = kdDict.GetData(cdt, false);
                    if (data == null)
                        continue;
                    DrawKDataGraph(g, rtCanvas, data, missRelHeight, kddc, cdt);

                    if (selectKDataIndex == i)
                    {
                        float xL = g.StandToCanvas(selectKDataIndex * canvasUpScale.x, true);
                        float xR = xL + canvasUpScale.x;
                        g.DrawLineInCanvasSpace(xL, 0, xL, rtCanvas.rect.height, Color.yellow);
                        g.DrawLineInCanvasSpace(xR, 0, xR, rtCanvas.rect.height, Color.yellow);
                        
                        string info = data.GetInfo() + 
                            "K值 = " + data.KValue.ToString() +
                            ", 上 = " + data.UpValue.ToString() +
                            ", 下 = " + data.DownValue.ToString() +
                            ", 开 = " + data.StartValue.ToString() +
                            ", 收 = " + data.EndValue.ToString();
                        PanelAnalyze.Instance.graphUp.AppendText(info);
                    }
                }
                
                // 画预测标尺
                if (enableKRuler)
                {
                    DrawKRuler(g, numIndex, cdt, rtCanvas, kddc, missRelHeight);
                }

                // 画均线图
                if (enableAvgLines)
                {
                    foreach (AvgDataContainer adc in kddc.avgDataContMap.Values)
                    {
                        if (CheckAvgShow(adc))
                        {
                            lastValue = 0;
                            findPrevPt = false;

                            for (int i = startIndex; i < endIndex; ++i)
                            {
                                DrawAvgLineGraph(g, adc.avgPointMapLst[i], rtCanvas, cdt, adc.avgLineSetting.color);
                            }
                        }
                    }
                }

                // 画布林带
                if (enableBollinBand)
                {
                    lastValue = 0;
                    findPrevPt = false;
                    endIndex = kddc.bollinDataLst.bollinMapLst.Count;
                    if (PanelAnalyze.Instance.endShowDataItemIndex != -1)
                    {
                        if (endIndex > PanelAnalyze.Instance.endShowDataItemIndex + 1)
                            endIndex = PanelAnalyze.Instance.endShowDataItemIndex + 1;
                    }
                    for (int i = 0; i < endIndex; ++i)
                    {
                        DrawBollinLineGraph(g, kddc.bollinDataLst.bollinMapLst[i], rtCanvas, cdt);
                    }
                }

                // 画预测结果
                for (int i = startIndex; i < endIndex; ++i)
                {
                    KDataMap kdDict = kddc.dataLst[i];
                    KData data = kdDict.GetData(cdt, false);
                    if (data == null)
                        continue;
                    DrawHotNumsPredictResult(g, rtCanvas, data, missRelHeight, kddc, cdt);
                }
            }
        }
    }

    public override void DrawDownPanel(Painter g, RectTransform rtCanvas)
    {
        float MaxLen = 99999;
        g.DrawLine(-MaxLen, 0, MaxLen, 0, Color.gray);
        g.DrawLine(0, -MaxLen, 0, MaxLen, Color.gray);

        int numIndex = PanelAnalyze.Instance.numIndex;
        CollectDataType cdt = PanelAnalyze.Instance.cdt;
        KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
        
        if (enableMACD && kddc != null)
        {
            int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
            float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];

            float oriYOff = canvasDownOffset.y;
            canvasDownOffset.y = rtCanvas.rect.height * 0.5f + DownGraphYOffset;
            lastValue = 0;
            findPrevPt = false;

            int startIndex = 0;
            if (canvasDownOffset.x < 0)
            {
                startIndex = (int)(-canvasDownOffset.x / canvasDownScale.x) - 1;
                if (startIndex < 0)
                    startIndex = 0;
            }
            int endIndex = startIndex + (int)(rtCanvas.rect.width / canvasDownScale.x) + 1;
            if (PanelAnalyze.Instance.endShowDataItemIndex != -1)
            {
                if (endIndex > PanelAnalyze.Instance.endShowDataItemIndex + 1)
                    endIndex = PanelAnalyze.Instance.endShowDataItemIndex + 1;
            }
            if (endIndex > kddc.macdDataLst.macdMapLst.Count)
                endIndex = kddc.macdDataLst.macdMapLst.Count;

            for (int i = startIndex; i < endIndex; ++i)
            {
                DrawMACDGraph(g, kddc.macdDataLst.macdMapLst[i], rtCanvas, cdt, i == selectKDataIndex);

                if (selectKDataIndex == i)
                {
                    float xL = g.StandToCanvas(selectKDataIndex * canvasDownScale.x, true);
                    float xR = xL + canvasDownScale.x;
                    g.DrawLineInCanvasSpace(xL, 0, xL, rtCanvas.rect.height, Color.yellow);
                    g.DrawLineInCanvasSpace(xR, 0, xR, rtCanvas.rect.height, Color.yellow);

                    MACDPointMap mpm = kddc.macdDataLst.macdMapLst[selectKDataIndex];
                    MACDPoint mp = mpm.GetData(cdt, false);
                    string info = mpm.index + ", MACD 快线值 = " + mp.DIF + ", 慢线值 = " + mp.DEA + ", 柱值 = " + mp.BAR + "\n";
                    PanelAnalyze.Instance.graphDown.AppendText(info);
                }
            }
            
            canvasDownOffset.y = oriYOff;

            //if (preViewDataIndex != -1)
            //{
            //    g.DrawLine(grayDotLinePen, selDataPtX, 0, selDataPtX, winH);
            //    g.DrawLine(grayDotLinePen, selDataPtX + gridScaleDown.X, 0, selDataPtX + gridScaleDown.X, winH);
            //}

            //// 画辅助线
            //if (enableAuxiliaryLine)
            //{
            //    DrawAuxLineGraph(g, winW, winH, mouseRelPos, numIndex, cdt, auxiliaryLineListDownPanel, mouseHitPtsDownPanel, false);
            //}
        }
    }

    void DrawKDataGraph(Painter g, RectTransform rtCanvas, KData data, float missRelHeight, KDataDictContainer kddc, CollectDataType cdt)
    {
        KData prevData = data.GetPrevKData();
        float standX = data.index * canvasUpScale.x;
        float midX = standX + canvasUpScale.x * 0.5f;
        float up = data.UpValue * canvasUpScale.y;
        float down = data.DownValue * canvasUpScale.y;
        float start = data.StartValue * canvasUpScale.y;
        float end = data.EndValue * canvasUpScale.y;
        float rcY = data.StartValue > data.EndValue ? end : start;
        float rcH = Mathf.Abs(start - end);
        if (rcH < 1)
            rcH = 1;
        Color linePen = data.EndValue > data.StartValue ? Color.red : (data.EndValue < data.StartValue ? Color.cyan : Color.white);
        g.DrawLine(midX, up, midX, down, linePen);
        g.DrawFillRect(standX, rcY, canvasUpScale.x, rcH, linePen);
    }

    bool CheckPredictResults(KData data, int numIndex)
    {
        int number = (int)data.parent.startItem.GetNumberByIndex(numIndex);
        if (predict_results.Contains(number))
            return true;
        return false;
    }

    void DrawHotNumsPredictResult(Painter g, RectTransform rtCanvas, KData data, float missRelHeight, KDataDictContainer kddc, CollectDataType cdt)
    {
        int numIndex = PanelAnalyze.Instance.numIndex;

        if (GlobalSetting.G_SHOW_KCURVE_HOTNUMS_RESULT && GraphDataManager.CurrentCircle == 1)
        {
            float standX = data.index * canvasUpScale.x;
            Color col = Color.white;

            KData prevData = data.GetPrevKData();
            if (prevData != null)
            {
                predict_results.Clear();
                if (GlobalSetting.G_KCURVE_HOTNUMS_PREDICT_TYPE == HotNumPredictType.eNumber)
                {
                    int startID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum0);
                    int endID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.eNum9);
                    StatisticUnitMap sum = prevData.parent.startItem.statisticInfo.allStatisticInfo[numIndex];
                    for (int i = startID; i <= endID; ++i)
                    {
                        CollectDataType pcdt = GraphDataManager.S_CDT_LIST[i];
                        StatisticUnit su = sum.statisticUnitMap[pcdt];
                        int num = i - startID;

                        if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_3)
                        {
                            if (su.sample3Data.appearProbabilityDiffWithTheory > 0.5f)
                            {
                                if (predict_results.Contains(num) == false)
                                    predict_results.Add(num);
                            }
                        }
                        if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_5)
                        {
                            if (su.sample5Data.appearProbabilityDiffWithTheory > 0.5f)
                            {
                                if (predict_results.Contains(num) == false)
                                    predict_results.Add(num);
                            }
                        }
                        if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_10)
                        {
                            if (su.sample10Data.appearProbabilityDiffWithTheory > 0.5f)
                            {
                                if (predict_results.Contains(num) == false)
                                    predict_results.Add(num);
                            }
                        }
                    }
                }
                else if (GlobalSetting.G_KCURVE_HOTNUMS_PREDICT_TYPE == HotNumPredictType.ePath012)
                {
                    int startID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
                    int endID = GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath2);
                    StatisticUnitMap sum = prevData.parent.startItem.statisticInfo.allStatisticInfo[numIndex];
                    for (int i = startID; i <= endID; ++i)
                    {
                        CollectDataType pcdt = GraphDataManager.S_CDT_LIST[i];
                        StatisticUnit su = sum.statisticUnitMap[pcdt];
                        int path = i - startID;

                        if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_3)
                        {
                            if (su.sample3Data.appearProbabilityDiffWithTheory > 0.5f)
                            {
                                if (predict_results.Contains(path) == false)
                                    predict_results.Add(path);
                            }
                        }
                        if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_5)
                        {
                            if (su.sample5Data.appearProbabilityDiffWithTheory > 0.5f)
                            {
                                if (predict_results.Contains(path) == false)
                                    predict_results.Add(path);
                            }
                        }
                        if (GlobalSetting.G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_10)
                        {
                            if (su.sample10Data.appearProbabilityDiffWithTheory > 0.5f)
                            {
                                if (predict_results.Contains(path) == false)
                                    predict_results.Add(path);
                            }
                        }
                    }
                    int[] paths = predict_results.ToArray();
                    predict_results.Clear();
                    for (int i = 0; i < paths.Length; ++i)
                    {
                        if (paths[i] == 0)
                        {
                            predict_results.Add(0); predict_results.Add(3); predict_results.Add(6); predict_results.Add(9);
                        }
                        else if (paths[i] == 1)
                        {
                            predict_results.Add(1); predict_results.Add(4); predict_results.Add(7);
                        }
                        else
                        {
                            predict_results.Add(2); predict_results.Add(5); predict_results.Add(8);
                        }
                    }
                }
                if (predict_results.Count > 0)
                {
                    bool predict_success = CheckPredictResults(data, numIndex);
                    if (predict_success)
                        col = Color.red;
                    else
                        col = Color.cyan;
                }
            }

            float standY = g.CanvasToStand(0, false);
            g.DrawFillRect(standX + 1, standY, canvasUpScale.x - 1, 30, col);

            if (selectKDataIndex >= 0 && selectKDataIndex == data.index)
            {
                string info = "\n热号预测 : ";
                int last = predict_results.Count - 1;
                for (int i = 0; i <= last; ++i)
                {
                    info += predict_results[i];
                    if (i < last)
                        info += ", ";
                }
                PanelAnalyze.Instance.graphUp.AppendText(info);
            }
        }
    }

    void DrawAvgLineGraph(Painter g, AvgPointMap apm, RectTransform rtCanvas, CollectDataType cdt, Color pen)
    {
        AvgPoint ap = apm.apMap[cdt];
        float standX = (apm.index + 0.5f) * canvasUpScale.x;
        float standY = ap.avgKValue * canvasUpScale.y;
        if (findPrevPt == false)
        {
            findPrevPt = true;
            prevPt.x = standX;
            prevPt.y = standY;
            return;
        }
        float cx = g.StandToCanvas(standX, true);
        if (cx < 0 || cx > rtCanvas.rect.width)
        {
            prevPt.x = standX;
            prevPt.y = standY;
            return;
        }
        g.DrawLine(prevPt.x, prevPt.y, standX, standY, pen);
        prevPt.x = standX;
        prevPt.y = standY;
    }

    void DrawKRuler(Painter g, int numIndex, CollectDataType cdt, RectTransform rtCanvas, KDataDictContainer kddc, float missRelHeight)
    {
        float gridScaleW = canvasUpScale.x;
        float gridScaleH = canvasUpScale.y;

        int endIndex = kddc.dataLst.Count - 1;
        if (PanelAnalyze.Instance.endShowDataItemIndex != -1 && PanelAnalyze.Instance.endShowDataItemIndex < kddc.dataLst.Count)
            endIndex = PanelAnalyze.Instance.endShowDataItemIndex;

        //if (TradeDataManager.Instance.autoAnalyzeTool != null)
        //{
        //    int startID = endIndex - GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT;
        //    if (startID < 0)
        //        startID = 0;
        //    float x = g.StandToCanvas(startID * gridScaleW, true);
        //    g.DrawLineInCanvasSpace(x, 0, x, rtCanvas.rect.height, Color.white);
        //}

        float downRCH = gridScaleH * missRelHeight;

        KDataMap lastKDD = kddc.dataLst[endIndex];
        KData data = lastKDD.GetData(cdt, false);
        float standX = (data.index + 1) * gridScaleW;
        float standY = (data.KValue) * gridScaleH;
        standX = g.StandToCanvas(standX, true);
        standY = g.StandToCanvas(standY, false);
        if (standX > rtCanvas.rect.width)
            return;
        bool isUpOK = false;
        bool isDownOK = false;

        int i = 0;
        while (isUpOK == false || isDownOK == false)
        {
            float X = standX + i * gridScaleW;
            int stepID = i + 1;
            if (isUpOK == false)
            {
                float upY = standY + i * gridScaleH;
                g.DrawRectInCanvasSpace(X, upY, gridScaleW, gridScaleH, Color.red);
                if (X > rtCanvas.rect.width || upY < 0)
                    isUpOK = true;
            }
            if (isDownOK == false)
            {
                float downY = standY - (i + 1) * downRCH;
                g.DrawRectInCanvasSpace(X, downY, gridScaleW, downRCH, Color.cyan);
                if (X > rtCanvas.rect.width || downY > rtCanvas.rect.height)
                    isDownOK = true;
            }
            ++i;
        }
    }

    void DrawBollinLineGraph(Painter g, BollinPointMap bpm, RectTransform rtCanvas, CollectDataType cdt)
    {
        BollinPoint bp = bpm.bpMap[cdt];
        float standX = (bpm.index + 0.5f) * canvasUpScale.x;
        if (findPrevPt == false)
        {
            findPrevPt = true;
            return;
        }
        float cx = g.StandToCanvas(standX, true);
        if (cx < 0 || cx > rtCanvas.rect.width)
        {
            return;
        }
        BollinPointMap prevBPM = bpm.GetPrevBPM();
        BollinPoint prevBP = prevBPM.bpMap[cdt];
        float px  = g.StandToCanvas((prevBPM.index + 0.5f) * canvasUpScale.x, true);
        float pyU = g.StandToCanvas(prevBP.upValue * canvasUpScale.y, false);
        float pyM = g.StandToCanvas(prevBP.midValue * canvasUpScale.y, false);
        float pyD = g.StandToCanvas(prevBP.downValue * canvasUpScale.y, false);
        float cyU = g.StandToCanvas(bp.upValue * canvasUpScale.y, false);
        float cyM = g.StandToCanvas(bp.midValue * canvasUpScale.y, false);
        float cyD = g.StandToCanvas(bp.downValue * canvasUpScale.y, false);
        Color midPen = Color.white;
        if (bp.bolleanMidKeepDownCountContinue > 0)
            midPen = Color.green;
        else if (bp.bolleanMidKeepUpCountContinue > 0)
            midPen = Color.red;
        g.DrawLineInCanvasSpace(px, pyU, cx, cyU, Color.red, 2);
        g.DrawLineInCanvasSpace(px, pyM, cx, cyM, midPen, 2);
        g.DrawLineInCanvasSpace(px, pyD, cx, cyD, Color.cyan, 2);
    }

    void DrawMACDGraph(Painter g, MACDPointMap mpm, RectTransform rtCanvas, CollectDataType cdt, bool drawSelectedLine)
    {
        MACDPoint mp = mpm.macdpMap[cdt];
        bool isUp = mp.BAR > 0;
        MACDPointMap prevMPM = mp.parent.GetPrevMACDPM();
        if (prevMPM != null)
        {
            isUp = mp.BAR > prevMPM.GetData(cdt, false).BAR;
        }
        float standX = (mpm.index + 0.5f) * canvasDownScale.x;
        float halfW = canvasDownScale.x * 0.5f;
        float cx = g.StandToCanvas(standX, true);
        if (cx < 0 || cx > rtCanvas.rect.width)
        {
            return;
        }
        MACDLimitValue mlv = mpm.parent.macdLimitValueMap[cdt];
        float gridScaleH = rtCanvas.rect.height * 0.45f / Mathf.Max(Mathf.Abs(mlv.MaxValue), Mathf.Abs(mlv.MinValue));
        float standY = mp.BAR * gridScaleH;
        float cyDIF = g.StandToCanvas((mp.DIF * gridScaleH), false);
        float cyDEA = g.StandToCanvas((mp.DEA * gridScaleH), false);
        float cyBAR = g.StandToCanvas(standY, false);
        float rcY = g.StandToCanvas(standY > 0 ? 0 : standY, false);
        g.DrawFillRectInCanvasSpace(cx - halfW, rcY, canvasDownScale.x, Mathf.Abs(standY), isUp ? Color.red : Color.cyan);
        if (prevMPM != null)
        {
            MACDPoint prevMP = prevMPM.macdpMap[cdt];
            float px = cx - canvasDownScale.x;
            float pyDIF = g.StandToCanvas((prevMP.DIF * gridScaleH), false);
            float pyDEA = g.StandToCanvas((prevMP.DEA * gridScaleH), false);
            g.DrawLineInCanvasSpace(px, pyDIF, cx, cyDIF, Color.yellow);
            g.DrawLineInCanvasSpace(px, pyDEA, cx, cyDEA, Color.white);
        }

        if (drawSelectedLine)
        {
            float xL = cx - halfW;
            float xR = cx + halfW;
            g.DrawLineInCanvasSpace(xL, 0, xL, rtCanvas.rect.height, Color.yellow);
            g.DrawLineInCanvasSpace(xR, 0, xR, rtCanvas.rect.height, Color.yellow);
        }

        canvasDownScale.y = gridScaleH;
    }

}
