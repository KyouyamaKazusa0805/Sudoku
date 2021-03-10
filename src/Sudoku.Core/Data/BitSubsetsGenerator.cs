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
	[DisableParameterlessConstructor]
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode() => 0;

		/// <inheritdoc cref="object.ToString"/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToString() => nameof(BitSubsetsGenerator);

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		public Enumerator GetEnumerator() => _enumerator;
	}
}
