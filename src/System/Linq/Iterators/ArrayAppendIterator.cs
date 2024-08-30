namespace System.Linq.Iterators;

/// <summary>
/// Represents an enumerator that will be created after <see cref="ArrayEnumerable.Append"/>.
/// </summary>
/// <typeparam name="T">The type of each element.</typeparam>
/// <param name="_array">The array.</param>
/// <param name="value">The final element to be iterated.</param>
/// <seealso cref="ArrayEnumerable.Append"/>
public sealed class ArrayAppendIterator<T>(T[] _array, T value) : IIterator<ArrayAppendIterator<T>, T>
{
	/// <summary>
	/// Indicates the internal value.
	/// </summary>
	private readonly T _value = value;

	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -1;


	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public ref readonly T Current => ref _index == _array.Length ? ref _value : ref _array[_index];

	/// <inheritdoc/>
	T IEnumerator<T>.Current => Current;


	/// <inheritdoc/>
	public bool MoveNext() => ++_index < _array.Length + 1;

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public ArrayAppendIterator<T> GetEnumerator() => this;
}
