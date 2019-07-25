using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    // 图表数据容器基类
    public class GraphDataContainerBase
    {
        public virtual int DataLength() { return 0; }
        public virtual bool HasData() { return false; }
        public virtual void CollectGraphData() { }
        public virtual void CollectGraphDataExcept(OneDayDatas odd) { }
    }
}
