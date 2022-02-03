namespace Sudoku;

/// <summary>
/// Provides extension methods on <see cref="Region"/>.
/// </summary>
/// <seealso cref="Region"/>
public static class RegionExtensions
{
	/// <summary>
	/// The tables.
	/// </summary>
	[SuppressMessage("Style", "IDE0055:Fix formatting", Justification = "<Pending>")]
	private static readonly int[] BlockTable =
	{
		 0,  0,  0,  1,  1,  1,  2,  2,  2,
		 0,  0,  0,  1,  1,  1,  2,  2,  2,
		 0,  0,  0,  1,  1,  1,  2,  2,  2,
		 3,  3,  3,  4,  4,  4,  5,  5,  5,
		 3,  3,  3,  4,  4,  4,  5,  5,  5,
		 3,  3,  3,  4,  4,  4,  5,  5,  5,
		 6,  6,  6,  7,  7,  7,  8,  8,  8,
		 6,  6,  6,  7,  7,  7,  8,  8,  8,
		 6,  6,  6,  7,  7,  7,  8,  8,  8
	}, RowTable =
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
	}, ColumnTable =
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
	/// Gets the row, column and block value and copies to the specified array that represents by a pointer
	/// of 3 elements, where the first element stores the block index, second element stores the row index
	/// and the third element stores the column index.
	/// </summary>
	/// <param name="cell">The cell. The available values must be between 0 and 80.</param>
	/// <param name="ptr">The specified array that represents by a pointer of 3 elements.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe void RopyRegionInfo(this int cell, int* ptr)
	{
		ptr[0] = BlockTable[cell];
		ptr[1] = RowTable[cell];
		ptr[2] = ColumnTable[cell];
	}

	/// <summary>
	/// Get the region index (0..27 for block 1-9, row 1-9 and column 1-9)
	/// for the specified cell and the region type.
	/// </summary>
	/// <param name="cell">The cell. The available values must be between 0 and 80.</param>
	/// <param name="region">The region type.</param>
	/// <returns>The region index. The return value must be between 0 and 26.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToRegionIndex(this byte cell, Region region) =>
		region switch
		{
			Region.Block => BlockTable[cell],
			Region.Row => RowTable[cell],
			Region.Column => ColumnTable[cell]
		};

	/// <inheritdoc cref="ToRegionIndex(byte, Region)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToRegionIndex(this int cell, Region region) =>
		region switch
		{
			Region.Block => BlockTable[cell],
			Region.Row => RowTable[cell],
			Region.Column => ColumnTable[cell]
		};

	/// <summary>
	/// Get the region type for the specified region index.
	/// </summary>
	/// <param name="regionIndex">The region index.</param>
	/// <returns>
	/// The region type. The possible return values are:
	/// <list type="table">
	/// <listheader>
	/// <term>Range for the region index</term>
	/// <description>Return value</description>
	/// </listheader>
	/// <item>
	/// <term><paramref name="regionIndex"/> is <![CDATA[>= 0 and < 9]]></term>
	/// <description><see cref="Region.Block"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="regionIndex"/> is <![CDATA[>= 9 and < 18]]></term>
	/// <description><see cref="Region.Row"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="regionIndex"/> is <![CDATA[>= 18 and < 27]]></term>
	/// <description><see cref="Region.Column"/></description>
	/// </item>
	/// </list>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Region ToRegion(this int regionIndex) => (Region)/*(byte)*/(regionIndex / 9);
}
