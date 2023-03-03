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
	Conclusion[] Conclusions,
	View[]? Views,
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
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, .2M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitStr, CellsStr, ExtraCellStr } },
			{ "zh", new[] { DigitStr, CellsStr, ExtraCellStr } }
		};

	private string DigitStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string CellsStr => Cells.ToString();

	private string ExtraCellStr => RxCyNotation.ToCellString(ExtraCell);
}
