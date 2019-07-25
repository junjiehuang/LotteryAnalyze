using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{

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


    // 图表基类
    public class GraphBase
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

        public PointF CanvasToValue(Point pt, bool upPanel)
        {
            Point sv = CanvasToStand(pt, upPanel);
            return StandToValue(sv, upPanel);
        }
        public PointF StandToValue(Point sv, bool upPanel)
        {
            PointF ret = new PointF();
            if (upPanel)
            {
                ret.X = sv.X / gridScaleUp.X;
                ret.Y = sv.Y / gridScaleUp.Y;
            }
            else
            {
                ret.X = sv.X / gridScaleDown.X;
                ret.Y = sv.Y / gridScaleDown.Y;
            }
            return ret;
        }

        public Point ValueToCanvas(PointF vPt, bool upPanel)
        {
            Point ret = ValueToStand(vPt, upPanel);
            return StandToCanvas(ret, upPanel);
        }

        public Point ValueToStand(PointF vPt, bool upPanel)
        {
            Point ret = new Point();
            if (upPanel)
            {
                ret.X = (int)(vPt.X * gridScaleUp.X);
                ret.Y = (int)(vPt.Y * gridScaleUp.Y);
            }
            else
            {
                ret.X = (int)(vPt.X * gridScaleDown.X);
                ret.Y = (int)(vPt.Y * gridScaleDown.Y);
            }
            return ret;
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
        public virtual void ScrollToData(int index, int winW, int winH, bool needSelect, int xOffset = 0, bool needScrollToData = true) { }

        public virtual void OnGridScaleChanged() { }
    }
}
