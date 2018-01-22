using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    class GraphUtil
    {

        public static Pen GetLinePen(System.Drawing.Drawing2D.DashStyle dashStyle, Color color, int width )
        {
            Pen sLinePen = new Pen(color);
            sLinePen.Color = color;
            sLinePen.DashStyle = dashStyle;
            sLinePen.Width = width;
            return sLinePen;
        }

        public static Pen GetSolidPen(Color color)
        {
            Pen sSolidPen = new Pen(color);    
            sSolidPen.Brush = new SolidBrush(color);
            return sSolidPen;
        }
    }
}
