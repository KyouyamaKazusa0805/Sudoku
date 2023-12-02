using Sudoku.Concepts;

namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value via the specified <see cref="CellMap"/> instance.
/// </summary>
/// <param name="cells">A list of cells.</param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="cells"/>.</returns>
public delegate string CellNotationConverter(scoped ref readonly CellMap cells);
