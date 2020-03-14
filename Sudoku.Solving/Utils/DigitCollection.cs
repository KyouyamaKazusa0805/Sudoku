using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of digit collection.
	/// </summary>
	[DebuggerStepThrough]
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
			foreach (int digit in new HashSet<int>(
				from digit in digits orderby digit select digit))
			{
				sb.Append($"{digit + 1}{separator}");
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}

		/// <summary>
		/// Get a simple string consists of digits' text.
		/// </summary>
		/// <param name="digits">The digits.</param>
		/// <returns>The string.</returns>
		public static string ToSimpleString(IEnumerable<int> digits)
		{
			var sb = new StringBuilder();
			foreach (int digit in new HashSet<int>(
				from digit in digits orderby digit select digit))
			{
				sb.Append($"{digit + 1}");
			}

			return sb.ToString();
		}
	}
}
