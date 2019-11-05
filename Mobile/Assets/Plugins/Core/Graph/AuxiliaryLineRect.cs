using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

#if WIN
using System.Windows.Forms;
#else
using UnityEngine;
#endif

namespace LotteryAnalyze
{
    // 测试矩形
    public class AuxiliaryLineRect : AuxiliaryLineBase
    {
        public const int C_LINE_WIDTH = 5;
        public AuxiliaryLineRect()
        {
            lineType = AuxLineType.eRectLine;
            color = SystemColor2UnityColor(System.Drawing.Color.GreenYellow);
        }

#if WIN
        public static Color sOriLineColor = Color.GreenYellow;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, C_LINE_WIDTH);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, C_LINE_WIDTH);

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
#else
        public override bool HitTest(CollectDataType cdt, int numIndex, Vector2 standMousePos, float rcHalfSize, float rcHalfSizeSel, ref int selKeyPtIndex)
        {
            //selKeyPtIndex = -1;
            if (this.cdt == cdt && this.numIndex == numIndex)
            {
                float minx = this.keyPoints[0].x;
                float maxx = this.keyPoints[0].x;
                float miny = this.keyPoints[0].y;
                float maxy = this.keyPoints[0].y;

                for (int j = 0; j < this.keyPoints.Count; ++j)
                {
                    float testSize = selKeyPtIndex == j ? rcHalfSizeSel : rcHalfSize;
                    Vector2 pt = this.keyPoints[j];
                    if (pt.x < minx)
                        minx = pt.x;
                    if (pt.x > maxx)
                        maxx = pt.x;
                    if (pt.y < miny)
                        miny = pt.y;
                    if (pt.y > maxy)
                        maxy = pt.y;

                    if (pt.x - testSize > standMousePos.x ||
                        pt.x + testSize < standMousePos.x ||
                        pt.y - testSize > standMousePos.y ||
                        pt.y + testSize < standMousePos.y)
                    {
                        continue;
                    }
                    else
                    {
                        selKeyPtIndex = j;
                        return true;
                    }
                }

                if (standMousePos.x > minx && standMousePos.x < maxx &&
                    standMousePos.y > miny && standMousePos.y < maxy)
                {
                    return true;
                }
            }
            selKeyPtIndex = -1;
            return false;
        }
#endif
    }
}