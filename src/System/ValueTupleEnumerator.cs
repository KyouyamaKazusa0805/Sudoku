namespace System;

/// <summary>
/// Provides with a mechanism to iterate a value tuple instance of a uniform type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The uniform type of a pair of instances.</typeparam>
[ToString]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
public ref partial struct ValueTupleEnumerator<T>
{
	/// <summary>
	/// Indicates the maximum number of values to be iterated.
	/// </summary>
	private readonly int _limit;

	/// <summary>
	/// Indicates the internal tuple.
	/// </summary>
	private readonly ReadOnlySpan<T> _innerTuple;

	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -1;


	/// <summary>
	/// Initializes a <see cref="ValueTupleEnumerator{T}"/> instance via a <see cref="ValueTuple{T1, T2}"/> instance.
	/// </summary>
	/// <param name="pair">A pair instance to be iterated.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueTupleEnumerator((T, T) pair)
	{
		_innerTuple = (T[])[pair.Item1, pair.Item2];
		_limit = 2;
	}

	/// <summary>
	/// Initializes a <see cref="ValueTupleEnumerator{T}"/> instance via a <see cref="ValueTuple{T1, T2, T3}"/> instance.
	/// </summary>
	/// <param name="triple">A triple instance to be iterated.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueTupleEnumerator((T, T, T) triple)
	{
		_innerTuple = (T[])[triple.Item1, triple.Item2, triple.Item3];
		_limit = 3;
	}

	/// <summary>
	/// Initializes a <see cref="ValueTupleEnumerator{T}"/> instance via a <see cref="ValueTuple{T1, T2, T3, T4}"/> instance.
	/// </summary>
	/// <param name="quadruple">A quadruple instance to be iterated.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueTupleEnumerator((T, T, T, T) quadruple)
	{
		_innerTuple = (T[])[quadruple.Item1, quadruple.Item2, quadruple.Item3, quadruple.Item4];
		_limit = 4;
	}

	/// <summary>
	/// Initializes a <see cref="ValueTupleEnumerator{T}"/> instance via a <see cref="ValueTuple{T1, T2, T3, T4, T5}"/> instance.
	/// </summary>
	/// <param name="quintuple">A quintuple instance to be iterated.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueTupleEnumerator((T, T, T, T, T) quintuple)
	{
		_innerTuple = (T[])[quintuple.Item1, quintuple.Item2, quintuple.Item3, quintuple.Item4, quintuple.Item5];
		_limit = 5;
	}

	/// <summary>
	/// Initializes a <see cref="ValueTupleEnumerator{T}"/> instance via a <see cref="ValueTuple{T1, T2, T3, T4, T5, T6}"/> instance.
	/// </summary>
	/// <param name="sextuple">A sextuple instance to be iterated.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueTupleEnumerator((T, T, T, T, T, T) sextuple)
	{
		_innerTuple = (T[])[sextuple.Item1, sextuple.Item2, sextuple.Item3, sextuple.Item4, sextuple.Item5, sextuple.Item6];
		_limit = 6;
	}

	/// <summary>
	/// Initializes a <see cref="ValueTupleEnumerator{T}"/> instance via a <see cref="ValueTuple{T1, T2, T3, T4, T5, T6, T7}"/> instance.
	/// </summary>
	/// <param name="septuple">A septuple instance to be iterated.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueTupleEnumerator((T, T, T, T, T, T, T) septuple)
	{
		_innerTuple = (T[])[septuple.Item1, septuple.Item2, septuple.Item3, septuple.Item4, septuple.Item5, septuple.Item6, septuple.Item7];
		_limit = 7;
	}


	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public readonly ref readonly T Current => ref _innerTuple[_index];


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext() => ++_index < _limit;
}
