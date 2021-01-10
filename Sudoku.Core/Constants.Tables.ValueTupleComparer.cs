using System;
using System.Collections.Generic;

namespace Sudoku
{
	partial class Constants
	{
		partial class Tables
		{
			/// <summary>
			/// The inner comparer of <see cref="ValueTuple{T1, T2}"/> used in
			/// the field <see cref="IntersectionMaps"/>.
			/// </summary>
			/// <seealso cref="IntersectionMaps"/>
			private readonly struct ValueTupleComparer : IEqualityComparer<(byte Value1, byte Value2)>
			{
				/// <inheritdoc/>
				public bool Equals((byte Value1, byte Value2) x, (byte Value1, byte Value2) y) => x == y;

				/// <inheritdoc/>
				public int GetHashCode((byte Value1, byte Value2) obj) => obj.Value1 << 5 | obj.Value2;
			}
		}
	}
}
