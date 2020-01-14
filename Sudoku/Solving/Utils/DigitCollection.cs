using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Utils
{
	public static class DigitCollection
	{
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
