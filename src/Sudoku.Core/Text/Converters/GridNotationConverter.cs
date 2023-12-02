using Sudoku.Concepts;

namespace Sudoku.Text.Converters;

/// <summary>
/// Defines a method that converts a <see cref="Grid"/> instance, converting it into an equivalent <see cref="string"/> value.
/// </summary>
/// <param name="Grid">The grid.</param>
/// <returns>The equivalent <see cref="string"/> result.</returns>
public delegate string GridNotationConverter(scoped ref readonly Grid Grid);
