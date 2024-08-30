namespace System.Linq.Iterators;

/// <summary>
/// Represents an iterator type.
/// </summary>
/// <typeparam name="TSelf">The type itself.</typeparam>
/// <typeparam name="T">The type of each element.</typeparam>
public interface IIterator<TSelf, T> : IEnumerable<T>, IEnumerator<T> where TSelf : IIterator<TSelf, T>
{
	/// <inheritdoc/>
	object? IEnumerator.Current => Current;


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public new sealed TSelf GetEnumerator() => (TSelf)this;

	/// <inheritdoc/>
	[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "<Pending>")]
	void IDisposable.Dispose() { }

	/// <inheritdoc/>
	void IEnumerator.Reset() => throw new NotImplementedException();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => this;

	/// <inheritdoc/>
	IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
}
