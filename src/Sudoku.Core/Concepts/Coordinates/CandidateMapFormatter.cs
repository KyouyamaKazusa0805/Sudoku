namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Provides with a formatter object that convert the current <see cref="CandidateMap"/> instance
/// into a <see cref="string"/> representation equivalent to the current object.
/// </summary>
/// <param name="candidates">The candidates to be converted.</param>
/// <returns>The string result.</returns>
public delegate string CandidateMapFormatter(in CandidateMap candidates);
