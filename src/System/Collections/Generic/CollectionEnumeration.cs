namespace System.Collections.Generic;

/// <summary>
/// Provides the extension <c>GetEnumerator</c> methods on collection types.
/// </summary>
public static class CollectionEnumeration
{
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

	/// <summary>
	/// Get all possible flags that the current enumeration field set.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
	/// <param name="this">The current enumeration type instance.</param>
	/// <returns>All flags.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the type isn't applied the attribute <see cref="FlagsAttribute"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FlagsEnumTypeFieldEnumerator<TEnum> GetEnumerator<TEnum>(this TEnum @this)
		where TEnum : unmanaged, Enum => new(@this);
}
