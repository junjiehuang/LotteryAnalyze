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
        
        [Parameter("窗口透明度")]
        private static float g_WINDOW_OPACITY = 1;
        [Parameter("支撑压力线取样数")]
        private static int g_ANALYZE_TOOL_SAMPLE_COUNT = 30;
        [Parameter("曲线图刷新率")]
        private static int g_LOTTERY_GRAPH_UPDATE_INTERVAL = 1500;
        [Parameter("模拟图刷新率")]
        private static int g_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = 1500;
        [Parameter("是否开启分析工具")]
        private static bool g_EANBLE_ANALYZE_TOOL = true;
        [Parameter("是否开启同路保持检测")]
        private static bool g_ENABLE_CheckAndKeepSamePath = true;
        [Parameter("是否开启支撑压力检测")]
        private static bool g_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL = true;
        [Parameter("是否开启布林通道检测")]
        private static bool g_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE = true;
        [Parameter("1注1星交易成本")]
        private static float g_ONE_STARE_TRADE_COST = 1.0f;
        [Parameter("1注1星交易奖金")]
        private static float g_ONE_STARE_TRADE_REWARD = 9.8f;
        [Parameter("数据源类型")]
        public static AutoUpdateUtil.DataSourceType G_DATA_SOURCE_TYPE = AutoUpdateUtil.DataSourceType.e360;

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
                return g_ENABLE_CheckAndKeepSamePath;
            }

            set
            {
                g_ENABLE_CheckAndKeepSamePath = value;
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

            HAS_MODIFY = false;
        }
    }
}
