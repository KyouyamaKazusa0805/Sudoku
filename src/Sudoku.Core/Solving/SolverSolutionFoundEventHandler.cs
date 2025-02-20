namespace Sudoku.Solving;

/// <summary>
/// Provides a mechanism to declare a callback, to be called when a solution is found in solving operation,
/// by a certain <see cref="ISolutionEnumerableSolver"/> instance.
/// </summary>
/// <param name="sender">The sender which triggers this event.</param>
/// <param name="e">The event arguments provided.</param>
/// <seealso cref="ISolutionEnumerableSolver"/>
public delegate void SolverSolutionFoundEventHandler(ISolutionEnumerableSolver sender, SolverSolutionFoundEventArgs e);
