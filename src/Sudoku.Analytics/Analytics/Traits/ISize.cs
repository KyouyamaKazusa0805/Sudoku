namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents the size trait.
/// </summary>
public interface ISize
{
	/// <summary>
	/// Indicates the size of the pattern.
	/// </summary>
	public abstract int Size { get; }
}
