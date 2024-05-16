namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Zip</c>.
/// </summary>
/// <inheritdoc/>
public interface IZipMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource> where TSelf : IZipMethod<TSelf, TSource>
{
	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond}(IEnumerable{TFirst}, IEnumerable{TSecond})"/>
	public virtual IEnumerable<(TSource First, TSecond Second)> Zip<TSecond>(IEnumerable<TSecond> second)
		=> Zip(second, static (first, second) => (first, second));

	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond, TResult}(IEnumerable{TFirst}, IEnumerable{TSecond}, Func{TFirst, TSecond, TResult})"/>
	public virtual IEnumerable<(TSource First, TSecond Second, TThird Third)> Zip<TSecond, TThird>(IEnumerable<TSecond> second, IEnumerable<TThird> third)
		=> Zip(second, third, static (first, second, third) => (first, second, third));

	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond, TResult}(IEnumerable{TFirst}, IEnumerable{TSecond}, Func{TFirst, TSecond, TResult})"/>
	public virtual IEnumerable<TResult> Zip<TSecond, TResult>(IEnumerable<TSecond> second, Func<TSource, TSecond, TResult> resultSelector)
	{
		var result = new List<TResult>();
		using var e1 = GetEnumerator();
		using var e2 = second.GetEnumerator();
		while (e1.MoveNext() && e2.MoveNext())
		{
			result.Add(resultSelector(e1.Current, e2.Current));
		}
		return result;
	}

	/// <inheritdoc/>
	public virtual IEnumerable<TResult> Zip<TSecond, TThird, TResult>(IEnumerable<TSecond> second, IEnumerable<TThird> third, Func<TSource, TSecond, TThird, TResult> resultSelector)
	{
		var result = new List<TResult>();
		using var e1 = GetEnumerator();
		using var e2 = second.GetEnumerator();
		using var e3 = third.GetEnumerator();
		while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
		{
			result.Add(resultSelector(e1.Current, e2.Current, e3.Current));
		}
		return result;
	}
}
