namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides methods for step accumulating.
/// </summary>
internal static class StepAccumulating
{
	/// <summary>
	/// Distinct the list, that is, remove all duplicate elements in this list.
	/// </summary>
	/// <typeparam name="TDistinctableStep">The type of the steps.</typeparam>
	/// <param name="list">The list of steps to be processed.</param>
	/// <returns>The list of steps.</returns>
	public static IEnumerable<TDistinctableStep> Distinct<TDistinctableStep>(IList<TDistinctableStep> list)
		where TDistinctableStep : class, IDistinctableStep<TDistinctableStep>
	{
		var resultList = new List<TDistinctableStep>();
		for (int i = 0, length = list.Count, outerLength = length - 1; i < outerLength; i++)
		{
			var e1 = list[i];
			for (int j = i + 1; j < length; j++)
			{
				var e2 = list[j];
				if (!TDistinctableStep.Equals(e1, e2))
				{
					resultList.Add(e1);
				}
			}
		}

		return resultList;
	}
}
