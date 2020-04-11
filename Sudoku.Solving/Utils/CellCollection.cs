using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sudoku.Extensions;

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
			var group = from cell in from cell in cellOffsets orderby cell select cell
						group cell by cell / 9;
			int cellGroupCount = group.Count();
			if (cellGroupCount >= 2)
			{
				sb.Append("{ ");
			}

			foreach (var cellByRowGroup in group)
			{
				int row = cellByRowGroup.Key;
				sb.Append($"r{row + 1}c");
				foreach (int cell in cellByRowGroup)
				{
					sb.Append(cell % 9 + 1);
				}

				sb.Append(separator);
			}

			sb.RemoveFromEnd(separator.Length);
			if (cellGroupCount >= 2)
			{
				sb.Append(" }");
			}

			return sb.ToString();
		}
	}
}
