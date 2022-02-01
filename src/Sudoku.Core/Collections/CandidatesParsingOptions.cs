namespace Sudoku.Collections;

/// <summary>
/// Encapsulates an option that is specified to parse the <see cref="Candidates"/> instance.
/// </summary>
/// <seealso cref="Candidates"/>
[Flags]
public enum CandidatesParsingOptions : byte
{
	/// <summary>
	/// Indicates the short form, such as <c>312</c>.
	/// </summary>
	ShortForm = 1,

	/// <summary>
	/// Indicates the bracket form, such as <c>{ r1c1, r3c3 }(12)</c>.
	/// </summary>
	BracketForm = 2,

	/// <summary>
	/// Indicates the prepositional form, such as <c>12{ r1c1, r3c3 }</c>.
	/// </summary>
	PrepositionalForm = 4
}
