using System.Collections.Generic;

namespace Sudoku.Data.Extensions
{
	public static class CollectionEx
	{
		public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> values)
		{
			foreach (var value in values)
			{
				@this.Add(value);
			}
		}
	}
}
