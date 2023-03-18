namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a type that can be used for comparison on two <typeparamref name="TSelf"/> instances.
/// </summary>
/// <typeparam name="TSelf">The type of the comparsison object. The type must be derived from <see cref="Step"/>.</typeparam>
/// <seealso cref="Step"/>
public interface IEquatableStep<TSelf> where TSelf : Step, IEquatableStep<TSelf>
{
	/// <summary>
	/// Try to compare elements, removing all duplicated ones in this collection,
	/// which uses the operator <see cref="op_Equality(TSelf, TSelf)"/> defined in this interface.
	/// </summary>
	/// <param name="list">The list of steps to be processed.</param>
	/// <returns>The list of steps.</returns>
	/// <remarks>
	/// This method does not change the ordering of the original list.
	/// In other words, if the original list is in order, the final list after invoking this method will be also in order.
	/// </remarks>
	/// <seealso cref="op_Equality(TSelf, TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static IEnumerable<TSelf> Distinct(List<TSelf> list)
		=> list switch
		{
			[] => Array.Empty<TSelf>(),
			[var firstElement] => new[] { firstElement },
			[var a, var b] => a == b ? new[] { a } : new[] { a, b },
			_ => new HashSet<TSelf>(list, LocalEqualityComparer<TSelf>.Instance)
		};


	/// <summary>
	/// Determines whether two <typeparamref name="TSelf"/> instances are considered equal.
	/// </summary>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating whether two <typeparamref name="TSelf"/> instances are considered equal.</returns>
	static abstract bool operator ==(TSelf left, TSelf right);

	/// <summary>
	/// Determines whether two <typeparamref name="TSelf"/> instances are not considered equal.
	/// </summary>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating whether two <typeparamref name="TSelf"/> instances are not considered equal.</returns>
	static virtual bool operator !=(TSelf left, TSelf right) => !(left == right);
}

/// <summary>
/// The internal comparer type for <typeparamref name="T"/> instances.
/// </summary>
/// <typeparam name="T">The type of the step.</typeparam>
file sealed class LocalEqualityComparer<T> : IEqualityComparer<T> where T : Step, IEquatableStep<T>
{
	/// <summary>
	/// The singleton instance.
	/// </summary>
	public static readonly LocalEqualityComparer<T> Instance = new();


	/// <summary>
	/// The parameterless constructor.
	/// </summary>
	private LocalEqualityComparer()
	{
	}


#nullable disable warnings
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(T x, T y) => x == y;
#nullable restore warnings

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode(T obj) => 0;
}
