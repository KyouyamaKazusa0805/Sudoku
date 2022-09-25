namespace Sudoku.Solving.Logical.Prototypes;

/// <summary>
/// Defines the step whose technique used is the loop.
/// </summary>
public interface ILoopStep : IChainStep, ILoopLikeStep
{
	/// <inheritdoc/>
	bool? ILoopLikeStep.IsNice => true;
}
