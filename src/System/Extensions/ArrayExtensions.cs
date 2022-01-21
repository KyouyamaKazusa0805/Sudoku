namespace System;

/// <summary>
/// Provides extension methods on <see cref="Array"/>.
/// </summary>
/// <seealso cref="Array"/>
public static class ArrayExtensions
{
	/// <summary>
	/// Gets the reference that points to the first element in the current array.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>The reference to the first element of the array.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T GetPinnableReference<T>(this T[] @this) => ref @this[0];

	/// <summary>
	/// Gets the read-only reference that points to the first element in the current array.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>The read-only reference to the first element of the array.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T GetPinnableReadOnlyReference<T>(this T[] @this) => ref @this[0];

	/// <summary>
	/// Creates a <see cref="OneDimensionalArrayEnumerator{T}"/> instance that iterates on each element.
	/// </summary>
	/// <typeparam name="TStruct">The type of the array elements.</typeparam>
	/// <param name="this">The array.</param>
	/// <returns>
	/// The enumerable collection that allows the iteration on an one-dimensional array.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OneDimensionalArrayEnumerator<TStruct> EnumerateImmutable<TStruct>(this TStruct[] @this)
	where TStruct : struct => new(@this);

	/// <summary>
	/// Creates a <see cref="OneDimensionalArrayRefEnumerator{T}"/> instance that iterates on each element.
	/// Different with the default iteration operation, this type will iterate each element by reference,
	/// in order that you can write the code like:
	/// <code><![CDATA[
	/// foreach (ref int element in new[] { 1, 3, 6, 10 })
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
}
