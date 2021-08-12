namespace Sudoku.Data;

/// <summary>
/// Provides extension methods on conversions between a cell and a region.
/// </summary>
public static class RegionCalculator
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
		 9,  9,  9,  9,  9,  9,  9,  9,  9,
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
	/// <param name="this">The cell.</param>
	/// <param name="label">The label to represent a region type.</param>
	/// <returns>The region index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToRegion(this int @this, RegionLabel label) => (
		label switch
		{
			RegionLabel.Row => RowTable,
			RegionLabel.Column => ColumnTable,
			RegionLabel.Block => BlockTable
		}
	)[@this];
}
