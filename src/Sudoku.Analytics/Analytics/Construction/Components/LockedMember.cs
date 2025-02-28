namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a locked member. This type is only used by searching for exocets.
/// </summary>
/// <param name="lockedCells">Indicates the locked cells.</param>
/// <param name="lockedBlock">Indicates the locked block.</param>
[TypeImpl(TypeImplFlags.Object_Equals | TypeImplFlags.Object_GetHashCode | TypeImplFlags.EqualityOperators)]
public sealed partial class LockedMember([Property, HashCodeMember] in CellMap lockedCells, [Property, HashCodeMember] House lockedBlock) :
	IComponent,
	IEquatable<LockedMember>,
	IEqualityOperators<LockedMember, LockedMember, bool>
{
	/// <inheritdoc/>
	ComponentType IComponent.Type => ComponentType.LockedMember;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap lockedCells, out House lockedBlock)
		=> (lockedCells, lockedBlock) = (LockedCells, LockedBlock);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] LockedMember? other)
		=> other is not null && LockedCells == other.LockedCells && LockedBlock == other.LockedBlock;
}
