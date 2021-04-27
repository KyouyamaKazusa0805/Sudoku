using System;
using System.Collections.Generic;
using System.Extensions;
using System.Text;
using Sudoku.CodeGen.Equality.Annotations;
using Sudoku.CodeGen.StructParameterlessConstructor.Annotations;
using Sudoku.DocComments;
using static System.Numerics.BitOperations;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Indicates a collection that contains the several digits.
	/// </summary>
	[DisallowParameterlessConstructor]
	[AutoEquality(nameof(_mask))]
	public readonly ref partial struct DigitCollection
	{
		/// <summary>
		/// Indicates the inner mask.
		/// </summary>
		private readonly short _mask;


		/// <summary>
		/// Initializes the collection using a mask.
		/// </summary>
		/// <param name="mask">The mask.</param>
		public DigitCollection(short mask) => _mask = mask;

		/// <summary>
		/// Initializes an instance with the specified digits.
		/// </summary>
		/// <param name="digits">The digits.</param>
		public DigitCollection(in ReadOnlySpan<int> digits) : this()
		{
			foreach (int digit in digits)
			{
				_mask |= (short)(1 << digit);
			}
		}

		/// <summary>
		/// Initializes an instance with the specified digits.
		/// </summary>
		/// <param name="digits">The digits.</param>
		public DigitCollection(IEnumerable<int> digits) : this()
		{
			foreach (int digit in digits)
			{
				_mask |= (short)(1 << digit);
			}
		}


		/// <summary>
		/// Get the number of digits in the collection.
		/// </summary>
		public int Count => PopCount((uint)_mask);


		/// <summary>
		/// Indicates whether the specified collection contains the digit.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool Contains(int digit) => (_mask >> digit & 1) != 0;

		/// <inheritdoc/>
		public override string ToString() => ToString(", ");

		/// <inheritdoc cref="Formattable.ToString(string)"/>
		public string ToString(string? format)
		{
			if (_mask == 0)
			{
				return "{ }";
			}

			if ((_mask & _mask - 1) == 0)
			{
				return (TrailingZeroCount(_mask) + 1).ToString();
			}

			string separator = format ?? string.Empty;
			var sb = new ValueStringBuilder(stackalloc char[9]);
			foreach (int digit in this)
			{
				sb.Append(digit + 1);
				sb.Append(separator);
			}

			sb.RemoveFromEnd(separator.Length);
			return sb.ToString();
		}

		/// <summary>
		/// Get the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public ReadOnlySpan<int>.Enumerator GetEnumerator() => _mask.GetEnumerator();


		/// <summary>
		/// Reverse all statuses, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="collection">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		public static DigitCollection operator ~(in DigitCollection collection) => new((short)~collection._mask);
	}
}
