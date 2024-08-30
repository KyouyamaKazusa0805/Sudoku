namespace System.Linq.Iterators;

/// <summary>
/// Represents an enumerator that will be created after <see cref="ArrayEnumerable.Prepend"/>.
/// </summary>
/// <typeparam name="T">The type of each element.</typeparam>
/// <param name="_array">The array.</param>
/// <param name="value">The final element to be iterated.</param>
/// <seealso cref="ArrayEnumerable.Prepend"/>
public sealed class ArrayPrependIterator<T>(T[] _array, T value) : IIterator<ArrayPrependIterator<T>, T>
{
	/// <summary>
	/// Indicates the internal value.
	/// </summary>
	private readonly T _value = value;

	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -2;


	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public ref readonly T Current => ref _index == -1 ? ref _value : ref _array[_index];

	/// <inheritdoc/>
	T IEnumerator<T>.Current => Current;


	/// <inheritdoc/>
	public bool MoveNext() => ++_index < _array.Length;

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public ArrayPrependIterator<T> GetEnumerator() => this;
}
