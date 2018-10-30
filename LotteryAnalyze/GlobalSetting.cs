using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    class GlobalSetting
    {
        public static float G_WINDOW_OPACITY = 1;
        public static int G_ANALYZE_TOOL_SAMPLE_COUNT = 30;
        public static int G_LOTTERY_GRAPH_UPDATE_INTERVAL = 1500;
        public static int G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = 1500;

        public static bool G_EANBLE_ANALYZE_TOOL = true;
        public static bool G_ENABLE_CheckAndKeepSamePath = true;
        public static bool G_ENABLE_SAME_PATH_CHECK_BY_ANALYZE_TOOL = true;
        public static bool G_ENABLE_SAME_PATH_CHECK_BY_BOOLEAN_LINE = true;

        public static float G_ONE_STARE_TRADE_COST = 1.0f;
        public static float G_ONE_STARE_TRADE_REWARD = 9.8f;

        public static AutoUpdateUtil.DataSourceType G_DATA_SOURCE_TYPE = AutoUpdateUtil.DataSourceType.e360;

        static IniFile cfg = null;

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
        }

        public static void SaveCfg()
        {
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
        }
    }
}
