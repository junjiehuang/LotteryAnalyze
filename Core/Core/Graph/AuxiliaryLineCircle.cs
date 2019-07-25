using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    // 测试圆线
    public class AuxiliaryLineCircle : AuxiliaryLineBase
    {
        public float x, y, size;
        public static Color sOriLineColor = Color.White;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, 2);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, 1);
        public AuxiliaryLineCircle()
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
}
