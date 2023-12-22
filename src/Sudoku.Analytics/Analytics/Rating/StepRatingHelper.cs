namespace Sudoku.Analytics.Rating;

/// <summary>
/// The helper type that calculates for <see cref="Step"/> instances.
/// </summary>
/// <see cref="Step"/>
internal static unsafe class StepRatingHelper
{
	/// <summary>
	/// The inner executor to get the difficulty value (total, average).
	/// </summary>
	/// <param name="steps">The steps to be calculated.</param>
	/// <param name="executor">The execute method.</param>
	/// <param name="d">
	/// The default value as the return value when <see cref="Steps"/> is <see langword="null"/> or empty.
	/// </param>
	/// <returns>The result.</returns>
	/// <seealso cref="Steps"/>
	public static decimal EvaluatorUnsafe(Step[]? steps, delegate*<IEnumerable<Step>, delegate*<Step, decimal>, decimal> executor, decimal d)
	{
		static decimal f(Step step) => step.Difficulty;
		return steps is null ? d : executor(steps, &f);
	}

	/// <inheritdoc cref="Enumerable.Max(IEnumerable{decimal})"/>
	public static decimal MaxUnsafe<T>(IEnumerable<T> collection, delegate*<T, decimal> selector)
	{
		var result = decimal.MinValue;
		foreach (var element in collection)
		{
			var converted = selector(element);
			if (converted >= result)
			{
				result = converted;
			}
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, decimal})"/>
	public static decimal SumUnsafe<T>(IEnumerable<T> collection, delegate*<T, decimal> selector)
	{
		var result = 0M;
		foreach (var element in collection)
		{
			result += selector(element);
		}

		return result;
	}
}
