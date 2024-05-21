namespace Sudoku.Linq;

/// <summary>
/// Provides with a list of LINQ methods operating with <see cref="Grid"/> instances.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridEnumerable
{
	/// <summary>
	/// Filters the candidates that satisfies the specified condition.
	/// </summary>
	/// <param name="this">The instance to be iterated.</param>
	/// <param name="predicate">The condition to filter candidates.</param>
	/// <returns>All candidates satisfied the specified condition.</returns>
	public static ReadOnlySpan<Candidate> Where(this scoped in Grid @this, Func<Candidate, bool> predicate)
	{
		var (result, i) = (new Candidate[@this.CandidatesCount], 0);
		foreach (var candidate in @this.Candidates)
		{
			if (predicate(candidate))
			{
				result[i++] = candidate;
			}
		}
		return result.AsReadOnlySpan()[..i];
	}

	/// <summary>
	/// Projects each element of a sequence into a new form.
	/// </summary>
	/// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
	/// <param name="this">The instance to be iterated.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <returns>
	/// An array of <typeparamref name="TResult"/> elements converted.
	/// </returns>
	public static ReadOnlySpan<TResult> Select<TResult>(this scoped in Grid @this, Func<Candidate, TResult> selector)
	{
		var (result, i) = (new TResult[@this.CandidatesCount], 0);
		foreach (var candidate in @this.Candidates)
		{
			result[i++] = selector(candidate);
		}
		return result.AsReadOnlySpan()[..i];
	}
}
