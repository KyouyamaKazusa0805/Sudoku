using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

/// <summary>
/// Provides with a list of extension <c>GetEnumerator</c> methods for iterating one-dimensional arrays.
/// </summary>
public static class ArrayEnumeration
{
	/// <summary>
	/// Creates a <see cref="OneDimensionalArrayEnumerator{T}"/> instance that iterates on each element.
	/// </summary>
	/// <typeparam name="T">The type of the array elements.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>
	/// The enumerable collection that allows the iteration on an one-dimensional array.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OneDimensionalArrayEnumerator<T> Enumerate<T>(this T[] @this) => new(@this);

	/// <summary>
	/// Creates a <see cref="OneDimensionalArrayRefEnumerator{T}"/> instance that iterates on each element.
	/// Different with the default iteration operation, this type will iterate each element by reference,
	/// in order that you can write the code like:
	/// <code><![CDATA[
	/// foreach (ref int element in new[] { 1, 3, 6, 10 }.EnumerateRef())
	/// {
	///	    Console.WriteLine(++element);
	/// }
	/// ]]></code>
	/// </summary>
	/// <typeparam name="T">The type of the array elements.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>
	/// The enumerable collection that allows the iteration by reference on an one-dimensional array.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OneDimensionalArrayRefEnumerator<T> EnumerateRef<T>(this T[] @this) => new(@this);

	/// <summary>
	/// Same as for-each method <see cref="Array.ForEach{T}(T[], Action{T})"/>, but iterating on references to corresponding elements.
	/// </summary>
	/// <typeparam name="T">The type of each element in this array.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="callback">The callback method to handle for each reference to each element.</param>
	public static void ForEachRef<T>(this T[] @this, ActionRef<T> callback)
	{
		foreach (ref var element in @this.EnumerateRef())
		{
			callback(ref element);
		}
	}

	/// <inheritdoc cref="ForEachRef{T}(T[], ActionRef{T})"/>
	public static unsafe void ForEachRefUnsafe<T>(this T[] @this, delegate*<ref T, void> callback)
	{
		foreach (ref var element in @this.EnumerateRef())
		{
			callback(ref element);
		}
	}

	/// <inheritdoc cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})"/>.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReverseEnumerator<T> EnumerateReversely<T>(this T[] @this) => new(@this);

	/// <summary>
	/// Creates a <see cref="ArrayPairEnumerator{T, TFirst, TSecond}"/> instance that iterates on each element of pair elements.
	/// </summary>
	/// <typeparam name="T">The type of the array elements.</typeparam>
	/// <typeparam name="TFirst">The first element returned.</typeparam>
	/// <typeparam name="TSecond">The second element returned.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>An enumerable collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArrayPairEnumerator<T, TFirst, TSecond> EnumerateAsPair<T, TFirst, TSecond>(this T[] @this)
		where T : notnull where TFirst : notnull, T where TSecond : notnull, T => new(@this);
}
