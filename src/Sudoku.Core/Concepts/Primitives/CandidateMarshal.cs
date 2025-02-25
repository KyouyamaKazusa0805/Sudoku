namespace Sudoku.Concepts.Primitives;

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
#if CACHE_CANDIDATE_MAPS
	public static ref readonly CandidateMap AsCandidateMap(this Candidate @this) => ref CandidateMaps[@this];
#else
	public static CandidateMap AsCandidateMap(this Candidate @this) => [@this];
#endif

	/// <inheritdoc cref="AsCandidateMap(Span{Candidate})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap AsCandidateMap(this Candidate[] @this) => [.. @this];

	/// <summary>
	/// Converts the specified list of <see cref="Candidate"/> instances into a <see cref="CandidateMap"/> instance.
	/// </summary>
	/// <param name="this">The cells to be converted.</param>
	/// <returns>A <see cref="CandidateMap"/> instance, containing all elements come from <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap AsCandidateMap(this Span<Candidate> @this) => [.. @this];

	/// <inheritdoc cref="AsCandidateMap(Span{Candidate})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidateMap AsCandidateMap(this ReadOnlySpan<Candidate> @this) => [.. @this];
}
