namespace Sudoku.Concepts;

/// <summary>
/// Represents a method that checks for an offset of type <typeparamref name="TElement"/>
/// in the collection of type <typeparamref name="TSelf"/>, referenced from <paramref name="grid"/>,
/// determining whether the offset satisfies the specified condition.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the bit status map. The value can be <see cref="CellMap"/> or <see cref="CandidateMap"/>.
/// </typeparam>
/// <typeparam name="TElement">
/// The type of each element in the collection of type <typeparamref name="TSelf"/>.
/// </typeparam>
/// <param name="offset">The <typeparamref name="TElement"/> offset value to be checked.</param>
/// <param name="grid">The grid as candidate reference.</param>
/// <returns>A <see cref="bool"/> result indicating that.</returns>
public delegate bool BitStatusMapPredicate<TSelf, TElement>(TElement offset, scoped ref readonly Grid grid)
	where TSelf : unmanaged, IBitStatusMap<TSelf, TElement>
	where TElement : unmanaged, IBinaryInteger<TElement>;
