namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Defines a step that can be distinctable.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the element to compare. The type must derive from <see cref="IStep"/> (e.g. <see cref="Step"/>),
/// and implement this interface type, with type argument be itself.
/// </typeparam>
/// <remarks>
/// A <b>distinctable step</b> is a step that is with the unique information,
/// in order that multiple steps of the same type can be recognized by the relative methods to filter and remove same-value instances.
/// </remarks>
public interface IDistinctableStep<in TSelf> : IStep where TSelf : IDistinctableStep<TSelf>
{
	/// <summary>
	/// To compare 2 instances of type <typeparamref name="TSelf"/>, to determine whether 2 instances holds the same value.
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
	/// but <see langword="record"/>s are automatically implemented useless and unmeaningful methods.
	/// </remarks>
	static abstract bool Equals(TSelf left, TSelf right);

	/// <summary>
	/// Distinct the list, that is, remove all duplicate elements in this list,
	/// which uses the method <see cref="Equals(TSelf, TSelf)"/> defined in this interface.
	/// </summary>
	/// <param name="list">The list of steps to be processed.</param>
	/// <returns>The list of steps.</returns>
	/// <remarks>
	/// This method does not change the ordering of the original list.
	/// In other words, if the original list is in order, the final list after invoking this method will be also in order.
	/// </remarks>
	/// <seealso cref="Equals(TSelf, TSelf)"/>
	static IEnumerable<TSelf> Distinct(IList<TSelf> list)
		=> list switch
		{
			[] => Array.Empty<TSelf>(),
			[var firstElement] => new[] { firstElement },
			[var a, var b] => TSelf.Equals(a, b) ? new[] { a } : new[] { a, b },
			_ => new HashSet<TSelf>(list, Comparer<TSelf>.Instance)
		};
}

/// <summary>
/// The internal comparer instance.
/// </summary>
/// <typeparam name="T">The type of the step.</typeparam>
file sealed class Comparer<T> : IEqualityComparer<T> where T : IDistinctableStep<T>
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static readonly Comparer<T> Instance = new();


	/// <summary>
	/// Initializes a <see cref="Comparer{T}"/> instance.
	/// </summary>
	private Comparer()
	{
	}


	/// <inheritdoc/>
	public bool Equals(T? x, T? y) => T.Equals(x!, y!);

	/// <inheritdoc/>
	public int GetHashCode(T obj) => 0;
}
