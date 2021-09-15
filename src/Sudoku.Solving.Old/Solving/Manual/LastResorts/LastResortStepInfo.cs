namespace Sudoku.Solving.Manual.LastResorts;

/// <summary>
/// Provides a usage of <b>last resort</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
public abstract record class LastResortStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
	: StepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.LastResort;
}
