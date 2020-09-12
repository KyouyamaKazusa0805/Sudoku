using System;
using System.Collections;
using System.Collections.Generic;
using Sudoku.Data.Collections;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulats a map that contains 729 positions to represent a candidate.
	/// </summary>
	public unsafe struct ValueSudokuMap : IEnumerable<int>, IEquatable<ValueSudokuMap>
	{
		/// <summary>
		/// The length of the buffer.
		/// </summary>
		/// <remarks>
		/// Why 12? Because 12 is equals to <c>Ceiling(729 / 64)</c>.
		/// </remarks>
		private const int BufferLength = 12;


		/// <summary>
		/// <para>Indicates an empty instance (all bits are 0).</para>
		/// <para>
		/// I strongly recommend you <b>should</b> use this instance instead of default constructor
		/// <see cref="GridMap()"/>.
		/// </para>
		/// </summary>
		/// <seealso cref="GridMap()"/>
		public static readonly ValueSudokuMap Empty = default;


		/// <summary>
		/// The inner binary values.
		/// </summary>
		private /*readonly*/ fixed long _innerBinary[BufferLength];





		/// <inheritdoc cref="object.Equals(object?)"/>
		public override readonly bool Equals(object? obj) => obj is ValueSudokuMap comparer && Equals(comparer);

		/// <inheritdoc/>
		public readonly bool Equals(ValueSudokuMap other)
		{
			for (int i = 0; i < BufferLength; i++)
			{
				if (_innerBinary[i] != other._innerBinary[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <inheritdoc cref="object.GetHashCode"/>
		public override readonly int GetHashCode()
		{
			long @base = 0xDECADE;
			for (int i = 0; i < BufferLength; i++)
			{
				@base ^= _innerBinary[i];
			}

			return (int)(@base & 0xABCDEF);
		}

		/// <inheritdoc/>
		public readonly IEnumerator<int> GetEnumerator()
		{
#if NOT_TEST
#error Please implement 'GetEnumerator'.
#endif
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public override readonly string ToString() => new CandidateCollection(this).ToString();

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(ValueSudokuMap left, ValueSudokuMap right) => left.Equals(right);

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(ValueSudokuMap left, ValueSudokuMap right) => !(left == right);
	}
}
