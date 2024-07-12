namespace Sudoku.Concepts;

/// <summary>
/// Represents an extended rectangle pattern.
/// </summary>
/// <param name="IsFat">Indicates whether the pattern is fat.</param>
/// <param name="PatternCells">Indicates the cells used.</param>
/// <param name="PairCells">Indicates a list of pairs of cells used.</param>
/// <param name="Size">Indicates the size of the pattern.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode)]
public readonly partial record struct ExtendedRectangle(
	bool IsFat,
	[property: HashCodeMember] ref readonly CellMap PatternCells,
	(Cell Left, Cell Right)[] PairCells,
	int Size
)
{
	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ref readonly ExtendedRectangle other) => PatternCells == other.PatternCells;
}
