namespace Sudoku.Concepts;

/// <summary>
/// Represents a type that supports comparison on its internal value.
/// </summary>
/// <param name="comparisonType">Indicates the comparison type.</param>
public sealed class GridComparer(GridComparison comparisonType) : IComparer<Grid>
{
	/// <summary>
	/// Indicates an instance that supports default comparison.
	/// </summary>
	public static readonly GridComparer Default = new(GridComparison.Default);

	/// <summary>
	/// Indicates an instance that supports transforming comparison.
	/// </summary>
	public static readonly GridComparer IncludingTransforms = new(GridComparison.IncludingTransforms);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Compare(Grid x, Grid y) => x.CompareTo(in y, comparisonType);
}
