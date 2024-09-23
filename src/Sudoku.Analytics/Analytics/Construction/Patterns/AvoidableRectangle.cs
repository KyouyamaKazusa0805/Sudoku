namespace Sudoku.Analytics.Construction.Patterns;

/// <summary>
/// Represents an avoidable rectangle.
/// </summary>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the digits used.</param>
/// <param name="valuesMap">Indicates the value cells.</param>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public sealed partial class AvoidableRectangle(
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap cells,
	[PrimaryConstructorParameter, HashCodeMember] Mask digitsMask,
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap valuesMap
) : IEquatable<AvoidableRectangle>, IEqualityOperators<AvoidableRectangle, AvoidableRectangle, bool>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] AvoidableRectangle? other)
		=> other is not null && Cells == other.Cells && DigitsMask == other.DigitsMask && ValuesMap == other.ValuesMap;
}
