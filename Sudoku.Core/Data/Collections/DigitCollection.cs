using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Sudoku.DocComments;
using Sudoku.Extensions;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Indicates a collection that contains the several digits.
	/// </summary>
	public readonly ref struct DigitCollection
	{
		/// <summary>
		/// Indicates the inner mask.
		/// </summary>
		private readonly short _mask;


		/// <summary>
		/// Initializes an instance with the specified digit.
		/// </summary>
		/// <param name="digit">The digit.</param>
		public DigitCollection(int digit) : this() => _mask |= (short)(1 << digit);

		/// <summary>
		/// Initializes an instance with the specified digits.
		/// </summary>
		/// <param name="digits">(<see langword="in"/> parameter) The digits.</param>
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
		/// Initializes the collection using a mask.
		/// </summary>
		/// <param name="mask">The mask.</param>
		private DigitCollection(short mask) => _mask = mask;


		/// <summary>
		/// Get the number of digits in the collection.
		/// </summary>
		public int Count => _mask.PopCount();


		/// <inheritdoc/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never), DoesNotReturn]
		public override bool Equals(object? obj) =>
			throw new NotSupportedException(
				"This instance doesn't support this member, " +
				"because this method will cause box and unbox operations, " +
				"which is invalid in ref structures.");

		/// <inheritdoc cref="IValueEquatable{TStruct}.Equals(in TStruct)"/>
		public bool Equals(in DigitCollection other) => _mask == other._mask;

		/// <summary>
		/// Indicates whether the specified collection contains the digit.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool Contains(int digit) => (_mask >> digit & 1) != 0;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never), DoesNotReturn]
		public override int GetHashCode() =>
			throw new NotSupportedException(
				"This instance doesn't support this member, " +
				"because this method will cause box and unbox operations, " +
				"which is invalid in ref structures.");

		/// <inheritdoc/>
		public override string ToString() => ToString(", ");

		/// <inheritdoc cref="Formattable.ToString(string)"/>
		public string ToString(string? format)
		{
			if (_mask == 0)
			{
				return "{ }";
			}

			if (_mask.IsPowerOfTwo())
			{
				return (_mask.FindFirstSet() + 1).ToString();
			}

			string separator = format.NullableToString();
			var sb = new StringBuilder();
			foreach (int digit in this)
			{
				sb.Append(digit + 1).Append(separator);
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}

		/// <summary>
		/// Get the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<int> GetEnumerator() => _mask.GetAllSets().GetEnumerator();


		/// <summary>
		/// Reverse all statuses, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="collection">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		public static DigitCollection operator ~(in DigitCollection collection) => new((short)~collection._mask);

		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in DigitCollection left, in DigitCollection right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in DigitCollection left, in DigitCollection right) => !(left == right);
	}
}
