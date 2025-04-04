namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents a pattern for multi-sector locked sets.
/// </summary>
/// <param name="map">The map of cells used.</param>
/// <param name="rowCount">The number of rows used.</param>
/// <param name="columnCount">The number of columns used.</param>
[TypeImpl(TypeImplFlags.Object_GetHashCode)]
public sealed partial class MultisectorLockedSetPattern(
	[Property, HashCodeMember] in CellMap map,
	[Property] RowIndex rowCount,
	[Property] ColumnIndex columnCount
) : Pattern
{
	/// <inheritdoc/>
	public override bool IsChainingCompatible => false;

	/// <inheritdoc/>
	public override PatternType Type => PatternType.MultisectorLockedSet;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap map, out RowIndex rowCount, out ColumnIndex columnCount)
		=> (map, rowCount, columnCount) = (Map, RowCount, ColumnCount);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Pattern? other)
		=> other is MultisectorLockedSetPattern comparer && Map == comparer.Map;

	/// <inheritdoc/>
	public override MultisectorLockedSetPattern Clone() => new(Map, RowCount, ColumnCount);
}
