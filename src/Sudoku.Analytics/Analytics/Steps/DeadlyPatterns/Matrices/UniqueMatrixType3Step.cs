namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="subsetDigitsMask">Indicates the mask that describes the extra digits used in the subset.</param>
/// <param name="subsetCells">Indicates the cells that the subset used.</param>
public sealed partial class UniqueMatrixType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] scoped in CellMap subsetCells,
	[PrimaryConstructorParameter] Mask subsetDigitsMask
) : UniqueMatrixStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [new(ExtraDifficultyCaseNames.ExtraDigit, PopCount((uint)SubsetDigitsMask) * .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitStr, ExtraCellsStr, SubsetName]),
			new(ChineseLanguage, [ExtraDigitStr, ExtraCellsStr, SubsetName, DigitsStr, CellsStr])
		];

	private string ExtraCellsStr => SubsetCells.ToString();

	private string ExtraDigitStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string SubsetName => TechniqueFact.GetSubsetName(SubsetCells.Count);
}
