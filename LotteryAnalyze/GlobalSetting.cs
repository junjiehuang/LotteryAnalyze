using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public class Parameter : Attribute
    {
        public string name = "";

        public Parameter(string _name)
        {
            name = _name;
        }
    }

    class GlobalSetting
    {
        static bool HAS_MODIFY = false;

        [Parameter("是否在主线程刷新")]
        private static bool g_UPDATE_IN_MAIN_THREAD = true;
        
        [Parameter("窗口透明度")]
        private static float g_WINDOW_OPACITY = 1;
        [Parameter("支撑压力线取样数")]
        private static int g_ANALYZE_TOOL_SAMPLE_COUNT = 30;
        [Parameter("曲线图刷新时间(毫秒)")]
        private static int g_LOTTERY_GRAPH_UPDATE_INTERVAL = 1500;
        [Parameter("模拟图刷新时间(毫秒)")]
        private static int g_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = 1500;
        [Parameter("是否开启通道线分析工具")]
        private static bool g_EANBLE_ANALYZE_TOOL = true;
        [Parameter("是否开启同路保持检测")]
        private static bool g_ENABLE_CHECK_AND_KEEP_SAME_PATH = true;
        [Parameter("是否开启同路支撑压力检测")]
        private static bool g_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL = true;
        [Parameter("是否开启同路布林通道检测")]
        private static bool g_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE = true;
        [Parameter("是否开启布林下轨提升检测")]
        private static bool g_ENABLE_BOOLEAN_DOWN_UPWARD_CHECK = true;
        [Parameter("是否开启最大出现率检测")]
        private static bool g_ENABLE_MAX_APPEARENCE_FIRST_CHECK = true;
        [Parameter("是否开启布林中轨之上数量统计")]
        private static bool g_ENABLE_UPBOLLEAN_COUNT_STATISTIC = true;
        [Parameter("是否开启MACD上升检测")]
        private static bool g_ENABLE_MACD_UP_CHECK = true;
        //private static bool g_ENABLE_SAME_PATH_CHECK_MAX_DELTA_APPEAR_RATE = true;
        [Parameter("1注1星交易成本")]
        private static float g_ONE_STARE_TRADE_COST = 1.0f;
        [Parameter("1注1星交易奖金")]
        private static float g_ONE_STARE_TRADE_REWARD = 9.8f;
        [Parameter("选择交易策略ID")]
        private static int g_CUR_TRADE_INDEX = -1;
        [Parameter("每批加载多少天的数据")]
        private static int g_DAYS_PER_BATCH = 3;
        [Parameter("数据源类型")]
        private static AutoUpdateUtil.DataSourceType g_DATA_SOURCE_TYPE = AutoUpdateUtil.DataSourceType.e360;

        [Parameter("是否只交易指定的CollectionDataType")]
        private static bool g_ONLY_TRADE_SPEC_CDT = true;
        [Parameter("交易指定的CollectionDataType")]
        private static CollectDataType g_TRADE_SPEC_CDT = CollectDataType.ePath0;

        [Parameter("是否记录交易数据")]
        private static bool g_ENABLE_REC_TRADE_DATAS = true;


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

        public static bool G_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE
        {
            get
            {
                return g_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE;
            }

            set
            {
                g_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE = value;
                HAS_MODIFY = true;
            }
        }

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

        public static bool G_ENABLE_BOOLEAN_DOWN_UP_CHECK
        {
            get
            {
                return g_ENABLE_BOOLEAN_DOWN_UPWARD_CHECK;
            }

            set
            {
                g_ENABLE_BOOLEAN_DOWN_UPWARD_CHECK = value;
                HAS_MODIFY = true;
            }
        }

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

        public static bool G_ENABLE_UPBOLLEAN_COUNT_STATISTIC
        {
            get
            {
                return g_ENABLE_UPBOLLEAN_COUNT_STATISTIC;
            }

            set
            {
                g_ENABLE_UPBOLLEAN_COUNT_STATISTIC = value;
                HAS_MODIFY = true;
            }
        }

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

        static GlobalSetting()
        {
            cfg = new IniFile(Environment.CurrentDirectory + "\\GlobalSetting.ini");
            ReadCfg();
        }

        public static void ReadCfg()
        {
            G_WINDOW_OPACITY = cfg.ReadFloat("GlobalSetting", "WindowOpacity", 1);
            G_LOTTERY_GRAPH_UPDATE_INTERVAL = cfg.ReadInt("GlobalSetting", "LotteryGraphUpdateInterval", 1500);
            G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = cfg.ReadInt("GlobalSetting", "GlobalSimTradeUpdateInterval", 1500);
            G_ANALYZE_TOOL_SAMPLE_COUNT = cfg.ReadInt("GlobalSetting", "AnalyzeToolSampleCount", 30);
            G_EANBLE_ANALYZE_TOOL = cfg.ReadBool("GlobalSetting", "EnableAnalyzeTool", true);
            G_ENABLE_CheckAndKeepSamePath = cfg.ReadBool("GlobalSetting", "CheckAndKeepSamePath", true);
            G_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL = cfg.ReadBool("GlobalSetting", "EnableSamePathCheckByAnalyzeTool", true);
            G_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE = cfg.ReadBool("GlobalSetting", "EnableSamePathCheckByBooleanLine", true);
            G_DATA_SOURCE_TYPE = (AutoUpdateUtil.DataSourceType)cfg.ReadInt("GlobalSetting", "DataSourceType", 0);
            G_ONE_STARE_TRADE_COST = cfg.ReadFloat("GlobalSetting", "OneStartTradeCost", 1);
            G_ONE_STARE_TRADE_REWARD = cfg.ReadFloat("GlobalSetting", "OneStartTradeReward", 9.8f);
            G_ENABLE_BOOLEAN_DOWN_UP_CHECK = cfg.ReadBool("GlobalSetting", "EnableBooleanDownUpCheck", false);
            G_ENABLE_MAX_APPEARENCE_FIRST = cfg.ReadBool("GlobalSetting", "EnableMaxAppearenceFirstCheck", false);
            G_ENABLE_UPBOLLEAN_COUNT_STATISTIC = cfg.ReadBool("GlobalSetting", "EnableUpBolleanCountStatistic", false);
            G_ENABLE_MACD_UP_CHECK = cfg.ReadBool("GlobalSetting", "EnableMACDUpCheck", false);
            G_DATA_SOURCE_TYPE = (AutoUpdateUtil.DataSourceType)cfg.ReadInt("GlobalSetting", "DataSourceType", 1);
            G_DAYS_PER_BATCH = cfg.ReadInt("GlobalSetting", "DaysPerBatch", 3);
            G_ENABLE_REC_TRADE_DATAS = cfg.ReadBool("GlobalSetting", "EnableRecTradeDatas", true);

            G_ONLY_TRADE_SPEC_CDT = cfg.ReadBool("GlobalSetting", "OnlyTradeSpecCDT", false);
            G_TRADE_SPEC_CDT = (CollectDataType)cfg.ReadInt("GlobalSetting", "TradeSpecCDT", (int)(CollectDataType.ePath0));

            G_CUR_TRADE_INDEX = cfg.ReadInt("TradeSets", "CurTradeIndex", -1);
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

            cfg.WriteFloat("GlobalSetting", "WindowOpacity", G_WINDOW_OPACITY);
            cfg.WriteInt("GlobalSetting", "LotteryGraphUpdateInterval", G_LOTTERY_GRAPH_UPDATE_INTERVAL);
            cfg.WriteInt("GlobalSetting", "GlobalSimTradeUpdateInterval", G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL);
            cfg.WriteInt("GlobalSetting", "AnalyzeToolSampleCount", G_ANALYZE_TOOL_SAMPLE_COUNT);
            cfg.WriteBool("GlobalSetting", "EnableAnalyzeTool", G_EANBLE_ANALYZE_TOOL);
            cfg.WriteBool("GlobalSetting", "CheckAndKeepSamePath", G_ENABLE_CheckAndKeepSamePath);
            cfg.WriteBool("GlobalSetting", "EnableSamePathCheckByAnalyzeTool", G_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL);
            cfg.WriteBool("GlobalSetting", "EnableSamePathCheckByBooleanLine", G_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE);
            cfg.WriteInt("GlobalSetting", "DataSourceType", (int)G_DATA_SOURCE_TYPE);
            cfg.WriteFloat("GlobalSetting", "OneStartTradeCost", G_ONE_STARE_TRADE_COST);
            cfg.WriteFloat("GlobalSetting", "OneStartTradeReward", G_ONE_STARE_TRADE_REWARD);
            cfg.WriteBool("GlobalSetting", "EnableBooleanDownUpCheck", G_ENABLE_BOOLEAN_DOWN_UP_CHECK);
            cfg.WriteBool("GlobalSetting", "EnableMaxAppearenceFirstCheck", G_ENABLE_MAX_APPEARENCE_FIRST);
            cfg.WriteBool("GlobalSetting", "EnableUpBolleanCountStatistic", G_ENABLE_UPBOLLEAN_COUNT_STATISTIC);
            cfg.WriteBool("GlobalSetting", "EnableMACDUpCheck", G_ENABLE_MACD_UP_CHECK);
            cfg.WriteInt("GlobalSetting", "DataSourceType", (int)G_DATA_SOURCE_TYPE);
            cfg.WriteInt("GlobalSetting", "DaysPerBatch", G_DAYS_PER_BATCH);
            cfg.WriteBool("GlobalSetting", "EnableRecTradeDatas", G_ENABLE_REC_TRADE_DATAS);
            cfg.WriteBool("GlobalSetting", "OnlyTradeSpecCDT", G_ONLY_TRADE_SPEC_CDT);
            cfg.WriteInt("GlobalSetting", "TradeSpecCDT", (int)G_TRADE_SPEC_CDT);

            cfg.WriteInt("TradeSets", "CurTradeIndex", G_CUR_TRADE_INDEX);

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
    }
}
