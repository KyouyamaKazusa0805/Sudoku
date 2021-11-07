namespace System;

/// <summary>
/// Defines an enumerable collection that is only used for itertion on a one-dimensional array.
/// </summary>
/// <typeparam name="T">The type of the array elements.</typeparam>
public readonly ref partial struct OneDimensionalArrayEnumerable<T>
{
	/// <summary>
	/// Indicates the array to iterate.
	/// </summary>
	private readonly T[] _innerArray;


	/// <summary>
	/// Initializes a <see cref="OneDimensionalArrayEnumerable{T}"/> instance with the specified array.
	/// </summary>
	/// <param name="innerArray">The array to iterate.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OneDimensionalArrayEnumerable(T[] innerArray) => _innerArray = innerArray;

	/// <summary>
	/// Gets the enumerator to iterate on each elements that is with the <see langword="ref"/> keyword.
	/// </summary>
	/// <returns>The enumerator type.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(_innerArray);
}