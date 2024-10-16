namespace Sudoku.Analytics.Construction.Patterns;

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
[TypeImpl(TypeImplFlags.Object_GetHashCode | TypeImplFlags.ComparisonOperators)]
public sealed partial class AlmostHiddenSetPattern(
	[Property, HashCodeMember] ref readonly CellMap cells,
	[Property, HashCodeMember] House house,
	[Property, HashCodeMember] Mask digitsMask,
	[Property, HashCodeMember] Mask subsetDigitsMask,
	[Property, HashCodeMember] ref readonly CandidateMap candidatesCanFormWeakLink
) :
	Pattern,
	IComparable<AlmostHiddenSetPattern>,
	IComparisonOperators<AlmostHiddenSetPattern, AlmostHiddenSetPattern, bool>
{
	/// <inheritdoc/>
	public override bool IsChainingCompatible => true;


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Pattern? other)
		=> other is AlmostHiddenSetPattern comparer
		&& Cells == comparer.Cells && House == comparer.House
		&& DigitsMask == comparer.DigitsMask && SubsetDigitsMask == comparer.SubsetDigitsMask;

	/// <inheritdoc/>
	public int CompareTo(AlmostHiddenSetPattern? other)
		=> other is null
			? 1
			: Cells.CompareTo(other.Cells) is var r1 and not 0
				? r1
				: DigitsMask.CompareTo(other.DigitsMask) is var r2 and not 0
					? r2
					: SubsetDigitsMask.CompareTo(other.SubsetDigitsMask) is var r3 and not 0 ? r3 : 0;

	/// <inheritdoc/>
	public override AlmostHiddenSetPattern Clone() => new(Cells, House, DigitsMask, SubsetDigitsMask, CandidatesCanFormWeakLink);


	/// <inheritdoc/>
	static bool IEqualityOperators<AlmostHiddenSetPattern, AlmostHiddenSetPattern, bool>.operator ==(AlmostHiddenSetPattern? left, AlmostHiddenSetPattern? right)
		=> left == right;

	/// <inheritdoc/>
	static bool IEqualityOperators<AlmostHiddenSetPattern, AlmostHiddenSetPattern, bool>.operator !=(AlmostHiddenSetPattern? left, AlmostHiddenSetPattern? right)
		=> left != right;
}
