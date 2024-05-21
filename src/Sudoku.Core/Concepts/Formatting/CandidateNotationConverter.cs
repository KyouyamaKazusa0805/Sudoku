namespace Sudoku.Concepts.Formatting;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value via the specified <see cref="CandidateMap"/> instance.
/// </summary>
/// <param name="candidates">A list of candidates.</param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="candidates"/>.</returns>
public delegate string CandidateNotationConverter(params CandidateMap candidates);
