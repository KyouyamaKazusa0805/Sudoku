namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Symmetrical</b> technique.
/// </summary>
public abstract class SymmetryStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;
}
