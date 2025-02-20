namespace Sudoku.Solving;

/// <summary>
/// Provides a mechanism to declare a callback, to be called when a solution is found in solving operation,
/// by a certain <see cref="ISolutionEnumerableSolver{TSelf}"/> instance.
/// </summary>
/// <typeparam name="TSolver">The type of solver.</typeparam>
/// <param name="sender">The sender which triggers this event.</param>
/// <param name="e">The event arguments provided.</param>
/// <seealso cref="ISolutionEnumerableSolver{TSelf}"/>
public delegate void SolverSolutionFoundEventHandler<TSolver>(TSolver sender, SolverSolutionFoundEventArgs e)
	where TSolver : ISolutionEnumerableSolver<TSolver>;
