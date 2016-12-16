using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    public class CollectorBase
    {
        public bool enable = true;

        public CollectorBase() { }
        public virtual string GetDesc() { return "基类收集器"; }
        public virtual void Collect() { throw new Exception("should override Collect() function!"); }
        public virtual void OutPutToTreeView(TreeNode parentNode) { throw new Exception("should override OutPutToTreeView() function!"); }
    }

    // 往期和值统计
    public class AndValueCollector : CollectorBase
    {
        public int totalCount;
        public Dictionary<int, int> andValueGraphTotal;
        public Dictionary<OneDayDatas, Dictionary<int, int>> oneDayAndValueGraph;

        public AndValueCollector()
        {
            totalCount = 0;
            andValueGraphTotal = new Dictionary<int, int>();
            oneDayAndValueGraph = new Dictionary<OneDayDatas, Dictionary<int, int>>();
        }

        public override string GetDesc() { return "往期和值统计"; }

        public override void Collect()
        {
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            andValueGraphTotal.Clear();
            oneDayAndValueGraph.Clear();
            totalCount = 0;
            for (int i = 0; i < count; ++i)
            {
                int oneDayID = DataManager.GetInst().indexs[i];
                OneDayDatas odd = DataManager.GetInst().allDatas[oneDayID];
                Dictionary<int, int> oddCol = new Dictionary<int, int>();
                oneDayAndValueGraph.Add(odd, oddCol);
                for (int j = 0; j < odd.datas.Count; ++j)
                {
                    ++totalCount;

                    int av = odd.datas[j].andValue;
                    if (oddCol.ContainsKey(av))
                        oddCol[av] = oddCol[av] + 1;
                    else
                        oddCol.Add(av, 1);

                    if (andValueGraphTotal.ContainsKey(av))
                        andValueGraphTotal[av] = andValueGraphTotal[av] + 1;
                    else
                        andValueGraphTotal.Add(av, 1);
                }
            }
        }

        public override void OutPutToTreeView(TreeNode parentNode) 
        {
            TreeNode mainNode = new TreeNode("整体数据");
            TreeNode oddNodes = new TreeNode("按期划分");
            parentNode.Nodes.Add(mainNode);
            parentNode.Nodes.Add(oddNodes);
            for (int i = 0; i <= 27; ++i)
            {
                string txt = "和值-" + i.ToString();
                int curAndValueCount = 0;
                if (andValueGraphTotal.ContainsKey(i))
                    curAndValueCount = andValueGraphTotal[i];
                txt += " [ " + curAndValueCount + " / " + totalCount + " ] = " + curAndValueCount * 100 / totalCount + "%";
                if (mainNode.Nodes.Count == 0 )
                {
                    TreeNode node = mainNode.Nodes.Add(txt);
                    node.Tag = curAndValueCount;
                }
                else
                {
                    bool hasAdd = false;
                    for (int nodeID = 0; nodeID < mainNode.Nodes.Count; ++nodeID)
                    {
                        TreeNode curNode = mainNode.Nodes[nodeID];
                        int colCount = (int)(curNode.Tag);
                        if( colCount <= curAndValueCount )
                        {
                            TreeNode node = mainNode.Nodes.Insert( nodeID, txt );
                            node.Tag = curAndValueCount;
                            hasAdd = true;
                            break;
                        }
                    }
                    if( !hasAdd )
                    {
                        TreeNode node = mainNode.Nodes.Add(txt);
                        node.Tag = curAndValueCount;
                    }
                }
            }
            foreach (OneDayDatas odd in oneDayAndValueGraph.Keys)
            {
                Dictionary<int, int> pl = oneDayAndValueGraph[odd];
                TreeNode oddnode = oddNodes.Nodes.Add(odd.dateID.ToString());
                int oddcount = odd.datas.Count;
                for (int i = 0; i <= 27; ++i)
                {
                    string txt = i.ToString();
                    int curAndValueCount = 0;
                    if (pl.ContainsKey(i))
                        curAndValueCount = pl[i];
                    txt += "[ " + curAndValueCount + " / " + oddcount + " ] = " + curAndValueCount * 100 / oddcount + "%";

                    if (oddnode.Nodes.Count == 0 )
                    {
                        TreeNode node = oddnode.Nodes.Add(txt);
                        node.Tag = curAndValueCount;
                    }
                    else
                    {
                        bool hasAdd = false;
                        for (int nodeID = 0; nodeID < oddnode.Nodes.Count; ++nodeID)
                        {
                            TreeNode curNode = oddnode.Nodes[nodeID];
                            int colCount = (int)(curNode.Tag);
                            if( colCount <= curAndValueCount )
                            {
                                TreeNode node = oddnode.Nodes.Insert( nodeID, txt );
                                node.Tag = curAndValueCount;
                                hasAdd = true;
                                break;
                            }
                        }
                        if( !hasAdd )
                        {
                            TreeNode node = oddnode.Nodes.Add(txt);
                            node.Tag = curAndValueCount;
                        }
                    }                    
                }
            }
        }
    }

    // 开出长组6之后的组3信息收集
    public class Group3AfterLongGroup6Collector : CollectorBase
    {
        public Group3AfterLongGroup6Collector()
        {

        }
        public override string GetDesc() { return "开出长组6之后的组3信息收集"; }
        public override void Collect() 
        {
        }
        public override void OutPutToTreeView(TreeNode parentNode) 
        { 
        }
    }

    public class StatisticsCollector
    {
        static List<CollectorBase> sCollectorList = new List<CollectorBase>();

        static StatisticsCollector()
        {
            sCollectorList.Add(new AndValueCollector());
            sCollectorList.Add(new Group3AfterLongGroup6Collector());
        }

        public static List<CollectorBase> CollectorList
        {
            get { return sCollectorList; }
        }

        public static void Collect()
        {
            for (int i = 0; i < sCollectorList.Count; ++i)
            {
                CollectorBase col = sCollectorList[i];
                if (col.enable)
                {
                    col.Collect();
                }
            }
        }
    }
}
