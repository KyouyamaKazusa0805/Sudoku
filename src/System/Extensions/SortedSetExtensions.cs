namespace System.Collections.Generic;

/// <summary>
/// Provides with extension methods on <see cref="SortedSet{T}"/>.
/// </summary>
/// <seealso cref="SortedSet{T}"/>
public static class SortedSetExtensions
{
	/// <summary>
	/// Adds a new instance into the current collection.
	/// </summary>
	/// <typeparam name="T">The type of the item to be added.</typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="item">The current item to be added.</param>
	/// <remarks>
	/// <inheritdoc cref="ListExtensions.AddRef{T}(List{T}, ref readonly T)" path="/remarks"/>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddRef<T>(this SortedSet<T> @this, scoped ref readonly T item)
#if NET9_0_OR_GREATER && VIRTUAL_GENERIC_METHOD_UNSAFE_ACCESSOR
		=> @this.AddIfNotPresent(item);
#else
		=> @this.Add(item);
#endif

#if NET9_0_OR_GREATER
	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "AddIfNotPresent")]
	private static extern bool AddIfNotPresent<T>(this SortedSet<T> @this, T item);
#endif
}
