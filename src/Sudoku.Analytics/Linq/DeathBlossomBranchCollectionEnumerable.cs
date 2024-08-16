namespace Sudoku.Linq;

/// <summary>
/// Represents a list of methods that is used as LINQ methods on type <see cref="DeathBlossomBranchCollection{TSelf, TKey}"/>.
/// </summary>
/// <seealso cref="DeathBlossomBranchCollection{TSelf, TKey}"/>
public static class DeathBlossomBranchCollectionEnumerable
{
	/// <summary>
	/// Transforms the current collection into another representation, using the specified function to transform.
	/// </summary>
	/// <typeparam name="TSelf">
	/// <inheritdoc cref="DeathBlossomBranchCollection{TSelf, TKey}" path="/typeparam[@name='TSelf']"/>
	/// </typeparam>
	/// <typeparam name="TKey">
	/// <inheritdoc cref="DeathBlossomBranchCollection{TSelf, TKey}" path="/typeparam[@name='TKey']"/>
	/// </typeparam>
	/// <typeparam name="TResult">The type of the results.</typeparam>
	/// <param name="this">The current collection.</param>
	/// <param name="selector">The selector to transform elements.</param>
	/// <returns>The results.</returns>
	public static ReadOnlySpan<TResult> Select<TSelf, TKey, TResult>(this DeathBlossomBranchCollection<TSelf, TKey> @this, Func<KeyValuePair<TKey, AlmostLockedSet>, TResult> selector)
		where TSelf : DeathBlossomBranchCollection<TSelf, TKey>, IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>, new()
		where TKey : notnull, IAdditiveIdentity<TKey, TKey>, IEquatable<TKey>, IEqualityOperators<TKey, TKey, bool>, new()
	{
		var (result, i) = (new TResult[@this.Count], 0);
		foreach (var kvp in @this)
		{
			ref readonly var key = ref kvp.KeyRef();
			ref readonly var value = ref kvp.ValueRef();
			result[i++] = selector(new(key, value));
		}
		return result;
	}
}
