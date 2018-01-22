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

        public GraphKCurve()
        {
            selDataFont = new Font(FontFamily.GenericSerif, 16);
        }

        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            if (GraphDataManager.Instance.HasData(GraphType.eKCurveGraph) == false)
                return false;
            int curIndex = (int)(mousePos.X / gridScaleW);
            if (curIndex == selDataIndex)
                return false;
            return true;
        }

        public override bool MoveLeftRight(bool left)
        {
            if (left && startDataIndex < (GraphDataManager.Instance.DataLength(GraphType.eKCurveGraph) - 1))
            {
                ++startDataIndex;
                return true;
            }
            else if (startDataIndex > 0)
            {
                --startDataIndex;
                return true;
            }
            return false;
        }

        public override void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            int baseY = winH / 2;
            g.DrawLine(GraphUtil.GetSolidPen(Color.White), new Point(0, baseY), new Point(winW, baseY));

            int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
            float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];

            selDataIndex = -1;
            lastDataHitValue = 0;
            lastDataMissValue = 0;
            startDataHitValue = 0;
            startDataMissValue = 0;
            KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
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

                foreach( AvgDataContainer adc in kddc.avgDataContMap.Values)
                {
                    if(adc.avgLineSetting.enable)
                    {
                        lastDataHitValue = 0;
                        lastDataMissValue = 0;
                        startDataHitValue = 0;
                        startDataMissValue = 0;
                        findPrevPt = false;
                        for ( int i = 0; i < adc.avgPointMapLst.Count; ++i )
                        {
                            DrawAvgLine(g, adc.avgPointMapLst[i], winW, winH, missRelHeight, cdt, adc);
                        }
                    }
                }
            }
        }

        void DrawData(Graphics g, KData data, int winW, int winH, float missRelHeight, Point mouseRelPos)
        {
            if (data.index < startDataIndex)
            {
                lastDataHitValue += data.HitValue;
                lastDataMissValue += data.MissValue;
                startDataHitValue += data.HitValue;
                startDataMissValue += data.MissValue;
                return;
            }

            float baseX = 0;
            float baseY = winH / 2;
            baseX += (data.index - startDataIndex) * gridScaleW;
            baseY -= (lastDataHitValue - startDataHitValue) * gridScaleH;
            baseY += (lastDataMissValue - startDataMissValue) * gridScaleH * missRelHeight;
            int upBound = (int)(baseY - data.HitValue * gridScaleH);
            int downBound = (int)(baseY + data.MissValue * gridScaleH * missRelHeight);
            float valueChange = data.HitValue - data.MissValue;
            
            Color col = valueChange > 0 ? Color.Red : (valueChange < 0 ? Color.Cyan : Color.White);
            Pen linePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, col, 1);
            Pen rcPen = GraphUtil.GetSolidPen(col);
            int midX = (int)(baseX + gridScaleW * 0.5f);
            Point p0 = new Point(midX, upBound);
            Point p1 = new Point(midX, downBound);
            g.DrawLine(linePen, p0, p1);
            float rcY = valueChange > 0 ? (baseY - valueChange * gridScaleH) : baseY;
            float height = (int)Math.Abs(valueChange * gridScaleH);
            if (valueChange < 0)
                height *= missRelHeight;
            if (height < 1)
                height = 1;
            g.FillRectangle(new SolidBrush(col), baseX, rcY, gridScaleW, height);
            lastDataHitValue += data.HitValue;
            lastDataMissValue += data.MissValue;

            if (selDataIndex < 0 && baseX <= mouseRelPos.X && baseX + gridScaleW >= mouseRelPos.X)
            {
                selDataIndex = data.index;
                Pen selPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);
                g.DrawLine(selPen, baseX, 0, baseX, winH);
                g.DrawLine(selPen, baseX + gridScaleW, 0, baseX + gridScaleW, winH);
                g.DrawString(data.GetInfo(), selDataFont, new SolidBrush(Color.White), 0, 0);
            }
        }
     
        void DrawAvgLine(Graphics g, AvgPointMap apm, int winW, int winH, float missRelHeight, CollectDataType cdt, AvgDataContainer adc)
        {
            AvgPoint ap = apm.apMap[cdt];
            if (apm.index < startDataIndex)
            {
                lastDataHitValue += ap.avgHit;
                lastDataMissValue += ap.avgMiss;
                startDataHitValue += ap.avgHit;
                startDataMissValue += ap.avgMiss;
                return;
            }
            float baseX = 0;
            float baseY = winH / 2;
            baseX += (apm.index - startDataIndex) * gridScaleW;
            baseY -= (lastDataHitValue - startDataHitValue) * gridScaleH;
            baseY += (lastDataMissValue - startDataMissValue) * gridScaleH * missRelHeight;
            float avgPtH = baseY - ap.avgHit * gridScaleH + ap.avgMiss * gridScaleH * missRelHeight;
            lastDataHitValue += ap.avgHit;
            lastDataMissValue += ap.avgMiss;
            if(findPrevPt == false)
            {
                prevPt.X = baseX + gridScaleW * 0.5f;
                prevPt.Y = avgPtH;
                findPrevPt = true;
            }
            else
            {
                Pen pen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, adc.avgLineSetting.color, 1);
                g.DrawLine(pen, prevPt.X, prevPt.Y, prevPt.X = baseX + gridScaleW * 0.5f, prevPt.Y = avgPtH);
            }
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
        public void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            if (curGraph != null)
                curGraph.DrawGraph(g, numIndex, cdt, winW, winH, mouseRelPos);
        }
    }

#endregion
}