namespace Sudoku.Filtering.Bottleneck;

/// <summary>
/// Provides with a method that determines whether a <see cref="Step"/> is a bottleneck.
/// </summary>
/// <param name="grid">The grid that the step will be applied to.</param>
/// <param name="step">The step to be determined.</param>
/// <returns>A <see cref="bool"/> result.</returns>
[Obsolete("This type will be removed in the future.", false)]
public delegate bool BottleneckFilter(scoped ref readonly Grid grid, Step step);
