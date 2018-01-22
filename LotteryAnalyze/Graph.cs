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
    }

    // 图表基类
    class GraphBase
    {
        public virtual bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return false;
        }
        public virtual bool MoveLeftRight(bool left)
        {
            return false;
        }
        public virtual bool MoveUpDown(bool up)
        {
            return false;
        }
        public virtual void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {

        }
    }

    // K线图
    class GraphKCurve : GraphBase
    {
        float gridScaleH = 20;
        float gridScaleW = 10;

        float originX = 0;
        float originY = 0;

        int startDataIndex = 0;
        float startDataHitValue = 0;
        float startDataMissValue = 0;
        float lastDataHitValue = 0;
        float lastDataMissValue = 0;

        int selDataIndex = -1;

        Font selDataFont;
        PointF prevPt = new PointF();
        bool findPrevPt = false;

        float lastValue = 0;
        PointF canvasOffset = new PointF(0, 0);

        Pen grayDotLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);
        Pen redLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 1);
        Pen cyanLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Cyan, 1);
        Pen whiteLinePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.White, 1);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush cyanBrush = new SolidBrush(Color.Cyan);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush tmpBrush;

        Dictionary<Pen, List<PointF>> linePools = new Dictionary<Pen, List<PointF>>();
        Dictionary<SolidBrush, List<RectangleF>> rcPools = new Dictionary<SolidBrush,List<RectangleF>>();
        //List<PointF> linePts = new List<PointF>();

        public GraphKCurve()
        {
            selDataFont = new Font(FontFamily.GenericSerif, 16);
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

        public override bool MoveLeftRight(bool left)
        {
            //if (left && startDataIndex < (GraphDataManager.Instance.DataLength(GraphType.eKCurveGraph) - 1))
            //{
            //    ++startDataIndex;
            //    return true;
            //}
            //else if (startDataIndex > 0)
            //{
            //    --startDataIndex;
            //    return true;
            //}
            //return false;
            if (left && canvasOffset.X > 0)
            {
                canvasOffset.X -= gridScaleW;
                return true;
            }
            else if (canvasOffset.X < GraphDataManager.KGDC.DataLength() * gridScaleW)
            {
                canvasOffset.X += gridScaleW;
                return true;
            }
            return false;
        }
        public override bool MoveUpDown(bool up)
        {
            if (up)
            {
                canvasOffset.Y += gridScaleH;
                return true;
            }
            else
            {
                canvasOffset.Y -= gridScaleH;
                return true;
            }
            return false;
        }


        public override void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            foreach(Pen k in linePools.Keys)
            {
                linePools[k].Clear();
            }
            foreach(SolidBrush k in rcPools.Keys)
            {
                rcPools[k].Clear();
            }

            selDataIndex = -1;
            lastValue = 0;
            if (canvasOffset.Y == 0 && canvasOffset.X == 0)
                canvasOffset.Y = winH * 0.5f;
            KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
            int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
            float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];
            if (kddc != null)
            {
                for (int i = 0; i < kddc.dataLst.Count; ++i)
                {
                    KDataDict kdDict = kddc.dataLst[i];
                    KData data = kdDict.GetData(cdt, false);
                    if (data == null)
                        continue;
                    DrawData(g, data, winW, winH, missRelHeight, mouseRelPos);
                }

                foreach (AvgDataContainer adc in kddc.avgDataContMap.Values)
                {
                    if (adc.avgLineSetting.enable)
                    {
                        Pen pen = adc.avgLineSetting.pen;
                        GetLineLst(pen);

                        lastValue = 0;
                        findPrevPt = false;
                        for (int i = 0; i < adc.avgPointMapLst.Count; ++i)
                        {
                            DrawAvgLine(g, adc.avgPointMapLst[i], winW, winH, cdt, adc.avgLineSetting.pen);
                        }
                    }
                }
            }
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

        float StandToCanvas(float v, bool isX)
        {
            if (isX)
                return v - canvasOffset.X;
            else
                return canvasOffset.Y - v;
        }

        void DrawData(Graphics g, KData data, int winW, int winH, float missRelHeight, Point mouseRelPos)
        {
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
                selDataIndex = data.index;
                g.DrawLine(grayDotLinePen, standX, 0, standX, winH);
                g.DrawLine(grayDotLinePen, standX + gridScaleW, 0, standX + gridScaleW, winH);
                //PushLinePts(grayDotLinePen, standX, 0, standX, winH);
                //PushLinePts(grayDotLinePen, standX + gridScaleW, 0, standX + gridScaleW, winH);
                g.DrawString(data.GetInfo(), selDataFont, whiteBrush, 0, 0);
            }
        }

        void DrawAvgLine(Graphics g, AvgPointMap apm, int winW, int winH, CollectDataType cdt, Pen pen)
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
            //linePools[pen].Add(new PointF(px, py));
            //linePools[pen].Add(new PointF(cx, cy));
            //g.DrawLine(pen, px, py, cx, cy);
        }
    }

    // 柱状图
    class GraphBar : GraphBase
    {
        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return false;
        }
        public override bool MoveLeftRight(bool left)
        {
            return false;
        }
        public override void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
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
            SolidBrush brush = new SolidBrush(Color.Red);
            SolidBrush tagBrush = new SolidBrush(Color.White);
            Font tagFont = new Font(FontFamily.GenericSerif, 16);
            for (int i = 0; i < dul.dataLst.Count; ++i)
            {
                float rcH = MaxRcH * ((float)(dul.dataLst[i].data) / (float)bgdc.totalCollectCount);
                startY = bottom - rcH;
                g.FillRectangle(brush, startX, startY, gap * 0.9f, rcH);
                g.DrawString(dul.dataLst[i].tag, tagFont, tagBrush, startX, bottom);
                g.DrawString(dul.dataLst[i].data.ToString(), tagFont, tagBrush, startX, startY - 30);
                startX += gap;
            }
        }
    }

    // 图表管理器
    class GraphManager
    {
        Dictionary<GraphType, GraphBase> sGraphMap = new Dictionary<GraphType, GraphBase>();
        GraphBase curGraph = null;
        GraphType curGraphType = GraphType.eNone;
        public GraphManager()
        {
            sGraphMap.Add(GraphType.eKCurveGraph, new GraphKCurve());
            sGraphMap.Add(GraphType.eBarGraph, new GraphBar());
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
        public bool MoveLeftRight(bool left)
        {
            if (curGraph != null)
                return curGraph.MoveLeftRight(left);
            return false;
        }
        public bool MoveUpDown(bool up)
        {
            if (curGraph != null)
                return curGraph.MoveUpDown(up);
            return false;
        }
        public void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            if (curGraph != null)
                curGraph.DrawGraph(g, numIndex, cdt, winW, winH, mouseRelPos);
        }
    }

#endregion
}