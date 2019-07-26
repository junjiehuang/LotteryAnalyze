using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{

    /// <summary>
    /// 批量模拟交易
    /// </summary>
    public class BatchTradeSimulator
    {
        enum SimState
        {
            eNone,
            // 准备原始数据和分析数据状态
            ePrepareData,
            // 模拟交易阶段
            eSimTrade,
            // 暂停模拟交易
            eSimPause,
            // 针对当前这批原始数据的交易模拟完成
            eFinishBatch,
            // 针对所有的原始数据的交易模拟结束
            eFinishAll,
        }

        static BatchTradeSimulator sInst;
        public static BatchTradeSimulator Instance
        {
            get
            {
                if (sInst == null)
                    sInst = new BatchTradeSimulator();
                return sInst;
            }
        }


        //public Dictionary<int, int> missCountInfos = new Dictionary<int, int>();
        public Dictionary<CollectDataType, Dictionary<int, int>> missCountInfos = new Dictionary<CollectDataType, Dictionary<int, int>>();
        public Dictionary<int, int> tradeMissInfo = new Dictionary<int, int>();
        List<int> fileIDLst = new List<int>();
        int lastIndex = -1;
        SimState _state = SimState.eNone;
        SimState state
        {
            get { return _state; }
            set
            {
                _state = value;
                if (value == SimState.eFinishAll)
                {
                    if (onTradeSimulateCompleted != null)
                    {
                        onTradeSimulateCompleted();
                    }
                }
            }
        }

        string lastTradeIDTag = null;
        public string LastTradeIDTag
        {
            get { return lastTradeIDTag; }
        }
        int lastTradeCountIndex = -1;
        DataItem curTradeItem;
        SimState backUpState = SimState.eNone;

        public int batch = 5;
        public float currentMoney;
        public float startMoney = 2000;
        public float minMoney;
        public float maxMoney;
        public int totalCount;
        public int tradeRightCount;
        public int tradeWrongCount;
        public int untradeCount;

        public delegate void CallBackOnPrepareDataItems(DataItem startTradeItem);
        public static CallBackOnPrepareDataItems onPrepareDataItems;

        public delegate void CallBackOnCompleted();
        public static CallBackOnCompleted onTradeSimulateCompleted;

        public BatchTradeSimulator()
        {

        }

        void RecordTradeDatas()
        {
            if (GlobalSetting.G_ENABLE_REC_TRADE_DATAS == false)
                return;
            var etor = TradeDataManager.Instance.longWrongTradeInfo.GetEnumerator();
            while (etor.MoveNext())
            {
                List<LongWrongTradeInfo> lst = etor.Current.Value;
                for (int i = 0; i < lst.Count; ++i)
                {
                    LongWrongTradeInfo info = lst[i];
                    if (info.tradeDatas != null && info.tradeDatas.Count > 0)
                        continue;
                    info.tradeDatas = new List<string>();
                    DataItem itemS = DataManager.GetInst().GetDataItemByIdTag(info.startDataItemTag);
                    DataItem itemE = DataManager.GetInst().GetDataItemByIdTag(info.endDataItemTag);
                    itemS = itemS.parent.GetFirstItem();
                    itemE = itemE.parent.GetTailItem();
                    while (itemS != itemE)
                    {
                        if (itemS.tag != null)
                        {
                            TradeDataBase t = itemS.tag as TradeDataBase;
                            info.tradeDatas.Add(t.GetTradeXML());
                        }
                        itemS = itemS.parent.GetNextItem(itemS);
                    }
                }
            }
        }

        public void OnOneTradeCompleted(TradeDataBase trade)
        {
            bool tradeSuccess = trade.reward > 0;
            if (tradeSuccess)
            {
                if (TradeDataManager.Instance.continueTradeMissCount > 0)
                {
                    if (tradeMissInfo.ContainsKey(TradeDataManager.Instance.continueTradeMissCount))
                        tradeMissInfo[TradeDataManager.Instance.continueTradeMissCount] = tradeMissInfo[TradeDataManager.Instance.continueTradeMissCount] + 1;
                    else
                        tradeMissInfo.Add(TradeDataManager.Instance.continueTradeMissCount, 1);

                    if (TradeDataManager.Instance.continueTradeMissCount >= TradeDataManager.Instance.tradeCountList.Count)
                    {
                        TradeDataManager.Instance.tmpLongWrongTradeInfo.endDataItemTag = trade.targetLotteryItem.idTag;
                        TradeDataManager.Instance.tmpLongWrongTradeInfo.count = TradeDataManager.Instance.continueTradeMissCount;
                        TradeDataManager.Instance.tmpLongWrongTradeInfo.tradeID = trade.INDEX;
                        if (TradeDataManager.Instance.longWrongTradeCallBack != null)
                            TradeDataManager.Instance.longWrongTradeCallBack(TradeDataManager.Instance.tmpLongWrongTradeInfo);
                        List<LongWrongTradeInfo> lst = null;
                        if (TradeDataManager.Instance.longWrongTradeInfo.ContainsKey(TradeDataManager.Instance.continueTradeMissCount))
                            lst = TradeDataManager.Instance.longWrongTradeInfo[TradeDataManager.Instance.continueTradeMissCount];
                        else
                        {
                            lst = new List<LongWrongTradeInfo>();
                            TradeDataManager.Instance.longWrongTradeInfo.Add(TradeDataManager.Instance.continueTradeMissCount, lst);
                        }
                        lst.Add(TradeDataManager.Instance.tmpLongWrongTradeInfo);
                        TradeDataManager.Instance.tmpLongWrongTradeInfo = null;
                    }
                }
                TradeDataManager.Instance.continueTradeMissCount = 0;
                if (TradeDataManager.Instance.tmpLongWrongTradeInfo != null)
                    TradeDataManager.Instance.tmpLongWrongTradeInfo.startDataItemTag = null;
            }
            else
            {
                if (TradeDataManager.Instance.tmpLongWrongTradeInfo == null)
                    TradeDataManager.Instance.tmpLongWrongTradeInfo = new LongWrongTradeInfo();
                if (string.IsNullOrEmpty(TradeDataManager.Instance.tmpLongWrongTradeInfo.startDataItemTag))
                    TradeDataManager.Instance.tmpLongWrongTradeInfo.startDataItemTag = trade.targetLotteryItem.idTag;
                if (trade.cost > 0)
                    ++TradeDataManager.Instance.continueTradeMissCount;
            }

            for (int i = 0; i < 5; ++i)
            {
                StatisticUnitMap sum = trade.lastDateItem.statisticInfo.allStatisticInfo[i];
                for (int j = 0; j < GraphDataManager.S_CDT_LIST.Count; ++j)
                {
                    CollectDataType cdt = GraphDataManager.S_CDT_LIST[j];
                    StatisticUnit su = sum.statisticUnitMap[cdt];
                    Dictionary<int, int> lst = null;
                    if (missCountInfos.ContainsKey(cdt))
                        lst = missCountInfos[cdt];
                    else
                    {
                        lst = new Dictionary<int, int>();
                        missCountInfos[cdt] = lst;
                    }

                    if (lst.ContainsKey(su.missCount))
                    {
                        lst[su.missCount] = lst[su.missCount] + 1;
                    }
                    else
                    {
                        lst[su.missCount] = 1;
                    }
                }
            }
        }

        public int GetMainProgress()
        {
            if (state == SimState.eFinishAll)
                return 100;
            else if (lastIndex < 0 || fileIDLst.Count == 0)
                return 0;
            else
                return (lastIndex * 100 / fileIDLst.Count);
        }
        public int GetBatchProgress()
        {
            if (state == SimState.ePrepareData)
                return 0;
            else if (state == SimState.eFinishBatch || state == SimState.eFinishAll)
                return 100;
            int totalItemCount = DataManager.GetInst().GetAllDataItemCount();
            if (totalItemCount == 0)
                return 0;
            int v = TradeDataManager.Instance.historyTradeDatas.Count * 100 / totalItemCount;
            return v;
        }

        public void ResetTradeInfo()
        {
            missCountInfos.Clear();
            lastTradeIDTag = "";
            tradeMissInfo.Clear();
            TradeDataManager.Instance.longWrongTradeInfo.Clear();
            TradeDataManager.Instance.tmpLongWrongTradeInfo = null;
            TradeDataManager.Instance.startMoney = startMoney;
            TradeDataManager.Instance.StopAtTheLatestItem = true;
            minMoney = maxMoney = currentMoney = startMoney;
            totalCount = tradeRightCount = tradeWrongCount = untradeCount = 0;
        }


        public void Start(ref int startDateID, ref int endDateID)
        {
            GlobalSetting.IsCurrentFetchingLatestData = false;

            ResetTradeInfo();
            fileIDLst.Clear();
            DataManager dm = DataManager.GetInst();
            foreach (int id in dm.mFileMetaInfo.Keys)
            {
                if (startDateID != -1 && id < startDateID)
                    continue;
                if (endDateID != -1 && id > endDateID)
                    continue;
                fileIDLst.Add(id);
            }
            fileIDLst.Sort();
            state = SimState.ePrepareData;
            lastIndex = -1;
            if (fileIDLst.Count > 0)
            {
                startDateID = fileIDLst[0];
                endDateID = fileIDLst[fileIDLst.Count - 1];
            }
        }

        public void Update()
        {
            switch (state)
            {
                case SimState.ePrepareData:
                    DoPrepareData();
                    break;
                case SimState.eSimTrade:
                    DoSimTrade();
                    break;
                case SimState.eFinishBatch:
                    DoFinishBatch();
                    break;
            }
        }

        public bool IsPause()
        {
            return state == SimState.eSimPause;
        }

        public void Pause()
        {
            backUpState = state;
            state = SimState.eSimPause;
            TradeDataManager.Instance.PauseAutoTradeJob();
        }
        public void Resume()
        {
            state = backUpState;
            TradeDataManager.Instance.ResumeAutoTradeJob();
        }
        public void Stop()
        {
            TradeDataManager.Instance.StopAutoTradeJob();
            DoFinishBatch();
            state = SimState.eFinishAll;
            lastIndex = fileIDLst.Count;
        }
        public bool HasFinished()
        {
            return state == SimState.eFinishAll;
        }
        public bool IsSimulating
        {
            get { return state != SimState.eNone; }
        }
        public bool HasJob()
        {
            return (fileIDLst.Count > 0 && lastIndex < fileIDLst.Count) || (state != SimState.eFinishAll);
        }

        void DoPrepareData()
        {
            RecordTradeDatas();

            if (fileIDLst.Count > lastIndex)
            {
                if (lastIndex == -1)
                    lastIndex = 0;
                DataManager dataMgr = DataManager.GetInst();
                dataMgr.ClearAllDatas();
                int startIndex = lastIndex;
                int endIndex = lastIndex + batch;
                if (endIndex >= fileIDLst.Count)
                {
                    endIndex = fileIDLst.Count - 1;
                    lastIndex = fileIDLst.Count;
                }
                else
                    lastIndex = endIndex - 1;

                for (int i = startIndex; i <= endIndex; ++i)
                {
                    int key = fileIDLst[i];
                    dataMgr.LoadData(key);
                }
                dataMgr.SetDataItemsGlobalID();
                if (dataMgr.GetAllDataItemCount() == 0)
                    return;
                Util.CollectPath012Info(null);
                GraphDataManager.Instance.CollectGraphData(GraphType.eKCurveGraph);

                TradeDataManager.Instance.startMoney = currentMoney;
                DataItem startTradeItem = null;
                if (string.IsNullOrEmpty(lastTradeIDTag))
                    startTradeItem = TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromFirst, lastTradeIDTag);
                else
                {
                    startTradeItem = TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromSpec, lastTradeIDTag);
                    TradeDataManager.Instance.CurrentTradeCountIndex = lastTradeCountIndex;
                }
                if (startTradeItem != null)
                {
                    if (onPrepareDataItems != null)
                        onPrepareDataItems(startTradeItem);
                }
                state = SimState.eSimTrade;
            }
            else
                state = SimState.eFinishAll;
        }
        public void RefreshMoney()
        {
            currentMoney = TradeDataManager.Instance.currentMoney;
            if (maxMoney < currentMoney)
                maxMoney = currentMoney;
            if (minMoney > currentMoney)
                minMoney = currentMoney;
        }
        void DoSimTrade()
        {
            RefreshMoney();
            if (TradeDataManager.Instance.hasCompleted == true)
            {
                lastTradeIDTag = TradeDataManager.Instance.CurTestTradeItem.idTag;
                lastTradeCountIndex = TradeDataManager.Instance.CurrentTradeCountIndex;
                state = SimState.eFinishBatch;
            }
        }
        void DoFinishBatch()
        {
            currentMoney = TradeDataManager.Instance.currentMoney;
            tradeRightCount += TradeDataManager.Instance.rightCount;
            tradeWrongCount += TradeDataManager.Instance.wrongCount;
            untradeCount += TradeDataManager.Instance.untradeCount;
            totalCount += TradeDataManager.Instance.rightCount + TradeDataManager.Instance.wrongCount + TradeDataManager.Instance.untradeCount;
            state = SimState.ePrepareData;
        }


        public static void LoadNextBatchDatas()
        {
            DataManager dataMgr = DataManager.GetInst();
            OneDayDatas odd = null;
            TradeDataBase trade = TradeDataManager.Instance.GetLatestTradeData();
            if (trade != null)
            {
                odd = trade.lastDateItem.parent;
            }
            else
            {
                DataItem latestItem = dataMgr.GetLatestItem();
                if (latestItem != null)
                {
                    odd = latestItem.parent;
                }
            }
            if (odd == null)
            {
                return;
            }
            List<int> dayIDLst = new List<int>(dataMgr.mFileMetaInfo.Count);
            foreach (int key in DataManager.GetInst().mFileMetaInfo.Keys)
            {
                dayIDLst.Add(key);
            }
            dayIDLst.Sort((x, y) =>
            {
                if (x < y)
                    return -1;
                return 1;
            });
            int startID = dayIDLst.IndexOf(odd.dateID);
            int endID = startID + Instance.batch;
            if (endID >= dayIDLst.Count)
                endID = dayIDLst.Count - 1;
            dataMgr.ClearDatasExcept(odd);
            for (int i = startID + 1; i <= endID; ++i)
            {
                int key = dayIDLst[i];
                dataMgr.LoadData(key);
            }
            dataMgr.SetDataItemsGlobalID();
            if (dataMgr.GetAllDataItemCount() == 0)
                return;
            Util.CollectPath012Info(null);
            GraphDataManager.Instance.CollectGraphDataExcept(GraphType.eKCurveGraph, odd);

            if (TradeDataManager.Instance.historyTradeDatas != null)
            {
                for (int i = TradeDataManager.Instance.historyTradeDatas.Count - 1; i >= 0; --i)
                {
                    if (TradeDataManager.Instance.historyTradeDatas[i].lastDateItem.parent == odd)
                        continue;
                    TradeDataManager.Instance.historyTradeDatas.RemoveAt(i);
                }
            }
        }
    }
}
