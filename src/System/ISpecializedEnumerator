namespace System;

/// <summary>
/// Represents an enumerator type that can iterate elements of type <typeparamref name="TElement"/>,
/// stored in collection of type <typeparamref name="TCollection"/>.
/// </summary>
/// <typeparam name="TCollection">The type of collection.</typeparam>
/// <typeparam name="TElement">The type of each element.</typeparam>
internal interface ISpecializedEnumerator<TCollection, TElement> where TCollection : IEnumerable<TElement>
{
	/// <inheritdoc cref="IEnumerator{T}.Current"/>
	public TElement Current { get; }


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public abstract bool MoveNext();
}
