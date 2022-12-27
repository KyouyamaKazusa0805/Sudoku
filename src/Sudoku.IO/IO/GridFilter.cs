namespace Sudoku.IO;

/// <summary>
/// Defines a type that holds a method to filter <see cref="Grid"/> instances.
/// </summary>
/// <param name="grid">The grid to be checked.</param>
/// <returns>A <see cref="bool"/> result indicating whether the <see cref="Grid"/> can be passed.</returns>
public delegate bool GridFilter(scoped in Grid grid);
