namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Defines a temporary type that records a pair of data for a bi-value oddagon.
/// </summary>
/// <param name="loopCells">Indicates the cells of the whole loop.</param>
/// <param name="extraCells">Indicates the extra cells.</param>
/// <param name="digitsMask">Indicates the mask of digits that the loop used.</param>
[TypeImpl(TypeImplFlags.Object_GetHashCode)]
public sealed partial class BivalueOddagonPattern(
	[Property, HashCodeMember] ref readonly CellMap loopCells,
	[Property] ref readonly CellMap extraCells,
	[Property] Mask digitsMask
) : Pattern
{
	/// <inheritdoc/>
	public override bool IsChainingCompatible => false;

	/// <inheritdoc/>
	public override PatternType Type => PatternType.BivalueOddagon;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out CellMap loopCells, out CellMap extraCells, out Mask digitsMask)
		=> (loopCells, extraCells, digitsMask) = (LoopCells, ExtraCells, DigitsMask);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Pattern? other)
		=> other is BivalueOddagonPattern comparer && LoopCells == comparer.LoopCells;

	/// <inheritdoc/>
	public override BivalueOddagonPattern Clone() => new(LoopCells, ExtraCells, DigitsMask);
}
