namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Wing</b> technique.
/// </summary>
public abstract class WingStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	[AllowNull]
	[MaybeNull]
	public sealed override string Format => base.Format;

	/// <inheritdoc/>
	public sealed override TechniqueGroup Group => TechniqueGroup.Wing;
}
