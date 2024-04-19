namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="LinkedList{T}"/>.
/// </summary>
/// <seealso cref="LinkedList{T}"/>
public static class LinkedListExtensions
{
	/// <summary>
	/// Checks whether the collection exists at least one element satisfying the specified condition.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">A <see cref="LinkedList{T}"/> instance.</param>
	/// <param name="match">A method to be called.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool Exists<T>(this LinkedList<T> @this, Func<T, bool> match)
	{
		foreach (var element in @this)
		{
			if (match(element))
			{
				return true;
			}
		}
		return false;
	}
}
