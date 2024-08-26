namespace System.Collections.ObjectModel;

/// <summary>
/// Provides with extension methods on <see cref="Collection{T}"/>.
/// </summary>
/// <seealso cref="Collection{T}"/>
public static class CollectionExtensions
{
	/// <inheritdoc cref="Collection{T}.RemoveAt(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveAt<T>(this Collection<T> @this, Index index) => @this.RemoveAt(index.GetOffset(@this.Count));
}
