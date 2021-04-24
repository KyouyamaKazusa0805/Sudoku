#pragma warning disable IDE0079
#pragma warning disable IDE0060
#pragma warning disable CA1716
#pragma warning disable CA1720

using System;
using System.Collections.Generic;

namespace Sudoku.DocComments
{
	/// <summary>
	/// Provides with doc comments for checking methods around integers.
	/// </summary>
	public readonly struct Integer
	{
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
