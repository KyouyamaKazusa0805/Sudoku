namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for almost locked candidates steps.
/// </summary>
public interface IAlmostLockedCandidatesStepSearcher : IIntersectionStepSearcher
{
	/// <summary>
	/// Indicates whether the user checks the almost locked quadruple.
	/// </summary>
	bool CheckAlmostLockedQuadruple { get; set; }

	/// <summary>
	/// Indicates whether the searcher checks for values (givens and modifiables)
	/// to form an almost locked candidates. If the value is <see langword="true"/>,
	/// some possible Sue de Coqs steps will be replaced with Almost Locked Candidates ones.
	/// </summary>
	bool CheckForValues { get; set; }
}