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
    }

    public class ColumnAndValue : ColumnBase
    {
        public ColumnAndValue()
        {
            forceActive = true;
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
    }

    public class ColumnAndRearValue : ColumnBase
    {
        public ColumnAndRearValue()
        {
            forceActive = true;
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
    }

    public class DataGridViewColumnManager
    {
        static int sActiveColumnCount = 0;
        static List<ColumnBase> sColumns = new List<ColumnBase>();
        static DataGridViewColumnManager()
        {
            sColumns.Add(new ColumnGlobalID());
            sColumns.Add(new ColumnIDTag());
            sColumns.Add(new ColumnNumber());
            sColumns.Add(new ColumnAndValue());
            sColumns.Add(new ColumnAndRearValue());
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
            view.Rows.Add(parms.ToArray());
        }
    }
}
