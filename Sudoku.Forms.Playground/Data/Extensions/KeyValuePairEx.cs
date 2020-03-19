using System.Collections.Generic;

namespace Sudoku.Data.Extensions
{
	public static class KeyValuePairEx
	{
		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> @this, out TKey key, out TValue value)
		{
			key = @this.Key;
			value = @this.Value;
		}
	}
}
