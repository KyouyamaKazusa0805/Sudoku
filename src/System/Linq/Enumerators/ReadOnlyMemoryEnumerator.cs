namespace System.Linq.Enumerators;

/// <summary>
/// Represents an enumerator that iterates on each element of type <typeparamref name="T"/> inside a <see cref="ReadOnlyMemory{T}"/>.
/// </summary>
/// <typeparam name="T">The type of each element.</typeparam>
/// <seealso cref="ReadOnlyMemory{T}"/>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(
	TypeImplFlags.AllObjectMethods | TypeImplFlags.Disposable,
	OtherModifiersOnDisposableDispose = "readonly",
	ExplicitlyImplsDisposable = true)]
public ref partial struct ReadOnlyMemoryEnumerator<T>(ReadOnlyMemory<T> value) : IEnumerator<T>
{
	/// <summary>
	/// Indicates the span to the memory.
	/// </summary>
	private readonly ReadOnlySpan<T> _span = value.Span;

	/// <summary>
	/// Indicates the index that is currently iterated.
	/// </summary>
	private int _index = -1;


	/// <summary>
	/// Indicates the current instance.
	/// </summary>
	public readonly ref readonly T Current => ref _span[_index];

	/// <inheritdoc/>
	readonly object? IEnumerator.Current => Current;

	/// <inheritdoc/>
	readonly T IEnumerator<T>.Current => Current;


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => ++_index < value.Length;

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();
}
