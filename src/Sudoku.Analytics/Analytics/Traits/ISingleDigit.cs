namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents single-digit trait.
/// </summary>
public interface ISingleDigit
{
	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public abstract Digit Digit { get; }
}
