using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterKData : GraphPainterBase
{
    bool enableAvgLines = true;
    int startItemIndex;
    bool autoAllign = false;
    int selectKDataIndex = -1;
    List<int> predict_results = new List<int>();

    float lastValue = 0;
    bool findPrevPt = false;
    Vector2 prevPt = Vector2.zero;


    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        //upPainter.DrawFillRect(5, 5, 100, 200, Color.blue);
        //upPainter.DrawFillRectInCanvasSpace(5, 200, 100, 30, Color.white);
        //downPainter.DrawFillRect(50, 5, 100, 200, Color.green);
        //upPainter.DrawLine(5, 5, 100, 200, Color.white);
    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {
        float MaxLen = 99999;
        g.DrawLine(-MaxLen, 0, MaxLen, 0, Color.white);
        g.DrawLine(0, -MaxLen, 0, MaxLen, Color.white);

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
                    startIndex = (int)(canvasUpOffset.x / canvasUpScale.x) - 1;
                    if (startIndex < 0)
                        startIndex = 0;
                }
                int endIndex = (int)((canvasUpOffset.x + rtCanvas.rect.width) / canvasUpScale.x) + 1;
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

                    DrawHotNumsPredictResult(g, rtCanvas, data, missRelHeight, kddc, cdt);
                }

                // 画均线图
                if (enableAvgLines)
                {
                    foreach (AvgDataContainer adc in kddc.avgDataContMap.Values)
                    {
                        if (adc.avgLineSetting.enable)
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
            }
        }
    }

    public override void DrawDownPanel(Painter g, RectTransform rtCanvas)
    {

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

}
