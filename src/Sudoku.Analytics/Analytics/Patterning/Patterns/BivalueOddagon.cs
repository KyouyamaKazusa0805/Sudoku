namespace Sudoku.Concepts;

/// <summary>
/// Defines a temporary type that records a pair of data for a bi-value oddagon.
/// </summary>
/// <param name="LoopCells">Indicates the cells of the whole loop.</param>
/// <param name="ExtraCells">Indicates the extra cells.</param>
/// <param name="DigitsMask">Indicates the mask of digits that the loop used.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode)]
public readonly partial record struct BivalueOddagon(
	[property: HashCodeMember] ref readonly CellMap LoopCells,
	ref readonly CellMap ExtraCells,
	Mask DigitsMask
)
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(BivalueOddagon other) => LoopCells == other.LoopCells;
}
