namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave XZ</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="xzCell">Indicates the extra cell used. This cell is a bivalue cell that only contains digit X and Z.</param>
public sealed partial class BivalueUniversalGraveXzStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] Mask digitsMask,
	[PrimaryConstructorParameter] scoped in CellMap cells,
	[PrimaryConstructorParameter] int xzCell
) : BivalueUniversalGraveStep(conclusions, views)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveXzRule;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new[] { (ExtraDifficultyCaseNames.ExtraDigit, .2M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitStr, CellsStr, ExtraCellStr } },
			{ "zh", new[] { DigitStr, CellsStr, ExtraCellStr } }
		};

	private string DigitStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string CellsStr => Cells.ToString();

	private string ExtraCellStr => RxCyNotation.ToCellString(XzCell);
}
