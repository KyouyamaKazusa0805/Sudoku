using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Sudoku.Data.Extensions
{
	public static class CollectionEx
	{
		public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> values)
		{
			Contract.Assume(!(@this is null));
			Contract.Assume(!(values is null));

			foreach (var value in values)
			{
				@this.Add(value);
			}
		}
	}
}
