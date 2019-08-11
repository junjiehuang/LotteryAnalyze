using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
    #region base class
    public class ColumnBase
    {
        public bool forceActive = false;
        public bool active = true;
        public int columnID = -1;

        public ColumnBase() { }
        public virtual string GetColumnName() { return "ColumnBase"; }
        public virtual void SetColumnText(DataItem item, DataGridViewRow row) { throw new Exception("Need override function SetColumnText()"); }
        public virtual int GetColumnCount() { return 1; }
        public virtual void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            columnID = startIndex;
            startIndex++;
            int id = view.Columns.Add(GetColumnName(), GetColumnName());
        }
        public virtual void OnAddRow(DataItem item, List<object> colValues) { throw new Exception("Need override function OnAddRow()"); }
    }

    public class ColumnSet : ColumnBase
    {
        public List<ColumnBase> subColumns = new List<ColumnBase>();

        public ColumnSet() { }
        public override string GetColumnName() { return "ColumnSet"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row) 
        {
            for (int i = 0; i < subColumns.Count; ++i)
            {
                ColumnBase subcol = subColumns[i];
                subcol.SetColumnText(item, row);
            }
        }
        public override int GetColumnCount() 
        {
            int count = 0;
            for (int i = 0; i < subColumns.Count; ++i)
            {
                ColumnBase subcol = subColumns[i];
                count += subcol.GetColumnCount();
            }
            return count;
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            for (int i = 0; i < subColumns.Count; ++i)
            {
                ColumnBase subcol = subColumns[i];
                subcol.SetColumnIndex(ref startIndex, view);
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues) 
        {
            for (int i = 0; i < subColumns.Count; ++i)
            {
                ColumnBase subcol = subColumns[i];
                subcol.OnAddRow(item, colValues);
            }
        }

    }

    public class DataGridViewColumnManager
    {
        static int sActiveColumnCount = 0;
        static List<ColumnBase> sColumns = new List<ColumnBase>();

        public static List<ColumnBase> COLUMNS
        {
            get { return sColumns; }
        }

        static DataGridViewColumnManager()
        {
            sColumns.Add(new ColumnGlobalID());
            sColumns.Add(new ColumnIDTag());
            sColumns.Add(new ColumnNumber());
#if ENABLE_GROUP_COLLECT
            sColumns.Add(new ColumnSimulateGroup3BuyLottery());
            sColumns.Add(new ColumnSimulateGroup2BuyLottery());
#endif
            sColumns.Add(new ColumnSimulateSingleBuyLottery());
        }

        public static void ReassignColumns(DataGridView view)
        {
            view.Rows.Clear();
            view.Columns.Clear();
            int startIndex = 0;
            for (int i = 0; i < sColumns.Count; ++i)
            {
                ColumnBase col = sColumns[i];
                if (col.forceActive || col.active)
                {
                    col.SetColumnIndex(ref startIndex, view);
                }
            }
            sActiveColumnCount = startIndex;
        }

        public static void AddRow(DataItem item, DataGridView view)
        {
            List<object> parms = new List<object>();
            for (int i = 0; i < sColumns.Count; ++i)
            {
                ColumnBase col = sColumns[i];
                if (col.forceActive || col.active)
                {
                    col.OnAddRow(item, parms);
                }
            }
            int rid = view.Rows.Add(parms.ToArray());
            DataGridViewRow row = view.Rows[rid];
            row.Tag = item;
        }

        public static void SetColumnText(DataItem item, DataGridViewRow row)
        {
            for (int i = 0; i < sColumns.Count; ++i)
            {
                ColumnBase col = sColumns[i];
                if (col.forceActive || col.active)
                {
                    col.SetColumnText(item, row);
                }
            }
        }
    }

#endregion

#region common data column

    public class ColumnGlobalID : ColumnBase
    {
        public ColumnGlobalID()
        {
            forceActive = true;
        }
        public override string GetColumnName() { return "索引"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = item.idGlobal;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add(item.idGlobal);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.Width = 60;
        }
    }

    public class ColumnIDTag : ColumnBase
    {
        public ColumnIDTag()
        {
            forceActive = true;
        }
        public override string GetColumnName() { return "期号"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row) 
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = item.idTag;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add(item.idTag);
        }
    }

    public class ColumnNumber : ColumnBase
    {
        public ColumnNumber()
        {
            forceActive = true;
        }
        public override string GetColumnName() { return "出号"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = item.lotteryNumber;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add(item.lotteryNumber);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.Width = 60;
        }
    }
#endregion

#region group 3 column
#if ENABLE_GROUP_COLLECT
    public class ColumnAndValue : ColumnBase
    {
        public ColumnAndValue()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "和值"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = item.andValue;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add(item.andValue);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.Width = 60;
        }
    }

    public class ColumnAndRearValue : ColumnBase
    {
        public ColumnAndRearValue()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "合值"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = item.rearValue;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add(item.rearValue);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.Width = 60;
        }
    }

    public class ColumnG6 : ColumnBase
    {
        public ColumnG6()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "组六"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.groupType == GroupType.eGT6 ? "组六" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.groupType == GroupType.eGT6 ? "组六" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkRed;
            col.Width = 60;
        }
    }

    public class ColumnG3 : ColumnBase
    {
        public ColumnG3()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "组三"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.groupType == GroupType.eGT3 ? "组三" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.groupType == GroupType.eGT3 ? "组三" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkGreen;
            col.Width = 60;
        }
    }

    public class ColumnG1 : ColumnBase
    {
        public ColumnG1()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "豹子"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.groupType == GroupType.eGT1 ? "豹子" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.groupType == GroupType.eGT1 ? "豹子" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 60;
        }
    }

    public class ColumnGroupType : ColumnSet
    {
        public ColumnGroupType() 
        {
            forceActive = true;
            subColumns.Add(new ColumnG6());
            subColumns.Add(new ColumnG3());
            subColumns.Add(new ColumnG1());
        }
        public override string GetColumnName() { return "组选类型"; }
    }

    public class ColumnGroupScore : ColumnBase
    {
        public ColumnGroupScore()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "组选评分"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (int)item.simData.g6Score + " : " + (int)item.simData.g3Score + " : " + (int)item.simData.g1Score;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((int)item.simData.g6Score + " : " + (int)item.simData.g3Score + " : " + (int)item.simData.g1Score);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            //col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            //col.Width = 60;
        }
    }

    public class ColumnGroup3KillNumber : ColumnBase
    {
        public ColumnGroup3KillNumber()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "模拟杀号"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                string kt = "";
                switch (item.simData.killType)
                {
                    case KillType.eKTGroup3: kt = "杀组三 "; break;
                    case KillType.eKTGroup6: kt = "杀组六 "; break;
                    case KillType.eKTNone: kt = "忽略 "; break;
                }
                cell.Value = kt + item.simData.killList;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add("");
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
        }
    }
#endif
#endregion

#region group 2 column
#if ENABLE_GROUP_COLLECT
    public class ColumnTenOdd : ColumnBase
    {
        public ColumnTenOdd()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "十位单"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.GetShiNumber() % 2 == 1 ? "单" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.GetShiNumber() % 2 == 1 ? "单" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 60;
        }
    }
    public class ColumnTenEven : ColumnBase
    {
        public ColumnTenEven()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "十位双"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.GetShiNumber() % 2 == 0 ? "双" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.GetShiNumber() % 2 == 0 ? "双" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 60;
        }
    }
    public class ColumnTenBig : ColumnBase
    {
        public ColumnTenBig()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "十位大"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.GetShiNumber() > 4 ? "大" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.GetShiNumber() > 4 ? "大" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 60;
        }
    }
    public class ColumnTenSmall : ColumnBase
    {
        public ColumnTenSmall()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "十位小"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.GetShiNumber() <= 4 ? "小" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.GetShiNumber() <= 4 ? "小" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 60;
        }
    }
    public class ColumnGroupTen : ColumnSet
    {
        public ColumnGroupTen()
        {
            forceActive = true;
            subColumns.Add(new ColumnTenOdd());
            subColumns.Add(new ColumnTenEven());
            subColumns.Add(new ColumnTenBig());
            subColumns.Add(new ColumnTenSmall());
        }
        public override string GetColumnName() { return "十位"; }
    }



    public class ColumnGeOdd : ColumnBase
    {
        public ColumnGeOdd()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "个位单"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.GetGeNumber() % 2 == 1 ? "单" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.GetGeNumber() % 2 == 1 ? "单" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 60;
        }
    }
    public class ColumnGeEven : ColumnBase
    {
        public ColumnGeEven()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "个位双"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.GetGeNumber() % 2 == 0 ? "双" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.GetGeNumber() % 2 == 0 ? "双" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 60;
        }
    }
    public class ColumnGeBig : ColumnBase
    {
        public ColumnGeBig()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "个位大"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.GetGeNumber() > 4 ? "大" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.GetGeNumber() > 4 ? "大" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 60;
        }
    }
    public class ColumnGeSmall : ColumnBase
    {
        public ColumnGeSmall()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "个位小"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (item.GetGeNumber() <= 4 ? "小" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add((item.GetGeNumber() <= 4 ? "小" : ""));
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 60;
        }
    }
    public class ColumnGroupGe : ColumnSet
    {
        public ColumnGroupGe()
        {
            forceActive = true;
            subColumns.Add(new ColumnGeOdd());
            subColumns.Add(new ColumnGeEven());
            subColumns.Add(new ColumnGeBig());
            subColumns.Add(new ColumnGeSmall());
        }
        public override string GetColumnName() { return "个位"; }
    }

#endif
#endregion

#region single column

    public class ColumnSinglePath0 : ColumnBase
    {
        public ColumnSinglePath0()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "0路"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (ColumnSimulateSingleBuyLottery.GetNum(item) % 3 == 0 ? "0" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add(ColumnSimulateSingleBuyLottery.GetNum(item) % 3 == 0 ? "0" : "");
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 40;
        }
    }
    public class ColumnSinglePath1 : ColumnBase
    {
        public ColumnSinglePath1()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "1路"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (ColumnSimulateSingleBuyLottery.GetNum(item) % 3 == 1 ? "1" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add(ColumnSimulateSingleBuyLottery.GetNum(item) % 3 == 1 ? "1" : "");
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 40;
        }
    }
    public class ColumnSinglePath2 : ColumnBase
    {
        public ColumnSinglePath2()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "2路"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = (ColumnSimulateSingleBuyLottery.GetNum(item) % 3 == 2 ? "2" : "");
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add(ColumnSimulateSingleBuyLottery.GetNum(item) % 3 == 2 ? "2" : "");
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 40;
        }
    }

    public class ColumnSinglePath012Missing : ColumnBase
    {
        public ColumnSinglePath012Missing()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "012路遗漏"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                //string info = item.simData.path012MissingInfo[ColumnSimulateSingleBuyLottery.S_INDEX][0] + " ; " +
                //    item.simData.path012MissingInfo[ColumnSimulateSingleBuyLottery.S_INDEX][1] + " : " +
                //    item.simData.path012MissingInfo[ColumnSimulateSingleBuyLottery.S_INDEX][2];
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
                string info =   sum.statisticUnitMap[CollectDataType.ePath0].missCount + " ; " +
                                sum.statisticUnitMap[CollectDataType.ePath1].missCount + " : " +
                                sum.statisticUnitMap[CollectDataType.ePath2].missCount;
                cell.Value = (info);
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            //string info = item.simData.path012MissingInfo[ColumnSimulateSingleBuyLottery.S_INDEX][0] + " ; " +
            //        item.simData.path012MissingInfo[ColumnSimulateSingleBuyLottery.S_INDEX][1] + " : " +
            //        item.simData.path012MissingInfo[ColumnSimulateSingleBuyLottery.S_INDEX][2];
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
            string info = sum.statisticUnitMap[CollectDataType.ePath0].missCount + " ; " +
                            sum.statisticUnitMap[CollectDataType.ePath1].missCount + " : " +
                            sum.statisticUnitMap[CollectDataType.ePath2].missCount;
            colValues.Add(info);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 80;
        }
    }
    public class ColumnSinglePath012CountLong : ColumnBase
    {
        public ColumnSinglePath012CountLong()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "012次数统计全期"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                //string info = item.simData.path012CountInfoLong[ColumnSimulateSingleBuyLottery.S_INDEX][0] + " : " +
                //    item.simData.path012CountInfoLong[ColumnSimulateSingleBuyLottery.S_INDEX][1] + " : " +
                //    item.simData.path012CountInfoLong[ColumnSimulateSingleBuyLottery.S_INDEX][2];
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
                string info = sum.statisticUnitMap[CollectDataType.ePath0].sample30Data.appearCount + " ; " +
                                sum.statisticUnitMap[CollectDataType.ePath1].sample30Data.appearCount + " : " +
                                sum.statisticUnitMap[CollectDataType.ePath2].sample30Data.appearCount;
                cell.Value = (info);
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            //string info = item.simData.path012CountInfoLong[ColumnSimulateSingleBuyLottery.S_INDEX][0] + " : " +
            //        item.simData.path012CountInfoLong[ColumnSimulateSingleBuyLottery.S_INDEX][1] + " : " +
            //        item.simData.path012CountInfoLong[ColumnSimulateSingleBuyLottery.S_INDEX][2];
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
            string info = sum.statisticUnitMap[CollectDataType.ePath0].sample30Data.appearCount + " ; " +
                            sum.statisticUnitMap[CollectDataType.ePath1].sample30Data.appearCount + " : " +
                            sum.statisticUnitMap[CollectDataType.ePath2].sample30Data.appearCount;
            colValues.Add(info);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 80;
        }
    }
    public class ColumnSinglePath012ProbabilityLong : ColumnBase
    {
        public ColumnSinglePath012ProbabilityLong()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "012路实际比率（全期）"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                //string info = item.simData.path012ProbabilityLong[ColumnSimulateSingleBuyLottery.S_INDEX][0] + "% : " +
                //     item.simData.path012ProbabilityLong[ColumnSimulateSingleBuyLottery.S_INDEX][1] + "% : " +
                //    item.simData.path012ProbabilityLong[ColumnSimulateSingleBuyLottery.S_INDEX][2] + "%";
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
                string info = sum.statisticUnitMap[CollectDataType.ePath0].sample30Data.appearProbability + "% ; " +
                                sum.statisticUnitMap[CollectDataType.ePath1].sample30Data.appearProbability + "% : " +
                                sum.statisticUnitMap[CollectDataType.ePath2].sample30Data.appearProbability + "%";
                cell.Value = (info);
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            //string info = item.simData.path012ProbabilityLong[ColumnSimulateSingleBuyLottery.S_INDEX][0] + "% : " +
            //        item.simData.path012ProbabilityLong[ColumnSimulateSingleBuyLottery.S_INDEX][1] + "% : " +
            //        item.simData.path012ProbabilityLong[ColumnSimulateSingleBuyLottery.S_INDEX][2] + "%";
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
            string info = sum.statisticUnitMap[CollectDataType.ePath0].sample30Data.appearProbability + "% ; " +
                            sum.statisticUnitMap[CollectDataType.ePath1].sample30Data.appearProbability + "% : " +
                            sum.statisticUnitMap[CollectDataType.ePath2].sample30Data.appearProbability + "%";
            colValues.Add(info);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 80;
        }
    }
    public class ColumnSinglePath012CountShort : ColumnBase
    {
        public ColumnSinglePath012CountShort()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "012次数统计短期"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                //string info = item.simData.path012CountInfoShort[ColumnSimulateSingleBuyLottery.S_INDEX][0] + " : " +
                //    item.simData.path012CountInfoShort[ColumnSimulateSingleBuyLottery.S_INDEX][1] + " : " +
                //    item.simData.path012CountInfoShort[ColumnSimulateSingleBuyLottery.S_INDEX][2];
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
                string info = sum.statisticUnitMap[CollectDataType.ePath0].sample10Data.appearCount + " ; " +
                                sum.statisticUnitMap[CollectDataType.ePath1].sample10Data.appearCount + " : " +
                                sum.statisticUnitMap[CollectDataType.ePath2].sample10Data.appearCount;
                cell.Value = (info);
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            //string info = item.simData.path012CountInfoShort[ColumnSimulateSingleBuyLottery.S_INDEX][0] + " : " +
            //        item.simData.path012CountInfoShort[ColumnSimulateSingleBuyLottery.S_INDEX][1] + " : " +
            //        item.simData.path012CountInfoShort[ColumnSimulateSingleBuyLottery.S_INDEX][2];
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
            string info = sum.statisticUnitMap[CollectDataType.ePath0].sample10Data.appearCount + " ; " +
                            sum.statisticUnitMap[CollectDataType.ePath1].sample10Data.appearCount + " : " +
                            sum.statisticUnitMap[CollectDataType.ePath2].sample10Data.appearCount;
            colValues.Add(info);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 80;
        }
    }
    public class ColumnSinglePath012ProbabilityShort : ColumnBase
    {
        public ColumnSinglePath012ProbabilityShort()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "012路实际比率（短期）"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                //string info = item.simData.path012ProbabilityShort[ColumnSimulateSingleBuyLottery.S_INDEX][0] + "% : " +
                //    item.simData.path012ProbabilityShort[ColumnSimulateSingleBuyLottery.S_INDEX][1] + "% : " +
                //    item.simData.path012ProbabilityShort[ColumnSimulateSingleBuyLottery.S_INDEX][2] + "%";
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
                string info = sum.statisticUnitMap[CollectDataType.ePath0].sample10Data.appearProbability + "% ; " +
                                sum.statisticUnitMap[CollectDataType.ePath1].sample10Data.appearProbability + "% : " +
                                sum.statisticUnitMap[CollectDataType.ePath2].sample10Data.appearProbability + "%";
                cell.Value = (info);
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            //string info = item.simData.path012ProbabilityShort[ColumnSimulateSingleBuyLottery.S_INDEX][0] + "% : " +
            //        item.simData.path012ProbabilityShort[ColumnSimulateSingleBuyLottery.S_INDEX][1] + "% : " +
            //        item.simData.path012ProbabilityShort[ColumnSimulateSingleBuyLottery.S_INDEX][2] + "%";
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
            string info = sum.statisticUnitMap[CollectDataType.ePath0].sample10Data.appearProbability + "% ; " +
                            sum.statisticUnitMap[CollectDataType.ePath1].sample10Data.appearProbability + "% : " +
                            sum.statisticUnitMap[CollectDataType.ePath2].sample10Data.appearProbability + "%";
            colValues.Add(info);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 80;
        }
    }
    public class ColumnSinglePath012ProbabilityShortNormalize : ColumnBase
    {
        public ColumnSinglePath012ProbabilityShortNormalize()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "012实际与理论比率差（短期）"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                //string info = item.simData.path012ProbabilityShortDiffWithTheory[ColumnSimulateSingleBuyLottery.S_INDEX][0] + "% : " +
                //    item.simData.path012ProbabilityShortDiffWithTheory[ColumnSimulateSingleBuyLottery.S_INDEX][1] + "% : " +
                //    item.simData.path012ProbabilityShortDiffWithTheory[ColumnSimulateSingleBuyLottery.S_INDEX][2] + "%";
                StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
                string info = sum.statisticUnitMap[CollectDataType.ePath0].sample10Data.appearProbabilityDiffWithTheory + "% ; " +
                                sum.statisticUnitMap[CollectDataType.ePath1].sample10Data.appearProbabilityDiffWithTheory + "% : " +
                                sum.statisticUnitMap[CollectDataType.ePath2].sample10Data.appearProbabilityDiffWithTheory + "%";
                cell.Value = (info);
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            //string info = item.simData.path012ProbabilityShortDiffWithTheory[ColumnSimulateSingleBuyLottery.S_INDEX][0] + "% : " +
            //        item.simData.path012ProbabilityShortDiffWithTheory[ColumnSimulateSingleBuyLottery.S_INDEX][1] + "% : " +
            //        item.simData.path012ProbabilityShortDiffWithTheory[ColumnSimulateSingleBuyLottery.S_INDEX][2] + "%";
            StatisticUnitMap sum = item.statisticInfo.allStatisticInfo[ColumnSimulateSingleBuyLottery.S_INDEX];
            string info = sum.statisticUnitMap[CollectDataType.ePath0].sample10Data.appearProbabilityDiffWithTheory + "% ; " +
                            sum.statisticUnitMap[CollectDataType.ePath1].sample10Data.appearProbabilityDiffWithTheory + "% : " +
                            sum.statisticUnitMap[CollectDataType.ePath2].sample10Data.appearProbabilityDiffWithTheory + "%";
            colValues.Add(info);
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            col.Width = 80;
        }
    }


#endregion

#region simulate step info

#if ENABLE_GROUP_COLLECT
    public class ColumnKillResultTrue : ColumnBase
    {
        public ColumnKillResultTrue()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "对"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = item.simData.predictResult == TestResultType.eTRTSuccess ? "对" : "";
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add("");
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.Red;
            //col.Width = 60;
        }
    }
    public class ColumnKillResultFalse : ColumnBase
    {
        public ColumnKillResultFalse()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "错"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = item.simData.predictResult == TestResultType.eTRTFailed ? "错" : "";
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add("");
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            col.DefaultCellStyle.ForeColor = System.Drawing.Color.Blue ;
            //col.Width = 60;
        }
    }
    public class ColumnSimCost : ColumnBase
    {
        public ColumnSimCost()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "成本"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = -item.simData.cost;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add("");
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            //col.Width = 60;
        }
    }
    public class ColumnSimReward : ColumnBase
    {
        public ColumnSimReward()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "奖金"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = item.simData.reward;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add("");
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            //col.Width = 60;
        }
    }
    public class ColumnSimProfit : ColumnBase
    {
        public ColumnSimProfit()
        {
            forceActive = false;
        }
        public override string GetColumnName() { return "获利"; }
        public override void SetColumnText(DataItem item, DataGridViewRow row)
        {
            if (columnID >= 0)
            {
                DataGridViewCell cell = row.Cells[columnID];
                cell.Value = item.simData.profit;
            }
        }
        public override void OnAddRow(DataItem item, List<object> colValues)
        {
            colValues.Add("");
        }
        public override void SetColumnIndex(ref int startIndex, DataGridView view)
        {
            base.SetColumnIndex(ref startIndex, view);
            DataGridViewColumn col = view.Columns[columnID];
            //col.Width = 60;
        }
    }
#endif

#endregion

#if ENABLE_GROUP_COLLECT
    public class ColumnSimulateGroup3BuyLottery : ColumnSet
    {
        public ColumnSimulateGroup3BuyLottery()
        {
            forceActive = false;
            active = false;
            subColumns.Add(new ColumnGroupType());
            subColumns.Add(new ColumnAndValue());
            subColumns.Add(new ColumnAndRearValue());
            subColumns.Add(new ColumnGroupScore());

            subColumns.Add(new ColumnGroup3KillNumber());
            subColumns.Add(new ColumnKillResultTrue());
            subColumns.Add(new ColumnKillResultFalse());
            subColumns.Add(new ColumnSimCost());
            subColumns.Add(new ColumnSimReward());
            subColumns.Add(new ColumnSimProfit());
        }
        public override string GetColumnName() { return "模拟组三方案"; }
    }


    public class ColumnSimulateGroup2BuyLottery : ColumnSet
    {
        public ColumnSimulateGroup2BuyLottery()
        {
            forceActive = false;
            active = false;
            subColumns.Add(new ColumnGroupTen());
            subColumns.Add(new ColumnGroupGe());

            subColumns.Add(new ColumnKillResultTrue());
            subColumns.Add(new ColumnKillResultFalse());
            subColumns.Add(new ColumnSimCost());
            subColumns.Add(new ColumnSimReward());
            subColumns.Add(new ColumnSimProfit());
        }
        public override string GetColumnName() { return "模拟组二方案"; }
    }
#endif

    public class ColumnSimulateSingleBuyLottery : ColumnSet
    {
        public static int S_INDEX = 0;
        public static int S_SHORT_COUNT = 30;

        public static int GetNum(DataItem data)
        {
            switch(S_INDEX)
            {
                case 0: return data.GetWanNumber();
                case 1: return data.GetQianNumber();
                case 2: return data.GetBaiNumber();
                case 3: return data.GetShiNumber();
                case 4: return data.GetGeNumber();
            }
            Console.WriteLine("not find number at (" + data.lotteryNumber +") [" + S_INDEX + "]");
            return -1;
        }

        public ColumnSimulateSingleBuyLottery()
        {
            forceActive = false;
            active = false;
            subColumns.Add(new ColumnSinglePath0());
            subColumns.Add(new ColumnSinglePath1());
            subColumns.Add(new ColumnSinglePath2());
            subColumns.Add(new ColumnSinglePath012Missing());
            subColumns.Add(new ColumnSinglePath012CountLong());
            subColumns.Add(new ColumnSinglePath012ProbabilityLong());
            subColumns.Add(new ColumnSinglePath012CountShort());
            subColumns.Add(new ColumnSinglePath012ProbabilityShort());
            subColumns.Add(new ColumnSinglePath012ProbabilityShortNormalize());
#if ENABLE_GROUP_COLLECT
            subColumns.Add(new ColumnKillResultTrue());
            subColumns.Add(new ColumnKillResultFalse());
            subColumns.Add(new ColumnSimCost());
            subColumns.Add(new ColumnSimReward());
            subColumns.Add(new ColumnSimProfit());
#endif
        }
        public override string GetColumnName() { return "模拟单选方案"; }
    }

}
