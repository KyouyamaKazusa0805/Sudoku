namespace System.Collections.Generic;

/// <summary>
/// Provides the extension <c>GetEnumerator</c> methods on collection types.
/// </summary>
public static class CollectionEnumeration
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
	public static OneDimensionalArrayEnumerator<T> EnumerateImmutable<T>(this T[] @this) where T : struct => new(@this);

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

	/// <summary>
	/// Get all possible flags that the current enumeration field set.
	/// </summary>
	/// <typeparam name="T">The type of the enumeration.</typeparam>
	/// <param name="this">The current enumeration type instance.</param>
	/// <returns>All flags.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the type isn't applied the attribute <see cref="FlagsAttribute"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FlagsEnumTypeFieldEnumerator<T> GetEnumerator<T>(this T @this) where T : unmanaged, Enum => new(@this);
}
