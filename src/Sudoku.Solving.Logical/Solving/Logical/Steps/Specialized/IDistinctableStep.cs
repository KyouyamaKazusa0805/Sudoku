namespace Sudoku.Solving.Logical.Steps.Specialized;

/// <summary>
/// Defines a step that can be distinctable.
/// </summary>
/// <typeparam name="TStep">The type of the element to compare.</typeparam>
/// <remarks>
/// A <b>distinctable step</b> is a step that is with the unique information,
/// in order that multiple steps of the same type can be recognized by the relative methods,
/// to filter and remove same-value instances.
/// </remarks>
internal interface IDistinctableStep<in TStep> : IStep where TStep : Step
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
	/// <remarks>
	/// This method does not change the ordering of the original list. In other words, if the original list
	/// is in order, the final list after invoking this method will be also in order.
	/// </remarks>
	/// <seealso cref="Equals(TStep, TStep)"/>
	static IEnumerable<TDistinctableStep> Distinct<TDistinctableStep>(IList<TDistinctableStep> list)
		where TDistinctableStep : Step, IDistinctableStep<TDistinctableStep>
		=> list switch
		{
			[] => Array.Empty<TDistinctableStep>(),
			[var firstElement] => new[] { firstElement },
			[var a, var b] => TDistinctableStep.Equals(a, b) ? new[] { a } : new[] { a, b },
			_ => new HashSet<TDistinctableStep>(list, DefaultComparer<TDistinctableStep>.Instance)
		};
}

/// <summary>
/// The internal comparer instance.
/// </summary>
/// <typeparam name="TStep">The type of the step.</typeparam>
file sealed class DefaultComparer<TStep> : IEqualityComparer<TStep> where TStep : Step, IDistinctableStep<TStep>
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static DefaultComparer<TStep> Instance = new();


	/// <summary>
	/// Initializes a <see cref="DefaultComparer{TStep}"/> instance.
	/// </summary>
	private DefaultComparer()
	{
	}


	/// <inheritdoc/>
	public bool Equals(TStep? x, TStep? y) => TStep.Equals(x!, y!);

	/// <inheritdoc/>
	public int GetHashCode(TStep obj) => 0;
}
