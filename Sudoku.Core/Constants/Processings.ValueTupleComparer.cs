using System;
using System.Collections.Generic;

namespace Sudoku.Constants
{
	partial class Processings
	{
		/// <summary>
		/// The inner comparer of <see cref="ValueTuple{T1, T2}"/> used in
		/// the field <see cref="IntersectionMaps"/>.
		/// </summary>
		/// <seealso cref="IntersectionMaps"/>
		private readonly struct ValueTupleComparer : IEqualityComparer<(byte _a, byte _b)>
		{
			/// <inheritdoc/>
			public bool Equals((byte _a, byte _b) x, (byte _a, byte _b) y) => x == y;

			/// <inheritdoc/>
			public int GetHashCode((byte _a, byte _b) obj) => obj._a << 5 | obj._b;
		}
	}
}
