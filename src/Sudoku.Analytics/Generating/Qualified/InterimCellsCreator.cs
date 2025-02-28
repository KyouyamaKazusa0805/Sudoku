namespace Sudoku.Generating.Qualified;

/// <summary>
/// Defines a type that creates a <see cref="CellMap"/> instance as interim cells used in generating operation.
/// </summary>
/// <param name="grid">The grid to be used.</param>
/// <param name="step">The step.</param>
/// <returns>A <see cref="CellMap"/> indicating the result.</returns>
public delegate CellMap InterimCellsCreator(in Grid grid, Step step);
