namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Blocks"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="ExtraCell">Indicates the extra cell used.</param>
/// <param name="DigitsMask"><inheritdoc/></param>
internal sealed partial record ChromaticPatternType1Step(
	ConclusionList Conclusions,
	ViewList Views,
	int[] Blocks,
	scoped in CellMap Pattern,
	int ExtraCell,
	short DigitsMask
) : ChromaticPatternStep(Conclusions, Views, Blocks, Pattern, DigitsMask)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.ChromaticPatternType1;
}
