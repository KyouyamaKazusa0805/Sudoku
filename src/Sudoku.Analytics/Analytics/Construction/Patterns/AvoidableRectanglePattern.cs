namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents an avoidable rectangle.
/// </summary>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the digits used.</param>
/// <param name="valuesMap">Indicates the value cells.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode)]
public sealed partial class AvoidableRectanglePattern(
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap cells,
	[PrimaryConstructorParameter, HashCodeMember] Mask digitsMask,
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap valuesMap
) : Pattern
{
	/// <inheritdoc/>
	public override bool IsChainingCompatible => true;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Pattern? other)
		=> other is AvoidableRectanglePattern comparer
		&& Cells == comparer.Cells && DigitsMask == comparer.DigitsMask && ValuesMap == comparer.ValuesMap;

	/// <inheritdoc/>
	public override AvoidableRectanglePattern Clone() => new(Cells, DigitsMask, ValuesMap);
}
