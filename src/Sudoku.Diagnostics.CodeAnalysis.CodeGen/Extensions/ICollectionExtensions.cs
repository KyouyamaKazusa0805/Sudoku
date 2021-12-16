namespace System.Collections.Generic;

/// <summary>
/// Provides extension methods on <see cref="ICollection{T}"/>.
/// </summary>
/// <seealso cref="ICollection{T}"/>
internal static class ICollectionExtensions
{
	/// <summary>
	/// Performs the specified action on each element of the <see cref="ICollection{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of the each element.</typeparam>
	/// <param name="this">The collection itself.</param>
	/// <param name="action">The action.</param>
	public static void ForEach<T>(this ICollection<T> @this, Action<T> action)
	{
		foreach (var element in @this)
		{
			action(element);
		}
	}
}
