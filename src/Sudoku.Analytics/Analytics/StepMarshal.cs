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
		=> accumulator.Sort(Comparer<TStep>.Create(static (l, r) => l.CompareTo(r)));

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
	public static ReadOnlySpan<(Grid CurrentGrid, Step CurrentStep)> Combine(ReadOnlySpan<Grid> interimGrids, ReadOnlySpan<Step> interimSteps)
	{
		if (interimGrids.Length != interimSteps.Length)
		{
			var message = string.Format(SR.ExceptionMessage("LengthMustBeSame"), [nameof(interimGrids), nameof(interimSteps)]);
			throw new InvalidOperationException(message);
		}

		var result = new List<(Grid, Step)>(interimGrids.Length);
		for (var i = 0; i < interimGrids.Length; i++)
		{
			result.AddRef((interimGrids[i], interimSteps[i]));
		}
		return result.AsReadOnlySpan();
	}
}
