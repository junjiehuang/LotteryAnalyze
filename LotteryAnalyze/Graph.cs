using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public enum CollectDataType
    {
        eNone,

        ePath0,
        ePath1,
        ePath2,

        eMax,
    }

    class KData
    {
        public int index;
        public float HitValue;
        public float MissValue;
    }

    class KDataDict
    {
        public int index;
        public Dictionary<CollectDataType, KData> dataDict = new Dictionary<CollectDataType, KData>();

        public KData GetData(CollectDataType collectDataType, bool createIfNotExist)
        {
            if (dataDict.ContainsKey(collectDataType))
                return dataDict[collectDataType];
            else if (createIfNotExist == false)
                return null;
            KData data = new KData();
            dataDict.Add(collectDataType, data);
            return data;
        }

        public void Clear()
        {
            dataDict.Clear();
        }
    }

    class Graph
    {
        float gridScaleH = 5;
        float gridScaleW = 10;

        float originX = 0;
        float originY = 0;

        List<KDataDict> kdLst = new List<KDataDict>();
        int startDataIndex = 0;
        float lastValue = 0;

        public Graph()
        {

        }

        public void Destroy()
        {
            kdLst.Clear();
        }

        public void DrawGraph(Graphics g, CollectDataType cdt, int winW, int winH)
        {
            int baseX = 0;
            int baseY = winH / 2;
            g.DrawLine(GraphUtil.GetSolidPen(Color.White), new Point(0, baseY), new Point(winW, baseY));

            lastValue = 0;
            for ( int i = 0; i < kdLst.Count; ++i )
            {
                KDataDict kdDict = kdLst[i];
                KData data = kdDict.GetData(cdt, false);
                if (data == null)
                    continue;
                DrawData(g, data, winW, winH);
            }
        }

        void DrawData(Graphics g, KData data, int winW, int winH)
        {
            if (data.index < startDataIndex)
                return;
            float baseX = 0;
            float baseY = winH / 2;
            baseX += (data.index - startDataIndex) * gridScaleW;
            baseY -= lastValue * gridScaleH;
            int up = (int)(baseY - data.HitValue * gridScaleH);
            int down = (int)(baseY + data.MissValue * gridScaleH);
            float valueChange = data.HitValue - data.MissValue;
            Color col = valueChange > 0 ? Color.Red : (valueChange < 0 ? Color.Blue : Color.White);
            Pen linePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, col, 1);
            Pen rcPen = GraphUtil.GetSolidPen(col);
            int midX = (int)(baseX + gridScaleW * 0.5f);
            Point p0 = new Point(midX, up);
            Point p1 = new Point(midX, down);
            g.DrawLine(linePen, p0, p1);
            linePen.Width = gridScaleW;
            float rcY = valueChange > 0 ? (baseY - valueChange * gridScaleH) : baseY;
            int height = (int)Math.Abs(valueChange * gridScaleH);
            if (height < 1)
                height = 1;
            g.FillRectangle(new SolidBrush(col), baseX, rcY, gridScaleW, height);
            lastValue += valueChange;
        }

        public void CollectKDatas(int NUM_INDEX, CollectDataType collectDataType, int cycleLength = 5)
        {
            kdLst.Clear();

            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;

            int loop = 0;
            KDataDict curData = CreateKDataDict();

            for (int i = 0; i < count; ++i)
            {
                int oneDayID = DataManager.GetInst().indexs[i];
                OneDayDatas odd = DataManager.GetInst().allDatas[oneDayID];

                for (int j = 0; j < odd.datas.Count; ++j)
                {
                    DataItem item = odd.datas[j];
                    CollectItem(NUM_INDEX, item, curData, collectDataType);
                    ++loop;
                    if (loop == cycleLength)
                    {
                        curData = CreateKDataDict();
                        loop = 0;
                    }
                }
            }
        }

        void CollectItem(int NUM_INDEX, DataItem item, KDataDict kd, CollectDataType collectDataType)
        {
            CollectDataType cdt = collectDataType;//CollectDataType.eNone;
            //for (; cdt < CollectDataType.eMax; ++cdt)
            {
                //if ((collectDataType & cdt) > 0)
                {
                    KData data = kd.GetData(cdt, true);
                    data.index = kd.index;
                    switch (cdt)
                    {
                        case CollectDataType.ePath0:
                            if (item.path012OfEachSingle[NUM_INDEX] == 0)
                                data.HitValue++;
                            else
                                data.MissValue++;
                            break;
                        case CollectDataType.ePath1:
                            if (item.path012OfEachSingle[NUM_INDEX] == 1)
                                data.HitValue++;
                            else
                                data.MissValue++;
                            break;
                        case CollectDataType.ePath2:
                            if (item.path012OfEachSingle[NUM_INDEX] == 2)
                                data.HitValue++;
                            else
                                data.MissValue++;
                            break;
                    }
                }
            }
        }

        KDataDict CreateKDataDict()
        {
            KDataDict curData = new KDataDict();
            curData.index = kdLst.Count;
            kdLst.Add(curData);
            return curData;
        }


    }
}