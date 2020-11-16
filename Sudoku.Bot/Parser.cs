using System;
using Sudoku.Data;

namespace Sudoku.Bot
{
	/// <summary>
	/// Encapsulates a parser.
	/// </summary>
	public static class Parser
	{
		/// <summary>
		/// Try to parse a color ID.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <param name="withTransparency">
		/// Indicates whether the parser will convert the value to the color with transparency.
		/// </param>
		/// <param name="colorId">(<see langword="out"/> parameter) The color ID.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		public static bool TryParseColorId(string @this, bool withTransparency, out long colorId)
		{
			switch (@this)
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
					string[] s = @this.Split(',', StringSplitOptions.RemoveEmptyEntries);
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
		/// Try to parse cells.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <param name="cells">(<see langword="out"/> parameter) All cells parsed.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool TryParseCells(string @this, out GridMap cells)
		{
			cells = GridMap.Empty;
			if (string.IsNullOrWhiteSpace(@this))
			{
				return false;
			}

			foreach (string cellStr in @this.Split(','))
			{
				if (TryParseCell(cellStr, out byte row, out byte column))
				{
					cells.AddAnyway(row * 9 + column);
				}
			}

			return true;
		}

		/// <summary>
		/// Try to parse a cell.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <param name="row">
		/// (<see langword="out"/> parameter) The row. If failed to parse, the value will be
		/// <see langword="default"/>(<see cref="byte"/>).
		/// </param>
		/// <param name="column">
		/// (<see langword="out"/> parameter) The column. If failed to parse, the value will be
		/// <see langword="default"/>(<see cref="byte"/>).
		/// </param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool TryParseCell(string @this, out byte row, out byte column)
		{
			switch (@this.Length)
			{
				case 2 when isAtoI(@this[0]) && isDigit(@this[1]):
				{
					row = (byte)(@this[0] - toWeight(@this[0]));
					column = toDigit(@this[1]);
					return true;
				}
				case 4 when isRowLabel(@this[0]) && isColumnLabel(@this[2]) && isDigit(@this[1]) && isDigit(@this[3]):
				{
					row = toDigit(@this[1]);
					column = toDigit(@this[3]);
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
