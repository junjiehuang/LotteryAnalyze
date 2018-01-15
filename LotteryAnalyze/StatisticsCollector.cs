using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    public class CollectTag
    {
        public int curAndValueCount = -1;

        public int itemIndex = -1;
        public int continueCount = -1;
        public CollectTag(int andValueCount, int itemID) 
        {
            curAndValueCount = andValueCount;
            itemIndex = itemID;
        }
    }

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
            if (totalCount <= 0)
                return;
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
                    node.Tag = new CollectTag(curAndValueCount, -1);
                }
                else
                {
                    bool hasAdd = false;
                    for (int nodeID = 0; nodeID < mainNode.Nodes.Count; ++nodeID)
                    {
                        TreeNode curNode = mainNode.Nodes[nodeID];
                        int colCount = (curNode.Tag as CollectTag).curAndValueCount;
                        if( colCount <= curAndValueCount )
                        {
                            TreeNode node = mainNode.Nodes.Insert( nodeID, txt );
                            node.Tag = new CollectTag(curAndValueCount,-1);
                            hasAdd = true;
                            break;
                        }
                    }
                    if( !hasAdd )
                    {
                        TreeNode node = mainNode.Nodes.Add(txt);
                        node.Tag = new CollectTag(curAndValueCount, -1);
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
                        node.Tag = new CollectTag(curAndValueCount, -1);
                    }
                    else
                    {
                        bool hasAdd = false;
                        for (int nodeID = 0; nodeID < oddnode.Nodes.Count; ++nodeID)
                        {
                            TreeNode curNode = oddnode.Nodes[nodeID];
                            int colCount = (curNode.Tag as CollectTag).curAndValueCount;
                            if( colCount <= curAndValueCount )
                            {
                                TreeNode node = oddnode.Nodes.Insert( nodeID, txt );
                                node.Tag = new CollectTag(curAndValueCount, -1);
                                hasAdd = true;
                                break;
                            }
                        }
                        if( !hasAdd )
                        {
                            TreeNode node = oddnode.Nodes.Add(txt);
                            node.Tag = new CollectTag(curAndValueCount, -1);
                        }
                    }                    
                }
            }
        }
    }

    // 开出长组6之后的组3信息收集
    public class Group3AfterLongGroup6Collector : CollectorBase
    {
        public class G3AfterG6Info
        {
            public int mG6CountContinuity = 0;
            public int mCollectCountAfter = 0;
            public string mStartID = "";
            public int mG3CountAfter = 0;
            public int mG1CountAfter = 0;
            public int mItemIndex = -1;
            public G3AfterG6Info()
            {
            }
        }
        List<G3AfterG6Info> infoList = new List<G3AfterG6Info>();

        public Group3AfterLongGroup6Collector()
        {
        }
        public override string GetDesc() { return "开出长组6之后的组3信息收集"; }
        public override void Collect() 
        {
            G3AfterG6Info curInfo = null;
            infoList.Clear();
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            int oneDayID = DataManager.GetInst().indexs[0];
            OneDayDatas odd = DataManager.GetInst().allDatas[oneDayID];
            DataItem curItem = odd.GetFirstItem();
            while (curItem != null)
            {
                if (curInfo == null)
                {
                    if (curItem.groupType == GroupType.eGT6)
                    {
                        curInfo = new G3AfterG6Info();
                        curInfo.mG6CountContinuity++;
                        curInfo.mStartID = curItem.idTag;
                        curInfo.mItemIndex = curItem.idGlobal;
                    }
                }
                else
                {
                    if (curItem.groupType == GroupType.eGT6)
                    {
                        curInfo.mG6CountContinuity++;
                    }
                    else
                    {
                        RecordInfo(curInfo, curItem);
                        infoList.Add(curInfo);
                        curInfo = null;
                    }
                }
                curItem = DataManager.GetInst().GetNextItem(curItem);
            }
            infoList.Sort((x, y) =>
                {
                    if (x.mG6CountContinuity > y.mG6CountContinuity)
                        return -1;
                    return 1;
                });
        }
        void RecordInfo(G3AfterG6Info curInfo, DataItem curItem)
        {
            DataItem checkItem = curItem;
            curInfo.mCollectCountAfter = 0;
            while (true)
            {
                if (checkItem.groupType == GroupType.eGT1)
                    curInfo.mG1CountAfter++;
                else if (checkItem.groupType == GroupType.eGT3)
                    curInfo.mG3CountAfter++;
                curInfo.mCollectCountAfter++;
                if (curInfo.mCollectCountAfter == curInfo.mG6CountContinuity)
                    break;
                checkItem = DataManager.GetInst().GetNextItem(checkItem);
                if (checkItem == null)
                    break;
            }
        }
        public override void OutPutToTreeView(TreeNode parentNode) 
        {
            for (int i = 0; i < infoList.Count; ++i)
            {
                G3AfterG6Info info = infoList[i];
                string txt = "[前组6:" + info.mG6CountContinuity + "] [组1:" + info.mG1CountAfter + "] [组3:" + info.mG3CountAfter + "] [共:" + info.mCollectCountAfter + "] [起始:" + info.mStartID + "]";
                TreeNode node = parentNode.Nodes.Add(txt);
                CollectTag tag = new CollectTag(-1, info.mItemIndex);
                tag.continueCount = info.mCollectCountAfter + info.mG6CountContinuity;
                node.Tag = tag;
            }
        }
    }

    // 往期开出类型统计
    public class GroupTypeCollector : CollectorBase
    {
        public class GTCount
        {
            public int totalCount = 0;
            public int g6Count = 0;
            public int g3Count = 0;
            public int g1Count = 0;
            public GTCount() { }
            public void Reset()
            {
                int totalCount = 0;
                int g6Count = 0;
                int g3Count = 0;
                int g1Count = 0;
            }
        }
        GTCount total = new GTCount();
        Dictionary<OneDayDatas, GTCount> infoList = new Dictionary<OneDayDatas, GTCount>();

        public GroupTypeCollector() { }
        public override string GetDesc()
        {
            return "往期开出组六，组三，豹子次数统计";
        }
        public override void Collect()
        {
            total.Reset();
            infoList.Clear();
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            for (int i = 0; i < count; ++i)
            {
                int oneDayID = DataManager.GetInst().indexs[i];
                OneDayDatas odd = DataManager.GetInst().allDatas[oneDayID];
                GTCount gtc = new GTCount();
                infoList.Add(odd, gtc);
                for (int j = 0; j < odd.datas.Count; ++j)
                {
                    DataItem item = odd.datas[j];
                    switch (item.groupType)
                    {
                        case GroupType.eGT1: gtc.g1Count++; break;
                        case GroupType.eGT3: gtc.g3Count++; break;
                        case GroupType.eGT6: gtc.g6Count++; break;
                    }
                    gtc.totalCount++;
                }
                total.totalCount += gtc.totalCount;
                total.g1Count += gtc.g1Count;
                total.g3Count += gtc.g3Count;
                total.g6Count += gtc.g6Count;
            }

        }
        public override void OutPutToTreeView(TreeNode parentNode)
        {
            TreeNode mainNode = new TreeNode("整体数据");
            TreeNode oddNodes = new TreeNode("按期划分");
            parentNode.Nodes.Add(mainNode);
            parentNode.Nodes.Add(oddNodes);
            string txt = "[总:" + total.totalCount + "] [组六:" + total.g6Count + "] [组三:" + total.g3Count + "] [豹子:" + total.g1Count + "]";
            mainNode.Nodes.Add(txt);
            foreach (OneDayDatas odd in infoList.Keys)
            {
                GTCount gtc = infoList[odd];
                txt = odd.dateID.ToString() + " [总:" + gtc.totalCount + "] [组六:" + gtc.g6Count + "] [组三:" + gtc.g3Count + "] [豹子:" + gtc.g1Count + "]";
                oddNodes.Nodes.Add(txt);
            }
        }
    }

    // 针对每个位的出号按012路统计最大遗漏值
    public class SinglePath012MaxMissingCollector : CollectorBase
    {
        public class MissingInfo
        {
            public int[] maxPath012MissingData = new int[3];
            public int[] maxPath012MissingID = new int[3];
        }

        string[] SingleNames = new string[] { "万", "千", "百", "十", "个", }; 
        public List<MissingInfo> maxPath012Missing = new List<MissingInfo>(); 


        public SinglePath012MaxMissingCollector()
        {
            maxPath012Missing.Clear();
            for (int i = 0; i < 5; ++i)
            {
                maxPath012Missing.Add(new MissingInfo());
            }
        }
        public override string GetDesc()
        {
            return "针对每个位的出号按012路统计最大遗漏值";
        }
        public override void Collect()
        {
            Util.CollectPath012Info(maxPath012Missing);
        }

        public override void OutPutToTreeView(TreeNode parentNode)
        {
            TreeNode mainNode = new TreeNode("每位012路最大遗漏值统计");
            parentNode.Nodes.Add(mainNode);
            for (int i = 0; i < 5; ++i)
            {
                TreeNode sub = mainNode.Nodes.Add(SingleNames[i]);
                for ( int j = 0; j < 3; ++j)
                {
                    TreeNode node = sub.Nodes.Add(j + " 路最大遗漏 = " + maxPath012Missing[i].maxPath012MissingData[j] + "(" + maxPath012Missing[i].maxPath012MissingID[j] + ")");
                    node.Tag = new CollectTag(-1, maxPath012Missing[i].maxPath012MissingID[j]);
                }
            }

            TreeNode dayNodes = new TreeNode("每日012路个数统计");
            parentNode.Nodes.Add(dayNodes);
            if (DataManager.GetInst().indexs == null) return;
            int count = DataManager.GetInst().indexs.Count;
            if (count == 0) return;
            for (int i = 0; i < count; ++i)
            {
                int oneDayID = DataManager.GetInst().indexs[i];
                OneDayDatas odd = DataManager.GetInst().allDatas[oneDayID];
                TreeNode dn = new TreeNode(odd.dateID.ToString());
                dayNodes.Nodes.Add(dn);
                for (int j = 0; j < 5; ++j)
                {
                    TreeNode sn = new TreeNode(SingleNames[j] + " - " + odd.simData.path012CountInfoShort[j][0] + ":" +
                    odd.simData.path012CountInfoShort[j][1] + ":" +
                    odd.simData.path012CountInfoShort[j][2]);
                    dn.Nodes.Add(sn);
                }
            }
        }
    }

    public class StatisticsCollector
    {
        static List<CollectorBase> sCollectorList = new List<CollectorBase>();

        static StatisticsCollector()
        {
            sCollectorList.Add(new AndValueCollector());
            sCollectorList.Add(new Group3AfterLongGroup6Collector());
            sCollectorList.Add(new GroupTypeCollector());
            sCollectorList.Add(new SinglePath012MaxMissingCollector());
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
