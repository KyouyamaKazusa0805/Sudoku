namespace System.Collections.ObjectModel;

/// <summary>
/// Provides with extension methos on <see cref="Collection{T}"/>.
/// </summary>
/// <seealso cref="Collection{T}"/>
public static class CollectionExtensions
{
	/// <inheritdoc cref="Collection{T}.Insert(int, T)"/>
	/// <param name="this">The current collection.</param>
	/// <param name="index"/>
	/// <param name="item"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Insert<T>(this Collection<T> @this, Index index, T item)
	{
		if (index.Equals(Index.End))
		{
			@this.Add(item);
		}
		else
		{
			@this.Insert(index.GetOffset(@this.Count), item);
		}
	}
}
