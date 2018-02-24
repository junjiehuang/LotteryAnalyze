﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze.UI
{
    public partial class LotteryGraph : Form
    {
        static List<LotteryGraph> instLst = new List<LotteryGraph>();

        GraphManager graphMgr = new GraphManager();
        int numberIndex = 0;        
        int curCDTIndex = 0;
        Point upPanelMousePosLastDrag = new Point();
        Point upPanelMousePosOnMove = new Point();
        Point upPanelMousePosOnBtnDown = new Point();
        Point downPanelMousePosOnMove = new Point();
        bool hasNewDataUpdate = false;
        static Pen redPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Red, 2);
        static Pen greenPen = GraphUtil.GetLinePen(System.Drawing.Drawing2D.DashStyle.Solid, Color.Green, 2);
        Pen curUpdatePen;
        System.Windows.Forms.Timer updateTimer;


        #region ctor and common

        public static void Open(bool forceCreateNewOne)
        {
            if (forceCreateNewOne || instLst.Count == 0)
            {
                LotteryGraph graphInst = new LotteryGraph();
                graphInst.Show();
            }
        }
        
        public LotteryGraph()
        {
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
            textBoxCycleLength.Text = GraphDataManager.KGDC.CycleLength.ToString();

            comboBoxBarCollectType.DataSource = BarGraphDataContianer.S_StatisticsType_STRS;
            comboBoxCollectRange.DataSource = BarGraphDataContianer.S_StatisticsRange_STRS;
            comboBoxBarCollectType.SelectedIndex = (int)GraphDataManager.BGDC.curStatisticsType;
            comboBoxCollectRange.SelectedIndex = (int)GraphDataManager.BGDC.curStatisticsRange;
            textBoxCustomCollectRange.Text = GraphDataManager.BGDC.customStatisticsRange.ToString();

            comboBoxAvgAlgorithm.DataSource = KGraphDataContainer.S_AVG_ALGORITHM_STRS;
            comboBoxAvgAlgorithm.SelectedIndex = (int)KGraphDataContainer.S_AVG_ALGORITHM;

            checkBoxShowAvgLines.Checked = graphMgr.kvalueGraph.enableAvgLines;
            groupBoxAvgSettings.Enabled = graphMgr.kvalueGraph.enableAvgLines;
            checkBoxBollinBand.Checked = graphMgr.kvalueGraph.enableBollinBand;
            checkBoxMACD.Checked = graphMgr.kvalueGraph.enableMACD;

            SetUIGridWH();
            RefreshUI();

            graphMgr.kvalueGraph.autoAllign = true;
            comboBoxOperations.DataSource = GraphKCurve.S_AUX_LINE_OPERATIONS;
            comboBoxOperations.SelectedIndex = (int)graphMgr.kvalueGraph.auxOperationIndex;
            checkBoxShowAuxLines.Checked = graphMgr.kvalueGraph.enableAuxiliaryLine;

            updateTimer = new Timer();
            updateTimer.Interval = 500;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
            curUpdatePen = redPen;

            comboBoxTradeNumIndex.DataSource = KDataDictContainer.C_TAGS;
            comboBoxTradeNumIndex.SelectedIndex = TradeDataManager.Instance.simSelNumIndex;
            checkBoxTradeSpecNumIndex.Checked = TradeDataManager.Instance.simSelNumIndex != -1;
            textBoxDefaultCount.Text = TradeDataManager.Instance.defaultTradeCount.ToString();
            textBoxMultiCount.Text = TradeDataManager.Instance.GetTradeCountInfoStr();
            textBoxStartMoney.Text = TradeDataManager.Instance.startMoney.ToString();

        }

        void SetUIGridWH()
        {
            if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
            {
                textBoxGridScaleW.Text = graphMgr.kvalueGraph.gridScaleW.ToString();
                textBoxGridScaleH.Text = graphMgr.kvalueGraph.gridScaleH.ToString();
            }
            else if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
            {
                textBoxGridScaleW.Text = graphMgr.tradeGraph.gridScaleW.ToString();
                textBoxGridScaleH.Text = graphMgr.tradeGraph.gridScaleH.ToString();
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            trackBarKData.Minimum = 0;
            trackBarKData.Maximum = GraphDataManager.KGDC.DataLength();
            trackBarTradeData.Minimum = 0;
            trackBarTradeData.Maximum = TradeDataManager.Instance.historyTradeDatas.Count;
            if(hasNewDataUpdate)
            {
                if (curUpdatePen == redPen)
                    curUpdatePen = greenPen;
                else if (curUpdatePen == greenPen)
                    curUpdatePen = redPen;
                else
                    curUpdatePen = redPen;
                Invalidate(true);
            }
            else
                curUpdatePen = redPen;
        }

        void RefreshUI()
        {
            CheckBox[] cbs = new CheckBox[] { checkBoxAvg5, checkBoxAvg10, checkBoxAvg20, checkBoxAvg30, checkBoxAvg50, checkBoxAvg100, };
            Button[] btns = new Button[] { buttonAvg5, buttonAvg10, buttonAvg20, buttonAvg30, buttonAvg50, buttonAvg100, };
            for (int i = 0; i < KGraphDataContainer.S_AVG_LINE_SETTINGS.Count; ++i)
            {
                cbs[i].Tag = KGraphDataContainer.S_AVG_LINE_SETTINGS[i];
                cbs[i].Checked = KGraphDataContainer.S_AVG_LINE_SETTINGS[i].enable;
                cbs[i].Text = KGraphDataContainer.S_AVG_LINE_SETTINGS[i].tag;
                btns[i].BackColor = KGraphDataContainer.S_AVG_LINE_SETTINGS[i].color;
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

        #endregion

        #region Draw and Operations

        void DrawUpCanvas(Graphics g)
        {
            g.Clear(Color.Black);

            CollectDataType cdt = GraphDataManager.S_CDT_LIST[curCDTIndex];
            graphMgr.DrawUpGraph(g, numberIndex, cdt, this.panelUp.ClientSize.Width, this.panelUp.ClientSize.Height, upPanelMousePosOnMove);

            Rectangle r = new Rectangle(1, 1, this.panelUp.ClientSize.Width - 2, this.panelUp.ClientSize.Height - 2);
            g.DrawRectangle(curUpdatePen, r);

            g.Flush();
        }

        void DrawDownCanvas(Graphics g)
        {
            g.Clear(Color.Black);

            CollectDataType cdt = GraphDataManager.S_CDT_LIST[curCDTIndex];
            graphMgr.DrawDownGraph(g, numberIndex, cdt, this.panelDown.ClientSize.Width, this.panelDown.ClientSize.Height, downPanelMousePosOnMove);

            Rectangle r = new Rectangle(1, 1, this.panelDown.ClientSize.Width - 2, this.panelDown.ClientSize.Height - 2);
            g.DrawRectangle(curUpdatePen, r);

            g.Flush();
        }
        

        private void panelUp_MouseMove(object sender, MouseEventArgs e)
        {
            upPanelMousePosOnMove = e.Location;
            bool needUpdate = false;
            if (graphMgr.NeedRefreshCanvasOnMouseMove(e.Location))
                needUpdate = true;
            if (e.Button == MouseButtons.Left)
            {
                if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
                {
                    if (graphMgr.kvalueGraph.selAuxLine == null)
                    {
                        float dx = e.Location.X - upPanelMousePosLastDrag.X;
                        float dy = e.Location.Y - upPanelMousePosLastDrag.Y;
                        upPanelMousePosLastDrag = e.Location;
                        graphMgr.MoveGraph(dx, dy);
                        needUpdate = true;
                    }
                    else
                    {
                        graphMgr.kvalueGraph.UpdateSelectAuxLinePoint( upPanelMousePosOnMove );
                    }
                }
                else if(graphMgr.CurrentGraphType == GraphType.eTradeGraph)
                {
                    float dx = e.Location.X - upPanelMousePosLastDrag.X;
                    float dy = e.Location.Y - upPanelMousePosLastDrag.Y;
                    upPanelMousePosLastDrag = e.Location;
                    graphMgr.MoveGraph(dx, dy);
                    needUpdate = true;
                }
            }
            if (needUpdate)
                this.Invalidate(true);//触发Paint事件
        }

        private void panelUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (hasNewDataUpdate)
                hasNewDataUpdate = false;
            upPanelMousePosLastDrag = e.Location;
            upPanelMousePosOnBtnDown = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph &&
                    graphMgr.kvalueGraph.enableAuxiliaryLine)
                {
                    if(graphMgr.kvalueGraph.auxOperationIndex == AuxLineType.eNone)
                        graphMgr.kvalueGraph.SelectAuxLine(e.Location);
                }
            }
        }

        private void panelUp_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if(graphMgr.CurrentGraphType == GraphType.eTradeGraph)
                {
                    int kdataID = -1;
                    if (e.X == upPanelMousePosOnBtnDown.X && e.Y == upPanelMousePosOnBtnDown.Y)
                    {
                        int selID = graphMgr.tradeGraph.SelectTradeData(e.Location);
                        if (selID != -1)
                        {
                            kdataID = TradeDataManager.Instance.historyTradeDatas[selID].targetLotteryItem.idGlobal;

                            GraphDataManager.BGDC.CurrentSelectItem = TradeDataManager.Instance.historyTradeDatas[selID].targetLotteryItem;
                            GraphDataManager.Instance.CollectGraphData(GraphType.eBarGraph);
                        }
                        else
                        {
                            GraphDataManager.BGDC.CurrentSelectItem = null;
                            GraphDataManager.Instance.CollectGraphData(GraphType.eBarGraph);
                        }
                    }
                    else
                    {
                        GraphDataManager.BGDC.CurrentSelectItem = null;
                        GraphDataManager.Instance.CollectGraphData(GraphType.eBarGraph);

                        graphMgr.tradeGraph.UnselectTradeData();
                    }
                    graphMgr.kvalueGraph.ScrollToData(kdataID, panelUp.ClientSize.Width, panelUp.ClientSize.Height);
                    if (kdataID != -1)
                        FormMain.Instance.SelectDataItem(kdataID);
                }
                else if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph &&
                    graphMgr.kvalueGraph.enableAuxiliaryLine)
                {
                    switch (graphMgr.kvalueGraph.auxOperationIndex)
                    {
                        case AuxLineType.eNone:
                            {
                                graphMgr.kvalueGraph.SelectAuxLine(e.Location);
                            }
                            break;
                        case AuxLineType.eHorzLine:
                            {
                                graphMgr.kvalueGraph.AddHorzLine(e.Location);
                                graphMgr.kvalueGraph.mouseHitPts.Clear();
                            }
                            break;
                        case AuxLineType.eVertLine:
                            {
                                graphMgr.kvalueGraph.AddVertLine(e.Location);
                                graphMgr.kvalueGraph.mouseHitPts.Clear();
                            }
                            break;
                        case AuxLineType.eSingleLine:
                            {
                                graphMgr.kvalueGraph.mouseHitPts.Add(e.Location);
                                if (graphMgr.kvalueGraph.mouseHitPts.Count == 2)
                                {
                                    graphMgr.kvalueGraph.AddSingleLine(
                                        graphMgr.kvalueGraph.mouseHitPts[0],
                                        graphMgr.kvalueGraph.mouseHitPts[1]);
                                    graphMgr.kvalueGraph.mouseHitPts.Clear();
                                }
                            }
                            break;
                        case AuxLineType.eChannelLine:
                            {
                                graphMgr.kvalueGraph.mouseHitPts.Add(e.Location);
                                if (graphMgr.kvalueGraph.mouseHitPts.Count == 3)
                                {
                                    graphMgr.kvalueGraph.AddChannelLine(
                                        graphMgr.kvalueGraph.mouseHitPts[0],
                                        graphMgr.kvalueGraph.mouseHitPts[1],
                                        graphMgr.kvalueGraph.mouseHitPts[2]);
                                    graphMgr.kvalueGraph.mouseHitPts.Clear();
                                }
                            }
                            break;
                        case AuxLineType.eGoldSegmentedLine:
                            {
                                graphMgr.kvalueGraph.mouseHitPts.Add(e.Location);
                                if (graphMgr.kvalueGraph.mouseHitPts.Count == 2)
                                {
                                    graphMgr.kvalueGraph.AddGoldSegLine(
                                        graphMgr.kvalueGraph.mouseHitPts[0],
                                        graphMgr.kvalueGraph.mouseHitPts[1]);
                                    graphMgr.kvalueGraph.mouseHitPts.Clear();
                                }
                            }
                            break;
                    }
                }
            }
            else if(e.Button == MouseButtons.Right)
            {
                graphMgr.kvalueGraph.mouseHitPts.Clear();
            }
        }
        private void panelDown_MouseMove(object sender, MouseEventArgs e)
        {
            downPanelMousePosOnMove = e.Location;
            this.Invalidate(true);//触发Paint事件
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
                if (graphMgr.kvalueGraph.selAuxLine != null)
                {
                    graphMgr.kvalueGraph.RemoveSelectAuxLine();
                }
            }
        }

        private void trackBarKData_Scroll(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.ScrollToData(trackBarKData.Value, panelUp.ClientSize.Width, panelUp.ClientSize.Height);
            this.Invalidate(true);//触发Paint事件
        }

        private void trackBarTradeData_Scroll(object sender, EventArgs e)
        {
            TradeDataManager tdm = TradeDataManager.Instance;
            graphMgr.tradeGraph.ScrollToData(trackBarTradeData.Value, panelUp.ClientSize.Width, panelUp.ClientSize.Height);
            this.Invalidate(true);//触发Paint事件
        }

        #endregion

        #region control callbacks
        private void LotteryGraph_FormClosed(object sender, FormClosedEventArgs e)
        {
            instLst.Remove(this);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            this.Invalidate(true);//触发Paint事件
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014) // 禁掉清除背景消息
                return;
            base.WndProc(ref m);
        }

        private void comboBoxNumIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.autoAllign = true;
            numberIndex = comboBoxNumIndex.SelectedIndex;
            this.Invalidate(true);//触发Paint事件
        }

        private void comboBoxCollectionDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.autoAllign = true;
            curCDTIndex = comboBoxCollectionDataType.SelectedIndex;
            this.Invalidate(true);//触发Paint事件
        }

        private void textBoxCycleLength_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCycleLength.Text) == false)
            {
                int value = 5;
                if (int.TryParse(textBoxCycleLength.Text, out value))
                {
                    GraphDataManager.KGDC.CycleLength = value;
                    GraphDataManager.KGDC.CollectGraphData();
                    this.Invalidate(true);//触发Paint事件
                }
            }
        }
        
        private void tabControlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.SetCurrentGraph((GraphType)(tabControlView.SelectedIndex+1));
            //if(graphMgr.CurrentGraphType == GraphType.eBarGraph)
            //    GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            SetUIGridWH();
            this.Invalidate(true);
        }

        private void comboBoxBarCollectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphDataManager.BGDC.curStatisticsType = (BarGraphDataContianer.StatisticsType)comboBoxBarCollectType.SelectedIndex;
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            this.Invalidate(true);
        }

        private void comboBoxCollectRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphDataManager.BGDC.curStatisticsRange = (BarGraphDataContianer.StatisticsRange)comboBoxCollectRange.SelectedIndex;
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            this.Invalidate(true);
        }

        private void textBoxCustomCollectRange_TextChanged(object sender, EventArgs e)
        {
            GraphDataManager.BGDC.customStatisticsRange = int.Parse(textBoxCustomCollectRange.Text);
            GraphDataManager.Instance.CollectGraphData(graphMgr.CurrentGraphType);
            this.Invalidate(true);
        }

        private void comboBoxAvgAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.S_AVG_ALGORITHM = (AvgAlgorithm)comboBoxAvgAlgorithm.SelectedIndex;
            NotifyOtherGraphRefreshUI(this);
            GraphDataManager.KGDC.CollectAvgDatas();
            this.Invalidate(true);
        }

        private void checkBoxAvg5_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg5.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg5.Checked;
            NotifyOtherGraphRefreshUI(this);
            this.Invalidate(true);
        }

        private void checkBoxAvg10_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg10.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg10.Checked;
            NotifyOtherGraphRefreshUI(this);
            this.Invalidate(true);
        }

        private void checkBoxAvg20_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg20.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg20.Checked;
            NotifyOtherGraphRefreshUI(this);
            this.Invalidate(true);
        }

        private void checkBoxAvg30_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg30.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg30.Checked;
            NotifyOtherGraphRefreshUI(this);
            this.Invalidate(true);
        }

        private void checkBoxAvg50_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg50.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg50.Checked;
            NotifyOtherGraphRefreshUI(this);
            this.Invalidate(true);
        }

        private void checkBoxAvg100_CheckedChanged(object sender, EventArgs e)
        {
            KGraphDataContainer.AvgLineSetting als = checkBoxAvg100.Tag as KGraphDataContainer.AvgLineSetting;
            als.enable = checkBoxAvg100.Checked;
            NotifyOtherGraphRefreshUI(this);
            this.Invalidate(true);
        }

        private void checkBoxBollinBand_CheckedChanged(object sender, EventArgs e)
        {
           graphMgr.kvalueGraph.enableBollinBand = checkBoxBollinBand.Checked;
            this.Invalidate(true);
        }

        private void checkBoxMACD_CheckedChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.enableMACD = checkBoxMACD.Checked;
            this.Invalidate(true);
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
                graphMgr.kvalueGraph.gridScaleH = v;
            else if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
                graphMgr.tradeGraph.gridScaleH = v;
            this.Invalidate(true);
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
                graphMgr.kvalueGraph.gridScaleW = v;
            else if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
                graphMgr.tradeGraph.gridScaleW = v;
            this.Invalidate(true);
        }

        private void autoAllignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (graphMgr.CurrentGraphType == GraphType.eKCurveGraph)
                graphMgr.kvalueGraph.autoAllign = true;
            else if (graphMgr.CurrentGraphType == GraphType.eTradeGraph)
                graphMgr.tradeGraph.autoAllign = true;
            this.Invalidate(true);
        }

        private void checkBoxShowAvgLines_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowAvgLines.Checked)
                groupBoxAvgSettings.Enabled = true;
            else
                groupBoxAvgSettings.Enabled = false;
            graphMgr.kvalueGraph.enableAvgLines = checkBoxShowAvgLines.Checked;
            this.Invalidate(true);
        }

        private void comboBoxOperations_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.auxOperationIndex = (AuxLineType)comboBoxOperations.SelectedIndex;
        }
        private void delAllAuxLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.RemoveAllAuxLines();
            this.Invalidate(true);
        }
        private void checkBoxShowAuxLines_CheckedChanged(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.enableAuxiliaryLine = checkBoxShowAuxLines.Checked;
            this.Invalidate(true);
        }
        private void cancelAddAuxLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphMgr.kvalueGraph.mouseHitPts.Clear();
        }

        #endregion

        #region trade call back

        private void tradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TradeWindow win = new TradeWindow();
            win.Show();
        }

        private void tradeSimFromFirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                TradeDataManager.Instance.simSelNumIndex = comboBoxTradeNumIndex.SelectedIndex;
            else
                TradeDataManager.Instance.simSelNumIndex = -1;
        }

        private void comboBoxTradeNumIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBoxTradeSpecNumIndex.Checked)
                TradeDataManager.Instance.simSelNumIndex = comboBoxTradeNumIndex.SelectedIndex;
            else
                TradeDataManager.Instance.simSelNumIndex = -1;
        }

        private void clearAllTradeDatasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TradeDataManager.Instance.ClearAllTradeDatas();
        }

        private void textBoxStartMoney_TextChanged(object sender, EventArgs e)
        {
            TradeDataManager.Instance.startMoney = float.Parse(textBoxStartMoney.Text);
        }


        #endregion

        private void globalSimTradeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalSimTradeWindow.Open();
        }


    }
}
