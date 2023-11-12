using System.Collections;

namespace System.Linq;

/// <summary>
/// Provides with extension methods on <see cref="ArrayList"/>.
/// </summary>
/// <seealso cref="ArrayList"/>
public static class ArrayListEnumerable
{
	/// <inheritdoc cref="Enumerable.Cast{TResult}(IEnumerable)"/>
	public static ReadOnlySpan<TResult?> Cast<TResult>(this ArrayList @this)
	{
		var result = new TResult?[@this.Count];
		for (var i = 0; i < @this.Count; i++)
		{
			result[i] = (TResult?)@this[i];
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
	public static ReadOnlySpan<T> Select<T>(this ArrayList @this, Func<object?, T> selector)
	{
		var result = new T[@this.Count];
		for (var i = 0; i < @this.Count; i++)
		{
			result[i] = selector(@this[i]);
		}

		return result;
	}
}
