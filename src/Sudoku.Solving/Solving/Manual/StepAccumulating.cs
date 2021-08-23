namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides methods for step accumulating.
/// </summary>
internal static class StepAccumulating
{
	/// <summary>
	/// Distinct the list, that is, remove all duplicate elements in this list.
	/// </summary>
	/// <typeparam name="TSelf">The type of the steps.</typeparam>
	/// <param name="list">The list of steps to be processed.</param>
	/// <returns>The list of steps.</returns>
	public static IEnumerable<TSelf> Distinct<TSelf>(IList<TSelf> list)
		where TSelf : class, IDistinctableStep<TSelf>
	{
		var resultList = new List<TSelf>();
		for (int i = 0, length = list.Count, outerLength = length - 1; i < outerLength; i++)
		{
			var e1 = list[i];
			for (int j = i + 1; j < length; j++)
			{
				var e2 = list[j];
				if (!e1.IsSameAs(e2))
				{
					resultList.Add(e1);
				}
			}
		}

		return resultList;
	}
}
