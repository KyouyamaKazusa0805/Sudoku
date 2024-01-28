namespace System.Collections.Generic;

/// <summary>
/// Provides extension methods on <see cref="List{T}"/>.
/// </summary>
/// <seealso cref="List{T}"/>
public static class ListExtensions
{
	/// <summary>
	/// Removes the last element stored in the current <see cref="List{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The list to be modified.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Remove<T>(this List<T> @this) => @this.RemoveAt(@this.Count - 1);

	/// <inheritdoc cref="List{T}.RemoveAt(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveAt<T>(this List<T> @this, Index index) => @this.RemoveAt(index.GetOffset(@this.Count));

	/// <inheritdoc cref="CollectionsMarshal.AsSpan{T}(List{T}?)"/>
	/// <param name="this">The instance to be transformed.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this List<T> @this) => CollectionsMarshal.AsSpan(@this);

	/// <summary>
	/// Gets a <see cref="ReadOnlySpan{T}"/> view over the data in a list. Items should not be added or removed from the <see cref="List{T}"/>
	/// while the <see cref="ReadOnlySpan{T}"/> is in use.
	/// </summary>
	/// <param name="this">The instance to be transformed.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> instance over the <see cref="List{T}"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> AsReadOnlySpan<T>(this List<T> @this) => CollectionsMarshal.AsSpan(@this);
}
