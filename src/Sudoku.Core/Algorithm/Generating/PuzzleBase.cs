namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a base type for puzzle generated, need creating a data structure to store the details for the generated puzzle.
/// </summary>
/// <param name="Result">Indicates the generation result.</param>
/// <param name="Puzzle">
/// Indicates the puzzle just created. If the value <see cref="Result"/> returns a value
/// not <see cref="GeneratingResult.Success"/>, the value will always be <see cref="Grid.Undefined"/>.
/// </param>
/// <seealso cref="Result"/>
/// <seealso cref="GeneratingResult.Success"/>
/// <seealso cref="Grid.Undefined"/>
public abstract record PuzzleBase(GeneratingResult Result, scoped ref readonly Grid Puzzle);
