namespace System.Collections.Generic;

/// <summary>
/// Provides extension methods on <see cref="ICollection{T}"/> and <see cref="IReadOnlyCollection{T}"/>.
/// </summary>
/// <seealso cref="ICollection{T}"/>
/// <seealso cref="IReadOnlyCollection{T}"/>
public static class CollectionExtensions
{
	/// <summary>
	/// Adds the elements of the specified collection to the end of the
	/// <see cref="ICollection{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="values">
	/// The values you want to add to the end of the collection.
	/// </param>
	public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> values)
	{
		foreach (var value in values)
		{
			@this.Add(value);
		}
	}
}
