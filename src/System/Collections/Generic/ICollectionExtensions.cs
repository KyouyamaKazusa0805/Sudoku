namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="ICollection{T}"/>.
/// </summary>
/// <seealso cref="ICollection{T}"/>
public static class ICollectionExtensions
{
	/// <summary>
	/// Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list.</typeparam>
	/// <param name="this">The source collection.</param>
	/// <param name="elements">The collection whose elements should be added to the end of the <see cref="ICollection{T}"/>.</param>
	public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> elements)// where T : allows ref struct
	{
		foreach (var element in elements)
		{
			@this.Add(element);
		}
	}
}
