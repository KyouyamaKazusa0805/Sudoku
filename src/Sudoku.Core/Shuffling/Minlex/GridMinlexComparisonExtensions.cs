namespace Sudoku.Shuffling.Minlex;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/> for minlex.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridMinlexComparisonExtensions
{
	/// <summary>
	/// Determine whether the specified <see cref="Grid"/> instance hold the same values as the current instance,
	/// by using the specified comparison type.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="other">The instance to compare.</param>
	/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="comparisonType"/> is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Equals(this ref readonly Grid @this, ref readonly Grid other, GridComparison comparisonType)
		=> comparisonType switch
		{
			GridComparison.Default => @this.Equals(in other),
			GridComparison.IncludingTransforms => @this.GetMinLexGrid() == other.GetMinLexGrid(),
			_ => throw new ArgumentOutOfRangeException(nameof(comparisonType))
		};

	/// <inheritdoc cref="Grid.GetHashCode"/>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="comparisonType">
	/// Indicates the comparison type that specifies the target grid to be calculated its hash code.
	/// </param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparisonType"/> isn't defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetHashCode(this ref readonly Grid @this, GridComparison comparisonType)
	{
		var grid = comparisonType switch
		{
			GridComparison.Default => @this,
			GridComparison.IncludingTransforms => @this.GetMinLexGrid(),
			_ => throw new ArgumentOutOfRangeException(nameof(comparisonType))
		};
		return grid.GetHashCode();
	}

	/// <summary>
	/// Compares the current instance with another object of the same type and returns an integer
	/// that indicates whether the current instance precedes, follows or occurs in the same position in the sort order as the other object.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="other">The other object to be compared.</param>
	/// <param name="comparisonType">The comparison type to be used.</param>
	/// <returns>A value that indicates the relative order of the objects being compared.</returns>
	/// <exception cref="InvalidOperationException">Throws when one of the grids to be compared is a Sukaku puzzle.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="comparisonType"/> is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CompareTo(this ref readonly Grid @this, ref readonly Grid other, GridComparison comparisonType)
		=> (@this.PuzzleType, other.PuzzleType) switch
		{
			(not SudokuType.Sukaku, not SudokuType.Sukaku) => comparisonType switch
			{
				GridComparison.Default => @this.ToString("#").CompareTo(other.ToString("#")),
				GridComparison.IncludingTransforms => @this.GetMinLexGrid().ToString("#").CompareTo(other.GetMinLexGrid().ToString("#")),
				_ => throw new ArgumentOutOfRangeException(nameof(comparisonType))
			},
			_ => throw new InvalidOperationException(SR.ExceptionMessage("ComparableGridMustBeStandard"))
		};
}
