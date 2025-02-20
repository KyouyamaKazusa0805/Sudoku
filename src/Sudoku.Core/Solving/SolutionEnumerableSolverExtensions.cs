namespace Sudoku.Solving;

/// <summary>
/// Provides with extension methods on <see cref="ISolutionEnumerableSolver"/>.
/// </summary>
/// <seealso cref="ISolutionEnumerableSolver"/>
public static class SolutionEnumerableSolverExtensions
{
	/// <summary>
	/// Count the number of solutions can be found of a grid.
	/// </summary>
	/// <param name="this">The solver.</param>
	/// <param name="grid">The grid to be solved.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>
	/// A <see cref="Task{TResult}"/> instance indicating the asynchronous operation,
	/// with an <see cref="int"/> value indicating the result can be produced after the operation executed.
	/// </returns>
	public static async Task<int> CountSolutionsAsync(this ISolutionEnumerableSolver @this, Grid grid, CancellationToken cancellationToken = default)
	{
		var result = 0;
		await foreach (var _ in @this.EnumerateSolutionsAsync(grid, cancellationToken))
		{
			result++;
			if (cancellationToken.IsCancellationRequested)
			{
				return 0;
			}
		}
		return result;
	}

	/// <summary>
	/// Try to enumerate all possible solutions of the specified grid, by using the current solver.
	/// </summary>
	/// <param name="this">The solver.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>A sequence of <see cref="Grid"/> values indicating the raw solution text to the puzzle.</returns>
	public static IEnumerable<Grid> EnumerateSolutions(this ISolutionEnumerableSolver @this, Grid grid, CancellationToken cancellationToken = default)
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


		void this_SolutionFound(object? _, SolverSolutionFoundEventArgs e) => buffer.Add(e.Solution, cancellationToken);
	}

	/// <summary>
	/// Try to enumerate all possible solutions of the specified grid, by using the current solver, in asynchronous way.
	/// </summary>
	/// <param name="this">The solver.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>A sequence of <see cref="Grid"/> values indicating the raw solution text to the puzzle.</returns>
	public static async IAsyncEnumerable<Grid> EnumerateSolutionsAsync(
		this ISolutionEnumerableSolver @this,
		Grid grid,
		[EnumeratorCancellation] CancellationToken cancellationToken = default
	)
	{
		using var buffer = new BlockingCollection<Grid>();
		try
		{
			// Temporarily add handler.
			@this.SolutionFound += this_SolutionFound;

			// Perform adding operation.
			// Here we must omit 'await' keyword here because here we should make code concurrently executed on purpose,
			// i.e. making the following code (consuming enumerable method) become available.
			_ = Task.Run(
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
			await foreach (var item in buffer.GetConsumingEnumerableAsync(cancellationToken))
			{
				yield return item;
			}
		}
		finally
		{
			// Remove handler.
			@this.SolutionFound -= this_SolutionFound;
		}


		void this_SolutionFound(object? _, SolverSolutionFoundEventArgs e) => buffer.Add(e.Solution, cancellationToken);
	}
}
