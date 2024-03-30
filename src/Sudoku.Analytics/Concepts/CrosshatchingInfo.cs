namespace Sudoku.Concepts;

/// <summary>
/// Defines the target crosshatching information.
/// </summary>
/// <param name="BaseCells">The base cells to be used.</param>
/// <param name="EmptyCells">The empty cells in the final.</param>
/// <param name="ExcludedCells">The excluded cells to be used.</param>
public sealed record CrosshatchingInfo(scoped ref readonly CellMap BaseCells, scoped ref readonly CellMap EmptyCells, scoped ref readonly CellMap ExcludedCells);
