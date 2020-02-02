using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sudoku.Data.Extensions;

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

			foreach (int candidateOffset in candidateOffsets)
			{
				sb.Append($"{CandidateUtils.ToString(candidateOffset)}{separator}");
			}

			sb.RemoveFromEnd(separator.Length);
			return sb.ToString();
		}
	}
}
