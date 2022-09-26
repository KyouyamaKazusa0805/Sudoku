namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Represents for a step searcher that searches for irregular wing steps.
/// </summary>
/// <remarks>
/// In fact, <c>Hybrid-Wing</c>s, <c>Local-Wing</c>s, <c>Split-Wing</c>s and <c>M-Wing</c>s can
/// be found in another searcher. In addition, these wings are not elementary and necessary techniques
/// so we doesn't need to list them.
/// </remarks>
public interface IIrregularWingStepSearcher : IWingStepSearcher
{
}
