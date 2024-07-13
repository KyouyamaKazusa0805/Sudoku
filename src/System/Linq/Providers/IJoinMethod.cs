namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Join</c> and <c>GroupJoin</c>.
/// </summary>
/// <inheritdoc/>
public interface IJoinMethod<TSelf, TSource> : IQueryExpressionMethod<TSelf, TSource>
	where TSelf : IJoinMethod<TSelf, TSource>, allows ref struct
	where TSource : allows ref struct
{
	/// <inheritdoc cref="Enumerable.Join{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, TInner, TResult})"/>
	public virtual IEnumerable<TResult> Join<TInner, TKey, TResult>(
		IEnumerable<TInner> inner,
		Func<TSource, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<TSource, TInner, TResult> resultSelector
	) where TKey : notnull => Join(inner, outerKeySelector, innerKeySelector, resultSelector, null);

	/// <inheritdoc cref="Enumerable.Join{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, TInner, TResult}, IEqualityComparer{TKey}?)"/>
	public virtual IEnumerable<TResult> Join<TInner, TKey, TResult>(
		IEnumerable<TInner> inner,
		Func<TSource, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<TSource, TInner, TResult> resultSelector,
		IEqualityComparer<TKey>? comparer
	) where TKey : notnull
	{
		comparer ??= EqualityComparer<TKey>.Default;

		var result = new List<TResult>();
		foreach (var outerItem in this)
		{
			var outerKey = outerKeySelector(outerItem);
			var outerKeyHash = comparer.GetHashCode(outerKey);
			foreach (var innerItem in inner)
			{
				var innerKey = innerKeySelector(innerItem);
				var innerKeyHash = comparer.GetHashCode(innerKey);
				if (outerKeyHash != innerKeyHash)
				{
					// They are not same due to hash code difference.
					continue;
				}

				if (!comparer.Equals(outerKey, innerKey))
				{
					// They are not same due to inequality.
					continue;
				}

				result.Add(resultSelector(outerItem, innerItem));
			}
		}
		return [.. result];
	}

	/// <inheritdoc cref="Enumerable.GroupJoin{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, IEnumerable{TInner}, TResult})"/>
	public virtual IEnumerable<TResult> GroupJoin<TInner, TKey, TResult>(
		IEnumerable<TInner> inner,
		Func<TSource, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<TSource, IEnumerable<TInner>, TResult> resultSelector
	) where TKey : notnull => GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, null);

	/// <inheritdoc cref="Enumerable.GroupJoin{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, IEnumerable{TInner}, TResult}, IEqualityComparer{TKey}?)"/>
	public virtual IEnumerable<TResult> GroupJoin<TInner, TKey, TResult>(
		IEnumerable<TInner> inner,
		Func<TSource, TKey> outerKeySelector,
		Func<TInner, TKey> innerKeySelector,
		Func<TSource, IEnumerable<TInner>, TResult> resultSelector,
		IEqualityComparer<TKey>? comparer
	) where TKey : notnull
	{
		comparer ??= EqualityComparer<TKey>.Default;

		var innerKvps = inner.Select(element => (innerKeySelector(element), element));
		var result = new List<TResult>();
		foreach (var outerItem in this)
		{
			var outerKey = outerKeySelector(outerItem);
			var outerKeyHash = comparer.GetHashCode(outerKey);
			var satisfiedInnerKvps = new List<TInner>();
			foreach (var (innerKey, innerItem) in innerKvps)
			{
				var innerKeyHash = comparer.GetHashCode(innerKey);
				if (outerKeyHash != innerKeyHash)
				{
					// They are not same due to hash code difference.
					continue;
				}

				if (!comparer.Equals(outerKey, innerKey))
				{
					// They are not same due to inequality.
					continue;
				}

				satisfiedInnerKvps.Add(innerItem);
			}

			result.Add(resultSelector(outerItem, [.. satisfiedInnerKvps]));
		}
		return [.. result];
	}
}
