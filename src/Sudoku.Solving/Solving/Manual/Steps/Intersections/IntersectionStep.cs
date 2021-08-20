namespace Sudoku.Solving.Manual.Steps.Intersections;

/// <summary>
/// Provides with a step that is an <b>Intersection</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public abstract record IntersectionStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views
) : Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;
}
