using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    // 测试矩形
    public class AuxiliaryLineRect : AuxiliaryLineBase
    {
        public const int C_LINE_WIDTH = 5;
        public static Color sOriLineColor = Color.GreenYellow;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, C_LINE_WIDTH);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, C_LINE_WIDTH);
        public AuxiliaryLineRect()
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

}
