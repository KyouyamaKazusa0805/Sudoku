namespace System.Collections.Generic;

/// <summary>
/// Represents a reverse enumerator.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="array">The array.</param>
public ref struct ReverseEnumerator<T>(T[] array)
{
	/// <summary>
	/// The internal array.
	/// </summary>
	private readonly T[] _array = array;

	/// <summary>
	/// Indicates the current index.
	/// </summary>
	private int _index = array.Length;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public ref T Current => ref _array[_index];


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public readonly ReverseEnumerator<T> GetEnumerator() => this;

	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => --_index >= 0;
}
