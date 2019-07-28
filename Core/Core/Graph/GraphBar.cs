using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    // 柱状图
    public class GraphBar : GraphBase
    {
        Font tagFont = new Font(FontFamily.GenericSerif, 12);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush greenBrush = new SolidBrush(Color.Green);
        SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
        Pen redPen = new Pen(Color.Red, 1);
        Pen yellowPen = new Pen(Color.Yellow, 1);
        Pen whitePen = new Pen(Color.White, 1);
        int selKDataIndex = -1;

        public override bool NeedRefreshCanvasOnMouseMove(Point mousePos)
        {
            return true;
        }
        public override void DrawUpGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            GraphDataContainerBarGraph bgdc = GraphDataManager.BGDC;
            if (bgdc.HasData() == false ||
                bgdc.totalCollectCount == 0)
                return;

            float startX = 0;
            float startY = 60;// winH * 0.1f;
            float bottom = winH - 30;// winH * 0.9f;
            float MaxRcH = bottom - startY;// winH * 0.8f;

            GraphDataContainerBarGraph.DataUnitLst dul = bgdc.allDatas[numIndex];
            float gap = (float)winW / dul.dataLst.Count;

            string statisticInfo = "";
            for (int i = 0; i < dul.dataLst.Count; ++i)
            {
                float r = 0;
                float rate = ((float)(dul.dataLst[i].data) / (float)bgdc.totalCollectCount);
                if (dul.dataLst[i].type == GraphDataContainerBarGraph.StatisticsType.eAppearCountPath012)
                {
                    if (string.IsNullOrEmpty(statisticInfo))
                        statisticInfo = "012路";

                    if (i == 0)
                        r = (rate - 0.4f) / 0.4f;
                    else
                        r = (rate - 0.3f) / 0.3f;
                }
                else if (dul.dataLst[i].type == GraphDataContainerBarGraph.StatisticsType.eAppearCountFrom0To9)
                {
                    if (string.IsNullOrEmpty(statisticInfo))
                        statisticInfo = "数字0-9";
                    r = (rate - 0.1f) / 0.1f;
                }
                string brand = "";
                Brush brandBrush = yellowBrush;
                Pen pen = yellowPen;
                if (r > 0.5f)
                {
                    brand = "热";
                    brandBrush = redBrush;
                    pen = redPen;
                }
                else if (r < -0.5f)
                {
                    brand = "冷";
                    brandBrush = whiteBrush;
                    pen = whitePen;
                }
                else
                {
                    brand = "温";
                }

                float rcH = MaxRcH * rate;
                if (rcH < 1)
                    rcH = 1;
                startY = bottom - rcH;
                g.DrawRectangle(pen, startX, bottom - MaxRcH, gap * 0.9f, MaxRcH);
                g.FillRectangle(brandBrush, startX, startY, gap * 0.9f, rcH);
                g.DrawString(dul.dataLst[i].tag, tagFont, brandBrush, startX, bottom);
                g.DrawString(dul.dataLst[i].data.ToString(), tagFont, brandBrush, startX, startY - 30);
                g.DrawString(brand, tagFont, brandBrush, startX, startY - 60);
                startX += gap;
            }

            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (bgdc.CurrentSelectItem != null)
                currentItem = bgdc.CurrentSelectItem;
            string info = "[" + currentItem.idTag + "] [" + currentItem.lotteryNumber + "]\n";
            info += "统计 " + currentItem.idTag + " " + KDataDictContainer.C_TAGS[numIndex] + " 前" +
                GraphDataManager.BGDC.StatisticRangeCount + "期 " + statisticInfo + "的出现概率";
            g.DrawString(info, tagFont, whiteBrush, 5, 5);
        }

        public override void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            GraphDataContainerBarGraph bgdc = GraphDataManager.BGDC;

            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (bgdc.CurrentSelectItem != null)
                currentItem = bgdc.CurrentSelectItem;
            if (currentItem == null)
                return;

            float startX = 0;
            float startY = 30;// winH * 0.1f;
            float bottom = winH - 30;// winH * 0.9f;
            float MaxRcH = bottom - startY;// winH * 0.8f;
            float r = 0;

            if (bgdc.curStatisticsType == GraphDataContainerBarGraph.StatisticsType.eAppearCountPath012)
            {
                float gap = (float)winW / 3;

                List<PathCmpInfo> res = new List<PathCmpInfo>();
                TradeDataManager.FindAllPathsProbabilities(currentItem, ref res, bgdc.StatisticRangeCount);
                
                for (int i = 0; i < res.Count; ++i)
                {
                    Brush brush = greenBrush;
                    float rate = res[i].pathValue / 100.0f;
                    if (i == 0)
                        r = (rate - 0.4f) / 0.4f;
                    else
                        r = (rate - 0.3f) / 0.3f;

                    string brand = null;
                    Brush brandBrush = yellowBrush;
                    Pen pen = yellowPen;
                    if (r > 0.5f)
                    {
                        brand = "热";
                        brandBrush = redBrush;
                        pen = redPen;
                    }
                    else if (r < -0.5f)
                    {
                        brand = "冷";
                        brandBrush = whiteBrush;
                        pen = whitePen;
                    }
                    else
                    {
                        brand = "温";
                    }

                    float rcH = MaxRcH * rate;
                    if (rcH < 1)
                        rcH = 1;
                    startY = bottom - rcH;
                    g.DrawRectangle(pen, startX, bottom - MaxRcH, gap * 0.9f, MaxRcH);
                    g.FillRectangle(brandBrush, startX, startY, gap * 0.9f, rcH);
                    g.DrawString(i.ToString(), tagFont, brandBrush, startX, bottom);
                    g.DrawString(res[i].pathValue.ToString("f1") + "%", tagFont, brandBrush, startX, startY - 30);
                    startX += gap;
                }
                g.DrawString("统计 " + currentItem.idTag + " 前" + bgdc.StatisticRangeCount + "期所有位012路的出现概率", tagFont, whiteBrush, 5, 5);

            }
            else if(bgdc.curStatisticsType == GraphDataContainerBarGraph.StatisticsType.eAppearCountFrom0To9)
            {
                List<NumberCmpInfo> nums = new List<NumberCmpInfo>();
                TradeDataManager.FindAllNumberProbabilities(currentItem, ref nums, bgdc.StatisticRangeCount);

                float gap = (float)winW / 10;
                for (int i = 0; i < nums.Count; ++i)
                {
                    int index = NumberCmpInfo.FindIndex(nums, (sbyte)(i), false);
                    Brush brush = greenBrush;
                    float rate = nums[index].rate / 100.0f;
                    r = (rate - 0.1f) / 0.1f;

                    string brand = null;
                    Brush brandBrush = yellowBrush;
                    Pen pen = yellowPen;
                    if (r > 0.5f)
                    {
                        brand = "热";
                        brandBrush = redBrush;
                        pen = redPen;
                    }
                    else if (r < -0.5f)
                    {
                        brand = "冷";
                        brandBrush = whiteBrush;
                        pen = whitePen;
                    }
                    else
                    {
                        brand = "温";
                    }

                    float rcH = MaxRcH * rate;
                    if (rcH < 1)
                        rcH = 1;
                    startY = bottom - rcH;
                    g.DrawRectangle(pen, startX, bottom - MaxRcH, gap * 0.9f, MaxRcH);
                    g.FillRectangle(brandBrush, startX, startY, gap * 0.9f, rcH);
                    g.DrawString(nums[index].number.ToString(), tagFont, brandBrush, startX, bottom);
                    g.DrawString(nums[index].rate.ToString("f1") + "%", tagFont, brandBrush, startX, startY - 30);
                    startX += gap;
                }
                g.DrawString("统计 " + currentItem.idTag + " 前" + LotteryStatisticInfo.LONG_COUNT + "期所有位数字0-9的出现概率", tagFont, whiteBrush, 5, 5);
            }
        }
    }
}
