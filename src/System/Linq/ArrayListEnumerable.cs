namespace System.Linq;

/// <summary>
/// Provides with extension methods on <see cref="ArrayList"/>.
/// </summary>
/// <seealso cref="ArrayList"/>
public static class ArrayListEnumerable
{
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
	public static IEnumerable<T> Select<T>(this ArrayList @this, Func<object, T> selector)
	{
		foreach (var element in @this)
		{
			yield return selector(element);
		}
	}
}
