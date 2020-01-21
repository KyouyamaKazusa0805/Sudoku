using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of digit collection.
	/// </summary>
	public static class DigitCollection
	{
		/// <summary>
		/// Get a string consists of digits' text.
		/// </summary>
		/// <param name="digits">The digits.</param>
		/// <returns>The string.</returns>
		public static string ToString(IEnumerable<int> digits)
		{
			var sb = new StringBuilder();
			const string separator = ", ";
			foreach (int digit in (from digit in digits orderby digit select digit).Distinct())
			{
				sb.Append($"{digit + 1}{separator}");
			}
			sb.RemoveFromEnd(separator.Length);

			return sb.ToString();
		}
	}
}
