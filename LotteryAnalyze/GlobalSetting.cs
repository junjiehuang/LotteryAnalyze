using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    class GlobalSetting
    {
        public static float G_WINDOW_OPACITY = 1;
        public static int G_LOTTERY_GRAPH_UPDATE_INTERVAL = 1500;
        public static int G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL = 1500;

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
        }

        public static void SaveCfg()
        {
            cfg.WriteFloat("GlobalSetting", "WindowOpacity", G_WINDOW_OPACITY);
            cfg.WriteInt("GlobalSetting", "LotteryGraphUpdateInterval", G_LOTTERY_GRAPH_UPDATE_INTERVAL);
            cfg.WriteInt("GlobalSetting", "GlobalSimTradeUpdateInterval", G_GLOBAL_SIM_TRADE_UPDATE_INTERVAL);
        }
    }
}
