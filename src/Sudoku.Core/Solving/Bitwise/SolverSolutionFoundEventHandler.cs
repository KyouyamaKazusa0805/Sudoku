namespace Sudoku.Solving.Bitwise;

/// <summary>
/// Provides a mechanism to declare a callback, to be called when a solution is found in solving operation,
/// by a certain <see cref="ISolutionEnumerableSolver{TSelf, TInput}"/> instance.
/// </summary>
/// <typeparam name="TSolver">The type of solver.</typeparam>
/// <typeparam name="TInput">The type of input.</typeparam>
/// <param name="sender">The sender which triggers this event.</param>
/// <param name="e">The event arguments provided.</param>
/// <seealso cref="ISolutionEnumerableSolver{TSelf, TInput}"/>
public delegate void SolverSolutionFoundEventHandler<TSolver, TInput>(TSolver sender, SolverSolutionFoundEventArgs<TInput> e)
	where TSolver : ISolutionEnumerableSolver<TSolver, TInput>;
