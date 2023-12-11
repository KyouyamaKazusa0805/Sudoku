using Sudoku.Analytics;

namespace Sudoku.Diff;

/// <summary>
/// Represents a filter that checks whether a <see cref="Step"/> is valid.
/// </summary>
/// <param name="step">A step to be checked.</param>
/// <returns>A <see cref="bool"/> result indicating that.</returns>
public delegate bool StepFilter(Step step);
