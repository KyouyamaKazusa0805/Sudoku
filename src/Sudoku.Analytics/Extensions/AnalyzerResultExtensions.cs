namespace Sudoku.Analytics;

/// <summary>
/// Provides with extension methods on <see cref="AnalyzerResult"/>.
/// </summary>
/// <seealso cref="AnalyzerResult"/>
public static class AnalyzerResultExtensions
{
	/// <summary>
	/// Try to get all possible bottleneck steps, and their own indices.
	/// </summary>
	/// <param name="this">The <see cref="AnalyzerResult"/> instance to be analyzed.</param>
	/// <param name="bottleneckFilters">
	/// The filters of bottleneck checkers.
	/// You can use <see cref="BottleneckFilter"/>.<see langword="operator"/> +(<see cref="BottleneckFilter"/>, <see cref="BottleneckFilter"/>)
	/// to combine multiple filter methods.
	/// </param>
	/// <returns>An array of results, representing the bottlenecks.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle hasn't been solved.</exception>
	public static (int Index, Step BottleneckStep)[] GetBottlenecks(this AnalyzerResult @this, BottleneckFilter bottleneckFilters)
	{
		if (!@this.IsSolved)
		{
			throw new InvalidOperationException("The puzzle hasn't been solved.");
		}

		var invocations = bottleneckFilters.GetInvocations();
		var result = new List<(int, Step)>(@this.Steps.Length);
		for (var i = 0; i < @this.Steps.Length; i++)
		{
			var step = @this.Steps[i];
			var flag = true;
			foreach (var invocation in invocations)
			{
				if (!invocation(in @this.SteppingGrids[i], step))
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}

			result.Add((i, step));
		}

		return [.. result];
	}
}
