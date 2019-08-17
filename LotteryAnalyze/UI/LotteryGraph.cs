using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze.UI
{
    public partial class LotteryGraph : Form, UpdaterBase
    {
        static List<LotteryGraph> instLst = new List<LotteryGraph>();

        GraphManager graphMgr = null;
        int numberIndex = 0;        
        int curCDTIndex = 0;
        CollectDataType curCDT = CollectDataType.eNone;

        Point lastMouseMovePos = new Point();
        Point lastMouseMovePosDown = new System.Drawing.Point();

        Point upPanelMousePosLastDrag = new Point();
        Point upPanelMousePosOnMove = new Point();
        Point upPanelMousePosOnBtnDown = new Point();
        Point downPanelMousePosOnMove = new Point();
        Point downPanelMousePosLastDrag = new Point();
        Point downPanelMousePosOnBtnDown = new Point();
        bool hasNewDataUpdate = false;
        bool hasMouseMoveOnUpPanel = false;
        bool hasMouseMoveOnDownPanel = false;
        static Pen redPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 2);
        static Pen greenPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Green, 2);
        static Pen yellowPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Yellow, 2);
        Pen curUpdatePen;
        System.Windows.Forms.Timer updateTimer;
        int updateInterval = GlobalSetting.G_LOTTERY_GRAPH_UPDATE_INTERVAL;
        double updateCountDown = 0;
        DataItem itemSel = null;

        bool isAddingAuxLine = false;
        bool needRefresh = false;
        bool needCalcDataOnGridScaleChanged = false;

        #region ctor and common

        public static void Open(bool forceCreateNewOne)
        {
            if (forceCreateNewOne || instLst.Count == 0)
            {
                LotteryGraph graphInst = new LotteryGraph();
                graphInst.Show();
            }
        }

        static LotteryGraph()
        {
            BatchTradeSimulator.onPrepareDataItems += OnPrepareDataItems;
        }

        static void OnPrepareDataItems(DataItem startTradeItem)
        {
            NotifyAllGraphsStartSimTrade(startTradeItem.idGlobal);
        }

        public LotteryGraph()
        {
            graphMgr = new GraphManager(this);
            instLst.Add(this);
            InitializeComponent();
            
            //采用双缓冲技术的控件必需的设置
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            graphMgr.SetCurrentGraph(GraphType.eKCurveGraph);

            comboBoxCollectionDataType.DataSource = GraphDataManager.S_CDT_TAG_LIST;
            comboBoxCollectionDataType.SelectedIndex = curCDTIndex;
            comboBoxNumIndex.SelectedIndex = numberIndex;
            comboBoxKDataCircle.DataSource = GraphDataManager.G_Circles_STRs;
            comboBoxKDataCircle.SelectedIndex = 0;
            //textBoxCycleLength.Text = GraphDataManager.KGDC.CycleLength.ToString();
            curCDT = GraphDataManager.S_CDT_LIST[curCDTIndex];

            comboBoxBarCollectType.DataSource = GraphDataContainerBarGraph.S_StatisticsType_STRS;
            comboBoxCollectRange.DataSource = GraphDataContainerBarGraph.S_StatisticsRange_STRS;
            comboBoxBarCollectType.SelectedIndex = (int)GraphDataManager.BGDC.curStatisticsType;
            comboBoxCollectRange.SelectedIndex = (int)GraphDataManager.BGDC.curStatisticsRange;
            textBoxCustomCollectRange.Text = GraphDataManager.BGDC.customStatisticsRange.ToString();

            int selAvgCalcTypeID = (int)GraphDataContainerKGraph.S_AVG_ALGORITHM;
            comboBoxAvgAlgorithm.DataSource = GraphDataContainerKGraph.S_AVG_ALGORITHM_STRS;
            comboBoxAvgAlgorithm.SelectedIndex = selAvgCalcTypeID;

            checkBoxShowAvgLines.Checked = graphMgr.kvalueGraph.enableAvgLines;
            groupBoxAvgSettings.Enabled = graphMgr.kvalueGraph.enableAvgLines;
            checkBoxBollinBand.Checked = graphMgr.kvalueGraph.enableBollinBand;
            checkBoxMACD.Checked = graphMgr.kvalueGraph.enableMACD;
            checkBoxKRuler.Checked = graphMgr.kvalueGraph.enableKRuler;

            SetUIGridWH();
            RefreshUI();

            graphMgr.kvalueGraph.autoAllign = true;
            comboBoxOperations.DataSource = GraphKCurve.S_AUX_LINE_OPERATIONS;
            comboBoxOperations.SelectedIndex = (int)graphMgr.kvalueGraph.auxOperationIndex;
            checkBoxShowAuxLines.Checked = graphMgr.kvalueGraph.enableAuxiliaryLine;

            updateTimer = new Timer();
            updateTimer.Interval = updateInterval;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            updateCountDown = 0;
            textBoxRefreshTimeLength.Text = GlobalSetting.G_LOTTERY_GRAPH_UPDATE_INTERVAL.ToString();
            curUpdatePen = redPen;

            comboBoxTradeNumIndex.DataSource = KDataDictContainer.C_TAGS;
            comboBoxTradeNumIndex.SelectedIndex = TradeDataManager.Instance.simSelNumIndex;
            checkBoxTradeSpecNumIndex.Checked = TradeDataManager.Instance.simSelNumIndex != -1;
            textBoxDefaultCount.Text = TradeDataManager.Instance.defaultTradeCount.ToString();
            textBoxMultiCount.Text = TradeDataManager.Instance.GetTradeCountInfoStr();
            textBoxStartMoney.Text = TradeDataManager.Instance.startMoney.ToString();

            textBoxStartDataItem.Text = graphMgr.endShowDataItemIndex.ToString();

            TradeDataManager.Instance.tradeCompletedCallBack += OnTradeCompleted;
            buttonHorzExpand.Hide();
            buttonVertExpand.Hide();

            int selID = (int)TradeDataManager.Instance.curTradeStrategy;
            comboBoxTradeStrategy.DataSource = TradeDataManager.STRATEGY_NAMES;
            comboBoxTradeStrategy.SelectedIndex = selID;
            TradeDataManager.Instance.curTradeStrategy = (TradeDataManager.TradeStrategy)(selID);

            checkBoxShowSingleLine.Checked = graphMgr.appearenceGraph.onlyShowSelectCDTLine;
            checkBoxMissCountShowSingleLine.Checked = graphMgr.missCountGraph.onlyShowSelectCDTLine;

            int selMissCountType = (int)graphMgr.missCountGraph.missCountType;
            comboBoxMissCountType.DataSource = GraphMissCount.MissCountTypeStrs;
            comboBoxMissCountType.SelectedIndex = selMissCountType;

            int selAppearenceType = (int)graphMgr.appearenceGraph.AppearenceCycleType;
            comboBoxAppearenceType.DataSource = GraphAppearence.AppearenceTypeStrs;
            comboBoxAppearenceType.SelectedIndex = selAppearenceType;

            int Y = 10;
            int X = 10;
            int half_size = groupBoxCDTShowSetting.Size.Width / 2;
            int colorBoxSize = 18;
            int gap = 2;
            for ( int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++ i )
            {
                X = (i % 2) * half_size + 10;
                Y = (i / 2) * (colorBoxSize + gap) + 20;
                Button btn = new Button();
                btn.Size = new Size(colorBoxSize, colorBoxSize);
                btn.Location = new Point(X, Y);
                btn.Enabled = false;
                btn.BackColor = GraphDataManager.S_CDT_COLOR_LIST[i];
                groupBoxCDTShowSetting.Controls.Add(btn);

                X += 10 + colorBoxSize;
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
                CheckBox chk = new CheckBox();
                chk.Tag = cdt;
                chk.Text = GraphDataManager.S_CDT_TAG_LIST[i];
                chk.Location = new Point(X, Y);
                chk.Size = new Size(half_size - colorBoxSize - 10, colorBoxSize);
                chk.CheckedChanged += chk_CheckedChangedAppearence;
                groupBoxCDTShowSetting.Controls.Add(chk);

                chk.Checked = graphMgr.appearenceGraph.GetCDTLineShowState(cdt);
            }

            Y = 10;
            X = 10;
            half_size = groupBoxMissCountCDTShowSetting.Size.Width / 2;
            colorBoxSize = 18;
            gap = 2;
            for (int i = 0; i < GraphDataManager.S_CDT_LIST.Count; ++i)
            {
                X = (i % 2) * half_size + 10;
                Y = (i / 2) * (colorBoxSize + gap) + 20;
                Button btn = new Button();
                btn.Size = new Size(colorBoxSize, colorBoxSize);
                btn.Location = new Point(X, Y);
                btn.Enabled = false;
                btn.BackColor = GraphDataManager.S_CDT_COLOR_LIST[i];
                groupBoxMissCountCDTShowSetting.Controls.Add(btn);

                X += 10 + colorBoxSize;
                CollectDataType cdt = GraphDataManager.S_CDT_LIST[i];
                CheckBox chk = new CheckBox();
                chk.Tag = cdt;
                chk.Text = GraphDataManager.S_CDT_TAG_LIST[i];
                chk.Location = new Point(X, Y);
                chk.Size = new Size(half_size - colorBoxSize - 10, colorBoxSize);
                chk.CheckedChanged += chk_CheckedChangedMissCount;
                groupBoxMissCountCDTShowSetting.Controls.Add(chk);

                chk.Checked = graphMgr.missCountGraph.GetCDTLineShowState(cdt);
            }

            this.KeyPreview = true;
            FormMain.AddWindow(this);
            Program.AddUpdater(this);
        }

        void chk_CheckedChangedAppearence(object sender, EventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            CollectDataType cdt = (CollectDataType)(ckb.Tag);
            graphMgr.appearenceGraph.SetCDTLineShowState(cdt, ckb.Checked);

            if (graphMgr.appearenceGraph.onlyShowSelectCDTLine == false)
            {
                Invalidate(true);
            }
        }
        void chk_CheckedChangedMissCount(object sender, EventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            CollectDataType cdt = (CollectDataType)(ckb.Tag);
            graphMgr.missCountGraph.SetCDTLineShowState(cdt, ckb.Checked);

            if (graphMgr.missCountGraph.onlyShowSelectCDTLine == false)
            {
                Invalidate(true);
            }
        }

        void SetUIGridWH()
        {
            if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            {
                textBoxGridScaleW.Text = graphMgr.kvalueGraph.gridScaleUp.X.ToString();
                textBoxGridScaleH.Text = graphMgr.kvalueGraph.gridScaleUp.Y.ToString();
            }
            else if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
            {
                textBoxGridScaleW.Text = graphMgr.tradeGraph.gridScaleUp.X.ToString();
                textBoxGridScaleH.Text = graphMgr.tradeGraph.gridScaleUp.Y.ToString();
            }
            else if (graphMgr.CurrentGraphType == GraphType.eAppearenceGraph)
            {
                textBoxGridScaleW.Text = graphMgr.appearenceGraph.gridScaleUp.X.ToString();
                textBoxGridScaleH.Text = graphMgr.appearenceGraph.gridScaleUp.Y.ToString();
            }
            else if (graphMgr.CurrentGraphType == GraphType.eMissCountGraph)
            {
                textBoxGridScaleW.Text = graphMgr.missCountGraph.gridScaleUp.X.ToString();
                textBoxGridScaleH.Text = graphMgr.missCountGraph.gridScaleUp.Y.ToString();
            }
        }

        public virtual void OnUpdate()
        {
            if (GlobalSetting.G_UPDATE_IN_MAIN_THREAD)
            {
                if (updateCountDown <= 0)
                {
                    //UpdateTimer_Tick(null, null);
                    UpdateImpl();
                    updateCountDown = (double)GlobalSetting.G_LOTTERY_GRAPH_UPDATE_INTERVAL / 1000.0;
                }
                else
                {
                    updateCountDown -= Program.DeltaTime;
                }
            }
            if (needRefresh)
            {
                this.Invalidate(true);
            }
            needRefresh = false;
            if(needCalcDataOnGridScaleChanged)
            {
                if(graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
                {
                    graphMgr.kvalueGraph.OnGridScaleChanged();
                }
            }
            needCalcDataOnGridScaleChanged = false;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (GlobalSetting.G_UPDATE_IN_MAIN_THREAD == false)
            {
                UpdateImpl();
            }
        }

        private void UpdateImpl()
        {
            trackBarKData.Minimum = 0;
            trackBarKData.Maximum = GraphDataManager.KGDC.DataLength();
            trackBarGraphBar.Minimum = 0;
            trackBarGraphBar.Maximum = GraphDataManager.KGDC.DataLength(); ;
            trackBarTradeData.Minimum = 0;
            trackBarTradeData.Maximum = TradeDataManager.Instance.historyTradeDatas.Count;
            trackBarMissCount.Minimum = 0;
            trackBarMissCount.Maximum = DataManager.GetInst().GetAllDataItemCount();
            trackBarAppearRate.Minimum = 0;
            trackBarAppearRate.Maximum = DataManager.GetInst().GetAllDataItemCount();

            if (hasNewDataUpdate)
            {
                if (curUpdatePen == redPen || curUpdatePen == yellowPen)
                    curUpdatePen = greenPen;
                else if (curUpdatePen == greenPen)
                    curUpdatePen = redPen;
                else
                    curUpdatePen = redPen;
            }
            else if (TradeDataManager.Instance.IsPause())
            {
                if (curUpdatePen == redPen || curUpdatePen == yellowPen)
                    curUpdatePen = greenPen;
                else if (curUpdatePen == greenPen)
                    curUpdatePen = yellowPen;
                else
                    curUpdatePen = redPen;
            }
            else
            {
                curUpdatePen = redPen;
            }
            Invalidate(true);
        }

        void RefreshUI()
        {
            CheckBox[] cbs = new CheckBox[] { checkBoxAvg5, checkBoxAvg10, checkBoxAvg20, checkBoxAvg30, checkBoxAvg50, checkBoxAvg100, };
            Button[] btns = new Button[] { buttonAvg5, buttonAvg10, buttonAvg20, buttonAvg30, buttonAvg50, buttonAvg100, };
            for (int i = 0; i < GraphDataContainerKGraph.S_AVG_LINE_SETTINGS.Count; ++i)
            {
                cbs[i].Tag = GraphDataContainerKGraph.S_AVG_LINE_SETTINGS[i];
                cbs[i].Checked = GraphDataContainerKGraph.S_AVG_LINE_SETTINGS[i].enable;
                cbs[i].Text = GraphDataContainerKGraph.S_AVG_LINE_SETTINGS[i].tag;
                btns[i].BackColor = GraphDataContainerKGraph.S_AVG_LINE_SETTINGS[i].color;
            }
        }

        static void NotifyOtherGraphRefreshUI(LotteryGraph sender)
        {
            for( int i = 0; i < instLst.Count; ++i )
            {
                if(instLst[i] != sender)
                {
                    instLst[i].RefreshUI();
                }
            }
        }

        public static void NotifyAllGraphsRefresh(bool hasDataUpdate = false)
        {
            for (int i = 0; i < instLst.Count; ++i)
            {
                instLst[i].hasNewDataUpdate = hasDataUpdate;
                instLst[i].Invalidate(true);//触发Paint事件
            }
        }
        public static void NotifyAllGraphsStartSimTrade(int startIndex = 0)
        {
            for (int i = 0; i < instLst.Count; ++i)
            {
                instLst[i].graphMgr.endShowDataItemIndex = startIndex;
            }
        }

        #endregion

        #region Draw and Operations

        void DrawUpCanvas(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Black);

            CollectDataType cdt = GraphDataManager.S_CDT_LIST[curCDTIndex];
            graphMgr.DrawUpGraph(g, numberIndex, cdt, this.panelUp.ClientSize.Width, this.panelUp.ClientSize.Height, upPanelMousePosOnMove);

            Rectangle r = new Rectangle(1, 1, this.panelUp.ClientSize.Width - 2, this.panelUp.ClientSize.Height - 2);
            g.DrawRectangle(curUpdatePen, r);

            g.Flush();
        }

        void DrawDownCanvas(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Black);

            CollectDataType cdt = GraphDataManager.S_CDT_LIST[curCDTIndex];
            graphMgr.DrawDownGraph(g, numberIndex, cdt, this.panelDown.ClientSize.Width, this.panelDown.ClientSize.Height, downPanelMousePosOnMove);

            Rectangle r = new Rectangle(1, 1, this.panelDown.ClientSize.Width - 2, this.panelDown.ClientSize.Height - 2);
            g.DrawRectangle(curUpdatePen, r);

            g.Flush();
        }

        private void panelUp_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Location.X > panelUp.ClientSize.Width - buttonHorzExpand.ClientSize.Width && buttonHorzExpand.Visible == false)
                buttonHorzExpand.Visible = true;
            else if (buttonHorzExpand.Visible)
                buttonHorzExpand.Visible = false;

            if (e.Location.Y > panelUp.ClientSize.Height - buttonVertExpand.ClientSize.Height && buttonVertExpand.Visible == false)
                buttonVertExpand.Visible = true;
            else if (buttonVertExpand.Visible)
                buttonVertExpand.Visible = false;

            hasMouseMoveOnUpPanel = true;
            upPanelMousePosOnMove = e.Location;
            bool needUpdate = false;
            if (graphMgr.NeedRefreshCanvasOnMouseMove(e.Location))
                needUpdate = true;
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
            {
                float dx = e.Location.X - upPanelMousePosLastDrag.X;
                float dy = e.Location.Y - upPanelMousePosLastDrag.Y;

                if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
                {
                    if (graphMgr.kvalueGraph.selAuxLineUpPanel == null)
                    {
                        upPanelMousePosLastDrag = e.Location;
                        graphMgr.MoveGraph(dx, dy);
                        needUpdate = true;

                        int startIndex = 0, maxIndex = 0;
                        graphMgr.kvalueGraph.GetViewItemIndexInfo(ref startIndex, ref maxIndex);
                        if (startIndex > trackBarKData.Maximum)
                            trackBarKData.Value = trackBarKData.Maximum;
                        else if (startIndex < trackBarKData.Minimum)
                            trackBarKData.Value = trackBarKData.Minimum;
                        else
                            trackBarKData.Value = startIndex;

                        if (startIndex > trackBarKData.Maximum)
                            trackBarGraphBar.Value = trackBarGraphBar.Maximum;
                        else if (startIndex < trackBarKData.Minimum)
                            trackBarGraphBar.Value = trackBarGraphBar.Minimum;
                        else
                            trackBarGraphBar.Value = startIndex;
                    }
                    else
                    {
                        int idx = upPanelMousePosOnMove.X - lastMouseMovePos.X;
                        int idy = upPanelMousePosOnMove.Y - lastMouseMovePos.Y;
                        graphMgr.kvalueGraph.UpdateSelectAuxLinePoint( upPanelMousePosOnMove, idx, -idy, true );
                    }
                }
                else if(graphMgr.CurrentGraphType == GraphType.eTradeGraph)
                {
                    upPanelMousePosLastDrag = e.Location;
                    graphMgr.MoveGraph(dx, dy);
                    needUpdate = true;

                    int startIndex = 0, maxIndex = 0;
                    graphMgr.tradeGraph.GetViewItemIndexInfo(ref startIndex, ref maxIndex);
                    if (startIndex > trackBarTradeData.Maximum)
                        trackBarTradeData.Value = trackBarTradeData.Maximum;
                    else if (startIndex < trackBarTradeData.Minimum)
                        trackBarTradeData.Value = trackBarTradeData.Minimum;
                    else
                        trackBarTradeData.Value = startIndex;
                }
                else if(graphMgr.CurrentGraphType == GraphType.eAppearenceGraph)
                {
                    upPanelMousePosLastDrag = e.Location;
                    graphMgr.MoveGraph(dx, dy);
                    needUpdate = true;

                    int startIndex = 0, maxIndex = 0;
                    graphMgr.appearenceGraph.GetViewItemIndexInfo(ref startIndex, ref maxIndex);
                    if (startIndex > trackBarAppearRate.Maximum)
                        trackBarAppearRate.Value = trackBarAppearRate.Maximum;
                    else if (startIndex < trackBarAppearRate.Minimum)
                        trackBarAppearRate.Value = trackBarAppearRate.Minimum;
                    else
                        trackBarAppearRate.Value = startIndex;
                }
                else if(graphMgr.CurrentGraphType == GraphType.eMissCountGraph)
                {
                    upPanelMousePosLastDrag = e.Location;
                    graphMgr.MoveGraph(dx, dy);
                    needUpdate = true;

                    int startIndex = 0, maxIndex = 0;
                    graphMgr.missCountGraph.GetViewItemIndexInfo(ref startIndex, ref maxIndex);
                    if (startIndex > trackBarMissCount.Maximum)
                        trackBarMissCount.Value = trackBarMissCount.Maximum;
                    else if (startIndex < trackBarMissCount.Minimum)
                        trackBarMissCount.Value = trackBarMissCount.Minimum;
                    else
                        trackBarMissCount.Value = startIndex;
                }
            }
            if (needUpdate)
                RefreshPanel();//触发Paint事件

            lastMouseMovePos = e.Location;
        }

        private void panelUp_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelUp.Focus();
            hasMouseMoveOnUpPanel = false;
            if (hasNewDataUpdate)
                hasNewDataUpdate = false;
            upPanelMousePosLastDrag = e.Location;
            upPanelMousePosOnBtnDown = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph &&
                    graphMgr.kvalueGraph.enableAuxiliaryLine)
                {
                    //if(graphMgr.kvalueGraph.auxOperationIndex == AuxLineType.eNone)
                        graphMgr.kvalueGraph.SelectAuxLine(e.Location, numberIndex, curCDT, true);
                }
            }
            RefreshPanel();//触发Paint事件
        }

        private void CollectBarGraphData(DataItem item)
        {
            GraphDataManager.BGDC.CurrentSelectItem = item;
            GraphDataManager.Instance.CollectGraphData(GraphType.eBarGraph);
        }

        void SelItemForGraph(int selID, GraphBase graph, bool needScrollToData = true)
        {
            int checkW = (int)(this.panelUp.ClientSize.Width * 0.5f);
            int xOffset = 0;
            if (selID * graph.gridScaleUp.X > checkW)
                xOffset = -checkW;
            else
                xOffset = -(int)(selID * graph.gridScaleUp.X);
            graph.ScrollToData(selID, panelUp.ClientSize.Width, panelUp.ClientSize.Height, true, xOffset, needScrollToData);
        }
        void SelItemForKCurveGraph(int selID, bool needScrollToData = true)
        {
            SelItemForGraph(selID, graphMgr.kvalueGraph, needScrollToData);
        }
        void SelItemForAppearenceGraph(int selID, bool needScrollToData = true)
        {
            SelItemForGraph(selID, graphMgr.appearenceGraph, needScrollToData);
        }
        void SelItemForMissCountGraph(int selID, bool needScrollToData = true)
        {
            SelItemForGraph(selID, graphMgr.missCountGraph, needScrollToData);
        }
        void SelItemForTradeGraph(int selID, bool isTradeID, bool needScrollToData = true)
        {
            if (isTradeID)
            {
                if (TradeDataManager.Instance.GetTrade(selID) == null)
                    return;
            }
            else
            {
               int tradeID = TradeDataManager.Instance.GetTradeIndex(selID);
                if (tradeID == -1)
                    return;
                selID = tradeID;
            }
            SelItemForGraph(selID, graphMgr.tradeGraph, needScrollToData);
        }
        void SelItemByItemID(int kdIndex, bool needScrollToData = true)
        {
            SelItemForKCurveGraph(kdIndex, !(graphMgr.CurrentGraphType == GraphType.eKCurveGraph));
            SelItemForAppearenceGraph(kdIndex, !(graphMgr.CurrentGraphType == GraphType.eAppearenceGraph));
            SelItemForMissCountGraph(kdIndex, !(graphMgr.CurrentGraphType == GraphType.eMissCountGraph));
            SelItemForTradeGraph(kdIndex, false, !(graphMgr.CurrentGraphType == GraphType.eTradeGraph));
            FormMain.Instance.SelectDataItem(kdIndex);
            DataItem item = DataManager.GetInst().FindDataItem(kdIndex);
            CollectBarGraphData(item);
            itemSel = item;
            if(itemSel != null)
            {
                if (trackBarGraphBar.Maximum < DataManager.GetInst().GetAllDataItemCount() - 1)
                    trackBarGraphBar.Maximum = DataManager.GetInst().GetAllDataItemCount() - 1;
                trackBarGraphBar.Value = itemSel.idGlobal;
            }
        }
        void UnselectItem()
        {
            graphMgr.kvalueGraph.UnSelectData();
            graphMgr.appearenceGraph.UnselectDataItem();
            graphMgr.missCountGraph.UnselectDataItem();
            graphMgr.tradeGraph.UnselectTradeData();
            FormMain.Instance.SelectDataItem(null);
            CollectBarGraphData(null);
            itemSel = null;
        }
        void SelItem(int selID, bool needScrollToData = true)
        {
            if (selID != -1)
            {
                SelItemByItemID(selID, needScrollToData);
            }
            else
            {
                CollectBarGraphData(null);
                //UnselectItem();
            }
        }
        public static void G_SelItem(int selID)
        {
            for (int i = 0; i < instLst.Count; ++i)
            {
                instLst[i].SelItem(selID);
            }
        }
        void ProcAddAuxLine(Point mousePos, bool upPanel)
        {
            isAddingAuxLine = false;
            if (graphMgr.kvalueGraph.enableAuxiliaryLine)
            {
                graphMgr.kvalueGraph.SelectAuxLine(mousePos, numberIndex, curCDT, upPanel);
                bool canProcAdd = false;
                List<Point> mouseHitPoints = null;
                if (upPanel)
                {
                    canProcAdd = graphMgr.kvalueGraph.selAuxLineUpPanel == null && hasMouseMoveOnUpPanel == false;
                    mouseHitPoints = graphMgr.kvalueGraph.mouseHitPtsUpPanel;
                }
                else
                {
                    canProcAdd = graphMgr.kvalueGraph.selAuxLineDownPanel == null && hasMouseMoveOnDownPanel == false;
                    mouseHitPoints = graphMgr.kvalueGraph.mouseHitPtsDownPanel;
                }

                if (canProcAdd)
                {
                    switch (graphMgr.kvalueGraph.auxOperationIndex)
                    {
                        case AuxLineType.eNone:
                            {

                            }
                            break;
                        case AuxLineType.eHorzLine:
                            {
                                graphMgr.kvalueGraph.AddHorzLine(mousePos, numberIndex, curCDT, upPanel);
                                mouseHitPoints.Clear();
                                isAddingAuxLine = true;
                            }
                            break;
                        case AuxLineType.eVertLine:
                            {
                                graphMgr.kvalueGraph.AddVertLine(mousePos, numberIndex, curCDT, upPanel);
                                mouseHitPoints.Clear();
                                isAddingAuxLine = true;
                            }
                            break;
                        case AuxLineType.eSingleLine:
                            {
                                mouseHitPoints.Add(mousePos);
                                if (mouseHitPoints.Count == 2)
                                {
                                    graphMgr.kvalueGraph.AddSingleLine(
                                        mouseHitPoints[0],
                                        mouseHitPoints[1], numberIndex, curCDT, upPanel);
                                    mouseHitPoints.Clear();
                                }
                                isAddingAuxLine = true;
                            }
                            break;
                        case AuxLineType.eChannelLine:
                            {
                                mouseHitPoints.Add(mousePos);
                                if (mouseHitPoints.Count == 3)
                                {
                                    graphMgr.kvalueGraph.AddChannelLine(
                                        mouseHitPoints[0],
                                        mouseHitPoints[1],
                                        mouseHitPoints[2], numberIndex, curCDT, upPanel);
                                    mouseHitPoints.Clear();
                                }
                                isAddingAuxLine = true;
                            }
                            break;
                        case AuxLineType.eGoldSegmentedLine:
                            {
                                mouseHitPoints.Add(mousePos);
                                if (mouseHitPoints.Count == 2)
                                {
                                    graphMgr.kvalueGraph.AddGoldSegLine(
                                        mouseHitPoints[0],
                                        mouseHitPoints[1], numberIndex, curCDT, upPanel);
                                    mouseHitPoints.Clear();
                                }
                                isAddingAuxLine = true;
                            }
                            break;
                        case AuxLineType.eCircleLine:
                            {
                                mouseHitPoints.Add(mousePos);
                                if (mouseHitPoints.Count == 2)
                                {
                                    graphMgr.kvalueGraph.AddCircleLine(
                                        mouseHitPoints[0],
                                        mouseHitPoints[1], numberIndex, curCDT, upPanel);
                                    mouseHitPoints.Clear();
                                }
                                isAddingAuxLine = true;
                            }
                            break;
                        case AuxLineType.eArrowLine:
                            {
                                mouseHitPoints.Add(mousePos);
                                if (mouseHitPoints.Count == 2)
                                {
                                    graphMgr.kvalueGraph.AddArrowLine(
                                        mouseHitPoints[0],
                                        mouseHitPoints[1], numberIndex, curCDT, upPanel);
                                    mouseHitPoints.Clear();
                                }
                                isAddingAuxLine = true;
                            }
                            break;
                        case AuxLineType.eRectLine:
                            {
                                mouseHitPoints.Add(mousePos);
                                if (mouseHitPoints.Count == 2)
                                {
                                    graphMgr.kvalueGraph.AddRectLine(
                                        mouseHitPoints[0],
                                        mouseHitPoints[1], numberIndex, curCDT, upPanel);
                                    mouseHitPoints.Clear();
                                }
                                isAddingAuxLine = true;
                            }
                            break;
                    }
                }
            }
        }

        private void panelUp_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 当前是出号率视图
                if(graphMgr.CurrentGraphType == GraphType.eAppearenceGraph)
                {
                    int selID = graphMgr.appearenceGraph.SelectDataItem(e.Location);
                    SelItem(selID);
                }
                // 当前是遗漏视图
                else if(graphMgr.CurrentGraphType == GraphType.eMissCountGraph)
                {
                    int selID = graphMgr.missCountGraph.SelectDataItem(e.Location);
                    SelItem(selID);
                }
                // 当前是交易视图
                else if(graphMgr.CurrentGraphType == GraphType.eTradeGraph)
                {
                    TradeDataBase selectTD = null;
                    int kdataID = -1;
                    // 鼠标没有移动
                    if (e.X == upPanelMousePosOnBtnDown.X && e.Y == upPanelMousePosOnBtnDown.Y)
                    {
                        // 计算点击选中的交易项
                        int selID = graphMgr.tradeGraph.SelectTradeData(e.Location);
                        if (selID != -1)
                        {
                            selectTD = TradeDataManager.Instance.historyTradeDatas[selID];
                            kdataID = selectTD.targetLotteryItem.idGlobal;
                        }
                    }
                    if (kdataID != -1)
                    {
                        // 对所有图形视图中的选中项进行选中
                        SelItemByItemID(kdataID);

                        // 在K线图中，切换到交易选中的那个数字位以及012路对应的视图
                        int numID = -1, pathID = -1;
                        selectTD.GetTradeNumIndexAndPathIndex(ref numID, ref pathID);
                        if (numID != -1 && pathID != -1)
                        {
                            graphMgr.kvalueGraph.autoAllign = true;
                            numberIndex = comboBoxNumIndex.SelectedIndex = numID;
                            curCDTIndex = comboBoxCollectionDataType.SelectedIndex = pathID + GraphDataManager.S_CDT_LIST.IndexOf(CollectDataType.ePath0);
                            curCDT = GraphDataManager.S_CDT_LIST[curCDTIndex];
                        }
                    }
                    //else
                    //{
                    //    UnselectItem();
                    //}
                }
                // 当前是K线图
                else if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
                {
                    // 处理辅助线的添加
                    ProcAddAuxLine(e.Location, true);
                    if (graphMgr.kvalueGraph.selAuxLineUpPanel == null && 
                        hasMouseMoveOnUpPanel == false &&
                        isAddingAuxLine == false &&
                        graphMgr.kvalueGraph.enableAuxiliaryLine && graphMgr.kvalueGraph.auxOperationIndex == AuxLineType.eNone
                        )
                    {
                        int kdataID = -1;
                        // 鼠标没有移动
                        if (e.X == upPanelMousePosOnBtnDown.X && e.Y == upPanelMousePosOnBtnDown.Y)
                        {
                            // 计算点击选中的K值
                            int selID = graphMgr.kvalueGraph.SelectKData(e.Location, true);
                            SelItem(selID, false);
                        }
                    }
                }
            }
            else if(e.Button == MouseButtons.Right)
            {
                graphMgr.kvalueGraph.mouseHitPtsUpPanel.Clear();
            }
            RefreshPanel();//触发Paint事件
        }
        private void panelDown_MouseMove(object sender, MouseEventArgs e)
        {
            hasMouseMoveOnDownPanel = true;
            downPanelMousePosOnMove = e.Location;

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
            {
                float dx = e.Location.X - downPanelMousePosLastDrag.X;
                float dy = e.Location.Y - downPanelMousePosLastDrag.Y;
                downPanelMousePosLastDrag = e.Location;

                if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
                {
                    if (graphMgr.kvalueGraph.selAuxLineDownPanel == null)
                    {
                        graphMgr.kvalueGraph.DownGraphYOffset += dy;
                        graphMgr.MoveGraph(dx, 0);
                    }
                    else
                    {
                        int idx = downPanelMousePosOnMove.X - lastMouseMovePosDown.X;
                        int idy = downPanelMousePosOnMove.Y - lastMouseMovePosDown.Y;
                        graphMgr.kvalueGraph.UpdateSelectAuxLinePoint(downPanelMousePosOnMove, idx, -idy, false);
                    }
                }
            }
            RefreshPanel();//触发Paint事件

            lastMouseMovePosDown = e.Location;
        }

        private void panelDown_MouseDown(object sender, MouseEventArgs e)
        {
            this.panelDown.Focus();
            hasMouseMoveOnDownPanel = false;
            downPanelMousePosLastDrag = e.Location;
            downPanelMousePosOnBtnDown = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph &&
                    graphMgr.kvalueGraph.enableAuxiliaryLine)
                {
                    graphMgr.kvalueGraph.SelectAuxLine(e.Location, numberIndex, curCDT, false);
                }
            }
            RefreshPanel();//触发Paint事件
        }
        private void panelDown_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if(graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
                {
                    // 处理辅助线的添加
                    ProcAddAuxLine(e.Location, false);
                    if (graphMgr.kvalueGraph.selAuxLineDownPanel == null &&
                        hasMouseMoveOnDownPanel == false &&
                        isAddingAuxLine == false &&
                        graphMgr.kvalueGraph.enableAuxiliaryLine && graphMgr.kvalueGraph.auxOperationIndex == AuxLineType.eNone
                        )
                    {
                        int kdataID = -1;
                        // 鼠标没有移动
                        if (e.X == downPanelMousePosOnBtnDown.X && e.Y == downPanelMousePosOnBtnDown.Y)
                        {
                            // 计算点击选中的K值
                            int selID = graphMgr.kvalueGraph.SelectKData(e.Location, false);
                            SelItem(selID, false);
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                graphMgr.kvalueGraph.mouseHitPtsDownPanel.Clear();
            }
            RefreshPanel();//触发Paint事件
        }

        private void panelUp_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawUpCanvas(g);
        }

        private void panelDown_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawDownCanvas(g);
        }

        private void delSelAuxLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            {
                if (graphMgr.kvalueGraph.selAuxLineUpPanel != null)
                {
                    graphMgr.kvalueGraph.RemoveSelectAuxLine(true);
                }
                if(graphMgr.kvalueGraph.selAuxLineDownPanel != null)
                {
                    graphMgr.kvalueGraph.RemoveSelectAuxLine(false);
                }
            }
        }

        private void trackBarKData_Scroll(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.ScrollToData(trackBarKData.Value, panelUp.ClientSize.Width, panelUp.ClientSize.Height, false);
            graphMgr.kvalueGraph.ScrollToData(trackBarKData.Value, panelDown.ClientSize.Width, panelDown.ClientSize.Height, false);
            trackBarGraphBar.Value = trackBarKData.Value;
            RefreshPanel();//触发Paint事件
        }

        private void trackBarTradeData_Scroll(object sender, EventArgs e)
        {
            TradeDataManager tdm = TradeDataManager.Instance;
            graphMgr.tradeGraph.ScrollToData(trackBarTradeData.Value, panelUp.ClientSize.Width, panelUp.ClientSize.Height, false);
            RefreshPanel();//触发Paint事件
        }

        #endregion

        #region control callbacks
        private void LotteryGraph_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (updateTimer != null)
            //{
            //    updateTimer.Stop();
            //    updateTimer.Dispose();
            //    updateTimer = null;
            //}
            Program.RemoveUpdater(this);
            TradeDataManager.Instance.tradeCompletedCallBack -= OnTradeCompleted;
            FormMain.RemoveWindow(this);
            instLst.Remove(this);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            RefreshPanel();//触发Paint事件
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014) // 禁掉清除背景消息
                return;
            base.WndProc(ref m);
        }


        public static void G_SetSelNumIndex(int _numIndex)
        {
            for (int i = 0; i < instLst.Count; ++i)
            {
                instLst[i].SetSelNumIndex(_numIndex);
            }
        }
        public static void G_SetSelCollectDataType(int _cdtIndex)
        {
            for (int i = 0; i < instLst.Count; ++i)
            {
                instLst[i].SetSelCollectDataType(_cdtIndex);
            }
        }
        void SetSelNumIndex(int _numIndex)
        {
            comboBoxNumIndex.SelectedIndex = _numIndex;
            comboBoxNumIndex_SelectedIndexChanged(null, null);
        }
        void SetSelCollectDataType(int _cdtIndex)
        {
            comboBoxCollectionDataType.SelectedIndex = _cdtIndex;
            comboBoxCollectionDataType_SelectedIndexChanged(null, null);
        }

        private void comboBoxNumIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.autoAllign = true;
            numberIndex = comboBoxNumIndex.SelectedIndex;
            if(graphMgr.CurrentGraphType == GraphType.eBarGraph)
            {
                GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            }
            RefreshPanel();//触发Paint事件
        }

        private void comboBoxCollectionDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.autoAllign = true;
            curCDTIndex = comboBoxCollectionDataType.SelectedIndex;
            curCDT = GraphDataManager.S_CDT_LIST[curCDTIndex];
            if (graphMgr.CurrentGraphType == GraphType.eBarGraph)
            {
                GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            }
            RefreshPanel();//触发Paint事件
        }

        private void textBoxCycleLength_TextChanged(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(textBoxCycleLength.Text) == false)
            //{
            //    int value = 5;
            //    if (int.TryParse(textBoxCycleLength.Text, out value))
            //    {
            //        GraphDataManager.KGDC.CycleLength = value;
            //        GraphDataManager.KGDC.CollectGraphData();
            //        RefreshPanel();//触发Paint事件
            //    }
            //}
        }

        private void tabControlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.SetCurrentGraph((GraphType)(tabControlView.SelectedIndex+1));
            if (graphMgr.CurrentGraphType == GraphType.eBarGraph)
                GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            SetUIGridWH();
            RefreshPanel();
        }

        private void comboBoxBarCollectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphDataManager.BGDC.curStatisticsType = (GraphDataContainerBarGraph.StatisticsType)comboBoxBarCollectType.SelectedIndex;
            if (graphMgr.CurrentGraphType == GraphType.eBarGraph)
                GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            RefreshPanel();
        }

        private void comboBoxCollectRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphDataManager.BGDC.curStatisticsRange = (GraphDataContainerBarGraph.StatisticsRange)comboBoxCollectRange.SelectedIndex;
            if (graphMgr.CurrentGraphType == GraphType.eBarGraph)
                GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            RefreshPanel();
        }

        private void textBoxCustomCollectRange_TextChanged(object sender, EventArgs e)
        {
            GraphDataManager.BGDC.customStatisticsRange = int.Parse(textBoxCustomCollectRange.Text);
            if (graphMgr.CurrentGraphType == GraphType.eBarGraph)
                GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            RefreshPanel();
        }

        private void comboBoxAvgAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphDataContainerKGraph.S_AVG_ALGORITHM = (AvgAlgorithm)comboBoxAvgAlgorithm.SelectedIndex;
            NotifyOtherGraphRefreshUI(this);
            GraphDataManager.KGDC.CollectAvgDatas();
            RefreshPanel();
        }

        private void checkBoxAvg5_CheckedChanged(object sender, EventArgs e)
        {
            GraphDataContainerKGraph.AvgLineSetting als = checkBoxAvg5.Tag as GraphDataContainerKGraph.AvgLineSetting;
            als.enable = checkBoxAvg5.Checked;
            NotifyOtherGraphRefreshUI(this);
            RefreshPanel();
        }

        private void checkBoxAvg10_CheckedChanged(object sender, EventArgs e)
        {
            GraphDataContainerKGraph.AvgLineSetting als = checkBoxAvg10.Tag as GraphDataContainerKGraph.AvgLineSetting;
            als.enable = checkBoxAvg10.Checked;
            NotifyOtherGraphRefreshUI(this);
            RefreshPanel();
        }

        private void checkBoxAvg20_CheckedChanged(object sender, EventArgs e)
        {
            GraphDataContainerKGraph.AvgLineSetting als = checkBoxAvg20.Tag as GraphDataContainerKGraph.AvgLineSetting;
            als.enable = checkBoxAvg20.Checked;
            NotifyOtherGraphRefreshUI(this);
            RefreshPanel();
        }

        private void checkBoxAvg30_CheckedChanged(object sender, EventArgs e)
        {
            GraphDataContainerKGraph.AvgLineSetting als = checkBoxAvg30.Tag as GraphDataContainerKGraph.AvgLineSetting;
            als.enable = checkBoxAvg30.Checked;
            NotifyOtherGraphRefreshUI(this);
            RefreshPanel();
        }

        private void checkBoxAvg50_CheckedChanged(object sender, EventArgs e)
        {
            GraphDataContainerKGraph.AvgLineSetting als = checkBoxAvg50.Tag as GraphDataContainerKGraph.AvgLineSetting;
            als.enable = checkBoxAvg50.Checked;
            NotifyOtherGraphRefreshUI(this);
            RefreshPanel();
        }

        private void checkBoxAvg100_CheckedChanged(object sender, EventArgs e)
        {
            GraphDataContainerKGraph.AvgLineSetting als = checkBoxAvg100.Tag as GraphDataContainerKGraph.AvgLineSetting;
            als.enable = checkBoxAvg100.Checked;
            NotifyOtherGraphRefreshUI(this);
            RefreshPanel();
        }

        private void checkBoxBollinBand_CheckedChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.enableBollinBand = checkBoxBollinBand.Checked;
            RefreshPanel();
        }

        private void checkBoxMACD_CheckedChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.enableMACD = checkBoxMACD.Checked;
            RefreshPanel();
        }

        private void textBoxGridScaleH_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            int.TryParse(textBoxGridScaleH.Text, out v);
            if (v < 1)
            {
                textBoxGridScaleH.Text = "1";
                v = 1;
            }
            if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            {
                graphMgr.kvalueGraph.gridScaleUp.Y = v;
                graphMgr.kvalueGraph.OnGridScaleChanged();
            }
            else if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
            {
                graphMgr.tradeGraph.gridScaleUp.Y = v;
            }
            else if (graphMgr.CurrentGraphType == GraphType.eAppearenceGraph)
            {
                graphMgr.appearenceGraph.gridScaleUp.Y = v;
            }
            else if (graphMgr.CurrentGraphType == GraphType.eMissCountGraph)
            {
                graphMgr.missCountGraph.gridScaleUp.Y = v;
            }
            RefreshPanel();
        }

        private void textBoxGridScaleW_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            int.TryParse(textBoxGridScaleW.Text, out v);
            if (v < 1)
            {
                textBoxGridScaleW.Text = "1";
                v = 1;
            }
            if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            {
                graphMgr.kvalueGraph.gridScaleUp.X = v;
                graphMgr.kvalueGraph.gridScaleDown.X = v;
                graphMgr.kvalueGraph.OnGridScaleChanged();
            }
            else if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
            {
                graphMgr.tradeGraph.gridScaleUp.X = v;
                graphMgr.tradeGraph.gridScaleDown.X = v;
            }
            else if (graphMgr.CurrentGraphType == GraphType.eAppearenceGraph)
            {
                graphMgr.appearenceGraph.gridScaleUp.X = v;
                graphMgr.appearenceGraph.gridScaleDown.X = v;
            }
            else if (graphMgr.CurrentGraphType == GraphType.eMissCountGraph)
            {
                graphMgr.missCountGraph.gridScaleUp.X = v;
                graphMgr.missCountGraph.gridScaleDown.X = v;
            }
            RefreshPanel();
        }

        private void autoAllignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
                graphMgr.kvalueGraph.autoAllign = true;
            else if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
                graphMgr.tradeGraph.autoAllign = true;
            RefreshPanel();
        }

        private void checkBoxShowAvgLines_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowAvgLines.Checked)
                groupBoxAvgSettings.Enabled = true;
            else
                groupBoxAvgSettings.Enabled = false;
            graphMgr.kvalueGraph.enableAvgLines = checkBoxShowAvgLines.Checked;
            RefreshPanel();
        }

        private void comboBoxOperations_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.auxOperationIndex = (AuxLineType)comboBoxOperations.SelectedIndex;
        }
        private void delAllAuxLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.RemoveAllAuxLines();
            RefreshPanel();
        }
        private void checkBoxShowAuxLines_CheckedChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.enableAuxiliaryLine = checkBoxShowAuxLines.Checked;
            RefreshPanel();
        }
        private void cancelAddAuxLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.mouseHitPtsUpPanel.Clear();
        }
        private void modifyAuxLineColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            {
                if (graphMgr.kvalueGraph.selAuxLineUpPanel != null)
                {
                    ColorDialog dlg = new ColorDialog();
                    dlg.Color = graphMgr.kvalueGraph.selAuxLineUpPanel.GetSolidPen().Color;

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        Color GetColor = dlg.Color;
                        graphMgr.kvalueGraph.selAuxLineUpPanel.SetColor(GetColor);
                    }
                }
            }
        }

        private void autoAddAuxLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.needAutoAddAuxLine = true;
        }

        #endregion

        #region trade call back

        private void tradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TradeWindow win = new TradeWindow(graphMgr);
            win.Show();
        }

        private void tradeSimFromFirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphMgr.endShowDataItemIndex = 0;
            TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromFirst, null);
            graphMgr.kvalueGraph.autoAllign = true;
            graphMgr.tradeGraph.autoAllign = true;
        }

        private void tradeSimFromLatestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromLatest, null);
            graphMgr.kvalueGraph.autoAllign = true;
            graphMgr.tradeGraph.autoAllign = true;
        }

        private void pauseSimTradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TradeDataManager.Instance.PauseAutoTradeJob();
        }

        private void resumeSimTradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TradeDataManager.Instance.ResumeAutoTradeJob();
        }

        private void stopSimTradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphMgr.endShowDataItemIndex = -1;
            TradeDataManager.Instance.StopAutoTradeJob();
        }

        private void buttonCommitTradeCount_Click(object sender, EventArgs e)
        {
            TradeDataManager.Instance.defaultTradeCount = int.Parse(textBoxDefaultCount.Text);
            TradeDataManager.Instance.SetTradeCountInfo(textBoxMultiCount.Text);
        }

        private void checkBoxTradeSpecNumIndex_Click(object sender, EventArgs e)
        {
            if (checkBoxTradeSpecNumIndex.Checked)
            {
                if (comboBoxTradeNumIndex.SelectedIndex == -1)
                {
                    comboBoxTradeNumIndex.SelectedIndex = 0;
                }
                TradeDataManager.Instance.simSelNumIndex = comboBoxTradeNumIndex.SelectedIndex;
            }
            else
                TradeDataManager.Instance.simSelNumIndex = -1;
        }

        private void checkBoxKRuler_CheckedChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.enableKRuler = checkBoxKRuler.Checked;
        }

        private void comboBoxTradeNumIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (checkBoxTradeSpecNumIndex.Checked)
            //    TradeDataManager.Instance.simSelNumIndex = comboBoxTradeNumIndex.SelectedIndex;
            //else
            //    TradeDataManager.Instance.simSelNumIndex = -1;
            TradeDataManager.Instance.simSelNumIndex = comboBoxTradeNumIndex.SelectedIndex;
            checkBoxTradeSpecNumIndex.Checked = TradeDataManager.Instance.simSelNumIndex != -1;
        }

        private void clearAllTradeDatasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchTradeSimulator.Instance.ResetTradeInfo();
            TradeDataManager.Instance.ClearAllTradeDatas();
        }

        private void textBoxStartMoney_TextChanged(object sender, EventArgs e)
        {
            float v = float.Parse(textBoxStartMoney.Text);
            TradeDataManager.Instance.SetStartMoney(v);
        }
        private void globalSimTradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalSimTradeWindow.Open();
        }

        private void tradeCalculaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LotteryAnalyze.UI.TradeCalculater.Open();
        }


        #endregion

        private void listBoxFavoriteCharts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listBoxFavoriteCharts.SelectedIndex >= 0)
                {
                    string tag = listBoxFavoriteCharts.SelectedItem as string;
                    graphMgr.RemoveFavoriteChart(tag);
                    listBoxFavoriteCharts.Items.RemoveAt(listBoxFavoriteCharts.SelectedIndex);
                }
            }
        }

        private void listBoxFavoriteCharts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxFavoriteCharts.SelectedIndex >= 0)
            {
                GraphManager.FavoriteChart fc = graphMgr.GetFavoriteChart(listBoxFavoriteCharts.SelectedIndex);
                if(fc != null)
                {
                    graphMgr.kvalueGraph.autoAllign = true;
                    comboBoxCollectionDataType.SelectedIndex = GraphDataManager.S_CDT_LIST.IndexOf(fc.cdt);
                    comboBoxNumIndex.SelectedIndex = fc.numIndex;
                    curCDTIndex = comboBoxCollectionDataType.SelectedIndex;
                    curCDT = GraphDataManager.S_CDT_LIST[curCDTIndex];
                    numberIndex = fc.numIndex;
                    RefreshPanel();
                }
            }
        }

        private void buttonAddFavoriteChart_Click(object sender, EventArgs e)
        {
            GraphManager.FavoriteChart fc = graphMgr.AddFavoriteChart(numberIndex, curCDT);
            if (fc != null)
            {
                listBoxFavoriteCharts.Items.Add(fc.tag);
            }
        }

        private void buttonClearFavoriteCharts_Click(object sender, EventArgs e)
        {
            listBoxFavoriteCharts.Items.Clear();
            graphMgr.ClearFavoriteCharts();
        }

        private void btnSetAsStartTrade_Click(object sender, EventArgs e)
        {
            graphMgr.endShowDataItemIndex = int.Parse(textBoxStartDataItem.Text);
        }

        private void OnTradeCompleted()
        {
            //this.BringToFront();
            graphMgr.OnTradeCompleted();
            int index = TradeDataManager.Instance.historyTradeDatas.Count;
            int checkW = (int)(this.panelUp.ClientSize.Width * 0.5f);
            if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
            {
                graphMgr.tradeGraph.autoAllign = true;
                int xOffSet = 0;
                if (index * graphMgr.tradeGraph.gridScaleUp.X > checkW)
                    xOffSet = -checkW;
                else
                    xOffSet = -(int)(index * graphMgr.tradeGraph.gridScaleUp.X);
                graphMgr.tradeGraph.ScrollToData(
                    index,
                    this.panelUp.ClientSize.Width,
                    this.panelUp.ClientSize.Height,
                    true, xOffSet);
            }
            else if(graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            {
                graphMgr.kvalueGraph.autoAllign = true;
                TradeDataBase latestTradedItem = TradeDataManager.Instance.historyTradeDatas[index - 1];
                index = latestTradedItem.targetLotteryItem.idGlobal;
                int xOffSet = 0;
                if (index * graphMgr.kvalueGraph.gridScaleUp.X > checkW)
                    xOffSet = -checkW;
                else
                    xOffSet = -(int)(index * graphMgr.kvalueGraph.gridScaleUp.X);
                graphMgr.kvalueGraph.ScrollToData(
                    index,
                    this.panelUp.ClientSize.Width,
                    this.panelUp.ClientSize.Height,
                    true, xOffSet);
            }
            else if(graphMgr.CurrentGraphType == GraphType.eAppearenceGraph)
            {
                graphMgr.appearenceGraph.autoAllign = true;
                TradeDataBase latestTradedItem = TradeDataManager.Instance.historyTradeDatas[index - 1];
                index = latestTradedItem.targetLotteryItem.idGlobal;
                int xOffSet = 0;
                if (index * graphMgr.appearenceGraph.gridScaleUp.X > checkW)
                    xOffSet = -checkW;
                else
                    xOffSet = -(int)(index * graphMgr.appearenceGraph.gridScaleUp.X);
                graphMgr.appearenceGraph.ScrollToData(
                    index,
                    this.panelUp.ClientSize.Width,
                    this.panelUp.ClientSize.Height,
                    true, xOffSet);
            }
            else if(graphMgr.CurrentGraphType == GraphType.eMissCountGraph)
            {
                graphMgr.missCountGraph.autoAllign = true;
                TradeDataBase latestTradedItem = TradeDataManager.Instance.historyTradeDatas[index - 1];
                index = latestTradedItem.targetLotteryItem.idGlobal;
                int xOffSet = 0;
                if (index * graphMgr.missCountGraph.gridScaleUp.X > checkW)
                    xOffSet = -checkW;
                else
                    xOffSet = -(int)(index * graphMgr.missCountGraph.gridScaleUp.X);
                graphMgr.missCountGraph.ScrollToData(
                    index,
                    this.panelUp.ClientSize.Width,
                    this.panelUp.ClientSize.Height,
                    true, xOffSet);
            }
            trackBarTradeData.Value = trackBarTradeData.Maximum;            
            textBoxStartDataItem.Text = graphMgr.endShowDataItemIndex.ToString();
            DataItem curItem = DataManager.GetInst().FindDataItem(graphMgr.endShowDataItemIndex);
            CollectBarGraphData(curItem);

            //RefreshPanel();
        }

        public static void OnSelectDataItemOuter(int dataItemIndex, int tradeIndex)
        {
            for( int i = 0; i < instLst.Count; ++i )
            {
                instLst[i].SelectDataItem( dataItemIndex, tradeIndex);
            }
        }

        public void SelectDataItem(int dataItemIndex, int tradeIndex)
        {
            int checkW = (int)(this.panelUp.ClientSize.Width * 0.5f);
            if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
            {
                graphMgr.tradeGraph.autoAllign = true;
                int xOffSet = 0;
                if (tradeIndex * graphMgr.tradeGraph.gridScaleUp.X > checkW)
                    xOffSet = -checkW;
                else
                    xOffSet = -(int)(tradeIndex * graphMgr.tradeGraph.gridScaleUp.X);
                graphMgr.tradeGraph.ScrollToData(
                    tradeIndex,
                    this.panelUp.ClientSize.Width,
                    this.panelUp.ClientSize.Height,
                    true, xOffSet);
            }
            else if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            {
                graphMgr.kvalueGraph.autoAllign = true;
                int xOffSet = 0;
                if (dataItemIndex * graphMgr.kvalueGraph.gridScaleUp.X > checkW)
                    xOffSet = -checkW;
                else
                    xOffSet = -(int)(dataItemIndex * graphMgr.kvalueGraph.gridScaleUp.X);
                graphMgr.kvalueGraph.ScrollToData(
                    dataItemIndex,
                    this.panelUp.ClientSize.Width,
                    this.panelUp.ClientSize.Height,
                    true, xOffSet);
            }
            trackBarTradeData.Value = trackBarTradeData.Maximum;
            RefreshPanel();
            textBoxStartDataItem.Text = graphMgr.endShowDataItemIndex.ToString();

            DataItem curItem = DataManager.GetInst().FindDataItem(graphMgr.endShowDataItemIndex);
            CollectBarGraphData(curItem);
        }

        private void buttonHorzExpand_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            RefreshPanel();
        }

        private void buttonVertExpand_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;
            RefreshPanel();
        }

        private void simTradeOneStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(TradeDataManager.Instance.IsCompleted() == false)
            {
                TradeDataManager.Instance.PauseAutoTradeJob();
                TradeDataManager.Instance.SimTradeOneStep();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemplus)
            {
                GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT++;
                RefreshPanel();
            }
            else if (e.KeyCode == Keys.OemMinus)
            {
                if (GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT > 1)
                {
                    GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT--;
                    RefreshPanel();
                }
            }
        }

        private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.Control)
            {
                if (e.KeyCode == Keys.F)
                    TradeDataManager.Instance.StartAutoTradeJob(TradeDataManager.StartTradeType.eFromFirst, null);
                else if (e.KeyCode == Keys.P)
                    TradeDataManager.Instance.PauseAutoTradeJob();
                else if (e.KeyCode == Keys.R)
                    TradeDataManager.Instance.ResumeAutoTradeJob();
                else if (e.KeyCode == Keys.C)
                    TradeDataManager.Instance.StopAutoTradeJob();
                else if (e.KeyCode == Keys.O)
                    TradeDataManager.Instance.SimTradeOneStep();
                //else if (e.KeyCode == Keys.Oemplus)
                //{
                //    GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT++;
                //    RefreshPanel();
                //}
                //else if (e.KeyCode == Keys.OemMinus)
                //{
                //    if (GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT > 1)
                //    {
                //        GlobalSetting.G_ANALYZE_TOOL_SAMPLE_COUNT--;
                //        RefreshPanel();
                //    }
                //}
            }
        }

        private void comboBoxTradeStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            TradeDataManager.Instance.curTradeStrategy = (TradeDataManager.TradeStrategy)comboBoxTradeStrategy.SelectedIndex;
        }

        private void checkBoxShowSingleLine_CheckedChanged(object sender, EventArgs e)
        {
            graphMgr.appearenceGraph.onlyShowSelectCDTLine = checkBoxShowSingleLine.Checked;
            RefreshPanel();
        }

        private void checkBoxMissCountShowSingleLine_CheckedChanged(object sender, EventArgs e)
        {
            graphMgr.missCountGraph.onlyShowSelectCDTLine = checkBoxMissCountShowSingleLine.Checked;
            RefreshPanel();
        }
        
        private void comboBoxMissCountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.missCountGraph.missCountType = (GraphMissCount.MissCountType)comboBoxMissCountType.SelectedIndex;
            RefreshPanel();
        }

        private void textBoxRefreshTimeLength_TextChanged(object sender, EventArgs e)
        {
            int v = 0;
            if (int.TryParse(textBoxRefreshTimeLength.Text, out v) == false)
                GlobalSetting.G_LOTTERY_GRAPH_UPDATE_INTERVAL = 50;
            else
                GlobalSetting.G_LOTTERY_GRAPH_UPDATE_INTERVAL = v;
            if (GlobalSetting.G_LOTTERY_GRAPH_UPDATE_INTERVAL < 10)
                GlobalSetting.G_LOTTERY_GRAPH_UPDATE_INTERVAL = 10;
            textBoxRefreshTimeLength.Text = GlobalSetting.G_LOTTERY_GRAPH_UPDATE_INTERVAL.ToString();
            RefreshPanel();
        }

        private void comboBoxAppearenceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.appearenceGraph.AppearenceCycleType = (GraphAppearence.AppearenceType)comboBoxAppearenceType.SelectedIndex;
            RefreshPanel();
        }

        private void loadNextBatchDatasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int itemIndex = graphMgr.kvalueGraph.StartItemIndex;
            DataItem item = DataManager.GetInst().FindDataItem(itemIndex);
            BatchTradeSimulator.LoadNextBatchDatas();
            if(item != null)
            {
                graphMgr.kvalueGraph.ScrollToData(item.idGlobal, panelUp.ClientSize.Width, panelUp.ClientSize.Height, false);
            }
            graphMgr.endShowDataItemIndex = TradeDataManager.Instance.historyTradeDatas.Count - 1;
            RefreshPanel();
        }

        private void refreshLatestDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMain.RefreshLatestData();
        }

        private void setBreakPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (itemSel != null)
                TradeDataManager.Instance.debugInfo.dataItemTagBP = itemSel.idTag;
            TradeDebugWindow.Open();
            TradeDebugWindow.RefreshUI();
        }

        private void comboBoxKDataCircle_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphDataManager.CurrentCircle = GraphDataManager.G_Circles[comboBoxKDataCircle.SelectedIndex];
            RefreshPanel();
        }

        private void trackBarAppearRate_Scroll(object sender, EventArgs e)
        {
            graphMgr.appearenceGraph.ScrollToData(trackBarAppearRate.Value, panelUp.ClientSize.Width, panelUp.ClientSize.Height, false);
            RefreshPanel();
        }

        private void trackBarMissCount_Scroll(object sender, EventArgs e)
        {
            graphMgr.missCountGraph.ScrollToData(trackBarMissCount.Value, panelUp.ClientSize.Width, panelUp.ClientSize.Height, false);
            RefreshPanel();//触发Paint事件
        }


        void RefreshPanel()
        {
            //if (needRefresh)
            //    return;
            needRefresh = true;
            //this.Invalidate(true);//触发Paint事件
        }

        private void panelDown_Resize(object sender, EventArgs e)
        {
            needCalcDataOnGridScaleChanged = true;
            //if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            //{
            //    graphMgr.kvalueGraph.OnGridScaleChanged();
            //}
        }

        private void splitContainer2_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            needCalcDataOnGridScaleChanged = true;
            //if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            //{
            //    graphMgr.kvalueGraph.OnGridScaleChanged();
            //}
        }

        private void trackBarGraphBar_Scroll(object sender, EventArgs e)
        {
            SelItem(trackBarGraphBar.Value);
            trackBarKData.Value = trackBarGraphBar.Value;
            graphMgr.kvalueGraph.ScrollToData(trackBarGraphBar.Value, panelUp.ClientSize.Width, panelUp.ClientSize.Height, true);
            RefreshPanel();
        }

        private void buttonPrevItem_Click(object sender, EventArgs e)
        {
            if(trackBarGraphBar.Value > trackBarGraphBar.Minimum)
            {
                --trackBarGraphBar.Value;
                trackBarGraphBar_Scroll(null, null);
            }
        }

        private void buttonNextItem_Click(object sender, EventArgs e)
        {
            if (trackBarGraphBar.Value < trackBarGraphBar.Maximum)
            {
                ++trackBarGraphBar.Value;
                trackBarGraphBar_Scroll(null, null);
            }
        }
    }
}
