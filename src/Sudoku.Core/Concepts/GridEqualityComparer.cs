namespace Sudoku.Concepts;

/// <summary>
/// Represents a type that supports equality comparison on both hash code and equality checking.
/// </summary>
/// <param name="comparisonType">Indicates the comparison type.</param>
public sealed class GridEqualityComparer(GridComparison comparisonType) : IEqualityComparer<Grid>
{
	/// <summary>
	/// Indicates an instance that supports default comparison.
	/// </summary>
	public static readonly GridEqualityComparer Default = new(GridComparison.Default);

	/// <summary>
	/// Indicates an instance that supports transforming comparison.
	/// </summary>
	public static readonly GridEqualityComparer IncludingTransforms = new(GridComparison.IncludingTransforms);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Grid x, Grid y) => x.Equals(in y, comparisonType);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode(Grid obj) => obj.GetHashCode(comparisonType);
}
