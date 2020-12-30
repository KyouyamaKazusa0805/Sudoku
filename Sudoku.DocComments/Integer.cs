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
		///	<param name="this">(<see langword="this"/> parameter) The value.</param>
		///	<returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsPowerOfTwo(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// Indicates whether the current value is an odd.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsOdd(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// Indicates whether the current value is an even.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool IsEven(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// Indicates whether the current value contains the bit specified as <paramref name="bitPosition"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="bitPosition">The position of that bit to check.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public static bool ContainsBit(Integer @this, int bitPosition) => throw new NotImplementedException();

		/// <summary>
		/// Find the first offset of set bit of the binary representation
		/// of the specified value. If the value is 0, this method
		/// will always return -1.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>
		/// An <see cref="int"/> value indicating that. Please note that if the value is 0,
		/// the return value will be always 32 (for <see cref="int"/>) or 64 (for <see cref="long"/>).
		/// The method simply calls the method <see cref="BitOperations.TrailingZeroCount(int)"/>
		/// or <see cref="BitOperations.TrailingZeroCount(long)"/>.
		/// </returns>
		/// <seealso cref="BitOperations.TrailingZeroCount(int)"/>
		/// <seealso cref="BitOperations.TrailingZeroCount(long)"/>
		public static int FindFirstSet(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// Get the total number of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>An <see cref="int"/> value indicating that.</returns>
		public static int PopCount(Integer @this) => throw new NotImplementedException();


		/// <summary>
		/// Find a index of the binary representation of a value after the specified index,
		/// whose bit is set <see langword="true"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="index">The index.</param>
		/// <returns>The index.</returns>
		public static int GetNextSet(Integer @this, int index) => throw new NotImplementedException();

		/// <summary>
		/// Get an <see cref="int"/> value, indicating that the absolute position of
		/// all set bits with the specified set bit order.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <param name="order">The number of the order of set bits.</param>
		/// <returns>The position.</returns>
		public static int SetAt(Integer @this, int order) => throw new NotImplementedException();

		/// <summary>
		/// Find all offsets of set bits of the binary representation of a specified value.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The value.</param>
		/// <returns>All offsets.</returns>
		public static IEnumerable<int> GetAllSets(Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// <para>Reverse all bits in a specified value.</para>
		/// <para>
		/// Note that the value is passed by <b>reference</b> though the
		/// method is an extension method, and returns nothing.
		/// </para>
		/// </summary>
		/// <param name="this">(<see langword="this ref"/> parameter) The value.</param>
		public static void ReverseBits(ref Integer @this) => throw new NotImplementedException();

		/// <summary>
		/// <para>Extension get enumerator of this type.</para>
		/// <para>
		/// This method will allow you to use <see langword="foreach"/> loop to iterate on
		/// all indices of set bits.
		/// </para>
		/// </summary>
		/// <param name="this">
		/// (<see langword="this"/> parameter) The value.
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
