﻿namespace System.Collections.Generic;

/// <summary>
/// Defines an enumerator that iterates the one-dimensional array.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
public ref partial struct OneDimensionalArrayEnumerator<T>
{
	/// <summary>
	/// Indicates the length of the array to iterate.
	/// The value is equal to <c><see cref="_innerArray"/>.Length</c>.
	/// </summary>
	private readonly int _length;

	/// <summary>
	/// Indicates the array to iterate.
	/// </summary>
	private readonly T[] _innerArray;

	/// <summary>
	/// Indicates the current index being iterated.
	/// </summary>
	private int _index = -1;


	/// <summary>
	/// Initializes a <see cref="OneDimensionalArrayEnumerator{T}"/> instance
	/// via the specified array to iterate.
	/// </summary>
	/// <param name="innerArray">The array to iterate.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal OneDimensionalArrayEnumerator(T[] innerArray) => (_innerArray, _length) = (innerArray, innerArray.Length);


	/// <summary>
	/// Indicates the current instance being iterated. Please note that the value is returned by reference.
	/// </summary>
	public readonly T Current
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _innerArray[_index];
	}


	[GeneratedOverridingMember(GeneratedEqualsBehavior.RefStructDefault)]
	public override readonly partial bool Equals(object? obj);

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.RefStructDefault)]
	public override readonly partial int GetHashCode();

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
	public bool MoveNext() => ++_index < _length;
}
