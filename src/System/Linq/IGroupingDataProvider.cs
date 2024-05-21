namespace System.Linq;

/// <summary>
/// Represents a grouping data structure for LINQ grouping methods.
/// </summary>
public interface IGroupingDataProvider<TSelf, out TKey, TElement> :
	IEnumerable<TElement>,
	IEqualityOperators<TSelf, TSelf, bool>,
	IEquatable<TSelf>,
	IGrouping<TKey, TElement>,
	IReadOnlyCollection<TElement>
	where TSelf : IGroupingDataProvider<TSelf, TKey, TElement>
	where TKey : notnull
{
	/// <summary>
	/// Indicates the backing elements.
	/// </summary>
	protected abstract ReadOnlySpan<TElement> Elements { get; }

	/// <inheritdoc/>
	int IReadOnlyCollection<TElement>.Count => Elements.Length;


	/// <summary>
	/// Gets the element at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	public abstract ref readonly TElement this[int index] { get; }


	/// <inheritdoc cref="ReadOnlySpan{T}.GetPinnableReference"/>
	public abstract ref readonly TElement GetPinnableReference();

	/// <summary>
	/// Creates an enumerator that can enumerate each element in the source collection.
	/// </summary>
	/// <returns>An enumerator instance.</returns>
	public abstract new ReadOnlySpan<TElement>.Enumerator GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TElement>)Elements.ToArray()).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => ((IEnumerable<TElement>)Elements.ToArray()).GetEnumerator();
}