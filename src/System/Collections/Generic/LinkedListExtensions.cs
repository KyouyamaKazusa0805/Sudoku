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

	/// <summary>
	/// Removes the first element and return that element.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <returns>The first element to be removed.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T RemoveFirstNode<T>(this LinkedList<T> @this)
	{
		var node = @this.First!.Value;
		@this.RemoveFirst();
		return node;
	}

	/// <summary>
	/// Removes the last element and return that element.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The collection.</param>
	/// <returns>The last element to be removed.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T RemoveLastNode<T>(this LinkedList<T> @this)
	{
		var node = @this.Last!.Value;
		@this.RemoveLast();
		return node;
	}

	/// <inheritdoc cref="LinkedList{T}.First"/>
	/// <exception cref="NullReferenceException">Throws when the list is empty.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T FirstValue<T>(this LinkedList<T> @this) => @this.First!.Value;

	/// <inheritdoc cref="LinkedList{T}.Last"/>
	/// <exception cref="NullReferenceException">Throws when the list is empty.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T LastValue<T>(this LinkedList<T> @this) => @this.Last!.Value;
}
