namespace System;

/// <summary>
/// Represents an enumerator that can iterate on elements, adjacent-pairly combined.
/// </summary>
/// <typeparam name="T">The type of each element to be iterated.</typeparam>
/// <param name="sequence">The sequence value.</param>
[StructLayout(LayoutKind.Auto)]
[DebuggerStepThrough]
[Equals]
[GetHashCode]
[ToString]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref partial struct PairEnumerator<T>([RecordParameter(DataMemberKinds.Field)] ReadOnlySpan<T> sequence) where T : notnull
{
	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -2;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly (T First, T Second) Current => (_sequence[_index], _sequence[_index + 1]);


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => (_index += 2) < _sequence.Length - 1;

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
}
