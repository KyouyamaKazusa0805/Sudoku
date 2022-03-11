namespace System.Linq;

/// <summary>
/// Provides with the LINQ-related methods on type <see cref="Array"/>, especially for the one-dimensional array.
/// </summary>
public static class ArrayEnumerable
{
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
	/// <param name="this">
	/// A sequence of values to invoke a transform function on.
	/// </param>
	/// <param name="selector">
	/// A transform function to apply to each element.
	/// </param>
	public static TResult[] Select<T, TResult>(this T[] @this, Func<T, TResult> selector)
	{
		int length = @this.Length;
		var result = new TResult[length];
		for (int i = 0; i < length; i++)
		{
			result[i] = selector(@this[i]);
		}

		return result;
	}
}
