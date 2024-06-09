namespace Sudoku.Concepts;

/// <summary>
/// Defines a data pattern that describes an AHS.
/// </summary>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="house">Indicates the house used.</param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="subsetDigitsMask">Indicates the mask of subset digits used.</param>
/// <param name="candidatesCanFormWeakLink">Indicates all candidates that can be used as weak links.</param>
/// <remarks>
/// An <b>Almost Hidden Set</b> is a sudoku concept, which describes a case that
/// <c>n</c> digits are only appeared inside <c>(n + 1)</c> cells in a house.
/// </remarks>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.AllOperators)]
public sealed partial class AlmostHiddenSet(
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CellMap cells,
	[PrimaryConstructorParameter, HashCodeMember] House house,
	[PrimaryConstructorParameter, HashCodeMember] Mask digitsMask,
	[PrimaryConstructorParameter, HashCodeMember] Mask subsetDigitsMask,
	[PrimaryConstructorParameter, HashCodeMember] ref readonly CandidateMap candidatesCanFormWeakLink
) :
	IComparable<AlmostHiddenSet>,
	IComparisonOperators<AlmostHiddenSet, AlmostHiddenSet, bool>,
	IEquatable<AlmostHiddenSet>,
	IEqualityOperators<AlmostHiddenSet, AlmostHiddenSet, bool>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] AlmostHiddenSet? other)
		=> other is not null
		&& Cells == other.Cells && House == other.House
		&& DigitsMask == other.DigitsMask && SubsetDigitsMask == other.SubsetDigitsMask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(AlmostHiddenSet? other)
		=> other is null
			? 1
			: Cells.CompareTo(other.Cells) is var r1 and not 0
				? r1
				: DigitsMask.CompareTo(other.DigitsMask) is var r2 and not 0
					? r2
					: SubsetDigitsMask.CompareTo(other.SubsetDigitsMask) is var r3 and not 0 ? r3 : 0;
}
