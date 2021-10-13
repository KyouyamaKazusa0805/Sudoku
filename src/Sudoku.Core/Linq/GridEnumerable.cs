namespace Sudoku.Linq;

/// <summary>
/// Encapsulates a serial of methods that are LINQ methods for type <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static unsafe class GridEnumerable
{
	/// <summary>
	/// Applies an accumulator function over a sequence.
	/// </summary>
	/// <param name="source">A <see cref="Grid"/> to aggregate over.</param>
	/// <param name="func">An accumulator function to be invoked on each element.</param>
	/// <returns>The final accumulator value.</returns>
	/// <exception cref="InvalidOperationException">Throws when there's no elements to be iterated.</exception>
	public static int Aggregate(this in Grid source, [NotNull, DisallowNull] delegate*<int, int, int> func)
	{
		var e = source.GetEnumerator();
		if (!e.MoveNext())
		{
			throw new InvalidOperationException("There's no elements to be iterated.");
		}

		int result = e.Current;
		while (e.MoveNext())
		{
			result = func(result, e.Current);
		}

		return result;
	}

	/// <summary>
	/// Applies an accumulator function over a sequence. The specified seed value is
	/// used as the initial accumulator value.
	/// </summary>
	/// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
	/// <param name="this">A <see cref="Grid"/> to aggregate over.</param>
	/// <param name="seed">The initial accumulator value.</param>
	/// <param name="func">An accumulator function to be invoked on each element.</param>
	/// <returns>The final accumulator value.</returns>
	public static TAccumulate Aggregate<TAccumulate>(
		this in Grid @this,
		TAccumulate seed,
		[NotNull, DisallowNull] delegate*<TAccumulate, int, TAccumulate> func
	)
	{
		var result = seed;
		foreach (int element in @this)
		{
			result = func(result, element);
		}

		return result;
	}

	/// <summary>
	/// Applies an accumulator function over a sequence. The specified seed value is
	/// used as the initial accumulator value, and the specified function is used to
	/// select the result value.
	/// </summary>
	/// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
	/// <typeparam name="TResult">The type of the resulting value.</typeparam>
	/// <param name="this">A <see cref="Grid"/> to aggregate over.</param>
	/// <param name="seed">The initial accumulator value.</param>
	/// <param name="func">An accumulator function to be invoked on each element.</param>
	/// <param name="resultSelector">
	/// A function to transform the final accumulator value into the result value.
	/// </param>
	/// <returns>The transformed final accumulator value.</returns>
	public static TResult Aggregate<TAccumulate, TResult>(
		this in Grid @this,
		TAccumulate seed,
		[NotNull, DisallowNull] delegate*<TAccumulate, int, TAccumulate> func,
		[NotNull, DisallowNull] delegate*<TAccumulate, TResult> resultSelector
	)
	{
		var result = seed;
		foreach (int element in @this)
		{
			result = func(result, element);
		}

		return resultSelector(result);
	}

	/// <summary>
	/// Determines whether a sequence contains any candidate.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> to check for emptiness.</param>
	/// <returns>
	/// <see langword="true"/> if the source sequence contains any candidate; otherwise, <see langword="false"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Any(this in Grid @this) =>
#if true
		@this.IsSolved;
#else
		@this.GetEnumerator().MoveNext();
#endif

	/// <summary>
	/// Determines whether any candidate of a sequence satisfies a condition.
	/// </summary>
	/// <param name="this">A <see cref="Grid"/> whose elements to apply the predicate to.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <returns>
	/// <see langword="true"/> if the source sequence isn't empty and at least one of its candidates passes
	/// the test in the specified predicate; otherwise, <see langword="false"/>.
	/// </returns>
	public static bool Any(this in Grid @this, [NotNull, DisallowNull] delegate*<int, bool> predicate)
	{
		foreach (int element in @this)
		{
			if (predicate(element))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Determines whether all candidates of a sequence satisfy a condition.
	/// </summary>
	/// <param name="this">A <see cref="Grid"/> that contains the elements to apply the predicate to.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <returns>
	/// <see langword="true"/> if every candidate of the source sequence passes the test in the specified
	/// predicate, or if the sequence is empty; otherwise, <see langword="false"/>.
	/// </returns>
	public static bool All(this in Grid @this, [NotNull, DisallowNull] delegate*<int, bool> predicate)
	{
		foreach (int element in @this)
		{
			if (!predicate(element))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Extracts the candidate collection and groups up by the specified size.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="size">The specified size.</param>
	/// <returns>The <see cref="CandidatesChunkEnumerable"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidatesChunkEnumerable Chunk(this in Grid @this, int size) =>
		new(@this.Candidates.GetEnumerator(), size);

	/// <summary>
	/// Determines whether a sequence contains a specified candidate.
	/// </summary>
	/// <param name="this">A sequence in which to locate a candidate.</param>
	/// <param name="candidate">The candidate to locate in the sequence.</param>
	/// <returns>
	/// <see langword="true"/> if the source sequence contains an element that has the specified value;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	public static bool Contains(this in Grid @this, int candidate)
	{
		foreach (int cand in @this)
		{
			if (candidate == cand)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Determines whether a sequence contains a specified candidate by using the specified comparing method.
	/// </summary>
	/// <param name="this">A sequence in which to locate a candidate.</param>
	/// <param name="predicate">The condition to locate in the sequence.</param>
	/// <returns>
	/// <see langword="true"/> if the source sequence contains an element that has the specified value;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	public static bool Contains(this in Grid @this, [NotNull, DisallowNull] delegate*<int, bool> predicate)
	{
		foreach (int cand in @this)
		{
			if (predicate(cand))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Determines whether a sequence contains a specified candidate by using the specified comparing method,
	/// and defines an argument to receive the possible found result.
	/// </summary>
	/// <param name="this">A sequence in which to locate a candidate.</param>
	/// <param name="predicate">The condition to locate in the sequence.</param>
	/// <param name="result">The result found that is filtered by <paramref name="predicate"/>.</param>
	/// <returns>
	/// <see langword="true"/> if the source sequence contains an element that has the specified value;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	public static bool Contains(
		this in Grid @this,
		[NotNull, DisallowNull] delegate*<int, bool> predicate,
		[DiscardWhen(false)] out int result
	)
	{
		foreach (int cand in @this)
		{
			if (predicate(cand))
			{
				result = cand;
				return true;
			}
		}

		result = default;
		return false;
	}

	/// <summary>
	/// Gets the number of candidates found.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <returns>The number of candidates found.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Count(this in Grid @this) => @this.CandidatesCount;

	/// <summary>
	/// Gets the number of candidates found that satisfy the specified condition.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="predicate">The predicate to filter the candidates.</param>
	/// <returns>The number of candidates found that satisfy the specified condition.</returns>
	public static int Count(this in Grid @this, [NotNull, DisallowNull] delegate*<int, bool> predicate)
	{
		int result = 0;
		foreach (int candidate in @this)
		{
			if (predicate(candidate))
			{
				result++;
			}
		}

		return result;
	}

	/// <summary>
	/// Filters and removes the duplicate candidates that is defined and selected by the specified method
	/// and only leaves one.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> instance.</param>
	/// <param name="keySelector">The key selector.</param>
	/// <returns>The collection that only contains the selected values.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CandidatesDistinctEnumerable DistinctBy(
		this in Grid @this,
		[NotNull, DisallowNull] delegate*<int, int, bool> keySelector
	) => new(@this, keySelector);

	/// <summary>
	/// Returns the candidate at a specified index in a sequence.
	/// </summary>
	/// <param name="this">A <see cref="Grid"/> to return a candidate from.</param>
	/// <param name="index">The zero-based index of the candidate to retrieve.</param>
	/// <returns>The candidate at the specified position in the source sequence.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified collection doesn't contain the number of candidates.
	/// </exception>
	public static int ElementAt(this in Grid @this, int index)
	{
		var enumerator = @this.Candidates.GetEnumerator();
		for (int times = 0; times < index; times++)
		{
			if (!enumerator.MoveNext())
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}
		}

		return enumerator.Current;
	}

	/// <summary>
	/// Returns the candidate at a specified index in a sequence or a <see langword="default"/> value if
	/// the index is out of range.
	/// </summary>
	/// <param name="this">A <see cref="Grid"/> to return a candidate from.</param>
	/// <param name="index">
	/// The index of the candidate to retrieve, which is either from the start or the end.
	/// </param>
	/// <returns>
	/// <see langword="default"/> if index is outside the bounds of the source sequence;
	/// otherwise, the element at the specified position in the source sequence.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ElementAt(this in Grid @this, Index index) =>
		@this.ElementAt(index.GetOffset(@this.CandidatesCount));

	/// <summary>
	/// Returns the candidate at a specified index in a sequence or <c>-1</c> if
	/// the index is out of range.
	/// </summary>
	/// <param name="this">A <see cref="Grid"/> to return a candidate from.</param>
	/// <param name="index">The zero-based index of the element to retrieve.</param>
	/// <returns>
	/// <c>-1</c> if the index is outside the bounds of the source sequence;
	/// otherwise, the candidate at the specified position in the source sequence.
	/// </returns>
	public static int ElementAtOrNegativeOne(this in Grid @this, int index)
	{
		var enumerator = @this.Candidates.GetEnumerator();
		for (int times = 0; times < index; times++)
		{
			if (!enumerator.MoveNext())
			{
				return -1;
			}
		}

		return enumerator.Current;
	}

	/// <summary>
	/// Returns the candidate at a specified index in a sequence or a default value if
	/// the index is out of range.
	/// </summary>
	/// <param name="this">A <see cref="Grid"/> to return a candidate from.</param>
	/// <param name="index">
	/// The index of the candidate to retrieve, which is either from the start or the end.
	/// </param>
	/// <returns>
	/// <c>-1</c> if index is outside the bounds of the source sequence;
	/// otherwise, the candidate at the specified position in the source sequence.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ElementAtOrNegativeOne(this in Grid @this, Index index) =>
		@this.ElementAtOrNegativeOne(index.GetOffset(@this.CandidatesCount));

	/// <summary>
	/// Returns the first candidate of a sequence.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> to return the first candidate of.</param>
	/// <returns>The first candidate in the specified sequence.</returns>
	/// <exception cref="InvalidOperationException">The source sequence is empty.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int First(this in Grid @this) =>
		!@this.IsSolved && @this.Candidates.GetEnumerator() is var enumerator && enumerator.MoveNext()
			? enumerator.Current
			: throw new InvalidOperationException("The collection doesn't contain any possible value.");

	/// <summary>
	/// Gets the first candidate of a sequence or <see langword="default"/> if the source sequence is empty.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> to return the first candidate of.</param>
	/// <param name="result">The result to receive the first candidate value.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the collection contains any candidate.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryFirst(this in Grid @this, [DiscardWhen(false)] out int result)
	{
		(bool @return, result) =
			!@this.IsSolved && @this.Candidates.GetEnumerator() is var enumerator && enumerator.MoveNext()
				? (true, enumerator.Current)
				: (false, default);
		return @return;
	}

	/// <summary>
	/// Returns the last candidate of a sequence.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> to return the last candidate of.</param>
	/// <returns>The last candidate in the specified sequence.</returns>
	/// <exception cref="InvalidOperationException">The source sequence is empty.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Last(this in Grid @this)
	{
		if (@this.IsSolved)
		{
			goto ThrowExceptionAboutNoElement;
		}

		int? lastElement = null;
		foreach (int candidate in @this.Candidates)
		{
			lastElement = candidate;
		}

		if (lastElement is { } r)
		{
			return r;
		}

	ThrowExceptionAboutNoElement:
		throw new InvalidOperationException("The source sequence doesn't contain any possible candidate.");
	}

	/// <summary>
	/// Gets the last candidate of a sequence or <see langword="default"/> if the source sequence is empty.
	/// </summary>
	/// <param name="this">The <see cref="Grid"/> to return the last candidate of.</param>
	/// <param name="result">The result to receive the last candidate value.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the collection contains any candidate.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryLast(this in Grid @this, out int result)
	{
		if (@this.IsSolved)
		{
			goto ReturnFalseAboutNoElement;
		}

		int? lastElement = null;
		foreach (int candidate in @this.Candidates)
		{
			lastElement = candidate;
		}

		if (lastElement is { } r)
		{
			result = r;
			return true;
		}

	ReturnFalseAboutNoElement:
		result = default;
		return false;
	}

	/// <summary>
	/// Returns the minimum value in a candidate sequence according to a specified key selector function.
	/// </summary>
	/// <typeparam name="TComparableWithMaxValue">The type of key to compare candidates by.</typeparam>
	/// <param name="this">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A function to extract the key for each element.</param>
	/// <returns>The value with the minimum key in the sequence.</returns>
	public static TComparableWithMaxValue MinBy<TComparableWithMaxValue>(
		this in Grid @this,
		[NotNull, DisallowNull] delegate*<int, TComparableWithMaxValue> selector
	)
	where TComparableWithMaxValue : IComparable<TComparableWithMaxValue>, IMinMaxValue<TComparableWithMaxValue>
	{
		var result = TComparableWithMaxValue.MaxValue;
		foreach (int candidate in @this.Candidates)
		{
			var selected = selector(candidate);
			if (selected.CompareTo(result) <= 0)
			{
				result = selected;
			}
		}

		return result;
	}

	/// <summary>
	/// Returns the maximum value in a candidate sequence according to a specified key selector function.
	/// </summary>
	/// <typeparam name="TComparableWithMinValue">The type of key to compare candidates by.</typeparam>
	/// <param name="this">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A function to extract the key for each element.</param>
	/// <returns>The value with the minimum key in the sequence.</returns>
	public static TComparableWithMinValue MaxBy<TComparableWithMinValue>(
		this in Grid @this,
		[NotNull, DisallowNull] delegate*<int, TComparableWithMinValue> selector
	)
	where TComparableWithMinValue : IComparable<TComparableWithMinValue>, IMinMaxValue<TComparableWithMinValue>
	{
		var result = TComparableWithMinValue.MinValue;
		foreach (int candidate in @this.Candidates)
		{
			var selected = selector(candidate);
			if (selected.CompareTo(result) >= 0)
			{
				result = selected;
			}
		}

		return result;
	}
}
