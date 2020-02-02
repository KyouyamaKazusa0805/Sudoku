using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of conclusion collection.
	/// </summary>
	public static class ConclusionCollection
	{
		/// <summary>
		/// Get a string consists of all conclusion text.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <returns>The string.</returns>
		[DebuggerStepThrough]
		public static string ToString(IEnumerable<Conclusion> conclusions)
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			foreach (var conslusion in conclusions)
			{
				sb.Append($"{conslusion}{separator}");
			}

			sb.RemoveFromEnd(separator.Length);
			return sb.ToString();
		}
	}
}
