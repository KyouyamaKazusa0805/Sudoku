namespace Sudoku.Analytics.Patterns;

/// <summary>
/// Represents for a data set that describes the complete information about a guardian technique.
/// </summary>
/// <param name="LoopCells">Indicates the cells used in this whole guardian loop.</param>
/// <param name="Guardians">Indicates the extra cells that is used as guardians.</param>
/// <param name="Digit">Indicates the digit used.</param>
public readonly record struct Guardian(scoped in CellMap LoopCells, scoped in CellMap Guardians, int Digit);
