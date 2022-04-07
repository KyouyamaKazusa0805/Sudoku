namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Defines a step that can be distinctable.
/// </summary>
/// <typeparam name="TStep">The type of the element to compare.</typeparam>
/// <remarks>
/// A <b>distinctable step</b> is a step that is with the unique information,
/// in order that multiple steps of the same type can be recognized by the relative methods,
/// to filter and remove same-value instances.
/// </remarks>
public interface IDistinctableStep<in TStep> : IStep where TStep : Step
{
	/// <summary>
	/// To compare 2 instances of type <typeparamref name="TStep"/>,
	/// to determine whether 2 instances holds the same value.
	/// </summary>
	/// <param name="left">Indicates the first instance to compare.</param>
	/// <param name="right">Indicates the second instance to compare.</param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the elements are same.
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>Two elements are same.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>Two elements holds the different values.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// The method can be the same implemented as the method <see cref="object.Equals(object?)"/>,
	/// but <see langword="record"/>s are automatically implemented the method, which is useless
	/// and unmeaningful.
	/// </remarks>
	static abstract bool Equals(TStep left, TStep right);


	/// <summary>
	/// Distinct the list, that is, remove all duplicate elements in this list, that uses the method
	/// <see cref="Equals(TStep, TStep)"/> defined in this interface.
	/// </summary>
	/// <typeparam name="TDistinctableStep">The type of the steps.</typeparam>
	/// <param name="list">The list of steps to be processed.</param>
	/// <returns>The list of steps.</returns>
	/// <seealso cref="Equals(TStep, TStep)"/>
	public static IEnumerable<TDistinctableStep> Distinct<TDistinctableStep>(IList<TDistinctableStep> list)
		where TDistinctableStep : Step, IDistinctableStep<TDistinctableStep>
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
