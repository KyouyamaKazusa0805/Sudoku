using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of cell collection.
	/// </summary>
	[DebuggerStepThrough]
	public static class CellCollection
	{
		/// <summary>
		/// Get a string consists of all cell text.
		/// </summary>
		/// <param name="cellOffsets">The cell offsets.</param>
		/// <returns>The string.</returns>
		public static string ToString(IEnumerable<int> cellOffsets)
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			foreach (var cellOffset in cellOffsets)
			{
				sb.Append($"{CellUtils.ToString(cellOffset)}{separator}");
			}

			sb.RemoveFromEnd(separator.Length);
			return sb.ToString();
		}
	}
}
