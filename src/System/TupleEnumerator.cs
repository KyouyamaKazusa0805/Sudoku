namespace System.Runtime.CompilerServices;

/// <summary>
/// Represents for an enumerator that iterates on each elements stored in a <see cref="ITuple"/>.
/// </summary>
/// <param name="tuple">A tuple instance.</param>
public ref struct TupleEnumerator(ITuple tuple) : IEnumerator, IEnumerator<object?>
{
	/// <summary>
	/// The current index.
	/// </summary>
	private int _index = -1;


	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly object? Current => tuple[_index];


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => ++_index < tuple.Length;

	/// <inheritdoc/>
	readonly void IDisposable.Dispose() { }

	/// <inheritdoc/>
	[DoesNotReturn]
	readonly void IEnumerator.Reset() => throw new NotImplementedException();
}
