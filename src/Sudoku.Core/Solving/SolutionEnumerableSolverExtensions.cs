namespace Sudoku.Solving;

/// <summary>
/// Provides with extension methods on <see cref="ISolutionEnumerableSolver{TSelf}"/>.
/// </summary>
/// <seealso cref="ISolutionEnumerableSolver{TSelf}"/>
public static class SolutionEnumerableSolverExtensions
{
	/// <summary>
	/// Try to enumerate all possible solutions of the specified grid, by using the current solver.
	/// </summary>
	/// <typeparam name="TSolver">The type of solver.</typeparam>
	/// <param name="this">The solver.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>A sequence of <see cref="Grid"/> values indicating the raw solution text to the puzzle.</returns>
	public static IEnumerable<Grid> EnumerateSolutions<TSolver>(this TSolver @this, Grid grid, CancellationToken cancellationToken = default)
		where TSolver : ISolutionEnumerableSolver<TSolver>
	{
		using var buffer = new BlockingCollection<Grid>();
		try
		{
			// Temporarily add handler.
			@this.SolutionFound += this_SolutionFound;

			// Perform adding operation.
			// Here we must omit 'await' keyword here because here we should make code concurrently executed on purpose,
			// i.e. making the following code (consuming enumerable method) become available.
			Task.Run(
				() =>
				{
					try
					{
						@this.EnumerateSolutionsCore(grid, cancellationToken);
					}
					finally
					{
						buffer.CompleteAdding();
					}
				},
				cancellationToken
			);

			// Consume the solutions concurrently.
			foreach (var item in buffer.GetConsumingEnumerable(cancellationToken))
			{
				yield return item;
			}
		}
		finally
		{
			// Remove handler.
			@this.SolutionFound -= this_SolutionFound;
		}


		void this_SolutionFound(TSolver _, SolverSolutionFoundEventArgs e) => buffer.Add(e.Solution, cancellationToken);
	}
}
