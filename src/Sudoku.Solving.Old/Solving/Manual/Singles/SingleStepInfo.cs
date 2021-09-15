namespace Sudoku.Solving.Manual.Singles;

/// <summary>
/// Provides a usage of <b>single</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Cell">The cell.</param>
/// <param name="Digit">The digit.</param>
public abstract record class SingleStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit
) : StepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Easy;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Singles;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Single;
}
