namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Average</c>.
/// </summary>
/// <inheritdoc/>
public interface IAverageProvider<TSelf, TSource> : ILinqMethodProvider<TSelf, TSource>
	where TSelf : IAverageProvider<TSelf, TSource>
	where TSource : INumberBase<TSource>
{
	/// <inheritdoc/>
	public virtual TResult Average<TAccumulator, TResult>()
		where TAccumulator : INumberBase<TAccumulator>
		where TResult : INumberBase<TResult>
	{
		using var e = GetEnumerator();
		if (!e.MoveNext())
		{
			throw new InvalidOperationException();
		}

		var sum = TAccumulator.CreateChecked(e.Current);
		var count = 1L;
		while (e.MoveNext())
		{
			sum += TAccumulator.CreateChecked(e.Current);
			count++;
		}

		return TResult.CreateChecked(sum) / TResult.CreateChecked(count);
	}
}
