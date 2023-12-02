using Sudoku.Concepts;

namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value via the specified <see cref="Chute"/> instance.
/// </summary>
/// <param name="chutes">A list of chutes.</param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="chutes"/>.</returns>
public delegate string ChuteNotationConverter(scoped ReadOnlySpan<Chute> chutes);
