using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Windows.Forms;

namespace coms.COMMON.Utility
{
    public static class TextUtility
    {
        private static Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
        public static string COMMA = ",";
        public static string SPACE = " ";
        public static Char SEPARATOR = ',';



        /// <summary>
        /// アルファベットでチェックします。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAlphabet(string value)
        {
            Regex objAlphaPattern = new Regex("[^a-zA-Z]");
            return !objAlphaPattern.IsMatch(value);
        }

        /// <summary>
        /// アルファベットでチェックします。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPassword(string value)
        {
            Regex objAlphaNumericPattern = new Regex("^[A-Za-z0-9.@#$%&]+$");
            return objAlphaNumericPattern.IsMatch(value);
        }

        private const string MatchEmailPattern =    @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                                    @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                                    @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        public static bool IsValidEmailAddress(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return Regex.IsMatch(value, MatchEmailPattern);
            }
            else
            {
                return false;
            }
        }

        public static bool IsZenkaku(string value)
        {
            int num = sjisEnc.GetByteCount(value);
            return num == value.Length * 2;
        }

        public static bool IsHankaku(string value)
        {
            int num = sjisEnc.GetByteCount(value);
            return num == value.Length;
        }

        public static string LeftStr(string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            if (string.IsNullOrEmpty(param))
            {
                return "";
            }
            string result = param;
            if (result.Length > length)
            {
                result = param.Substring(0, length);
            }

            //return the result of the operation
            return result;
        }
        public static string RightStr(string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            if (string.IsNullOrEmpty(param))
            {
                return "";
            }
            int nStart = param.Length - length;
            string result = param.Substring(nStart, length);

            //return the result of the operation
            return result;
        }

        public static string MidStr(string param, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            if (string.IsNullOrEmpty(param))
            {
                return "";
            }
            string result = param.Substring(startIndex, length);

            //return the result of the operation
            return result;
        }

        internal static string MidStr(string param, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            if (string.IsNullOrEmpty(param))
            {
                return "";
            }
            string result = param.Substring(startIndex);

            //return the result of the operation
            return result;
        }
        public static string StrDup(string szDupString, int nCount)
        {
            StringBuilder objBuilder = new StringBuilder();
            for (int nCounter = 0; nCounter < nCount; nCounter++)
            {
                objBuilder.Append(szDupString);
            }
            return objBuilder.ToString();
        }

        public static string StripText(string szText, string szStripString)
        {
            if (szText == "")
            {
                return szText;
            }
            string szReturn = szText;
            foreach (char cChar in szStripString)
            {
                szReturn = szReturn.Replace(cChar.ToString(), "");
            }
            return szReturn;
        }

        public static string ConvertToHankaku(string strSrc)
        {
            if (!string.IsNullOrEmpty(strSrc))
            {
                return Strings.StrConv(strSrc, VbStrConv.Narrow, 0);
            }
            //else return string.Empty;
            else return strSrc;
        }

        public static string ConvertToZenkaku(string strSrc)
        {
            if (!string.IsNullOrEmpty(strSrc))
            {
                return Strings.StrConv(strSrc, VbStrConv.Wide, 0);
            }
            else return strSrc;
        }

        public static string FormatText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            string result = "";

            string val = text.Trim().Replace(COMMA, String.Empty).Replace(SPACE, string.Empty);
            if (Number.IsInteger(val) || Number.IsDouble(val))
            {
                while (((val[0].Equals('0') && double.Parse(val) >= 1) || val[0].Equals(SEPARATOR)) && val.Length > 1)
                {
                    string temp = val.Substring(1);
                    val = temp;
                }
                result = Number.FormatNumberAutoSeparate(val);
            }

            val = text;
            if (val.Length > 1)
            {
                if (val[1].Equals(SEPARATOR) && val[0].Equals('-'))
                {
                    result = val.Substring(0, 1) + val.Substring(2);
                }
                else
                {
                    double? valData = Number.ParseToDouble(val);
                    if (valData.HasValue && valData.Value == 0)
                    {
                        result = "0";
                    }
                }
            }
            return result;
        }

        public static string ParseToStringWithNull(object value)
        {
            string result = null;

            if (value != null)
            {
                result = value.ToString();
            }
            
            return result != "" ? result : null;
        }

        public static string FormatPeriodVew(int? financialYear, DateTime? startDate, DateTime? endDate)
        {
            string year = (financialYear.HasValue) ? financialYear.ToString() : string.Empty;
            string from = (startDate.HasValue) ? startDate.Value.ToString("yyyyMM") : string.Empty;
            string to = (endDate.HasValue) ? endDate.Value.ToString("yyyyMM") : string.Empty;
            return string.Format("{0}年（{1} ～ {2}）", year, from, to);
        }
        public static string SplitFileName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                return fileName.Substring(fileName.LastIndexOf("\\") + 1);
            }
            return fileName;
        }

        public static bool IsChangeText(string oldValue, string newValue)
        {
            oldValue = (oldValue == null) ? string.Empty : oldValue.ToString();
            newValue = (newValue == null) ? string.Empty : newValue.ToString();
            return (string.Compare(oldValue, newValue) != 0);
        }

        public static string CsvEscape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            bool mustQuote =
                value.Contains(",") ||
                value.Contains("\"") ||
                value.Contains("\r") ||
                value.Contains("\n");

            if (value.Contains("\""))
                value = value.Replace("\"", "\"\"");

            return mustQuote ? $"\"{value}\"" : value;
        }

    }
}
