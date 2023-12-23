namespace System.Linq;

/// <summary>
/// Provides with LINQ methods for <see cref="ITuple"/> instances.
/// </summary>
/// <seealso cref="ITuple"/>
public static class TupleEnumerable
{
	/// <inheritdoc cref="Enumerable.Cast{TResult}(IEnumerable)"/>
	public static ReadOnlySpan<T?> Cast<T>(this ITuple @this)
	{
		var result = new T?[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = (T?)element;
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
	public static ReadOnlySpan<TResult> Select<T, TResult>(this ITuple @this, Func<T?, TResult> selector)
	{
		var result = new TResult[@this.Length];
		var i = 0;
		foreach (var element in from T? element in @this select element)
		{
			result[i++] = selector(element);
		}

		return result;
	}
}
