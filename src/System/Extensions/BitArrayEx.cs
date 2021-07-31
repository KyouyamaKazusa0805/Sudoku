using System.Buffers;
using System.Collections;
using static System.Numerics.BitOperations;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="BitArray"/>.
	/// </summary>
	/// <seealso cref="BitArray"/>
	public static class BitArrayEx
	{
		/// <summary>
		/// Get the cardinality of the specified <see cref="BitArray"/>.
		/// </summary>
		/// <param name="this">The array.</param>
		/// <returns>The total number of bits set <see langword="true"/>.</returns>
		public static unsafe int GetCardinality(this BitArray @this)
		{
			int[] integers = ArrayPool<int>.Shared.Rent((@this.Length >> 5) + 1);
			try
			{
				@this.CopyTo(integers, 0);

				int result = 0;
				fixed (int* p = integers)
				{
					int i = 0, length = integers.Length;
					for (int* ptr = p; i < length; ptr++, i++)
					{
						result += PopCount((uint)*ptr);
					}
				}

				return result;
			}
			finally
			{
				ArrayPool<int>.Shared.Return(integers);
			}
		}
	}
}
