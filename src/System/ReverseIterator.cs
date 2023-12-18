using System.Collections;
using System.Runtime.InteropServices;
using System.SourceGeneration;

namespace System;

/// <summary>
/// Represents a reverse enumerator.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="array">The internal array.</param>
[StructLayout(LayoutKind.Auto)]
[Equals]
[GetHashCode]
[ToString]
public ref partial struct ReverseIterator<T>([Data(DataMemberKinds.Field)] ReadOnlySpan<T> array)
{
	/// <summary>
	/// Indicates the current index.
	/// </summary>
	private int _index = array.Length;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly ref readonly T Current => ref _array[_index];


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public readonly ReverseIterator<T> GetEnumerator() => this;

	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => --_index >= 0;
}
