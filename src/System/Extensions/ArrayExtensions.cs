namespace System;

/// <summary>
/// Provides extension methods on <see cref="Array"/>.
/// </summary>
/// <seealso cref="Array"/>
public static class ArrayExtensions
{
	/// <summary>
	/// Gets the first element that satisfies the specified condition.
	/// </summary>
	/// <typeparam name="T">The type of the each element.</typeparam>
	/// <param name="this">The array to be iterated.</param>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>
	/// The first element that satisfies the specified condition.
	/// If none found, return <see langword="default"/>(<typeparamref name="T"/>).
	/// </returns>
	public static T? First<T>(this T?[] @this, Predicate<T?> predicate)
	{
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				return element;
			}
		}

		return default;
	}

	/// <summary>
	/// Gets the first element that satisfies the specified condition, and throws an exception
	/// to report the invalid case if none possible elements found.
	/// </summary>
	/// <typeparam name="T">The type of the each element.</typeparam>
	/// <param name="this">The array to be interated.</param>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>
	/// The first element that satisfies the specified condition.
	/// </returns>
	/// <exception cref="ArgumentException">Throws when none possible elements found.</exception>
	public static T? FirstOnThrow<T>(this T?[] @this, Predicate<T?> predicate)
	{
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				return element;
			}
		}

		throw new ArgumentException("Can't fetch the result due to none satisfied elements.", nameof(@this));
	}
}
