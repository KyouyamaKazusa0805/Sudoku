namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Deadly Pattern</b> technique.
/// </summary>
public abstract class DeadlyPatternStep(Conclusion[] conclusions, View[]? views) : Step(conclusions, views)
{
	/// <inheritdoc/>
	public override string Name => base.Name;

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.DeadlyPattern;
}
