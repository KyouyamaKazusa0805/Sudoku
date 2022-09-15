namespace System.Linq;

/// <summary>
/// Defines a type that supports <see langword="group"/>-<see langword="by"/> clauses.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
public interface IGroupByClauseProvider<T> : ILinqProvider<T>
{
	/// <summary>
	/// Groups the elements of a sequence.
	/// </summary>
	/// <typeparam name="TKey">The type of key as the grouping rule.</typeparam>
	/// <param name="keySelector">The function that projects the key from elements.</param>
	/// <returns>The grouped result.</returns>
	public abstract IEnumerable<IGrouping<TKey, T>> GroupBy<TKey>(Func<T, TKey> keySelector);

	/// <inheritdoc cref="GroupBy{TKey}(Func{T, TKey})"/>
	public sealed unsafe IEnumerable<IGrouping<TKey, T>> GroupByUnsafe<TKey>(delegate*<T, TKey> keySelector)
		=> GroupBy(e => keySelector(e));
}
