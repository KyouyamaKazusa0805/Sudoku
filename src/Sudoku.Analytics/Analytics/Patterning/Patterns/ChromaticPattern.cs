namespace Sudoku.Analytics.Patterning.Patterns;

/// <summary>
/// Represents a chromatic pattern.
/// </summary>
/// <param name="Block1Cells">Indicates the cells used in first block.</param>
/// <param name="Block2Cells">Indicates the cells used in second block.</param>
/// <param name="Block3Cells">Indicates the cells used in third block.</param>
/// <param name="Block4Cells">Indicates the cells used in fourth block.</param>
public readonly record struct ChromaticPattern(Cell[] Block1Cells, Cell[] Block2Cells, Cell[] Block3Cells, Cell[] Block4Cells);
