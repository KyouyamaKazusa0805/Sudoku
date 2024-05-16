namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>CountBy</c>.
/// </summary>
/// <inheritdoc/>
public interface ICountByProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : ICountByProvider<TSelf, TSource>
{
	/// <inheritdoc/>
	public virtual IEnumerable<KeyValuePair<TKey, int>> CountBy<TKey>(Func<TSource, TKey> keySelector) where TKey : notnull
	{
		var dictionary = new Dictionary<TKey, int>();
		foreach (var element in this)
		{
			if (!dictionary.TryAdd(keySelector(element), 1))
			{
				dictionary[keySelector(element)]++;
			}
		}
		return dictionary;
	}

	/// <inheritdoc/>
	public virtual IEnumerable<KeyValuePair<TKey, int>> CountBy<TKey>(Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? keyComparer)
		where TKey : notnull
	{
		var dictionary = new Dictionary<TKey, int>(keyComparer);
		foreach (var element in this)
		{
			if (!dictionary.TryAdd(keySelector(element), 1))
			{
				dictionary[keySelector(element)]++;
			}
		}
		return dictionary;
	}
}
