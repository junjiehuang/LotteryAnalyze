using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LotteryAnalyze
{
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

    public class ColumnKillNumber : ColumnBase
    {
        public ColumnKillNumber()
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
            //col.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
            //col.Width = 60;
        }
    }
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
    public class ColumnSimulateBuyLottery : ColumnSet
    {
        public ColumnSimulateBuyLottery()
        {
            forceActive = false;
            subColumns.Add(new ColumnKillNumber());
            subColumns.Add(new ColumnKillResultTrue());
            subColumns.Add(new ColumnKillResultFalse());
            subColumns.Add(new ColumnSimCost());
            subColumns.Add(new ColumnSimReward());
            subColumns.Add(new ColumnSimProfit());
        }
        public override string GetColumnName() { return "模拟杀号"; }
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
            sColumns.Add(new ColumnAndValue());
            sColumns.Add(new ColumnAndRearValue());
            sColumns.Add(new ColumnGroupType());
            sColumns.Add(new ColumnGroupScore());
            sColumns.Add(new ColumnSimulateBuyLottery());
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
}
