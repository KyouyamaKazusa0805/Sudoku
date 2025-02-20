namespace Sudoku.Solving;

/// <summary>
/// Provides a mechanism to declare a callback to be called when a solution is found in solving operation by a solver.
/// </summary>
/// <param name="sender">The sender which triggers this event.</param>
/// <param name="e">The event arguments provided.</param>
public delegate void SolverSolutionFoundEventHandler(object? sender, SolverSolutionFoundEventArgs e);
