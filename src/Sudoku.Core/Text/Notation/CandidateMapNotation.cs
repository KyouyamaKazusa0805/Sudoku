namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a notation that represents for a <see cref="CandidateMap"/> instance.
/// </summary>
/// <seealso cref="CandidateMap"/>
public sealed class CandidateMapNotation : INotation<CandidateMapNotation, CandidateMap, CandidateMapNotationKind>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap Parse(string text, CandidateMapNotationKind notation)
		=> notation switch
		{
			CandidateMapNotationKind.RxCy => CandidateNotation.ParseCollection(text, CandidateNotationKind.RxCy),
			CandidateMapNotationKind.K9 => CandidateNotation.ParseCollection(text, CandidateNotationKind.K9),
			_ => throw new ArgumentOutOfRangeException(nameof(notation)),
		};

	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(scoped in CandidateMap value, CandidateMapNotationKind notation)
		=> notation switch
		{
			CandidateMapNotationKind.RxCy => CandidateNotation.ToCollectionString(value, CandidateNotationKind.RxCy),
			CandidateMapNotationKind.K9 => CandidateNotation.ToCollectionString(value, CandidateNotationKind.K9),
			_ => throw new ArgumentOutOfRangeException(nameof(notation)),
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static string INotation<CandidateMapNotation, CandidateMap, CandidateMapNotationKind>.ToString(CandidateMap value, CandidateMapNotationKind notation)
		=> ToString(value, notation);
}
