using Sudoku.Concepts;

namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value
/// via the specified <see cref="IntersectionBase"/> and <see cref="IntersectionResult"/> instance.
/// </summary>
/// <param name="intersections">A list of intersections.</param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="intersections"/>.</returns>
public delegate string IntersectionNotationConverter(scoped ReadOnlySpan<(IntersectionBase Base, IntersectionResult Result)> intersections);
