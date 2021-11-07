namespace System;

/// <summary>
/// Provides extension methods on <see cref="Array"/>.
/// </summary>
/// <seealso cref="Array"/>
public static class ArrayExtensions
{
	/// <summary>
	/// Gets the first element that satisfies the specified condition.
	/// </summary>
	/// <typeparam name="T">The type of the each element.</typeparam>
	/// <param name="this">The array to be iterated.</param>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>
	/// The first element that satisfies the specified condition.
	/// If none found, return <see langword="default"/>(<typeparamref name="T"/>).
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? First<T>(this T?[] @this, Predicate<T?> predicate) =>
		Array.FindIndex(@this, predicate) is var i and not -1 ? @this[i] : default;

	/// <summary>
	/// Gets the first element that satisfies the specified condition, and throws an exception
	/// to report the invalid case if none possible elements found.
	/// </summary>
	/// <typeparam name="T">The type of the each element.</typeparam>
	/// <param name="this">The array to be interated.</param>
	/// <param name="predicate">The condition to be checked.</param>
	/// <returns>
	/// The first element that satisfies the specified condition.
	/// </returns>
	/// <exception cref="ArgumentException">Throws when none possible elements found.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? FirstOnThrow<T>(this T?[] @this, Predicate<T?> predicate) =>
		Array.FindIndex(@this, predicate) is var i and not -1
			? @this[i]
			: throw new ArgumentException("Can't fetch the result due to none satisfied elements.", nameof(@this));

	/// <summary>
	/// Creates a <see cref="OneDimensionalArrayEnumerable{T}"/> instance that iterates on each element.
	/// Different with the default iteration operation, this type will iterate each element by reference,
	/// in order that you can write the code like:
	/// <code><![CDATA[
	/// foreach (ref int element in new[] { 1, 3, 6, 10 })
	/// {
	///	    Console.WriteLine(++element);
	/// }
	/// ]]></code>
	/// </summary>
	/// <typeparam name="T">The type of the array.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>
	/// The enumerable collection that allows the iteration by reference on an one-dimensional array.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OneDimensionalArrayEnumerable<T> AsRefEnumerable<T>(this T[] @this) => new(@this);
}
