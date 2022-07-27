namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Blocks"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="ExtraCell">Indicates the extra cell used.</param>
/// <param name="DigitsMask"><inheritdoc/></param>
public sealed record ChromaticPatternType1Step(
	ConclusionList Conclusions,
	ViewList Views,
	int[] Blocks,
	scoped in Cells Cells,
	int ExtraCell,
	short DigitsMask
) : ChromaticPatternStep(Conclusions, Views, Blocks, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.ChromaticPatternType1;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;
}
