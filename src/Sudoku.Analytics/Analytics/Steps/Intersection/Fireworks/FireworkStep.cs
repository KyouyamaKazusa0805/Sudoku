namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class FireworkStep(Conclusion[] conclusions, View[]? views, StepSearcherOptions options) :
	IntersectionStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.9M;

	/// <inheritdoc/>
	public abstract int Size { get; }
}
