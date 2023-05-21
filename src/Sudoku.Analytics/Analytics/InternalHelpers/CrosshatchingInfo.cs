namespace Sudoku.Analytics.InternalHelpers;

/// <summary>
/// Defines the target crosshatching information.
/// </summary>
/// <param name="BaseCells">The base cells to be used.</param>
/// <param name="EmptyCells">The empty cells in the final.</param>
/// <param name="ExcludedCells">The excluded cells to be used.</param>
internal readonly record struct CrosshatchingInfo(scoped in CellMap BaseCells, scoped in CellMap EmptyCells, scoped in CellMap ExcludedCells);
