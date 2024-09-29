namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents a pattern for multi-sector locked sets.
/// </summary>
/// <param name="map">The map of cells used.</param>
/// <param name="rowCount">The number of rows used.</param>
/// <param name="columnCount">The number of columns used.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class MultisectorLockedSet(
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap map,
	[PrimaryConstructorParameter/*, HashCodeMember*/] int rowCount,
	[PrimaryConstructorParameter/*, HashCodeMember*/] int columnCount
) : IEquatable<MultisectorLockedSet>, IEqualityOperators<MultisectorLockedSet, MultisectorLockedSet, bool>
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap map, out int rowCount, out int columnCount)
		=> (map, rowCount, columnCount) = (Map, RowCount, ColumnCount);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] MultisectorLockedSet? other)
		=> other is not null && Map == other.Map/* && RowCount == other.RowCount && ColumnCount == other.ColumnCount*/;
}
