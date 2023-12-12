using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Concepts;
using Sudoku.Concepts.Primitive;

namespace Sudoku.Linq;

/// <summary>
/// Represents a map group for <see cref="CandidateMap"/> and <see cref="CellMap"/>.
/// </summary>
/// <typeparam name="TMap">The type of the map that stores the <see cref="Values"/>.</typeparam>
/// <typeparam name="TElement">The type of elements stored in <see cref="Values"/>.</typeparam>
/// <typeparam name="TKey">The type of the key in the group.</typeparam>
/// <param name="key">Indicates the key used.</param>
/// <param name="values">Indicates the candidates.</param>
/// <seealso cref="CellMap"/>
/// <seealso cref="CandidateMap"/>
[Equals]
[GetHashCode]
[EqualityOperators]
[LargeStructure]
public readonly partial struct BitStatusMapGroup<TMap, TElement, TKey>([Data] TKey key, [Data, HashCodeMember] scoped ref readonly TMap values) :
	IEnumerable<TElement>,
	IEquatable<BitStatusMapGroup<TMap, TElement, TKey>>,
	IEqualityOperators<BitStatusMapGroup<TMap, TElement, TKey>, BitStatusMapGroup<TMap, TElement, TKey>, bool>,
	IGrouping<TKey, TElement>
	where TMap : unmanaged, IBitStatusMap<TMap, TElement>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TKey : notnull
{
	/// <summary>
	/// Indicates the number of values stored in <see cref="Values"/>, i.e. the shorthand of expression <c>Values.Count</c>.
	/// </summary>
	/// <seealso cref="Values"/>
	public int Count => Values.Count;


	/// <inheritdoc cref="IBitStatusMap{TSelf, TElement}.this[int]"/>
	public TElement this[int index] => Values[index];


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out TKey key, out TMap values) => (key, values) = (Key, Values);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExplicitInterfaceImpl(typeof(IEquatable<>))]
	public bool Equals(scoped ref readonly BitStatusMapGroup<TMap, TElement, TKey> other) => Values == other.Values;

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An enumerator object that can be used to iterate through the collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<TElement>.Enumerator GetEnumerator() => Values.GetEnumerator();

	/// <summary>
	/// Filters a sequence of values based on a predicate.
	/// </summary>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <returns>A (An) <typeparamref name="TElement"/>[] that contains elements from the input sequence that satisfy the condition.</returns>
	public ReadOnlySpan<TElement> Where(Func<TElement, bool> predicate)
	{
		var result = new TElement[Values.Count];
		var i = 0;
		foreach (var element in Values)
		{
			if (predicate(element))
			{
				result[i++] = element;
			}
		}

		return result.AsReadOnlySpan()[..i];
	}

	/// <summary>
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/summary"/>
	/// </summary>
	/// <typeparam name="TResult">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/typeparam[@name='TResult']"/>
	/// </typeparam>
	/// <param name="selector">
	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})" path="/param[@name='selector']"/>
	/// </param>
	/// <returns>
	/// An array of <typeparamref name="TResult"/> instances whose elements are the result of invoking the transform function
	/// on each element of the current instance.
	/// </returns>
	public ReadOnlySpan<TResult> Select<TResult>(Func<TElement, TResult> selector)
	{
		var result = new TResult[Values.Count];
		var i = 0;
		foreach (var element in Values)
		{
			result[i++] = selector(element);
		}

		return result;
	}

	/// <summary>
	/// Makes a <see cref="CellMap"/> instance that is concatenated by a list of groups
	/// of type <see cref="BitStatusMapGroup{TMap, TElement, TKey}"/>, adding their keys.
	/// </summary>
	/// <param name="groups">The groups.</param>
	/// <returns>A <see cref="CellMap"/> instance.</returns>
	public static CellMap CreateMapByKeys(scoped ReadOnlySpan<BitStatusMapGroup<TMap, TElement, Cell>> groups)
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
