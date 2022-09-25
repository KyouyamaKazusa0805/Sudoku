namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Deadly Pattern</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record DeadlyPatternStep(ConclusionList Conclusions, ViewList Views) :
	Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.DeadlyPattern;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.DeadlyPattern;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;
}
