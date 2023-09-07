using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public abstract class FireworkStep(Conclusion[] conclusions, View[]? views) : IntersectionStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.9M;
}
