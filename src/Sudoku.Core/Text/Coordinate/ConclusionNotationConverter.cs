using Sudoku.Analytics;

namespace Sudoku.Text.Coordinate;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value via the specified <see cref="Conclusion"/> instance.
/// </summary>
/// <param name="conclusions">A list of conjugate pairs.</param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="conclusions"/>.</returns>
public delegate string ConclusionNotationConverter(scoped ReadOnlySpan<Conclusion> conclusions);
