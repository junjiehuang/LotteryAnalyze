#define TRADE_DBG

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
#region Graph 
    




    #region Aux Lines







    #endregion

    #region Graphs













    #endregion

    // 图表管理器
    public class GraphManager
    {
        public class FavoriteChart
        {
            public int numIndex;
            public CollectDataType cdt;
            public string tag;
        }

        List<FavoriteChart> favoriteCharts = new List<FavoriteChart>();
        Dictionary<GraphType, GraphBase> sGraphMap = new Dictionary<GraphType, GraphBase>();
        GraphBase curGraph = null;
        GraphType curGraphType = GraphType.eNone;
        public GraphBar barGraph;
        public GraphKCurve kvalueGraph;
        public GraphTrade tradeGraph;
        public GraphAppearence appearenceGraph;
        public GraphMissCount missCountGraph;
        public int endShowDataItemIndex = -1;
        Form window;

        public GraphManager(Form _window)
        {
            window = _window;
            barGraph = new GraphBar(); barGraph.parent = this;
            kvalueGraph = new GraphKCurve(); kvalueGraph.parent = this;
            tradeGraph = new GraphTrade(); tradeGraph.parent = this;
            appearenceGraph = new GraphAppearence(); appearenceGraph.parent = this;
            missCountGraph = new GraphMissCount(); missCountGraph.parent = this;
            sGraphMap.Add(GraphType.eKCurveGraph, kvalueGraph);
            sGraphMap.Add(GraphType.eBarGraph, barGraph);
            sGraphMap.Add(GraphType.eTradeGraph, tradeGraph);
            sGraphMap.Add(GraphType.eAppearenceGraph, appearenceGraph);
            sGraphMap.Add(GraphType.eMissCountGraph, missCountGraph);
        }

        public void MakeWindowRepaint()
        {
            window.Invalidate(true);
        }

        public void OnTradeCompleted()
        {
            if (endShowDataItemIndex != -1)
                endShowDataItemIndex++;
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
        public void MoveGraph(float dx, float dy)
        {
            if (curGraph != null)
                curGraph.MoveGraph(dx, dy);
        }
        public void ResetGraphPosition()
        {
            if (curGraph != null)
                curGraph.ResetGraphPosition();
        }
        public void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            if (curGraph != null)
                curGraph.DrawUpGraph(g, numIndex, cdt, winW, winH, mouseRelPos);
        }
        public void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            if (curGraph != null)
                curGraph.DrawDownGraph(g, numIndex, cdt, winW, winH, mouseRelPos);
        }

        public FavoriteChart AddFavoriteChart(int numIndex, CollectDataType cdt)
        {
            if(FindFavoriteChart(numIndex, cdt) == null)
            {
                FavoriteChart fc = new FavoriteChart();
                fc.numIndex = numIndex;
                fc.cdt = cdt;
                int cdtID = GraphDataManager.S_CDT_LIST.IndexOf(cdt);
                fc.tag = TradeDataBase.NUM_TAGS[numIndex] + "_" + GraphDataManager.S_CDT_TAG_LIST[cdtID];
                favoriteCharts.Add(fc);
                return fc;
            }
            return null;
        }
        public FavoriteChart FindFavoriteChart(int numIndex, CollectDataType cdt)
        {
            for (int i = 0; i < favoriteCharts.Count; ++i)
            {
                if (favoriteCharts[i].numIndex == numIndex && favoriteCharts[i].cdt == cdt)
                    return favoriteCharts[i];
            }
            return null;
        }
        public FavoriteChart GetFavoriteChart(int index)
        {
            if (index >= 0 && index < favoriteCharts.Count)
                return favoriteCharts[index];
            return null;
        }
        public void ClearFavoriteCharts()
        {
            favoriteCharts.Clear();
        }
        public void RemoveFavoriteChart(string tag)
        {
            for( int i = 0; i < favoriteCharts.Count; ++i )
            {
                if(favoriteCharts[i].tag == tag)
                {
                    favoriteCharts.RemoveAt(i);
                    return;
                }
            }
        }
    }

#endregion
}