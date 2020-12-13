using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a bit combination generator.
	/// </summary>
	/// <remarks>
	/// You can use this struct like this:
	/// <code>
	/// foreach (short mask in new BitSubsetsGenerator(9, 3))
	/// {
	///     // Do something to use the mask.
	/// }
	/// </code>
	/// </remarks>
	public readonly ref partial struct BitSubsetsGenerator
	{
		/// <summary>
		/// The inner enumerator.
		/// </summary>
		private readonly Enumerator _enumerator;


		/// <summary>
		/// Initializes an instance with the specified number of bits
		/// and <see langword="true"/> bits.
		/// </summary>
		/// <param name="bitCount">The number of bits.</param>
		/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
		public BitSubsetsGenerator(int bitCount, int oneCount) => _enumerator = new(bitCount, oneCount);


		/// <inheritdoc cref="object.Equals(object?)"/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object? obj) => false;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never), DoesNotReturn]
		public override int GetHashCode() =>
			throw new NotSupportedException(
				"This instance doesn't support this member, " +
				"because this method will cause box and unbox operations, " +
				"which is invalid in ref structures.");

		/// <inheritdoc cref="object.ToString"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never), DoesNotReturn]
		public override string ToString() =>
			throw new NotSupportedException(
				"This instance doesn't support this member, " +
				"because this method will cause box and unbox operations, " +
				"which is invalid in ref structures.");

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		public Enumerator GetEnumerator() => _enumerator;
	}
}
