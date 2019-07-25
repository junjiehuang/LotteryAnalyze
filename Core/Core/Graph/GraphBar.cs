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
        Font tagFont = new Font(FontFamily.GenericSerif, 16);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush tagBrush = new SolidBrush(Color.White);
        SolidBrush greenBrush = new SolidBrush(Color.Green);
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
            float startY = winH * 0.1f;
            float MaxRcH = winH * 0.8f;
            float bottom = winH * 0.9f;

            GraphDataContainerBarGraph.DataUnitLst dul = bgdc.allDatas[numIndex];
            float gap = (float)winW / dul.dataLst.Count;

            string statisticInfo = "";
            for (int i = 0; i < dul.dataLst.Count; ++i)
            {
                float r = 0;
                Brush brush = greenBrush;
                float rate = ((float)(dul.dataLst[i].data) / (float)bgdc.totalCollectCount);
                if (dul.dataLst[i].type == GraphDataContainerBarGraph.StatisticsType.eAppearCountPath012)
                {
                    if (string.IsNullOrEmpty(statisticInfo))
                        statisticInfo = "012路";
                    if (i == 0 && rate > 0.4f)
                        brush = redBrush;
                    else if (i > 0 && rate > 0.3f)
                        brush = redBrush;

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
                    if (rate > 0.1f)
                        brush = redBrush;
                }
                string brand = "";
                Brush brandBrush = tagBrush;
                if (r > 0.5f)
                {
                    brand = "热";
                    brandBrush = redBrush;
                }
                else if (r < -0.5f)
                {
                    brand = "冷";
                    brandBrush = greenBrush;
                }
                else
                {
                    brand = "温";
                }

                float rcH = MaxRcH * rate;
                startY = bottom - rcH;
                g.FillRectangle(brush, startX, startY, gap * 0.9f, rcH);
                g.DrawString(dul.dataLst[i].tag, tagFont, tagBrush, startX, bottom);
                g.DrawString(dul.dataLst[i].data.ToString(), tagFont, tagBrush, startX, startY - 30);
                g.DrawString(brand, tagFont, brandBrush, startX, 60);
                startX += gap;
            }

            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (bgdc.CurrentSelectItem != null)
                currentItem = bgdc.CurrentSelectItem;
            string info = "[" + currentItem.idTag + "] [" + currentItem.lotteryNumber + "]\n";
            info += "统计 " + currentItem.idTag + " " + KDataDictContainer.C_TAGS[numIndex] + " 前" +
                GraphDataManager.BGDC.StatisticRangeCount + "期 " + statisticInfo + "的出现概率";
            g.DrawString(info, tagFont, tagBrush, 5, 5);
        }

        public override void DrawDownGraph(Graphics g, int numIndex, CollectDataType cdt, int winW, int winH, Point mouseRelPos)
        {
            GraphDataContainerBarGraph bgdc = GraphDataManager.BGDC;
            if (bgdc.HasData() == false ||
                bgdc.totalCollectCount == 0 ||
                bgdc.curStatisticsType != GraphDataContainerBarGraph.StatisticsType.eAppearCountFrom0To9)
                return;

            DataItem currentItem = DataManager.GetInst().GetLatestItem();
            if (bgdc.CurrentSelectItem != null)
                currentItem = bgdc.CurrentSelectItem;
            if (currentItem == null)
                return;
            List<NumberCmpInfo> nums = new List<NumberCmpInfo>();
            TradeDataManager.FindAllNumberProbabilities(currentItem, ref nums);
            //nums.Sort(NumberCmpInfo.SortByNumber);

            float startX = 0;
            float startY = winH * 0.1f;
            float MaxRcH = winH * 0.8f;
            float bottom = winH * 0.9f;

            float gap = (float)winW / 10;
            Font tagFont = new Font(FontFamily.GenericSerif, 12);
            for (int i = 0; i < nums.Count; ++i)
            {
                int index = NumberCmpInfo.FindIndex(nums, (sbyte)(i), false);
                Brush brush = greenBrush;
                float rate = nums[index].rate / 100.0f;
                if (rate > 0.1f)
                    brush = redBrush;

                float rcH = MaxRcH * rate;
                startY = bottom - rcH;
                g.FillRectangle(brush, startX, startY, gap * 0.9f, rcH);
                g.DrawString(nums[index].number.ToString(), tagFont, tagBrush, startX, bottom);
                g.DrawString(nums[index].rate.ToString("f1") + "%", tagFont, tagBrush, startX, startY - 30);
                startX += gap;
            }
            g.DrawString("统计 " + currentItem.idTag + " 前" + LotteryStatisticInfo.LONG_COUNT + "期所有位数字0-9的出现概率", tagFont, tagBrush, 5, 5);
        }
    }
}
