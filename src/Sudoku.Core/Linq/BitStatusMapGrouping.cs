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
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators, IsLargeStructure = true)]
public readonly partial struct BitStatusMapGrouping<TMap, TElement, TEnumerator, TKey>(
	[PrimaryConstructorParameter] TKey key,
	[PrimaryConstructorParameter, HashCodeMember] ref readonly TMap values
) :
	IEnumerable<TElement>,
	IEquatable<BitStatusMapGrouping<TMap, TElement, TEnumerator, TKey>>,
	IEqualityOperators<BitStatusMapGrouping<TMap, TElement, TEnumerator, TKey>, BitStatusMapGrouping<TMap, TElement, TEnumerator, TKey>, bool>,
	IGrouping<TKey, TElement>,
	ISelectMethod<TMap, TElement>,
	IWhereMethod<TMap, TElement>
	where TMap : unmanaged, ICellMapOrCandidateMap<TMap, TElement, TEnumerator>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TEnumerator : struct, IEnumerator<TElement>, allows ref struct
	where TKey : notnull
{
	/// <summary>
	/// Indicates the number of values stored in <see cref="Values"/>, i.e. the shorthand of expression <c>Values.Count</c>.
	/// </summary>
	/// <seealso cref="Values"/>
	public int Count => Values.Count;


	/// <inheritdoc cref="ICellMapOrCandidateMap{TSelf, TElement, TEnumerator}.this[int]"/>
	public TElement this[int index] => Values[index];


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out TKey key, out TMap values) => (key, values) = (Key, Values);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ref readonly BitStatusMapGrouping<TMap, TElement, TEnumerator, TKey> other) => Values == other.Values;

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An enumerator object that can be used to iterate through the collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TEnumerator GetEnumerator() => Values.GetEnumerator();

	/// <summary>
	/// Makes a <see cref="CellMap"/> instance that is concatenated by a list of groups
	/// of type <see cref="BitStatusMapGrouping{TMap, TElement, TEnumerator, TKey}"/>, adding their keys.
	/// </summary>
	/// <param name="groups">The groups.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	public static CellMap CreateMapByKeys(ReadOnlySpan<BitStatusMapGrouping<TMap, TElement, TEnumerator, Cell>> groups)
	{
		var result = CellMap.Empty;
		foreach (ref readonly var group in groups)
		{
			result.Add(group.Key);
		}
		return result;
	}

	/// <inheritdoc/>
	bool IEquatable<BitStatusMapGrouping<TMap, TElement, TEnumerator, TKey>>.Equals(BitStatusMapGrouping<TMap, TElement, TEnumerator, TKey> other) => Equals(in other);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Values).GetEnumerator();

	/// <inheritdoc/>
	IEnumerable<TElement> IWhereMethod<TMap, TElement>.Where(Func<TElement, bool> predicate) => this.Where(predicate).ToArray();

	/// <inheritdoc/>
	IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => ((IEnumerable<TElement>)Values).GetEnumerator();

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectMethod<TMap, TElement>.Select<TResult>(Func<TElement, TResult> selector)
		=> this.Select(selector).ToArray();
}
