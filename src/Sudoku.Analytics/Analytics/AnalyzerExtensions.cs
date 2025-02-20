using System.Collections.Concurrent;
using System.Threading.Tasks;

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
		using var buffer = new BlockingCollection<Step>();
		try
		{
			@this.StepFound += this_StepFound;

			_ = Task.Run(
				() =>
				{
					try
					{
						var context = new AnalyzerContext(in grid) { CancellationToken = cancellationToken };
						@this.Analyze(in context);
					}
					finally
					{
						buffer.CompleteAdding();
					}
				},
				cancellationToken
			);

			await foreach (var step in buffer.GetConsumingEnumerableAsync(cancellationToken))
			{
				yield return step;
			}
		}
		finally
		{
			@this.StepFound -= this_StepFound;
		}


		void this_StepFound(Analyzer sender, AnalyzerStepFoundEventArgs e) => buffer.Add(e.Step, cancellationToken);
	}
}
