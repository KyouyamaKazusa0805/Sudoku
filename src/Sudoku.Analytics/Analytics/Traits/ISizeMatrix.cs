namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents size trait, describing a pattern contains "size" concept in multiple dimensions.
/// </summary>
public interface ISizeMatrix
{
	/// <summary>
	/// Indicates the size values from separate dimensions.
	/// </summary>
	public abstract int[] SizeMatrix { get; }
}
