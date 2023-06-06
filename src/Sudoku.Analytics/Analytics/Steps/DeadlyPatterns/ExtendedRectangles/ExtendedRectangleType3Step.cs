namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="subsetCells">Indicates the extra cells used that can form the subset.</param>
/// <param name="subsetDigitsMask">Indicates the subset digits used.</param>
/// <param name="house">Indicates the house that subset formed.</param>
public sealed partial class ExtendedRectangleType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] scoped in CellMap subsetCells,
	[PrimaryConstructorParameter] Mask subsetDigitsMask,
	[PrimaryConstructorParameter] House house
) : ExtendedRectangleStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { base.ExtraDifficultyCases[0], (ExtraDifficultyCaseNames.ExtraDigit, PopCount((uint)SubsetDigitsMask) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ EnglishLanguage, new[] { DigitsStr, CellsStr, ExtraDigitsStr, ExtraCellsStr, HouseStr } },
			{ ChineseLanguage, new[] { DigitsStr, CellsStr, HouseStr, ExtraCellsStr, ExtraDigitsStr } }
		};

	private string ExtraDigitsStr => DigitMaskFormatter.Format(SubsetDigitsMask, FormattingMode.Normal);

	private string ExtraCellsStr => SubsetCells.ToString();

	private string HouseStr => HouseFormatter.Format(1 << House);
}
