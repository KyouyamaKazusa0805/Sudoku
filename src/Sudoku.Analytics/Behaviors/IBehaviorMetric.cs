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
	/// Try to return a list of integers representing the logical filling distance between two adjacent steps,
	/// by using the current filling rule.
	/// </summary>
	/// <param name="collector">The collector instance.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="steps">The steps and corresponding grid states.</param>
	/// <returns>A list of distance values.</returns>
	public static abstract ReadOnlySpan<int> GetDistanceArray(
		Collector collector,
		ref readonly Grid grid,
		out ReadOnlySpan<KeyValuePair<SingleStep, Grid>> steps
	);
}
