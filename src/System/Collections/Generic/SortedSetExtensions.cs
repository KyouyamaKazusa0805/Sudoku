namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="SortedSet{T}"/>.
/// </summary>
/// <seealso cref="SortedSet{T}"/>
public static class SortedSetExtensions
{
	/// <summary>
	/// Adds the elements into the collection.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The current collection.</param>
	/// <param name="values">The elements to be added.</param>
	/// <returns>The number of elements successfully to be added.</returns>
	public static int AddRange<T>(this SortedSet<T> @this, ReadOnlySpan<T> values)
	{
		var result = 0;
		foreach (var element in values)
		{
			if (@this.Add(element))
			{
				result++;
			}
		}
		return result;
	}

	/// <summary>
	/// Try to convert the current instance into an array.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>An array of <typeparamref name="T"/> elements.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] ToArray<T>(this SortedSet<T> @this)
	{
		var result = new T[@this.Count];
		@this.CopyTo(result);
		return result;
	}
}
