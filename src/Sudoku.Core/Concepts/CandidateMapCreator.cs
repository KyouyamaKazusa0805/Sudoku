namespace Sudoku.Concepts;

/// <summary>
/// Represents a type that has ability to create <see cref="CandidateMap"/> instances called by compiler.
/// For the users' aspect, we can just use collection expressions.
/// </summary>
/// <seealso cref="CandidateMap"/>
public static class CandidateMapCreator
{
	/// <summary>
	/// Creates a <see cref="CandidateMap"/> instance.
	/// </summary>
	/// <returns>A <see cref="CandidateMap"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap Create() => CandidateMap.Empty;

	/// <summary>
	/// Creates a <see cref="CandidateMap"/> instance via the specified candidates.
	/// </summary>
	/// <param name="candidates">The candidates.</param>
	/// <returns>A <see cref="CandidateMap"/> instance.</returns>
	public static CandidateMap Create(scoped ReadOnlySpan<Candidate> candidates)
	{
		var result = CandidateMap.Empty;
		foreach (var candidate in candidates)
		{
			result.Add(candidate);
		}
		return result;
	}
}
