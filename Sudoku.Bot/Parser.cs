using System;

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
		/// <param name="colorId">(<see langword="out"/> parameter) The color ID.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		public static bool TryParseColorId(string @this, out long colorId)
		{
			switch (@this)
			{
				case "红色" or "红": colorId = cid(64, 255, 0, 0); return true;
				case "绿色" or "绿": colorId = cid(64, 0, 255, 0); return true;
				case "蓝色" or "蓝": colorId = cid(64, 0, 0, 255); return true;
				case "黄色" or "黄": colorId = cid(64, 255, 255, 0); return true;
				case "青色" or "青": colorId = cid(64, 0, 255, 255); return true;
				case "粉红" or "粉": colorId = cid(64, 255, 0, 255); return true;
				case "灰色" or "灰": colorId = cid(64, 192, 192, 192); return true;
				case "黑色" or "黑": colorId = cid(64, 255, 255, 255); return true;
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

#pragma warning disable 675
			static long cid(byte a, byte r, byte g, byte b) => 0xDEADL << 32 | a << 24 | r << 16 | g << 8 | b;
#pragma warning restore 675
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
