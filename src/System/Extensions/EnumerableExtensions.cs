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
		decimal result = decimal.MinValue;
		foreach (var element in @this)
		{
			decimal converted = selector(element);
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

		int count = 0;
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
}
