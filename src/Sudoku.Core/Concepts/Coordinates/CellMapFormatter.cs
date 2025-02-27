namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Provides with a formatter object that convert the current <see cref="CellMap"/> instance
/// into a <see cref="string"/> representation equivalent to the current object.
/// </summary>
/// <param name="cells">The cells to be converted.</param>
/// <returns>The string result.</returns>
public delegate string CellMapFormatter(in CellMap cells);
