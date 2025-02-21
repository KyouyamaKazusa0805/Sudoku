namespace Sudoku.Analytics;

/// <summary>
/// Provides with extension methods on <see cref="Analyzer"/>.
/// </summary>
/// <seealso cref="Analyzer"/>
public static class AnalyzerExtensions
{
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
						@this.Analyze(new AnalyzerContext(in grid) { CancellationToken = cancellationToken });
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
