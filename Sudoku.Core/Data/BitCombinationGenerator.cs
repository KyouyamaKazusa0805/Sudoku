#pragma warning disable CA1815
#pragma warning disable CA2231

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Constants;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a bit combination generator.
	/// </summary>
	/// <example>
	/// You can use this struct like this:
	/// <code>
	/// foreach (short mask in new BitCombinationGenerator(9, 3))
	/// {
	///     // Do something to use the mask.
	/// }
	/// </code>
	/// </example>
	public ref struct BitCombinationGenerator
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
			(_value, _mask, _isLast) = ((1 << oneCount) - 1, (1 << bitCount - oneCount) - 1, bitCount == 0);
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


		/// <inheritdoc/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override readonly bool Equals(object? obj) => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override readonly int GetHashCode() => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="object.ToString"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override readonly string ToString() => throw Throwings.RefStructNotSupported;

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

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<long> GetEnumerator()
		{
			var list = new List<long>();
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

				list.Add(result);
			}

			return list.GetEnumerator();
		}
	}
}
