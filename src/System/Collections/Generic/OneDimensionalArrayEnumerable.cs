namespace System.Collections.Generic;

/// <summary>
/// Defines an enumerable collection that is only used for itertion on a one-dimensional array.
/// Different with the default iterator, this iterator don't allow reference type in order to prevent
/// any modifications on each element.
/// </summary>
/// <typeparam name="TStruct">The type of the array elements.</typeparam>
public partial struct OneDimensionalArrayEnumerable<TStruct> where TStruct : struct
{
	/// <summary>
	/// Indicates the array to iterate.
	/// </summary>
	private readonly TStruct[] _innerArray;


	/// <summary>
	/// Initializes a <see cref="OneDimensionalArrayEnumerable{TStruct}"/> instance with the specified array.
	/// </summary>
	/// <param name="innerArray">The array to iterate.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OneDimensionalArrayEnumerable(TStruct[] innerArray) => _innerArray = innerArray;

	/// <summary>
	/// Gets the enumerator to iterate on each elements that is with the <see langword="ref"/> keyword.
	/// </summary>
	/// <returns>The enumerator type.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(_innerArray);
}
