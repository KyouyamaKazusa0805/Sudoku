namespace System.Collections.Immutable;

/// <summary>
/// Provides with extension methods on <see cref="ImmutableArray{T}"/>.
/// </summary>
/// <see cref="ImmutableArray{T}"/>
public static class ImmutableArrayExtensions
{
	/// <inheritdoc cref="Enumerable.Count{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static int Count<T>(this ImmutableArray<T> @this, Predicate<T> predicate)
	{
		if (@this.IsDefault)
		{
			throw new ArgumentException("The array is not initialized.", nameof(@this));
		}

		int result = 0;
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				result++;
			}
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.Count{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static unsafe int Count<T>(this ImmutableArray<T> @this, delegate*<T, bool> predicate)
	{
		if (@this.IsDefault)
		{
			throw new ArgumentException("The array is not initialized.", nameof(@this));
		}

		int result = 0;
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				result++;
			}
		}

		return result;
	}

	/// <summary>
	/// Computes the sum of sequence of <typeparamref name="TResult"/> values that are obtained
	/// by invoking a transform function on each element of the input sequence.
	/// </summary>
	/// <typeparam name="T">The type of the elements in the array.</typeparam>
	/// <typeparam name="TResult">
	/// The result selected. The type must be a number type that implements interface type <see cref="INumber{TSelf}"/>.
	/// For example, a <see cref="decimal"/> type.
	/// </typeparam>
	/// <param name="this">The array.</param>
	/// <param name="selector">
	/// The selector method that converts each elements to an instance of type <typeparamref name="TResult"/>.
	/// </param>
	/// <returns>The sum of the projected values.</returns>
	public static TResult Sum<T, TResult>(this ImmutableArray<T> @this, Func<T, TResult> selector)
		where TResult : INumber<TResult>
	{
		var result = TResult.Zero;
		foreach (var element in @this)
		{
			result += selector(element);
		}

		return result;
	}
}
