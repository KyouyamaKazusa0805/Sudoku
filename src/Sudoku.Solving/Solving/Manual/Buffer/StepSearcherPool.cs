#nullable disable warnings

using Sudoku.Solving.Manual.Searchers;

namespace Sudoku.Solving.Manual.Buffer;

/// <summary>
/// Indicates a pool that stores the <see cref="IStepSearcher"/>s.
/// </summary>
/// <seealso cref="IStepSearcher"/>
public static class StepSearcherPool
{
	/// <summary>
	/// The step searchers to find steps to apply to a certain puzzle.
	/// </summary>
	internal static IStepSearcher[] Collection;


	/// <summary>
	/// Gets the <see cref="IStepSearcher"/>s.
	/// </summary>
	public static IStepSearcher[] Searchers => Collection;
}
