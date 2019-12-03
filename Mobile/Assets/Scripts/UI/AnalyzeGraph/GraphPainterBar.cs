using LotteryAnalyze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPainterBar : GraphPainterBase
{

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnAutoAllign()
    {
        base.OnAutoAllign();
    }

    public override void OnGraphDragging(Vector2 offset, Painter painter)
    {
        base.OnGraphDragging(offset, painter);
    }

    public override void OnPointerClick(Vector2 pos, Painter g)
    {
        base.OnPointerClick(pos, g);
    }

    public override void OnScrollToData(float dataIndex)
    {
        base.OnScrollToData(dataIndex);
    }

    public override void DrawUpPanel(Painter g, RectTransform rtCanvas)
    {
        GraphDataContainerBarGraph bgdc = GraphDataManager.BGDC;
        if (bgdc.HasData() == false ||
            bgdc.totalCollectCount == 0)
            return;
        float winH = rtCanvas.rect.height;
        float winW = rtCanvas.rect.width;
        int numIndex = PanelAnalyze.Instance.numIndex;

        float upGap = 90;
        float downGap = 50;
        float startX = 0;
        float startY = downGap;
        float bottom = winH - upGap;
        float MaxRcH = bottom - startY;

        GraphDataContainerBarGraph.DataUnitLst dul = bgdc.allDatas[numIndex];
        float gap = (float)winW / dul.dataLst.Count;

        string statisticInfo = "";
        for (int i = 0; i < dul.dataLst.Count; ++i)
        {
            float r = dul.dataLst[i].relRate;
            float rate = dul.dataLst[i].rate;
            if (dul.dataLst[i].type == GraphDataContainerBarGraph.StatisticsType.eAppearCountPath012)
            {
                if (string.IsNullOrEmpty(statisticInfo))
                    statisticInfo = "012路";
            }
            else if (dul.dataLst[i].type == GraphDataContainerBarGraph.StatisticsType.eAppearCountFrom0To9)
            {
                if (string.IsNullOrEmpty(statisticInfo))
                    statisticInfo = "数字0-9";
            }
            string brand = "";
            Color color = Color.yellow;
            if (r > 0.5f)
            {
                brand = "热 - ";
                color = Color.red;
            }
            else if (r < -0.5f)
            {
                brand = "冷 - ";
                color = Color.white;
            }
            else
            {
                brand = "温 - ";
            }

            float rcH = MaxRcH * rate;
            if (rcH < 1)
                rcH = 1;
            float rcW = gap * 0.9f;
            g.DrawRectInCanvasSpace(startX, startY, rcW, MaxRcH, color);
            g.DrawFillRectInCanvasSpace(startX, startY, rcW, rcH, color);
            float txtX = startX + 10;
            PanelAnalyze.Instance.graphUp.DrawText(dul.dataLst[i].tag, txtX, startY, Color.white);
            PanelAnalyze.Instance.graphUp.DrawText(brand + dul.dataLst[i].data.ToString(), txtX, bottom, color);
            startX += gap;
        }

        DataItem currentItem = DataManager.GetInst().GetLatestItem();
        if (bgdc.CurrentSelectItem != null)
            currentItem = bgdc.CurrentSelectItem;
        string info = "[" + currentItem.idTag + "] [" + currentItem.lotteryNumber + "]\n";
        info += "统计 " + currentItem.idTag + " " + KDataDictContainer.C_TAGS[numIndex] + " 前" +
            GraphDataManager.BGDC.StatisticRangeCount + "期 " + statisticInfo + "的出现概率";
        PanelAnalyze.Instance.graphUp.AppendText(info);

    }

    public override void DrawDownPanel(Painter g, RectTransform rtCanvas)
    {
        GraphDataContainerBarGraph bgdc = GraphDataManager.BGDC;

        DataItem currentItem = DataManager.GetInst().GetLatestItem();
        if (bgdc.CurrentSelectItem != null)
            currentItem = bgdc.CurrentSelectItem;
        if (currentItem == null)
            return;

        float winH = rtCanvas.rect.height;
        float winW = rtCanvas.rect.width;

        float upGap = 90;
        float downGap = 50;
        float startX = 0;
        float startY = downGap;
        float bottom = winH - upGap;
        float MaxRcH = bottom - startY;
        float r = 0;

        if (bgdc.curStatisticsType == GraphDataContainerBarGraph.StatisticsType.eAppearCountPath012)
        {
            float gap = (float)winW / 3;

            List<PathCmpInfo> res = new List<PathCmpInfo>();
            TradeDataManager.FindAllPathsProbabilities(currentItem, ref res, bgdc.StatisticRangeCount);

            for (int i = 0; i < res.Count; ++i)
            {
                Color color = Color.yellow;
                float rate = res[i].pathValue / 100.0f;
                if (i == 0)
                    r = (rate - 0.4f) / 0.4f;
                else
                    r = (rate - 0.3f) / 0.3f;

                string brand = null;
                if (r > 0.5f)
                {
                    brand = "热 - ";
                    color = Color.red;
                }
                else if (r < -0.5f)
                {
                    brand = "冷 - ";
                    color = Color.white;
                }
                else
                {
                    brand = "温 - ";
                }

                float rcH = MaxRcH * rate;
                if (rcH < 1)
                    rcH = 1;
                float rcW = gap * 0.9f;
                g.DrawRectInCanvasSpace(startX, startY, rcW, MaxRcH, color);
                g.DrawFillRectInCanvasSpace(startX, startY, rcW, rcH, color);
                float txtX = startX + 10;
                PanelAnalyze.Instance.graphDown.DrawText(i.ToString(), txtX, startY, Color.white);
                PanelAnalyze.Instance.graphDown.DrawText(brand + res[i].pathValue.ToString("f1") + "%", txtX, startY + MaxRcH, color);
                startX += gap;
            }
            string info = "统计 " + currentItem.idTag + " 前" + bgdc.StatisticRangeCount + "期所有位012路的出现概率";
            PanelAnalyze.Instance.graphUp.AppendText(info);

        }
        else if (bgdc.curStatisticsType == GraphDataContainerBarGraph.StatisticsType.eAppearCountFrom0To9)
        {
            List<NumberCmpInfo> nums = new List<NumberCmpInfo>();
            TradeDataManager.FindAllNumberProbabilities(currentItem, ref nums, bgdc.StatisticRangeCount);

            float gap = (float)winW / 10;
            for (int i = 0; i < nums.Count; ++i)
            {
                int index = NumberCmpInfo.FindIndex(nums, (sbyte)(i), false);
                float rate = nums[index].rate / 100.0f;
                r = (rate - 0.1f) / 0.1f;

                string brand = null;
                Color color = Color.yellow;
                if (r > 0.5f)
                {
                    brand = "热 - ";
                    color = Color.red;
                }
                else if (r < -0.5f)
                {
                    brand = "冷 - ";
                    color = Color.white;
                }
                else
                {
                    brand = "温 - ";
                }

                float rcH = MaxRcH * rate;
                if (rcH < 1)
                    rcH = 1;
                float rcW = gap * 0.9f;
                g.DrawRectInCanvasSpace(startX, startY, rcW, MaxRcH, color);
                g.DrawFillRectInCanvasSpace(startX, startY, rcW, rcH, color);
                float txtX = startX + 10;
                PanelAnalyze.Instance.graphDown.DrawText(nums[index].number.ToString(), txtX, startY, Color.white);
                PanelAnalyze.Instance.graphDown.DrawText(brand + nums[index].rate.ToString("f1") + "%", txtX, startY + MaxRcH, color);
                startX += gap;
            }
            string info = "统计 " + currentItem.idTag + " 前" + bgdc.StatisticRangeCount + "期所有位数字0-9的出现概率";
            PanelAnalyze.Instance.graphDown.AppendText(info);
        }
    }
}
