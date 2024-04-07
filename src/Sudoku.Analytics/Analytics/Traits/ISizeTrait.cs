namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents a size trait.
/// </summary>
public interface ISizeTrait : IStepTrait
{
	/// <summary>
	/// Indicates the size of the pattern.
	/// </summary>
	public abstract int Size { get; }
}
