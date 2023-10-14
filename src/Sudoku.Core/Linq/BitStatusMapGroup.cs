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
public readonly partial struct BitStatusMapGroup<TMap, TElement, TKey>([DataMember] TKey key, [DataMember] scoped ref readonly TMap values) :
	IEnumerable<TElement>,
	IGrouping<TKey, TElement>
	where TMap : unmanaged, IBitStatusMap<TMap, TElement>
	where TElement : unmanaged, IBinaryInteger<TElement>
	where TKey : notnull
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out TKey key, out TMap values) => (key, values) = (Key, Values);

	/// <summary>
	/// Returns an enumerator that iterates through a collection.
	/// </summary>
	/// <returns>An <see cref="OneDimensionalArrayEnumerator{T}"/> object that can be used to iterate through the collection.</returns>
	/// <seealso cref="OneDimensionalArrayEnumerator{T}"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OneDimensionalArrayEnumerator<TElement> GetEnumerator() => Values.GetEnumerator();

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

		return result.AsSpan()[..i];
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
	/// A (An) <typeparamref name="TResult"/>[] whose elements are the result of invoking the transform function
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

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Values).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() => ((IEnumerable<TElement>)Values).GetEnumerator();
}
