#define TRADE_DBG

using LotteryAnalyze.UI;
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
        // 出号率曲线
        eAppearenceGraph,
        // 遗漏图
        eMissCountGraph,
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
        // 测试矩形
        eRectLine,
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
        public virtual bool HitTest(CollectDataType cdt, int numIndex, Point standMousePos, float rcHalfSize, ref int selKeyPtIndex)
        {
            selKeyPtIndex = -1;
            if (this.cdt == cdt && this.numIndex == numIndex)
            {
                for (int j = 0; j < this.keyPoints.Count; ++j)
                {
                    Point pt = this.keyPoints[j];
                    if (pt.X - rcHalfSize > standMousePos.X ||
                        pt.X + rcHalfSize < standMousePos.X ||
                        pt.Y - rcHalfSize > standMousePos.Y ||
                        pt.Y + rcHalfSize < standMousePos.Y)
                        continue;
                    else
                    {
                        selKeyPtIndex = j;
                        return true;
                    }
                }
            }
            return false;
        }
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
        public const int C_LINE_WIDTH = 1;
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
    // 测试矩形
    class RectLine : AuxiliaryLine
    {
        public const int C_LINE_WIDTH = 5;
        public static Color sOriLineColor = Color.GreenYellow;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, C_LINE_WIDTH);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, C_LINE_WIDTH);
        public RectLine()
        {
            lineType = AuxLineType.eRectLine;
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
        public override bool HitTest(CollectDataType cdt, int numIndex, Point standMousePos, float rcHalfSize, ref int selKeyPtIndex)
        {
            selKeyPtIndex = -1;
            if (this.cdt == cdt && this.numIndex == numIndex)
            {
                float minx = this.keyPoints[0].X;
                float maxx = this.keyPoints[0].X;
                float miny = this.keyPoints[0].Y;
                float maxy = this.keyPoints[0].Y;

                for (int j = 0; j < this.keyPoints.Count; ++j)
                {
                    Point pt = this.keyPoints[j];
                    if (pt.X < minx)
                        minx = pt.X;
                    if (pt.X > maxx)
                        maxx = pt.X;
                    if (pt.Y < miny)
                        miny = pt.Y;
                    if (pt.Y > maxy)
                        maxy = pt.Y;

                    if (pt.X - rcHalfSize > standMousePos.X ||
                        pt.X + rcHalfSize < standMousePos.X ||
                        pt.Y - rcHalfSize > standMousePos.Y ||
                        pt.Y + rcHalfSize < standMousePos.Y)
                        continue;
                    else
                    {
                        selKeyPtIndex = j;
                        return true;
                    }
                }

                if (standMousePos.X > minx && standMousePos.X < maxx &&
                    standMousePos.Y > miny && standMousePos.Y < maxy)
                {
                    return true;
                }
            }
            return false;
        }

    }

    #endregion

    #region Graphs

    // 图表基类
    class GraphBase
    {
        public GraphManager parent = null;
        //public float gridScaleH = 20;
        //public float gridScaleW = 5;
        public PointF gridScaleUp = new PointF(5, 20);
        public PointF gridScaleDown = new Point(5, 20);
        public PointF canvasOffset = new PointF(0, 0);
        public PointF downCanvasOffset = new PointF(0, 0);

        public float DownCanvasToStand(float v, bool isX)
        {
            if (isX)
                return (v + downCanvasOffset.X);
            else
                return (downCanvasOffset.Y) - v;
        }

        public float DownStandToCanvas(float v, bool isX)
        {
            if (isX)
                return v - downCanvasOffset.X;
            else
                return downCanvasOffset.Y - v;
        }

        public float UpCanvasToStand(float v, bool isX)
        {
            if (isX)
                return (v + canvasOffset.X);
            else
                return (canvasOffset.Y) - v;
        }
        public float UpStandToCanvas(float v, bool isX)
        {
            if (isX)
                return v - canvasOffset.X;
            else
                return canvasOffset.Y - v;
        }
        public Point UpCanvasToStand(Point pt)
        {
            Point res = new Point();
            res.X = (int)UpCanvasToStand((float)pt.X, true);
            res.Y = (int)UpCanvasToStand((float)pt.Y, false);
            return res;
        }
        public Point UpStandToCanvas(Point pt)
        {
            Point res = new Point();
            res.X = (int)UpStandToCanvas((float)pt.X, true);
            res.Y = (int)UpStandToCanvas((float)pt.Y, false);
            return res;
        }

        public Point DownCanvasToStand(Point pt)
        {
            Point res = new Point();
            res.X = (int)DownCanvasToStand((float)pt.X, true);
            res.Y = (int)DownCanvasToStand((float)pt.Y, false);
            return res;
        }
        public Point DownStandToCanvas(Point pt)
        {
            Point res = new Point();
            res.X = (int)DownStandToCanvas((float)pt.X, true);
            res.Y = (int)DownStandToCanvas((float)pt.Y, false);
            return res;
        }

        public float StandToCanvas(float v, bool isX, bool upPanel)
        {
            if (upPanel)
                return UpStandToCanvas(v, isX);
            else
                return DownStandToCanvas(v, isX);
        }

        public float CanvasToStand(float v, bool isX, bool upPanel)
        {
            if (upPanel)
                return UpCanvasToStand(v, isX);
            else
                return DownCanvasToStand(v, isX);
        }

        public Point StandToCanvas(Point pt, bool upPanel)
        {
            if (upPanel)
                return UpStandToCanvas(pt);
            else
                return DownStandToCanvas(pt);
        }

        public Point CanvasToStand(Point pt, bool upPanel)
        {
            if (upPanel)
                return UpCanvasToStand(pt);
            else
                return DownCanvasToStand(pt);
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
        public virtual void ScrollToData(int index, int winW, int winH, bool needSelect, int xOffset = 0, bool needScrollToData=true) { }
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
        Dictionary<SolidBrush, List<RectangleF>> rcPools = new Dictionary<SolidBrush,List<RectangleF>>();
        List<AuxiliaryLine> auxiliaryLineListUpPanel = new List<AuxiliaryLine>();
        List<AuxiliaryLine> auxiliaryLineListDownPanel = new List<AuxiliaryLine>();
        public List<Point> mouseHitPtsUpPanel = new List<Point>();
        public List<Point> mouseHitPtsDownPanel = new List<Point>();
        public AuxiliaryLine selAuxLineUpPanel = null;
        public AuxiliaryLine selAuxLineDownPanel = null;
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
                    if(parent.endShowDataItemIndex != -1)
                    {
                        if (endIndex > parent.endShowDataItemIndex+1)
                            endIndex = parent.endShowDataItemIndex+1;
                    }

                    startItemIndex = startIndex;

                    // 自动对齐
                    if (autoAllign)
                    {
                        float endY = kddc.dataLst[endIndex - 1].dataDict[cdt].KValue * gridScaleUp.Y;
                        if(selectKDataIndex >= 0 && selectKDataIndex < kddc.dataLst.Count)
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
                        KDataDict kdDict = kddc.dataLst[i];
                        KData data = kdDict.GetData(cdt, false);
                        if (data == null)
                            continue;
                        DrawKDataGraph(g, data, winW, winH, missRelHeight, mouseRelPos, kddc, cdt);
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
                            if(i == preViewDataIndex)
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
                if(enableAuxiliaryLine)
                {
                    DrawAuxLineGraph(g, winW, winH, mouseRelPos, numIndex, cdt, auxiliaryLineListUpPanel, mouseHitPtsUpPanel, true);
                }

                g.DrawLine(grayDotLinePen, 0, mouseRelPos.Y, winW, mouseRelPos.Y);
                float kValueMouse = CanvasToStand(mouseRelPos.Y, false, true) / gridScaleUp.Y;
                g.DrawString(kValueMouse.ToString("f3"), selDataFont, whiteBrush, winW - 80, mouseRelPos.Y - 20);
                //g.DrawLine(grayDotLinePen, mouseRelPos.X, 0, mouseRelPos.X, winH);
            }

            if(enableAuxiliaryLine && preViewDataIndex != beforeID)
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
            if(enableMACD && kddc != null)
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
                for ( int i = startIndex; i <= endIndex; ++i )
                {
                    DrawMACDGraph(g, kddc.macdDataLst.macdMapLst[i], winW, winH, cdt, i == selectKDataIndex);
                }

                if(preViewDataIndex != -1)
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
                        (selAuxLineUpPanel as CircleLine).CalcRect();
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
                        (selAuxLineDownPanel as CircleLine).CalcRect();
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
            if(upPanel)
            {
                selAuxLineUpPanel = null;
                selAuxLinePointIndexUpPanel = -1;
                Point standMousePos = CanvasToStand(mouseRelPos, upPanel);
                for (int i = 0; i < auxiliaryLineListUpPanel.Count; ++i)
                {
                    AuxiliaryLine al = auxiliaryLineListUpPanel[i];
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
                    AuxiliaryLine al = auxiliaryLineListDownPanel[i];
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
            if(upPanel)
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
        public void AddHorzLine( Point pt, int numIndex, CollectDataType cdt, bool upPanel)
        {
            HorzLine line = new HorzLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add( CanvasToStand(pt, upPanel) );
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddVertLine(Point pt, int numIndex, CollectDataType cdt, bool upPanel)
        {
            VertLine line = new VertLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(pt, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddSingleLine(Point p1, Point p2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            SingleLine line = new SingleLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(p2, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddChannelLine(Point line0P1, Point line0P2, Point line1P, int numIndex, CollectDataType cdt, bool upPanel)
        {
            ChannelLine line = new ChannelLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(line0P1, upPanel));
            line.keyPoints.Add(CanvasToStand(line0P2, upPanel));
            line.keyPoints.Add(CanvasToStand(line1P, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddGoldSegLine(Point p1, Point P2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            GoldSegmentedLine line = new GoldSegmentedLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(P2, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddCircleLine(Point p1, Point p2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            CircleLine line = new CircleLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(p2, upPanel));
            line.CalcRect();
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddArrowLine(Point p1, Point p2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            ArrowLine line = new ArrowLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(p2, upPanel));
            (upPanel ? auxiliaryLineListUpPanel : auxiliaryLineListDownPanel).Add(line);
        }
        public void AddRectLine(Point p1, Point p2, int numIndex, CollectDataType cdt, bool upPanel)
        {
            RectLine line = new RectLine();
            line.numIndex = numIndex;
            line.cdt = cdt;
            line.keyPoints.Add(CanvasToStand(p1, upPanel));
            line.keyPoints.Add(CanvasToStand(p2, upPanel));
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
            if(res == null)
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

            if(data.index == selectKDataIndex)
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
            if(prevMPM != null)
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
            g.FillRectangle(isUp? redBrush : cyanBrush, cx - halfW, rcY, gridScaleDown.X, Math.Abs(standY));
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

            if(drawSelectedLine)
            {
                float xL = cx - halfW;
                float xR = cx + halfW;
                g.DrawLine(yellowLinePen, xL, 0, xL, winH);
                g.DrawLine(yellowLinePen, xR, 0, xR, winH);
            }
        }

        void DrawAuxLineGraph(Graphics g, int winW, int winH, Point mouseRelPos, int numIndex, CollectDataType cdt, List<AuxiliaryLine> auxLines, List<Point> auxPoints, bool upPanel)
        {
            float rcHalfSize = 3;
            float rcSize = rcHalfSize * 2;
            for ( int i = 0; i < auxLines.Count; ++i )
            {
                AuxiliaryLine al = auxLines[i];
                if(al.cdt == cdt && al.numIndex == numIndex)
                    DrawAuxLine(g, winW, winH, al, upPanel);
            }

            if(auxPoints.Count > 0 && auxOperationIndex > AuxLineType.eNone)
            {
                DrawPreviewAuxLine(g, winW, winH, mouseRelPos, auxPoints);
            }

            if(selAuxLineUpPanel!=null && selAuxLinePointIndexUpPanel != -1)
            {
                Point pt = StandToCanvas(selAuxLineUpPanel.keyPoints[selAuxLinePointIndexUpPanel], upPanel);
                g.DrawRectangle(selAuxLineUpPanel.GetSolidPen(), pt.X - rcHalfSize - 4, pt.Y - rcHalfSize - 4, rcSize + 8, rcSize + 8);
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
                        KGraphDataContainer kgdc = GraphDataManager.KGDC;
                        KDataDictContainer kddc = kgdc.GetKDataDictContainer(numIndex);
                        KDataDict kdd = kddc.dataLst[preViewDataIndex];
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
            catch(Exception e)
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
            switch(auxOperationIndex)
            {
                case AuxLineType.eSingleLine:
                    {
                        float sx, sy, ex, ey;
                        sx = auxPoints[0].X;
                        sy = auxPoints[0].Y;
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
                        sx = auxPoints[0].X;
                        sy = auxPoints[0].Y;
                        g.DrawRectangle(ChannelLine.sOriSolidPen, sx - rcHalfSize, sy - rcHalfSize, rcSize, rcSize);
                        if (auxPoints.Count == 1)
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
                        else if(auxPoints.Count == 2)
                        {
                            ex = auxPoints[1].X;
                            ey = auxPoints[1].Y;
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
                        float sx = auxPoints[0].X;
                        float sy = auxPoints[0].Y;
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
                        float cx = auxPoints[0].X;
                        float cy = auxPoints[0].Y;
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
                        float cx = auxPoints[0].X;
                        float cy = auxPoints[0].Y;
                        float ex = mouseRelPos.X;
                        float ey = mouseRelPos.Y;
                        ArrowLine.sOriSolidPen.Width = ArrowLine.C_LINE_WIDTH;
                        g.DrawLine(ArrowLine.sOriSolidPen, cx, cy, ex, ey);
                        ArrowLine.sOriSolidPen.Width = 1;
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
                        ArrowLine.sOriSolidPen.Width = ArrowLine.C_LINE_WIDTH;
                        g.DrawLine(ArrowLine.sOriSolidPen, cx, cy, ex, ey);
                        ArrowLine.sOriSolidPen.Width = 1;

                        float sx = Math.Min(cx, ex);
                        float sy = Math.Min(cy, ey);
                        float bx = Math.Max(cx, ex);
                        float by = Math.Max(cy, ey);
                        g.DrawRectangle(ArrowLine.sOriSolidPen, sx, sy, bx - sx, by - sy);
                    }
                    break;
            }
        }

        static readonly float[] C_GOLDEN_VALUE = new float[]  { 0.382f, 0.618f, 1.382f, 1.618f, 2f, 2.382f, 2.618f, 4, 4.382f, 4.618f, 8, };

        void DrawAuxLine(Graphics g, int winW, int winH, AuxiliaryLine line, bool upPanel)//, List<Point> pts, AuxLineType lineType)
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
                        solidPen.Width = ArrowLine.C_LINE_WIDTH;
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
                        solidPen.Width = ArrowLine.C_LINE_WIDTH;
                        g.DrawLine(solidPen, cx, cy, ex, ey);

                        float sx = Math.Min(cx, ex);
                        float sy = Math.Min(cy, ey);
                        float bx = Math.Max(cx, ex);
                        float by = Math.Max(cy, ey);
                        g.DrawRectangle(ArrowLine.sOriSolidPen, sx, sy, bx - sx, by - sy);
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
                float x = StandToCanvas( startID * gridScaleW, true, true );
                g.DrawLine(whiteLinePen, x, 0, x, winH);
            }

            float downRCH = gridScaleH * missRelHeight;
            float strDist = 1.5f * gridScaleW;

            KDataDict lastKDD = kddc.dataLst[endIndex];
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
        Font tagFont = new Font(FontFamily.GenericSerif, 16);
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
            if (bgdc.HasData() == false || 
                bgdc.totalCollectCount == 0)
                return;

            float startX = 0;
            float startY = winH * 0.1f;
            float MaxRcH = winH * 0.8f;
            float bottom = winH * 0.9f;

            BarGraphDataContianer.DataUnitLst dul = bgdc.allDatas[numIndex];
            float gap = (float)winW / dul.dataLst.Count;

            string statisticInfo = ""; 
            for (int i = 0; i < dul.dataLst.Count; ++i)
            {
                float r = 0;
                Brush brush = greenBrush;
                float rate = ((float)(dul.dataLst[i].data) / (float)bgdc.totalCollectCount);
                if(dul.dataLst[i].type == BarGraphDataContianer.StatisticsType.eAppearCountPath012)
                {
                    if(string.IsNullOrEmpty(statisticInfo))
                        statisticInfo = "012路";
                    if (i == 0 && rate > 0.4f)
                        brush = redBrush;
                    else if (i > 0 && rate > 0.3f)
                        brush = redBrush;

                    if (i == 0)
                        r = (rate - 0.4f) / 0.4f;
                    else
                        r = (rate - 0.3f) / 0.3f;
                }
                else if(dul.dataLst[i].type == BarGraphDataContianer.StatisticsType.eAppearCountFrom0To9)
                {
                    if (string.IsNullOrEmpty(statisticInfo))
                        statisticInfo = "数字0-9";
                    r = (rate - 0.1f) / 0.1f;
                    if(rate > 0.1f)
                        brush = redBrush;
                }
                string brand = "";
                Brush brandBrush = tagBrush;
                if (r > 0.5f)
                {
                    brand = "热";
                    brandBrush = redBrush;
                }
                else if (r < -0.5f)
                {
                    brand = "冷";
                    brandBrush = greenBrush;
                }
                else
                {
                    brand = "温";
                }

                float rcH = MaxRcH * rate;
                startY = bottom - rcH;
                g.FillRectangle(brush, startX, startY, gap * 0.9f, rcH);
                g.DrawString(dul.dataLst[i].tag, tagFont, tagBrush, startX, bottom);
                g.DrawString(dul.dataLst[i].data.ToString(), tagFont, tagBrush, startX, startY - 30);
                g.DrawString(brand, tagFont, brandBrush, startX, 60);
                startX += gap;
            }

            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (bgdc.CurrentSelectItem != null)
                currentItem = bgdc.CurrentSelectItem;
            string info = "[" + currentItem.idTag + "] [" + currentItem.lotteryNumber + "]\n";
            info += "统计 " + currentItem.idTag + " " + KDataDictContainer.C_TAGS[numIndex] + " 前" + 
                GraphDataManager.BGDC.StatisticRangeCount + "期 " + statisticInfo + "的出现概率";
            g.DrawString(info, tagFont, tagBrush, 5, 5);
        }

        public override void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            BarGraphDataContianer bgdc = GraphDataManager.BGDC;
            if (bgdc.HasData() == false || 
                bgdc.totalCollectCount == 0 ||
                bgdc.curStatisticsType != BarGraphDataContianer.StatisticsType.eAppearCountFrom0To9)
                return;

            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (bgdc.CurrentSelectItem != null)
                currentItem = bgdc.CurrentSelectItem;
            if (currentItem == null)
                return;
            List<NumberCmpInfo> nums = new List<NumberCmpInfo>();
            TradeDataManager.FindAllNumberProbabilities(currentItem, ref nums);
            //nums.Sort(NumberCmpInfo.SortByNumber);

            float startX = 0;
            float startY = winH * 0.1f;
            float MaxRcH = winH * 0.8f;
            float bottom = winH * 0.9f;
            
            float gap = (float)winW / 10;
            Font tagFont = new Font(FontFamily.GenericSerif, 12);
            for (int i = 0; i < nums.Count; ++i)
            {
                int index = NumberCmpInfo.FindIndex(nums, (sbyte)(i), false);
                Brush brush = greenBrush;
                float rate = nums[index].rate / 100.0f;
                if (rate > 0.1f)
                    brush = redBrush;

                float rcH = MaxRcH * rate;
                startY = bottom - rcH;
                g.FillRectangle(brush, startX, startY, gap * 0.9f, rcH);
                g.DrawString(nums[index].number.ToString(), tagFont, tagBrush, startX, bottom);
                g.DrawString(nums[index].rate.ToString("f1") + "%", tagFont, tagBrush, startX, startY - 30);
                startX += gap;
            }
            g.DrawString("统计 " + currentItem.idTag + " 前" + LotteryStatisticInfo.LONG_COUNT + "期所有位数字0-9的出现概率", tagFont, tagBrush, 5, 5);
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
                else if( lv == 0)
                    moneyLvLinePen.Color = Color.Gray;
                else if (lv == 1)
                    moneyLvLinePen.Color = Color.Orange;
                else if (lv > 1)
                    moneyLvLinePen.Color = Color.White;
                else
                    moneyLvLinePen.Color = Color.Cyan;
                g.DrawLine(moneyLvLinePen, 0, relY, winW, relY);
                if(isMouseAtRight)
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
            for(int i = startIndex; i < endIndex; ++i)
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
                        "] [最低:" + tdm.minValue + "]\n";
#if TRADE_DBG
                    info += tdb.GetDbgInfo();
#endif
                    g.DrawString(info, tipsFont, whiteBrush, 5, 5);
                }

                if(selectTradeIndex == i)
                {
                    g.DrawLine(whiteLinePen, cx - halfGridW, 0, cx - halfGridW, winH);
                    g.DrawLine(whiteLinePen, cx + halfGridW, 0, cx + halfGridW, winH);
                }

                if(i == TradeDataManager.Instance.historyTradeDatas.Count - 1)
                {
                    g.DrawLine(whiteLinePen, cx, cy, winW, cy);
                    //if(isMouseAtRight)
                    if(tdb.moneyAtferTrade <= 0)
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
            if(tdm.waitingTradeDatas.Count > 0)
            {
                float y = winH - 2 * gridScaleDown.X;

                for (int i = 0; i < tdm.waitingTradeDatas.Count; ++i)
                {
                    float x = i * gridScaleDown.X;
                    g.DrawRectangle(whiteLinePen, x, y, gridScaleDown.X, gridScaleDown.X);

                    if(selID == -1 && mouseRelPos.X >= x && mouseRelPos.X <= x + gridScaleDown.X)
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

    // 出号率曲线图
    class GraphAppearence : GraphBase
    {
        public bool autoAllign = false;
        public bool onlyShowSelectCDTLine = true;
        public Dictionary<CollectDataType, bool> cdtLineShowStates = new Dictionary<CollectDataType, bool>();

        protected SolidBrush redBrush = new SolidBrush(Color.Red);
        protected SolidBrush tagBrush = new SolidBrush(Color.White);
        protected SolidBrush greenBrush = new SolidBrush(Color.Green);
        protected Pen grayDotLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);
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
        }
        public static string[] AppearenceTypeStrs = new string[]
        {
            "统计5期的出号率",
            "统计10期的出号率",
            "统计30期的出号率",
            "统计5期的出号个数",
            "统计10期的出号个数",
            "统计30期的出号个数",
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

            for( int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i )
            {
                cdtLineShowStates.Add(GraphDataManager.S_CDT_LIST[i], false);
            }
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
                Pen pen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, col, 1);
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
            if (halfSize < 4)
                halfSize = 4;
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
                DrawSingleCDTLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH, mouseRelPos);

                float tp = GraphDataManager.GetTheoryProbability(cdt);
                float tY = bottom - (bottom - top) * tp / 100;
                g.DrawLine(GetLinePen(cdt), 0, tY, winW, tY);
            }
            else
            {
                var etor = cdtLineShowStates.GetEnumerator();
                while(etor.MoveNext())
                {
                    CollectDataType type = etor.Current.Key;
                    if(etor.Current.Value)
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
                if(appearenceType < AppearenceType.eAppearCountFast)
                    v = (bottom - mouseRelPos.Y) / (bottom - top) * 100.0f;
                else if(appearenceType == AppearenceType.eAppearCountFast)
                    v = (bottom - mouseRelPos.Y) / (bottom - top) * LotteryStatisticInfo.FAST_COUNT;
                else if (appearenceType == AppearenceType.eAppearCountShort)
                    v = (bottom - mouseRelPos.Y) / (bottom - top) * LotteryStatisticInfo.SHOR_COUNT;
                else if (appearenceType == AppearenceType.eAppearCountLong)
                    v = (bottom - mouseRelPos.Y) / (bottom - top) * LotteryStatisticInfo.LONG_COUNT;
                g.DrawString(v.ToString("f2") + "%", tagFont, tagBrush, winW - 60, mouseRelPos.Y );
            }

            if (hoverItem != null)
            {
                StatisticUnit su = hoverItem.statisticInfo.allStatisticInfo[numIndex].statisticUnitMap[cdt];
                float apprate = 0;
                int underTheoRateCount = 0;
                switch(appearenceType)
                {
                    case AppearenceType.eAppearCountFast:
                        apprate = (su.fastData.appearCount * 100 / LotteryStatisticInfo.FAST_COUNT);
                        underTheoRateCount = su.fastData.underTheoryCount;
                        break;
                    case AppearenceType.eAppearCountShort:
                        apprate = (su.shortData.appearCount * 100 / LotteryStatisticInfo.SHOR_COUNT);
                        underTheoRateCount = su.shortData.underTheoryCount;
                        break;
                    case AppearenceType.eAppearCountLong:
                        apprate = (su.longData.appearCount * 100 / LotteryStatisticInfo.LONG_COUNT);
                        underTheoRateCount = su.longData.underTheoryCount;
                        break;
                    case AppearenceType.eAppearenceFast:
                        apprate = su.fastData.appearProbability;
                        underTheoRateCount = su.fastData.underTheoryCount;
                        break;
                    case AppearenceType.eAppearenceShort:
                        apprate = su.shortData.appearProbability;
                        underTheoRateCount = su.shortData.underTheoryCount;
                        break;
                    case AppearenceType.eAppearenceLong:
                        apprate = su.longData.appearProbability;
                        underTheoRateCount = su.longData.underTheoryCount;
                        break;
                }
                string info = "[" + hoverItem.idTag + "] [" + hoverItem.lotteryNumber + "] [出号率 = " + apprate + "] [连续低于理论概率期数 = " + underTheoRateCount + "]";
                g.DrawString(info, tagFont, tagBrush, 5, 5);
            }

            if(selectDataIndex != -1)
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
                switch(appearenceType)
                {
                    case AppearenceType.eAppearenceFast:
                        apr = sum.statisticUnitMap[cdt].fastData.appearProbability;
                        break;
                    case AppearenceType.eAppearenceShort:
                        apr = sum.statisticUnitMap[cdt].shortData.appearProbability;
                        break;
                    case AppearenceType.eAppearenceLong:
                        apr = sum.statisticUnitMap[cdt].longData.appearProbability;
                        break;
                    case AppearenceType.eAppearCountFast:
                        apr = sum.statisticUnitMap[cdt].fastData.appearCount * 100 / LotteryStatisticInfo.FAST_COUNT;
                        break;
                    case AppearenceType.eAppearCountShort:
                        apr = sum.statisticUnitMap[cdt].shortData.appearCount * 100 / LotteryStatisticInfo.SHOR_COUNT;
                        break;
                    case AppearenceType.eAppearCountLong:
                        apr = sum.statisticUnitMap[cdt].longData.appearCount * 100 / LotteryStatisticInfo.LONG_COUNT;
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

    class GraphMissCount : GraphAppearence
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
        }
        public static string[] MissCountTypeStrs = new string[]
        {
            "遗漏值",
            "统计5期的遗漏面积",
            "统计10期的遗漏面积",
            "统计30期的遗漏面积",
            "统计5期的遗漏数",
            "统计10期的遗漏数",
            "统计30期的遗漏数",
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
            if (halfSize < 4)
                halfSize = 4;
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
                DrawSingleMissCountLine(g, numIndex, startIndex, endIndex, cdt, ref hasChoose, bottom, maxHeight, halfGridW, halfSize, fullSize, winH, mouseRelPos);

                //float tp = GraphDataManager.GetTheoryProbability(cdt);
                //float tY = bottom - (bottom - top) * tp / 100;
                //g.DrawLine(GetLinePen(cdt), 0, tY, winW, tY);
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
                g.DrawString(mouseMissCount.ToString("f1"), tagFont, tagBrush, winW - 60, mouseRelPos.Y);
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
                g.DrawString(info, tagFont, tagBrush, 5, 5);
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
                else if(missCountType == MissCountType.eDisappearCountFast)
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
        public GraphAppearence appearenceGraph;
        public GraphMissCount missCountGraph;
        public int endShowDataItemIndex = -1;
        LotteryGraph window;

        public GraphManager(LotteryGraph _window)
        {
            window = _window;
            barGraph = new GraphBar(); barGraph.parent = this;
            kvalueGraph = new GraphKCurve(); kvalueGraph.parent = this;
            tradeGraph = new GraphTrade(); tradeGraph.parent = this;
            appearenceGraph = new GraphAppearence(); appearenceGraph.parent = this;
            missCountGraph = new GraphMissCount(); missCountGraph.parent = this;
            sGraphMap.Add(GraphType.eKCurveGraph, kvalueGraph);
            sGraphMap.Add(GraphType.eBarGraph, barGraph);
            sGraphMap.Add(GraphType.eTradeGraph, tradeGraph);
            sGraphMap.Add(GraphType.eAppearenceGraph, appearenceGraph);
            sGraphMap.Add(GraphType.eMissCountGraph, missCountGraph);
        }

        public void MakeWindowRepaint()
        {
            window.Invalidate(true);
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
        public void RemoveFavoriteChart(string tag)
        {
            for( int i = 0; i < favoriteCharts.Count; ++i )
            {
                if(favoriteCharts[i].tag == tag)
                {
                    favoriteCharts.RemoveAt(i);
                    return;
                }
            }
        }
    }

#endregion
}