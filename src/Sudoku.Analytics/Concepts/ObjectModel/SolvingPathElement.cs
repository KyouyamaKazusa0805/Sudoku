namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents an element that is the unit data structure of a <see cref="SolvingPath"/>.
/// </summary>
/// <param name="SteppingGrid">Indicates the stepping grid.</param>
/// <param name="Step">Indicates the current step.</param>
/// <seealso cref="SolvingPath"/>
public readonly record struct SolvingPathElement(scoped ref readonly Grid SteppingGrid, Step Step);
