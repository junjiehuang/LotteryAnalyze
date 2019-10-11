using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

#if WIN
using System.Windows.Forms;

namespace LotteryAnalyze
{
    // 通道直线
    public class AuxiliaryLineSupportPressure : AuxiliaryLineBase
    {
        public static Color sOriLineColor = Color.Blue;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, 2);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, 1);

        public AuxiliaryLineSupportPressure()
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
}
#endif