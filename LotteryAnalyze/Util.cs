using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LotteryAnalyze
{
    public class Util
    {
        public static bool ReadFile(int fileID, string filePath, ref OneDayDatas datas)
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
                    item.idTag = fileID.ToString() + "-" + strs[0];
                    item.andValue = Util.CalAndValue(item.lotteryNumber);
                    item.rearValue = Util.CalRearValue(item.lotteryNumber);
                    item.crossValue = Util.CalCrossValue(item.lotteryNumber);
                    item.groupType = Util.GetGroupType(item.lotteryNumber);
                    datas.datas.Add(item);
                }
            }
            sr.Close();
            return true;
        }

        public static int CharValue(char ch)
        {
            int value = ch - '0';
            return value;
        }

        public static int CalAndValue(string str)
        {
            int curId = str.Length - 1;
            int ge = CharValue(str[curId]); curId--;
            int shi = CharValue(str[curId]); curId--;
            int bai = CharValue(str[curId]); curId--;
            return ge + shi + bai;
        }

        public static int CalRearValue(string str)
        {
            int andValue = CalAndValue(str);
            int rearValue = andValue % 10;
            return rearValue;
        }

        public static int CalCrossValue(string str)
        {
            int curId = str.Length - 1;
            int ge = CharValue(str[curId]); curId--;
            int shi = CharValue(str[curId]); curId--;
            int bai = CharValue(str[curId]); curId--;
            int abs1 = Math.Abs(ge - shi);
            int abs2 = Math.Abs(ge - bai);
            int abs3 = Math.Abs(shi - bai);
            return Math.Max( abs3, Math.Max(abs1, abs2) );
        }

        public static int GetGroupType(string str)
        {
            int curId = str.Length - 1;
            int ge = CharValue(str[curId]); curId--;
            int shi = CharValue(str[curId]); curId--;
            int bai = CharValue(str[curId]); curId--;
            if (ge == shi && ge == bai)
                return 1;
            if (ge == shi || ge == bai || shi == bai)
                return 2;
            return 3;
        }

        // 获取整数srcNumber第index（从右向左数, index >= 0）位上的数字
        public static int GetNumberByPos(int srcNumber, int index)
        {
            string str = srcNumber.ToString();
            if (index >= str.Length)
                return 0;
            int realIndex = str.Length - index - 1;
            char ch = str[realIndex];
            int chValue = CharValue(ch);
            return chValue;
        }
    }
}
