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

        ePath0 = 1 << 0,
        ePath1 = 1 << 1,
        ePath2 = 1 << 2,

        eMax = 0xFFFFFFF,
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

    class KDataDictContainer
    {
        public List<KDataDict> dataLst = new List<KDataDict>();

        public KDataDict CreateKDataDict()
        {
            KDataDict kdd = new KDataDict();
            kdd.index = dataLst.Count;
            dataLst.Add(kdd);
            return kdd;
        }
    }

    class GraphDataManager
    {
        public static CollectDataType[] S_CDTS = new CollectDataType[]
        {
            CollectDataType.ePath0,
            CollectDataType.ePath1,
            CollectDataType.ePath2,
        };
        public static string[] S_CDTSTRS = new string[]
        {
            "0路",
            "1路",
            "2路",
        };

        static GraphDataManager sInst = null;
        public static GraphDataManager Instance
        {
            get
            {
                if (sInst == null) sInst = new GraphDataManager();
                return sInst;
            }
        }

        int dataLength = 0;
        int cycleLength = 5;
        List<KDataDictContainer> allDatas = new List<KDataDictContainer>();

        public int CycleLength
        {
            get { return cycleLength; }
            set { cycleLength = value; }
        }
        public int DataLength
        {
            get
            {
                return dataLength;
            }
        }

        public void Clear()
        {
            allDatas.Clear();
        }

        public KDataDictContainer GetKDataDictContainer( int numIndex )
        {
            if(allDatas.Count > numIndex)
                return allDatas[numIndex];
            return null;
        }

        public void Init()
        {
            KDataDictContainer kddc = null;
            for ( int i = 0; i < 5; ++i )
            {
                if (i >= allDatas.Count)
                {
                    kddc = new KDataDictContainer();
                    allDatas.Add(kddc);
                }
                else
                {
                    kddc = allDatas[i];
                    kddc.dataLst.Clear();
                }
            }
        }

        public void CollectGraphData()
        {
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            Init();

            int loop = 0;
            KDataDict[] curDatas = CreateKDataDicts();

            for (int i = 0; i < count; ++i)
            {
                int oneDayID = DataManager.GetInst().indexs[i];
                OneDayDatas odd = DataManager.GetInst().allDatas[oneDayID];

                for (int j = 0; j < odd.datas.Count; ++j)
                {
                    DataItem item = odd.datas[j];
                    for (int numIndex = 0; numIndex < 5; ++numIndex)
                    {
                        CollectItem(numIndex, item, curDatas[numIndex]);
                    }

                    ++loop;
                    if (loop == cycleLength)
                    {
                        curDatas = CreateKDataDicts();
                        loop = 0;
                    }
                }
            }

            dataLength = allDatas[0].dataLst.Count;
        }

        void CollectItem(int NUM_INDEX, DataItem item, KDataDict kd)
        {
            for (int i = 0; i < S_CDTS.Length; ++i)
            {
                CollectDataType cdt = S_CDTS[i];
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

        KDataDict[] CreateKDataDicts()
        {
            KDataDict[] curDatas = new KDataDict[5];
            for (int i = 0; i < 5; ++i)
            {
                curDatas[i] = allDatas[i].CreateKDataDict();
            }
            return curDatas;
        }
    }

    class Graph
    {
        float gridScaleH = 5;
        float gridScaleW = 10;

        float originX = 0;
        float originY = 0;

        int startDataIndex = 0;
        float startDataValue = 0;        
        float lastValue = 0;

        public Graph()
        {

        }

        public bool MoveLeftRight( bool left )
        {
            if (left && startDataIndex < GraphDataManager.Instance.DataLength - 1)
            {
                ++startDataIndex;
                return true;
            }
            else if (startDataIndex > 0)
            {
                --startDataIndex;
                return true;
            }
            return false;
        }
        
        public void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH)
        {
            int baseX = 0;
            int baseY = winH / 2;
            g.DrawLine(GraphUtil.GetSolidPen(Color.White), new Point(0, baseY), new Point(winW, baseY));

            lastValue = 0;
            startDataValue = 0;
            KDataDictContainer kddc = GraphDataManager.Instance.GetKDataDictContainer(numIndex);
            if (kddc != null)
            {
                for (int i = 0; i < kddc.dataLst.Count; ++i)
                {
                    KDataDict kdDict = kddc.dataLst[i];
                    KData data = kdDict.GetData(cdt, false);
                    if (data == null)
                        continue;
                    DrawData(g, data, winW, winH);
                }
            }
        }

        void DrawData(Graphics g, KData data, int winW, int winH)
        {
            float valueChange = data.HitValue - data.MissValue;
            if (data.index < startDataIndex)
            {
                lastValue += valueChange;
                startDataValue += valueChange;
                return;
            }

            float baseX = 0;
            float baseY = winH / 2;
            baseX += (data.index - startDataIndex) * gridScaleW;
            baseY -= (lastValue - startDataValue) * gridScaleH;
            int upBound = (int)(baseY - data.HitValue * gridScaleH);
            int downBound = (int)(baseY + data.MissValue * gridScaleH);
            
            Color col = valueChange > 0 ? Color.Red : (valueChange < 0 ? Color.Blue : Color.White);
            Pen linePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, col, 1);
            Pen rcPen = GraphUtil.GetSolidPen(col);
            int midX = (int)(baseX + gridScaleW * 0.5f);
            Point p0 = new Point(midX, upBound);
            Point p1 = new Point(midX, downBound);
            g.DrawLine(linePen, p0, p1);
            float rcY = valueChange > 0 ? (baseY - valueChange * gridScaleH) : baseY;
            int height = (int)Math.Abs(valueChange * gridScaleH);
            if (height < 1)
                height = 1;
            g.FillRectangle(new SolidBrush(col), baseX, rcY, gridScaleW, height);
            lastValue += valueChange;
        }




    }
}