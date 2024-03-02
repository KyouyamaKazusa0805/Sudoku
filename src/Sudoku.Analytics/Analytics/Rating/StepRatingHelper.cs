namespace Sudoku.Analytics.Rating;

/// <summary>
/// The helper type that calculates for <see cref="Step"/> instances.
/// </summary>
/// <see cref="Step"/>
internal static class StepRatingHelper
{
	/// <summary>
	/// Get extra difficulty rating for a chain node sequence.
	/// </summary>
	/// <param name="length">The length.</param>
	/// <returns>The difficulty.</returns>
	public static decimal GetExtraDifficultyByLength(int length)
	{
		var result = 0M;
		for (var (isOdd, ceil) = (false, 4); length > ceil; isOdd = !isOdd)
		{
			result += .1M;
			ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
		}

		return result;
	}

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
	internal static unsafe decimal EvaluateRatingUnsafe(Step[]? steps, StepRatingEvaluatorFuncPtr executor, decimal d)
	{
		static decimal f(Step step) => step.Difficulty;
		return steps is null ? d : executor(steps, &f);
	}

	/// <inheritdoc cref="Enumerable.Max(IEnumerable{decimal})"/>
	internal static unsafe decimal MaxUnsafe<T>(T[] collection, delegate*<T, decimal> selector)
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
	internal static unsafe decimal SumUnsafe<T>(T[] collection, delegate*<T, decimal> selector)
	{
		var result = 0M;
		foreach (var element in collection)
		{
			result += selector(element);
		}

		return result;
	}
}
