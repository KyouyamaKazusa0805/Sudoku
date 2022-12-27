namespace Sudoku.Buffers;

/// <summary>
/// Indicates a pool that stores a <see cref="StepSearcherCollection"/> instance.
/// </summary>
/// <seealso cref="StepSearcherCollection"/>
internal static class StepSearcherPool
{
	/// <summary>
	/// The step searchers to find steps to apply to a certain puzzle.
	/// </summary>
	public static StepSearcherCollection Collection = null!;
}
