using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LotteryAnalyze
{
    public class Util
    {
        public static bool ReadFile(string filePath, ref OneDayDatas datas)
        {
            String line;
            StreamReader sr = null;
            datas = new OneDayDatas();

            try
            {
                sr = new StreamReader(filePath, Encoding.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                sr.Close();
                return false;
            }
            while ((line = sr.ReadLine()) != null)
            {
                string[] strs = line.Split( '\t' );
                if (strs.Length > 0)
                {
                    DataItem item = new DataItem();
                    item.id = int.Parse(strs[0]);
                    item.lotteryNumber = strs[1];
                    datas.datas.Add(item);
                }
            }
            sr.Close();
            return true;
        }
    }
}
