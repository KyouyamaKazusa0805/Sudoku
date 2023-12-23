namespace System;

/// <summary>
/// Defines an enumerator that iterates on elements, combined with two adjacent elements.
/// </summary>
/// <typeparam name="T">The type of each element.</typeparam>
/// <typeparam name="TFirst">
/// The first element returned. This type argument must be derived from <typeparamref name="T"/>, or same type.
/// </typeparam>
/// <typeparam name="TSecond">
/// The second element returned. This type argument must be derived from <typeparamref name="T"/>, or same type.
/// </typeparam>
/// <param name="array">The array value.</param>
/// <exception cref="ArgumentException">Throws when <paramref name="array"/> has the odd number of elements.</exception>
[Equals]
[GetHashCode]
[ToString]
public ref partial struct ArrayPairIterator<T, TFirst, TSecond>(T[] array)
	where T : notnull
	where TFirst : notnull, T
	where TSecond : notnull, T
{
	/// <summary>
	/// Indicates the internal array to be iterated.
	/// </summary>
	private readonly T[] _array = (array.Length & 1) == 0 ? array : throw new ArgumentException("The array must contain the even number of elements.");

	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -2;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly (TFirst, TSecond) Current => ((TFirst)_array[_index], (TSecond)_array[_index + 1]);


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public readonly ArrayPairIterator<T, TFirst, TSecond> GetEnumerator() => this;

	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => (_index += 2) < _array.Length - 1;
}
