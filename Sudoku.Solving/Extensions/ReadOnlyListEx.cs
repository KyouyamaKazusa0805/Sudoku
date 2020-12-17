using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IReadOnlyList{T}"/>.
	/// </summary>
	/// <seealso cref="IReadOnlyList{T}"/>
	public static class ReadOnlyListEx
	{
		/// <summary>
		/// Converts all cells to the links that is used in drawing ULs or Reverse BUGs.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The list of cells.</param>
		/// <param name="offset">The offset. The default value is 4.</param>
		/// <returns>All links.</returns>
		public static IReadOnlyList<Link> GetLinks(this IReadOnlyList<int> @this, int offset = 4)
		{
			var result = new List<Link>();

			for (int i = 0, length = @this.Count - 1; i < length; i++)
			{
				result.Add(new(@this[i] * 9 + offset, @this[i + 1] * 9 + offset, LinkType.Line));
			}

			result.Add(new(@this[^1] * 9 + offset, @this[0] * 9 + offset, LinkType.Line));

			return result;
		}
	}
}
