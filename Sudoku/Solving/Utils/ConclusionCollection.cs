using System.Collections.Generic;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Utils
{
	public static class ConclusionCollection
	{
		public static string ToString(IEnumerable<Conclusion> conclusions)
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			foreach (var conslusion in conclusions)
				sb.Append($"{conslusion}{separator}");

			sb.RemoveFromEnd(separator.Length);
			return sb.ToString();
		}
	}
}
