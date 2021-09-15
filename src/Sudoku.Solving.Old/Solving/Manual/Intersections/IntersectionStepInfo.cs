namespace Sudoku.Solving.Manual.Intersections;

/// <summary>
/// Provides a usage of <b>intersection</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
public abstract record class IntersectionStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
	: StepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Intersections;
}
