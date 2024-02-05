namespace Sudoku.Linq;

/// <summary>
/// Represents a map group for <see cref="CandidateMap"/> and <see cref="CellMap"/>.
/// </summary>
/// <typeparam name="TMap">The type of the map that stores the <see cref="Values"/>.</typeparam>
/// <typeparam name="TElement">The type of elements stored in <see cref="Values"/>.</typeparam>
/// <typeparam name="TEnumerator">The type of enumerator.</typeparam>
/// <typeparam name="TKey">The type of the key in the group.</typeparam>
/// <param name="key">Indicates the key used.</param>
/// <param name="values">Indicates the candidates.</param>
/// <seealso cref="CellMap"/>
/// <seealso cref="CandidateMap"/>
[Equals]
[GetHashCode]
[EqualityOperators]
[LargeStructure]
public readonly partial struct BitStatusMapGroup<TMap, TElement, TEnumerator, TKey>(
	[PrimaryConstructorParameter] TKey key,
	[PrimaryConstructorParameter, HashCodeMember] scoped ref readonly TMap values
) :
	IEnumerable<TElement>,
	IEquatable<BitStatusMapGroup<TMap, TElement, TEnumerator, TKey>>,
	IEqualityOperators<BitStatusMapGroup<TMap, TElement, TEnumerator, TKey>, BitStatusMapGroup<TMap, TElement, TEnumerator, TKey>, bool>,
	IGrouping<TKey, TElement>
	where TMap : unmanaged, IBitStatusMap<TMap, TElement, TEnumerator>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TEnumerator : struct, IEnumerator<TElement>
	where TKey : notnull
{
	/// <summary>
	/// Indicates the number of values stored in <see cref="Values"/>, i.e. the shorthand of expression <c>Values.Count</c>.
	/// </summary>
	/// <seealso cref="Values"/>
	public int Count => Values.Count;


	/// <inheritdoc cref="IBitStatusMap{TSelf, TElement, TEnumerator}.this[int]"/>
	public TElement this[int index] => Values[index];


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out TKey key, out TMap values) => (key, values) = (Key, Values);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(IEquatable<>))]
	public bool Equals(scoped ref readonly BitStatusMapGroup<TMap, TElement, TEnumerator, TKey> other) => Values == other.Values;

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An enumerator object that can be used to iterate through the collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TEnumerator GetEnumerator() => Values.GetEnumerator();

	/// <summary>
	/// Makes a <see cref="CellMap"/> instance that is concatenated by a list of groups
	/// of type <see cref="BitStatusMapGroup{TMap, TElement, TEnumerator, TKey}"/>, adding their keys.
	/// </summary>
	/// <param name="groups">The groups.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	public static CellMap CreateMapByKeys(scoped ReadOnlySpan<BitStatusMapGroup<TMap, TElement, TEnumerator, Cell>> groups)
	{
		var result = CellMap.Empty;
		foreach (ref readonly var group in groups)
		{
			result.Add(group.Key);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Values).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => ((IEnumerable<TElement>)Values).GetEnumerator();
}
