namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets</b> technique.
/// </summary>
public abstract class AlmostLockedSetsStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override string Name => base.Name;
}
