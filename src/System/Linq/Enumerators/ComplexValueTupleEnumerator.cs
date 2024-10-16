namespace System.Linq.Enumerators;

/// <summary>
/// Defines a complex value tuple enumerator.
/// </summary>
/// <typeparam name="T">The type of each element.</typeparam>
/// <typeparam name="TRest">The type that encapsulate for a list of rest elements.</typeparam>
/// <param name="tuple">The tuple.</param>
[TypeImpl(
	TypeImplFlags.AllObjectMethods | TypeImplFlags.Disposable,
	OtherModifiersOnDisposableDispose = "readonly",
	ExplicitlyImplsDisposable = true)]
public ref partial struct ComplexValueTupleEnumerator<T, TRest>(ValueTuple<T, T, T, T, T, T, T, TRest> tuple) : IEnumerator<T>
	where TRest : struct
{
	/// <summary>
	/// Indicates the internal values to be iterated.
	/// </summary>
	private readonly ReadOnlySpan<T> _values = from T element in tuple select element;

	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -1;


	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public readonly ref readonly T Current => ref _values[_index];

	/// <inheritdoc/>
	readonly object? IEnumerator.Current => Current;

	/// <inheritdoc/>
	readonly T IEnumerator<T>.Current => Current;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext() => ++_index < _values.Length;

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();
}
