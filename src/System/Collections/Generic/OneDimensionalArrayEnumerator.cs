using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;

namespace System.Collections.Generic;

/// <summary>
/// Defines an enumerator that iterates the one-dimensional array.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="innerArray">An array to be iterated.</param>
[StructLayout(LayoutKind.Auto)]
[Equals]
[GetHashCode]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref partial struct OneDimensionalArrayEnumerator<T>([DataMember(MemberKinds.Field)] T[] innerArray)
{
	/// <summary>
	/// Indicates the current index being iterated.
	/// </summary>
	private int _index = -1;


	/// <summary>
	/// Indicates the length of the array.
	/// This property can be used for implementing LINQ logic to create an array that can store the specified number of elements.
	/// </summary>
	public readonly int Length { get; } = innerArray.Length;

	/// <summary>
	/// Indicates the current instance being iterated. Please note that the value is returned by reference.
	/// </summary>
	public readonly T Current => _innerArray[_index];


	/// <summary>
	/// Gets the enumerator to iterate on each elements that is with the <see langword="ref"/> keyword.
	/// </summary>
	/// <returns>The enumerator type.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly OneDimensionalArrayEnumerator<T> GetEnumerator() => this;

	/// <summary>
	/// Retrieve the iterator to make it points to the next element.
	/// </summary>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the moving operation is successful.
	/// Returns <see langword="false"/> when the last iteration is for the last element,
	/// and now there's no elements to be iterated.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext() => ++_index < Length;
}
