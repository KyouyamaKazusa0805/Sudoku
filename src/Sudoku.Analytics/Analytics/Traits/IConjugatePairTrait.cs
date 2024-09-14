namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents a trait that describes the conjugate pairs.
/// </summary>
public interface IConjugatePairTrait : ITrait
{
	/// <summary>
	/// Indicates the number of conjugate pairs.
	/// </summary>
	public abstract int ConjugatePairsCount { get; }
}
