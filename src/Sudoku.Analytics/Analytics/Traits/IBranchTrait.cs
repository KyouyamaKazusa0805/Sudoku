namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents a trait that describes the branches.
/// </summary>
public interface IBranchTrait
{
	/// <summary>
	/// Indicates the number of branches.
	/// </summary>
	public abstract int BranchesCount { get; }
}
