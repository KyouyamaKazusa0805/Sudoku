namespace Sudoku.Traits;

/// <summary>
/// Represents a size trait.
/// </summary>
public interface ISizeTrait : ITrait
{
	/// <summary>
	/// Indicates the size of the pattern.
	/// </summary>
	public abstract int Size { get; }
}
