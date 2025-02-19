namespace Sudoku.Solving.Bitwise;

/// <summary>
/// Provides a mechanism to declare a callback, to be called when a solution is found in solving operation,
/// by a certain <see cref="BitwiseSolver"/> instance.
/// </summary>
/// <param name="sender">The sender which triggers this event.</param>
/// <param name="e">The event arguments provided.</param>
public delegate void BitwiseSolverSolutionFoundEventHandler(BitwiseSolver sender, BitwiseSolverSolutionFoundEventArgs e);

