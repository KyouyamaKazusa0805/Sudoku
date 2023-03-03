namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Symmetrical</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record SymmetryStep(Conclusion[] Conclusions, View[]? Views) : Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Symmetry;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Symmetry;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;
}
