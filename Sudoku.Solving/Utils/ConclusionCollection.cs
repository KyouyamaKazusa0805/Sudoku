using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of conclusion collection.
	/// </summary>
	[DebuggerStepThrough]
	public static class ConclusionCollection
	{
		/// <summary>
		/// Get a string consists of all conclusion text.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <returns>The string.</returns>
		public static string ToString(IEnumerable<Conclusion> conclusions)
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			foreach (var conclusionGroupByType in
				from conclusion in conclusions
				group conclusion by conclusion.ConclusionType)
			{
				var type = conclusionGroupByType.Key;
				foreach (var conclusionGroupByDigit in
					from conclusion in conclusionGroupByType
					orderby conclusion.Digit
					group conclusion by conclusion.Digit)
				{
					int digit = conclusionGroupByDigit.Key;
					foreach (var conclusionGroupByCellRow in
						from conclusion in conclusionGroupByDigit
						group conclusion by conclusion.CellOffset / 9)
					{
						int row = conclusionGroupByCellRow.Key;
						sb.Append($"r{row + 1}c");
						foreach (var conclusion in conclusionGroupByCellRow)
						{
							sb.Append($"{conclusion.CellOffset % 9 + 1}");
						}

						sb.Append($"{(type == ConclusionType.Elimination ? " <> " : " = ")}{digit + 1}{separator}");
					}
				}

				sb.RemoveFromEnd(separator.Length);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Get a simple string consists of all conclusion text.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="separator">The string separator.</param>
		/// <returns>The string.</returns>
		public static string ToSimpleString(IEnumerable<Conclusion> conclusions, string separator = ", ")
		{
			var sb = new StringBuilder();

			foreach (var conclusion in conclusions)
			{
				sb.Append($"{conclusion}{separator}");
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}
	}
}
