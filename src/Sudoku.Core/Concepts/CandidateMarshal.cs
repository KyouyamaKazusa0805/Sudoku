namespace Sudoku.Concepts;

/// <summary>
/// Represents methods that operates with <see cref="Candidate"/> values.
/// </summary>
/// <seealso cref="Candidate"/>
public static class CandidateMarshal
{
	/// <summary>
	/// Converts the specified <see cref="Candidate"/> into a singleton <see cref="CandidateMap"/> instance.
	/// </summary>
	/// <param name="this">The cell to be converted.</param>
	/// <returns>A <see cref="CandidateMap"/> instance, containing only one element of <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap AsCandidateMap(this Candidate @this) => [@this];
}
