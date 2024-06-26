namespace Sudoku.Concepts;

/// <summary>
/// Represents a pair of candidates as start and end candidates in a blossom loop branch.
/// </summary>
/// <param name="Start">Indicates the start candidate.</param>
/// <param name="End">Indicates the end candidate.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString | TypeImplFlag.ComparisonOperators)]
public readonly partial record struct BlossomLoopEntry(
	[property: HashCodeMember] Candidate Start,
	[property: HashCodeMember] Candidate End
) : IComparable<BlossomLoopEntry>, IDictionaryEntry<BlossomLoopEntry>, IFormattable
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(BlossomLoopEntry other)
		=> Start.CompareTo(other.Start) is var r1 and not 0
			? r1
			: End.CompareTo(other.End) is var r2 and not 0 ? r2 : 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(BlossomLoopEntry other) => (Start, End) == (other.Start, other.End);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetConverter(formatProvider);
		var startStr = converter.CandidateConverter(Start);
		var endStr = converter.CandidateConverter(End);
		return $"{startStr} -> {endStr}";
	}

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
