using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
		/// Initializes an instance with the specified digits.
		/// </summary>
		/// <param name="digits">The digits.</param>
		public DigitCollection(params int[] digits) : this((IEnumerable<int>)digits) { }

		/// <summary>
		/// Initializes an instance with the specified digits.
		/// </summary>
		/// <param name="digits">The digits.</param>
		public DigitCollection(ReadOnlySpan<int> digits)
		{
			_mask = 0;
			foreach (int digit in digits)
			{
				_mask |= (short)(1 << digit);
			}
		}

		/// <summary>
		/// Initializes an instance with the specified digits.
		/// </summary>
		/// <param name="digits">The digits.</param>
		public DigitCollection(IEnumerable<int> digits)
		{
			_mask = 0;
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
		public int Count => _mask.CountSet();

		/// <summary>
		/// Get all digits that contains in this collection.
		/// </summary>
		public IEnumerable<int> Digits => _mask.GetAllSets();


		/// <inheritdoc/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object? obj) => throw Throwing.RefStructNotSupported;

		/// <summary>
		/// Indicates whether two specified collection are same.
		/// </summary>
		/// <param name="other">The other collection.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool Equals(DigitCollection other) => _mask == other._mask;

		/// <summary>
		/// Indicates whether the specified collection contains the digit.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool Contains(int digit) => (_mask >> digit & 1) != 0;

		/// <inheritdoc/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode() => throw Throwing.RefStructNotSupported;

		/// <inheritdoc/>
		public override string ToString() => ToString(", ");

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="string"]'/>
		public string ToString(string? format)
		{
			string separator = format ?? string.Empty;
			var sb = new StringBuilder();
			foreach (int digit in Digits)
			{
				sb.Append($"{digit}{separator}");
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}

		/// <summary>
		/// Get the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<int> GetEnumerator() => Digits.GetEnumerator();


		/// <summary>
		/// Reverse all statuses, which means all <see langword="true"/> bits
		/// will be set <see langword="false"/>, and all <see langword="false"/> bits
		/// will be set <see langword="true"/>.
		/// </summary>
		/// <param name="collection">The instance to negate.</param>
		/// <returns>The negative result.</returns>
		public static DigitCollection operator ~(DigitCollection collection) =>
			new DigitCollection((short)~collection._mask);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(DigitCollection left, DigitCollection right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(DigitCollection left, DigitCollection right) => !(left == right);

		/// <summary>
		/// Converts the <see cref="ReadOnlySpan{T}"/> to the <see cref="DigitCollection"/>
		/// instance.
		/// </summary>
		/// <param name="digits">The digits.</param>
		public static implicit operator DigitCollection(ReadOnlySpan<int> digits) => new DigitCollection(digits);
	}
}
