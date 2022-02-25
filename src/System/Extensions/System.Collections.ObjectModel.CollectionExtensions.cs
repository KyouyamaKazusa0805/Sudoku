namespace System.Collections.ObjectModel;

/// <summary>
/// Provides extension methods on <see cref="Collection{T}"/>.
/// </summary>
/// <seealso cref="Collection{T}"/>
public static class CollectionExtensions
{
	/// <summary>
	/// Prepends the specified element into the collection.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <typeparam name="TDerived">
	/// The type of the element to be prepended. The type must be derived from <typeparamref name="T"/>.
	/// </typeparam>
	/// <param name="this">The collection.</param>
	/// <param name="element">The element to be prepended.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Prepend<T, TDerived>(this Collection<T> @this, TDerived element) where TDerived : T =>
		@this.Insert(0, element);
}
