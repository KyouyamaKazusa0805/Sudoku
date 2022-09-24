namespace Sudoku.Buffers;

/// <summary>
/// Indicates a pool that stores the <see cref="IStepSearcher"/>s.
/// </summary>
/// <seealso cref="IStepSearcher"/>
internal static class StepSearcherPool
{
	/// <summary>
	/// The step searchers to find steps to apply to a certain puzzle.
	/// </summary>
	public static IStepSearcher[] Collection = null!;
}
