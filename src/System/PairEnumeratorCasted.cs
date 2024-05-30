namespace System;

/// <inheritdoc cref="PairEnumerator{T}"/>
/// <typeparam name="T">The type of each element to be iterated.</typeparam>
/// <typeparam name="TFirst">The type of the first element in a pair.</typeparam>
/// <typeparam name="TSecond">The type of the second element in a pair.</typeparam>
[StructLayout(LayoutKind.Auto)]
[DebuggerStepThrough]
[ToString]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.Object_ToString)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref partial struct PairEnumeratorCasted<T, TFirst, TSecond>([PrimaryConstructorParameter(MemberKinds.Field)] ReadOnlySpan<T> sequence)
	where T : notnull
	where TFirst : T
	where TSecond : T
{
	/// <summary>
	/// Indicates the index.
	/// </summary>
	private int _index = -2;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly (TFirst First, TSecond Second) Current => ((TFirst)_sequence[_index], (TSecond)_sequence[_index + 1]);


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => (_index += 2) < _sequence.Length - 1;

	/// <inheritdoc cref="ReverseEnumerator{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly PairEnumeratorCasted<T, TFirst, TSecond> GetEnumerator() => this;
}
