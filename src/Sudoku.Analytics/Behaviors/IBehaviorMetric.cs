namespace Sudoku.Behaviors;

/// <summary>
/// Represents a type that can measure a kind of user behavior.
/// </summary>
public interface IBehaviorMetric
{
	/// <summary>
	/// Indicates a behavior to be measured.
	/// </summary>
	public static abstract UserBehavior MeasurableBehavior { get; }


	/// <summary>
	/// Returns a list of integers representing the logical filling distance between two adjacent steps,
	/// by using the current filling rule.
	/// </summary>
	/// <param name="collector">The collector instance.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="steps">The steps and corresponding grid states.</param>
	/// <param name="stepsAll">
	/// Indicates all possible steps applied. If the puzzle cannot be solved with direct techniques,
	/// this method will find indirect techniques again and again, until at least one single step can be found here.
	/// </param>
	/// <returns>A list of distance values.</returns>
	/// <remarks>
	/// This method allows solving puzzles with indirect techniques. If the puzzle cannot be solved with direct-only techniques,
	/// argument <paramref name="stepsAll"/> will record the usages of all techniques (including direct and indirect techniques);
	/// and argument <paramref name="steps"/> only records for direct technique usages.
	/// </remarks>
	public static abstract ReadOnlySpan<int> GetDistanceArray(
		Collector collector,
		in Grid grid,
		out ReadOnlySpan<KeyValuePair<SingleStep, Grid>> steps,
		out ReadOnlySpan<KeyValuePair<Step, Grid>> stepsAll
	);
}
