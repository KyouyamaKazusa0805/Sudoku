namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Locked Candidates</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Pointing</item>
/// <item>Claiming</item>
/// </list>
/// </summary>
public interface ILockedCandidatesStepSearcher : IIntersectionStepSearcher
{
}
