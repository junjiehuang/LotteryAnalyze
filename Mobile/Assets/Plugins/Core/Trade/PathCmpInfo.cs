using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    public class PathCmpInfo
    {
        public Dictionary<string, object> paramMap = new Dictionary<string, object>();
        public int pathIndex;
        public StatisticUnit su;

        public float pathValue;
        public int uponBMCount;
        public TradeDataManager.MACDLineWaveConfig macdLineCfg;
        public TradeDataManager.MACDBarConfig macdBarCfg;
        public TradeDataManager.KGraphConfig kGraphCfg;

        public bool isStrongUp;
        public int maxMissCount;

        public float avgPathValue = 0;

        public int horzDist = 0;
        public int vertDistToMinKValue = 0;
        public int vertDistToBML = 0;
        public int vertDistToBDL = 0;
        public int maxVertDist = 0;

        public PathCmpInfo(int _id, StatisticUnit _su)
        {
            pathIndex = _id;
            su = _su;
        }

        public PathCmpInfo(int id, float v, int _uponBMCount,
            TradeDataManager.MACDLineWaveConfig lineCFG,
            TradeDataManager.MACDBarConfig barCFG,
            TradeDataManager.KGraphConfig kCFG,
            StatisticUnit _su,
            bool _isStrongUp,
            int _maxMissCount)
        {
            pathIndex = id;
            pathValue = v;
            macdLineCfg = lineCFG;
            macdBarCfg = barCFG;
            kGraphCfg = kCFG;
            uponBMCount = _uponBMCount;
            su = _su;
            isStrongUp = _isStrongUp;
            maxMissCount = _maxMissCount;
        }

        public PathCmpInfo(int id, float v, int _uponBMCount)
        {
            pathIndex = id;
            pathValue = v;
            uponBMCount = _uponBMCount;
        }

        public PathCmpInfo(int id, float v)
        {
            pathIndex = id;
            pathValue = v;
        }

        public PathCmpInfo(int id, float v, int _horzDist, int _uponBMCount, int _vertDistToMinKValue, int _vertDistToBML, int _vertDistToBDL)
        {
            pathIndex = id;
            pathValue = v;
            horzDist = _horzDist;
            uponBMCount = _uponBMCount;
            vertDistToMinKValue = _vertDistToMinKValue;
            vertDistToBML = _vertDistToBML;
            vertDistToBDL = _vertDistToBDL;
            //maxVertDist = Math.Abs(vertDistToMaxMissV);
            //if (vertDistToBML < 0 && maxVertDist < -vertDistToBML)
            //    maxVertDist = Math.Abs(vertDistToBML);
            if (vertDistToBML < 0)
            {
                maxVertDist = vertDistToBML;
                if (vertDistToMinKValue < 0 && vertDistToMinKValue > vertDistToBML)
                    maxVertDist = vertDistToMinKValue;
            }
            else if (vertDistToBML > 0)
            {
                if (vertDistToMinKValue < 0)
                {
                    maxVertDist = -vertDistToMinKValue;
                    if (vertDistToBDL < 0 && vertDistToBDL > vertDistToMinKValue)
                        maxVertDist = -vertDistToBDL;
                }
                else
                    maxVertDist = 0xFFFFFF;
            }
            else
                maxVertDist = 0;
        }
    }
}
