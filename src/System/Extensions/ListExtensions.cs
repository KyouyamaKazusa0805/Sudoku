namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="List{T}"/>.
/// </summary>
/// <seealso cref="List{T}"/>
public static class ListExtensions
{
	/// <summary>
	/// Removes the last element of the list, and return that removed element.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <param name="this">The removed list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Remove<T>(this List<T> @this)
	{
		var result = @this[^1];
		@this.RemoveAt(@this.Count - 1);
		return result;
	}

	/// <summary>
	/// Slices the list.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="startIndex">The desired start index.</param>
	/// <param name="count">The desired number of elements.</param>
	/// <returns>The sliced list.</returns>
	public static List<T> Slice<T>(this List<T> @this, int startIndex, int count)
	{
		var result = new List<T>(count);
		for (int i = startIndex, j = 0; j < count; i++, j++)
		{
			result.Add(@this[i]);
		}

		return result;
	}
}
