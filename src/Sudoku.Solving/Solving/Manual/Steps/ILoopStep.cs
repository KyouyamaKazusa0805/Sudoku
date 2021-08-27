namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Defines a loop step.
/// </summary>
public interface ILoopStep : IChainStep
{
	/// <summary>
	/// Indicates whether the loop is a nice loop.
	/// </summary>
	/// <remarks>
	/// A <b>Nice</b> loop is a loop that all weak links can be gathered to remove candidates
	/// (if possible removable candidates exist).
	/// </remarks>
	bool IsNice { get; }
}