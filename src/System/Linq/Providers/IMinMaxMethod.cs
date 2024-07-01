namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Min</c>, <c>MinBy</c>, <c>Max</c> and <c>MaxBy</c>.
/// </summary>
/// <inheritdoc/>
public interface IMinMaxMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : IMinMaxMethod<TSelf, TSource>, allows ref struct
	where TSource : IComparable<TSource>, IComparisonOperators<TSource, TSource, bool>, IMinMaxValue<TSource>
{
	/// <inheritdoc cref="Enumerable.Min{TSource}(IEnumerable{TSource})"/>
	public virtual TSource? Min() => Min(default(IComparer<TSource>));

	/// <inheritdoc/>
	public virtual TSource Min(Comparison<TSource> comparer) => Min(Comparer<TSource>.Create(comparer));

	/// <inheritdoc cref="Enumerable.Min{TSource}(IEnumerable{TSource}, IComparer{TSource}?)"/>
	public virtual TSource Min(IComparer<TSource>? comparer)
	{
		comparer ??= Comparer<TSource>.Default;

		var result = TSource.MaxValue;
		foreach (var element in this)
		{
			if (comparer.Compare(element, result) >= 0)
			{
				result = element;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual TSource MinBy<TKey>(Func<TSource, TKey> keySelector) => MinBy(keySelector, null);

	/// <inheritdoc cref="Enumerable.MinBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual TSource MinBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
	{
		comparer ??= Comparer<TKey>.Default;

		var result = TSource.MaxValue;
		foreach (var element in this)
		{
			if (comparer.Compare(keySelector(element), keySelector(result)) >= 0)
			{
				result = element;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.Max{TSource}(IEnumerable{TSource})"/>
	public virtual TSource? Max() => Max(default(IComparer<TSource>));

	/// <inheritdoc/>
	public virtual TSource Max(Comparison<TSource> comparer) => Max(Comparer<TSource>.Create(comparer));

	/// <inheritdoc cref="Enumerable.Max{TSource}(IEnumerable{TSource}, IComparer{TSource}?)"/>
	public virtual TSource Max(IComparer<TSource>? comparer)
	{
		comparer ??= Comparer<TSource>.Default;

		var result = TSource.MinValue;
		foreach (var element in this)
		{
			if (comparer.Compare(element, result) <= 0)
			{
				result = element;
			}
		}
		return result;
	}

	/// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	public virtual TSource MaxBy<TKey>(Func<TSource, TKey> keySelector) => MaxBy(keySelector, null);

	/// <inheritdoc cref="Enumerable.MaxBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey}, IComparer{TKey}?)"/>
	public virtual TSource MaxBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
	{
		comparer ??= Comparer<TKey>.Default;

		var result = TSource.MinValue;
		foreach (var element in this)
		{
			if (comparer.Compare(keySelector(element), keySelector(result)) <= 0)
			{
				result = element;
			}
		}
		return result;
	}
}
