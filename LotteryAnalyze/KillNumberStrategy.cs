using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryAnalyze
{
    public interface KillNumberStrategy
    {
        void KillNumber(DataItem item, ref List<int> killList);
    }

    public class KillNumberByDateValue : KillNumberStrategy
    {
        public void KillNumber(DataItem item, ref List<int> killList)
        {
            int dateRearValue = Util.GetNumberByPos(item.id, 0);
            if (killList.Contains(dateRearValue) == false)
                killList.Add(dateRearValue);
        }
    }

    public class KillNumberByAndValue : KillNumberStrategy
    {
        public void KillNumber(DataItem item, ref List<int> killList)
        {

        }
    }

    public class KillNumberByRearValue : KillNumberStrategy
    {
        public void KillNumber(DataItem item, ref List<int> killList)
        {
            if (killList.Contains(item.rearValue) == false)
                killList.Add(item.rearValue);
        }
    }

    public class KillNumberByCrossValue : KillNumberStrategy
    {
        public void KillNumber(DataItem item, ref List<int> killList)
        {

        }
    }

    public class KillNumberByReverseSelect : KillNumberStrategy
    {
        public void KillNumber(DataItem item, ref List<int> killList)
        {
            int ge = item.GetGeNumber();
            int shi = item.GetShiNumber();
            int bai = item.GetBaiNumber();
            if (killList.Contains(ge) == false)
                killList.Add(ge);
            if (killList.Contains(shi) == false)
                killList.Add(shi);
            if (killList.Contains(bai) == false)
                killList.Add(bai);
        }
    }
}
