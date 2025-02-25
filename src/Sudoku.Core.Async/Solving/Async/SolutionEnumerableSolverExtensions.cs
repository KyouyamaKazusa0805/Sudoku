namespace Sudoku.Solving.Async;

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
		var channel = Channel.CreateUnbounded<Grid>(new() { SingleReader = true, SingleWriter = true });
		try
		{
			// Temporarily add handler.
			@this.SolutionFound += this_SolutionFound;

			// Perform adding operation.
			// Here we must omit 'await' keyword here because here we should make code concurrently executed on purpose,
			// i.e. making the following code (consuming enumerable method) become available.
			ThreadPool.QueueUserWorkItem(
				_ =>
				{
					try
					{
						@this.EnumerateSolutionsCore(grid, cancellationToken);
					}
					finally
					{
						channel.Writer.TryComplete();
					}
				}
			);

			// Consume the solutions concurrently.
			while (await channel.Reader.WaitToReadAsync(cancellationToken))
			{
				if (channel.Reader.TryRead(out var step))
				{
					yield return step;
				}
			}

			// Wait for completion.
			await channel.Reader.Completion;
		}
		finally
		{
			// Remove temporary handler.
			@this.SolutionFound -= this_SolutionFound;
		}


		async void this_SolutionFound(object? _, SolverSolutionFoundEventArgs e)
			=> await channel.Writer.WriteAsync(e.Solution, cancellationToken);
	}
}
