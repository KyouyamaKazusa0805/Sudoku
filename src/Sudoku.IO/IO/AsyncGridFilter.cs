namespace Sudoku.IO;

/// <summary>
/// Defines a type that holds a method to filter <see cref="Grid"/> instances.
/// </summary>
/// <param name="grid">The grid to be checked.</param>
/// <param name="cancellationToken">The cancellation token that can be used for cancelling the asynchronous operation.</param>
/// <returns>A <see cref="bool"/> result indicating whether the <see cref="Grid"/> can be passed.</returns>
public delegate Task<bool> AsyncGridFilter(Grid grid, CancellationToken cancellationToken = default);
