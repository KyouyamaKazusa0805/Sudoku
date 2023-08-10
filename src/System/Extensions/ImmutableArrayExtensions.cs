namespace System.Collections.Immutable;

/// <summary>
/// Provides with extension methods on <see cref="ImmutableArray{T}"/>.
/// </summary>
/// <see cref="ImmutableArray{T}"/>
public static class ImmutableArrayExtensions
{
	/// <summary>
	/// Determines whether two sequences are equal according to an equality comparer method, specified as a function pointer.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The fist collection to be compared.</param>
	/// <param name="other">The second collection to be compared.</param>
	/// <param name="comparison">
	/// The function pointer that points to a function that compares two <typeparamref name="T"/> instances,
	/// and returns a <see cref="bool"/> value indicating whether they are considered equal.
	/// </param>
	/// <returns><see langword="true"/> to indicate the sequences are equal; otherwise, <see langword="false"/>.</returns>
	public static unsafe bool CollectionElementEquals<T>(this ImmutableArray<T> @this, ImmutableArray<T> other, delegate*<T, T, bool> comparison)
	{
		if (@this.IsDefault || other.IsDefault)
		{
			return false;
		}

		if (@this.Length != other.Length)
		{
			return false;
		}

		for (var i = 0; i < @this.Length; i++)
		{
			if (!comparison(@this[i], other[i]))
			{
				return false;
			}
		}

		return true;
	}

#pragma warning disable CS1026, CS1584, CS1658
	/// <summary>
	/// Determines whether two sequences are equal according to an equality comparer method, specified as a function pointer.
	/// Different with <see cref="CollectionElementEquals{T}(ImmutableArray{T}, ImmutableArray{T}, delegate*{T, T, bool})"/>,
	/// this method requires references instead of the value to optimize the argument passing rule.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The fist collection to be compared.</param>
	/// <param name="other">The second collection to be compared.</param>
	/// <param name="comparison">
	/// The function pointer that points to a function that compares two <typeparamref name="T"/> instances,
	/// and returns a <see cref="bool"/> value indicating whether they are considered equal.
	/// </param>
	/// <returns><see langword="true"/> to indicate the sequences are equal; otherwise, <see langword="false"/>.</returns>
	/// <seealso cref="CollectionElementEquals{T}(ImmutableArray{T}, ImmutableArray{T}, delegate*{T, T, bool})"/>
#pragma warning restore CS1026, CS1584, CS1658
	public static unsafe bool CollectionElementRefEquals<T>(this ImmutableArray<T> @this, ImmutableArray<T> other, delegate*<in T, in T, bool> comparison)
	{
		if (@this.IsDefault || other.IsDefault)
		{
			return false;
		}

		if (@this.Length != other.Length)
		{
			return false;
		}

		for (var i = 0; i < @this.Length; i++)
		{
			if (!comparison(@this.ItemRef(i), other.ItemRef(i)))
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc cref="Enumerable.Count{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static int Count<T>(this ImmutableArray<T> @this, Predicate<T> predicate)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(@this.IsDefault, false);

		var result = 0;
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
		ArgumentOutOfRangeException.ThrowIfNotEqual(@this.IsDefault, false);

		var result = 0;
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
	public static TResult Sum<T, TResult>(this ImmutableArray<T> @this, Func<T, TResult> selector) where TResult : INumber<TResult>
	{
		var result = TResult.Zero;
		foreach (var element in @this)
		{
			result += selector(element);
		}

		return result;
	}

	/// <summary>
	/// Casts the current array elements into target typed values.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array to be casted.</param>
	/// <returns>The casted array.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<T> CastToNotNull<T>(this ImmutableArray<T?> @this) where T : struct
		=> ImmutableArray.CreateRange(from element in @this select element!.Value);

	/// <summary>
	/// Casts the current array elements into target typed values.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array to be casted.</param>
	/// <returns>The casted array.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<T> CastToNotNull<T>(this ImmutableArray<T?> @this) where T : class
		=> ImmutableArray.CreateRange(from element in @this select element!);
}
