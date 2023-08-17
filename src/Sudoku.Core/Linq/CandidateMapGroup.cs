namespace Sudoku.Linq;

/// <summary>
/// Represents a candidate map group.
/// </summary>
/// <typeparam name="TKey">The type of the key in the group.</typeparam>
/// <param name="key">Indicates the key used.</param>
/// <param name="values">Indicates the candidates.</param>
public readonly partial struct CandidateMapGroup<TKey>([DataMember] TKey key, [DataMember] scoped in CandidateMap values) where TKey : notnull
{
	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An <see cref="Enumerator"/> object that can be used to iterate through the collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(Values);
}
