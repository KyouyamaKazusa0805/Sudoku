namespace Sudoku.Solving.Manual.Buffers;

/// <summary>
/// Indicates a pool that stores the <see cref="StepSearcher"/>s.
/// </summary>
/// <seealso cref="StepSearcher"/>
public static class StepSearcherPool
{
#nullable disable warnings
	/// <summary>
	/// The inner value.
	/// </summary>
	internal static StepSearcher[] InnerCollection;
#nullable restore warnings


	/// <summary>
	/// Gets the <see cref="StepSearcher"/>s.
	/// </summary>
	public static StepSearcher[] Searchers => InnerCollection;
}
