namespace Sudoku.Concepts;

/// <summary>
/// Represents a list of methods handling with <see cref="HouseMask"/> instances.
/// </summary>
/// <seealso cref="HouseMask"/>
public static class HouseMaskOperations
{
	/// <summary>
	/// Indicates the mask that means all blocks.
	/// </summary>
	public const HouseMask AllBlocksMask = Grid.MaxCandidatesMask;

	/// <summary>
	/// Indicates the mask that means all rows.
	/// </summary>
	public const HouseMask AllRowsMask = Grid.MaxCandidatesMask << 9;

	/// <summary>
	/// Indicates the mask that means all columns.
	/// </summary>
	public const HouseMask AllColumnsMask = Grid.MaxCandidatesMask << 18;

	/// <summary>
	/// Indicates the mask that means all houses.
	/// </summary>
	public const HouseMask AllHousesMask = (1 << 27) - 1;


	/// <summary>
	/// Creates for a <see cref="HouseMask"/> instance via the specified houses.
	/// </summary>
	/// <param name="houses">The houses.</param>
	/// <returns>A <see cref="HouseMask"/> instance.</returns>
	public static HouseMask Create(ReadOnlySpan<House> houses)
	{
		var result = 0;
		foreach (var house in houses)
		{
			result |= 1 << house;
		}
		return result;
	}

	/// <summary>
	/// Try to split mask into three parts.
	/// </summary>
	/// <param name="this">The mask to be split.</param>
	/// <returns>The mask split.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Mask Block, Mask Row, Mask Column) SplitMask(this HouseMask @this)
	{
		var blockMask = (Mask)(@this & Grid.MaxCandidatesMask);
		var rowMask = (Mask)(@this >> 9 & Grid.MaxCandidatesMask);
		var columnMask = (Mask)(@this >> 18 & Grid.MaxCandidatesMask);
		return (blockMask, rowMask, columnMask);
	}
}
