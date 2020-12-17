using System.Runtime.CompilerServices;
using static Sudoku.Data.RegionLabel;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods for <see cref="RegionLabel"/>.
	/// </summary>
	/// <seealso cref="RegionLabel"/>
	public static class RegionLabelEx
	{
		/// <summary>
		/// The block table.
		/// </summary>
		private static readonly int[] BlockTable =
		{
			0, 0, 0, 1, 1, 1, 2, 2, 2,
			0, 0, 0, 1, 1, 1, 2, 2, 2,
			0, 0, 0, 1, 1, 1, 2, 2, 2,
			3, 3, 3, 4, 4, 4, 5, 5, 5,
			3, 3, 3, 4, 4, 4, 5, 5, 5,
			3, 3, 3, 4, 4, 4, 5, 5, 5,
			6, 6, 6, 7, 7, 7, 8, 8, 8,
			6, 6, 6, 7, 7, 7, 8, 8, 8,
			6, 6, 6, 7, 7, 7, 8, 8, 8
		};

		/// <summary>
		/// The row table.
		/// </summary>
		private static readonly int[] RowTable =
		{
			9, 9, 9, 9, 9, 9, 9, 9, 9,
			10, 10, 10, 10, 10, 10, 10, 10, 10,
			11, 11, 11, 11, 11, 11, 11, 11, 11,
			12, 12, 12, 12, 12, 12, 12, 12, 12,
			13, 13, 13, 13, 13, 13, 13, 13, 13,
			14, 14, 14, 14, 14, 14, 14, 14, 14,
			15, 15, 15, 15, 15, 15, 15, 15, 15,
			16, 16, 16, 16, 16, 16, 16, 16, 16,
			17, 17, 17, 17, 17, 17, 17, 17, 17
		};

		/// <summary>
		/// The column table.
		/// </summary>
		private static readonly int[] ColumnTable =
		{
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			18, 19, 20, 21, 22, 23, 24, 25, 26,
			18, 19, 20, 21, 22, 23, 24, 25, 26
		};


		/// <summary>
		/// Get the region index for the specified cell and the region type.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="@this">(<see langword="this"/> parameter) The label.</param>
		/// <returns>The region index (<c>0..27</c>).</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToRegion(this RegionLabel @this, int cell) =>
			(@this switch { Row => RowTable, Column => ColumnTable, Block => BlockTable })[cell];

#if DOUBLE_LAYERED_ASSUMPTION
		/// <summary>
		/// Get the specified region cause with the specified region label.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The region label.</param>
		/// <returns>The cause.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Cause GetRegionCause(this RegionLabel @this) =>
			@this switch
			{
				Row => Cause.RowHiddenSingle,
				Column => Cause.ColumnHiddenSingle,
				Block => Cause.BlockHiddenSingle
			};
#endif
	}
}
