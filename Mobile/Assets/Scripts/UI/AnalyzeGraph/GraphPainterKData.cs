using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterKData : GraphPainterBase
{
    static readonly float[] C_GOLDEN_VALUE = new float[] { 0.382f, 0.618f, 1.382f, 1.618f, 2f, 2.382f, 2.618f, 4, 4.382f, 4.618f, 8, };


    public static List<string> S_AUX_LINE_OPERATIONS = new List<string>()
    {
        "浏览",
        "画水平线",
        "画垂直线",
        "画直线",
        "画通道线",
        "画黄金分割线",
        "画圆",
        "画箭头线",
        "画矩形",
    };
    public AuxLineType auxLineOperation = AuxLineType.eNone;

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
    //public int selectKDataIndex = -1;
    List<int> predict_results = new List<int>();

    float lastValue = 0;
    bool findPrevPt = false;
    Vector2 prevPt = Vector2.zero;

    float DownGraphYOffset;
    float DataMaxWidth = 0;

    Dictionary<int, Dictionary<CollectDataType, List<AuxiliaryLineBase>>> upAuxLines = new Dictionary<int, Dictionary<CollectDataType, List<AuxiliaryLineBase>>>();
    Dictionary<int, Dictionary<CollectDataType, List<AuxiliaryLineBase>>> downAuxLines = new Dictionary<int, Dictionary<CollectDataType, List<AuxiliaryLineBase>>>();

    Vector2 pointerDownPos = Vector2.zero;
    Painter pointerDownPainter = null;

    AuxiliaryLineBase upLineSelected = null;
    int upLineSelectedKeyID = -1;
    AuxiliaryLineBase downLineSelected;
    int downLineSelectedKeyID = -1;

    float rcSize
    {
        get
        {
            return GlobalSetting.G_AUX_LINE_KEY_POINT_HIT_SIZE;
        }
    }
    float rcHalfSize
    {
        get
        {
            return GlobalSetting.G_AUX_LINE_KEY_POINT_HIT_SIZE * 0.5f;
        }
    }
    float rcSizeSel
    {
        get
        {
            return GlobalSetting.G_AUX_LINE_KEY_POINT_HIT_SIZE * GlobalSetting.G_AUX_LINE_KEY_POINT_SEL_SIZE_SCALE;
        }
    }

    bool needRebuildAuxPoints = false;
    public bool NeedRebuildAuxPoints
    {
        get { return needRebuildAuxPoints; }
        set { needRebuildAuxPoints = value; }
    }

    public override void Start()
    {
        base.Start();

        canvasUpOffset.y = PanelAnalyze.Instance.splitPanel.panelLTSize.y * 0.5f;
        canvasDownOffset.y = PanelAnalyze.Instance.splitPanel.panelRBSize.y * 0.5f;

        for(int i = 0; i < 5; ++i)
        {
            Dictionary<CollectDataType, List<AuxiliaryLineBase>> upCdtAuxLines = new Dictionary<CollectDataType, List<AuxiliaryLineBase>>();
            Dictionary<CollectDataType, List<AuxiliaryLineBase>> downCdtAuxLines = new Dictionary<CollectDataType, List<AuxiliaryLineBase>>();

            for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                upCdtAuxLines.Add(cdt, new List<AuxiliaryLineBase>());
                downCdtAuxLines.Add(cdt, new List<AuxiliaryLineBase>());
            }

            upAuxLines.Add(i, upCdtAuxLines);
            downAuxLines.Add(i, downCdtAuxLines);
        }
    }

    public override void Update()
    {
        base.Update();

        if(needRebuildAuxPoints)
        {
            needRebuildAuxPoints = false;
            RebuildAuxlines();
        }
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
                PanelAnalyze.Instance.SelectKDataIndex = Mathf.FloorToInt(x / canvasUpScale.x);
            }
            else
            {
                PanelAnalyze.Instance.SelectKDataIndex = -1;
            }
        }
        else
        {
            float x = downPainter.CanvasToStand(pos.x, true);
            if (x >= 0 && x <= DataMaxWidth)
            {
                PanelAnalyze.Instance.SelectKDataIndex = Mathf.FloorToInt(x / canvasDownScale.x);
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
                if (PanelAnalyze.Instance.SelectKDataIndex >= 0)
                    startIndex = PanelAnalyze.Instance.SelectKDataIndex;
                if (startIndex >= kddc.dataLst.Count)
                    startIndex = kddc.dataLst.Count - 1;

                KDataMap kdDict = kddc.dataLst[startIndex];
                KData data = kdDict.GetData(cdt, false);
                float y = data.KValue * canvasUpScale.y;
                float canvasY = upPainter.StandToCanvas(y, false);
                canvasUpOffset.y += PanelAnalyze.Instance.graphUp.rectTransform.rect.height * 0.5f - canvasY;
                upPainter.BeforeDraw(canvasUpOffset);
            }
        }

        canvasDownOffset.y = PanelAnalyze.Instance.graphDown.rectTransform.rect.height * 0.5f;
        downPainter.BeforeDraw(canvasDownOffset);
        PanelAnalyze.Instance.NotifyUIRepaint();
    }

    public override void OnGraphDragging(Vector2 offset, Painter painter)
    {
        if(painter == upPainter)
        {
            if (upLineSelected != null)
            {
                Vector2 pos = upLineSelected.keyPoints[upLineSelectedKeyID] + offset;
                upLineSelected.keyPoints[upLineSelectedKeyID] = pos;
                pos.x = pos.x / canvasUpScale.x;
                pos.y = pos.y / canvasUpScale.y;
                upLineSelected.valuePoints[upLineSelectedKeyID] = pos;
                PanelAnalyze.Instance.NotifyUIRepaint();
                return;
            }
        }
        else if(painter == downPainter)
        {
            if (downLineSelected != null)
            {
                Vector2 pos = downLineSelected.keyPoints[downLineSelectedKeyID] + offset;
                downLineSelected.keyPoints[downLineSelectedKeyID] = pos;
                pos.x = pos.x / canvasDownScale.x;
                pos.y = pos.y / canvasDownScale.y;
                downLineSelected.valuePoints[downLineSelectedKeyID] = pos;
                PanelAnalyze.Instance.NotifyUIRepaint();
                return;
            }
        }
        base.OnGraphDragging(offset, painter);
    }

    public override void OnPointerDown(Vector2 pos, Painter g)
    {
        base.OnPointerDown(pos, g);
        pointerDownPos = pos;
        pointerDownPainter = g;

        SelectAuxLine(pos, g);
    }
    public override void OnPointerUp(Vector2 pos, Painter g)
    {
        bool notMove = pos.Equals(pointerDownPos) && (g == pointerDownPainter);
        if(notMove)
        {
            CreateAuxLine(pos, g);
        }
        else
        {
            SelectAuxLine(pos, g);
        }
        PanelAnalyze.Instance.NotifyUIRepaint();
    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {
        int selectKDataIndex = PanelAnalyze.Instance.SelectKDataIndex;
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

                if (selectKDataIndex >= 0 && selectKDataIndex < kddc.dataLst.Count)
                {
                    KDataMap kdDict = kddc.dataLst[selectKDataIndex];
                    KData data = kdDict.GetData(cdt, false);

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

                // 画K线图
                for (int i = startIndex; i < endIndex; ++i)
                {
                    KDataMap kdDict = kddc.dataLst[i];
                    KData data = kdDict.GetData(cdt, false);
                    if (data == null)
                        continue;
                    DrawKDataGraph(g, rtCanvas, data, missRelHeight, kddc, cdt);
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

                if(enableAuxLine)
                {
                    List<AuxiliaryLineBase> lines = upAuxLines[numIndex][cdt];
                    for(int i = 0; i < lines.Count; ++i)
                    {
                        DrawAuxLine(lines[i], upPainter, rtCanvas);
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
        int selectKDataIndex = PanelAnalyze.Instance.SelectKDataIndex;
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

        if (enableAuxLine)
        {
            List<AuxiliaryLineBase> lines = downAuxLines[numIndex][cdt];
            for (int i = 0; i < lines.Count; ++i)
            {
                DrawAuxLine(lines[i], downPainter, rtCanvas);
            }
        }
    }
   

    public void DelSelAuxLine()
    {
        bool removeSuccess = false;
        var etor1 = upAuxLines.GetEnumerator();
        while(etor1.MoveNext())
        {
            var etor2 = etor1.Current.Value.GetEnumerator();
            while(etor2.MoveNext())
            {
                if(etor2.Current.Value.Remove(upLineSelected))
                {
                    removeSuccess = true;
                    break;
                }
            }
            if (removeSuccess)
                break;
        }

        removeSuccess = false;
        etor1 = downAuxLines.GetEnumerator();
        while (etor1.MoveNext())
        {
            var etor2 = etor1.Current.Value.GetEnumerator();
            while (etor2.MoveNext())
            {
                if (etor2.Current.Value.Remove(downLineSelected))
                {
                    removeSuccess = true;
                    break;
                }
            }
            if (removeSuccess)
                break;
        }

        upLineSelected = null;
        upLineSelectedKeyID = -1;
        downLineSelected = null;
        downLineSelectedKeyID = -1;
        //for(int i = upAuxLines.Count; i >= 0; --i)
        //{
        //    if (upAuxLines[i].selected)
        //        upAuxLines.RemoveAt(i);
        //}
        //for (int i = downAuxLines.Count; i >= 0; --i)
        //{
        //    if (downAuxLines[i].selected)
        //        downAuxLines.RemoveAt(i);
        //}
    }

    public void DelAllAuxLine()
    {
        var etor1 = upAuxLines.GetEnumerator();
        while (etor1.MoveNext())
        {
            var etor2 = etor1.Current.Value.GetEnumerator();
            while (etor2.MoveNext())
            {
                etor2.Current.Value.Clear();
            }
        }

        etor1 = downAuxLines.GetEnumerator();
        while (etor1.MoveNext())
        {
            var etor2 = etor1.Current.Value.GetEnumerator();
            while (etor2.MoveNext())
            {
                etor2.Current.Value.Clear();
            }
        }

        //upAuxLines.Clear();
        //downAuxLines.Clear();
    }

    void RebuildAuxlines()
    {
        var etor1 = upAuxLines.GetEnumerator();
        while (etor1.MoveNext())
        {
            var etor2 = etor1.Current.Value.GetEnumerator();
            while (etor2.MoveNext())
            {
                for (int i = 0; i < etor2.Current.Value.Count; ++i)
                {
                    AuxiliaryLineBase line = etor2.Current.Value[i];
                    for(int j = 0; j < line.keyPoints.Count; ++j)
                    {
                        Vector2 kp = line.valuePoints[j];
                        kp.x *= canvasUpScale.x;
                        kp.y *= canvasUpScale.y;
                        line.keyPoints[j] = kp;
                    }
                }
            }
        }

        etor1 = downAuxLines.GetEnumerator();
        while (etor1.MoveNext())
        {
            var etor2 = etor1.Current.Value.GetEnumerator();
            while (etor2.MoveNext())
            {
                for (int i = 0; i < etor2.Current.Value.Count; ++i)
                {
                    AuxiliaryLineBase line = etor2.Current.Value[i];
                    for (int j = 0; j < line.keyPoints.Count; ++j)
                    {
                        Vector2 kp = line.valuePoints[j];
                        kp.x *= canvasDownScale.x;
                        kp.y *= canvasDownScale.y;
                        line.keyPoints[j] = kp;
                    }
                }
            }
        }
    }

    void AddAuxlineKeyPoint(AuxiliaryLineBase line, Vector2 pos, Painter g, Vector2 canvasScale)
    {
        Vector2 canvasPos = pos;
        canvasPos.x = g.CanvasToStand(canvasPos.x, true);
        canvasPos.y = g.CanvasToStand(canvasPos.y, false);
        line.keyPoints.Add(canvasPos);
        Vector2 valuePos = canvasPos;
        valuePos.x /= canvasScale.x;
        valuePos.y /= canvasScale.y;
        line.valuePoints.Add(valuePos);
    }

    void CreateAuxLine(Vector2 pos, Painter g)
    {
        int numIndex = PanelAnalyze.Instance.numIndex;
        CollectDataType cdt = PanelAnalyze.Instance.cdt;
        GraphBase graph = null;
        List<AuxiliaryLineBase> targetLineLst = null;
        Vector2 gridScale = Vector2.one;
        if (g == upPainter)
        {
            targetLineLst = upAuxLines[numIndex][cdt];
            graph = PanelAnalyze.Instance.graphUp;
            gridScale = canvasUpScale;
        }
        else if (g == downPainter)
        {
            targetLineLst = downAuxLines[numIndex][cdt];
            graph = PanelAnalyze.Instance.graphDown;
            gridScale = canvasDownScale;
        }
        switch (auxLineOperation)
        {
            case AuxLineType.eNone:
                {

                }
                break;
            case AuxLineType.eHorzLine:
                {
                    AuxiliaryLineHorz line = new AuxiliaryLineHorz();
                    line.cdt = cdt;
                    line.numIndex = numIndex;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    targetLineLst.Add(line);
                }
                break;
            case AuxLineType.eVertLine:
                {
                    AuxiliaryLineVert line = new AuxiliaryLineVert();
                    line.cdt = cdt;
                    line.numIndex = numIndex;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    targetLineLst.Add(line);
                }
                break;
            case AuxLineType.eSingleLine:
                {
                    AuxiliaryLineSupportPressure line = new AuxiliaryLineSupportPressure();
                    line.cdt = cdt;
                    line.numIndex = numIndex;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    pos.x += 300.0f;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    targetLineLst.Add(line);
                }
                break;
            case AuxLineType.eChannelLine:
                {
                    AuxiliaryLineChannel line = new AuxiliaryLineChannel();
                    line.cdt = cdt;
                    line.numIndex = numIndex;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    pos.x += 300.0f;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    pos.y += 300.0f;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    targetLineLst.Add(line);
                }
                break;
            case AuxLineType.eGoldSegmentedLine:
                {
                    AuxiliaryLineGoldenSection line = new AuxiliaryLineGoldenSection();
                    line.cdt = cdt;
                    line.numIndex = numIndex;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    pos.y += 300.0f;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    targetLineLst.Add(line);
                }
                break;
            case AuxLineType.eCircleLine:
                {
                    AuxiliaryLineCircle line = new AuxiliaryLineCircle();
                    line.cdt = cdt;
                    line.numIndex = numIndex;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    pos.y += 300.0f;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    targetLineLst.Add(line);
                }
                break;
            case AuxLineType.eArrowLine:
                {
                    AuxiliaryLineArrow line = new AuxiliaryLineArrow();
                    line.cdt = cdt;
                    line.numIndex = numIndex;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    pos.x += 300.0f;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    targetLineLst.Add(line);
                }
                break;
            case AuxLineType.eRectLine:
                {
                    AuxiliaryLineRect line = new AuxiliaryLineRect();
                    line.cdt = cdt;
                    line.numIndex = numIndex;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    pos.x += 300.0f;
                    pos.x += 300.0f;
                    AddAuxlineKeyPoint(line, pos, g, gridScale);
                    targetLineLst.Add(line);
                }
                break;
        }
    }

    void SelectAuxLine(Vector2 pos, Painter g)
    {
        Vector2 standpos = pos;
        standpos.x = g.CanvasToStand(standpos.x, true);
        standpos.y = g.CanvasToStand(standpos.y, false);

        CollectDataType cdt = PanelAnalyze.Instance.cdt;
        int numIndex = PanelAnalyze.Instance.numIndex;
        bool hasSelected = false;

        if (g == upPainter)
        {
            upLineSelected = null;
            upLineSelectedKeyID = -1;
            var etor1 = upAuxLines.GetEnumerator();
            while (etor1.MoveNext())
            {
                var etor2 = etor1.Current.Value.GetEnumerator();
                while (etor2.MoveNext())
                {
                    for (int i = 0; i < etor2.Current.Value.Count; ++i)
                    {
                        AuxiliaryLineBase line = etor2.Current.Value[i];

                        if (hasSelected == false && numIndex == etor1.Current.Key && cdt == etor2.Current.Key)
                        {
                            if (line.HitTest(etor2.Current.Key, etor1.Current.Key, standpos, rcSize, rcSizeSel, ref upLineSelectedKeyID))
                            {
                                line.selected = true;
                                line.selectedKeyID = upLineSelectedKeyID;
                                upLineSelected = line;
                                hasSelected = true;
                            }
                            else
                            {
                                line.selected = false;
                                line.selectedKeyID = -1;
                            }
                        }
                        else
                        {
                            line.selected = false;
                            line.selectedKeyID = -1;
                        }
                    }
                }
            }
        }
        else if (g == downPainter)
        {
            downLineSelected = null;
            downLineSelectedKeyID = -1;
            var etor1 = downAuxLines.GetEnumerator();
            while (etor1.MoveNext())
            {
                var etor2 = etor1.Current.Value.GetEnumerator();
                while (etor2.MoveNext())
                {
                    for (int i = 0; i < etor2.Current.Value.Count; ++i)
                    {
                        AuxiliaryLineBase line = etor2.Current.Value[i];

                        if (hasSelected == false && numIndex == etor1.Current.Key && cdt == etor2.Current.Key)
                        {
                            if (line.HitTest(etor2.Current.Key, etor1.Current.Key, standpos, rcSize, rcSizeSel, ref downLineSelectedKeyID))
                            {
                                line.selected = true;
                                line.selectedKeyID = downLineSelectedKeyID;
                                downLineSelected = line;
                                hasSelected = true;
                            }
                            else
                            {
                                line.selected = false;
                                line.selectedKeyID = -1;
                            }
                        }
                        else
                        {
                            line.selected = false;
                            line.selectedKeyID = -1;
                        }
                    }
                }
            }
        }
        PanelAnalyze.Instance.NotifyUIRepaint();
    }
    

    void DrawAuxLine(AuxiliaryLineBase lineBase, Painter p, RectTransform rtCanvas)
    {
        Color selGridCol = new Color(1, 1, 0, 0.5f);

        float w = rtCanvas.rect.width;
        float h = rtCanvas.rect.height;
        float lineWidth = lineBase.selected ? 6 : 2;
        float selWidth = 10;
        int segCount = 30;

        switch(lineBase.lineType)
        {
            case AuxLineType.eNone:
                {

                }
                break;
            case AuxLineType.eHorzLine:
                {
                    AuxiliaryLineHorz line = lineBase as AuxiliaryLineHorz;
                    Vector2 pos = line.keyPoints[0];
                    float x = p.StandToCanvas(pos.x, true);
                    float y = p.StandToCanvas(pos.y, false);
                    if(line.selected)
                    {
                        //p.DrawFillRectInCanvasSpace(x - rcSize, y - rcSize, rcSize2, rcSize2, selGridCol);
                        p.DrawCircleInCanvasSpace(new Vector2(x, y), rcSizeSel, selGridCol, selWidth, segCount);
                    }
                    p.DrawRectInCanvasSpace(x - rcHalfSize, y - rcHalfSize, rcSize, rcSize, line.color, lineWidth);

                    if (y >= 0 && y <= h)
                        p.DrawLineInCanvasSpace(0, y, w, y, line.color, lineWidth);
                }
                break;
            case AuxLineType.eVertLine:
                {
                    AuxiliaryLineVert line = lineBase as AuxiliaryLineVert;
                    Vector2 pos = line.keyPoints[0];
                    float x = p.StandToCanvas(pos.x, true);
                    float y = p.StandToCanvas(pos.y, false);
                    if (line.selected)
                    {
                        //p.DrawFillRectInCanvasSpace(x - rcSize, y - rcSize, rcSize2, rcSize2, selGridCol);
                        p.DrawCircleInCanvasSpace(new Vector2(x, y), rcSizeSel, selGridCol, selWidth, segCount);
                    }
                    p.DrawRectInCanvasSpace(x - rcHalfSize, y - rcHalfSize, rcSize, rcSize, line.color, lineWidth);

                    if (x >= 0 && x <= w)
                        p.DrawLineInCanvasSpace(x, 0, x, h, line.color, lineWidth);
                }
                break;
            case AuxLineType.eSingleLine:
                {
                    AuxiliaryLineSupportPressure line = lineBase as AuxiliaryLineSupportPressure;
                    Vector2 pos0 = line.keyPoints[0], pos1 = line.keyPoints[1];
                    float x0 = p.StandToCanvas(pos0.x, true);
                    float y0 = p.StandToCanvas(pos0.y, false);
                    float x1 = p.StandToCanvas(pos1.x, true);
                    float y1 = p.StandToCanvas(pos1.y, false);
                    if (line.selected)
                    {
                        if (line.selectedKeyID == 0)
                            p.DrawCircleInCanvasSpace(new Vector2(x0, y0), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x0 - rcSize, y0 - rcSize, rcSize2, rcSize2, selGridCol);
                        if (line.selectedKeyID == 1)
                            p.DrawCircleInCanvasSpace(new Vector2(x1, y1), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x1 - rcSize, y1 - rcSize, rcSize2, rcSize2, selGridCol);
                    }
                    p.DrawRectInCanvasSpace(x0 - rcHalfSize, y0 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawRectInCanvasSpace(x1 - rcHalfSize, y1 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    
                    if (x0 == x1)
                    {
                        p.DrawLineInCanvasSpace(x0, 0, x1, h, line.color, lineWidth);
                    }
                    else
                    {
                        float k = (y1 - y0) / (x1 - x0);
                        float fyl = y0 - x0 * k;
                        float fyr = y0 + (w - x0) * k;
                        p.DrawLineInCanvasSpace(0, fyl, w, fyr, line.color, lineWidth);
                    }
                }
                break;
            case AuxLineType.eChannelLine:
                {
                    AuxiliaryLineChannel line = lineBase as AuxiliaryLineChannel;
                    Vector2 pos0 = line.keyPoints[0], pos1 = line.keyPoints[1], pos2 = line.keyPoints[2];
                    float x0 = p.StandToCanvas(pos0.x, true);
                    float y0 = p.StandToCanvas(pos0.y, false);
                    float x1 = p.StandToCanvas(pos1.x, true);
                    float y1 = p.StandToCanvas(pos1.y, false);
                    float x2 = p.StandToCanvas(pos2.x, true);
                    float y2 = p.StandToCanvas(pos2.y, false);
                    if (line.selected)
                    {
                        if (line.selectedKeyID == 0)
                            p.DrawCircleInCanvasSpace(new Vector2(x0, y0), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x0 - rcSize, y0 - rcSize, rcSize2, rcSize2, selGridCol);
                        if (line.selectedKeyID == 1)
                            p.DrawCircleInCanvasSpace(new Vector2(x1, y1), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x1 - rcSize, y1 - rcSize, rcSize2, rcSize2, selGridCol);
                        if (line.selectedKeyID == 2)
                            p.DrawCircleInCanvasSpace(new Vector2(x2, y2), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x2 - rcSize, y2 - rcSize, rcSize2, rcSize2, selGridCol);
                    }
                    p.DrawRectInCanvasSpace(x0 - rcHalfSize, y0 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawRectInCanvasSpace(x1 - rcHalfSize, y1 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawRectInCanvasSpace(x2 - rcHalfSize, y2 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);

                    p.DrawLineInCanvasSpace(x0, y0, x2, y2, line.color, 1);
                    if (x0 == x1)
                    {
                        p.DrawLineInCanvasSpace(x0, 0, x1, h, line.color, lineWidth);
                        p.DrawLineInCanvasSpace(x2, 0, x2, h, line.color, lineWidth);
                    }
                    else
                    {
                        float k = (y1 - y0) / (x1 - x0);
                        float fyl = y0 - x0 * k;
                        float fyr = y0 + (w - x0) * k;
                        float oyl = y2 - x2 * k;
                        float oyr = y2 + (w - x2) * k;
                        p.DrawLineInCanvasSpace(0, fyl, w, fyr, line.color, lineWidth);
                        p.DrawLineInCanvasSpace(0, oyl, w, oyr, line.color, lineWidth);
                    }
                }
                break;
            case AuxLineType.eGoldSegmentedLine:
                {
                    AuxiliaryLineGoldenSection line = lineBase as AuxiliaryLineGoldenSection;
                    Vector2 pos0 = line.keyPoints[0], pos1 = line.keyPoints[1];
                    float x0 = p.StandToCanvas(pos0.x, true);
                    float y0 = p.StandToCanvas(pos0.y, false);
                    float x1 = p.StandToCanvas(pos1.x, true);
                    float y1 = p.StandToCanvas(pos1.y, false);
                    if (line.selected)
                    {
                        if (line.selectedKeyID == 0)
                            p.DrawCircleInCanvasSpace(new Vector2(x0, y0), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x0 - rcSize, y0 - rcSize, rcSize2, rcSize2, selGridCol);
                        if (line.selectedKeyID == 1)
                            p.DrawCircleInCanvasSpace(new Vector2(x1, y1), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x1 - rcSize, y1 - rcSize, rcSize2, rcSize2, selGridCol);
                    }
                    p.DrawRectInCanvasSpace(x0 - rcHalfSize, y0 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawRectInCanvasSpace(x1 - rcHalfSize, y1 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);

                    for (int i = 0; i < C_GOLDEN_VALUE.Length; ++i)
                    {
                        float v = C_GOLDEN_VALUE[i];
                        float yv = (y1 - y0) * v + y0;
                        p.DrawLineInCanvasSpace(0, yv, w, yv, line.color, 1);
                    }
                    p.DrawLineInCanvasSpace(x0, y0, x1, y1, line.color, 1);
                    p.DrawLineInCanvasSpace(0, y0, w, y0, line.color, 1);
                    p.DrawLineInCanvasSpace(0, y1, w, y1, line.color, 1);
                }
                break;
            case AuxLineType.eCircleLine:
                {
                    AuxiliaryLineCircle line = lineBase as AuxiliaryLineCircle;
                    Vector2 pos0 = line.keyPoints[0], pos1 = line.keyPoints[1];
                    float x0 = p.StandToCanvas(pos0.x, true);
                    float y0 = p.StandToCanvas(pos0.y, false);
                    float x1 = p.StandToCanvas(pos1.x, true);
                    float y1 = p.StandToCanvas(pos1.y, false);
                    if (line.selected)
                    {
                        if (line.selectedKeyID == 0)
                            p.DrawCircleInCanvasSpace(new Vector2(x0, y0), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x0 - rcSize, y0 - rcSize, rcSize2, rcSize2, selGridCol);
                        if (line.selectedKeyID == 1)
                            p.DrawCircleInCanvasSpace(new Vector2(x1, y1), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x1 - rcSize, y1 - rcSize, rcSize2, rcSize2, selGridCol);
                    }
                    p.DrawRectInCanvasSpace(x0 - rcHalfSize, y0 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawRectInCanvasSpace(x1 - rcHalfSize, y1 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawLineInCanvasSpace(x0, y0, x1, y1, line.color, lineWidth);

                    float dx = x0 - x1, dy = y0 - y1;
                    float radius = Mathf.Sqrt(dx * dx + dy * dy);
                    p.DrawCircleInCanvasSpace(new Vector2(x0, y0), radius, line.color, lineWidth, 30);
                }
                break;
            case AuxLineType.eArrowLine:
                {
                    AuxiliaryLineArrow line = lineBase as AuxiliaryLineArrow;
                    Vector2 pos0 = line.keyPoints[0], pos1 = line.keyPoints[1];
                    float x0 = p.StandToCanvas(pos0.x, true);
                    float y0 = p.StandToCanvas(pos0.y, false);
                    float x1 = p.StandToCanvas(pos1.x, true);
                    float y1 = p.StandToCanvas(pos1.y, false);
                    if (line.selected)
                    {
                        if (line.selectedKeyID == 0)
                            p.DrawCircleInCanvasSpace(new Vector2(x0, y0), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x0 - rcSize, y0 - rcSize, rcSize2, rcSize2, selGridCol);
                        if (line.selectedKeyID == 1)
                            p.DrawCircleInCanvasSpace(new Vector2(x1, y1), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x1 - rcSize, y1 - rcSize, rcSize2, rcSize2, selGridCol);
                    }
                    p.DrawRectInCanvasSpace(x0 - rcHalfSize, y0 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawRectInCanvasSpace(x1 - rcHalfSize, y1 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawLineInCanvasSpace(x0, y0, x1, y1, line.color, lineWidth);
                }
                break;
            case AuxLineType.eRectLine:
                {
                    AuxiliaryLineRect line = lineBase as AuxiliaryLineRect;
                    Vector2 pos0 = line.keyPoints[0], pos1 = line.keyPoints[1];
                    float x0 = p.StandToCanvas(pos0.x, true);
                    float y0 = p.StandToCanvas(pos0.y, false);
                    float x1 = p.StandToCanvas(pos1.x, true);
                    float y1 = p.StandToCanvas(pos1.y, false);
                    if (line.selected)
                    {
                        if (line.selectedKeyID == 0)
                            p.DrawCircleInCanvasSpace(new Vector2(x0, y0), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x0 - rcSize, y0 - rcSize, rcSize2, rcSize2, selGridCol);
                        if (line.selectedKeyID == 1)
                            p.DrawCircleInCanvasSpace(new Vector2(x1, y1), rcSizeSel, selGridCol, selWidth, segCount);
                        //p.DrawFillRectInCanvasSpace(x1 - rcSize, y1 - rcSize, rcSize2, rcSize2, selGridCol);
                    }
                    p.DrawRectInCanvasSpace(x0 - rcHalfSize, y0 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawRectInCanvasSpace(x1 - rcHalfSize, y1 - rcHalfSize, rcSize, rcSize, line.color, lineWidth);
                    p.DrawLineInCanvasSpace(x0, y0, x1, y1, line.color, lineWidth);

                    float sx = Mathf.Min(x0, x1);
                    float sy = Mathf.Min(y0, y1);
                    float bx = Mathf.Max(x0, x1);
                    float by = Mathf.Max(y0, y1);
                    p.DrawRectInCanvasSpace(sx, sy, bx - sx, by - sy, line.color, lineWidth);
                }
                break;
        }
    }

    bool CheckAvgShow(AvgDataContainer adc)
    {
        switch (adc.cycle)
        {
            case 5: return enableAvg5;
            case 10: return enableAvg10;
            case 20: return enableAvg20;
            case 30: return enableAvg30;
            case 50: return enableAvg50;
            case 100: return enableAvg100;
        }
        return false;
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
        int selectKDataIndex = PanelAnalyze.Instance.SelectKDataIndex;

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

        if (canvasDownScale.y != gridScaleH)
            needRebuildAuxPoints = true;
        canvasDownScale.y = gridScaleH;
    }


}
