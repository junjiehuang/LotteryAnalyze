using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

#if WIN
using System.Windows.Forms;
#endif

namespace LotteryAnalyze
{
    // 水平线
    public class AuxiliaryLineHorz : AuxiliaryLineBase
    {

        public AuxiliaryLineHorz()
        {
            lineType = AuxLineType.eHorzLine;
            color = SystemColor2UnityColor(Color.Aqua);
        }

#if WIN
        public static Color sOriLineColor = Color.Aqua;
        public static Pen sOriSolidPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, sOriLineColor, 2);
        public static Pen sOriDotPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, sOriLineColor, 1);

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
#endif
    }
}