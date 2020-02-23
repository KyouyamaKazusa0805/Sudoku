using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Encapsulates a bit combination generator.
	/// </summary>
	[SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "<Pending>")]
	[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
	public struct BitCombinationGenerator : IEnumerable<long>
	{
		/// <summary>
		/// The mask.
		/// </summary>
		private readonly long _mask;

		/// <summary>
		/// The value.
		/// </summary>
		private long _value;

		/// <summary>
		/// Indicates whether that the value is the last one.
		/// </summary>
		private bool _isLast;


		/// <summary>
		/// Initializes an instance with the specified number of bits
		/// and <see langword="true"/> bits.
		/// </summary>
		/// <param name="bitCount">The number of bits.</param>
		/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
		public BitCombinationGenerator(int bitCount, int oneCount)
		{
			(BitCount, OneCount) = (bitCount, oneCount);
			(_value, _mask, _isLast) = ((1 << oneCount) - 1, (1 << (bitCount - oneCount)) - 1, bitCount == 0);
		}


		/// <summary>
		/// Indicates how many bits should be generated.
		/// </summary>
		public readonly int BitCount { get; }

		/// <summary>
		/// Indicates how many <see langword="true"/> bits (1) are in
		/// the number.
		/// </summary>
		public readonly int OneCount { get; }


		/// <summary>
		/// Indicates whether the generator has the next combination number to iterate.
		/// </summary>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public bool HasNext()
		{
			bool result = !_isLast;
			_isLast = (_value & -_value & _mask) == 0;
			return result;
		}

		/// <inheritdoc/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object? obj) =>
			throw new NotSupportedException("The instance does not support this method.");

		/// <inheritdoc/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode() =>
			throw new NotSupportedException("The instance does not support this method.");

		/// <inheritdoc/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToString() =>
			throw new NotSupportedException("The instance does not support this method.");

		/// <inheritdoc/>
		public IEnumerator<long> GetEnumerator()
		{
			while (HasNext())
			{
				long result = _value;
				if (!_isLast)
				{
					long smallest = _value & -_value;
					long ripple = _value + smallest;
					long ones = _value ^ ripple;
					ones = (ones >> 2) / smallest;
					_value = ripple | ones;
				}

				yield return result;
			}
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
