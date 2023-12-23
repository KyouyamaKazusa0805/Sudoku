namespace SudokuStudio.ComponentModel;

/// <summary>
/// Represents a value provider that provides with data to be displayed.
/// </summary>
public interface IRunningStrategyItemsProvider
{
	/// <summary>
	/// Indicates the items to be displayed.
	/// </summary>
	public abstract IList<RunningStrategyItem> Items { get; }
}
