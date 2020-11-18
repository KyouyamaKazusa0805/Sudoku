#if AUTHOR_RESERVED

using System;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Bot
{
	/// <summary>
	/// 封装一个常用的解析工具类。
	/// </summary>
	public static class Parser
	{
		/// <summary>
		/// 尝试解析字符串，并转换为一个表示颜色的 ID。这个颜色 ID 一般用来封装和保存到 <see cref="DrawingInfo"/>
		/// 对象里。
		/// </summary>
		/// <param name="str">字符串。</param>
		/// <param name="withTransparency">表示当前解析后的颜色是否保留透明度。</param>
		/// <param name="colorId">(<see langword="out"/> 参数) 转换后的颜色 ID。</param>
		/// <returns>表示是否转换成功。</returns>
		public static bool TryParseColorId(string str, bool withTransparency, out long colorId)
		{
			switch (str)
			{
				case "红色" or "红": colorId = cid(withTransparency ? 128 : 255, 235, 0, 0); return true;
				case "浅红色" or "浅红": colorId = cid(withTransparency ? 64 : 255, 247, 165, 167); return true;
				case "橙色" or "橙": colorId = cid(withTransparency ? 64 : 255, 255, 192, 89); return true;
				case "浅橙色" or "浅橙": colorId = cid(withTransparency ? 64 : 255, 247, 222, 143); return true;
				case "黄色" or "黄": colorId = cid(withTransparency ? 64 : 255, 255, 255, 150); return true;
				case "绿色" or "绿": colorId = cid(withTransparency ? 64 : 255, 134, 242, 128); return true;
				case "浅绿色" or "浅绿": colorId = cid(withTransparency ? 64 : 255, 215, 255, 215); return true;
				case "青色" or "青": colorId = cid(withTransparency ? 64 : 255, 134, 232, 208); return true;
				case "浅青色" or "浅青": colorId = cid(withTransparency ? 64 : 255, 206, 251, 237); return true;
				case "蓝色" or "蓝": colorId = cid(withTransparency ? 64 : 255, 0, 0, 255); return true;
				case "浅蓝色" or "浅蓝": colorId = cid(withTransparency ? 64 : 255, 127, 187, 255); return true;
				case "紫色" or "紫": colorId = cid(withTransparency ? 64 : 255, 177, 165, 243); return true;
				case "浅紫色" or "浅紫": colorId = cid(withTransparency ? 64 : 255, 220, 212, 252); return true;
				default:
				{
					string[] s = str.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
					if (s.Length != 4)
					{
						colorId = default;
						return false;
					}

					if (!byte.TryParse(s[0], out byte a) || !byte.TryParse(s[1], out byte r)
						|| !byte.TryParse(s[2], out byte g) || !byte.TryParse(s[3], out byte b))
					{
						colorId = default;
						return false;
					}

					colorId = cid(a, r, g, b);
					return true;
				}
			}

			static long cid(long a, long r, long g, long b) => 0xDEADL << 32 | a << 24 | r << 16 | g << 8 | b;
		}

		/// <summary>
		/// 尝试解析字符串，并转换为一系列单元格，用 <see cref="GridMap"/> 对象表示。
		/// </summary>
		/// <param name="str">字符串。</param>
		/// <param name="cells">(<see langword="out"/> 参数) 解析后的对象。</param>
		/// <returns>表示是否转换成功。</returns>
		public static bool TryParseCells(string str, out GridMap cells)
		{
			cells = GridMap.Empty;
			if (string.IsNullOrWhiteSpace(str))
			{
				return false;
			}

			foreach (string cellStr in str.Split(new[] { ',', '，' }))
			{
				if (TryParseCell(cellStr, out byte row, out byte column))
				{
					cells.AddAnyway(row * 9 + column);
				}
			}

			return true;
		}

		/// <summary>
		/// 尝试解析一个字符串，并转换为一个单元格的绝对编号（偏移量）。
		/// </summary>
		/// <param name="str">字符串。</param>
		/// <param name="row">
		/// (<see langword="out"/> 参数) 表示所在行。如果解析失败，这个数值则为
		/// <see langword="default"/>(<see cref="byte"/>)。
		/// </param>
		/// <param name="column">
		/// (<see langword="out"/> 参数) 表示所在列。如果解析失败，这个数值则为
		/// <see langword="default"/>(<see cref="byte"/>)。
		/// </param>
		/// <returns>表示是否解析成功。</returns>
		public static bool TryParseCell(string str, out byte row, out byte column)
		{
			switch (str.Length)
			{
				case 2 when isAtoI(str[0]) && isDigit(str[1]):
				{
					row = (byte)(str[0] - toWeight(str[0]));
					column = toDigit(str[1]);
					return true;
				}
				case 4 when isRowLabel(str[0]) && isColumnLabel(str[2]) && isDigit(str[1]) && isDigit(str[3]):
				{
					row = toDigit(str[1]);
					column = toDigit(str[3]);
					return true;
				}
				default:
				{
					row = column = default;
					return false;
				}
			}

			static bool isAtoI(char c) => c is >= 'A' and <= 'I' or >= 'a' and <= 'i';
			static bool isDigit(char c) => c is >= '1' and <= '9';
			static bool isRowLabel(char c) => c is 'R' or 'r';
			static bool isColumnLabel(char c) => c is 'C' or 'c';
			static byte toDigit(char c) => (byte)(c - '1');
			static char toWeight(char c) => c is >= 'A' and <= 'I' ? 'A' : 'a';
		}
	}
}

#endif