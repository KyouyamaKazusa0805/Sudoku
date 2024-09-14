namespace Sudoku.Generating.Filtering;

/// <summary>
/// Defines a type that checks whether a <see cref="Step"/> satisfies the specified condition.
/// </summary>
/// <param name="step">The step.</param>
/// <returns>A <see cref="bool"/> result indicating that.</returns>
/// <seealso cref="Step"/>
public delegate bool StepFilter(Step step);
