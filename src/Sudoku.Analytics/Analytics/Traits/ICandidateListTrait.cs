namespace Sudoku.Analytics.Traits;

/// <summary>
/// Represents a trait that describes the candidate list.
/// </summary>
public interface ICandidateListTrait : ITrait
{
	/// <summary>
	/// Indicates the number of candidates.
	/// </summary>
	public abstract int CandidateSize { get; }
}
