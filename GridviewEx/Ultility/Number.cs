using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Data;

namespace coms.COMMON.Utility
{
    public static class Number
    {
        public static string COMMA = ",";
        public static string SPACE = " ";
        public static Char SEPARATOR = ',';
        public static string DOT = ".";
        public static double TSUBO = 0.3025;

        public enum RoundType
        {
            Normal,
            Up,
            Down
        }


        public static double Round(double d, int decimals)
        {
            return Math.Round(d, decimals);
        }

        public static decimal Round(decimal d, int decimals)
        {
            return (decimal)Math.Round((double)d, decimals);
        }

        public static double RoundUp(double d, int decimals)
        {
            double degree = Math.Pow(10, decimals - 1);
            return Math.Ceiling((d * 10) * degree) / (10 * degree);
        }

        public static decimal RoundUp(decimal d, int decimals)
        {
            double degree = Math.Pow(10, decimals - 1);
            double ret = Math.Ceiling(((double)d * 10) * degree) / (10 * degree);
            return (decimal)ret;
        }

        public static double RoundDown(double d, int decimals)
        {
            string valStr = d.ToString();
            valStr += (valStr.IndexOf(".") < 0 ? "." : "") + new String('0', decimals);
            return double.Parse(valStr.Substring(0, valStr.IndexOf(".") + decimals + 1));
        }

        public static decimal RoundDown(decimal d, int decimals)
        {
            decimal val = (decimal)RoundDown((double)d, decimals);
            return val;
        }

        public static string FormatNumber(double? d)
        {
            return FormatNumber(d, 2, RoundType.Normal);
        }

        public static string FormatNumberRemoveLastZero<T>(T number, int maxDecimals)
        {
            if (number == null) return "";

            return Regex.Replace(String.Format("{0:n" + maxDecimals + "}", number),
                                 @"[" + System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "]?0+$", "");
        }

        public static string FormatNumber(double? d, int decimals, RoundType type)
        {
            if (d == null)
            {
                return "";
            }

            double val = d.Value;

            string fmdS = (decimals > 0 && d.ToString().IndexOf(".") > 0 ? ("." + new String('#', decimals - 1) + "0") : "");

            if (type == RoundType.Up)
            {
                return String.Format("{0:#,##0" + fmdS + "}", RoundUp(val, decimals));
            }
            else if (type == RoundType.Down)
            {
                return String.Format("{0:#,##0" + fmdS + "}", RoundDown(val, decimals));
            }
            else
            {
                return String.Format("{0:#,##0" + fmdS + "}", Round(val, decimals));
            }
        }

        public static List<long> ConvertToListOfLong(List<string> listOfStrings)
        {
            return listOfStrings.Select<string, long>(q => Convert.ToInt64(q)).ToList();
        }

        public static string FormatNumber(long? value)
        {
            return (value == null) ? string.Empty : String.Format("{0:#,##0}", value.Value);
        }

        public static string FormatNumber(int? value)
        {
            return (value == null) ? string.Empty : String.Format("{0:#,##0}", value.Value);
        }

        public static string FormatNumber(decimal? value)
        {
            string valStr = null;
            if (value.HasValue)
            {
                //小数点以下が0の場合,表示しない。
                if (value.Value - Math.Floor(value.Value) == 0)
                {
                    valStr = String.Format("{0:#,##0}", value.Value);
                }
                else
                {
                    valStr = String.Format("{0:#,##0.#0}", value.Value);
                }
            }

            return valStr;
        }
        public static string FormatNumberTriming(decimal? value)
        {
            return (value == null) ? string.Empty : String.Format("{0:#,##0}", value.Value);
        }

        /// <summary>
        /// 数字のテキストからshortを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short? ParseToShort(object val)
        {
            try
            {
                string value = "";
                if (val != null)
                {
                    value = val.ToString();
                }

                value = value.Replace(",", "").Trim();
                return short.Parse(value);

            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 数字のテキストからintを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? ParseToInt(object val)
        {
            try
            {
                string value = "";
                if (val != null)
                {
                    value = val.ToString();
                }

                value = value.Replace(",", "").Trim();

                return int.Parse(value);
            }
            catch
            {
                return null;
            }
        }

     
        /// <summary>
        /// 数字のテキストからlongを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long? ParseToLong(object val)
        {
            try
            {
                string value = "";
                if (val != null)
                {
                    value = val.ToString();
                }

                value = value.Replace(",", "").Trim();
                return long.Parse(value);

            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 数字のテキストからdoubleを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double? ParseToDouble(object val)
        {
            try
            {
                string value = "";
                if (val != null)
                {
                    value = val.ToString();
                }
                value = value.Replace(",", "").Trim();
                return double.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 数字のテキストからdecimalを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal? ParseToDecimal(object val)
        {
            try
            {
                string value = "";
                if (val != null)
                {
                    value = val.ToString();
                }
                value = value.Replace(",", "").Trim();
                return decimal.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 数字のテキストからfloatを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float? ParseToFloat(object val)
        {
            try
            {
                string value = "";
                if (val != null)
                {
                    value = val.ToString();
                }
                value = value.Replace(",", "").Trim();
                return float.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        public static double Sum(double? val1, double? val2)
        {
            double val = 0;
            if (val1 == null && val2 == null)
            {
                return double.MinValue;
            }
            else if (val1 != null && val2 == null)
            {
                val = val1.Value;
            }
            else if (val1 == null && val2 != null)
            {
                val = val2.Value;
            }
            else if (val1 != null && val2 != null)
            {
                val = val1.Value + val2.Value;
            }
            return val;
        }

        public static decimal Sum(decimal? val1, decimal? val2)
        {
            decimal val = 0;
            if (val1 == null && val2 == null)
            {
                return decimal.MinValue;
            }
            else if (val1 != null && val2 == null)
            {
                val = val1.Value;
            }
            else if (val1 == null && val2 != null)
            {
                val = val2.Value;
            }
            else if (val1 != null && val2 != null)
            {
                val = val1.Value + val2.Value;
            }
            return val;
        }

        public static decimal? SumNull(decimal? val1, decimal? val2)
        {
            decimal? val = Sum(val1, val2);
            if (val == decimal.MinValue)
            {
                val = null;
            }
            return val;
        }

        public static long? Sum(long? val1, long? val2)
        {
            long? val = null;
            if (val1 == null && val2 == null)
            {
                return null;
            }
            else if (val1 != null && val2 == null)
            {
                val = val1.Value;
            }
            else if (val1 == null && val2 != null)
            {
                val = val2.Value;
            }
            else if (val1 != null && val2 != null)
            {
                val = val1.Value + val2.Value;
            }
            return val;
        }

        public static double? Sum(params double?[] list)
        {
            double? val = 0;
            foreach (double? item in list)
            {
                if (item.HasValue)
                { val += item.Value; }
            }
            return val;
        }

        public static double? SumNull(params double?[] list)
        {
            double? val = null;
            foreach (double? item in list)
            {
                if (item.HasValue)
                {
                    if (!val.HasValue)
                    {
                        val = 0;
                    }
                    val += item.Value;
                }
            }
            return val;
        }

        public static long? Sum(params long?[] list)
        {
            long? val = null;
            if (list != null)
            {
                foreach (long? item in list)
                {
                    if (item.HasValue)
                    {
                        if (!val.HasValue)
                        {
                            val = 0;
                        }
                        val += item.Value;
                    }
                }
            }
            return val;
        }

        public static int? Sum(params int?[] list)
        {
            int? val = null;
            if (list != null)
            {
                foreach (int? item in list)
                {
                    if (item.HasValue)
                    {
                        if (!val.HasValue)
                        {
                            val = 0;
                        }
                        val += item.Value;
                    }
                }
            }
            return val;
        }

        public static double Minus(double val1, double val2)
        {
            double val = double.MinValue;
            if (val1 != double.MinValue)
            {
                val = val1;
            }
            if (val == double.MinValue)
            {
                if (val2 != double.MinValue)
                {
                    val = -val2;
                }
            }
            else
            {
                if (val2 != double.MinValue)
                {
                    val -= val2;
                }
            }

            return val;
        }

        public static decimal Minus(decimal? val1, decimal? val2)
        {
            decimal val = decimal.MinValue;
            if (val1.HasValue && val1 != decimal.MinValue)
            {
                val = val1.Value;
            }
            if (val == decimal.MinValue)
            {
                if (val2.HasValue && val2 != decimal.MinValue)
                {
                    val = -val2.Value;
                }
            }
            else
            {
                if (val2.HasValue && val2 != decimal.MinValue)
                {
                    val -= val2.Value;
                }
            }

            return val;
        }

        public static double? Minus(double? val1, double? val2)
        {
            double result = double.MinValue;
            if (val1 == null && val2 == null)
            {
                result = Minus(double.MinValue, double.MinValue);
            }
            else if (val1 != null && val2 == null)
            {
                result = Minus(val1.Value, double.MinValue);
            }
            else if (val1 == null && val2 != null)
            {
                result = Minus(double.MinValue, val2.Value);
            }
            else if (val1 != null && val2 != null)
            {
                result = Minus(val1.Value, val2.Value);
            }
            if (result != double.MinValue)
            {
                return result;
            }
            return null;
        }

        public static long Minus(long val1, long val2)
        {
            long val = long.MinValue;
            if (val1 != long.MinValue)
            {
                val = val1;
            }
            if (val == long.MinValue)
            {
                if (val2 != long.MinValue)
                {
                    val = -val2;
                }
            }
            else
            {
                if (val2 != long.MinValue)
                {
                    val -= val2;
                }
            }

            return val;
        }

        public static long? Minus(long? val1, long? val2)
        {
            long result = long.MinValue;
            if (val1 == null && val2 == null)
            {
                result = Minus(long.MinValue, long.MinValue);
            }
            else if (val1 != null && val2 == null)
            {
                result = Minus(val1.Value, long.MinValue);
            }
            else if (val1 == null && val2 != null)
            {
                result = Minus(long.MinValue, val2.Value);
            }
            else if (val1 != null && val2 != null)
            {
                result = Minus(val1.Value, val2.Value);
            }
            if (result != long.MinValue)
            {
                return result;
            }
            return null;
        }

        public static int Minus(int val1, int val2)
        {
            int val = int.MinValue;
            if (val1 != int.MinValue)
            {
                val = val1;
            }
            if (val == int.MinValue)
            {
                if (val2 != int.MinValue)
                {
                    val = -val2;
                }
            }
            else
            {
                if (val2 != int.MinValue)
                {
                    val -= val2;
                }
            }

            return val;
        }

        public static int? Minus(int? val1, int? val2)
        {
            int result = int.MinValue;
            if (val1 == null && val2 == null)
            {
                result = Minus(int.MinValue, int.MinValue);
            }
            else if (val1 != null && val2 == null)
            {
                result = Minus(val1.Value, int.MinValue);
            }
            else if (val1 == null && val2 != null)
            {
                result = Minus(int.MinValue, val2.Value);
            }
            else if (val1 != null && val2 != null)
            {
                result = Minus(val1.Value, val2.Value);
            }
            if (result != int.MinValue)
            {
                return result;
            }
            return null;
        }

        public static double Sum(string val1Str, string val2Str)
        {
            double? val1 = ParseToDouble(val1Str);
            double? val2 = ParseToDouble(val2Str);
            return Sum(val1, val2);
        }

        public static string FormatFloatNumber(decimal? d)
        {
            if (d == null)
                return "";

            double val = (double)d.Value;

            return FormatNumberAutoSeparate(val.ToString());
        }

        public static string FormatFloatNumber(double? d)
        {
            if (d == null)
                return "";

            double val = d.Value;

            return FormatNumberAutoSeparate(val.ToString());
        }

        public static string FormatNumberAutoSeparate(int? number)
        {
            if (number.HasValue == false || number == int.MinValue)
            {
                return "";
            }
            else
            {
                return FormatNumberAutoSeparate(number.Value.ToString());
            }
        }

        public static string FormatNumberAutoSeparate(string number)
        {
            char[] chTrims = { '-' };
            string Fugou = "";
            if (!string.IsNullOrEmpty(number))
            {
                if (number.Substring(0, 1) == "-")
                {
                    Fugou = "-";

                    number = number.TrimStart(chTrims);
                }
            }
            else
            {
                return number;
            }

            string num = number.Replace(COMMA, string.Empty);
            string newNum = string.Empty;
            string newNum2 = string.Empty;
            int count = 0;
            string end = string.Empty;

            //check for decimal number
            if (num.IndexOf(DOT) != -1)
            {
                if (num.IndexOf(DOT) == num.Length - 1)
                {
                    num += "";
                }

                if (num.IndexOf(DOT) == num.Length - 2)
                {
                    num += "";
                }

                string[] a = num.Split('.');
                num = a[0];

                if (!string.IsNullOrEmpty(a[1]))
                {
                    end = a[1];
                }
            }
            else
            {
                end = "";
            }

            for (int k = num.Length - 1; k >= 0; k--)
            {
                char oneChar = num[k];
                if (count == 3)
                {
                    newNum += COMMA;
                    newNum += oneChar;
                    count = 1;
                    continue;
                }
                else
                {
                    newNum += oneChar;
                    count++;
                }
            }

            for (int k = newNum.Length - 1; k >= 0; k--)
            {
                char oneChar = newNum[k];
                newNum2 += oneChar;
            }

            if (end.Length > 0)
            {
                newNum2 = newNum2 + DOT + end;
            }

            return Fugou + newNum2;
        }

        public static string RemoveDigits(string name)
        {
            return Regex.Replace(name, @"\d", "");
        }

        public static string RemoveSpecialCharacters(string input)
        {

            return Regex.Replace(input, @"[^\d\w\s]", string.Empty);
        }

        public static string GetLimitBuidingName(string inputString, int maxlength)
        {
            if (string.IsNullOrEmpty(inputString) || inputString.Length <= maxlength) return inputString;
            return inputString.Substring(0, maxlength);
        }

        public static string FormatNumberReadOnly(string value)
        {

            if (!String.IsNullOrEmpty(value) && value.Contains("."))
                return value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(value));
            return value = String.Format("{0:#,##0}", Convert.ToDecimal(value));
        }

        public static bool IsNumber(string str)
        {
            return str.All(Char.IsNumber);
        }

        public static bool IsParseableAs(string value, Type type)
        {
            var tryParseMethod = type.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder,
                new[] { typeof(string), type.MakeByRefType() }, null);

            if (tryParseMethod == null) return false;

            var arguments = new[] { value, Activator.CreateInstance(type) };
            return (bool)tryParseMethod.Invoke(null, arguments);
        }

       
        public static string FormatToTsubo(double? d)
        {
            if (d == null)
            {
                return "";
            }

            double val = RoundDown((double)d * TSUBO, 2);

            return FormatNumberAutoSeparate(val.ToString());
        }

       
        //自然数でチェック
        public static bool IsNaturalNumber(String strNumber)
        {
            Regex objNotNaturalPattern = new Regex("[^0-9]");
            Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");

            return !objNotNaturalPattern.IsMatch(strNumber) && objNaturalPattern.IsMatch(strNumber);
        }

        //値が数値かチェックする
        public static bool IsNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            Double result;
            return Double.TryParse(val, NumberStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        /// Check to know if this is an integer or not
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsInteger(string val)
        {
            try
            {
                long.Parse(val);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check to know if this is an integer or not
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsDouble(string val)
        {
            try
            {
                double.Parse(val);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 数字のテキストからbyteを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte? ParseToByte(object val)
        {
            try
            {
                string value = "";
                if (val != null)
                {
                    value = val.ToString();
                }
                value = value.Replace(",", "").Trim();
                return byte.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        public static short? SumShort(short? val1, int? val2)
        {
            short? result = null;
            if(val1.HasValue && val2.HasValue)
            {
                try
                {
                    result = (short?) (val1.Value + val2.Value);
                }
                catch { }
            }          
           
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static double EvaluateFromMathExpression(string expression)
        {
            var loDataTable = new DataTable();
            var loDataColumn = new DataColumn("Eval", typeof(double), expression);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (double)(loDataTable.Rows[0]["Eval"]);
        }

        public static bool IsNumeric(object value, Type underlyingType, ref bool isMinNumber)
        {
            isMinNumber = false;

            if (underlyingType == null)
                return false;

            // Determine numeric types supported by the grid
            bool isNumeric =
                underlyingType == typeof(int) ||
                underlyingType == typeof(long) ||
                underlyingType == typeof(float) ||
                underlyingType == typeof(double) ||
                underlyingType == typeof(decimal);

            if (!isNumeric || value == null)
                return isNumeric;

            try
            {
                // Check MinValue per numeric type
                if (underlyingType == typeof(int))
                    isMinNumber = (int)value == int.MinValue;

                else if (underlyingType == typeof(long))
                    isMinNumber = (long)value == long.MinValue;

                else if (underlyingType == typeof(float))
                    isMinNumber = (float)value == float.MinValue;

                else if (underlyingType == typeof(double))
                    isMinNumber = (double)value == double.MinValue;

                else if (underlyingType == typeof(decimal))
                    isMinNumber = (decimal)value == decimal.MinValue;
            }
            catch
            {
                // Ignore cast issues — treat as not MinValue
                isMinNumber = false;
            }

            return isNumeric;
        }
    }
}
