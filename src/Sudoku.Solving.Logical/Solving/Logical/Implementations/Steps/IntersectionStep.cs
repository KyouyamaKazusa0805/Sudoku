namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is an <b>Intersection</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record IntersectionStep(ConclusionList Conclusions, ViewList Views) : Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.Intersections;
}
