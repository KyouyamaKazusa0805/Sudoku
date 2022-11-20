namespace System.Linq;

/// <summary>
/// Provides a set of static methods for querying objects that implement
/// <see cref="IEnumerable"/> and <see cref="IEnumerable{T}"/>.
/// </summary>
/// <remarks>This class has the same function and status with <see cref="Enumerable"/>.</remarks>
/// <seealso cref="IEnumerable"/>
/// <seealso cref="IEnumerable{T}"/>
/// <seealso cref="Enumerable"/>
public static class EnumerableExtensions
{
	/// <inheritdoc cref="Enumerable.Max(IEnumerable{decimal})"/>
	public static unsafe decimal Max<T>(this IEnumerable<T> @this, delegate*<T, decimal> selector)
	{
		var result = decimal.MinValue;
		foreach (var element in @this)
		{
			var converted = selector(element);
			if (converted >= result)
			{
				result = converted;
			}
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, decimal})"/>
	public static unsafe decimal Sum<T>(this IEnumerable<T> @this, delegate*<T, decimal> selector)
	{
		decimal result = 0;
		foreach (var element in @this)
		{
			result += selector(element);
		}

		return result;
	}

	/// <summary>
	/// Check whether the specified list has only one element.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <param name="this">The list.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool HasOnlyOneElement<T>(this IEnumerable<T> @this)
	{
		if (!@this.Any())
		{
			return false;
		}

		var count = 0;
		using var enumerator = @this.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (++count >= 2)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Try to treat the current instance as an array and convert it. If it is not an array,
	/// call <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/>
	/// to convert it into the valid array instance. This method will not throw exceptions or return <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>The final array converted.</returns>
	/// <seealso cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] CastToArray<T>(this IEnumerable<T> @this) => @this is T[] array ? array : @this.ToArray();

	/// <summary>
	/// Iterates all elements in this enumerable collection.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="visitor">The visitor method to handle and operate with each element.</param>
	public static void ForEach<T>(this IEnumerable<T> @this, Action<T> visitor)
	{
		switch (@this)
		{
			case T[] array:
			{
				Array.ForEach(array, visitor);
				break;
			}
			case List<T> list:
			{
				list.ForEach(visitor);
				break;
			}
			default:
			{
				foreach (var element in @this)
				{
					visitor(element);
				}

				break;
			}
		}
	}
}
