using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.VisualBasic;

namespace ConsoleApp1
{
    public class objSP
    {
        public string name { get; set; }
        public string name1 { get; set; }
        public string age { get; set; }
        public string age1 { get; set; }
        public objSP()
        {
            name = null;
            name1 = null;
            age = null;
            age1 = null;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var kkkkkkk = "1234" + 0;
            kkkkkkk = "1234" + 1;

            DateTime dt = DateTime.MinValue;
            string kk2k = dt.ToString();
            string thang = "tarou";
            string t1 = Convert2Zenkaku(thang);
            string t2 = toKatakana(thang);

            List<string> abcba = null;
            string kkkaaavvv = abcba.FirstOrDefault();
            string aaa = "一丁目ー１２－２３";
            string number = convert(aaa);
            string kkk = "";
            string aaab = "012";
            var newString = aaab.PadLeft(5, '0');
            string bbbbbb = "01234";
            newString = bbbbbb.PadLeft(5, '0');
            string cccc = "1204";
            newString = cccc.PadLeft(5, '0');
            return;
        }

        static string convert(string houseNumber)
        {
            string oneByteNumber = "0123456789";
            string twoByteNumber = "０１２３４５６７８９";
            char[] charList = houseNumber.ToCharArray();
            string retNum = string.Empty;
            int i = 0;
            foreach (char val in charList)
            {
                //１バイト数字
                if (oneByteNumber.Contains(val)) retNum += val.ToString();

                //２バイト数字
                else if (twoByteNumber.Contains(val))
                {
                    retNum += oneByteNumber.Substring(twoByteNumber.IndexOf(val), 1);
                }
                //次は文字
                //変換必要
                else
                {
                    break;
                }
                i++;
            }
            return retNum;
        }

        static string Convert2Zenkaku(string value)
        {
            if (!string.IsNullOrEmpty(value))
                return Microsoft.VisualBasic.Strings.StrConv(value, Microsoft.VisualBasic.VbStrConv.Wide, 0);
            return null;
        }

        static string toKatakana(string value)
        {
            return Microsoft.VisualBasic.Strings.StrConv(value, Microsoft.VisualBasic.VbStrConv.Katakana, 0);
        }

        static int? tryParse(string val)
        {
            try
            {
                return int.Parse(val);
            }
            catch
            {
                return null;
            }
        }

        static void SetData(ref object ObjDes)
        {
            PropertyInfo prop2 = ObjDes.GetType().GetProperty("a1", BindingFlags.Public | BindingFlags.Instance);
            if (null != prop2 && prop2.CanWrite)
            {
                prop2.SetValue(ObjDes, 12, null);
            }
        }
    }
}
