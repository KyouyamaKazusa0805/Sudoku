namespace Sudoku.Linq;

using CandidateMapPredicate = CellMapOrCandidateMapPredicate<CandidateMap, Candidate, CandidateMap.Enumerator>;

/// <summary>
/// Represents a list of LINQ methods that can operate with <see cref="CandidateMap"/> instances.
/// </summary>
/// <seealso cref="CandidateMap"/>
public static class CandidateMapEnumerable
{
	/// <summary>
	/// Finds the first candidate that satisfies the specified condition.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="match">The condition to be used.</param>
	/// <returns>The first found candidate.</returns>
	/// <exception cref="InvalidOperationException">Throws when no elements found.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidate First(this scoped in CandidateMap @this, Func<Candidate, bool> match)
		=> @this.FirstOrNull(match).Unwrap();

	/// <summary>
	/// Finds the first candidate that satisfies the specified condition.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="grid">The grid to be used.</param>
	/// <param name="match">The condition to be used.</param>
	/// <returns>The first found candidate.</returns>
	/// <exception cref="InvalidOperationException">Throws when no elements found.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Candidate First(this scoped in CandidateMap @this, ref readonly Grid grid, CandidateMapPredicate match)
		=> @this.FirstOrNull(in grid, match).Unwrap();

	/// <summary>
	/// Finds the first candidate that satisfies the specified condition.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="match">The condition to be used.</param>
	/// <returns>The first found candidate.</returns>
	public static Candidate? FirstOrNull(this scoped in CandidateMap @this, Func<Candidate, bool> match)
	{
		foreach (var candidate in @this.Offsets)
		{
			if (match(candidate))
			{
				return candidate;
			}
		}
		return null;
	}

	/// <summary>
	/// Finds the first candidate that satisfies the specified condition.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="grid">The grid to be used.</param>
	/// <param name="match">The condition to be used.</param>
	/// <returns>The first found candidate.</returns>
	public static Candidate? FirstOrNull(this scoped in CandidateMap @this, ref readonly Grid grid, CandidateMapPredicate match)
	{
		foreach (var candidate in @this.Offsets)
		{
			if (match(candidate, in grid))
			{
				return candidate;
			}
		}
		return null;
	}

	/// <inheritdoc cref="CellMapEnumerable.Select{TResult}(in CellMap, Func{int, TResult})"/>
	public static ReadOnlySpan<TResult> Select<TResult>(this scoped in CandidateMap @this, Func<Candidate, TResult> selector)
	{
		var offsets = @this.Offsets;
		var result = new TResult[offsets.Length];
		for (var i = 0; i < offsets.Length; i++)
		{
			result[i] = selector(offsets[i]);
		}
		return result;
	}

	/// <summary>
	/// Filters a <see cref="CandidateMap"/> collection based on a predicate.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <returns>
	/// A <see cref="CandidateMap"/> that contains elements from the input <see cref="CandidateMap"/> satisfying the condition.
	/// </returns>
	public static CandidateMap Where(this scoped in CandidateMap @this, Func<Candidate, bool> predicate)
	{
		var result = @this;
		foreach (var cell in @this.Offsets)
		{
			if (!predicate(cell))
			{
				result.Remove(cell);
			}
		}
		return result;
	}

	/// <inheritdoc cref="CellMapEnumerable.GroupBy{TKey}(in CellMap, Func{int, TKey})"/>
	public static ReadOnlySpan<BitStatusMapGrouping<CandidateMap, Candidate, CandidateMap.Enumerator, TKey>> GroupBy<TKey>(
		this scoped in CandidateMap @this,
		Func<Candidate, TKey> keySelector
	) where TKey : notnull
	{
		var dictionary = new Dictionary<TKey, CandidateMap>();
		foreach (var candidate in @this)
		{
			var key = keySelector(candidate);
			if (!dictionary.TryAdd(key, [candidate]))
			{
				var originalElement = dictionary[key];
				originalElement.Add(candidate);
				dictionary[key] = originalElement;
			}
		}

		var result = new BitStatusMapGrouping<CandidateMap, Candidate, CandidateMap.Enumerator, TKey>[dictionary.Count];
		var i = 0;
		foreach (var (key, value) in dictionary)
		{
			result[i++] = new(key, in value);
		}
		return result;
	}

	/// <summary>
	/// Projects each candidate (of type <see cref="Candidate"/>) of a <see cref="CandidateMap"/> to a mask (of type <see cref="Mask"/>),
	/// flattens the resulting sequence into one sequence, and invokes a result selector function on each element therein.
	/// </summary>
	/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
	/// <param name="this">A sequence of values to project.</param>
	/// <param name="collectionSelector">A transform function to apply to each element of the input <see cref="CandidateMap"/>.</param>
	/// <param name="resultSelector">A transform function to apply to each element of the intermediate mask (of type <see cref="Mask"/>).</param>
	/// <returns>
	/// A <see cref="ReadOnlySpan{T}"/> of <typeparamref name="TResult"/> whose elements are the result
	/// of invoking the one-to-many transform function <paramref name="collectionSelector"/> on each element of <paramref name="this"/>
	/// and then mapping each of those sequences and their corresponding source element to a result element.
	/// </returns>
	public static ReadOnlySpan<TResult> SelectMany<TResult>(
		this scoped in CandidateMap @this,
		Func<Candidate, Mask> collectionSelector,
		Func<Candidate, Digit, TResult> resultSelector
	)
	{
		var result = new List<TResult>(@this.Count << 1);
		foreach (var candidate in @this)
		{
			foreach (var digit in collectionSelector(candidate))
			{
				result.AddRef(resultSelector(candidate, digit));
			}
		}
		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Indicates whether at least one element satisfies the specified condition.
	/// </summary>
	/// <param name="this">The cell to be checked.</param>
	/// <param name="match">The match method.</param>
	/// <returns>A <see cref="bool"/> result indicating whether at least one element satisfies the specified condition.</returns>
	public static bool Any(this scoped in CandidateMap @this, Func<Candidate, bool> match)
	{
		foreach (var candidate in @this)
		{
			if (match(candidate))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Determine whether all <see cref="Candidate"/>s satisfy the specified condition.
	/// </summary>
	/// <param name="this">The candidate to be checked.</param>
	/// <param name="match">The match method.</param>
	/// <returns>A <see cref="bool"/> result indicating whether all elements satisfy the specified condition.</returns>
	public static bool All(this scoped in CandidateMap @this, Func<Candidate, bool> match)
	{
		foreach (var candidate in @this)
		{
			if (!match(candidate))
			{
				return false;
			}
		}
		return true;
	}
}
