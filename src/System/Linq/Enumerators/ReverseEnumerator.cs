namespace System.Linq.Enumerators;

/// <summary>
/// Represents a reverse enumerator.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="sequence">The internal sequence to be iterated.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(
	TypeImplFlag.AllObjectMethods | TypeImplFlag.Disposable,
	OtherModifiersOnDisposableDispose = "readonly",
	ExplicitlyImplsDisposable = true)]
public ref partial struct ReverseEnumerator<T>([Field] ReadOnlySpan<T> sequence) : IEnumerator<T>
{
	/// <summary>
	/// Indicates the current index.
	/// </summary>
	private int _index = sequence.Length;


	/// <summary>
	/// Indicates the length to the sequence.
	/// </summary>
	public readonly int Length => _sequence.Length;

	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly ref readonly T Current => ref _sequence[_index];

	/// <inheritdoc/>
	readonly object? IEnumerator.Current => Current;

	/// <inheritdoc/>
	readonly T IEnumerator<T>.Current => Current;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => --_index >= 0;

	/// <summary>
	/// Provides the basic iteration rule that iterates on each element using the current enumerator.
	/// </summary>
	/// <returns>The current enumerator.</returns>
	public readonly ReverseEnumerator<T> GetEnumerator() => this;

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();
}
