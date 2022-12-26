namespace Sudoku.Drawing;

/// <summary>
/// Defines a type that holds a list of methods to create <see cref="ISudokuPainter"/> using the specified rule.
/// </summary>
/// <param name="base">The base <see cref="ISudokuPainter"/> instance.</param>
/// <returns><see cref="ISudokuPainter"/> instance returned.</returns>
public delegate ISudokuPainter SudokuPainterPropertySetter(ISudokuPainter @base);
