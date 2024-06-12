namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Aggregate</c>.
/// </summary>
/// <inheritdoc/>
public interface IAggregateMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf : IAggregateMethod<TSelf, TSource>, allows ref struct
{
	/// <inheritdoc cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})"/>
	public virtual TSource? Aggregate(Func<TSource?, TSource?, TSource> func) => Aggregate(default, func, @delegate.Self);

	/// <inheritdoc cref="Enumerable.Aggregate{TSource, TAccumulate}(IEnumerable{TSource}, TAccumulate, Func{TAccumulate, TSource, TAccumulate})"/>
	public virtual TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		=> Aggregate(seed, func, @delegate.Self);

	/// <inheritdoc cref="Enumerable.Aggregate{TSource, TAccumulate, TResult}(IEnumerable{TSource}, TAccumulate, Func{TAccumulate, TSource, TAccumulate}, Func{TAccumulate, TResult})"/>
	public virtual TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
	{
		var result = seed;
		foreach (var element in this)
		{
			result = func(result, element);
		}
		return resultSelector(result);
	}
}
