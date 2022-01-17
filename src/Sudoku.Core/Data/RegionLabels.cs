namespace Sudoku.Data;

/// <summary>
/// Defines the <see langword="static"/> data for <see cref="RegionLabel"/> instances.
/// </summary>
/// <completionlist cref="RegionLabel"/>
public static class RegionLabels
{
	/// <summary>
	/// Indicates the region is the block.
	/// </summary>
	public static readonly RegionLabel Block = new(0);

	/// <summary>
	/// Indicates the region is the row.
	/// </summary>
	public static readonly RegionLabel Row = new(1);

	/// <summary>
	/// Indicates the region is the column.
	/// </summary>
	public static readonly RegionLabel Column = new(2);
}
