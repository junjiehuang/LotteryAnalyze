using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    class GraphUtil
    {
        static Pen sLinePen = null;
        static Pen sSolidPen = null;

        public static Pen GetLinePen(System.Drawing.Drawing2D.DashStyle dashStyle, Color color, int width )
        {
            if (sLinePen == null)
                sLinePen = new Pen(color);
            sLinePen.Brush = null;
            sLinePen.Color = color;
            sLinePen.DashStyle = dashStyle;
            sLinePen.Width = width;
            return sLinePen;
        }

        public static Pen GetSolidPen(Color color)
        {
            if (sSolidPen == null)
                sSolidPen = new Pen(color);    
            sSolidPen.Brush = new SolidBrush(color);
            return sSolidPen;
        }
    }
}
