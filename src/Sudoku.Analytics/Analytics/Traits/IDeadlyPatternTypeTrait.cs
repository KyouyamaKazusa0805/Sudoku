namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents a trait that describes the subtype used in a deadly pattern.
/// </summary>
public interface IDeadlyPatternTypeTrait : ITrait
{
	/// <summary>
	/// Indicates the subtype used in the pattern.
	/// </summary>
	public abstract int Type { get; }
}
