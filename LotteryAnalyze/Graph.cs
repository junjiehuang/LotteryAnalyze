﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
#region Graph 
    
    public enum GraphType
    {
        eNone,
        // K线图
        eKCurveGraph,
        // 柱状图
        eBarGraph,
        // 资金图
        eTradeGraph,
    }

    public enum AuxLineType
    {
        eNone,
        eHorzLine,
        eVertLine,
        eSingleLine,
        eChannelLine,
        eGoldSegmentedLine,
    }

    // 图表基类
    class GraphBase
    {
        public float gridScaleH = 20;
        public float gridScaleW = 5;
        public PointF canvasOffset = new PointF(0, 0);

        public float CanvasToStand(float v, bool isX)
        {
            if (isX)
                return (v + canvasOffset.X);
            else
                return (canvasOffset.Y) - v;
        }
        public float StandToCanvas(float v, bool isX)
        {
            if (isX)
                return v - canvasOffset.X;
            else
                return canvasOffset.Y - v;
        }
        public Point CanvasToStand(Point pt)
        {
            Point res = new Point();
            res.X = (int)CanvasToStand((float)pt.X, true);
            res.Y = (int)CanvasToStand((float)pt.Y, false);
            return res;
        }
        public Point StandToCanvas(Point pt)
        {
            Point res = new Point();
            res.X = (int)StandToCanvas((float)pt.X, true);
            res.Y = (int)StandToCanvas((float)pt.Y, false);
            return res;
        }


        public virtual bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return false;
        }
        public virtual void MoveGraph(float dx, float dy)
        {

        }
        public virtual void ResetGraphPosition()
        {

        }
        public virtual void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {

        }
        public virtual void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos) { }
    }

    // 辅助线基类
    class AuxiliaryLine
    {
        public AuxLineType lineType = AuxLineType.eNone;
        public List<Point> keyPoints = new List<Point>();
        public virtual Pen GetPen() { return null; }
    }
    // 水平线
    class HorzLine : AuxiliaryLine
    {
        public static Color lineColor = Color.Aqua;
        public static Pen solidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, lineColor, 2);

        public HorzLine()
        {
            lineType = AuxLineType.eHorzLine;
        }
        public override Pen GetPen() { return solidPen; }
    }
    // 垂直线
    class VertLine : AuxiliaryLine
    {
        public static Color lineColor = Color.Azure;
        public static Pen solidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, lineColor, 2);

        public VertLine()
        {
            lineType = AuxLineType.eVertLine;
        }
        public override Pen GetPen() { return solidPen; }
    }
    // 通道直线
    class SingleLine : AuxiliaryLine
    {
        public static Color lineColor = Color.Blue;
        public static Pen solidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, lineColor, 2);
        public SingleLine()
        {
            lineType = AuxLineType.eSingleLine;
        }
        public override Pen GetPen() { return solidPen; }
    }

    // 通道线
    class ChannelLine : AuxiliaryLine
    {
        public static Color lineColor = Color.Pink;
        public static Pen solidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, lineColor, 2);
        public static Pen dotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, lineColor, 1);

        public ChannelLine()
        {
            lineType = AuxLineType.eChannelLine;
        }
        public override Pen GetPen() { return solidPen; }
    }
    // 黄金分割线
    class GoldSegmentedLine : AuxiliaryLine
    {
        public static Color lineColor = Color.Yellow;
        public static Pen solidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, lineColor, 2);
        public static Pen dotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, lineColor, 1);

        public GoldSegmentedLine()
        {
            lineType = AuxLineType.eGoldSegmentedLine;
        }
        public override Pen GetPen() { return solidPen; }
    }

    // K线图
    class GraphKCurve : GraphBase
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
        };

        public bool enableAuxiliaryLine = true;
        public bool enableAvgLines = true;
        public bool enableBollinBand = true;
        public bool enableMACD = true;

        public AuxLineType auxOperationIndex = AuxLineType.eNone;


        public bool autoAllign = false;
        
        int selDataIndex = -1;
        float selDataPtX = -1;

        Font selDataFont;
        Font auxFont;
        PointF prevPt = new PointF();
        bool findPrevPt = false;

        float lastValue = 0;
        
        
        Pen bollinLinePenUp = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);
        Pen bollinLinePenMid = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);
        Pen bollinLinePenDown = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);

        Pen grayDotLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);
        Pen redLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 1);
        Pen cyanLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Cyan, 1);
        Pen whiteLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);
        Pen yellowLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Yellow, 1);

        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush cyanBrush = new SolidBrush(Color.Cyan);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush greenBrush = new SolidBrush(Color.Green);
        SolidBrush tmpBrush;

        Dictionary<Pen, List<PointF>> linePools = new Dictionary<Pen, List<PointF>>();
        Dictionary<SolidBrush, List<RectangleF>> rcPools = new Dictionary<SolidBrush,List<RectangleF>>();
        List<AuxiliaryLine> auxiliaryLineList = new List<AuxiliaryLine>();
        public List<Point> mouseHitPts = new List<Point>();
        public AuxiliaryLine selAuxLine = null;
        public int selAuxLinePointIndex = -1;

        public GraphKCurve()
        {
            selDataFont = new Font(FontFamily.GenericSerif, 14);
            auxFont = new Font(FontFamily.GenericMonospace, 12);
        }

        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            if (GraphDataManager.Instance.HasData(GraphType.eKCurveGraph) == false)
                return false;
            int curIndex = (int)((mousePos.X + canvasOffset.X) / gridScaleW);
            if (curIndex == selDataIndex)
                return false;
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
            canvasOffset.X = 0;
            canvasOffset.Y = 0;
        }
        public override void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            BeforeDraw();

            selDataIndex = -1;
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
                    int startIndex = (int)(canvasOffset.X / gridScaleW) - 1;
                    if (startIndex < 0)
                        startIndex = 1;
                    int endIndex = (int)((canvasOffset.X + winW) / gridScaleW) + 1;
                    if (endIndex > kddc.dataLst.Count)
                        endIndex = kddc.dataLst.Count;

                    // 自动对齐
                    if (autoAllign)
                    {
                        float endY = kddc.dataLst[endIndex - 1].dataDict[cdt].KValue * gridScaleH;
                        float relEY = StandToCanvas(endY, false);
                        bool isEYOut = relEY < 0 || relEY > winH;
                        if (isEYOut)
                            canvasOffset.Y = endY + winH * 0.5f;
                        autoAllign = false;
                    }

                    // 画K线图
                    for (int i = startIndex; i < endIndex; ++i)
                    {
                        KDataDict kdDict = kddc.dataLst[i];
                        KData data = kdDict.GetData(cdt, false);
                        if (data == null)
                            continue;
                        DrawKDataGraph(g, data, winW, winH, missRelHeight, mouseRelPos);
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

                                startIndex = (int)(canvasOffset.X / gridScaleW) - 1;
                                if (startIndex < 0)
                                    startIndex = 1;
                                endIndex = (int)((canvasOffset.X + winW) / gridScaleW) + 1;
                                if (endIndex > adc.avgPointMapLst.Count)
                                    endIndex = adc.avgPointMapLst.Count;
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
                        for (int i = 0; i < kddc.bollinDataLst.bollinMapLst.Count; ++i)
                        {
                            DrawBollinLineGraph(g, kddc.bollinDataLst.bollinMapLst[i], winW, winH, cdt);
                        }
                    }
                }

                // 画辅助线
                if(enableAuxiliaryLine)
                {
                    DrawAuxLineGraph(g, winW, winH, mouseRelPos);
                }

                g.DrawLine(grayDotLinePen, 0, mouseRelPos.Y, winW, mouseRelPos.Y);
            }
            EndDraw(g);
        }
        public override void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            BeforeDraw();
            if(enableMACD)
            {
                KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
                int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];

                float oriYOff = canvasOffset.Y;
                canvasOffset.Y = winH * 0.5f;
                lastValue = 0;
                findPrevPt = false;
                int startIndex = (int)(canvasOffset.X / gridScaleW) - 1;
                if (startIndex < 0)
                    startIndex = 1;
                int endIndex = (int)((canvasOffset.X + winW) / gridScaleW) + 1;
                if (endIndex > kddc.macdDataLst.macdMapLst.Count)
                    endIndex = kddc.macdDataLst.macdMapLst.Count;
                for ( int i = startIndex; i < endIndex; ++i )
                {
                    DrawMACDGraph(g, kddc.macdDataLst.macdMapLst[i], winW, winH, cdt);
                }
                canvasOffset.Y = oriYOff;

                if (selDataIndex != -1)
                {
                    g.DrawLine(grayDotLinePen, selDataPtX, 0, selDataPtX, winH);
                    g.DrawLine(grayDotLinePen, selDataPtX + gridScaleW, 0, selDataPtX + gridScaleW, winH);
                }
            }
            EndDraw(g);
        }

        public void UpdateSelectAuxLinePoint(Point mouseRelPos)
        {
            if(selAuxLine != null && selAuxLinePointIndex >= 0 && selAuxLinePointIndex < selAuxLine.keyPoints.Count)
            {
                selAuxLine.keyPoints[selAuxLinePointIndex] = CanvasToStand(mouseRelPos);
            }
        }
        public void SelectAuxLine(Point mouseRelPos)
        {
            selAuxLine = null;
            selAuxLinePointIndex = -1;
            Point standMousePos = CanvasToStand(mouseRelPos);
            for( int i = 0; i < auxiliaryLineList.Count; ++i )
            {
                AuxiliaryLine al = auxiliaryLineList[i];
                for(int j = 0; j < al.keyPoints.Count; ++j)
                {
                    Point pt = al.keyPoints[j];
                    if (pt.X - rcHalfSize > standMousePos.X ||
                        pt.X + rcHalfSize < standMousePos.X ||
                        pt.Y - rcHalfSize > standMousePos.Y ||
                        pt.Y + rcHalfSize < standMousePos.Y)
                        continue;
                    else
                    {
                        selAuxLine = al;
                        selAuxLinePointIndex = j;
                        return;
                    }
                }
            }
        }
        public void RemoveAllAuxLines()
        {
            auxiliaryLineList.Clear();
        }
        public void RemoveSelectAuxLine()
        {
            if(selAuxLine!=null)
            {
                auxiliaryLineList.Remove(selAuxLine);
                selAuxLine = null;
                selAuxLinePointIndex = -1;
            }
        }
        public void AddHorzLine( Point pt )
        {
            HorzLine line = new HorzLine();
            line.keyPoints.Add( CanvasToStand(pt) );
            auxiliaryLineList.Add(line);
        }
        public void AddVertLine(Point pt)
        {
            VertLine line = new VertLine();
            line.keyPoints.Add(CanvasToStand(pt));
            auxiliaryLineList.Add(line);
        }
        public void AddSingleLine(Point p1, Point p2)
        {
            SingleLine line = new SingleLine();
            line.keyPoints.Add(CanvasToStand(p1));
            line.keyPoints.Add(CanvasToStand(p2));
            auxiliaryLineList.Add(line);
        }
        public void AddChannelLine(Point line0P1, Point line0P2, Point line1P)
        {
            ChannelLine line = new ChannelLine();
            line.keyPoints.Add(CanvasToStand(line0P1));
            line.keyPoints.Add(CanvasToStand(line0P2));
            line.keyPoints.Add(CanvasToStand(line1P));
            auxiliaryLineList.Add(line);
        }
        public void AddGoldSegLine(Point p1, Point P2)
        {
            GoldSegmentedLine line = new GoldSegmentedLine();
            line.keyPoints.Add(CanvasToStand(p1));
            line.keyPoints.Add(CanvasToStand(P2));
            auxiliaryLineList.Add(line);
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


        void DrawKDataGraph(Graphics g, KData data, int winW, int winH, float missRelHeight, Point mouseRelPos)
        {
            KData prevData = data.GetPrevKData();
            if (prevData != null)
                lastValue = prevData.KValue;
            else
                lastValue = 0;

            float standX = data.index * gridScaleW;
            float valudChange = data.HitValue - data.MissValue * missRelHeight;
            float standY = lastValue * gridScaleH;
            float up = standY + data.HitValue * gridScaleH;
            float dowm = standY - data.MissValue * missRelHeight * gridScaleH;
            float rcY = standY;
            float rcH = Math.Abs(valudChange * gridScaleH);
            if (rcH < 1)
                rcH = 1;
            if (valudChange > 0)
                rcY += valudChange * gridScaleH;
            lastValue += valudChange;

            standX = StandToCanvas(standX, true);
            if (standX < 0 || standX > winW)
                return;
            standY = StandToCanvas(standY, false);
            up = StandToCanvas(up, false);
            dowm = StandToCanvas(dowm, false);
            rcY = StandToCanvas(rcY, false);
            tmpBrush = valudChange > 0 ? redBrush : (valudChange < 0 ? cyanBrush : whiteBrush);
            Pen linePen = valudChange > 0 ? redLinePen : (valudChange < 0 ? cyanLinePen : whiteLinePen);
            float midX = standX + gridScaleW * 0.5f;
            g.DrawLine(linePen, midX, up, midX, dowm);
            //g.FillRectangle(tmpBrush, standX, rcY, gridScaleW, rcH);
            //PushLinePts(linePen, midX, up, midX, dowm);
            PushRcPts(tmpBrush, standX, rcY, gridScaleW, rcH);

            if (selDataIndex < 0 && standX <= mouseRelPos.X && standX + gridScaleW >= mouseRelPos.X)
            {
                selDataPtX = standX;
                selDataIndex = data.index;
                g.DrawLine(grayDotLinePen, standX, 0, standX, winH);
                g.DrawLine(grayDotLinePen, standX + gridScaleW, 0, standX + gridScaleW, winH);
                //PushLinePts(grayDotLinePen, standX, 0, standX, winH);
                //PushLinePts(grayDotLinePen, standX + gridScaleW, 0, standX + gridScaleW, winH);
                g.DrawString(data.GetInfo(), selDataFont, whiteBrush, 0, 0);
            }
        }
        void DrawAvgLineGraph(Graphics g, AvgPointMap apm, int winW, int winH, CollectDataType cdt, Pen pen)
        {
            AvgPoint ap = apm.apMap[cdt];
            float standX = (apm.index + 0.5f) * gridScaleW;  
            float standY = ap.avgKValue * gridScaleH;
            if (findPrevPt == false)
            {
                findPrevPt = true;
                prevPt.X = standX;
                prevPt.Y = standY;
                return;
            }
            float cx = StandToCanvas(standX, true);
            if (cx < 0 || cx > winW)
            {
                prevPt.X = standX;
                prevPt.Y = standY;
                return;
            }
            float px = StandToCanvas(prevPt.X, true);
            float py = StandToCanvas(prevPt.Y, false);
            float cy = StandToCanvas(standY, false);
            prevPt.X = standX;
            prevPt.Y = standY;
            PushLinePts(pen, px, py, cx, cy);
        }
        void DrawBollinLineGraph(Graphics g, BollinPointMap bpm, int winW, int winH, CollectDataType cdt)
        {
            BollinPoint bp = bpm.bpMap[cdt];
            float standX = (bpm.index + 0.5f) * gridScaleW;
            if (findPrevPt == false)
            {
                findPrevPt = true;
                return;
            }
            float cx = StandToCanvas(standX, true);
            if (cx < 0 || cx > winW)
            {
                return;
            }
            BollinPointMap prevBPM = bpm.GetPrevBPM();
            BollinPoint prevBP = prevBPM.bpMap[cdt];
            float px = StandToCanvas((prevBPM.index + 0.5f) * gridScaleW, true);
            float pyU = StandToCanvas(prevBP.upValue * gridScaleH, false);
            float pyM = StandToCanvas(prevBP.midValue * gridScaleH, false);
            float pyD = StandToCanvas(prevBP.downValue * gridScaleH, false);
            float cyU = StandToCanvas(bp.upValue * gridScaleH, false);
            float cyM = StandToCanvas(bp.midValue * gridScaleH, false);
            float cyD = StandToCanvas(bp.downValue * gridScaleH, false);
            PushLinePts(bollinLinePenUp, px, pyU, cx, cyU);
            PushLinePts(bollinLinePenMid, px, pyM, cx, cyM);
            PushLinePts(bollinLinePenDown, px, pyD, cx, cyD);
        }
        void DrawMACDGraph(Graphics g, MACDPointMap mpm, int winW, int winH, CollectDataType cdt)
        {
            MACDPoint mp = mpm.macdpMap[cdt];
            float standX = (mpm.index + 0.5f) * gridScaleW;            
            if (findPrevPt == false)
            {
                findPrevPt = true;
                return;
            }
            float cx = StandToCanvas(standX, true);
            if (cx < 0 || cx > winW)
            {
                return;
            }
            MACDLimitValue mlv = mpm.parent.macdLimitValueMap[cdt];
            float gridScaleH = winH * 0.45f / Math.Max(Math.Abs(mlv.MaxValue), Math.Abs(mlv.MinValue));
            MACDPointMap prevMPM = mpm.GetPrevMACDPM();
            MACDPoint prevMP = prevMPM.macdpMap[cdt];
            float standY = mp.BAR * gridScaleH;
            float px = StandToCanvas((prevMPM.index + 0.5f) * gridScaleW, true);
            float pyDIF = StandToCanvas((prevMP.DIF * gridScaleH), false);
            float pyDEA = StandToCanvas((prevMP.DEA * gridScaleH), false);
            float cyDIF = StandToCanvas((mp.DIF * gridScaleH), false);
            float cyDEA = StandToCanvas((mp.DEA * gridScaleH), false);
            float cyBAR = StandToCanvas(standY, false);
            float rcY = StandToCanvas(standY > 0 ? standY : 0, false);

            PushLinePts(yellowLinePen, px, pyDIF, cx, cyDIF);
            PushLinePts(whiteLinePen, px, pyDEA, cx, cyDEA);
            PushRcPts(mp.BAR > 0 ? redBrush : cyanBrush, px - gridScaleW * 0.5f, rcY, gridScaleW, Math.Abs(standY));
        }

        void DrawAuxLineGraph(Graphics g, int winW, int winH, Point mouseRelPos)
        {
            float rcHalfSize = 3;
            float rcSize = rcHalfSize * 2;
            for ( int i = 0; i < auxiliaryLineList.Count; ++i )
            {
                AuxiliaryLine al = auxiliaryLineList[i];
                DrawAuxLine(g, winW, winH, al.keyPoints, al.lineType);
            }

            if(mouseHitPts.Count > 0 && auxOperationIndex > AuxLineType.eNone)
            {
                DrawPreviewAuxLine(g, winW, winH, mouseRelPos);
            }

            if(selAuxLine!=null && selAuxLinePointIndex != -1)
            {
                Point pt = StandToCanvas(selAuxLine.keyPoints[selAuxLinePointIndex]);
                g.DrawRectangle(selAuxLine.GetPen(), pt.X - rcHalfSize - 4, pt.Y - rcHalfSize - 4, rcSize + 8, rcSize + 8);
            }
        }

        void DrawPreviewAuxLine(Graphics g, int winW, int winH, Point mouseRelPos)
        {
            switch(auxOperationIndex)
            {
                case AuxLineType.eSingleLine:
                    {
                        float sx, sy, ex, ey;
                        sx = mouseHitPts[0].X;
                        sy = mouseHitPts[0].Y;
                        if (sx == mouseRelPos.X)
                        {
                            g.DrawLine(SingleLine.solidPen, sx, 0, sx, winH);
                        }
                        else
                        {
                            ex = mouseRelPos.X;
                            ey = mouseRelPos.Y;
                            float k = (ey - sy) / (ex - sx);
                            float fyl = sy - sx * k;
                            float fyr = sy + (winW - sx) * k;
                            g.DrawLine(SingleLine.solidPen, 0, fyl, winW, fyr);
                        }
                        g.DrawRectangle(SingleLine.solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eChannelLine:
                    {
                        float sx, sy, ex, ey;
                        sx = mouseHitPts[0].X;
                        sy = mouseHitPts[0].Y;
                        g.DrawRectangle(ChannelLine.solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        if (mouseHitPts.Count == 1)
                        {
                            ex = mouseRelPos.X;
                            ey = mouseRelPos.Y;
                            if (sx == ex)
                            {
                                g.DrawLine(ChannelLine.solidPen, sx, 0, sx, winH);
                            }
                            else
                            {
                                float k = (ey - sy) / (ex - sx);
                                float fyl = sy - sx * k;
                                float fyr = sy + (winW - sx) * k;
                                g.DrawLine(ChannelLine.solidPen, 0, fyl, winW, fyr);
                            }
                        }
                        else if(mouseHitPts.Count == 2)
                        {
                            ex = mouseHitPts[1].X;
                            ey = mouseHitPts[1].Y;
                            g.DrawRectangle(ChannelLine.solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                            if (sx == ex)
                            {
                                g.DrawLine(ChannelLine.solidPen, sx, 0, sx, winH);
                            }
                            else
                            {
                                float k = (ey - sy) / (ex - sx);
                                float fyl = sy - sx * k;
                                float fyr = sy + (winW - sx) * k;
                                g.DrawLine(ChannelLine.solidPen, 0, fyl, winW, fyr);
                            }
                            float mx = mouseRelPos.X;
                            float my = mouseRelPos.Y;
                            if (sx == ex)
                            {
                                g.DrawLine(ChannelLine.solidPen, mx, 0, mx, winH);
                            }
                            else
                            {
                                float k = (ey - sy) / (ex - sx);
                                float oyl = my - mx * k;
                                float oyr = my + (winW - mx) * k;
                                g.DrawLine(ChannelLine.solidPen, 0, oyl, winW, oyr);
                            }
                            g.DrawLine(ChannelLine.dotPen, sx, sy, mx, my);
                        }
                    }
                    break;
                case AuxLineType.eGoldSegmentedLine:
                    {
                        float sx = mouseHitPts[0].X;
                        float sy = mouseHitPts[0].Y;
                        float ex = mouseRelPos.X;
                        float ey = mouseRelPos.Y;
                        float y382 = (ey - sy) * 0.382f + sy;
                        float y618 = (ey - sy) * 0.618f + sy;
                        float y1382 = (ey - sy) * 1.382f + sy;
                        float y1618 = (ey - sy) * 1.618f + sy;
                        float y2 = (ey - sy) * 2.0f + sy;
                        float y2618 = (ey - sy) * 2.618f + sy;
                        g.DrawLine(GoldSegmentedLine.solidPen, 0, sy, winW, sy); g.DrawString("0", auxFont, greenBrush, 0, sy);
                        g.DrawLine(GoldSegmentedLine.solidPen, 0, ey, winW, ey); g.DrawString("1", auxFont, greenBrush, 0, ey);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y382, winW, y382); g.DrawString("0.382", auxFont, greenBrush, 0, y382);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y618, winW, y618); g.DrawString("0.618", auxFont, greenBrush, 0, y618);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y1382, winW, y1382); g.DrawString("1.382", auxFont, greenBrush, 0, y1382);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y1618, winW, y1618); g.DrawString("1.618", auxFont, greenBrush, 0, y1618);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y2, winW, y2); g.DrawString("2", auxFont, greenBrush, 0, y2);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y2618, winW, y2618); g.DrawString("2.618", auxFont, greenBrush, 0, y2618);
                        g.DrawLine(GoldSegmentedLine.dotPen, sx, sy, ex, ey);
                        g.DrawRectangle(GoldSegmentedLine.solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                    }
                    break;
            }
        }

        void DrawAuxLine(Graphics g, int winW, int winH, List<Point> pts, AuxLineType lineType)
        {
            switch (lineType)
            {
                case AuxLineType.eHorzLine:
                    {
                        float x = StandToCanvas(pts[0].X, true);
                        float y = StandToCanvas(pts[0].Y, false);
                        g.DrawLine(HorzLine.solidPen, 0, y, winW, y);
                        g.DrawRectangle(HorzLine.solidPen, x - rcHalfSize, y - rcHalfSize, rcSize, rcSize);
                    }
                    break;

                case AuxLineType.eVertLine:
                    {
                        float x = StandToCanvas(pts[0].X, true);
                        float y = StandToCanvas(pts[0].Y, false);
                        g.DrawLine(VertLine.solidPen, x, 0, x, winH);
                        g.DrawRectangle(VertLine.solidPen, x - rcHalfSize, y - rcHalfSize, rcSize, rcSize);
                    }
                    break;

                case AuxLineType.eSingleLine:
                    {
                        float sx, sy, ex, ey, ox, oy;
                        if (pts[0].X == pts[1].X)
                        {
                            sx = StandToCanvas(pts[0].X, true);
                            sy = StandToCanvas(pts[0].Y, false);
                            ex = sx;
                            ey = StandToCanvas(pts[1].Y, false);
                            g.DrawLine(SingleLine.solidPen, sx, 0, sx, winH);
                        }
                        else
                        {
                            sx = StandToCanvas(pts[0].X, true);
                            ex = StandToCanvas(pts[1].X, true);
                            sy = StandToCanvas(pts[0].Y, false);
                            ey = StandToCanvas(pts[1].Y, false);
                            float k = (ey - sy) / (ex - sx);
                            float fyl = sy - sx * k;
                            float fyr = sy + (winW - sx) * k;
                            g.DrawLine(SingleLine.solidPen, 0, fyl, winW, fyr);
                        }
                        g.DrawRectangle(SingleLine.solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(SingleLine.solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;

                case AuxLineType.eChannelLine:
                    {
                        float sx, sy, ex, ey, ox, oy;
                        if (pts[0].X == pts[1].X)
                        {
                            sx = StandToCanvas(pts[0].X, true);
                            sy = StandToCanvas(pts[0].Y, false);
                            ex = sx;
                            ey = StandToCanvas(pts[1].Y, false);
                            ox = StandToCanvas(pts[2].X, true);
                            oy = StandToCanvas(pts[2].Y, false);
                            g.DrawLine(ChannelLine.solidPen, sx, 0, sx, winH);
                            g.DrawLine(ChannelLine.solidPen, ox, 0, ox, winH);
                        }
                        else
                        {
                            sx = StandToCanvas(pts[0].X, true);
                            ex = StandToCanvas(pts[1].X, true);
                            ox = StandToCanvas(pts[2].X, true);
                            sy = StandToCanvas(pts[0].Y, false);
                            ey = StandToCanvas(pts[1].Y, false);
                            oy = StandToCanvas(pts[2].Y, false);
                            float k = (ey - sy) / (ex - sx);
                            float fyl = sy - sx * k;
                            float fyr = sy + (winW - sx) * k;
                            float oyl = oy - ox * k;
                            float oyr = oy + (winW - ox) * k;
                            g.DrawLine(ChannelLine.solidPen, 0, fyl, winW, fyr);
                            g.DrawLine(ChannelLine.solidPen, 0, oyl, winW, oyr);
                        }
                        g.DrawLine(ChannelLine.dotPen, sx, sy, ox, oy);
                        g.DrawRectangle(ChannelLine.solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(ChannelLine.solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(ChannelLine.solidPen, ox - rcHalfSize, oy - rcHalfSize, rcSize, rcSize);
                    }
                    break;

                case AuxLineType.eGoldSegmentedLine:
                    {
                        float sx = StandToCanvas(pts[0].X, true);
                        float sy = StandToCanvas(pts[0].Y, false);
                        float ex = StandToCanvas(pts[1].X, true);
                        float ey = StandToCanvas(pts[1].Y, false);
                        float y382 = (ey - sy) * 0.382f + sy;
                        float y618 = (ey - sy) * 0.618f + sy;
                        float y1382 = (ey - sy) * 1.382f + sy;
                        float y1618 = (ey - sy) * 1.618f + sy;
                        float y2 = (ey - sy) * 2.0f + sy;
                        float y2618 = (ey - sy) * 2.618f + sy;
                        g.DrawLine(GoldSegmentedLine.solidPen, 0, sy, winW, sy); g.DrawString("0", auxFont, greenBrush, 0, sy);
                        g.DrawLine(GoldSegmentedLine.solidPen, 0, ey, winW, ey); g.DrawString("1", auxFont, greenBrush, 0, ey);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y382, winW, y382); g.DrawString("0.382", auxFont, greenBrush, 0, y382);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y618, winW, y618); g.DrawString("0.618", auxFont, greenBrush, 0, y618);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y1382, winW, y1382); g.DrawString("1.382", auxFont, greenBrush, 0, y1382);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y1618, winW, y1618); g.DrawString("1.618", auxFont, greenBrush, 0, y1618);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y2, winW, y2); g.DrawString("2", auxFont, greenBrush, 0, y2);
                        g.DrawLine(GoldSegmentedLine.dotPen, 0, y2618, winW, y2618); g.DrawString("2.618", auxFont, greenBrush, 0, y2618);
                        g.DrawLine(GoldSegmentedLine.dotPen, sx, sy, ex, ey);
                        g.DrawRectangle(GoldSegmentedLine.solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(GoldSegmentedLine.solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;
            }
        }
    }

    // 柱状图
    class GraphBar : GraphBase
    {
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush tagBrush = new SolidBrush(Color.White);
        SolidBrush greenBrush = new SolidBrush(Color.Green);


        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return true;
        }
        public override void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            BarGraphDataContianer bgdc = GraphDataManager.BGDC;
            if (bgdc.HasData() == false || bgdc.totalCollectCount == 0)
                return;

            float startX = 0;
            float startY = winH * 0.1f;
            float MaxRcH = winH * 0.8f;
            float bottom = winH * 0.9f;

            BarGraphDataContianer.DataUnitLst dul = bgdc.allDatas[numIndex];
            float gap = (float)winW / dul.dataLst.Count;

            Font tagFont = new Font(FontFamily.GenericSerif, 16);
            for (int i = 0; i < dul.dataLst.Count; ++i)
            {
                Brush brush = greenBrush;
                float rate = ((float)(dul.dataLst[i].data) / (float)bgdc.totalCollectCount);
                if(dul.dataLst[i].type == BarGraphDataContianer.StatisticsType.eAppearCountPath012)
                {
                    if (i == 0 && rate > 0.4f)
                        brush = redBrush;
                    else if (i > 0 && rate > 0.3f)
                        brush = redBrush;
                }
                else if(dul.dataLst[i].type == BarGraphDataContianer.StatisticsType.eAppearCountFrom0To9)
                {
                    if(rate > 0.1f)
                        brush = redBrush;
                }

                float rcH = MaxRcH * rate;
                startY = bottom - rcH;
                g.FillRectangle(brush, startX, startY, gap * 0.9f, rcH);
                g.DrawString(dul.dataLst[i].tag, tagFont, tagBrush, startX, bottom);
                g.DrawString(dul.dataLst[i].data.ToString(), tagFont, tagBrush, startX, startY - 30);
                startX += gap;
            }
        }
    }

    class GraphTrade : GraphBase
    {

        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return true;
        }
        public override void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            TradeDataManager tdm = TradeDataManager.Instance;

            for(int i = 0; i < tdm.historyTradeDatas.Count; ++i)
            {

            }
        }

    }

    // 图表管理器
    class GraphManager
    {
        Dictionary<GraphType, GraphBase> sGraphMap = new Dictionary<GraphType, GraphBase>();
        GraphBase curGraph = null;
        GraphType curGraphType = GraphType.eNone;
        public GraphBar barGraph;
        public GraphKCurve kvalueGraph;
        public GraphTrade tradeGraph;


        public GraphManager()
        {
            barGraph = new GraphBar();
            kvalueGraph = new GraphKCurve();
            tradeGraph = new GraphTrade();
            sGraphMap.Add(GraphType.eKCurveGraph, kvalueGraph);
            sGraphMap.Add(GraphType.eBarGraph, barGraph);
            sGraphMap.Add(GraphType.eTradeGraph, tradeGraph);
        }

        public GraphType CurrentGraphType
        {
            get { return curGraphType; }
        }
        public void SetCurrentGraph(GraphType gt)
        {
            curGraphType = gt;
            if (sGraphMap.ContainsKey(gt))
                curGraph = sGraphMap[gt];
            else
                curGraph = null;
        }
        public bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            if (curGraph != null)
                return curGraph.NeedRefreshCanvasOnMouseMove(mousePos);
            return false;
        }
        public void MoveGraph(float dx, float dy)
        {
            if (curGraph != null)
                curGraph.MoveGraph(dx, dy);
        }
        public void ResetGraphPosition()
        {
            if (curGraph != null)
                curGraph.ResetGraphPosition();
        }
        public void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            if (curGraph != null)
                curGraph.DrawUpGraph(g, numIndex, cdt, winW, winH, mouseRelPos);
        }
        public void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            if (curGraph != null)
                curGraph.DrawDownGraph(g, numIndex, cdt, winW, winH, mouseRelPos);
        }
    }

#endregion
}