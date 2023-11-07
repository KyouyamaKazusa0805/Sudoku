using System.Runtime.InteropServices;
using System.SourceGeneration;

namespace System.Collections.Generic;

/// <summary>
/// Represents a reverse enumerator.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="array">The internal array.</param>
[StructLayout(LayoutKind.Auto)]
public ref partial struct ReverseEnumerator<T>([DataMember(MemberKinds.Field)] ReadOnlySpan<T> array)
{
	/// <summary>
	/// Indicates the current index.
	/// </summary>
	private int _index = array.Length;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly ref readonly T Current => ref _array[_index];


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public readonly ReverseEnumerator<T> GetEnumerator() => this;

	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => --_index >= 0;
}
