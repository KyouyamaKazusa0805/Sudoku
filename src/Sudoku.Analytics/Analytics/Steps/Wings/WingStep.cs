namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
public abstract class WingStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;
}
