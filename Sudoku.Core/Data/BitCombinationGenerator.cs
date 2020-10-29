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
	/// <remarks>
	/// You can use this struct like this:
	/// <code>
	/// foreach (short mask in new BitCombinationGenerator(9, 3))
	/// {
	///     // Do something to use the mask.
	/// }
	/// </code>
	/// </remarks>
	public readonly ref partial struct BitCombinationGenerator
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
		public BitCombinationGenerator(int bitCount, int oneCount) =>
			_enumerator = new(BitCount = bitCount, OneCount = oneCount);


		/// <summary>
		/// Indicates how many bits should be generated.
		/// </summary>
		public int BitCount { get; }

		/// <summary>
		/// Indicates how many <see langword="true"/> bits (1) are in
		/// the number.
		/// </summary>
		public int OneCount { get; }


		/// <inheritdoc/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override bool Equals(object? obj) => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="object.GetHashCode"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override int GetHashCode() => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="object.ToString"/>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DoesNotReturn]
		public override string ToString() => throw Throwings.RefStructNotSupported;

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		public Enumerator GetEnumerator() => _enumerator;
	}
}
