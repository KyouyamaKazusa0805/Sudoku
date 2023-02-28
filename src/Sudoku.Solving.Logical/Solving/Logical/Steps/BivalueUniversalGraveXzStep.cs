namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave XZ</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="ExtraCell">Indicates the XZ cell.</param>
internal sealed record BivalueUniversalGraveXzStep(
	ConclusionList Conclusions,
	ViewList Views,
	short DigitsMask,
	scoped in CellMap Cells,
	int ExtraCell
) : BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveXzRule;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, .2M) };


	[ResourceTextFormatter]
	internal string DigitStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string CellsStr() => Cells.ToString();

	[ResourceTextFormatter]
	internal string ExtraCellStr() => RxCyNotation.ToCellString(ExtraCell);
}
