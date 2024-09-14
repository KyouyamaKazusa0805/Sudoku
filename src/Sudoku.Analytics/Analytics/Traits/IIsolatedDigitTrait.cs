namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents a trait that describes the isolated digits appeared in the pattern.
/// </summary>
public interface IIsolatedDigitTrait : ITrait
{
	/// <summary>
	/// Indicates whether the pattern contains at least one isolated digit.
	/// </summary>
	public abstract bool ContainsIsolatedDigits { get; }

	/// <summary>
	/// Indicates the number of isolated digits.
	/// </summary>
	public abstract int IsolatedDigitsCount { get; }
}
