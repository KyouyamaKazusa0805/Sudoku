namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Intersection</b> technique.
/// </summary>
public abstract class IntersectionStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;
}
