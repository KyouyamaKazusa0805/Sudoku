namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Represents a part of coordinate, described as two arrays of row and column indices ranging 0..9.
/// </summary>
/// <param name="Rows">Indicates row indices.</param>
/// <param name="Columns">Indicates column indices.</param>
public readonly record struct CoordinateSplit(RowIndex[] Rows, ColumnIndex[] Columns);
