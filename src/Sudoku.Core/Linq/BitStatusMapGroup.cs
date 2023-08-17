namespace Sudoku.Linq;

/// <summary>
/// Represents a map group for <see cref="CandidateMap"/> and <see cref="CellMap"/>.
/// </summary>
/// <typeparam name="TMap">The type of the map that stores the <see cref="Values"/>.</typeparam>
/// <typeparam name="TElement">The type of elements stored in <see cref="Values"/>.</typeparam>
/// <typeparam name="TKey">The type of the key in the group.</typeparam>
/// <param name="key">Indicates the key used.</param>
/// <param name="values">Indicates the candidates.</param>
public readonly partial struct BitStatusMapGroup<TMap, TElement, TKey>([DataMember] TKey key, [DataMember] scoped in TMap values)
	where TMap : unmanaged, IBitStatusMap<TMap, TElement>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TKey : notnull
{
	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An <see cref="Enumerator"/> object that can be used to iterate through the collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(Values);
}
