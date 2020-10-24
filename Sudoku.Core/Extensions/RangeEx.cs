using System;
using System.Collections.Generic;
using Sudoku.DocComments;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Range"/>.
	/// </summary>
	/// <seealso cref="Range"/>
	public static partial class RangeEx
	{
		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		/// <param name="this">(<see langword="this"/> parameter) The range.</param>
		/// <exception cref="ArgumentException">
		/// Throws when the index is from end, or the start index is greater than end one.
		/// </exception>
		public static IEnumerator<int> GetEnumerator(this Range @this)
		{
			var ((sIsFromEnd, sValue), (eIsFromEnd, eValue)) = @this;
			_ = sIsFromEnd || eIsFromEnd ? throw new ArgumentException("The index should be from start.") : 0;
			_ = sValue > eValue ? throw new ArgumentException("The start index should be less than end index.") : 0;

			return new RangeIterator(sValue, eValue);
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this"/> parameter) The range.</param>
		/// <param name="start">(<see langword="out"/> parameter) The start index.</param>
		/// <param name="end">(<see langword="out"/> parameter) The end index.</param>
		public static void Deconstruct(this Range @this, out Index start, out Index end) =>
			(start, end) = (@this.Start, @this.End);
	}
}
