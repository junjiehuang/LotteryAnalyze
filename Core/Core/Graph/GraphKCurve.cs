using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    // K线图
    public class GraphKCurve : GraphBase
    {
        const float rcHalfSize = 4;
        const float rcSize = 8;
        public static string[] S_AUX_LINE_OPERATIONS = new string[]
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

        public bool enableAuxiliaryLine = true;
        public bool enableAvgLines = true;
        public bool enableBollinBand = true;
        public bool enableMACD = true;
        public bool enableKRuler = true;

        public AuxLineType auxOperationIndex = AuxLineType.eNone;


        public bool autoAllign = false;

        int selectKDataIndex = -1;
        int preViewDataIndex = -1;
        float selDataPtX = -1;

        Font redFont, greenFont;
        Font selDataFont;
        Font auxFont;
        Font rulerFont;
        PointF prevPt = new PointF();
        bool findPrevPt = false;

        float lastValue = 0;


        Pen bollinLinePenUp = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);
        Pen bollinLinePenMid = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 2);
        Pen bollinLinePenMidR = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 2);
        Pen bollinLinePenMidC = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Cyan, 2);
        Pen bollinLinePenMidG = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Green, 2);
        Pen bollinLinePenDown = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);

        Pen grayDotLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);
        Pen redLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 1);
        Pen redLinePenB = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 2);
        Pen cyanLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Cyan, 1);
        Pen cyanLinePenB = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Cyan, 2);
        Pen whiteLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);
        Pen whiteLinePenB = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 2);
        Pen yellowLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Yellow, 1);
        Pen yellowLinePenB = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Yellow, 2);
        Pen greenLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.LightGreen, 1);
        Pen greenLinePenB = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.LightGreen, 2);
        Pen orangeLinePenB = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Orange, 2);
        Pen blueLinePenB = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.BlueViolet, 2);

        Pen redDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Red, 1);
        Pen cyanDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Cyan, 1);

        SolidBrush previewAnalyzeToolBrush = new SolidBrush(Color.FromArgb(200, 80, 80, 80));
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush cyanBrush = new SolidBrush(Color.Cyan);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush greenBrush = new SolidBrush(Color.Green);
        SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
        SolidBrush tmpBrush;

        Dictionary<Pen, List<PointF>> segPools = new Dictionary<Pen, List<PointF>>();
        Dictionary<Pen, List<PointF>> linePools = new Dictionary<Pen, List<PointF>>();
        Dictionary<SolidBrush, List<RectangleF>> rcPools = new Dictionary<SolidBrush, List<RectangleF>>();
        List<AuxiliaryLineBase> auxiliaryLineListUpPanel = new List<AuxiliaryLineBase>();
        List<AuxiliaryLineBase> auxiliaryLineListDownPanel = new List<AuxiliaryLineBase>();
        public List<Point> mouseHitPtsUpPanel = new List<Point>();
        public List<Point> mouseHitPtsDownPanel = new List<Point>();
        public AuxiliaryLineBase selAuxLineUpPanel = null;
        public AuxiliaryLineBase selAuxLineDownPanel = null;
        public int selAuxLinePointIndexUpPanel = -1;
        public int selAuxLinePointIndexDownPanel = -1;

        //float downGraphYOffset = 0;
        public float DownGraphYOffset
        {
            get { return downCanvasOffset.Y; }
            set { downCanvasOffset.Y = value; }
        }

        int startItemIndex = -1;
        public int StartItemIndex
        {
            get { return startItemIndex; }
        }

        string graphInfo = "";

        public bool needAutoAddAuxLine = false;

        public GraphKCurve()
        {
            selDataFont = new Font(FontFamily.GenericSerif, 12);
            auxFont = new Font(FontFamily.GenericMonospace, 10);
            rulerFont = new Font(FontFamily.GenericMonospace, 9);
        }

        public override void OnGridScaleChanged()
        {
            for (int i = 0; i < auxiliaryLineListUpPanel.Count; ++i)
            {
                AuxiliaryLineBase line = auxiliaryLineListUpPanel[i];
                for (int j = 0; j < line.keyPoints.Count; ++j)
                {
                    line.keyPoints[j] = ValueToStand(line.valuePoints[j], true);
                }
            }

            for (int i = 0; i < auxiliaryLineListDownPanel.Count; ++i)
            {
                AuxiliaryLineBase line = auxiliaryLineListDownPanel[i];
                for (int j = 0; j < line.keyPoints.Count; ++j)
                {
                    line.keyPoints[j] = ValueToStand(line.valuePoints[j], false);
                }
            }
        }

        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            if (GraphDataManager.Instance.HasData(GraphType.eKCurveGraph) == false)
                return false;
            int curIndex = (int)((mousePos.X + canvasOffset.X) / gridScaleUp.X);
            if (curIndex == preViewDataIndex)
                return false;
            return true;
        }
        public override void MoveGraph(float dx, float dy)
        {
            canvasOffset.X += dx;
            canvasOffset.Y += dy;

            if (canvasOffset.X < 0)
                canvasOffset.X = 0;

            downCanvasOffset.X = canvasOffset.X;
        }
        public override void ResetGraphPosition()
        {
            canvasOffset.X = 0;
            canvasOffset.Y = 0;
            downCanvasOffset.X = downCanvasOffset.Y = 0;
        }

        public override void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            graphInfo = "";
            BeforeDraw();

            int beforeID = preViewDataIndex;
            if (enableAuxiliaryLine)
            {
                DrawAutoAuxTools(g, winW, winH, numIndex, cdt, true);
            }

            preViewDataIndex = -1;
            lastValue = 0;
            if (canvasOffset.Y == 0 && canvasOffset.X == 0)
                canvasOffset.Y = winH * 0.5f;
            KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
            int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
            float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];
            if (kddc != null)
            {
                if (kddc.dataLst.Count > 0)
                {
                    int startIndex = (int)(canvasOffset.X / gridScaleUp.X) - 1;
                    if (startIndex < 0)
                        startIndex = 0;
                    int endIndex = (int)((canvasOffset.X + winW) / gridScaleUp.X) + 1;
                    if (endIndex > kddc.dataLst.Count)
                        endIndex = kddc.dataLst.Count;
                    if (parent.endShowDataItemIndex != -1)
                    {
                        if (endIndex > parent.endShowDataItemIndex + 1)
                            endIndex = parent.endShowDataItemIndex + 1;
                    }

                    startItemIndex = startIndex;

                    // 自动对齐
                    if (autoAllign)
                    {
                        float endY = kddc.dataLst[endIndex - 1].dataDict[cdt].KValue * gridScaleUp.Y;
                        if (selectKDataIndex >= 0 && selectKDataIndex < kddc.dataLst.Count)
                            endY = kddc.dataLst[selectKDataIndex].dataDict[cdt].KValue * gridScaleUp.Y;
                        float relEY = StandToCanvas(endY, false, true);
                        bool isEYOut = relEY < 0 || relEY > winH;
                        if (isEYOut)
                            canvasOffset.Y = endY + winH * 0.5f;
                        autoAllign = false;
                    }

                    // 画K线图
                    for (int i = startIndex; i < endIndex; ++i)
                    {
                        KDataMap kdDict = kddc.dataLst[i];
                        KData data = kdDict.GetData(cdt, false);
                        if (data == null)
                            continue;
                        DrawKDataGraph(g, data, winW, winH, missRelHeight, mouseRelPos, kddc, cdt);
                    }

                    if (enableKRuler)
                    {
                        DrawKRuler(g, numIndex, cdt, winW, winH, mouseRelPos, kddc, missRelHeight);
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

                                startIndex = (int)(canvasOffset.X / gridScaleUp.X) - 1;
                                if (startIndex < 0)
                                    startIndex = 1;
                                endIndex = (int)((canvasOffset.X + winW) / gridScaleUp.X) + 1;
                                if (endIndex > adc.avgPointMapLst.Count)
                                    endIndex = adc.avgPointMapLst.Count;
                                if (parent.endShowDataItemIndex != -1)
                                {
                                    if (endIndex > parent.endShowDataItemIndex + 1)
                                        endIndex = parent.endShowDataItemIndex + 1;
                                }

                                for (int i = startIndex; i < endIndex; ++i)
                                {
                                    DrawAvgLineGraph(g, adc.avgPointMapLst[i], winW, winH, cdt, adc.avgLineSetting.pen);
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
                        if (parent.endShowDataItemIndex != -1)
                        {
                            if (endIndex > parent.endShowDataItemIndex + 1)
                                endIndex = parent.endShowDataItemIndex + 1;
                        }
                        for (int i = 0; i < endIndex; ++i)
                        {
                            DrawBollinLineGraph(g, kddc.bollinDataLst.bollinMapLst[i], winW, winH, cdt);
                            if (i == preViewDataIndex)
                            {
                                if (GlobalSetting.G_SHOW_KCURVE_DETAIL)
                                {
                                    int index = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                                    float missHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[index];
                                    BollinPointMap bpm = kddc.bollinDataLst.bollinMapLst[i];
                                    BollinPoint bp = bpm.GetData(cdt, false);
                                    float bandSize = bp.upValue - bp.downValue;
                                    float bandCount = bandSize / missHeight;
                                    string info = "Boll值 = (" + bp.upValue + ", " + bp.midValue + ", " + bp.downValue + ") [" + bandSize + ", " + bandCount + "]";
                                    KData kd = bpm.kdd.GetData(cdt, false);
                                    float bollUpCount = kd.RelateDistTo(bp.upValue) / missHeight;
                                    float bollMidCount = kd.RelateDistTo(bp.midValue) / missHeight;
                                    float bollDownCount = kd.RelateDistTo(bp.downValue) / missHeight;
                                    info += "\nBUpC = " + bollUpCount + ", BMdC = " + bollMidCount + ", BDnC = " + bollDownCount;
                                    info += "\nUpMC = " + bp.uponBolleanMidCount + ", OnMC = " + bp.onBolleanMidCount + ", DnMC = " + bp.underBolleanMidCount;
                                    info += "\nUpMCC = " + bp.uponBolleanMidCountContinue + ", OnMCC = " + bp.onBolleanMidCountContinue + ", DnMCC = " + bp.underBolleanMidCountContinue;
                                    info += "\nBMKUC = " + bp.bolleanMidKeepUpCount + ", BMKHC = " + bp.bolleanMidKeepHorzCount + ", BMKDC = " + bp.bolleanMidKeepDownCount;
                                    info += "\nBMKUCC = " + bp.bolleanMidKeepUpCountContinue + ", BMKHCC = " + bp.bolleanMidKeepHorzCountContinue + ", BMKDCC = " + bp.bolleanMidKeepDownCountContinue;
                                    info += "\nOnDownCC = " + bp.onBolleanDownCountContinue;
                                    graphInfo += "\n" + info;
                                    //g.DrawString(info, selDataFont, whiteBrush, 5, 45);
                                }
                            }
                        }
                    }
                }

                // 画辅助线
                if (enableAuxiliaryLine)
                {
                    DrawAuxLineGraph(g, winW, winH, mouseRelPos, numIndex, cdt, auxiliaryLineListUpPanel, mouseHitPtsUpPanel, true);
                }

                g.DrawLine(grayDotLinePen, 0, mouseRelPos.Y, winW, mouseRelPos.Y);
                float kValueMouse = CanvasToStand(mouseRelPos.Y, false, true) / gridScaleUp.Y;
                g.DrawString(kValueMouse.ToString("f3"), selDataFont, whiteBrush, winW - 80, mouseRelPos.Y - 20);
                //g.DrawLine(grayDotLinePen, mouseRelPos.X, 0, mouseRelPos.X, winH);
            }

            if (enableAuxiliaryLine && preViewDataIndex != beforeID)
            {
                parent.MakeWindowRepaint();
            }
            EndDraw(g);

            g.DrawString(graphInfo, selDataFont, whiteBrush, 5, 5);
        }
        public override void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);

            BeforeDraw();
            if (enableMACD && kddc != null)
            {
                int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];

                float oriYOff = downCanvasOffset.Y;
                downCanvasOffset.Y = winH * 0.5f + DownGraphYOffset;
                lastValue = 0;
                findPrevPt = false;
                int startIndex = (int)(canvasOffset.X / gridScaleDown.X) - 1;
                if (startIndex < 0)
                    startIndex = 0;
                int endIndex = (int)((canvasOffset.X + winW) / gridScaleDown.X) + 1;
                if (parent.endShowDataItemIndex != -1)
                {
                    if (endIndex > parent.endShowDataItemIndex + 1)
                        endIndex = parent.endShowDataItemIndex + 1;
                }
                if (endIndex >= kddc.macdDataLst.macdMapLst.Count)
                    endIndex = kddc.macdDataLst.macdMapLst.Count - 1;
                for (int i = startIndex; i <= endIndex; ++i)
                {
                    DrawMACDGraph(g, kddc.macdDataLst.macdMapLst[i], winW, winH, cdt, i == selectKDataIndex);
                }

                if (preViewDataIndex != -1)
                {
#if TRADE_DBG
                    MACDPointMap mpm = kddc.macdDataLst.macdMapLst[preViewDataIndex];
                    MACDPoint mp = mpm.GetData(cdt, false);
                    MACDLimitValue mlv = mpm.parent.macdLimitValueMap[cdt];
                    float _gridScaleH = winH * 0.45f / Math.Max(Math.Abs(mlv.MaxValue), Math.Abs(mlv.MinValue));
                    float CX = StandToCanvas(preViewDataIndex * gridScaleDown.X, true, false);
                    float CY = StandToCanvas(mp.DIF * _gridScaleH, false, false);

                    if (mpm.index >= 1 && mp.LEFT_DIF_INDEX != -1)
                    {
                        string info = "MACD线 = " + ((TradeDataManager.MACDLineWaveConfig)(mp.WAVE_CFG)).ToString() + 
                            ", MACD柱 = " + ((TradeDataManager.MACDBarConfig)(mp.BAR_CFG)).ToString() + 
                            ", K线 = " + ((TradeDataManager.KGraphConfig)(mp.KGRAPH_CFG)).ToString();
                        info += mp.IS_STRONG_UP ? ", 强列上升" : "";
                        g.DrawString(info, auxFont, whiteBrush, 5, 5);
                        info = "MACD 快线值 = " + mp.DIF + ", 慢线值 = " + mp.DEA + ", 柱值 = " + mp.BAR;
                        g.DrawString(info, auxFont, whiteBrush, 5, 25);
                        g.DrawString(mp.LAST_DATA_TAG, auxFont, whiteBrush, 5, 45);

                        int leftID = mp.LEFT_DIF_INDEX;

                        List<int> ids = new List<int>();
                        ids.Add(leftID);
                        if (mp.MAX_DIF_INDEX >= 0)
                            ids.Add(mp.MAX_DIF_INDEX);
                        if (mp.MIN_DIF_INDEX >= 0)
                            ids.Add(mp.MIN_DIF_INDEX);
                        ids.Add(mpm.index);
                        ids.Sort();

                        for (int i = 1; i < ids.Count; ++i)
                        {
                            MACDPointMap mpm1 = kddc.macdDataLst.macdMapLst[ids[i]];
                            MACDPointMap mpm2 = kddc.macdDataLst.macdMapLst[ids[i - 1]];
                            float x1 = StandToCanvas(mpm1.index * gridScaleDown.X, true, false);
                            float x2 = StandToCanvas(mpm2.index * gridScaleDown.X, true, false);
                            float y1 = StandToCanvas(mpm1.GetData(cdt, false).DIF * _gridScaleH, false, false);
                            float y2 = StandToCanvas(mpm2.GetData(cdt, false).DIF * _gridScaleH, false, false);
                            g.DrawLine(greenLinePen, x1, y1, x2, y2);
                        }
                    }
                    else
                    {
                        string info = mpm.index + ", MACD 快线值 = " + mp.DIF + ", 慢线值 = " + mp.DEA + ", 柱值 = " + mp.BAR;
                        g.DrawString(info, auxFont, whiteBrush, 5, 5);
                    }
#endif
                }

                downCanvasOffset.Y = oriYOff;

                if (preViewDataIndex != -1)
                {
                    g.DrawLine(grayDotLinePen, selDataPtX, 0, selDataPtX, winH);
                    g.DrawLine(grayDotLinePen, selDataPtX + gridScaleDown.X, 0, selDataPtX + gridScaleDown.X, winH);
                }

                // 画辅助线
                if (enableAuxiliaryLine)
                {
                    DrawAuxLineGraph(g, winW, winH, mouseRelPos, numIndex, cdt, auxiliaryLineListDownPanel, mouseHitPtsDownPanel, false);
                }
            }
            EndDraw(g);
        }
        public override void ScrollToData(int index, int winW, int winH, bool needSelect, int xOffset = 0, bool needScrollToData = true)
        {
            if (needSelect)
                selectKDataIndex = index;
            else
                selectKDataIndex = -1;
            if (needScrollToData)
            {
                canvasOffset.X = index * gridScaleDown.X + xOffset;
                downCanvasOffset.X = canvasOffset.X;
            }
            autoAllign = true;
        }

        public int SelectKData(Point mouseRelPos, bool upPanel)
        {
            DataManager dm = DataManager.GetInst();
            selectKDataIndex = -1;
            Point standMousePos = CanvasToStand(mouseRelPos, upPanel);
            int mouseHoverID = (int)(standMousePos.X / gridScaleUp.X);
            if (mouseHoverID >= dm.GetAllDataItemCount())
                mouseHoverID = -1;
            selectKDataIndex = mouseHoverID;
            return selectKDataIndex;
        }

        public void UnSelectData()
        {
            selectKDataIndex = -1;
            autoAllign = true;
        }

        public void GetViewItemIndexInfo(ref int startIndex, ref int maxIndex)
        {
            startIndex = (int)(canvasOffset.X / gridScaleUp.X) - 1;
            if (startIndex < 0)
                startIndex = 0;
            maxIndex = GraphDataManager.KGDC.DataLength();
        }

        public void UpdateSelectAuxLinePoint(Point mouseRelPos, int dx, int dy, bool upPanel)
        {
            if (upPanel)
            {
                if (selAuxLineUpPanel != null && selAuxLinePointIndexUpPanel >= 0 && selAuxLinePointIndexUpPanel < selAuxLineUpPanel.keyPoints.Count)
                {
                    selAuxLineUpPanel.keyPoints[selAuxLinePointIndexUpPanel] = CanvasToStand(mouseRelPos, upPanel);
                    if (selAuxLineUpPanel.lineType == AuxLineType.eCircleLine)
                    {
                        (selAuxLineUpPanel as AuxiliaryLineCircle).CalcRect();
                    }
                }
                else if (selAuxLineUpPanel != null && selAuxLinePointIndexUpPanel == -1)
                {
                    for (int i = 0; i < selAuxLineUpPanel.keyPoints.Count; ++i)
                    {
                        Point p = selAuxLineUpPanel.keyPoints[i];
                        p.Offset(dx, dy);
                        selAuxLineUpPanel.keyPoints[i] = p;
                    }
                }
            }
            else
            {
                if (selAuxLineDownPanel != null && selAuxLinePointIndexDownPanel >= 0 && selAuxLinePointIndexDownPanel < selAuxLineDownPanel.keyPoints.Count)
                {
                    selAuxLineDownPanel.keyPoints[selAuxLinePointIndexDownPanel] = CanvasToStand(mouseRelPos, upPanel);
                    if (selAuxLineDownPanel.lineType == AuxLineType.eCircleLine)
                    {
                        (selAuxLineDownPanel as AuxiliaryLineCircle).CalcRect();
                    }
                }
                else if (selAuxLineDownPanel != null && selAuxLinePointIndexDownPanel == -1)
                {
                    for (int i = 0; i < selAuxLineDownPanel.keyPoints.Count; ++i)
                    {
                        Point p = selAuxLineDownPanel.keyPoints[i];
                        p.Offset(dx, dy);
                        selAuxLineDownPanel.keyPoints[i] = p;
                    }
                }
            }
        }

        public void SelectAuxLine(Point mouseRelPos, int numIndex, CollectDataType cdt, bool upPanel)
        {
            if (upPanel)
            {
                selAuxLineUpPanel = null;
                selAuxLinePointIndexUpPanel = -1;
                Point standMousePos = CanvasToStand(mouseRelPos, upPanel);
                for (int i = 0; i < auxiliaryLineListUpPanel.Count; ++i)
                {
                    AuxiliaryLineBase al = auxiliaryLineListUpPanel[i];
                    int selPtID = -1;
                    bool sel = al.HitTest(cdt, numIndex, standMousePos, rcHalfSize, ref selPtID);
                    if (sel)
                    {
                        selAuxLineUpPanel = al;
                        selAuxLinePointIndexUpPanel = selPtID;
                        return;
                    }
                }
            }
            else
            {
                selAuxLineDownPanel = null;
                selAuxLinePointIndexDownPanel = -1;
                Point standMousePos = DownCanvasToStand(mouseRelPos);
                for (int i = 0; i < auxiliaryLineListDownPanel.Count; ++i)
                {
                    AuxiliaryLineBase al = auxiliaryLineListDownPanel[i];
                    int selPtID = -1;
                    bool sel = al.HitTest(cdt, numIndex, standMousePos, rcHalfSize, ref selPtID);
                    if (sel)
                    {
                        selAuxLineDownPanel = al;
                        selAuxLinePointIndexDownPanel = selPtID;
                        return;
                    }
                }
            }
        }
        public void RemoveAllAuxLines()
        {
            auxiliaryLineListUpPanel.Clear();
            auxiliaryLineListDownPanel.Clear();
        }
        public void RemoveSelectAuxLine(bool upPanel)
        {
            if (upPanel)
            {
                if (selAuxLineUpPanel != null)
                {
                    auxiliaryLineListUpPanel.Remove(selAuxLineUpPanel);
                    selAuxLineUpPanel = null;
                    selAuxLinePointIndexUpPanel = -1;
                }
            }
            else
            {
                if (selAuxLineDownPanel != null)
                {
                    auxiliaryLineListDownPanel.Remove(selAuxLineDownPanel);
                    selAuxLineDownPanel = null;
                    selAuxLinePointIndexDownPanel = -1;
                }
            }
        }
        public void AddHorzLine(Point pt, int numIndex, CollectDataType cdt, bool upPanel)
        {
            AuxiliaryLineHorz line = new AuxiliaryLineHorz();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(pt, upPanel));
            line.valuePoints.Add(CanvasToValue(pt, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddVertLine(Point pt, int numIndex, CollectDataType cdt, bool upPanel)
        {
            AuxiliaryLineVert line = new AuxiliaryLineVert();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(pt, upPanel));
            line.valuePoints.Add(CanvasToValue(pt, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddSingleLine(Point p1, Point p2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            AuxiliaryLineSupportPressure line = new AuxiliaryLineSupportPressure();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(p2, upPanel));
            line.valuePoints.Add(CanvasToValue(p1, upPanel));
            line.valuePoints.Add(CanvasToValue(p2, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddChannelLine(Point line0P1, Point line0P2, Point line1P, int numIndex, CollectDataType cdt, bool upPanel)
        {
            AuxiliaryLineChannel line = new AuxiliaryLineChannel();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(line0P1, upPanel));
            line.keyPoints.Add(CanvasToStand(line0P2, upPanel));
            line.keyPoints.Add(CanvasToStand(line1P, upPanel));
            line.valuePoints.Add(CanvasToValue(line0P1, upPanel));
            line.valuePoints.Add(CanvasToValue(line0P2, upPanel));
            line.valuePoints.Add(CanvasToValue(line1P, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddGoldSegLine(Point p1, Point P2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            AuxiliaryLineGoldenSection line = new AuxiliaryLineGoldenSection();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(P2, upPanel));
            line.valuePoints.Add(CanvasToValue(p1, upPanel));
            line.valuePoints.Add(CanvasToValue(P2, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddCircleLine(Point p1, Point p2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            AuxiliaryLineCircle line = new AuxiliaryLineCircle();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(p2, upPanel));
            line.valuePoints.Add(CanvasToValue(p1, upPanel));
            line.valuePoints.Add(CanvasToValue(p2, upPanel));
            line.CalcRect();
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddArrowLine(Point p1, Point p2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            AuxiliaryLineArrow line = new AuxiliaryLineArrow();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(p2, upPanel));
            line.valuePoints.Add(CanvasToValue(p1, upPanel));
            line.valuePoints.Add(CanvasToValue(p2, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddRectLine(Point p1, Point p2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            AuxiliaryLineRect line = new AuxiliaryLineRect();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(p2, upPanel));
            line.valuePoints.Add(CanvasToValue(p1, upPanel));
            line.valuePoints.Add(CanvasToValue(p2, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }

        void BeforeDraw()
        {
            foreach (Pen k in linePools.Keys)
            {
                linePools[k].Clear();
            }
            foreach (SolidBrush k in rcPools.Keys)
            {
                rcPools[k].Clear();
            }
            foreach (Pen k in segPools.Keys)
            {
                segPools[k].Clear();
            }
        }
        void EndDraw(Graphics g)
        {
            foreach (SolidBrush k in rcPools.Keys)
            {
                if (rcPools[k].Count > 0)
                    g.FillRectangles(k, rcPools[k].ToArray());
            }
            foreach (Pen k in linePools.Keys)
            {
                if (linePools[k].Count > 0)
                    g.DrawLines(k, linePools[k].ToArray());
            }
            foreach (Pen k in segPools.Keys)
            {
                if (segPools[k].Count > 0)
                {
                    List<PointF> pts = segPools[k];
                    for (int i = 0; i < pts.Count;)
                    {
                        g.DrawLine(k, pts[i], pts[i + 1]);
                        i += 2;
                    }
                }
            }
        }
        void PushSegPts(Pen pen, float x1, float y1, float x2, float y2)
        {
            List<PointF> pts = GetSegLst(pen);
            pts.Add(new PointF(x1, y1));
            pts.Add(new PointF(x2, y2));
        }
        void PushLinePts(Pen pen, float x1, float y1, float x2, float y2)
        {
            List<PointF> pts = GetLineLst(pen);
            pts.Add(new PointF(x1, y1));
            pts.Add(new PointF(x2, y2));
        }
        void PushRcPts(SolidBrush brush, float x, float y, float w, float h)
        {
            List<RectangleF> rcs = GetRCLst(brush);
            rcs.Add(new RectangleF(x, y, w, h));
        }
        List<PointF> GetSegLst(Pen pen)
        {
            List<PointF> res = null;
            segPools.TryGetValue(pen, out res);
            if (res == null)
            {
                res = new List<PointF>();
                segPools[pen] = res;
            }
            return res;
        }
        List<PointF> GetLineLst(Pen pen)
        {
            List<PointF> linePts = null;
            if (linePools.ContainsKey(pen))
                linePts = linePools[pen];
            else
            {
                linePts = new List<PointF>();
                linePools.Add(pen, linePts);
            }
            return linePts;
        }
        List<RectangleF> GetRCLst(SolidBrush brush)
        {
            List<RectangleF> rcLst = null;
            if (rcPools.ContainsKey(brush))
                rcLst = rcPools[brush];
            else
            {
                rcLst = new List<RectangleF>();
                rcPools.Add(brush, rcLst);
            }
            return rcLst;
        }


        void DrawKDataGraph(Graphics g, KData data, int winW, int winH, float missRelHeight, Point mouseRelPos, KDataDictContainer kddc, CollectDataType cdt)
        {
            KData prevData = data.GetPrevKData();
            float standX = data.index * gridScaleUp.X;
            float midX = standX + gridScaleUp.X * 0.5f;
            float up = data.UpValue * gridScaleUp.Y;
            float down = data.DownValue * gridScaleUp.Y;
            float start = data.StartValue * gridScaleUp.Y;
            float end = data.EndValue * gridScaleUp.Y;
            standX = StandToCanvas(standX, true, true);
            midX = StandToCanvas(midX, true, true);
            up = StandToCanvas(up, false, true);
            down = StandToCanvas(down, false, true);
            start = StandToCanvas(start, false, true);
            end = StandToCanvas(end, false, true);
            float rcY = data.StartValue > data.EndValue ? start : end;
            float rcH = Math.Abs(start - end);
            if (rcH < 1)
                rcH = 1;
            tmpBrush = data.EndValue > data.StartValue ? redBrush : (data.EndValue < data.StartValue ? cyanBrush : whiteBrush);
            Pen linePen = data.EndValue > data.StartValue ? redLinePen : (data.EndValue < data.StartValue ? cyanLinePen : whiteLinePen);
            g.DrawLine(linePen, midX, up, midX, down);
            PushRcPts(tmpBrush, standX, rcY, gridScaleUp.X, rcH);


            if (preViewDataIndex < 0 && standX <= mouseRelPos.X && standX + gridScaleUp.X >= mouseRelPos.X)
            {
                selDataPtX = standX;
                preViewDataIndex = data.index;
                g.DrawLine(grayDotLinePen, standX, 0, standX, winH);
                g.DrawLine(grayDotLinePen, standX + gridScaleUp.X, 0, standX + gridScaleUp.X, winH);
                //g.DrawString(data.GetInfo(), selDataFont, whiteBrush, 5, 5);
                //string str = "K值 = " + data.KValue.ToString() + ", 上 = " + data.UpValue.ToString() + ", 下 = " + data.DownValue.ToString();
                //g.DrawString(str, selDataFont, whiteBrush, 5, 25);
                graphInfo += data.GetInfo() +
                    "K值 = " + data.KValue.ToString() +
                    ", 上 = " + data.UpValue.ToString() +
                    ", 下 = " + data.DownValue.ToString() +
                    ", 开 = " + data.StartValue.ToString() +
                    ", 收 = " + data.EndValue.ToString();
            }

            if (data.index == selectKDataIndex)
            {
                g.DrawLine(yellowLinePen, standX, 0, standX, winH);
                g.DrawLine(yellowLinePen, standX + gridScaleUp.X, 0, standX + gridScaleUp.X, winH);
            }

            //if (prevData != null)
            //{
            //    MACDPoint mpC = kddc.GetMacdPointMap(data.index).GetData(cdt, false);
            //    MACDPoint mpP = kddc.GetMacdPointMap(prevData.index).GetData(cdt, false);
            //    if (mpC.BAR > mpP.BAR)
            //    {
            //        g.FillRectangle(redBrush, standX, winH-gridScaleH, gridScaleW, winH);
            //    }
            //    else if(mpC.DIF > 0)
            //    {
            //        g.FillRectangle(yellowBrush, standX, winH - gridScaleH, gridScaleW, winH);
            //    }
            //}
        }
        void DrawAvgLineGraph(Graphics g, AvgPointMap apm, int winW, int winH, CollectDataType cdt, Pen pen)
        {
            AvgPoint ap = apm.apMap[cdt];
            float standX = (apm.index + 0.5f) * gridScaleUp.X;
            float standY = ap.avgKValue * gridScaleUp.Y;
            if (findPrevPt == false)
            {
                findPrevPt = true;
                prevPt.X = standX;
                prevPt.Y = standY;
                return;
            }
            float cx = StandToCanvas(standX, true, true);
            if (cx < 0 || cx > winW)
            {
                prevPt.X = standX;
                prevPt.Y = standY;
                return;
            }
            float px = StandToCanvas(prevPt.X, true, true);
            float py = StandToCanvas(prevPt.Y, false, true);
            float cy = StandToCanvas(standY, false, true);
            prevPt.X = standX;
            prevPt.Y = standY;
            PushLinePts(pen, px, py, cx, cy);
        }
        void DrawBollinLineGraph(Graphics g, BollinPointMap bpm, int winW, int winH, CollectDataType cdt)
        {
            BollinPoint bp = bpm.bpMap[cdt];
            float standX = (bpm.index + 0.5f) * gridScaleUp.X;
            if (findPrevPt == false)
            {
                findPrevPt = true;
                return;
            }
            float cx = StandToCanvas(standX, true, true);
            if (cx < 0 || cx > winW)
            {
                return;
            }
            BollinPointMap prevBPM = bpm.GetPrevBPM();
            BollinPoint prevBP = prevBPM.bpMap[cdt];
            float px = StandToCanvas((prevBPM.index + 0.5f) * gridScaleUp.X, true, true);
            float pyU = StandToCanvas(prevBP.upValue * gridScaleUp.Y, false, true);
            float pyM = StandToCanvas(prevBP.midValue * gridScaleUp.Y, false, true);
            float pyD = StandToCanvas(prevBP.downValue * gridScaleUp.Y, false, true);
            float cyU = StandToCanvas(bp.upValue * gridScaleUp.Y, false, true);
            float cyM = StandToCanvas(bp.midValue * gridScaleUp.Y, false, true);
            float cyD = StandToCanvas(bp.downValue * gridScaleUp.Y, false, true);
            Pen midPen = bollinLinePenMid;
            if (bp.bolleanMidKeepDownCountContinue > 0)//prevBP.midValue > bp.midValue)
                midPen = bollinLinePenMidG;
            else if (bp.bolleanMidKeepUpCountContinue > 0)//prevBP.midValue < bp.midValue)
                midPen = bollinLinePenMidR;
            PushLinePts(bollinLinePenUp, px, pyU, cx, cyU);
            PushSegPts(midPen, px, pyM, cx, cyM);
            PushLinePts(bollinLinePenDown, px, pyD, cx, cyD);
        }
        void DrawMACDGraph(Graphics g, MACDPointMap mpm, int winW, int winH, CollectDataType cdt, bool drawSelectedLine)
        {
            MACDPoint mp = mpm.macdpMap[cdt];
            bool isUp = mp.BAR > 0;
            MACDPointMap prevMPM = mp.parent.GetPrevMACDPM();
            if (prevMPM != null)
            {
                isUp = mp.BAR > prevMPM.GetData(cdt, false).BAR;
            }
            float standX = (mpm.index + 0.5f) * gridScaleDown.X;
            float halfW = gridScaleDown.X * 0.5f;
            float cx = StandToCanvas(standX, true, false);
            if (cx < 0 || cx > winW)
            {
                return;
            }
            MACDLimitValue mlv = mpm.parent.macdLimitValueMap[cdt];
            float gridScaleH = winH * 0.45f / Math.Max(Math.Abs(mlv.MaxValue), Math.Abs(mlv.MinValue));
            float standY = mp.BAR * gridScaleH;
            float cyDIF = StandToCanvas((mp.DIF * gridScaleH), false, false);
            float cyDEA = StandToCanvas((mp.DEA * gridScaleH), false, false);
            float cyBAR = StandToCanvas(standY, false, false);
            float rcY = StandToCanvas(standY > 0 ? standY : 0, false, false);
            g.FillRectangle(isUp ? redBrush : cyanBrush, cx - halfW, rcY, gridScaleDown.X, Math.Abs(standY));
            //MACDPointMap prevMPM = mpm.GetPrevMACDPM();
            if (prevMPM != null)
            {
                MACDPoint prevMP = prevMPM.macdpMap[cdt];
                float px = cx - gridScaleDown.X;
                float pyDIF = StandToCanvas((prevMP.DIF * gridScaleH), false, false);
                float pyDEA = StandToCanvas((prevMP.DEA * gridScaleH), false, false);
                g.DrawLine(yellowLinePen, px, pyDIF, cx, cyDIF);
                g.DrawLine(whiteLinePen, px, pyDEA, cx, cyDEA);
            }

            if (drawSelectedLine)
            {
                float xL = cx - halfW;
                float xR = cx + halfW;
                g.DrawLine(yellowLinePen, xL, 0, xL, winH);
                g.DrawLine(yellowLinePen, xR, 0, xR, winH);
            }

            gridScaleDown.Y = gridScaleH;
        }

        void DrawAuxLineGraph(Graphics g, int winW, int winH, Point mouseRelPos, int numIndex, CollectDataType cdt, List<AuxiliaryLineBase> auxLines, List<Point> auxPoints, bool upPanel)
        {
            float rcHalfSize = 3;
            float rcSize = rcHalfSize * 2;
            for (int i = 0; i < auxLines.Count; ++i)
            {
                AuxiliaryLineBase al = auxLines[i];
                if (al.cdt == cdt && al.numIndex == numIndex)
                    DrawAuxLine(g, winW, winH, al, upPanel);
            }

            if (auxPoints.Count > 0 && auxOperationIndex > AuxLineType.eNone)
            {
                DrawPreviewAuxLine(g, winW, winH, mouseRelPos, auxPoints);
            }

            if (upPanel)
            {
                if (selAuxLineUpPanel != null && selAuxLinePointIndexUpPanel != -1)
                {
                    Point pt = StandToCanvas(selAuxLineUpPanel.keyPoints[selAuxLinePointIndexUpPanel], upPanel);
                    g.DrawRectangle(selAuxLineUpPanel.GetSolidPen(), pt.X - rcHalfSize - 4, pt.Y - rcHalfSize - 4, rcSize + 8, rcSize + 8);
                }
            }
            else
            {
                if (selAuxLineDownPanel != null && selAuxLinePointIndexDownPanel != -1)
                {
                    Point pt = StandToCanvas(selAuxLineDownPanel.keyPoints[selAuxLinePointIndexDownPanel], upPanel);
                    g.DrawRectangle(selAuxLineDownPanel.GetSolidPen(), pt.X - rcHalfSize - 4, pt.Y - rcHalfSize - 4, rcSize + 8, rcSize + 8);
                }
            }
        }

        void DrawAutoAuxTools(Graphics g, int winW, int winH, int numIndex, CollectDataType cdt, bool upPanel)
        {
            float gridScaleW = upPanel ? gridScaleUp.X : gridScaleDown.X;
            float gridScaleH = upPanel ? gridScaleUp.Y : gridScaleDown.Y;

            try
            {
                if (preViewDataIndex != -1)
                {
                    TradeDataManager.Instance.curPreviewAnalyzeTool.Analyze(preViewDataIndex);
                    AutoAnalyzeTool.SingleAuxLineInfo sali = TradeDataManager.Instance.curPreviewAnalyzeTool.GetSingleAuxLineInfo(numIndex, cdt);

                    if (needAutoAddAuxLine)
                    {
                        needAutoAddAuxLine = false;
                        if (sali.upLineData.valid)
                        {
                            if (sali.upLineData.dataPrevSharp != null && sali.upLineData.dataSharp != null)
                            {
                                int px = (int)StandToCanvas(sali.upLineData.dataPrevSharp.index * gridScaleW, true, upPanel);
                                int py = (int)StandToCanvas(sali.upLineData.dataPrevSharp.KValue * gridScaleH, false, upPanel);
                                int x = (int)StandToCanvas(sali.upLineData.dataSharp.index * gridScaleW, true, upPanel);
                                int y = (int)StandToCanvas(sali.upLineData.dataSharp.KValue * gridScaleH, false, upPanel);
                                AddSingleLine(
                                    new Point(px, py),
                                    new Point(x, y),
                                    numIndex, cdt, upPanel);
                            }
                            if (sali.upLineData.dataNextSharp != null && sali.upLineData.dataSharp != null)
                            {
                                int px = (int)StandToCanvas(sali.upLineData.dataNextSharp.index * gridScaleW, true, upPanel);
                                int py = (int)StandToCanvas(sali.upLineData.dataNextSharp.KValue * gridScaleH, false, upPanel);
                                int x = (int)StandToCanvas(sali.upLineData.dataSharp.index * gridScaleW, true, upPanel);
                                int y = (int)StandToCanvas(sali.upLineData.dataSharp.KValue * gridScaleH, false, upPanel);
                                AddSingleLine(
                                    new Point(px, py),
                                    new Point(x, y),
                                    numIndex, cdt, upPanel);
                            }
                        }

                        if (sali.downLineData.valid)
                        {
                            if (sali.downLineData.dataPrevSharp != null && sali.downLineData.dataSharp != null)
                            {
                                int px = (int)StandToCanvas(sali.downLineData.dataPrevSharp.index * gridScaleW, true, upPanel);
                                int py = (int)StandToCanvas(sali.downLineData.dataPrevSharp.KValue * gridScaleH, false, upPanel);
                                int x = (int)StandToCanvas(sali.downLineData.dataSharp.index * gridScaleW, true, upPanel);
                                int y = (int)StandToCanvas(sali.downLineData.dataSharp.KValue * gridScaleH, false, upPanel);
                                AddSingleLine(
                                    new Point(px, py),
                                    new Point(x, y),
                                    numIndex, cdt, upPanel);
                            }
                            if (sali.downLineData.dataNextSharp != null && sali.downLineData.dataSharp != null)
                            {
                                int px = (int)StandToCanvas(sali.downLineData.dataNextSharp.index * gridScaleW, true, upPanel);
                                int py = (int)StandToCanvas(sali.downLineData.dataNextSharp.KValue * gridScaleH, false, upPanel);
                                int x = (int)StandToCanvas(sali.downLineData.dataSharp.index * gridScaleW, true, upPanel);
                                int y = (int)StandToCanvas(sali.downLineData.dataSharp.KValue * gridScaleH, false, upPanel);
                                AddSingleLine(
                                    new Point(px, py),
                                    new Point(x, y),
                                    numIndex, cdt, upPanel);
                            }
                        }
                    }

                    DrawAutoAuxTools(TradeDataManager.Instance.curPreviewAnalyzeTool, g, winW, winH, numIndex, cdt, preViewDataIndex, upPanel);
                    if (sali.downLineData.valid)
                    {
                        GraphDataContainerKGraph kgdc = GraphDataManager.KGDC;
                        KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
                        KDataMap kdd = kddc.dataLst[preViewDataIndex];
                        KData kd = kdd.GetData(cdt, false);
                        float missHeight = GraphDataManager.GetMissRelLength(cdt);

                        bool hasPrevKV, hasNextKV, hasPrevHitPt, hasNextHitPt;
                        float prevKV, nextKV, prevSlope, nextSlope;
                        float prevHitPtX, prevHitPtY, nextHitPtX, nextHitPtY;

                        int testID = preViewDataIndex + 1;
                        float kdX = testID * gridScaleW;
                        float kdY = kd.UpValue * gridScaleH;
                        kdX = StandToCanvas(kdX, true, upPanel);
                        kdY = StandToCanvas(kdY, false, upPanel);

                        float lblX = winW - 100;
                        float lblY = 20;
                        float lblH = 20;

                        // 计算当前期在下通道线上的K值
                        sali.downLineData.GetKValue(testID, kd.UpValue, -missHeight,
                            out hasPrevKV, out prevKV, out prevSlope, out hasPrevHitPt, out prevHitPtX, out prevHitPtY,
                            out hasNextKV, out nextKV, out nextSlope, out hasNextHitPt, out nextHitPtX, out nextHitPtY);
                        if (hasPrevKV)
                        {
                            float prevY = prevKV * gridScaleH;
                            prevY = StandToCanvas(prevY, false, upPanel);
                            g.DrawRectangle(redLinePen, new Rectangle((int)kdX - 5, (int)prevY - 5, 10, 10));
                            g.DrawLine(redLinePen, kdX, kdY, kdX, prevY);

                            g.DrawLine(redLinePen, kdX, prevY, lblX, lblY);
                            g.DrawString(((prevKV - kd.KValue) / missHeight).ToString("f1"), rulerFont, redBrush, lblX, lblY);
                            lblY += lblH;
                        }
                        if (hasNextKV)
                        {
                            float nextY = nextKV * gridScaleH;
                            nextY = StandToCanvas(nextY, false, upPanel);
                            g.DrawRectangle(redLinePen, new Rectangle((int)kdX - 5, (int)nextY - 5, 10, 10));
                            g.DrawLine(redLinePen, kdX, kdY, kdX, nextY);

                            g.DrawLine(redLinePen, kdX, nextY, lblX, lblY);
                            g.DrawString(((nextKV - kd.KValue) / missHeight).ToString("f1"), rulerFont, redBrush, lblX, lblY);
                            lblY += lblH;
                        }
                        if (hasPrevHitPt)
                        {
                            float prevX = prevHitPtX * gridScaleW;
                            float prevY = prevHitPtY * gridScaleH;
                            prevX = StandToCanvas(prevX, true, upPanel);
                            prevY = StandToCanvas(prevY, false, upPanel);
                            g.DrawRectangle(greenLinePen, new Rectangle((int)prevX - 5, (int)prevY - 5, 10, 10));
                            g.DrawLine(greenLinePen, kdX, kdY, prevX, prevY);

                            g.DrawLine(greenLinePen, prevX, prevY, lblX, lblY);
                            g.DrawString(((prevHitPtY - kd.KValue) / missHeight).ToString("f1"), rulerFont, greenBrush, lblX, lblY);
                            lblY += lblH;
                        }
                        if (hasNextHitPt)
                        {
                            float nextX = nextHitPtX * gridScaleW;
                            float nextY = nextHitPtY * gridScaleH;
                            nextX = StandToCanvas(nextX, true, upPanel);
                            nextY = StandToCanvas(nextY, false, upPanel);
                            g.DrawRectangle(greenLinePen, new Rectangle((int)nextX - 5, (int)nextY - 5, 10, 10));
                            g.DrawLine(greenLinePen, kdX, kdY, nextX, nextY);

                            g.DrawLine(greenLinePen, nextX, nextY, lblX, lblY);
                            g.DrawString(((nextHitPtY - kd.KValue) / missHeight).ToString("f1"), rulerFont, greenBrush, lblX, lblY);
                            lblY += lblH;
                        }
                    }
                }
                else
                {
                    DrawAutoAuxTools(TradeDataManager.Instance.autoAnalyzeTool, g, winW, winH, numIndex, cdt, -1, upPanel);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception on DrawAutoAuxTools - " + e.ToString());
            }
        }

        void DrawAutoAuxTools(AutoAnalyzeTool autoAnalyzeTool, Graphics g, int winW, int winH, int numIndex, CollectDataType cdt, int preViewDataIndex, bool upPanel)
        {
            float gridScaleW = upPanel ? gridScaleUp.X : gridScaleDown.X;
            float gridScaleH = upPanel ? gridScaleUp.Y : gridScaleDown.Y;

            if (autoAnalyzeTool == null)
                return;
            if (preViewDataIndex != -1)
            {
                float ex = (preViewDataIndex + 1) * gridScaleW;
                float width = GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT * gridScaleW;
                float sx = ex - width;
                ex = StandToCanvas(ex, true, upPanel);
                sx = StandToCanvas(sx, true, upPanel);
                g.FillRectangle(previewAnalyzeToolBrush, sx, 0, width, winH);
            }
            AutoAnalyzeTool.SingleAuxLineInfo sali = autoAnalyzeTool.GetSingleAuxLineInfo(numIndex, cdt);
            if (sali.upLineData.valid)
                DrawAuxLineData(sali.upLineData, g, winW, winH, redLinePenB, orangeLinePenB, redLinePenB, upPanel);
            if (sali.downLineData.valid)
                DrawAuxLineData(sali.downLineData, g, winW, winH, cyanLinePenB, blueLinePenB, cyanLinePenB, upPanel);
        }

        void DrawAuxLineData(AutoAnalyzeTool.AuxLineData lineData, Graphics g, int winW, int winH, Pen pen, Pen prevPen, Pen nextPen, bool upPanel)
        {
            float gridScaleW = upPanel ? gridScaleUp.X : gridScaleDown.X;
            float gridScaleH = upPanel ? gridScaleUp.Y : gridScaleDown.Y;

            float bx = lineData.dataSharp.index * gridScaleW, sx;
            float by = lineData.dataSharp.KValue * gridScaleH, sy;
            bx = StandToCanvas(bx, true, upPanel);
            by = StandToCanvas(by, false, upPanel);
            g.DrawLine(grayDotLinePen, 0, by, winW, by);
            g.DrawRectangle(pen, bx - rcHalfSize, by - rcHalfSize, rcSize, rcSize);

            if (lineData.dataPrevSharp != null)
            {
                sx = lineData.dataPrevSharp.index * gridScaleW;
                sy = lineData.dataPrevSharp.KValue * gridScaleH;
                sx = StandToCanvas(sx, true, upPanel);
                sy = StandToCanvas(sy, false, upPanel);
                float k = (by - sy) / (bx - sx);
                float fyl = sy - sx * k;
                float fyr = sy + (winW - sx) * k;
                g.DrawLine(grayDotLinePen, 0, sy, winW, sy);
                g.DrawLine(prevPen, 0, fyl, winW, fyr);
                g.DrawRectangle(prevPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
            }
            if (lineData.dataNextSharp != null)
            {
                sx = lineData.dataNextSharp.index * gridScaleW;
                sy = lineData.dataNextSharp.KValue * gridScaleH;
                sx = StandToCanvas(sx, true, upPanel);
                sy = StandToCanvas(sy, false, upPanel);
                float k = (by - sy) / (bx - sx);
                float fyl = sy - sx * k;
                float fyr = sy + (winW - sx) * k;
                g.DrawLine(grayDotLinePen, 0, sy, winW, sy);
                g.DrawLine(nextPen, 0, fyl, winW, fyr);
                g.DrawRectangle(nextPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
            }
        }

        void DrawPreviewAuxLine(Graphics g, int winW, int winH, Point mouseRelPos, List<Point> auxPoints)
        {
            switch (auxOperationIndex)
            {
                case AuxLineType.eSingleLine:
                    {
                        float sx, sy, ex, ey;
                        sx = auxPoints[0].X;
                        sy = auxPoints[0].Y;
                        if (sx == mouseRelPos.X)
                        {
                            g.DrawLine(AuxiliaryLineSupportPressure.sOriSolidPen, sx, 0, sx, winH);
                        }
                        else
                        {
                            ex = mouseRelPos.X;
                            ey = mouseRelPos.Y;
                            float k = (ey - sy) / (ex - sx);
                            float fyl = sy - sx * k;
                            float fyr = sy + (winW - sx) * k;
                            g.DrawLine(AuxiliaryLineSupportPressure.sOriSolidPen, 0, fyl, winW, fyr);
                        }
                        g.DrawRectangle(AuxiliaryLineSupportPressure.sOriSolidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eChannelLine:
                    {
                        float sx, sy, ex, ey;
                        sx = auxPoints[0].X;
                        sy = auxPoints[0].Y;
                        g.DrawRectangle(AuxiliaryLineChannel.sOriSolidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        if (auxPoints.Count == 1)
                        {
                            ex = mouseRelPos.X;
                            ey = mouseRelPos.Y;
                            if (sx == ex)
                            {
                                g.DrawLine(AuxiliaryLineChannel.sOriSolidPen, sx, 0, sx, winH);
                            }
                            else
                            {
                                float k = (ey - sy) / (ex - sx);
                                float fyl = sy - sx * k;
                                float fyr = sy + (winW - sx) * k;
                                g.DrawLine(AuxiliaryLineChannel.sOriSolidPen, 0, fyl, winW, fyr);
                            }
                        }
                        else if (auxPoints.Count == 2)
                        {
                            ex = auxPoints[1].X;
                            ey = auxPoints[1].Y;
                            g.DrawRectangle(AuxiliaryLineChannel.sOriSolidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                            if (sx == ex)
                            {
                                g.DrawLine(AuxiliaryLineChannel.sOriSolidPen, sx, 0, sx, winH);
                            }
                            else
                            {
                                float k = (ey - sy) / (ex - sx);
                                float fyl = sy - sx * k;
                                float fyr = sy + (winW - sx) * k;
                                g.DrawLine(AuxiliaryLineChannel.sOriSolidPen, 0, fyl, winW, fyr);
                            }
                            float mx = mouseRelPos.X;
                            float my = mouseRelPos.Y;
                            if (sx == ex)
                            {
                                g.DrawLine(AuxiliaryLineChannel.sOriSolidPen, mx, 0, mx, winH);
                            }
                            else
                            {
                                float k = (ey - sy) / (ex - sx);
                                float oyl = my - mx * k;
                                float oyr = my + (winW - mx) * k;
                                g.DrawLine(AuxiliaryLineChannel.sOriSolidPen, 0, oyl, winW, oyr);
                            }
                            g.DrawLine(AuxiliaryLineChannel.sOriDotPen, sx, sy, mx, my);
                        }
                    }
                    break;
                case AuxLineType.eGoldSegmentedLine:
                    {
                        float sx = auxPoints[0].X;
                        float sy = auxPoints[0].Y;
                        float ex = mouseRelPos.X;
                        float ey = mouseRelPos.Y;
                        for (int i = 0; i < C_GOLDEN_VALUE.Length; ++i)
                        {
                            float v = C_GOLDEN_VALUE[i];
                            float yv = (ey - sy) * v + sy;
                            g.DrawLine(AuxiliaryLineGoldenSection.sOriDotPen, 0, yv, winW, yv); g.DrawString(v.ToString(), auxFont, greenBrush, 0, yv);
                        }
                        g.DrawLine(AuxiliaryLineGoldenSection.sOriSolidPen, 0, sy, winW, sy); g.DrawString("0", auxFont, greenBrush, 0, sy);
                        g.DrawLine(AuxiliaryLineGoldenSection.sOriSolidPen, 0, ey, winW, ey); g.DrawString("1", auxFont, greenBrush, 0, ey);
                        g.DrawLine(AuxiliaryLineGoldenSection.sOriDotPen, sx, sy, ex, ey);
                        g.DrawRectangle(AuxiliaryLineGoldenSection.sOriSolidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eCircleLine:
                    {
                        float cx = auxPoints[0].X;
                        float cy = auxPoints[0].Y;
                        float ex = mouseRelPos.X;
                        float ey = mouseRelPos.Y;
                        float dx = cx - ex;
                        float dy = cy - ey;
                        float radius = (float)Math.Sqrt(dx * dx + dy * dy);
                        float size = 2 * radius;
                        g.DrawEllipse(AuxiliaryLineCircle.sOriDotPen, cx - radius, cy - radius, size, size);
                        g.DrawRectangle(AuxiliaryLineCircle.sOriSolidPen, cx - rcHalfSize, cy - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eArrowLine:
                    {
                        float cx = auxPoints[0].X;
                        float cy = auxPoints[0].Y;
                        float ex = mouseRelPos.X;
                        float ey = mouseRelPos.Y;
                        AuxiliaryLineArrow.sOriSolidPen.Width = AuxiliaryLineArrow.C_LINE_WIDTH;
                        g.DrawLine(AuxiliaryLineArrow.sOriSolidPen, cx, cy, ex, ey);
                        AuxiliaryLineArrow.sOriSolidPen.Width = 1;
                        //g.DrawRectangle(ArrowLine.sOriSolidPen, cx - rcHalfSize, cy - rcHalfSize, rcSize, rcSize);
                        //g.DrawRectangle(ArrowLine.sOriSolidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eRectLine:
                    {
                        float cx = auxPoints[0].X;
                        float cy = auxPoints[0].Y;
                        float ex = mouseRelPos.X;
                        float ey = mouseRelPos.Y;
                        AuxiliaryLineArrow.sOriSolidPen.Width = AuxiliaryLineArrow.C_LINE_WIDTH;
                        g.DrawLine(AuxiliaryLineArrow.sOriSolidPen, cx, cy, ex, ey);
                        AuxiliaryLineArrow.sOriSolidPen.Width = 1;

                        float sx = Math.Min(cx, ex);
                        float sy = Math.Min(cy, ey);
                        float bx = Math.Max(cx, ex);
                        float by = Math.Max(cy, ey);
                        g.DrawRectangle(AuxiliaryLineArrow.sOriSolidPen, sx, sy, bx - sx, by - sy);
                    }
                    break;
            }
        }

        static readonly float[] C_GOLDEN_VALUE = new float[] { 0.382f, 0.618f, 1.382f, 1.618f, 2f, 2.382f, 2.618f, 4, 4.382f, 4.618f, 8, };

        void DrawAuxLine(Graphics g, int winW, int winH, AuxiliaryLineBase line, bool upPanel)//, List<Point> pts, AuxLineType lineType)
        {
            List<Point> pts = line.keyPoints;
            Pen solidPen = line.GetSolidPen();
            Pen dotPen = line.GetDotPen();

            switch (line.lineType)
            {
                case AuxLineType.eHorzLine:
                    {
                        float x = StandToCanvas(pts[0].X, true, upPanel);
                        float y = StandToCanvas(pts[0].Y, false, upPanel);
                        g.DrawLine(solidPen, 0, y, winW, y);
                        g.DrawRectangle(solidPen, x - rcHalfSize, y - rcHalfSize, rcSize, rcSize);
                    }
                    break;

                case AuxLineType.eVertLine:
                    {
                        float x = StandToCanvas(pts[0].X, true, upPanel);
                        float y = StandToCanvas(pts[0].Y, false, upPanel);
                        g.DrawLine(solidPen, x, 0, x, winH);
                        g.DrawRectangle(solidPen, x - rcHalfSize, y - rcHalfSize, rcSize, rcSize);
                    }
                    break;

                case AuxLineType.eSingleLine:
                    {
                        float sx, sy, ex, ey, ox, oy;
                        if (pts[0].X == pts[1].X)
                        {
                            sx = StandToCanvas(pts[0].X, true, upPanel);
                            sy = StandToCanvas(pts[0].Y, false, upPanel);
                            ex = sx;
                            ey = StandToCanvas(pts[1].Y, false, upPanel);
                            g.DrawLine(solidPen, sx, 0, sx, winH);
                        }
                        else
                        {
                            sx = StandToCanvas(pts[0].X, true, upPanel);
                            ex = StandToCanvas(pts[1].X, true, upPanel);
                            sy = StandToCanvas(pts[0].Y, false, upPanel);
                            ey = StandToCanvas(pts[1].Y, false, upPanel);
                            float k = (ey - sy) / (ex - sx);
                            float fyl = sy - sx * k;
                            float fyr = sy + (winW - sx) * k;
                            g.DrawLine(solidPen, 0, fyl, winW, fyr);
                        }
                        g.DrawRectangle(solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;

                case AuxLineType.eChannelLine:
                    {
                        float sx, sy, ex, ey, ox, oy;
                        if (pts[0].X == pts[1].X)
                        {
                            sx = StandToCanvas(pts[0].X, true, upPanel);
                            sy = StandToCanvas(pts[0].Y, false, upPanel);
                            ex = sx;
                            ey = StandToCanvas(pts[1].Y, false, upPanel);
                            ox = StandToCanvas(pts[2].X, true, upPanel);
                            oy = StandToCanvas(pts[2].Y, false, upPanel);
                            g.DrawLine(solidPen, sx, 0, sx, winH);
                            g.DrawLine(solidPen, ox, 0, ox, winH);
                        }
                        else
                        {
                            sx = StandToCanvas(pts[0].X, true, upPanel);
                            ex = StandToCanvas(pts[1].X, true, upPanel);
                            ox = StandToCanvas(pts[2].X, true, upPanel);
                            sy = StandToCanvas(pts[0].Y, false, upPanel);
                            ey = StandToCanvas(pts[1].Y, false, upPanel);
                            oy = StandToCanvas(pts[2].Y, false, upPanel);
                            float k = (ey - sy) / (ex - sx);
                            float fyl = sy - sx * k;
                            float fyr = sy + (winW - sx) * k;
                            float oyl = oy - ox * k;
                            float oyr = oy + (winW - ox) * k;
                            g.DrawLine(solidPen, 0, fyl, winW, fyr);
                            g.DrawLine(solidPen, 0, oyl, winW, oyr);
                        }
                        g.DrawLine(dotPen, sx, sy, ox, oy);
                        g.DrawRectangle(solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(solidPen, ox - rcHalfSize, oy - rcHalfSize, rcSize, rcSize);
                    }
                    break;

                case AuxLineType.eGoldSegmentedLine:
                    {
                        float sx = StandToCanvas(pts[0].X, true, upPanel);
                        float sy = StandToCanvas(pts[0].Y, false, upPanel);
                        float ex = StandToCanvas(pts[1].X, true, upPanel);
                        float ey = StandToCanvas(pts[1].Y, false, upPanel);
                        for (int i = 0; i < C_GOLDEN_VALUE.Length; ++i)
                        {
                            float v = C_GOLDEN_VALUE[i];
                            float yv = (ey - sy) * v + sy;
                            g.DrawLine(dotPen, 0, yv, winW, yv); g.DrawString(v.ToString(), auxFont, greenBrush, 0, yv);
                        }
                        g.DrawLine(solidPen, 0, sy, winW, sy); g.DrawString("0", auxFont, greenBrush, 0, sy);
                        g.DrawLine(solidPen, 0, ey, winW, ey); g.DrawString("1", auxFont, greenBrush, 0, ey);
                        g.DrawLine(dotPen, sx, sy, ex, ey);
                        g.DrawRectangle(solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eCircleLine:
                    {
                        AuxiliaryLineCircle cl = line as AuxiliaryLineCircle;
                        float x = StandToCanvas(cl.x, true, upPanel);
                        float y = StandToCanvas(cl.y, false, upPanel);
                        g.DrawEllipse(dotPen, x, y, cl.size, cl.size);

                        float sx = StandToCanvas(pts[0].X, true, upPanel);
                        float sy = StandToCanvas(pts[0].Y, false, upPanel);
                        float ex = StandToCanvas(pts[1].X, true, upPanel);
                        float ey = StandToCanvas(pts[1].Y, false, upPanel);
                        g.DrawLine(solidPen, sx, sy, ex, ey);
                        g.DrawRectangle(solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eArrowLine:
                    {
                        float cx = StandToCanvas(pts[0].X, true, upPanel);
                        float cy = StandToCanvas(pts[0].Y, false, upPanel);
                        float ex = StandToCanvas(pts[1].X, true, upPanel);
                        float ey = StandToCanvas(pts[1].Y, false, upPanel);
                        solidPen.Width = AuxiliaryLineArrow.C_LINE_WIDTH;
                        g.DrawLine(solidPen, cx, cy, ex, ey);
                        //g.DrawRectangle(solidPen, cx - rcHalfSize, cy - rcHalfSize, rcSize, rcSize);
                        //g.DrawRectangle(solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eRectLine:
                    {
                        float cx = StandToCanvas(pts[0].X, true, upPanel);
                        float cy = StandToCanvas(pts[0].Y, false, upPanel);
                        float ex = StandToCanvas(pts[1].X, true, upPanel);
                        float ey = StandToCanvas(pts[1].Y, false, upPanel);
                        solidPen.Width = AuxiliaryLineArrow.C_LINE_WIDTH;
                        g.DrawLine(solidPen, cx, cy, ex, ey);

                        float sx = Math.Min(cx, ex);
                        float sy = Math.Min(cy, ey);
                        float bx = Math.Max(cx, ex);
                        float by = Math.Max(cy, ey);
                        g.DrawRectangle(AuxiliaryLineArrow.sOriSolidPen, sx, sy, bx - sx, by - sy);
                    }
                    break;
            }
        }

        void DrawKRuler(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos, KDataDictContainer kddc, float missRelHeight)
        {
            float gridScaleW = true ? gridScaleUp.X : gridScaleDown.X;
            float gridScaleH = true ? gridScaleUp.Y : gridScaleDown.Y;

            int endIndex = kddc.dataLst.Count - 1;
            if (parent.endShowDataItemIndex != -1 && parent.endShowDataItemIndex < kddc.dataLst.Count)
                endIndex = parent.endShowDataItemIndex;

            if (TradeDataManager.Instance.autoAnalyzeTool != null)
            {
                int startID = endIndex - GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT;
                if (startID < 0)
                    startID = 0;
                float x = StandToCanvas(startID * gridScaleW, true, true);
                g.DrawLine(whiteLinePen, x, 0, x, winH);
            }

            float downRCH = gridScaleH * missRelHeight;
            float strDist = 1.5f * gridScaleW;

            KDataMap lastKDD = kddc.dataLst[endIndex];
            KData data = lastKDD.GetData(cdt, false);
            float standX = (data.index + 1) * gridScaleW;
            float standY = (data.KValue) * gridScaleH;
            standX = StandToCanvas(standX, true, true);
            standY = StandToCanvas(standY, false, true);
            if (standX > winW)
                return;
            bool isUpOK = false;
            bool isDownOK = false;
            int i = 0;

            //for( int i = 0; i < 20; ++i )
            while (isUpOK == false || isDownOK == false)
            {
                float X = standX + i * gridScaleW;
                float upY = standY - (i + 1) * gridScaleH;
                float downY = standY + i * downRCH;
                int stepID = i + 1;

                if (isUpOK == false)
                {
                    g.DrawRectangle(redDotPen, X, upY, gridScaleW, gridScaleH);
                    g.DrawString(stepID.ToString(), rulerFont, redBrush, X + strDist, upY + 5);
                    if (X > winW || upY < 0)
                        isUpOK = true;
                }
                if (isDownOK == false)
                {
                    g.DrawRectangle(cyanDotPen, X, downY, gridScaleW, downRCH);
                    g.DrawString(stepID.ToString(), rulerFont, cyanBrush, X + strDist, downY - 5);
                    if (X > winW || downY > winH)
                        isDownOK = true;
                }
                ++i;
            }
        }
    }
}
