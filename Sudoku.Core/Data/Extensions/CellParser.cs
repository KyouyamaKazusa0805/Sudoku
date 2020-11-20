using System;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Encapsulates the cell parser that converts the string to an <see cref="int"/> value that
	/// represents a cell offset.
	/// </summary>
	public static class CellParser
	{
		/// <summary>
		/// Try to parse the string, and converts the instance to a cell instance represented
		/// by a <see cref="byte"/> value.
		/// </summary>
		/// <param name="str">(<see langword="in"/> parameter) The string.</param>
		/// <param name="cell">(<see langword="out"/> parameter) The cell.</param>
		/// <returns>The <see cref="bool"/> indicating that.</returns>
		public static bool TryParse(in string str, out byte cell)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				goto Return;
			}

			string copied = str.Trim();
			unsafe
			{
				if (rcb(copied, out byte c)) { cell = c; return true; }
				if (k9(copied, out c)) { cell = c; return true; }
			}

		Return:
			cell = default;
			return false;

			static bool rcb(in string str, out byte cell)
			{
				if (str.Length != 4)
				{
					goto DefaultReturn;
				}

				if (str[0] is not ('R' or 'r') || str[2] is not ('C' or 'c'))
				{
					goto DefaultReturn;
				}

				if (!char.IsDigit(str[1]) || !char.IsDigit(str[2]))
				{
					goto DefaultReturn;
				}

				cell = (byte)((str[1] - '1') * 9 + (str[3] - '1'));
				return true;

			DefaultReturn:
				cell = default;
				return false;
			}

			static bool k9(in string str, out byte cell)
			{
				if (str.Length != 2)
				{
					goto DefaultReturn;
				}

				if (str[0] is not (>= 'a' and <= 'i' or >= 'A' and <= 'I') || !char.IsDigit(str[1]))
				{
					goto DefaultReturn;
				}

				char start = str[0] is >= 'A' and <= 'I' ? 'A' : 'a';
				cell = (byte)((str[0] - start) * 9 + str[1] - '1');
				return true;

			DefaultReturn:
				cell = default;
				return false;
			}
		}

		/// <summary>
		/// Try to parse the string, and converts the instance to cells instance represented
		/// by <see cref="byte"/> values.
		/// </summary>
		/// <param name="str">(<see langword="in"/> parameter) The string.</param>
		/// <param name="cells">(<see langword="out"/> parameter) The cells.</param>
		/// <returns>The <see cref="bool"/> indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryParse(in string str, out GridMap cells) => TryParse(str, out cells, new[] { ' ' });

		/// <summary>
		/// Try to parse the string, and converts the instance to cells instance represented
		/// by <see cref="byte"/> values, with the specified separators.
		/// </summary>
		/// <param name="str">(<see langword="in"/> parameter) The string.</param>
		/// <param name="cells">(<see langword="out"/> parameter) The cell.</param>
		/// <param name="separators">(<see langword="params"/> parameter) All separators.</param>
		/// <returns>The <see cref="bool"/> indicating that.</returns>
		public static bool TryParse(in string str, out GridMap cells, params char[] separators)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				cells = default;
				return false;
			}

			cells = GridMap.Empty;
			foreach (string split in str.Trim().Split(separators, StringSplitOptions.RemoveEmptyEntries))
			{
				if (TryParse(split, out byte cell))
				{
					cells.AddAnyway(cell);
				}
			}
			return true;
		}
	}
}
