using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

#if WIN
using System.Windows.Forms;

namespace LotteryAnalyze
{
    // 箭头
    public class AuxiliaryLineArrow : AuxiliaryLineBase
    {
        public const int C_LINE_WIDTH = 1;
        public static Color sOriLineColor = Color.GreenYellow;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, C_LINE_WIDTH);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, C_LINE_WIDTH);
        public AuxiliaryLineArrow()
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
}
#endif