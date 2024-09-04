namespace Sudoku.Concepts;

/// <summary>
/// Represents a method that checks for an offset of type <typeparamref name="TElement"/>
/// in the collection of type <typeparamref name="TSelf"/>, referenced from <paramref name="grid"/>,
/// determining whether the offset satisfies the specified condition.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
/// <typeparam name="TElement">
/// The type of each element in the collection of type <typeparamref name="TSelf"/>.
/// </typeparam>
/// <typeparam name="TEnumerator">The type of enumerator.</typeparam>
/// <param name="offset">The <typeparamref name="TElement"/> offset value to be checked.</param>
/// <param name="grid">The grid as candidate reference.</param>
/// <returns>A <see cref="bool"/> result indicating that.</returns>
public delegate bool CellMapOrCandidateMapPredicate<TSelf, TElement, TEnumerator>(TElement offset, ref readonly Grid grid)
	where TSelf : unmanaged, ICellMapOrCandidateMap<TSelf, TElement, TEnumerator>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TEnumerator : struct, IEnumerator<TElement>, allows ref struct;
