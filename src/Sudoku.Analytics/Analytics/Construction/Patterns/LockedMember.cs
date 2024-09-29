namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents a locked member. This type is only used by searching for exocets.
/// </summary>
/// <param name="lockedCells">Indicates the locked cells.</param>
/// <param name="lockedBlock">Indicates the locked block.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class LockedMember(
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap lockedCells,
	[PrimaryConstructorParameter, HashCodeMember] House lockedBlock
) : IEquatable<LockedMember>, IEqualityOperators<LockedMember, LockedMember, bool>
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap lockedCells, out House lockedBlock)
		=> (lockedCells, lockedBlock) = (LockedCells, LockedBlock);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] LockedMember? other)
		=> other is not null && LockedCells == other.LockedCells && LockedBlock == other.LockedBlock;
}
