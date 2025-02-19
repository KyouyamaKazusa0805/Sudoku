namespace Sudoku.Solving.Bitwise;

/// <summary>
/// Provides with extension methods on <see cref="BitwiseSolver"/>.
/// </summary>
/// <seealso cref="BitwiseSolver"/>
public static class BitwiseSolverExtensions
{
	/// <summary>
	/// Try to enumerate all possible solutions of the specified grid, by using the current solver.
	/// </summary>
	/// <param name="this">The solver.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>A sequence of <see cref="string"/> values indicating the raw solution text to the puzzle.</returns>
	public static IEnumerable<string> EnumerateSolutions(
		this BitwiseSolver @this,
		string grid,
		CancellationToken cancellationToken = default
	)
	{
		using var buffer = new BlockingCollection<string>();
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
						unsafe
						{
							@this.SolveString(grid, null, int.MaxValue);
						}
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


		void this_SolutionFound(BitwiseSolver _, BitwiseSolverSolutionFoundEventArgs e)
			=> buffer.Add(e.SolutionString, cancellationToken);
	}
}
