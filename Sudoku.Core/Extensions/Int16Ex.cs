using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="short"/>.
	/// </summary>
	/// <seealso cref="short"/>
	public static class Int16Ex
	{
		/// <inheritdoc cref="Integer.IsPowerOfTwo(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPowerOfTwo(this short @this) => @this != 0 && (@this & (@this - 1)) == 0;

		/// <inheritdoc cref="Integer.FindFirstSet(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this short @this) => Int32Ex.FindFirstSet(@this);

		/// <inheritdoc cref="Integer.PopCount(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(this short @this) => BitOperations.PopCount((uint)@this);

		/// <inheritdoc cref="Integer.GetNextSet(Integer, int)"/>
		public static int GetNextSet(this short @this, int index)
		{
			for (int i = index + 1; i < 16; i++)
			{
				if ((@this & 1 << i) != 0)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.SetAt(Integer, int)"/>
		public static int SetAt(this short @this, int order)
		{
			for (int i = 0, count = -1; i < 16; i++, @this >>= 1)
			{
				if ((@this & 1) != 0 && ++count == order)
				{
					return i;
				}
			}

			return -1;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static IEnumerable<int> GetAllSets(this short @this)
		{
			if (@this == 0)
			{
				yield break;
			}

			for (int i = 0; i < 16; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					yield return i;
				}
			}
		}

		/// <inheritdoc cref="Integer.GetEnumerator(Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerator<int> GetEnumerator(this short @this) => @this.GetAllSets().GetEnumerator();

		/// <inheritdoc cref="Integer.ReverseBits(ref Integer)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReverseBits(this ref short @this)
		{
			@this = (short)(@this >> 1 & 0x5555 | (@this & 0x5555) << 1);
			@this = (short)(@this >> 2 & 0x3333 | (@this & 0x3333) << 2);
			@this = (short)(@this >> 4 & 0x0F0F | (@this & 0x0F0F) << 4);
			@this = (short)(@this >> 8 | @this << 8);
		}

		/// <summary>
		/// <c>Where</c> method on <see cref="short"/> values to enable the usage of <see langword="where"/> clause.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="condition">The condition for each set bits.</param>
		/// <returns>
		/// All set bits satisfying the specified condition, representing as a <see cref="short"/> value.
		/// </returns>
		public static short Where(this short @this, Predicate<int> condition)
		{
			short result = 0;
			foreach (int i in @this)
			{
				if (condition(i))
				{
					result |= (short)(1 << i);
				}
			}

			return result;
		}

		/// <summary>
		/// <c>Where</c> method on <see cref="short"/> values to enable the usage of <see langword="select"/> clause.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="selector">The selector for each set bits.</param>
		/// <returns>The result list using projection.</returns>
		public static IEnumerable<TResult?> Select<TResult>(this short @this, Func<int, TResult?> selector)
		{
			foreach (int value in @this)
			{
				yield return selector(value);
			}
		}

		/// <summary>
		/// Simply called <c>.GetAllSets().ToArray().GetSubsets(count)</c>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The mask.</param>
		/// <param name="count">The number of set bits you want to take.</param>
		/// <returns>The mask subset list.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<int[]> GetMaskSubsets(this short @this, int count) =>
			@this.GetAllSets().ToArray().GetSubsets(count);
	}
}
