using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

/// <summary>
/// Provides with a list of extension <c>GetEnumerator</c> methods for iterating one-dimensional arrays.
/// </summary>
public static class ArrayEnumeration
{
	/// <summary>
	/// Same as for-each method <see cref="Array.ForEach{T}(T[], Action{T})"/>, but iterating on references to corresponding elements.
	/// </summary>
	/// <typeparam name="T">The type of each element in this array.</typeparam>
	/// <param name="this">The array.</param>
	/// <param name="callback">The callback method to handle for each reference to each element.</param>
	public static void ForEachRef<T>(this T[] @this, ActionRef<T> callback)
	{
		foreach (ref var element in @this.AsSpan())
		{
			callback(ref element);
		}
	}

	/// <inheritdoc cref="ForEachRef{T}(T[], ActionRef{T})"/>
	public static unsafe void ForEachRefUnsafe<T>(this T[] @this, delegate*<ref T, void> callback)
	{
		foreach (ref var element in @this.AsSpan())
		{
			callback(ref element);
		}
	}

	/// <inheritdoc cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})"/>.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReverseIterator<T> EnumerateReversely<T>(this T[] @this) => new(@this);

	/// <summary>
	/// Creates a <see cref="ArrayPairIterator{T, TFirst, TSecond}"/> instance that iterates on each element of pair elements.
	/// </summary>
	/// <typeparam name="T">The type of the array elements.</typeparam>
	/// <typeparam name="TFirst">The first element returned.</typeparam>
	/// <typeparam name="TSecond">The second element returned.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>An enumerable collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ArrayPairIterator<T, TFirst, TSecond> EnumerateAsPair<T, TFirst, TSecond>(this T[] @this)
		where T : notnull where TFirst : notnull, T where TSecond : notnull, T => new(@this);
}
