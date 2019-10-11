using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

#if WIN
using System.Windows.Forms;

namespace LotteryAnalyze
{

    public enum AuxLineType
    {
        eNone,
        // 水平线
        eHorzLine,
        // 垂直线
        eVertLine,
        // 任意直线
        eSingleLine,
        // 通道线
        eChannelLine,
        // 黄金分割线
        eGoldSegmentedLine,
        // 测试圆
        eCircleLine,
        // 箭头线
        eArrowLine,
        // 测试矩形
        eRectLine,
    }

    // 辅助线基类
    public class AuxiliaryLineBase
    {
        public int numIndex = -1;
        public CollectDataType cdt = CollectDataType.eNone;
        public AuxLineType lineType = AuxLineType.eNone;
        public List<Point> keyPoints = new List<Point>();
        public List<PointF> valuePoints = new List<PointF>();
        protected Pen solidPen = null;
        protected Pen dotPen = null;

        public virtual Pen GetSolidPen() { return null; }
        public virtual Pen GetDotPen() { return null; }
        public virtual void SetColor(Color col) { }
        public virtual bool HitTest(CollectDataType cdt, int numIndex, Point standMousePos, float rcHalfSize, ref int selKeyPtIndex)
        {
            selKeyPtIndex = -1;
            if (this.cdt == cdt && this.numIndex == numIndex)
            {
                for (int j = 0; j < this.keyPoints.Count; ++j)
                {
                    Point pt = this.keyPoints[j];
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
            }
            return false;
        }
    }
}
#endif