namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Defines a temporary type that records a pair of data for a bi-value oddagon.
/// </summary>
/// <param name="loopCells">Indicates the cells of the whole loop.</param>
/// <param name="extraCells">Indicates the extra cells.</param>
/// <param name="digitsMask">Indicates the mask of digits that the loop used.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class BivalueOddagon(
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap loopCells,
	[PrimaryConstructorParameter] ref readonly CellMap extraCells,
	[PrimaryConstructorParameter] Mask digitsMask
) : IEquatable<BivalueOddagon>, IEqualityOperators<BivalueOddagon, BivalueOddagon, bool>
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap loopCells, out CellMap extraCells, out Mask digitsMask)
		=> (loopCells, extraCells, digitsMask) = (LoopCells, ExtraCells, DigitsMask);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] BivalueOddagon? other) => other is not null && LoopCells == other.LoopCells;
}
