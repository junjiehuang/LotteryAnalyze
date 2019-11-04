using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WIN
using System.Drawing;
using System.Windows.Forms;
#else
using UnityEngine;
#endif

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

        public bool selected = false;
        public int selectedKeyID = -1;

#if WIN
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
#else
        public Color color = Color.white;
        public List<Vector2> keyPoints = new List<Vector2>();
        public List<Vector2> valuePoints = new List<Vector2>();


        public static UnityEngine.Color SystemColor2UnityColor(System.Drawing.Color col)
        {
            UnityEngine.Color _col = new Color(col.R / 255.0f, col.G / 255.0f, col.B / 255.0f, col.A / 255.0f);
            return _col;
        }

        public virtual bool HitTest(CollectDataType cdt, int numIndex, Vector2 standMousePos, float rcHalfSize, ref int selKeyPtIndex)
        {
            selKeyPtIndex = -1;
            if (this.cdt == cdt && this.numIndex == numIndex)
            {
                for (int j = 0; j < this.keyPoints.Count; ++j)
                {
                    Vector2 pt = this.keyPoints[j];
                    if (pt.x - rcHalfSize > standMousePos.x ||
                        pt.x + rcHalfSize < standMousePos.x ||
                        pt.y - rcHalfSize > standMousePos.y ||
                        pt.y + rcHalfSize < standMousePos.y)
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
#endif
    }
}