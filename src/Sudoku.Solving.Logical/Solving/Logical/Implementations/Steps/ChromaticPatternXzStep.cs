namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern XZ</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Blocks"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="Cells">The cells that contains extra digit in the pattern.</param>
/// <param name="ExtraCell">Indicates the extra cell used.</param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraDigitsMask">The extra digits mask.</param>
[StepDisplayingFeature(StepDisplayingFeature.VeryRare)]
internal sealed record ChromaticPatternXzStep(
	ConclusionList Conclusions,
	ViewList Views,
	int[] Blocks,
	scoped in CellMap Pattern,
	scoped in CellMap Cells,
	int ExtraCell,
	short DigitsMask,
	short ExtraDigitsMask
) : ChromaticPatternStep(Conclusions, Views, Blocks, Pattern, DigitsMask), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.ExtraDigit, .2M) };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.ChromaticPatternXzRule;


	[ResourceTextFormatter]
	internal string ExtraCellStr() => RxCyNotation.ToCellString(ExtraCell);
}
