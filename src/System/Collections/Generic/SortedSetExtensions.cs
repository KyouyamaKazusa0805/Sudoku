namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="SortedSet{T}"/>.
/// </summary>
/// <seealso cref="SortedSet{T}"/>
public static class SortedSetExtensions
{
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
