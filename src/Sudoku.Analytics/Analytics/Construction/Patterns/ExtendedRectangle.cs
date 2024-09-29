namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents an extended rectangle pattern.
/// </summary>
/// <param name="isFat">Indicates whether the pattern is fat.</param>
/// <param name="patternCells">Indicates the cells used.</param>
/// <param name="pairCells">Indicates a list of pairs of cells used.</param>
/// <param name="size">Indicates the size of the pattern.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class ExtendedRectangle(
	[PrimaryConstructorParameter] bool isFat,
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap patternCells,
	[PrimaryConstructorParameter] (Cell Left, Cell Right)[] pairCells,
	[PrimaryConstructorParameter] int size
) : IEquatable<ExtendedRectangle>, IEqualityOperators<ExtendedRectangle, ExtendedRectangle, bool>
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out bool isFat, out CellMap patternCells, out (Cell Left, Cell Right)[] pairCells, out int size)
		=> (isFat, patternCells, pairCells, size) = (IsFat, PatternCells, PairCells, Size);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] ExtendedRectangle? other) => other is not null && PatternCells == other.PatternCells;
}
