namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents length trait.
/// </summary>
public interface ILength
{
	/// <summary>
	/// Indicates the length of the pattern.
	/// </summary>
	public abstract int Length { get; }
}
