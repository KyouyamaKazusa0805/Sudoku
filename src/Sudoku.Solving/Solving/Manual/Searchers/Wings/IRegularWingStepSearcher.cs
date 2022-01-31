namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for regular wing steps.
/// </summary>
public interface IRegularWingStepSearcher : IWingStepSearcher
{
	/// <summary>
	/// Indicates the maximum size the searcher will search for. The maximum possible value is 9.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Throws when <c>value</c> is greater than 9.</exception>
	int MaxSize { get; set; }
}
