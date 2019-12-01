//#define TRADE_DBG
//#define RECORD_BOLLEAN_MID_COUNTS

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    #region Data Manager

    public enum CollectDataType
    {
        eNone,

        ePath0          = 1 << 0,
        ePath1          = 1 << 1,
        ePath2          = 1 << 2,
        eBigNum         = 1 << 3,
        eSmallNum       = 1 << 4,
        eOddNum         = 1 << 5,
        eEvenNum        = 1 << 6,
        ePrimeNum       = 1 << 7,
        eCompositeNum   = 1 << 8,
        eNum0           = 1 << 9,
        eNum1           = 1 << 10,
        eNum2           = 1 << 11,
        eNum3           = 1 << 12,
        eNum4           = 1 << 13,
        eNum5           = 1 << 14,
        eNum6           = 1 << 15,
        eNum7           = 1 << 16,
        eNum8           = 1 << 17,
        eNum9           = 1 << 18,


        eMax = 0xFFFFFFF,
    }



    // 均线计算方式
    public enum AvgAlgorithm
    {
        // 简单算术平均
        eSMA,
        // 末日加权平均
        eWMALastDay,
        // 线性加权平均
        eWMALinear,
        // 平方系数加权平均
        eWMASqr,
        // 指数平滑移动平均
        eEMA,
    }


    public enum MissCountType
    {
        eMissCountValue,
        eMissCountAreaFast,
        eMissCountAreaShort,
        eMissCountAreaLong,
        eDisappearCountFast,
        eDisappearCountShort,
        eDisappearCountLong,
        eMissCountAreaMulti,
    }

    public enum AppearenceType
    {
        eAppearenceFast,
        eAppearenceShort,
        eAppearenceLong,
        eAppearCountFast,
        eAppearCountShort,
        eAppearCountLong,
        eAppearenceMulti,
    }


    // 图表数据管理器
    public class GraphDataManager
    {
        public static List<string> S_MissCountTypeStrs = new List<string>()
        {
                "遗漏值",
                "统计5期的遗漏均线",
                "统计10期的遗漏均线",
                "统计30期的遗漏均线",
                "统计5期的遗漏数",
                "统计10期的遗漏数",
                "统计30期的遗漏数",
                "统计多周期的遗漏均线",
        };

        public static List<string> S_AppearenceTypeStrs = new List<string>()
        {
                "统计5期的出号率",
                "统计10期的出号率",
                "统计30期的出号率",
                "统计5期的出号个数",
                "统计10期的出号个数",
                "统计30期的出号个数",
                "多周期的出号率曲线",
        };

        public static List<CollectDataType> S_CDT_LIST = new List<CollectDataType>();
        public static List<string> S_CDT_TAG_LIST = new List<string>();
        public static Dictionary<string, int> S_CDT_STR_INDEX_MAP = new Dictionary<string, int>();
        public static List<float> S_CDT_PROBABILITY_LIST = new List<float>();
        public static List<float> S_CDT_MISS_REL_LENGTH_LIST = new List<float>();
        public static List<Color> S_CDT_COLOR_LIST = new List<Color>();
        public static List<UnityEngine.Color> S_CDT_UCOLOR_LIST = new List<UnityEngine.Color>();
        public static Dictionary<GraphType, GraphDataContainerBase> S_GRAPH_DATA_CONTS = new Dictionary<GraphType, GraphDataContainerBase>();
        public static Dictionary<string, int> S_CDT_NAME_INDEX_MAP = new Dictionary<string, int>();
        public static Dictionary<string, int> S_NUM_NAME_INDEX_MAP = new Dictionary<string, int>();
        public static GraphDataContainerKGraph KGDC
        {
            get
            {
                return MultiKGDCMap[CurrentCircle];
            }
        }
        public static GraphDataContainerBarGraph BGDC;

        public static int CurrentCircle = 1;
        public static Dictionary<int, GraphDataContainerKGraph> MultiKGDCMap = new Dictionary<int, GraphDataContainerKGraph>();

        public static int[] G_Circles = new int[] { 1, 3, 5, 10, };
        public static List<string> G_Circles_STRs = new List<string>() { "1", "3", "5", "10", };

        static GraphDataManager()
        {
            AddPreInfo(CollectDataType.ePath0, "0路", 4.0f / 10, Color.Red);
            AddPreInfo(CollectDataType.ePath1, "1路", 3.0f / 10, Color.Green);
            AddPreInfo(CollectDataType.ePath2, "2路", 3.0f / 10, Color.Blue);
            AddPreInfo(CollectDataType.eBigNum, "大数", 5.0f / 10, Color.Firebrick);
            AddPreInfo(CollectDataType.eSmallNum, "小数", 5.0f / 10, Color.Aquamarine);
            AddPreInfo(CollectDataType.eOddNum, "奇数", 5.0f / 10, Color.Beige);
            AddPreInfo(CollectDataType.eEvenNum, "偶数", 5.0f / 10, Color.Bisque);
            AddPreInfo(CollectDataType.ePrimeNum, "质数", 5.0f / 10, Color.BlueViolet);
            AddPreInfo(CollectDataType.eCompositeNum, "合数", 5.0f / 10, Color.Brown);
            AddPreInfo(CollectDataType.eNum0, "数字0", 1.0f / 10, Color.BurlyWood);
            AddPreInfo(CollectDataType.eNum1, "数字1", 1.0f / 10, Color.Chocolate);
            AddPreInfo(CollectDataType.eNum2, "数字2", 1.0f / 10, Color.Cyan);
            AddPreInfo(CollectDataType.eNum3, "数字3", 1.0f / 10, Color.DarkBlue);
            AddPreInfo(CollectDataType.eNum4, "数字4", 1.0f / 10, Color.DarkGoldenrod);
            AddPreInfo(CollectDataType.eNum5, "数字5", 1.0f / 10, Color.DarkGreen);
            AddPreInfo(CollectDataType.eNum6, "数字6", 1.0f / 10, Color.DarkOrange);
            AddPreInfo(CollectDataType.eNum7, "数字7", 1.0f / 10, Color.DarkSeaGreen);
            AddPreInfo(CollectDataType.eNum8, "数字8", 1.0f / 10, Color.DeepPink);
            AddPreInfo(CollectDataType.eNum9, "数字9", 1.0f / 10, Color.DodgerBlue);

            //S_GRAPH_DATA_CONTS.Add(GraphType.eKCurveGraph, KGDC = new GraphDataContainerKGraph());
            S_GRAPH_DATA_CONTS.Add(GraphType.eBarGraph, BGDC = new GraphDataContainerBarGraph());

            for( int i = 0; i < KDataDictContainer.C_TAGS.Count; ++i )
            {
                S_NUM_NAME_INDEX_MAP.Add(KDataDictContainer.C_TAGS[i], i);
            }
            
            for(int i = 0; i < G_Circles.Length; ++i )
            {
                MultiKGDCMap.Add(G_Circles[i], new GraphDataContainerKGraph(G_Circles[i]));
            }
        }
        static void AddPreInfo(CollectDataType cdt, string name, float probability, Color col)
        {
            S_CDT_LIST.Add(cdt);
            S_CDT_TAG_LIST.Add(name);
            S_CDT_NAME_INDEX_MAP.Add(name, S_CDT_NAME_INDEX_MAP.Count);
            S_CDT_STR_INDEX_MAP.Add(cdt.ToString(), S_CDT_STR_INDEX_MAP.Count);
            S_CDT_PROBABILITY_LIST.Add(probability);
            S_CDT_MISS_REL_LENGTH_LIST.Add(probability / (1.0f - probability));
            S_CDT_COLOR_LIST.Add(col);
            S_CDT_UCOLOR_LIST.Add(new UnityEngine.Color(col.R / 255.0f, col.G / 255.0f, col.B / 255.0f, col.A / 255.0f));
        }
        public static float GetTheoryProbability(CollectDataType cdt)
        {
            int index = S_CDT_LIST.IndexOf(cdt);
            return S_CDT_PROBABILITY_LIST[index] * 100.0f;
        }
        public static Color GetCdtColor(CollectDataType cdt)
        {
            int index = S_CDT_LIST.IndexOf(cdt);
            return S_CDT_COLOR_LIST[index];
        }
        
        public static UnityEngine.Color GetColorByCDT(CollectDataType cdt)
        {
            int index = S_CDT_LIST.IndexOf(cdt);
            return S_CDT_UCOLOR_LIST[index];
        }

        public static float GetMissRelLength(CollectDataType cdt)
        {
            int index = S_CDT_LIST.IndexOf(cdt);
            return S_CDT_MISS_REL_LENGTH_LIST[index];
        }
        public static int GetCdtIndex(string cdtSTR)
        {
            return S_CDT_NAME_INDEX_MAP[cdtSTR];
        }
        public static int GetNumIndex(string numSTR)
        {
            return S_NUM_NAME_INDEX_MAP[numSTR];
        }

        public static int GetCdtIndexByEnumStr(string cdtEnumStr)
        {
            return S_CDT_STR_INDEX_MAP[cdtEnumStr];
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
            if(gt == GraphType.eKCurveGraph)
            {
                return KGDC;
            }
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                return (S_GRAPH_DATA_CONTS[gt]);
            return null;
        }

        public int DataLength(GraphType gt)
        {
            if(gt == GraphType.eKCurveGraph)
            {
                return KGDC.DataLength();
            }
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                return S_GRAPH_DATA_CONTS[gt].DataLength();
            return 0;
        }

        public bool HasData(GraphType gt)
        {
            if(gt == GraphType.eKCurveGraph)
            {
                return KGDC.HasData();
            }
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                return S_GRAPH_DATA_CONTS[gt].HasData();
            return false;
        }

        public void CollectGraphData(GraphType gt)
        {
            if(gt == GraphType.eKCurveGraph)
            {
                foreach( GraphDataContainerKGraph kgdc in MultiKGDCMap.Values )
                {
                    kgdc.CollectGraphData();
                }
                return;
            }
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                S_GRAPH_DATA_CONTS[gt].CollectGraphData();
        }

        public void CollectGraphDataExcept(GraphType gt, OneDayDatas odd)
        {
            if (gt == GraphType.eKCurveGraph)
            {
                foreach (GraphDataContainerKGraph kgdc in MultiKGDCMap.Values)
                {
                    kgdc.CollectGraphDataExcept(odd);
                }
                return;
            }
            if (S_GRAPH_DATA_CONTS.ContainsKey(gt))
                S_GRAPH_DATA_CONTS[gt].CollectGraphDataExcept(odd);
        }

        public void Clear()
        {
            KGDC.Clear();
            BGDC.Clear();
        }

        public static void ResetCurKValueMap()
        {
            foreach( GraphDataContainerKGraph kgdc in MultiKGDCMap.Values )
            {
                kgdc.ResetCurKValueMap();
            }
        }
    }


#endregion
}