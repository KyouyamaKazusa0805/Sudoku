namespace System.Linq;

/// <summary>
/// The extension for <see cref="Enumerable"/>.
/// </summary>
/// <seealso cref="Enumerable"/>
public static class Enumerable2
{
	/// <summary>
	/// Determines whether an <see cref="IEnumerable"/> collection has no elements in it.
	/// </summary>
	/// <param name="this">The element.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool None(this IEnumerable @this) => !@this.GetEnumerator().MoveNext();

	/// <summary>
	/// Determines whether an <see cref="IEnumerable{T}"/> collection has no elements in it.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The element.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool None<T>(this IEnumerable<T> @this) => !@this.GetEnumerator().MoveNext();
}
