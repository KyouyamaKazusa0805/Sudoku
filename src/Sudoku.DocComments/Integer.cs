#pragma warning disable IDE0079
#pragma warning disable IDE0060

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sudoku.DocComments
{
	/// <summary>
	/// Provides with doc comments for checking methods around integers.
	/// </summary>
	public readonly ref struct Integer
	{
		/// <summary>
		///	Indicates whether the specified value is the power of two.
		/// </summary>
		///	<param name="this">The value.</param>
		///	<returns>A <see cref="bool"/> value indicating that.</returns>
		///	<remarks>
		///	The equivalent code: <c>x != 0 &amp;&amp; (x &amp; (x - 1)) != 0</c>.
		/// </remarks>
		public static bool IsPowerOfTwo(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// Indicates whether the current value is an odd.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <remarks>
		///	The equivalent code: <c>(x &amp; 1) != 0</c>.
		/// </remarks>
		public static bool IsOdd(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// Indicates whether the current value is an even.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <remarks>
		///	The equivalent code: <c>(x &amp; 1) == 0</c>.
		/// </remarks>
		public static bool IsEven(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// Indicates whether the current value contains the bit specified as <paramref name="bitPosition"/>.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <param name="bitPosition">The position of that bit to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <remarks>
		///	The equivalent code: <c>(x &gt;&gt; i &amp; 1) != 0</c>.
		/// </remarks>
		public static bool ContainsBit(Integer @this, int bitPosition) => throw new NotImplementedException();

		/// <summary>
		/// Determine whether the current value overlaps the specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <param name="other">The other value to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <remarks>
		///	The equivalent code: <c>(x &amp; y) != 0</c>.
		/// </remarks>
		public static bool Overlaps(Integer @this, Integer other) => throw new NotImplementedException();

		/// <summary>
		/// Determine whether the current value overlaps the negation of the specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <param name="other">The other value to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <remarks>
		///	The equivalent code: <c>(x &amp; ~y) != 0</c>.
		/// </remarks>
		public static bool ExceptOverlaps(Integer @this, Integer other) => throw new NotImplementedException();

		/// <summary>
		/// Determine whether the bits of the current value fully covers the ones of the specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <param name="other">The other value to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <remarks>
		/// The equivalent code: <c>(x &amp; y) == y</c>.
		/// </remarks>
		public static bool Covers(Integer @this, Integer other) => throw new NotImplementedException();

		/// <summary>
		/// Find the first offset of set bit of the binary representation
		/// of the specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>
		/// <para>An <see cref="int"/> value indicating that.</para>
		/// <para>
		/// Please note that if the value is 0,
		/// the return value will be always 32 (for <see cref="int"/>) or 64 (for <see cref="long"/>).
		/// The method simply calls the method <see cref="BitOperations.TrailingZeroCount(int)"/>
		/// or <see cref="BitOperations.TrailingZeroCount(long)"/>.
		/// </para>
		/// </returns>
		/// <seealso cref="BitOperations.TrailingZeroCount(int)"/>
		/// <seealso cref="BitOperations.TrailingZeroCount(long)"/>
		public static int FindFirstSet(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// Get the total number of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>An <see cref="int"/> value indicating that.</returns>
		public static int PopCount(Integer @this) => throw new NotImplementedException();


		/// <summary>
		/// Find a index of the binary representation of a value after the specified index,
		/// whose bit is set <see langword="true"/>.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <param name="index">The index.</param>
		/// <returns>The index.</returns>
		public static int GetNextSet(Integer @this, int index) => throw new NotImplementedException();

		/// <summary>
		/// Get an <see cref="int"/> value, indicating that the absolute position of
		/// all set bits with the specified set bit order.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <param name="order">The number of the order of set bits.</param>
		/// <returns>The position.</returns>
		public static int SetAt(Integer @this, int order) => throw new NotImplementedException();

		/// <summary>
		/// Find all offsets of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">The value.</param>
		/// <returns>All offsets.</returns>
		public static IEnumerable<int> GetAllSets(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// <para>Reverse all bits in a specified value.</para>
		/// <para>
		/// Note that the value is passed by <b>reference</b> though the
		/// method is an extension method, and returns nothing.
		/// </para>
		/// </summary>
		/// <param name="this">The value.</param>
		public static void ReverseBits(ref Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// Skip the specified number of set bits and iterate on the integer with other set bits.
		/// </summary>
		/// <param name="this">The integer to iterate.</param>
		/// <param name="setBitPosCount">Indicates how many set bits you want to skip to iterate.</param>
		/// <returns>The integer that only contains the other set bits.</returns>
		/// <remarks>
		/// For example:
		/// <code>
		/// byte value = 0b00010111;
		/// foreach (int bitPos in value.SkipSetBit(2))
		/// {
		///     yield return bitPos + 1;
		/// }
		/// </code>
		/// You will get 3 and 5, because all set bit positions are 0, 1, 2 and 4, and we have skipped
		/// two of them, so the result set bit positions to iterate on are only 2 and 4.
		/// </remarks>
		public static Integer SkipSetBit(Integer @this, int setBitPosCount) => throw new NotImplementedException();

		/// <summary>
		/// <para>Extension get enumerator of this type.</para>
		/// <para>
		/// This method will allow you to use <see langword="foreach"/> loop to iterate on
		/// all indices of set bits.
		/// </para>
		/// </summary>
		/// <param name="this">
		/// The value.
		/// </param>
		/// <returns>All indices of set bits.</returns>
		/// <remarks>
		/// This implementation will allow you use <see langword="foreach"/> loop:
		/// <code>
		/// foreach (int setIndex in 17)
		/// {
		/// 	// Do something...
		/// }
		/// </code>
		/// </remarks>
		public static IEnumerator<int> GetEnumerator(Integer @this) => throw new NotImplementedException();
	}
}
