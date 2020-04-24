using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sudoku.Extensions;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of candidate collection.
	/// </summary>
	[DebuggerStepThrough]
	public static class CandidateCollection
	{
		/// <summary>
		/// Get a string consists of all candidate text.
		/// </summary>
		/// <param name="candidateOffsets">The candidate offsets.</param>
		/// <returns>A string.</returns>
		public static string ToString(IEnumerable<int> candidateOffsets)
		{
			const string separator = ", ";
			var sb = new StringBuilder();
			foreach (var candidateGroupByDigit in
				from candidate in
					from cand in candidateOffsets
					orderby cand
					select cand
				group candidate by candidate % 9)
			{
				int digit = candidateGroupByDigit.Key;
				var group = from candidate in candidateGroupByDigit
							orderby candidate
							group candidate by candidate / 81;
				int cellGroupCount = group.Count();
				if (cellGroupCount >= 2)
				{
					sb.Append("{ ");
				}
				foreach (var candidateGroupByCellRow in group)
				{
					int cellRow = candidateGroupByCellRow.Key;
					sb.Append($"r{cellRow + 1}c");
					foreach (int cell in candidateGroupByCellRow)
					{
						sb.Append($"{cell / 9 % 9 + 1}");
					}

					sb.Append(separator);
				}

				sb.RemoveFromEnd(separator.Length);
				if (cellGroupCount >= 2)
				{
					sb.Append(" }");
				}
				sb.Append($"({digit + 1}){separator}");
			}

			sb.RemoveFromEnd(separator.Length);
			return sb.ToString();
		}
	}
}
