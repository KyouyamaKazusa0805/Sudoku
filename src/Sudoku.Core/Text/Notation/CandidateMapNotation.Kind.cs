using Sudoku.Concepts;

namespace Sudoku.Text.Notation;

partial class CandidateMapNotation
{
	/// <summary>
	/// Represents a kind of output text mode for <see cref="CandidateMap"/> instances.
	/// </summary>
	/// <seealso cref="CandidateMap"/>
	public enum Kind
	{
		/// <inheritdoc cref="CandidateNotation.Kind.RxCy"/>
		RxCy,

		/// <inheritdoc cref="CandidateNotation.Kind.K9"/>
		K9
	}
}
