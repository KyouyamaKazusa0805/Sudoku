namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents lasting trait.
/// </summary>
public interface ILastingTrait : IDirectTrait<int>
{
	/// <summary>
	/// Indicates the lasting of the single. The value describes the number of empty cells appeared in a house
	/// containing the target conclusion from the single step.
	/// </summary>
	public abstract int Lasting { get; }
}
