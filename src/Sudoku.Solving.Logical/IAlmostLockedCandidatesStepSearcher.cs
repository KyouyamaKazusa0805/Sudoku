namespace Sudoku.Solving.Logics.Prototypes;

/// <summary>
/// Provides with an <b>Almost Locked Candidates</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Almost Locked Pair</item>
/// <item>Almost Locked Triple</item>
/// <item>Almost Locked Quadruple (Maybe unnecessary)</item>
/// </list>
/// </summary>
public interface IAlmostLockedCandidatesStepSearcher : IIntersectionStepSearcher
{
	/// <summary>
	/// Indicates whether the user checks the almost locked quadruple.
	/// </summary>
	public abstract bool CheckAlmostLockedQuadruple { get; set; }

	/// <summary>
	/// Indicates whether the searcher checks for values (givens and modifiables)
	/// to form an almost locked candidates. If the value is <see langword="true"/>,
	/// some possible Sue de Coqs steps will be replaced with Almost Locked Candidates ones.
	/// </summary>
	public abstract bool CheckForValues { get; set; }
}
