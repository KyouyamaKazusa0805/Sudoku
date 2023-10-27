using System.Collections;
using System.Runtime.CompilerServices;

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

	/// <summary>
	/// Check whether the specified list has only one element.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <param name="this">The list.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool HasOnlyOneElement<T>(this IEnumerable<T> @this)
	{
		if (@this.None())
		{
			return false;
		}

		var count = 0;
		using var enumerator = @this.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (++count >= 2)
			{
				return false;
			}
		}

		return true;
	}
}
