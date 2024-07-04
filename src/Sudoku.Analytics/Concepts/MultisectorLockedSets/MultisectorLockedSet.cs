namespace Sudoku.Concepts;

/// <summary>
/// Represents a pattern for multi-sector locked sets.
/// </summary>
/// <param name="Map">The map of cells used.</param>
/// <param name="RowCount">The number of rows used.</param>
/// <param name="ColumnCount">The number of columns used.</param>
public readonly record struct MultisectorLockedSet(ref readonly CellMap Map, int RowCount, int ColumnCount);
