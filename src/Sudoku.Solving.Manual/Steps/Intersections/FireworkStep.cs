namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public abstract record FireworkStep(ConclusionList Conclusions, ViewList Views) :
	IntersectionStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.9M;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => base.TechniqueTags;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Firework;
}
