using System;

namespace Sudoku.Drawing.Extensions
{
	/// <summary>
	/// Encapsulates the color ID parser that converts the string to an <see cref="long"/> value that
	/// represents a color ID used in <see cref="DrawingInfo"/>.
	/// </summary>
	/// <seealso cref="DrawingInfo"/>
	public static class ColorIdParser
	{
		/// <summary>
		/// Try to parse the string, and convert it to a color ID.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <param name="colorId">(<see langword="out"/> parameter) The color ID.</param>
		/// <param name="alpha">The alpha.</param>
		/// <param name="separators">(<see langword="params"/> parameter) The separators.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool TryParse(string str, out long colorId, byte? alpha = null, params char[] separators)
		{
			string[] s = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
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

			colorId = ColorId.ToCustomColorId(alpha ?? a, r, g, b);
			return true;
		}
	}
}
