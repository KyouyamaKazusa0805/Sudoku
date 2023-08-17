namespace Sudoku.Linq;

/// <summary>
/// Represents a candidate map group.
/// </summary>
/// <typeparam name="TKey">The type of the key in the group.</typeparam>
/// <typeparam name="TValue">The type of the values in the group.</typeparam>
/// <param name="key">Indicates the key used.</param>
/// <param name="values">Indicates the values.</param>
public readonly partial struct CandidateMapGroup<TKey, TValue>([DataMember] TKey key, [DataMember] TValue[] values) where TKey : notnull
{
	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An <see cref="Enumerator"/> object that can be used to iterate through the collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(Values);
}
