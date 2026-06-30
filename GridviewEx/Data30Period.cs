using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace coms.COMSK.common
{
	/// <summary>
	/// 30 年分のデータの表示を監理するクラス
	/// </summary>
	public class Data30Period
	{
		#region 列挙値

		/// <summary>
		/// 表示フォーマット
		/// </summary>
		public enum Format
		{
			/// <summary>
			/// 指定なし
			/// </summary>
			None,
			/// <summary>
			/// カンマ区切り
			/// </summary>
			Comma,
			/// <summary>
			/// \表記
			/// </summary>
			Yen,
		}

		#endregion

		#region メンバ

		private double[] values = new double[COMSKCommon.YEAR100];

		#endregion

		#region プロパティ

		public string Name { get; set; }
        public string ReservePlanDetailCode { get; set; }
		public Format DisplayFormat { get; set; }
		public System.Drawing.Color BackColor { get; set; }
		public bool Visible { get; set; }
		public double MaxValue(int showYear)
		{
			double[] showValues = new double[showYear];
			Array.Copy(values, showValues, showYear);
			return (from item in showValues
					select item).Max();
		}

		#region Val1～Val100 アクセサ

		public double Val1 { get { return GetValue(0); } set { SetValue(0, value); } }
		public double Val2 { get { return GetValue(1); } set { SetValue(1, value); } }
		public double Val3 { get { return GetValue(2); } set { SetValue(2, value); } }
		public double Val4 { get { return GetValue(3); } set { SetValue(3, value); } }
		public double Val5 { get { return GetValue(4); } set { SetValue(4, value); } }
		public double Val6 { get { return GetValue(5); } set { SetValue(5, value); } }
		public double Val7 { get { return GetValue(6); } set { SetValue(6, value); } }
		public double Val8 { get { return GetValue(7); } set { SetValue(7, value); } }
		public double Val9 { get { return GetValue(8); } set { SetValue(8, value); } }
		public double Val10 { get { return GetValue(9); } set { SetValue(9, value); } }
		public double Val11 { get { return GetValue(10); } set { SetValue(10, value); } }
		public double Val12 { get { return GetValue(11); } set { SetValue(11, value); } }
		public double Val13 { get { return GetValue(12); } set { SetValue(12, value); } }
		public double Val14 { get { return GetValue(13); } set { SetValue(13, value); } }
		public double Val15 { get { return GetValue(14); } set { SetValue(14, value); } }
		public double Val16 { get { return GetValue(15); } set { SetValue(15, value); } }
		public double Val17 { get { return GetValue(16); } set { SetValue(16, value); } }
		public double Val18 { get { return GetValue(17); } set { SetValue(17, value); } }
		public double Val19 { get { return GetValue(18); } set { SetValue(18, value); } }
		public double Val20 { get { return GetValue(19); } set { SetValue(19, value); } }
		public double Val21 { get { return GetValue(20); } set { SetValue(20, value); } }
		public double Val22 { get { return GetValue(21); } set { SetValue(21, value); } }
		public double Val23 { get { return GetValue(22); } set { SetValue(22, value); } }
		public double Val24 { get { return GetValue(23); } set { SetValue(23, value); } }
		public double Val25 { get { return GetValue(24); } set { SetValue(24, value); } }
		public double Val26 { get { return GetValue(25); } set { SetValue(25, value); } }
		public double Val27 { get { return GetValue(26); } set { SetValue(26, value); } }
		public double Val28 { get { return GetValue(27); } set { SetValue(27, value); } }
		public double Val29 { get { return GetValue(28); } set { SetValue(28, value); } }
		public double Val30 { get { return GetValue(29); } set { SetValue(29, value); } }
		public double Val31 { get { return GetValue(30); } set { SetValue(30, value); } }
		public double Val32 { get { return GetValue(31); } set { SetValue(31, value); } }
		public double Val33 { get { return GetValue(32); } set { SetValue(32, value); } }
		public double Val34 { get { return GetValue(33); } set { SetValue(33, value); } }
		public double Val35 { get { return GetValue(34); } set { SetValue(34, value); } }
		public double Val36 { get { return GetValue(35); } set { SetValue(35, value); } }
		public double Val37 { get { return GetValue(36); } set { SetValue(36, value); } }
		public double Val38 { get { return GetValue(37); } set { SetValue(37, value); } }
		public double Val39 { get { return GetValue(38); } set { SetValue(38, value); } }
		public double Val40 { get { return GetValue(39); } set { SetValue(39, value); } }
		public double Val41 { get { return GetValue(40); } set { SetValue(40, value); } }
		public double Val42 { get { return GetValue(41); } set { SetValue(41, value); } }
		public double Val43 { get { return GetValue(42); } set { SetValue(42, value); } }
		public double Val44 { get { return GetValue(43); } set { SetValue(43, value); } }
		public double Val45 { get { return GetValue(44); } set { SetValue(44, value); } }
		public double Val46 { get { return GetValue(45); } set { SetValue(45, value); } }
		public double Val47 { get { return GetValue(46); } set { SetValue(46, value); } }
		public double Val48 { get { return GetValue(47); } set { SetValue(47, value); } }
		public double Val49 { get { return GetValue(48); } set { SetValue(48, value); } }
		public double Val50 { get { return GetValue(49); } set { SetValue(49, value); } }
		public double Val51 { get { return GetValue(50); } set { SetValue(50, value); } }
		public double Val52 { get { return GetValue(51); } set { SetValue(51, value); } }
		public double Val53 { get { return GetValue(52); } set { SetValue(52, value); } }
		public double Val54 { get { return GetValue(53); } set { SetValue(53, value); } }
		public double Val55 { get { return GetValue(54); } set { SetValue(54, value); } }
		public double Val56 { get { return GetValue(55); } set { SetValue(55, value); } }
		public double Val57 { get { return GetValue(56); } set { SetValue(56, value); } }
		public double Val58 { get { return GetValue(57); } set { SetValue(57, value); } }
		public double Val59 { get { return GetValue(58); } set { SetValue(58, value); } }
		public double Val60 { get { return GetValue(59); } set { SetValue(59, value); } }
		public double Val61 { get { return GetValue(60); } set { SetValue(60, value); } }
		public double Val62 { get { return GetValue(61); } set { SetValue(61, value); } }
		public double Val63 { get { return GetValue(62); } set { SetValue(62, value); } }
		public double Val64 { get { return GetValue(63); } set { SetValue(63, value); } }
		public double Val65 { get { return GetValue(64); } set { SetValue(64, value); } }
		public double Val66 { get { return GetValue(65); } set { SetValue(65, value); } }
		public double Val67 { get { return GetValue(66); } set { SetValue(66, value); } }
		public double Val68 { get { return GetValue(67); } set { SetValue(67, value); } }
		public double Val69 { get { return GetValue(68); } set { SetValue(68, value); } }
		public double Val70 { get { return GetValue(69); } set { SetValue(69, value); } }
		public double Val71 { get { return GetValue(70); } set { SetValue(70, value); } }
		public double Val72 { get { return GetValue(71); } set { SetValue(71, value); } }
		public double Val73 { get { return GetValue(72); } set { SetValue(72, value); } }
		public double Val74 { get { return GetValue(73); } set { SetValue(73, value); } }
		public double Val75 { get { return GetValue(74); } set { SetValue(74, value); } }
		public double Val76 { get { return GetValue(75); } set { SetValue(75, value); } }
		public double Val77 { get { return GetValue(76); } set { SetValue(76, value); } }
		public double Val78 { get { return GetValue(77); } set { SetValue(77, value); } }
		public double Val79 { get { return GetValue(78); } set { SetValue(78, value); } }
		public double Val80 { get { return GetValue(79); } set { SetValue(79, value); } }
		public double Val81 { get { return GetValue(80); } set { SetValue(80, value); } }
		public double Val82 { get { return GetValue(81); } set { SetValue(81, value); } }
		public double Val83 { get { return GetValue(82); } set { SetValue(82, value); } }
		public double Val84 { get { return GetValue(83); } set { SetValue(83, value); } }
		public double Val85 { get { return GetValue(84); } set { SetValue(84, value); } }
		public double Val86 { get { return GetValue(85); } set { SetValue(85, value); } }
		public double Val87 { get { return GetValue(86); } set { SetValue(86, value); } }
		public double Val88 { get { return GetValue(87); } set { SetValue(87, value); } }
		public double Val89 { get { return GetValue(88); } set { SetValue(88, value); } }
		public double Val90 { get { return GetValue(89); } set { SetValue(89, value); } }
		public double Val91 { get { return GetValue(90); } set { SetValue(90, value); } }
		public double Val92 { get { return GetValue(91); } set { SetValue(91, value); } }
		public double Val93 { get { return GetValue(92); } set { SetValue(92, value); } }
		public double Val94 { get { return GetValue(93); } set { SetValue(93, value); } }
		public double Val95 { get { return GetValue(94); } set { SetValue(94, value); } }
		public double Val96 { get { return GetValue(95); } set { SetValue(95, value); } }
		public double Val97 { get { return GetValue(96); } set { SetValue(96, value); } }
		public double Val98 { get { return GetValue(97); } set { SetValue(97, value); } }
		public double Val99 { get { return GetValue(98); } set { SetValue(98, value); } }
		public double Val100 { get { return GetValue(99); } set { SetValue(99, value); } }
		#endregion

		#endregion

		#region コンストラクタ

		/// <summary>
		/// 標準コンストラクタ
		/// </summary>
		public Data30Period()
		{
			Name = string.Empty;
			DisplayFormat = Format.None;
			BackColor = System.Drawing.Color.White;
		}

		/// <summary>
		/// long の配列から作成
		/// </summary>
		/// <param name="values">The values.</param>
		public Data30Period(long[] values)
		{
			var valueLen = values.Length;
			for (int i = 0; i < values.Length; i++)
			{
				if (i < valueLen)
					SetValue(i, values[i]);
			}
		}

		/// <summary>
		/// double の配列から作成
		/// </summary>
		/// <param name="values">The values.</param>
		public Data30Period(double[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				SetValue(i, values[i]);
			}
		}

		#endregion
		
		#region Public

		/// <summary>
		/// 値を取得
		/// </summary>
		/// <param name="i">The i.</param>
		/// <returns></returns>
		public double GetValue(int i)
		{
			return values[i];
		}

		/// <summary>
		/// 値を設定
		/// </summary>
		/// <param name="i">The i.</param>
		/// <param name="value">The value.</param>
		public void SetValue(int i, double value)
		{
			values[i] = value;
		}

		#endregion

	}
}
