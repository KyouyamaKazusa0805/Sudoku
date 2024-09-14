namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents a trait that describes the true candidates.
/// </summary>
public interface ITrueCandidatesTrait
{
	/// <summary>
	/// Indicates the true candidates used.
	/// </summary>
	public abstract CandidateMap TrueCandidates { get; }
}
