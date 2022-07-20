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
}
