namespace Sudoku.Analytics.Async;

/// <summary>
/// Represents an analyzer that can asynchronously analyze a puzzle.
/// </summary>
public static class AsyncAnalyzer
{
	/// <summary>
	/// Asynchronously analyzes the specified puzzle.
	/// </summary>
	/// <param name="analyzer">The analyzer.</param>
	/// <param name="grid">The grid to be analyzed.</param>
	/// <param name="progress">The progress reporter.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>An <see cref="AsyncAnalyzerAwaitable"/> object that can analyze the puzzle asynchronously.</returns>
	public static AsyncAnalyzerAwaitable AnalyzeAsync(
		this Analyzer analyzer,
		ref readonly Grid grid,
		IProgress<StepGathererProgressPresenter>? progress = null,
		CancellationToken cancellationToken = default
	) => new(analyzer, in grid, progress, false, cancellationToken);

	/// <summary>
	/// Updates the awaiting rule to specify whether the execution context will be back to the previous one,
	/// instead of just using the current context, to reduce memory allocation.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="continueOnCapturedContext">
	/// Indicates whether to continue works on captured context instead of reverting back to previous context.
	/// </param>
	/// <returns>A new <see cref="AsyncAnalyzerAwaitable"/> instance, with context switching option updated.</returns>
	public static AsyncAnalyzerAwaitable ConfigureAwait(
		this scoped ref readonly AsyncAnalyzerAwaitable @this,
		bool continueOnCapturedContext
	) => new(in @this, continueOnCapturedContext);

	/// <summary>
	/// Analyzes the specified grid, to find for all possible steps and iterate them in asynchronous way.
	/// </summary>
	/// <param name="this">The analyzer instance.</param>
	/// <param name="grid">Indicates the grid to be analyzed.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>A sequence that allows user iterating on it in asynchronous way.</returns>
	public static async IAsyncEnumerable<Step> EnumerateStepsAsync(
		this Analyzer @this,
		Grid grid,
		[EnumeratorCancellation] CancellationToken cancellationToken = default
	)
	{
		var channel = Channel.CreateUnbounded<Step>(new() { SingleReader = true, SingleWriter = true });
		try
		{
			@this.StepFound += this_StepFound;
			_ = Task.Run(
				() =>
				{
					try
					{
						@this.Analyze(in grid, cancellationToken: cancellationToken);
					}
					finally
					{
						channel.Writer.TryComplete();
					}
				},
				cancellationToken
			);

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
			@this.StepFound -= this_StepFound;
		}


		void this_StepFound(Analyzer sender, AnalyzerStepFoundEventArgs e) => channel.Writer.TryWrite(e.Step);
	}
}
