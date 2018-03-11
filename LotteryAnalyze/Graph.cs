using System;
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
        // 水平线
        eHorzLine,
        // 垂直线
        eVertLine,
        // 任意直线
        eSingleLine,
        // 通道线
        eChannelLine,
        // 黄金分割线
        eGoldSegmentedLine,
        // 测试圆
        eCircleLine,
        // 箭头线
        eArrowLine,
    }
    
    #region Aux Lines
    // 辅助线基类
    class AuxiliaryLine
    {
        public int numIndex = -1;
        public CollectDataType cdt = CollectDataType.eNone;
        public AuxLineType lineType = AuxLineType.eNone;
        public List<Point> keyPoints = new List<Point>();
        protected Pen solidPen = null;
        protected Pen dotPen = null;

        public virtual Pen GetSolidPen() { return null; }
        public virtual Pen GetDotPen() { return null; }
        public virtual void SetColor(Color col) { }
    }
    // 水平线
    class HorzLine : AuxiliaryLine
    {
        public static Color sOriLineColor = Color.Aqua;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, 2);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, 1);

        public HorzLine()
        {
            lineType = AuxLineType.eHorzLine;
        }
        public override Pen GetSolidPen()
        {
            if (solidPen == null)
                solidPen = sOriSolidPen.Clone() as Pen;
            return solidPen;
        }
        public override Pen GetDotPen()
        {
            if (dotPen == null)
                dotPen = sOriDotPen.Clone() as Pen;
            return dotPen;
        }
        public override void SetColor(Color col)
        {
            GetSolidPen().Color = col;
            GetDotPen().Color = col;
        }
    }
    // 垂直线
    class VertLine : AuxiliaryLine
    {
        public static Color sOriLineColor = Color.Azure;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, 2);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, 1);

        public VertLine()
        {
            lineType = AuxLineType.eVertLine;
        }
        public override Pen GetSolidPen()
        {
            if (solidPen == null)
                solidPen = sOriSolidPen.Clone() as Pen;
            return solidPen;
        }
        public override Pen GetDotPen()
        {
            if (dotPen == null)
                dotPen = sOriDotPen.Clone() as Pen;
            return dotPen;
        }
        public override void SetColor(Color col)
        {
            GetSolidPen().Color = col;
            GetDotPen().Color = col;
        }
    }
    // 通道直线
    class SingleLine : AuxiliaryLine
    {
        public static Color sOriLineColor = Color.Blue;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, 2);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, 1);

        public SingleLine()
        {
            lineType = AuxLineType.eSingleLine;
        }
        public override Pen GetSolidPen()
        {
            if (solidPen == null)
                solidPen = sOriSolidPen.Clone() as Pen;
            return solidPen;
        }
        public override Pen GetDotPen()
        {
            if (dotPen == null)
                dotPen = sOriDotPen.Clone() as Pen;
            return dotPen;
        }
        public override void SetColor(Color col)
        {
            GetSolidPen().Color = col;
            GetDotPen().Color = col;
        }
    }
    // 通道线
    class ChannelLine : AuxiliaryLine
    {
        public static Color sOriLineColor = Color.Pink;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, 2);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, 1);

        public ChannelLine()
        {
            lineType = AuxLineType.eChannelLine;
        }
        public override Pen GetSolidPen()
        {
            if (solidPen == null)
                solidPen = sOriSolidPen.Clone() as Pen;
            return solidPen;
        }
        public override Pen GetDotPen()
        {
            if (dotPen == null)
                dotPen = sOriDotPen.Clone() as Pen;
            return dotPen;
        }
        public override void SetColor(Color col)
        {
            GetSolidPen().Color = col;
            GetDotPen().Color = col;
        }
    }
    // 黄金分割线
    class GoldSegmentedLine : AuxiliaryLine
    {
        public static Color sOriLineColor = Color.Yellow;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, 2);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, 1);

        public GoldSegmentedLine()
        {
            lineType = AuxLineType.eGoldSegmentedLine;
        }
        public override Pen GetSolidPen()
        {
            if (solidPen == null)
                solidPen = sOriSolidPen.Clone() as Pen;
            return solidPen;
        }
        public override Pen GetDotPen()
        {
            if (dotPen == null)
                dotPen = sOriDotPen.Clone() as Pen;
            return dotPen;
        }
        public override void SetColor(Color col)
        {
            GetSolidPen().Color = col;
            GetDotPen().Color = col;
        }
    }
    // 测试圆线
    class CircleLine : AuxiliaryLine
    {
        public float x, y, size;
        public static Color sOriLineColor = Color.White;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, 2);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, 1);
        public CircleLine()
        {
            lineType = AuxLineType.eCircleLine;
        }
        public override Pen GetSolidPen()
        {
            if (solidPen == null)
                solidPen = sOriSolidPen.Clone() as Pen;
            return solidPen;
        }
        public override Pen GetDotPen()
        {
            if (dotPen == null)
                dotPen = sOriDotPen.Clone() as Pen;
            return dotPen;
        }
        public override void SetColor(Color col)
        {
            GetSolidPen().Color = col;
            GetDotPen().Color = col;
        }
        public void CalcRect()
        {
            float dy = keyPoints[1].Y - keyPoints[0].Y;
            float dx = keyPoints[1].X - keyPoints[0].X;
            float radius = (float)Math.Sqrt(dy * dy + dx * dx);
            x = keyPoints[0].X - radius;
            y = keyPoints[0].Y + radius;
            size = 2 * radius;
        }

    }
    // 箭头
    class ArrowLine : AuxiliaryLine
    {
        public const int C_LINE_WIDTH = 5;
        public static Color sOriLineColor = Color.GreenYellow;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, C_LINE_WIDTH);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, C_LINE_WIDTH);
        public ArrowLine()
        {
            lineType = AuxLineType.eArrowLine;
        }
        public override Pen GetSolidPen()
        {
            if (solidPen == null)
            {
                solidPen = sOriSolidPen.Clone() as Pen;
                solidPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                solidPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            }
            return solidPen;
        }
        public override Pen GetDotPen()
        {
            if (dotPen == null)
            {
                dotPen = sOriDotPen.Clone() as Pen;
                dotPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                dotPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            }
            return dotPen;
        }
        public override void SetColor(Color col)
        {
            GetSolidPen().Color = col;
            GetDotPen().Color = col;
        }
    }
    #endregion

    #region Graphs

    // 图表基类
    class GraphBase
    {
        public GraphManager parent = null;
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
        public virtual void ScrollToData(int index, int winW, int winH, bool needSelect) { }
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
            "画圆",
            "画箭头线",
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

        Font selDataFont;
        Font auxFont;
        Font rulerFont;
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

        Pen redDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Red, 1);
        Pen cyanDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Cyan, 1);

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
            selDataFont = new Font(FontFamily.GenericSerif, 12);
            auxFont = new Font(FontFamily.GenericMonospace, 10);
            rulerFont = new Font(FontFamily.GenericMonospace, 9);
        }

        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            if (GraphDataManager.Instance.HasData(GraphType.eKCurveGraph) == false)
                return false;
            int curIndex = (int)((mousePos.X + canvasOffset.X) / gridScaleW);
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
        }
        public override void ResetGraphPosition()
        {
            canvasOffset.X = 0;
            canvasOffset.Y = 0;
        }

        public override void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            BeforeDraw();

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
                    int startIndex = (int)(canvasOffset.X / gridScaleW) - 1;
                    if (startIndex < 0)
                        startIndex = 0;
                    int endIndex = (int)((canvasOffset.X + winW) / gridScaleW) + 1;
                    if (endIndex > kddc.dataLst.Count)
                        endIndex = kddc.dataLst.Count;
                    if(parent.endShowDataItemIndex != -1)
                    {
                        if (endIndex > parent.endShowDataItemIndex+1)
                            endIndex = parent.endShowDataItemIndex+1;
                    }

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

                    if(enableKRuler)
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

                                startIndex = (int)(canvasOffset.X / gridScaleW) - 1;
                                if (startIndex < 0)
                                    startIndex = 1;
                                endIndex = (int)((canvasOffset.X + winW) / gridScaleW) + 1;
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
                        }
                    }
                }

                // 画辅助线
                if(enableAuxiliaryLine)
                {
                    DrawAuxLineGraph(g, winW, winH, mouseRelPos, numIndex, cdt);
                }

                g.DrawLine(grayDotLinePen, 0, mouseRelPos.Y, winW, mouseRelPos.Y);
                g.DrawLine(grayDotLinePen, mouseRelPos.X, 0, mouseRelPos.X, winH);
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
                if (parent.endShowDataItemIndex != -1)
                {
                    if (endIndex > parent.endShowDataItemIndex + 1)
                        endIndex = parent.endShowDataItemIndex + 1;
                }
                for ( int i = startIndex; i < endIndex; ++i )
                {
                    DrawMACDGraph(g, kddc.macdDataLst.macdMapLst[i], winW, winH, cdt);
                }
                canvasOffset.Y = oriYOff;

                if (preViewDataIndex != -1)
                {
                    g.DrawLine(grayDotLinePen, selDataPtX, 0, selDataPtX, winH);
                    g.DrawLine(grayDotLinePen, selDataPtX + gridScaleW, 0, selDataPtX + gridScaleW, winH);
                }
            }
            EndDraw(g);
        }
        public override void ScrollToData(int index, int winW, int winH, bool needSelect)
        {
            if (needSelect)
                selectKDataIndex = index;
            else
                selectKDataIndex = -1;
            canvasOffset.X = index * gridScaleW;
            autoAllign = true;
        }

        public void UnSelectData()
        {
            selectKDataIndex = -1;
            autoAllign = true;
        }

        public void GetViewItemIndexInfo(ref int startIndex, ref int maxIndex)
        {
            startIndex = (int)(canvasOffset.X / gridScaleW) - 1;
            if (startIndex < 0)
                startIndex = 0;
            maxIndex = GraphDataManager.KGDC.DataLength();
        }

        public void UpdateSelectAuxLinePoint(Point mouseRelPos)
        {
            if(selAuxLine != null && selAuxLinePointIndex >= 0 && selAuxLinePointIndex < selAuxLine.keyPoints.Count)
            {
                selAuxLine.keyPoints[selAuxLinePointIndex] = CanvasToStand(mouseRelPos);
                if(selAuxLine.lineType == AuxLineType.eCircleLine)
                {
                    (selAuxLine as CircleLine).CalcRect();
                }
            }
        }

        public void SelectAuxLine(Point mouseRelPos, int numIndex, CollectDataType cdt)
        {
            selAuxLine = null;
            selAuxLinePointIndex = -1;
            Point standMousePos = CanvasToStand(mouseRelPos);
            for( int i = 0; i < auxiliaryLineList.Count; ++i )
            {
                AuxiliaryLine al = auxiliaryLineList[i];
                if (al.cdt == cdt && al.numIndex == numIndex)
                {
                    for (int j = 0; j < al.keyPoints.Count; ++j)
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
        public void AddHorzLine( Point pt, int numIndex, CollectDataType cdt)
        {
            HorzLine line = new HorzLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add( CanvasToStand(pt) );
            auxiliaryLineList.Add(line);
        }
        public void AddVertLine(Point pt, int numIndex, CollectDataType cdt)
        {
            VertLine line = new VertLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(pt));
            auxiliaryLineList.Add(line);
        }
        public void AddSingleLine(Point p1, Point p2, int numIndex, CollectDataType cdt)
        {
            SingleLine line = new SingleLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1));
            line.keyPoints.Add(CanvasToStand(p2));
            auxiliaryLineList.Add(line);
        }
        public void AddChannelLine(Point line0P1, Point line0P2, Point line1P, int numIndex, CollectDataType cdt)
        {
            ChannelLine line = new ChannelLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(line0P1));
            line.keyPoints.Add(CanvasToStand(line0P2));
            line.keyPoints.Add(CanvasToStand(line1P));
            auxiliaryLineList.Add(line);
        }
        public void AddGoldSegLine(Point p1, Point P2, int numIndex, CollectDataType cdt)
        {
            GoldSegmentedLine line = new GoldSegmentedLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1));
            line.keyPoints.Add(CanvasToStand(P2));
            auxiliaryLineList.Add(line);
        }
        public void AddCircleLine(Point p1, Point p2, int numIndex, CollectDataType cdt)
        {
            CircleLine line = new CircleLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1));
            line.keyPoints.Add(CanvasToStand(p2));
            line.CalcRect();
            auxiliaryLineList.Add(line);
        }
        public void AddArrowLine(Point p1, Point p2, int numIndex, CollectDataType cdt)
        {
            ArrowLine line = new ArrowLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1));
            line.keyPoints.Add(CanvasToStand(p2));
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

            if (preViewDataIndex < 0 && standX <= mouseRelPos.X && standX + gridScaleW >= mouseRelPos.X)
            {
                selDataPtX = standX;
                preViewDataIndex = data.index;
                g.DrawLine(grayDotLinePen, standX, 0, standX, winH);
                g.DrawLine(grayDotLinePen, standX + gridScaleW, 0, standX + gridScaleW, winH);
                //PushLinePts(grayDotLinePen, standX, 0, standX, winH);
                //PushLinePts(grayDotLinePen, standX + gridScaleW, 0, standX + gridScaleW, winH);
                g.DrawString(data.GetInfo(), selDataFont, whiteBrush, 5, 5);
            }

            if(data.index == selectKDataIndex)
            {
                g.DrawLine(yellowLinePen, standX, 0, standX, winH);
                g.DrawLine(yellowLinePen, standX + gridScaleW, 0, standX + gridScaleW, winH);
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

        void DrawAuxLineGraph(Graphics g, int winW, int winH, Point mouseRelPos, int numIndex, CollectDataType cdt)
        {
            float rcHalfSize = 3;
            float rcSize = rcHalfSize * 2;
            for ( int i = 0; i < auxiliaryLineList.Count; ++i )
            {
                AuxiliaryLine al = auxiliaryLineList[i];
                if(al.cdt == cdt && al.numIndex == numIndex)
                    DrawAuxLine(g, winW, winH, al);
            }

            if(mouseHitPts.Count > 0 && auxOperationIndex > AuxLineType.eNone)
            {
                DrawPreviewAuxLine(g, winW, winH, mouseRelPos);
            }

            if(selAuxLine!=null && selAuxLinePointIndex != -1)
            {
                Point pt = StandToCanvas(selAuxLine.keyPoints[selAuxLinePointIndex]);
                g.DrawRectangle(selAuxLine.GetSolidPen(), pt.X - rcHalfSize - 4, pt.Y - rcHalfSize - 4, rcSize + 8, rcSize + 8);
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
                            g.DrawLine(SingleLine.sOriSolidPen, sx, 0, sx, winH);
                        }
                        else
                        {
                            ex = mouseRelPos.X;
                            ey = mouseRelPos.Y;
                            float k = (ey - sy) / (ex - sx);
                            float fyl = sy - sx * k;
                            float fyr = sy + (winW - sx) * k;
                            g.DrawLine(SingleLine.sOriSolidPen, 0, fyl, winW, fyr);
                        }
                        g.DrawRectangle(SingleLine.sOriSolidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eChannelLine:
                    {
                        float sx, sy, ex, ey;
                        sx = mouseHitPts[0].X;
                        sy = mouseHitPts[0].Y;
                        g.DrawRectangle(ChannelLine.sOriSolidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        if (mouseHitPts.Count == 1)
                        {
                            ex = mouseRelPos.X;
                            ey = mouseRelPos.Y;
                            if (sx == ex)
                            {
                                g.DrawLine(ChannelLine.sOriSolidPen, sx, 0, sx, winH);
                            }
                            else
                            {
                                float k = (ey - sy) / (ex - sx);
                                float fyl = sy - sx * k;
                                float fyr = sy + (winW - sx) * k;
                                g.DrawLine(ChannelLine.sOriSolidPen, 0, fyl, winW, fyr);
                            }
                        }
                        else if(mouseHitPts.Count == 2)
                        {
                            ex = mouseHitPts[1].X;
                            ey = mouseHitPts[1].Y;
                            g.DrawRectangle(ChannelLine.sOriSolidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                            if (sx == ex)
                            {
                                g.DrawLine(ChannelLine.sOriSolidPen, sx, 0, sx, winH);
                            }
                            else
                            {
                                float k = (ey - sy) / (ex - sx);
                                float fyl = sy - sx * k;
                                float fyr = sy + (winW - sx) * k;
                                g.DrawLine(ChannelLine.sOriSolidPen, 0, fyl, winW, fyr);
                            }
                            float mx = mouseRelPos.X;
                            float my = mouseRelPos.Y;
                            if (sx == ex)
                            {
                                g.DrawLine(ChannelLine.sOriSolidPen, mx, 0, mx, winH);
                            }
                            else
                            {
                                float k = (ey - sy) / (ex - sx);
                                float oyl = my - mx * k;
                                float oyr = my + (winW - mx) * k;
                                g.DrawLine(ChannelLine.sOriSolidPen, 0, oyl, winW, oyr);
                            }
                            g.DrawLine(ChannelLine.sOriDotPen, sx, sy, mx, my);
                        }
                    }
                    break;
                case AuxLineType.eGoldSegmentedLine:
                    {
                        float sx = mouseHitPts[0].X;
                        float sy = mouseHitPts[0].Y;
                        float ex = mouseRelPos.X;
                        float ey = mouseRelPos.Y;
                        for(int i = 0; i < C_GOLDEN_VALUE.Length; ++i)
                        {
                            float v = C_GOLDEN_VALUE[i];
                            float yv = (ey - sy) * v + sy;
                            g.DrawLine(GoldSegmentedLine.sOriDotPen, 0, yv, winW, yv); g.DrawString(v.ToString(), auxFont, greenBrush, 0, yv);
                        }
                        g.DrawLine(GoldSegmentedLine.sOriSolidPen, 0, sy, winW, sy); g.DrawString("0", auxFont, greenBrush, 0, sy);
                        g.DrawLine(GoldSegmentedLine.sOriSolidPen, 0, ey, winW, ey); g.DrawString("1", auxFont, greenBrush, 0, ey);
                        g.DrawLine(GoldSegmentedLine.sOriDotPen, sx, sy, ex, ey);
                        g.DrawRectangle(GoldSegmentedLine.sOriSolidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eCircleLine:
                    {
                        float cx = mouseHitPts[0].X;
                        float cy = mouseHitPts[0].Y;
                        float ex = mouseRelPos.X;
                        float ey = mouseRelPos.Y;
                        float dx = cx - ex;
                        float dy = cy - ey;
                        float radius = (float)Math.Sqrt(dx * dx + dy * dy);
                        float size = 2 * radius;
                        g.DrawEllipse(CircleLine.sOriDotPen, cx - radius, cy - radius, size, size);
                        g.DrawRectangle(CircleLine.sOriSolidPen, cx - rcHalfSize, cy - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eArrowLine:
                    {
                        float cx = mouseHitPts[0].X;
                        float cy = mouseHitPts[0].Y;
                        float ex = mouseRelPos.X;
                        float ey = mouseRelPos.Y;
                        ArrowLine.sOriSolidPen.Width = ArrowLine.C_LINE_WIDTH;
                        g.DrawLine(ArrowLine.sOriSolidPen, cx, cy, ex, ey);
                        ArrowLine.sOriSolidPen.Width = 1;
                        //g.DrawRectangle(ArrowLine.sOriSolidPen, cx - rcHalfSize, cy - rcHalfSize, rcSize, rcSize);
                        //g.DrawRectangle(ArrowLine.sOriSolidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;
            }
        }

        static readonly float[] C_GOLDEN_VALUE = new float[]  { 0.382f, 0.618f, 1.382f, 1.618f, 2f, 2.382f, 2.618f, 4, 4.382f, 4.618f, 8, };

        void DrawAuxLine(Graphics g, int winW, int winH, AuxiliaryLine line)//, List<Point> pts, AuxLineType lineType)
        {
            List<Point> pts = line.keyPoints;
            Pen solidPen = line.GetSolidPen();
            Pen dotPen = line.GetDotPen();

            switch (line.lineType)
            {
                case AuxLineType.eHorzLine:
                    {
                        float x = StandToCanvas(pts[0].X, true);
                        float y = StandToCanvas(pts[0].Y, false);
                        g.DrawLine(solidPen, 0, y, winW, y);
                        g.DrawRectangle(solidPen, x - rcHalfSize, y - rcHalfSize, rcSize, rcSize);
                    }
                    break;

                case AuxLineType.eVertLine:
                    {
                        float x = StandToCanvas(pts[0].X, true);
                        float y = StandToCanvas(pts[0].Y, false);
                        g.DrawLine(solidPen, x, 0, x, winH);
                        g.DrawRectangle(solidPen, x - rcHalfSize, y - rcHalfSize, rcSize, rcSize);
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
                            g.DrawLine(solidPen, sx, 0, sx, winH);
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
                            sx = StandToCanvas(pts[0].X, true);
                            sy = StandToCanvas(pts[0].Y, false);
                            ex = sx;
                            ey = StandToCanvas(pts[1].Y, false);
                            ox = StandToCanvas(pts[2].X, true);
                            oy = StandToCanvas(pts[2].Y, false);
                            g.DrawLine(solidPen, sx, 0, sx, winH);
                            g.DrawLine(solidPen, ox, 0, ox, winH);
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
                        float sx = StandToCanvas(pts[0].X, true);
                        float sy = StandToCanvas(pts[0].Y, false);
                        float ex = StandToCanvas(pts[1].X, true);
                        float ey = StandToCanvas(pts[1].Y, false);
                        for( int i = 0; i < C_GOLDEN_VALUE.Length; ++i )
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
                        CircleLine cl = line as CircleLine;
                        float x = StandToCanvas(cl.x, true);
                        float y = StandToCanvas(cl.y, false);
                        g.DrawEllipse(dotPen, x, y, cl.size, cl.size);

                        float sx = StandToCanvas(pts[0].X, true);
                        float sy = StandToCanvas(pts[0].Y, false);
                        float ex = StandToCanvas(pts[1].X, true);
                        float ey = StandToCanvas(pts[1].Y, false);
                        g.DrawLine(solidPen, sx, sy, ex, ey);
                        g.DrawRectangle(solidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        g.DrawRectangle(solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;
                case AuxLineType.eArrowLine:
                    {
                        float cx = StandToCanvas(pts[0].X, true);
                        float cy = StandToCanvas(pts[0].Y, false);
                        float ex = StandToCanvas(pts[1].X, true);
                        float ey = StandToCanvas(pts[1].Y, false);
                        solidPen.Width = ArrowLine.C_LINE_WIDTH;
                        g.DrawLine(solidPen, cx, cy, ex, ey);
                        //g.DrawRectangle(solidPen, cx - rcHalfSize, cy - rcHalfSize, rcSize, rcSize);
                        //g.DrawRectangle(solidPen, ex - rcHalfSize, ey - rcHalfSize, rcSize, rcSize);
                    }
                    break;
            }
        }

        void DrawKRuler(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos, KDataDictContainer kddc, float missRelHeight)
        {
            float downRCH = gridScaleH * missRelHeight;
            float strDist = 1.5f * gridScaleW;
            int endIndex = kddc.dataLst.Count - 1;
            if (parent.endShowDataItemIndex != -1 && parent.endShowDataItemIndex < kddc.dataLst.Count)
                endIndex = parent.endShowDataItemIndex;
            KDataDict lastKDD = kddc.dataLst[endIndex];
            KData data = lastKDD.GetData(cdt, false);
            float standX = (data.index + 1) * gridScaleW;
            float standY = (data.KValue) * gridScaleH;
            standX = StandToCanvas(standX, true);
            standY = StandToCanvas(standY, false);
            if (standX > winW)
                return;
            bool isUpOK = false;
            bool isDownOK = false;
            int i = 0;

            //for( int i = 0; i < 20; ++i )
            while( isUpOK == false || isDownOK == false )
            {
                float X = standX + i * gridScaleW;
                float upY = standY - (i + 1) * gridScaleH;
                float downY = standY + i * downRCH;
                int stepID = i + 1;

                if (isUpOK == false)
                {
                    g.DrawRectangle(redDotPen, X, upY, gridScaleW, gridScaleH);
                    g.DrawString(stepID.ToString(), rulerFont, redBrush, X + strDist, upY+5);
                    if (X > winW || upY < 0)
                        isUpOK = true;
                }
                if (isDownOK == false)
                {
                    g.DrawRectangle(cyanDotPen, X, downY, gridScaleW, downRCH);
                    g.DrawString(stepID.ToString(), rulerFont, cyanBrush, X + strDist, downY-5);
                    if (X > winW || downY > winH)
                        isDownOK = true;
                }
                ++i;
            }
        }
    }

    // 柱状图
    class GraphBar : GraphBase
    {
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush tagBrush = new SolidBrush(Color.White);
        SolidBrush greenBrush = new SolidBrush(Color.Green);
        int selKDataIndex = -1;

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

            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (bgdc.CurrentSelectItem != null)
                currentItem = bgdc.CurrentSelectItem;
            string info = "[" + currentItem.idTag + "] [" + currentItem.lotteryNumber + "]";
            g.DrawString(info, tagFont, tagBrush, 5, 5);
        }
    }

    // 交易图
    class GraphTrade : GraphBase
    {
        public bool autoAllign = false;

        Pen grayDotLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);
        Pen redLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 1);
        Pen cyanLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Cyan, 1);
        Pen whiteLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);
        Pen moneyLvLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);

        Font tipsFont = new Font(FontFamily.GenericMonospace, 12);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        int selectTradeIndex = -1;
        static float[] TRADE_LVS = new float[] { 0, 0.5f, 1.0f, 1.5f, 2.0f, };

        public GraphTrade()
        {
            gridScaleW = 10;
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
            g.DrawLine(grayDotLinePen, 0, mouseRelPos.Y, winW, mouseRelPos.Y);

            float halfSize = gridScaleW * 0.2f;
            if (halfSize < 4)
                halfSize = 4;
            float fullSize = halfSize * 2;
            float halfWinH = winH * 0.5f;
            float halfGridW = gridScaleW * 0.5f;

            TradeDataManager tdm = TradeDataManager.Instance;
            /*
            if(tdm.historyTradeDatas.Count == 0)
            {
                float y = tdm.currentMoney * gridScaleH;
                float relY = StandToCanvas(y, false);
                if(relY < 0 || relY > winH)
                {
                    canvasOffset.Y = y + winH * 0.5f;
                }
                relY = StandToCanvas(y, false);
                g.DrawRectangle(whiteLinePen, halfGridW - halfSize, halfWinH - halfSize, fullSize, fullSize);
                g.DrawLine(whiteLinePen, halfGridW, halfWinH, winW, halfWinH);
                return;
            }
            */
            for(int i = 0; i < TRADE_LVS.Length; ++i)
            {
                float lv = TRADE_LVS[i];
                float money = tdm.startMoney * lv;
                float y = money * gridScaleH;
                float relY = StandToCanvas(y, false);
                if (lv <= 0)
                    moneyLvLinePen.Color = Color.Gray;
                else if (lv >= 1)
                    moneyLvLinePen.Color = Color.Red;
                else
                    moneyLvLinePen.Color = Color.Cyan;
                g.DrawLine(moneyLvLinePen, 0, relY, winW, relY);
                g.DrawString(money.ToString("f0"), tipsFont, whiteBrush, winW - 65, relY);
            }
            if (tdm.historyTradeDatas.Count == 0)
            {
                float y = tdm.startMoney * gridScaleH;
                float relY = StandToCanvas(y, false);
                if (relY < 0 || relY > winH)
                {
                    canvasOffset.Y = y + winH * 0.5f;
                }
                return;
            }

            //canvasOffset.Y = winH / 2;
            float maxGap = Math.Max(Math.Abs(tdm.maxValue), Math.Abs(tdm.minValue)) * 2;
            gridScaleH = winH / maxGap * 0.9f;
            int startIndex = (int)(canvasOffset.X / gridScaleW) - 1;
            if (startIndex < 0)
                startIndex = 0;
            int endIndex = (int)((canvasOffset.X + winW) / gridScaleW) + 1;
            if (endIndex > tdm.historyTradeDatas.Count)
                endIndex = tdm.historyTradeDatas.Count;

            // 自动对齐
            if (autoAllign)
            {
                float endY = tdm.historyTradeDatas[endIndex - 1].moneyAtferTrade * gridScaleH;
                float relEY = StandToCanvas(endY, false);
                bool isEYOut = relEY < 0 || relEY > winH;
                if (isEYOut)
                    canvasOffset.Y = endY + winH * 0.5f;
                autoAllign = false;
            }

            int selIndex = -1;
            for(int i = startIndex; i < endIndex; ++i)
            {
                TradeDataBase tdb = tdm.historyTradeDatas[i];
                float cx = i * gridScaleW + halfGridW;
                float px = cx - gridScaleW;
                float py = tdb.moneyBeforeTrade * gridScaleH;
                float cy = tdb.moneyAtferTrade * gridScaleH;
                cx = StandToCanvas(cx, true);
                px = StandToCanvas(px, true);
                cy = StandToCanvas(cy, false);
                py = StandToCanvas(py, false);
                Pen pen = (tdb.cost == 0 ? whiteLinePen : (tdb.reward > 0 ? redLinePen : cyanLinePen));
                if(i == 0)
                    g.DrawRectangle(whiteLinePen, px - halfSize, py - halfSize, fullSize, fullSize);
                g.DrawRectangle(pen, cx - halfSize, cy - halfSize, fullSize, fullSize);
                g.DrawLine(pen, px, py, cx, cy);

                if(mouseRelPos.X >= cx - halfGridW && mouseRelPos.X <= cx + halfGridW && selIndex == -1)
                {
                    selIndex = i;
                    g.DrawLine(grayDotLinePen, cx - halfGridW, 0, cx - halfGridW, winH);
                    g.DrawLine(grayDotLinePen, cx + halfGridW, 0, cx + halfGridW, winH);
                    string info = tdb.GetTips() + "\n" + 
                        "[对:" + tdm.rightCount + 
                        "] [错:" + tdm.wrongCount + 
                        "] [放弃:" + tdm.untradeCount + 
                        "]\n[最高:" + tdm.maxValue + 
                        "] [最低:" + tdm.minValue + "]";
                    g.DrawString(info, tipsFont, whiteBrush, 5, 5);
                }

                if(selectTradeIndex == i)
                {
                    g.DrawLine(whiteLinePen, cx - halfGridW, 0, cx - halfGridW, winH);
                    g.DrawLine(whiteLinePen, cx + halfGridW, 0, cx + halfGridW, winH);
                }
            }
        }
        public override void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            int selID = -1;
            TradeDataManager tdm = TradeDataManager.Instance;
            if(tdm.waitingTradeDatas.Count > 0)
            {
                float y = winH - 2 * gridScaleW;

                for (int i = 0; i < tdm.waitingTradeDatas.Count; ++i)
                {
                    float x = i * gridScaleW;
                    g.DrawRectangle(whiteLinePen, x, y, gridScaleW, gridScaleW);

                    if(selID == -1 && mouseRelPos.X >= x && mouseRelPos.X <= x + gridScaleW)
                    {
                        selID = i;
                        g.DrawLine(grayDotLinePen, x, 0, x, winH);
                        g.DrawLine(grayDotLinePen, x + gridScaleW, 0, x + gridScaleW, winH);

                        g.DrawString(tdm.waitingTradeDatas[i].GetTips(), tipsFont, whiteBrush, 5, 5);
                    }
                }
            }
        }

        public void GetViewItemIndexInfo(ref int startIndex, ref int maxIndex)
        {
            startIndex = (int)(canvasOffset.X / gridScaleW) - 1;
            if (startIndex < 0)
                startIndex = 0;

            maxIndex = TradeDataManager.Instance.historyTradeDatas.Count;
        }

        public override void ScrollToData(int index, int winW, int winH, bool needSelect)
        {
            if (needSelect)
                selectTradeIndex = index;
            else
                selectTradeIndex = -1;
            canvasOffset.X = (selectTradeIndex + 1) * gridScaleW;
            autoAllign = true;
        }

        public int SelectTradeData(Point mouseRelPos)
        {
            TradeDataManager tdm = TradeDataManager.Instance;
            selectTradeIndex = -1;
            Point standMousePos = CanvasToStand(mouseRelPos);
            int mouseHoverID = (int)(standMousePos.X / gridScaleW);
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

    #endregion

    // 图表管理器
    class GraphManager
    {
        public class FavoriteChart
        {
            public int numIndex;
            public CollectDataType cdt;
            public string tag;
        }

        List<FavoriteChart> favoriteCharts = new List<FavoriteChart>();
        Dictionary<GraphType, GraphBase> sGraphMap = new Dictionary<GraphType, GraphBase>();
        GraphBase curGraph = null;
        GraphType curGraphType = GraphType.eNone;
        public GraphBar barGraph;
        public GraphKCurve kvalueGraph;
        public GraphTrade tradeGraph;
        public int endShowDataItemIndex = -1;

        public GraphManager()
        {
            barGraph = new GraphBar(); barGraph.parent = this;
            kvalueGraph = new GraphKCurve(); kvalueGraph.parent = this;
            tradeGraph = new GraphTrade(); tradeGraph.parent = this;
            sGraphMap.Add(GraphType.eKCurveGraph, kvalueGraph);
            sGraphMap.Add(GraphType.eBarGraph, barGraph);
            sGraphMap.Add(GraphType.eTradeGraph, tradeGraph);

        }

        public void OnTradeCompleted()
        {
            if (endShowDataItemIndex != -1)
                endShowDataItemIndex++;
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

        public FavoriteChart AddFavoriteChart(int numIndex, CollectDataType cdt)
        {
            if(FindFavoriteChart(numIndex, cdt) == null)
            {
                FavoriteChart fc = new FavoriteChart();
                fc.numIndex = numIndex;
                fc.cdt = cdt;
                int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                fc.tag = TradeDataBase.NUM_TAGS[numIndex] + "_" + GraphDataManager.S_CDT_TAG_LIST[cdtID];
                favoriteCharts.Add(fc);
                return fc;
            }
            return null;
        }
        public FavoriteChart FindFavoriteChart(int numIndex, CollectDataType cdt)
        {
            for (int i = 0; i < favoriteCharts.Count; ++i)
            {
                if (favoriteCharts[i].numIndex == numIndex && favoriteCharts[i].cdt == cdt)
                    return favoriteCharts[i];
            }
            return null;
        }
        public FavoriteChart GetFavoriteChart(int index)
        {
            if (index >= 0 && index < favoriteCharts.Count)
                return favoriteCharts[index];
            return null;
        }
        public void ClearFavoriteCharts()
        {
            favoriteCharts.Clear();
        }
    }

#endregion
}