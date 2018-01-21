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


#region Data Manager
    class KData
    {
        public CollectDataType cdt;
        public KDataDict parent;
        public int index;
        public float HitValue;
        public float MissValue;

        string info = null;
        public string GetInfo()
        {
            if (info == null)
            {
                int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                info = "[" + parent.startItem.idTag + "-" + parent.endItem.idTag + "] [" +
                    GraphDataManager.S_CDT_TAG_LIST[cdtID] + "] [" + 
                    HitValue + " : " + MissValue + "]";
            }
            return info;
        }
    }

    class KDataDict
    {
        public int index;
        public Dictionary<CollectDataType, KData> dataDict = new Dictionary<CollectDataType, KData>();
        public DataItem startItem = null;
        public DataItem endItem = null;

        public KData GetData(CollectDataType collectDataType, bool createIfNotExist)
        {
            if (dataDict.ContainsKey(collectDataType))
                return dataDict[collectDataType];
            else if (createIfNotExist == false)
                return null;
            KData data = new KData();
            data.parent = this;
            data.cdt = collectDataType;
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

    class GraphDataContainerBase
    {
        public virtual int DataLength() { return 0; }
        public virtual bool HasData() { return false; }
        public virtual void CollectGraphData() { }
    }
    class KGraphDataContainer : GraphDataContainerBase
    {
        int dataLength = 0;
        int cycleLength = 5;
        List<KDataDictContainer> allKDatas = new List<KDataDictContainer>();
        public int CycleLength
        {
            get { return cycleLength; }
            set { cycleLength = value; }
        }
        public override int DataLength() { return dataLength; }
        public override bool HasData()
        {
            return allKDatas.Count > 0;
        }
        public override void CollectGraphData()
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

            dataLength = allKDatas[0].dataLst.Count;
        }

        public KDataDictContainer GetKDataDictContainer(int numIndex)
        {
            if (allKDatas.Count > numIndex)
                return allKDatas[numIndex];
            return null;
        }
        void Init()
        {
            KDataDictContainer kddc = null;
            for (int i = 0; i < 5; ++i)
            {
                if (i >= allKDatas.Count)
                {
                    kddc = new KDataDictContainer();
                    allKDatas.Add(kddc);
                }
                else
                {
                    kddc = allKDatas[i];
                    kddc.dataLst.Clear();
                }
            }
        }
        void CollectItem(int NUM_INDEX, DataItem item, KDataDict kd)
        {
            if (kd.startItem == null)
                kd.startItem = item;
            if (kd.endItem == null || item.idGlobal > kd.endItem.idGlobal)
                kd.endItem = item;
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
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
                curDatas[i] = allKDatas[i].CreateKDataDict();
            }
            return curDatas;
        }
    }
    class BarGraphDataContianer : GraphDataContainerBase
    {
        public enum StatisticsType
        {
            eAppearCountFrom0To9,
            eAppearCountPath012,
        }

        public enum StatisticsRange
        {
            e10,
            e20,
            e30,
            e50,
            e100,
            eCustom,
        }
        public static List<string> S_StatisticsType_STRS = new List<string>();
        public static List<string> S_StatisticsRange_STRS = new List<string>();
        static BarGraphDataContianer()
        {
            S_StatisticsType_STRS.Add("0-9出现次数");
            S_StatisticsType_STRS.Add("012路出现次数");

            S_StatisticsRange_STRS.Add("最近10期");
            S_StatisticsRange_STRS.Add("最近20期");
            S_StatisticsRange_STRS.Add("最近30期");
            S_StatisticsRange_STRS.Add("最近50期");
            S_StatisticsRange_STRS.Add("最近100期");
            S_StatisticsRange_STRS.Add("自定义期数");
        }

        public class DataUnit
        {
            public StatisticsType type;
            public int data;
            public string tag;
        }
        public class DataUnitLst
        {
            public List<DataUnit> dataLst = new List<DataUnit>();
        }
        public StatisticsType curStatisticsType = StatisticsType.eAppearCountFrom0To9;
        public StatisticsRange curStatisticsRange = StatisticsRange.e100;
        public int customStatisticsRange = 120;
        public List<DataUnitLst> allDatas = new List<DataUnitLst>();
        public int totalCollectCount = 0;

        public BarGraphDataContianer()
        {
            for (int i = 0; i < 5; ++i )
                allDatas.Add(new DataUnitLst());
        }
        void Init()
        {
            for (int c = 0; c < 5; ++c)
            {
                allDatas[c].dataLst.Clear();
                switch (curStatisticsType)
                {
                    case StatisticsType.eAppearCountFrom0To9:
                        {
                            for (int i = 0; i <= 9; ++i)
                            {
                                DataUnit du = new DataUnit();
                                du.data = 0;
                                du.type = StatisticsType.eAppearCountFrom0To9;
                                du.tag = i.ToString();
                                allDatas[c].dataLst.Add(du);
                            }
                        }
                        break;
                    case StatisticsType.eAppearCountPath012:
                        {
                            for (int i = 0; i <= 2; ++i)
                            {
                                DataUnit du = new DataUnit();
                                du.data = 0;
                                du.type = StatisticsType.eAppearCountPath012;
                                du.tag = i.ToString();
                                allDatas[c].dataLst.Add(du);
                            }
                        }
                        break;
                }
            }
        }
        void CollectItem(DataItem item)
        {
            for( int i = 0; i < 5; ++i )
            {
                switch (curStatisticsType)
                {
                    case StatisticsType.eAppearCountFrom0To9:
                        {
                            int num = item.GetNumberByIndex(i);
                            DataUnit du = allDatas[i].dataLst[num];
                            du.data = du.data + 1;
                        }
                        break;
                    case StatisticsType.eAppearCountPath012:
                        {
                            int num = item.path012OfEachSingle[i];
                            DataUnit du = allDatas[i].dataLst[num];
                            du.data = du.data + 1;
                        }
                        break;
                }
            }
        }

        public override int DataLength() { return allDatas[0].dataLst.Count; }
        public override bool HasData() { return allDatas[0].dataLst.Count > 0; }
        public override void CollectGraphData()
        {
            totalCollectCount = 0;
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            Init();

            int CollectCount = customStatisticsRange;
            switch (curStatisticsRange)
            {
                case StatisticsRange.e10: CollectCount = 10; break;
                case StatisticsRange.e20: CollectCount = 20; break;
                case StatisticsRange.e30: CollectCount = 30; break;
                case StatisticsRange.e50: CollectCount = 50; break;
                case StatisticsRange.e100: CollectCount = 100; break;
            }
            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (currentItem == null)
                return;
            for (int i = 0; i < CollectCount; ++i)
            {
                CollectItem(currentItem);
                ++totalCollectCount;
                currentItem = DataManager.GetInst().GetPrevItem(currentItem);
                if (currentItem == null)
                    break;
            }
        }
    }

    class GraphDataManager
    {
        public static List<CollectDataType> S_CDT_LIST = new List<CollectDataType>();
        public static List<string> S_CDT_TAG_LIST = new List<string>();
        public static List<float> S_CDT_PROBABILITY_LIST = new List<float>();
        public static List<float> S_CDT_MISS_REL_LENGTH_LIST = new List<float>();
        public static Dictionary<GraphType, GraphDataContainerBase> S_GRAPH_DATA_CONTS = new Dictionary<GraphType, GraphDataContainerBase>();

        public static KGraphDataContainer KGDC;
        public static BarGraphDataContianer BGDC;

        static GraphDataManager()
        {
            AddPreInfo(CollectDataType.ePath0, "0路", 4.0f / 10);
            AddPreInfo(CollectDataType.ePath1, "1路", 3.0f / 10);
            AddPreInfo(CollectDataType.ePath2, "2路", 3.0f / 10);

            S_GRAPH_DATA_CONTS.Add(GraphType.eKCurveGraph, KGDC = new KGraphDataContainer());
            S_GRAPH_DATA_CONTS.Add(GraphType.eBarGraph, BGDC = new BarGraphDataContianer());
        }
        static void AddPreInfo(CollectDataType cdt, string name, float probability)
        {
            S_CDT_LIST.Add(cdt);
            S_CDT_TAG_LIST.Add(name);
            S_CDT_PROBABILITY_LIST.Add(probability);
            S_CDT_MISS_REL_LENGTH_LIST.Add(probability / (1.0f - probability));
        }


        static GraphDataManager sInst = null;
        public static GraphDataManager Instance
        {
            get
            {
                if (sInst == null) sInst = new GraphDataManager();
                return sInst;
            }
        }

        public static GraphDataContainerBase GetGraphDataContianer(GraphType gt)
        {
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                return (S_GRAPH_DATA_CONTS[gt]);
            return null;
        }

        public int DataLength(GraphType gt)
        {
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                return S_GRAPH_DATA_CONTS[gt].DataLength();
            return 0;
        }

        public bool HasData(GraphType gt)
        {
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                return S_GRAPH_DATA_CONTS[gt].HasData();
            return false;
        }

        public void CollectGraphData(GraphType gt)
        {
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                S_GRAPH_DATA_CONTS[gt].CollectGraphData();
        }
    }
#endregion

#region Graph 
    
    public enum GraphType
    {
        eNone,
        // K线图
        eKCurveGraph,
        // 柱状图
        eBarGraph,
    }

    class GraphBase
    {
        public virtual bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return false;
        }
        public virtual bool MoveLeftRight(bool left)
        {
            return false;
        }
        public virtual void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {

        }
    }

    class GraphKCurve : GraphBase
    {
        float gridScaleH = 5;
        float gridScaleW = 10;

        float originX = 0;
        float originY = 0;

        int startDataIndex = 0;
        float startDataHitValue = 0;
        float startDataMissValue = 0;
        float lastDataHitValue = 0;
        float lastDataMissValue = 0;

        int selDataIndex = -1;

        Font selDataFont;

        public GraphKCurve()
        {
            selDataFont = new Font(FontFamily.GenericSerif, 16);
        }

        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            if (GraphDataManager.Instance.HasData(GraphType.eKCurveGraph) == false)
                return false;
            int curIndex = (int)(mousePos.X / gridScaleW);
            if (curIndex == selDataIndex)
                return false;
            return true;
        }

        public override bool MoveLeftRight(bool left)
        {
            if (left && startDataIndex < (GraphDataManager.Instance.DataLength(GraphType.eKCurveGraph) - 1))
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

        public override void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            int baseY = winH / 2;
            g.DrawLine(GraphUtil.GetSolidPen(Color.White), new Point(0, baseY), new Point(winW, baseY));

            selDataIndex = -1;
            lastDataHitValue = 0;
            lastDataMissValue = 0;
            startDataHitValue = 0;
            startDataMissValue = 0;
            KDataDictContainer kddc = GraphDataManager.KGDC.GetKDataDictContainer(numIndex);
            if (kddc != null)
            {
                for (int i = 0; i < kddc.dataLst.Count; ++i)
                {
                    KDataDict kdDict = kddc.dataLst[i];
                    KData data = kdDict.GetData(cdt, false);
                    if (data == null)
                        continue;
                    int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                    float missRelHeight = GraphDataManager.S_CDT_MISS_REL_LENGTH_LIST[cdtID];
                    DrawData(g, data, winW, winH, missRelHeight, mouseRelPos);
                }
            }
        }

        void DrawData(Graphics g, KData data, int winW, int winH, float missRelHeight, Point mouseRelPos)
        {
            if (data.index < startDataIndex)
            {
                lastDataHitValue += data.HitValue;
                lastDataMissValue += data.MissValue;
                startDataHitValue += data.HitValue;
                startDataMissValue += data.MissValue;
                return;
            }

            float baseX = 0;
            float baseY = winH / 2;
            baseX += (data.index - startDataIndex) * gridScaleW;
            baseY -= (lastDataHitValue - startDataHitValue) * gridScaleH;
            baseY += (lastDataMissValue - startDataMissValue) * gridScaleH * missRelHeight;
            int upBound = (int)(baseY - data.HitValue * gridScaleH);
            int downBound = (int)(baseY + data.MissValue * gridScaleH * missRelHeight);
            float valueChange = data.HitValue - data.MissValue;
            
            Color col = valueChange > 0 ? Color.Red : (valueChange < 0 ? Color.Cyan : Color.White);
            Pen linePen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, col, 1);
            Pen rcPen = GraphUtil.GetSolidPen(col);
            int midX = (int)(baseX + gridScaleW * 0.5f);
            Point p0 = new Point(midX, upBound);
            Point p1 = new Point(midX, downBound);
            g.DrawLine(linePen, p0, p1);
            float rcY = valueChange > 0 ? (baseY - valueChange * gridScaleH) : baseY;
            float height = (int)Math.Abs(valueChange * gridScaleH);
            if (valueChange < 0)
                height *= missRelHeight;
            if (height < 1)
                height = 1;
            g.FillRectangle(new SolidBrush(col), baseX, rcY, gridScaleW, height);
            lastDataHitValue += data.HitValue;
            lastDataMissValue += data.MissValue;

            if (selDataIndex < 0 && baseX <= mouseRelPos.X && baseX + gridScaleW >= mouseRelPos.X)
            {
                selDataIndex = data.index;
                Pen selPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Dot, Color.Gray, 1);
                g.DrawLine(selPen, baseX, 0, baseX, winH);
                g.DrawLine(selPen, baseX + gridScaleW, 0, baseX + gridScaleW, winH);
                g.DrawString(data.GetInfo(), selDataFont, new SolidBrush(Color.White), 0, 0);
            }
        }
    }

    class GraphBar : GraphBase
    {
        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return false;
        }
        public override bool MoveLeftRight(bool left)
        {
            return false;
        }
        public override void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            BarGraphDataContianer bgdc = GraphDataManager.BGDC;
            if (bgdc.HasData() == false || bgdc.totalCollectCount == 0)
                return;

            float startX = 0;
            float startY = winH * 0.1f;
            float MaxRcH = winH * 0.8f;
            float bottom = winH * 0.9f;

            BarGraphDataContianer.DataUnitLst dul = bgdc.allDatas[numIndex];
            float gap = (float)winW / dul.dataLst.Count;
            SolidBrush brush = new SolidBrush(Color.Red);
            SolidBrush tagBrush = new SolidBrush(Color.White);
            Font tagFont = new Font(FontFamily.GenericSerif, 16);
            for (int i = 0; i < dul.dataLst.Count; ++i)
            {
                float rcH = MaxRcH * ((float)(dul.dataLst[i].data) / (float)bgdc.totalCollectCount);
                startY = bottom - rcH;
                g.FillRectangle(brush, startX, startY, gap * 0.9f, rcH);
                g.DrawString(dul.dataLst[i].tag, tagFont, tagBrush, startX, bottom);
                g.DrawString(dul.dataLst[i].data.ToString(), tagFont, tagBrush, startX, startY - 30);
                startX += gap;
            }
        }
    }

    class GraphSet
    {
        Dictionary<GraphType, GraphBase> sGraphMap = new Dictionary<GraphType, GraphBase>();
        GraphBase curGraph = null;
        GraphType curGraphType = GraphType.eNone;
        public GraphSet()
        {
            sGraphMap.Add(GraphType.eKCurveGraph, new GraphKCurve());
            sGraphMap.Add(GraphType.eBarGraph, new GraphBar());
        }

        public GraphType CurrentGraphType
        {
            get { return curGraphType; }
        }

        public void SetCurrentGraph(GraphType gt)
        {
            curGraphType = gt;
            if (sGraphMap.ContainsKey(gt))
                curGraph = sGraphMap[gt];
            else
                curGraph = null;
        }
        public bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            if (curGraph != null)
                return curGraph.NeedRefreshCanvasOnMouseMove(mousePos);
            return false;
        }
        public bool MoveLeftRight(bool left)
        {
            if (curGraph != null)
                return curGraph.MoveLeftRight(left);
            return false;
        }
        public void DrawGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            if (curGraph != null)
                curGraph.DrawGraph(g, numIndex, cdt, winW, winH, mouseRelPos);
        }
    }

#endregion
}