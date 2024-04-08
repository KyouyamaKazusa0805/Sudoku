namespace Sudoku.Traits;

/// <summary>
/// Represents a trait that can describe whether the pattern is incomplete or not.
/// </summary>
public interface IIncompleteTrait : IStepTrait
{
	/// <summary>
	/// Indicates whether the pattern is incomplete.
	/// </summary>
	public abstract bool IsIncomplete { get; }
}
