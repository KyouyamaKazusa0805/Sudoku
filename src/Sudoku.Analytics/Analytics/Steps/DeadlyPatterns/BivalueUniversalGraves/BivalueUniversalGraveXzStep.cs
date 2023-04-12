namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave XZ</b> technique.
/// </summary>
public sealed class BivalueUniversalGraveXzStep(
	Conclusion[] conclusions,
	View[]? views,
	Mask digitsMask,
	scoped in CellMap cells,
	int xzCell
) : BivalueUniversalGraveStep(conclusions, views)
{
	/// <summary>
	/// Indicates the extra cell used. This cell is a bivalue cell that only contains digit X and Z.
	/// </summary>
	public int XzCell { get; } = xzCell;

	/// <summary>
	/// Indicates the mask of digits used.
	/// </summary>
	public Mask DigitsMask { get; } = digitsMask;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveXzRule;

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; } = cells;

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
