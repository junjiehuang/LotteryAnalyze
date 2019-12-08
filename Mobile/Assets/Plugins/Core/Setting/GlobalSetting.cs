using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
//using System.Windows.Forms;

#if UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using UnityEngine;
#endif


namespace LotteryAnalyze
{
    public class Parameter : Attribute
    {
        public string name = "";
        public object defV;

        public Parameter(string _name, object _defV)
        {
            name = _name;
            defV = _defV;
        }
    }

    public enum AppearenceCheckType
    {
        eUseFast,
        eUSeShort,
        eUseFastAndShort,
    }

    public enum HotNumPredictType
    {
        ePath012,
        eNumber,
    }

    public class GlobalSetting
    {
        static GlobalSetting sInstance = null;
        public static GlobalSetting Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new GlobalSetting();
                return sInstance;
            }
        }

        static bool sIsCurrentFetchingLatestData = false;
        public static bool IsCurrentFetchingLatestData
        {
            get { return sIsCurrentFetchingLatestData; }
            set { sIsCurrentFetchingLatestData = value; }
        }
        static bool HAS_MODIFY = false;
        
        [Parameter("数据收集设定/数据源类型", AutoUpdateUtil.DataSourceType.eCaiBow)]
        private static AutoUpdateUtil.DataSourceType g_DATA_SOURCE_TYPE = AutoUpdateUtil.DataSourceType.e360;
        [Parameter("数据收集设定/是否自动刷新最新数据", true)]
        private static bool g_AUTO_REFRESH_LATEST_DATA = true;
        [Parameter("数据收集设定/自动刷新最新数据时间间隔（秒）", 60.0f)]
        private static float g_AUTO_REFRESH_LATEST_DATA_INTERVAL = 60.0f;


        [Parameter("界面设置/是否在主线程刷新", true)]
        private static bool g_UPDATE_IN_MAIN_THREAD = true;
        [Parameter("界面设置/窗口透明度", 1.0f)]
        private static float g_WINDOW_OPACITY = 1.0f;
        [Parameter("界面设置/曲线图刷新毫秒间隔", 1500)]
        private static int g_LOTTERY_GRAPH_UPDATE_INTERVAL = 1500;
        [Parameter("界面设置/模拟图刷新毫秒间隔", 1500)]
        private static int g_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = 1500;
        [Parameter("界面设置/主线程更新休眠时间", 100)]
        private static int g_GLOBAL_MAIN_THREAD_UPDATE_INTERVAL = 1500;

        [Parameter("界面设置/统计界面/当前缩放X", 15.0f)]
        private static float g_STATISTIC_CANVAS_SCALE_X = 15.0f;
        [Parameter("界面设置/统计界面/最大缩放X", 15.0f)]
        private static float g_STATISTIC_CANVAS_SCALE_X_MAX = 15.0f;
        [Parameter("界面设置/统计界面/最小显示格大小", 15.0f)]
        private static float g_STATISTIC_CANVAS_MIN_GRID_SIZE = 15.0f;

        [Parameter("界面设置/交易界面/当前缩放X", 15.0f)]
        private static float g_TRADE_CANVAS_SCALE_X = 15.0f;
        [Parameter("界面设置/交易界面/最大缩放X", 15.0f)]
        private static float g_TRADE_CANVAS_SCALE_X_MAX = 15.0f;
        [Parameter("界面设置/交易界面/最小显示格大小", 15.0f)]
        private static float g_TRADE_CANVAS_MIN_GRID_SIZE = 15.0f;

        [Parameter("界面设置/K线界面缩放X", 15.0f)]
        private static float g_KGRAPH_CANVAS_SCALE_X = 15.0f;
        [Parameter("界面设置/K线界面缩放Y", 30.0f)]
        private static float g_KGRAPH_CANVAS_SCALE_Y = 30.0f;

        [Parameter("界面设置/遗漏线界面缩放X", 15.0f)]
        private static float g_MISSGRAPH_CANVAS_SCALE_X = 15.0f;
        [Parameter("界面设置/遗漏线界面缩放Y", 30.0f)]
        private static float g_MISSGRAPH_CANVAS_SCALE_Y = 30.0f;

        [Parameter("界面设置/辅助线点击检测范围", 30.0f)]
        private static float g_AUX_LINE_KEY_POINT_HIT_SIZE = 30.0f;
        [Parameter("界面设置/辅助线拖拽点击范围的缩放值", 3.0f)]
        private static float g_AUX_LINE_KEY_POINT_SEL_SIZE_SCALE = 3.0f;

        [Parameter("界面设置/是否强制分析界面的水平对齐", false)]
        private static bool g_FORCE_HORZ_ALLIGN = false;

        [Parameter("界面设置/设置界面字体/普通字体大小", 40)]
        private static int g_COMMON_FONT_SIZE = 40;
        [Parameter("界面设置/设置界面字体/按钮字体大小", 45)]
        private static int g_BUTTON_FONT_SIZE = 45;

        [Parameter("界面设置/K线图预测/是否启用", true)]
        private static bool g_ENABLE_SHOW_KCURVE_PREDICT = true;
        [Parameter("界面设置/K线图预测/是否启用", 10)]
        private static int g_KCURVE_PREDICT_SAMPLE_COUNT = 10;

        [Parameter("界面设置/K线图设置/是否显示K线详情", true)]
        private static bool g_SHOW_KCURVE_DETAIL = true;
        [Parameter("界面设置/K线图设置/是否显示热号预测结果", true)]
        private static bool g_SHOW_KCURVE_HOTNUMS_RESULT = true;
        [Parameter("界面设置/K线图设置/热号预测类型", HotNumPredictType.eNumber)]
        private static HotNumPredictType g_KCURVE_HOTNUMS_PREDICT_TYPE = HotNumPredictType.eNumber;
        [Parameter("界面设置/K线图设置/热号预测采样周期3", true)]
        private static bool g_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_3 = true;
        [Parameter("界面设置/K线图设置/热号预测采样周期5", true)]
        private static bool g_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_5 = true;
        [Parameter("界面设置/K线图设置/热号预测采样周期10", true)]
        private static bool g_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_10 = true;


        [Parameter("自动通道线工具/是否开启", true)]
        private static bool g_EANBLE_ANALYZE_TOOL = true;
        [Parameter("自动通道线工具/取样数", 30)]
        private static int g_ANALYZE_TOOL_SAMPLE_COUNT = 30;
        

        [Parameter("分析数据收集/是否统计MACD走势分析数据", false)]
        private static bool g_COLLECT_MACD_ANALYZE_DATA = false;
        [Parameter("分析数据收集/是否统计布林走势分析数据", false)]
        private static bool g_COLLECT_BOLLEAN_ANALYZE_DATA = false;
        [Parameter("分析数据收集/是否统计通道线分析数据", false)]
        private static bool g_COLLECT_ANALYZE_TOOL_DATA = false;


        [Parameter("筛选设定/是否开启同路保持检测", true)]
        private static bool g_ENABLE_CHECK_AND_KEEP_SAME_PATH = true;
        [Parameter("筛选设定/换路检查/是否在本路出号率很低的时候切换分路", true)]
        private static bool g_CHANGE_PATH_ON_LOW_APPEARENCE_RATE = true;
        [Parameter("筛选设定/换路检查/是否在计划次数全部失败的时候才切换分路", true)]
        private static bool g_CHANGE_PATH_ON_ALL_TRADE_MISS = true;
        [Parameter("筛选设定/是否开启同路支撑压力检测", true)]
        private static bool g_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL = true;
        //[Parameter("筛选设定/是否开启同路布林通道检测", true)]
        //private static bool g_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE = true;
        //[Parameter("筛选设定/是否开启布林下轨提升检测", true)]
        //private static bool g_ENABLE_BOOLEAN_DOWN_UPWARD_CHECK = true;
        [Parameter("筛选设定/是否交易最大出现率那一路", true)]
        private static bool g_ENABLE_MAX_APPEARENCE_FIRST_CHECK = true;

        [Parameter("筛选设定/是否开启MACD上升检测", true)]
        private static bool g_ENABLE_MACD_UP_CHECK = true;
        [Parameter("筛选设定/是否开启布林线形态检测", true)]
        private static bool g_ENABLE_BOLLEAN_CFG_CHECK = true;
        [Parameter("筛选设定/是否只交易最佳的012路", false)]
        private static bool g_ONLY_TRADE_BEST_PATH = false;
        [Parameter("筛选设定/是否开启判定这一路能否交易",false)]
        private static bool g_ENABLE_CHECK_PATH_CAN_TRADE = false;

        [Parameter("筛选设定/忽略设置/是否在布林下轨连续开出就忽略当前的交易",false)]
        private static bool g_IGNORE_CUR_TRADE_ON_BOLLEAN_DOWN_CONTINUE = false;
        [Parameter("筛选设定/忽略设置/是否在布林上轨连续超过3期没开出就忽略当前的交易", false)]
        private static bool g_IGNORE_CUR_TRADE_ON_BOLLEAN_UP_CONTINUE_MISS = false;
        [Parameter("筛选设定/忽略设置/是否不在布林中轨就忽略交易", false)]
        private static bool g_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_MID = false;
        [Parameter("筛选设定/忽略设置/是否不在布林上轨连续开出就忽略交易", false)]
        private static bool g_IGNORE_CUR_TRADE_ON_NOT_CONTINUE_HIT_UPON_BOLLEAN_MID = false;
        [Parameter("筛选设定/忽略设置/是否不在布林上轨就忽略交易", false)]
        private static bool g_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_UP = false;

        [Parameter("筛选设定/直接交易设置/是否交易布林中轨的k线", false)]
        private static bool g_TRADE_IMMEDIATE_AT_BOLLEAN_MID = false;
        [Parameter("筛选设定/直接交易设置/是否在触及布林下轨时交易", false)]
        private static bool g_TRADE_IMMEDIATE_AT_TOUCH_BOLLEAN_DOWN = false;
        [Parameter("筛选设定/直接交易设置/是否在布林中轨之上连续开出时直接交易", false)]
        private static bool g_TRADE_IMMEDIATE_ON_CONTINUE_HIT_UPON_BOLLEAN_MID = false;
        [Parameter("筛选设定/直接交易设置/最大遗漏容忍值", 5)]
        private static int  g_TRADE_IMMEDIATE_TORANCE_MAX_MISS_COUNT = 5;
        [Parameter("筛选设定/直接交易设置/是否直接交易遗漏为0的那一路", false)]
        private static bool g_TRADE_IMMEDIATE_ON_MISS_COUNT0_PATH = false;

        [Parameter("筛选设定/排序选项/是否按照Macd LINE分析数据排序", false)]
        private static bool g_SEQ_PATH_BY_MACD_LINE = false;
        [Parameter("筛选设定/排序选项/是否按照Macd Signal分析数据排序", false)]
        private static bool g_SEQ_PATH_BY_MACD_SIGNAL = false;
        [Parameter("筛选设定/排序选项/是否按照MACD分析数据排序", false)]
        private static bool g_SEQ_PATH_BY_MACD_CFG = false;
        [Parameter("筛选设定/排序选项/是否按照布林分析数据排序",false)]
        private static bool g_SEQ_PATH_BY_BOLLEAN_CFG = false;
        [Parameter("筛选设定/排序选项/是否按照出现率排序", false)]
        private static bool g_SEQ_PATH_BY_APPEARENCE_RATE = false;
        [Parameter("筛选设定/排序选项/出现率检查类型", AppearenceCheckType.eUseFast)]
        private static AppearenceCheckType g_AppearenceCheckType = AppearenceCheckType.eUseFast;

        [Parameter("模拟交易设置/是否只交易指定的012路",true)]
        private static bool g_ONLY_TRADE_SPEC_CDT = true;
        [Parameter("模拟交易设置/交易指定的012路", CollectDataType.ePath0)]
        private static CollectDataType g_TRADE_SPEC_CDT = CollectDataType.ePath0;

        [Parameter("数据设置/每注一星交易成本",1.0f)]
        private static float g_ONE_STARE_TRADE_COST = 1.0f;
        [Parameter("数据设置/每注一星交易奖金",9.8f)]
        private static float g_ONE_STARE_TRADE_REWARD = 9.8f;
        [Parameter("数据设置/选择交易策略ID",-1)]
        private static int g_CUR_TRADE_INDEX = -1;
        [Parameter("数据设置/每批加载多少天的数据",3)]
        private static int g_DAYS_PER_BATCH = 3;
        [Parameter("数据设置/是否记录交易数据",true)]
        private static bool g_ENABLE_REC_TRADE_DATAS = true;

        [Parameter("数据统计设置/当遗漏值超过多少期就记录其信息", 7)]
        private static int g_OVER_SPEC_MISS_COUNT = 7;
        [Parameter("数据统计设置/是否计算布林分析数据", true)]
        private static bool g_CALC_BOOLEAN_ANALYSE_DATA = true;
        [Parameter("数据统计设置/首次长遗漏值", 7)]
        private static int g_MISS_COUNT_FIRST =7;
        [Parameter("数据统计设置/二次长遗漏值", 7)]
        private static int g_MISS_COUNT_SECOND = 7;

        [Parameter("模拟交易/筛选最大个数", 5)]
        private static int g_SIM_SEL_MAX_COUNT = 5;
        [Parameter("模拟交易/是否做万位", true)]
        private static bool g_SIM_SEL_NUM_AT_POS_0 = true;
        [Parameter("模拟交易/是否做千位", true)]
        private static bool g_SIM_SEL_NUM_AT_POS_1 = true;
        [Parameter("模拟交易/是否做百位", true)]
        private static bool g_SIM_SEL_NUM_AT_POS_2 = true;
        [Parameter("模拟交易/是否做十位", true)]
        private static bool g_SIM_SEL_NUM_AT_POS_3 = true;
        [Parameter("模拟交易/是否做个位", true)]
        private static bool g_SIM_SEL_NUM_AT_POS_4 = true;
        [Parameter("模拟交易/每批模拟结束后是否暂停模拟", true)]
        private static bool g_SIM_PAUSE_AT_BATCH_FINISH = true;


        public static List<string> TradeTags = new List<string>();
        public static List<List<int>> TradeSets = new List<List<int>>();

        static IniFile cfg = null;

        public static float G_WINDOW_OPACITY
        {
            get
            {
                return g_WINDOW_OPACITY;
            }

            set
            {
                g_WINDOW_OPACITY = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_ANALYZE_TOOL_SAMPLE_COUNT
        {
            get
            {
                return g_ANALYZE_TOOL_SAMPLE_COUNT;
            }

            set
            {
                g_ANALYZE_TOOL_SAMPLE_COUNT = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_LOTTERY_GRAPH_UPDATE_INTERVAL
        {
            get
            {
                return g_LOTTERY_GRAPH_UPDATE_INTERVAL;
            }

            set
            {
                g_LOTTERY_GRAPH_UPDATE_INTERVAL = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL
        {
            get
            {
                return g_GLOBAL_SIM_TRADE_UPDATE_INTERVAL;
            }

            set
            {
                g_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_EANBLE_ANALYZE_TOOL
        {
            get
            {
                return g_EANBLE_ANALYZE_TOOL;
            }

            set
            {
                g_EANBLE_ANALYZE_TOOL = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_ENABLE_CheckAndKeepSamePath
        {
            get
            {
                return g_ENABLE_CHECK_AND_KEEP_SAME_PATH;
            }

            set
            {
                g_ENABLE_CHECK_AND_KEEP_SAME_PATH = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL
        {
            get
            {
                return g_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL;
            }

            set
            {
                g_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL = value;
                HAS_MODIFY = true;
            }
        }

        //public static bool G_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE
        //{
        //    get
        //    {
        //        return g_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE;
        //    }

        //    set
        //    {
        //        g_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE = value;
        //        HAS_MODIFY = true;
        //    }
        //}

        public static float G_ONE_STARE_TRADE_COST
        {
            get
            {
                return g_ONE_STARE_TRADE_COST;
            }

            set
            {
               g_ONE_STARE_TRADE_COST = value;
                HAS_MODIFY = true;
            }
        }

        public static float G_ONE_STARE_TRADE_REWARD
        {
            get
            {
                return g_ONE_STARE_TRADE_REWARD;
            }

            set
            {
                g_ONE_STARE_TRADE_REWARD = value;
                HAS_MODIFY = true;
            }
        }

        //public static bool G_ENABLE_BOOLEAN_DOWN_UP_CHECK
        //{
        //    get
        //    {
        //        return g_ENABLE_BOOLEAN_DOWN_UPWARD_CHECK;
        //    }

        //    set
        //    {
        //        g_ENABLE_BOOLEAN_DOWN_UPWARD_CHECK = value;
        //        HAS_MODIFY = true;
        //    }
        //}

        public static bool G_ENABLE_MAX_APPEARENCE_FIRST
        {
            get
            {
                return g_ENABLE_MAX_APPEARENCE_FIRST_CHECK;
            }

            set
            {
                g_ENABLE_MAX_APPEARENCE_FIRST_CHECK = value;
                HAS_MODIFY = true;
            }
        }

        //public static bool G_ENABLE_UPBOLLEAN_COUNT_STATISTIC
        //{
        //    get
        //    {
        //        return g_ENABLE_UPBOLLEAN_COUNT_STATISTIC;
        //    }

        //    set
        //    {
        //        g_ENABLE_UPBOLLEAN_COUNT_STATISTIC = value;
        //        HAS_MODIFY = true;
        //    }
        //}

        public static bool G_UPDATE_IN_MAIN_THREAD
        {
            get
            {
                return g_UPDATE_IN_MAIN_THREAD;
            }

            set
            {
                g_UPDATE_IN_MAIN_THREAD = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_CUR_TRADE_INDEX
        {
            get
            {
                return g_CUR_TRADE_INDEX;
            }

            set
            {
                g_CUR_TRADE_INDEX = value;
                HAS_MODIFY = true;
            }
        }

        public static AutoUpdateUtil.DataSourceType G_DATA_SOURCE_TYPE
        {
            get
            {
                return g_DATA_SOURCE_TYPE;
            }

            set
            {
                g_DATA_SOURCE_TYPE = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_DAYS_PER_BATCH
        {
            get
            {
                return g_DAYS_PER_BATCH;
            }

            set
            {
                g_DAYS_PER_BATCH = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_ENABLE_MACD_UP_CHECK
        {
            get
            {
                return g_ENABLE_MACD_UP_CHECK;
            }

            set
            {
                g_ENABLE_MACD_UP_CHECK = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_ENABLE_REC_TRADE_DATAS
        {
            get
            {
                return g_ENABLE_REC_TRADE_DATAS;
            }

            set
            {
                g_ENABLE_REC_TRADE_DATAS = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_ONLY_TRADE_SPEC_CDT
        {
            get
            {
                return g_ONLY_TRADE_SPEC_CDT;
            }

            set
            {
                g_ONLY_TRADE_SPEC_CDT = value;
                HAS_MODIFY = true;
            }
        }

        public static CollectDataType G_TRADE_SPEC_CDT
        {
            get
            {
                return g_TRADE_SPEC_CDT;
            }

            set
            {
                g_TRADE_SPEC_CDT = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_ONLY_TRADE_BEST_PATH
        {
            get
            {
                return g_ONLY_TRADE_BEST_PATH;
            }

            set
            {
                g_ONLY_TRADE_BEST_PATH = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_ENABLE_BOLLEAN_CFG_CHECK
        {
            get
            {
                return g_ENABLE_BOLLEAN_CFG_CHECK;
            }

            set
            {
                g_ENABLE_BOLLEAN_CFG_CHECK = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_ENABLE_CHECK_PATH_CAN_TRADE
        {
            get
            {
                return g_ENABLE_CHECK_PATH_CAN_TRADE;
            }

            set
            {
                g_ENABLE_CHECK_PATH_CAN_TRADE = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_CHANGE_PATH_ON_ALL_TRADE_MISS
        {
            get
            {
                return g_CHANGE_PATH_ON_ALL_TRADE_MISS;
            }

            set
            {
                g_CHANGE_PATH_ON_ALL_TRADE_MISS = value;
                HAS_MODIFY = true;
            }
        }

        public static AppearenceCheckType G_AppearenceCheckType
        {
            get
            {
                return g_AppearenceCheckType;
            }

            set
            {
                g_AppearenceCheckType = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_SEQ_PATH_BY_APPEARENCE_RATE
        {
            get
            {
                return g_SEQ_PATH_BY_APPEARENCE_RATE;
            }

            set
            {
                g_SEQ_PATH_BY_APPEARENCE_RATE = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_IGNORE_CUR_TRADE_ON_BOLLEAN_DOWN_CONTINUE
        {
            get
            {
                return g_IGNORE_CUR_TRADE_ON_BOLLEAN_DOWN_CONTINUE;
            }

            set
            {
                g_IGNORE_CUR_TRADE_ON_BOLLEAN_DOWN_CONTINUE = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_COLLECT_MACD_ANALYZE_DATA
        {
            get
            {
                return g_COLLECT_MACD_ANALYZE_DATA;
            }

            set
            {
                g_COLLECT_MACD_ANALYZE_DATA = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_COLLECT_BOLLEAN_ANALYZE_DATA
        {
            get
            {
                return g_COLLECT_BOLLEAN_ANALYZE_DATA;
            }

            set
            {
                g_COLLECT_BOLLEAN_ANALYZE_DATA = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_COLLECT_ANALYZE_TOOL_DATA
        {
            get
            {
                return g_COLLECT_ANALYZE_TOOL_DATA;
            }

            set
            {
                g_COLLECT_ANALYZE_TOOL_DATA = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_IGNORE_CUR_TRADE_ON_BOLLEAN_UP_CONTINUE_MISS
        {
            get
            {
                return g_IGNORE_CUR_TRADE_ON_BOLLEAN_UP_CONTINUE_MISS;
            }

            set
            {
                g_IGNORE_CUR_TRADE_ON_BOLLEAN_UP_CONTINUE_MISS = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_CHANGE_PATH_ON_LOW_APPEARENCE_RATE
        {
            get
            {
                return g_CHANGE_PATH_ON_LOW_APPEARENCE_RATE;
            }

            set
            {
                g_CHANGE_PATH_ON_LOW_APPEARENCE_RATE = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_SEQ_PATH_BY_BOLLEAN_CFG
        {
            get
            {
                return g_SEQ_PATH_BY_BOLLEAN_CFG;
            }

            set
            {
                g_SEQ_PATH_BY_BOLLEAN_CFG = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_SEQ_PATH_BY_MACD_CFG
        {
            get
            {
                return g_SEQ_PATH_BY_MACD_CFG;
            }

            set
            {
                g_SEQ_PATH_BY_MACD_CFG = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_TRADE_IMMEDIATE_AT_BOLLEAN_MID
        {
            get
            {
                return g_TRADE_IMMEDIATE_AT_BOLLEAN_MID;
            }
            set
            {
                g_TRADE_IMMEDIATE_AT_BOLLEAN_MID = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_TRADE_IMMEDIATE_AT_TOUCH_BOLLEAN_DOWN
        {
            get
            {
                return g_TRADE_IMMEDIATE_AT_TOUCH_BOLLEAN_DOWN;
            }
            set
            {
                g_TRADE_IMMEDIATE_AT_TOUCH_BOLLEAN_DOWN = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_MID
        {
            get
            {
                return g_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_MID;
            }
            set
            {
                g_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_MID = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_TRADE_IMMEDIATE_ON_CONTINUE_HIT_UPON_BOLLEAN_MID
        {
            get
            {
                return g_TRADE_IMMEDIATE_ON_CONTINUE_HIT_UPON_BOLLEAN_MID;
            }
            set
            {
                g_TRADE_IMMEDIATE_ON_CONTINUE_HIT_UPON_BOLLEAN_MID = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_IGNORE_CUR_TRADE_ON_NOT_CONTINUE_HIT_UPON_BOLLEAN_MID
        {
            get
            {
                return g_IGNORE_CUR_TRADE_ON_NOT_CONTINUE_HIT_UPON_BOLLEAN_MID;
            }
            set
            {
                g_IGNORE_CUR_TRADE_ON_NOT_CONTINUE_HIT_UPON_BOLLEAN_MID = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_TRADE_IMMEDIATE_ON_MISS_COUNT0_PATH
        {
            get
            {
                return g_TRADE_IMMEDIATE_ON_MISS_COUNT0_PATH;
            }

            set
            {
                g_TRADE_IMMEDIATE_ON_MISS_COUNT0_PATH = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_TRADE_IMMEDIATE_TORANCE_MAX_MISS_COUNT
        {
            get
            {
                return g_TRADE_IMMEDIATE_TORANCE_MAX_MISS_COUNT;
            }

            set
            {
                g_TRADE_IMMEDIATE_TORANCE_MAX_MISS_COUNT = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_GLOBAL_MAIN_THREAD_UPDATE_INTERVAL
        {
            get { return g_GLOBAL_MAIN_THREAD_UPDATE_INTERVAL; }
            set
            {
                g_GLOBAL_MAIN_THREAD_UPDATE_INTERVAL = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_SEQ_PATH_BY_MACD_SIGNAL
        {
            get { return g_SEQ_PATH_BY_MACD_SIGNAL; }
            set
            {
                g_SEQ_PATH_BY_MACD_SIGNAL = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_OVER_SPEC_MISS_COUNT
        {
            get { return g_OVER_SPEC_MISS_COUNT; }
            set { g_OVER_SPEC_MISS_COUNT = value; HAS_MODIFY = true; }
        }

        public static bool G_CALC_BOOLEAN_ANALYSE_DATA
        {
            get
            {
                return g_CALC_BOOLEAN_ANALYSE_DATA;
            }

            set
            {
                g_CALC_BOOLEAN_ANALYSE_DATA = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_UP
        {
            get { return g_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_UP; }
            set
            {
                g_IGNORE_CUR_TRADE_ON_NOT_AT_BOLLEAN_UP = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_MISS_COUNT_FIRST
        {
            get { return g_MISS_COUNT_FIRST; }
            set { g_MISS_COUNT_FIRST = value; HAS_MODIFY = true; }
        }

        public static int G_MISS_COUNT_SECOND
        {
            get
            { return g_MISS_COUNT_SECOND; }

            set
            {
                g_MISS_COUNT_SECOND = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_AUTO_REFRESH_LATEST_DATA
        {
            get { return g_AUTO_REFRESH_LATEST_DATA; }
            set
            {
                g_AUTO_REFRESH_LATEST_DATA = value;
                HAS_MODIFY = true;
            }
        }

        public static float G_AUTO_REFRESH_LATEST_DATA_INTERVAL
        {
            get { return g_AUTO_REFRESH_LATEST_DATA_INTERVAL; }
            set { g_AUTO_REFRESH_LATEST_DATA_INTERVAL = value; HAS_MODIFY = true; }
        }

        public static bool G_SHOW_KCURVE_DETAIL
        {
            get { return g_SHOW_KCURVE_DETAIL; }
            set { g_SHOW_KCURVE_DETAIL = value; HAS_MODIFY = true; }
        }

        public static bool G_SEQ_PATH_BY_MACD_LINE
        {
            get { return g_SEQ_PATH_BY_MACD_LINE; }
            set { g_SEQ_PATH_BY_MACD_LINE = value; HAS_MODIFY = true; }
        }

        public static bool G_SHOW_KCURVE_HOTNUMS_RESULT
        {
            get { return g_SHOW_KCURVE_HOTNUMS_RESULT; }
            set { g_SHOW_KCURVE_HOTNUMS_RESULT = value; HAS_MODIFY = true; }
        }

        public static bool G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_3
        {
            get { return g_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_3; }
            set { g_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_3 = value; HAS_MODIFY = true; }
        }

        public static bool G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_5
        {
            get { return g_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_5; }
            set { g_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_5 = value; HAS_MODIFY = true; }
        }

        public static bool G_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_10
        {
            get { return g_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_10; }
            set { g_USE_KCURVE_HOTNUMS_PREDICT_SAMPLE_10 = value; HAS_MODIFY = true; }
        }

        public static bool G_SIM_SEL_NUM_AT_POS_0
        {
            get
            {
                return g_SIM_SEL_NUM_AT_POS_0;
            }

            set
            {
                g_SIM_SEL_NUM_AT_POS_0 = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_SIM_SEL_NUM_AT_POS_1
        {
            get
            {
                return g_SIM_SEL_NUM_AT_POS_1;
            }

            set
            {
                g_SIM_SEL_NUM_AT_POS_1 = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_SIM_SEL_NUM_AT_POS_2
        {
            get
            {
                return g_SIM_SEL_NUM_AT_POS_2;
            }

            set
            {
                g_SIM_SEL_NUM_AT_POS_2 = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_SIM_SEL_NUM_AT_POS_3
        {
            get
            {
                return g_SIM_SEL_NUM_AT_POS_3;
            }

            set
            {
                g_SIM_SEL_NUM_AT_POS_3 = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_SIM_SEL_NUM_AT_POS_4
        {
            get
            {
                return g_SIM_SEL_NUM_AT_POS_4;
            }

            set
            {
                g_SIM_SEL_NUM_AT_POS_4 = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_SIM_SEL_MAX_COUNT
        {
            get
            {
                return g_SIM_SEL_MAX_COUNT;
            }

            set
            {
                g_SIM_SEL_MAX_COUNT = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_SIM_PAUSE_AT_BATCH_FINISH
        {
            get
            {
                return g_SIM_PAUSE_AT_BATCH_FINISH;
            }

            set
            {
                g_SIM_PAUSE_AT_BATCH_FINISH = value;
                HAS_MODIFY = true;
            }
        }

        public static HotNumPredictType G_KCURVE_HOTNUMS_PREDICT_TYPE
        {
            get
            {
                return g_KCURVE_HOTNUMS_PREDICT_TYPE;
            }

            set
            {
                g_KCURVE_HOTNUMS_PREDICT_TYPE = value;
                HAS_MODIFY = true;
            }
        }



        public static string ROOT_FOLDER
        {
            get
            {
                string path = "..\\";
#if UNITY_EDITOR_WIN || UNITY_EDITOR_WIN
                path = Application.persistentDataPath + "/LotteryAnalyze";
#elif UNITY_ANDROID
                path = "/mnt/sdcard/LotteryAnalyze";
#endif
                return path;
            }
        }

        public static float G_KGRAPH_CANVAS_SCALE_X
        {
            get
            {
                return g_KGRAPH_CANVAS_SCALE_X;
            }
            set
            {
                g_KGRAPH_CANVAS_SCALE_X = value;
                HAS_MODIFY = true;
            }
        }
        public static float G_KGRAPH_CANVAS_SCALE_Y
        {
            get
            {
                return g_KGRAPH_CANVAS_SCALE_Y;
            }
            set
            {
                g_KGRAPH_CANVAS_SCALE_Y = value;
                HAS_MODIFY = true;
            }
        }

        public static float G_MISSGRAPH_CANVAS_SCALE_X
        {
            get { return g_MISSGRAPH_CANVAS_SCALE_X; }
            set { g_MISSGRAPH_CANVAS_SCALE_X = value; HAS_MODIFY = true; }
            }
        public static float G_MISSGRAPH_CANVAS_SCALE_Y
        {
            get { return g_MISSGRAPH_CANVAS_SCALE_Y; }
            set { g_MISSGRAPH_CANVAS_SCALE_Y = value; HAS_MODIFY = true; }
        }

        public static float G_AUX_LINE_KEY_POINT_HIT_SIZE
        {
            get { return g_AUX_LINE_KEY_POINT_HIT_SIZE; }
            set { g_AUX_LINE_KEY_POINT_HIT_SIZE = value; HAS_MODIFY = true; }
        }

        public static float G_AUX_LINE_KEY_POINT_SEL_SIZE_SCALE
        {
            get { return g_AUX_LINE_KEY_POINT_SEL_SIZE_SCALE; }
            set { g_AUX_LINE_KEY_POINT_SEL_SIZE_SCALE = value; HAS_MODIFY = true; }
        }

        public static int G_COMMON_FONT_SIZE
        {
            get
            {
                return g_COMMON_FONT_SIZE;
            }

            set
            {
                g_COMMON_FONT_SIZE = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_BUTTON_FONT_SIZE
        {
            get
            {
                return g_BUTTON_FONT_SIZE;
            }

            set
            {
                g_BUTTON_FONT_SIZE = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_ENABLE_SHOW_KCURVE_PREDICT
        {
            get
            {
                return g_ENABLE_SHOW_KCURVE_PREDICT;
            }

            set
            {
                g_ENABLE_SHOW_KCURVE_PREDICT = value;
                HAS_MODIFY = true;
            }
        }

        public static int G_KCURVE_PREDICT_SAMPLE_COUNT
        {
            get
            {
                return g_KCURVE_PREDICT_SAMPLE_COUNT;
            }

            set
            {
                g_KCURVE_PREDICT_SAMPLE_COUNT = value;
                HAS_MODIFY = true;
            }
        }

        public static bool G_FORCE_HORZ_ALLIGN
        {
            get
            {
                return g_FORCE_HORZ_ALLIGN;
            }

            set
            {
                g_FORCE_HORZ_ALLIGN = value;
                HAS_MODIFY = true;
            }
        }

        public static float G_STATISTIC_CANVAS_SCALE_X
        {
            get
            {
                return g_STATISTIC_CANVAS_SCALE_X;
            }

            set
            {
                g_STATISTIC_CANVAS_SCALE_X = value;
                HAS_MODIFY = true;
            }
        }

        public static float G_STATISTIC_CANVAS_MIN_GRID_SIZE
        {
            get
            {
                return g_STATISTIC_CANVAS_MIN_GRID_SIZE;
            }

            set
            {
                g_STATISTIC_CANVAS_MIN_GRID_SIZE = value;
                HAS_MODIFY = true;
            }
        }

        public static float G_STATISTIC_CANVAS_SCALE_X_MAX
        {
            get
            {
                return g_STATISTIC_CANVAS_SCALE_X_MAX;
            }

            set
            {
                g_STATISTIC_CANVAS_SCALE_X_MAX = value;
                HAS_MODIFY = true;
            }
        }

        public static float G_TRADE_CANVAS_SCALE_X
        {
            get
            {
                return g_TRADE_CANVAS_SCALE_X;
            }

            set
            {
                g_TRADE_CANVAS_SCALE_X = value;
                HAS_MODIFY = true;
            }
        }

        public static float G_TRADE_CANVAS_SCALE_X_MAX
        {
            get
            {
                return g_TRADE_CANVAS_SCALE_X_MAX;
            }

            set
            {
                g_TRADE_CANVAS_SCALE_X_MAX = value;
                HAS_MODIFY = true;
            }
        }

        public static float G_TRADE_CANVAS_MIN_GRID_SIZE
        {
            get
            {
                return g_TRADE_CANVAS_MIN_GRID_SIZE;
            }

            set
            {
                g_TRADE_CANVAS_MIN_GRID_SIZE = value;
                HAS_MODIFY = true;
            }
        }

        static GlobalSetting()
        {
            string CFG_FILE = ROOT_FOLDER + "\\GlobalSetting.ini";
//#if UNITY_ANDROID
//            CFG_FILE = Environment.CurrentDirectory + "\\GlobalSetting.ini";
//#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
//            CFG_FILE =  "/mnt/sdcard/LotteryAnalyze" + "\\GlobalSetting.ini";
//#else
//            CFG_FILE = Environment.CurrentDirectory + "\\GlobalSetting.ini";
//#endif
            cfg = new IniFile(CFG_FILE);
            if(System.IO.File.Exists(CFG_FILE) == false)
            {
                SaveCfg(true);
            }
        }

        static void ReadParams()
        {
            FieldInfo[] fis = typeof(GlobalSetting).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            for (int i = 0; i < fis.Length; ++i)
            {
                FieldInfo fi = fis[i];
                Parameter par = Attribute.GetCustomAttribute(fi, typeof(Parameter)) as Parameter;
                if (par == null)
                    continue;

                if(fi.FieldType == typeof(int))
                {
                    int v = cfg.ReadInt("GlobalSetting", fi.Name, (int)par.defV);
                    fi.SetValue(Instance, v);
                }
                else if(fi.FieldType == typeof(bool))
                {
                    bool v = cfg.ReadBool("GlobalSetting", fi.Name, (bool)par.defV);
                    fi.SetValue(Instance, v);
                }
                else if (fi.FieldType == typeof(float))
                {
                    float v = cfg.ReadFloat("GlobalSetting", fi.Name, (float)par.defV);
                    fi.SetValue(Instance, v);
                }
                else if(fi.FieldType.IsEnum)
                {
                    int tv = cfg.ReadInt("GlobalSetting", fi.Name, (int)par.defV);
                    fi.SetValue(Instance, (object)(tv));

                    //if (fi.FieldType == typeof(AppearenceCheckType))
                    //{
                    //    AppearenceCheckType v = (AppearenceCheckType)tv;
                    //    fi.SetValue(Instance, v);
                    //}
                    //else if (fi.FieldType == typeof(AutoUpdateUtil.DataSourceType))
                    //{
                    //    AutoUpdateUtil.DataSourceType v = (AutoUpdateUtil.DataSourceType)tv;
                    //    fi.SetValue(Instance, v);
                    //}
                    //else if (fi.FieldType == typeof(CollectDataType))
                    //{
                    //    CollectDataType v = (CollectDataType)tv;
                    //    fi.SetValue(Instance, v);
                    //}
                    //else if (fi.FieldType == typeof(HotNumPredictType))
                    //{
                    //    HotNumPredictType v = (HotNumPredictType)tv;
                    //    fi.SetValue(Instance, v);
                    //}
                    //else
                    //{
                    //    MessageBox.Show(
                    //        "未注册解析枚举类型[ " + fi.FieldType + "]",
                    //        "警告",
                    //        MessageBoxButtons.OKCancel,
                    //        MessageBoxIcon.Error,
                    //        MessageBoxDefaultButton.Button1,
                    //        MessageBoxOptions.ServiceNotification);
                    //}
                }
            }
        }

        static void SaveParams()
        {
            FieldInfo[] fis = typeof(GlobalSetting).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            for (int i = 0; i < fis.Length; ++i)
            {
                FieldInfo fi = fis[i];
                Parameter par = Attribute.GetCustomAttribute(fi, typeof(Parameter)) as Parameter;
                if (par == null)
                    continue;

                object v = fi.GetValue(Instance);
                if (fi.FieldType == typeof(int))
                {
                    cfg.WriteInt("GlobalSetting", fi.Name, (int)v);
                }
                else if (fi.FieldType == typeof(bool))
                {
                    cfg.WriteBool("GlobalSetting", fi.Name, (bool)v);
                }
                else if (fi.FieldType == typeof(float))
                {
                    cfg.WriteFloat("GlobalSetting", fi.Name, (float)v);
                }
                else if (fi.FieldType.IsEnum)
                {
                    cfg.WriteInt("GlobalSetting", fi.Name, (int)v);
                }
            }
        }

        public static void ReadCfg()
        {
            ReadParams();

            TradeSets.Clear();
            TradeTags.Clear();
            string tradeNames = cfg.ReadString("TradeSets", "TradeNames", "");
            if(string.IsNullOrEmpty(tradeNames) == false)
            {
                string[] tags = tradeNames.Split(',');
                for(int i = 0; i < tags.Length; ++i)
                {
                    TradeTags.Add(tags[i]);

                    string tradeInfos = cfg.ReadString("TradeSets", tags[i], "");
                    if(string.IsNullOrEmpty(tradeInfos) == false)
                    {
                        List<int> lst = new List<int>();
                        string[] conts = tradeInfos.Split(',');
                        for(int j = 0; j < conts.Length; ++j)
                        {
                            lst.Add(int.Parse(conts[j]));
                        }
                        TradeSets.Add(lst);
                    }
                }
            }
            HAS_MODIFY = false;
        }

        public static void SaveCfg(bool forceSave = false)
        {
            if (HAS_MODIFY == false && forceSave == false)
                return;

            SaveParams();

            if (TradeSets.Count > 0)
            {
                string tradeNames = "";
                for( int i = 0; i < TradeTags.Count; ++i )
                {
                    string info = "";
                    List<int> lst = TradeSets[i];
                    for(int j = 0; j < lst.Count; ++j)
                    {
                        info += lst[j];
                        if(j < lst.Count-1)
                            info += ",";
                    }
                    tradeNames += TradeTags[i];
                    if(i < TradeTags.Count -1)
                        tradeNames += ",";
                    cfg.WriteString("TradeSets", TradeTags[i], ("\""+info+ "\""));
                }
                cfg.WriteString("TradeSets", "TradeNames", ("\""+tradeNames+ "\""));
            }
            HAS_MODIFY = false;
        }

        public static string WriteSettingToXMLString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t<GlobalSetting>\n");
            FieldInfo[] fis = typeof(GlobalSetting).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            for (int i = 0; i < fis.Length; ++i)
            {
                FieldInfo fi = fis[i];
                Parameter par = Attribute.GetCustomAttribute(fi, typeof(Parameter)) as Parameter;
                if (par != null)
                {
                    object v = fi.GetValue(GlobalSetting.Instance);
                    sb.Append("\t\t<" + fi.Name + " desc=\""+par.name+"\">" + v.ToString() + "</" + fi.Name + ">\n");
                }
            }
            sb.Append("\t</GlobalSetting>\n");
            return sb.ToString();
        }
    }
}
