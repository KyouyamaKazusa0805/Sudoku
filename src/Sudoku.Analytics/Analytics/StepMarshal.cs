namespace Sudoku.Analytics;

/// <summary>
/// Represents a list of methods operating with <see cref="Step"/> instances.
/// </summary>
/// <seealso cref="Step"/>
public static class StepMarshal
{
	/// <summary>
	/// Sorts <typeparamref name="TStep"/> instances from the list collection.
	/// </summary>
	/// <typeparam name="TStep">The type of each step.</typeparam>
	/// <param name="accumulator">The accumulator instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SortItems<TStep>(List<TStep> accumulator) where TStep : Step
		=> accumulator.Sort(ValueComparison.Create<TStep>(static (l, r) => l.CompareTo(r)));

#pragma warning disable format
	/// <summary>
	/// Compares <typeparamref name="TStep"/> instances from the list collection,
	/// removing duplicate items by using <see cref="Step.Equals(Step?)"/> to as equality comparison rules.
	/// </summary>
	/// <typeparam name="TStep">The type of each step.</typeparam>
	/// <param name="accumulator">The accumulator instance.</param>
	/// <returns>The final collection of <typeparamref name="TStep"/> instances.</returns>
	/// <seealso cref="Step.Equals(Step?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<TStep> RemoveDuplicateItems<TStep>(List<TStep> accumulator) where TStep : Step
		=> accumulator switch
		{
			[] => [],
			[var firstElement] => [firstElement],
			[var a, var b] => a == b ? [a] : [a, b],
			_ => new HashSet<TStep>(accumulator, ValueComparison.CreateByEqualityOperator<Step>())
		};
#pragma warning restore format

	/// <summary>
	/// Zips the collection, pairing each step and corresponding grid into a <see cref="ValueTuple{T1, T2}"/>,
	/// and return the collection of pairs.
	/// </summary>
	/// <param name="interimGrids">The grids corresponded.</param>
	/// <param name="interimSteps">The steps.</param>
	/// <returns>The zipped collection.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the length of arguments <paramref name="interimGrids"/> and <paramref name="interimSteps"/> aren't same.
	/// </exception>
	public static ReadOnlySpan<(Grid CurrentGrid, Step CurrentStep)> Combine(scoped ReadOnlySpan<Grid> interimGrids, scoped ReadOnlySpan<Step> interimSteps)
	{
		if (interimGrids.Length != interimSteps.Length)
		{
			throw new InvalidOperationException(
				string.Format(
					ResourceDictionary.ExceptionMessage("LengthMustBeSame"),
					nameof(interimGrids),
					nameof(interimSteps)
				)
			);
		}

		var result = new List<(Grid, Step)>(interimGrids.Length);
		for (var i = 0; i < interimGrids.Length; i++)
		{
			result.Add((interimGrids[i], interimSteps[i]));
		}
		return result.AsReadOnlySpan();
	}
}
