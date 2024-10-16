namespace System.Linq.Enumerators;

/// <summary>
/// Represents an enumerator that can iterate on elements, paired.
/// </summary>
/// <typeparam name="T">The type of each element to be iterated.</typeparam>
/// <param name="sequence">The sequence value.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(
	TypeImplFlags.AllObjectMethods | TypeImplFlags.Disposable,
	OtherModifiersOnDisposableDispose = "readonly",
	ExplicitlyImplsDisposable = true)]
public ref partial struct PairEnumerator<T>([Field] ReadOnlySpan<T> sequence) : IEnumerator<(T First, T Second)>
	where T : notnull
{
	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -2;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly (T First, T Second) Current => (_sequence[_index], _sequence[_index + 1]);

	/// <inheritdoc/>
	readonly object IEnumerator.Current => Current;


	/// <inheritdoc cref="ReverseEnumerator{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly PairEnumerator<T> GetEnumerator() => this;

	/// <summary>
	/// Creates a <see cref="PairEnumeratorCasted{T, TFirst, TSecond}"/> instance that supports down-casting
	/// for paired elements.
	/// </summary>
	/// <typeparam name="TFirst">
	/// <inheritdoc cref="PairEnumeratorCasted{T, TFirst, TSecond}" path="/typeparam[@name='TFirst']"/>
	/// </typeparam>
	/// <typeparam name="TSecond">
	/// <inheritdoc cref="PairEnumeratorCasted{T, TFirst, TSecond}" path="/typeparam[@name='TSecond']"/>
	/// </typeparam>
	/// <returns>A <see cref="PairEnumeratorCasted{T, TFirst, TSecond}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly PairEnumeratorCasted<T, TFirst, TSecond> Cast<TFirst, TSecond>() where TFirst : T where TSecond : T
		=> new(_sequence);

	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => (_index += 2) < _sequence.Length - 1;

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();
}
