namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a kind of output text mode for <see cref="CandidateMap"/> instances.
/// </summary>
/// <seealso cref="CandidateMap"/>
public enum CandidateMapNotationKind
{
	/// <inheritdoc cref="CandidateNotationKind.RxCy"/>
	RxCy,

	/// <inheritdoc cref="CandidateNotationKind.K9"/>
	K9
}
