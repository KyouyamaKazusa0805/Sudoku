using System.SourceGeneration;
using System.Runtime.CompilerServices;

namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a notation that represents for a <see cref="CandidateMap"/> instance.
/// </summary>
/// <seealso cref="CandidateMap"/>
public sealed partial class CandidateMapNotation : INotation<CandidateMapNotation, CandidateMap, CandidateMapNotation.Kind>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap Parse(string text, Kind notation)
		=> notation switch
		{
			Kind.RxCy => CandidateNotation.ParseCollection(text, CandidateNotation.Kind.RxCy),
			Kind.K9 => CandidateNotation.ParseCollection(text, CandidateNotation.Kind.K9),
			_ => throw new ArgumentOutOfRangeException(nameof(notation)),
		};

	/// <inheritdoc cref="INotation{TSelf, TElement, TConceptKindPresenter}.ToString(TElement, TConceptKindPresenter)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(INotation<,,>))]
	public static string ToString(scoped in CandidateMap value, Kind notation)
		=> notation switch
		{
			Kind.RxCy => CandidateNotation.ToCollectionString(value, CandidateNotation.Kind.RxCy),
			Kind.K9 => CandidateNotation.ToCollectionString(value, CandidateNotation.Kind.K9),
			_ => throw new ArgumentOutOfRangeException(nameof(notation)),
		};
}
