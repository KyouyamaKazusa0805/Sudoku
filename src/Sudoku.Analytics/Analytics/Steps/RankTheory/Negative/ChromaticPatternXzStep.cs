namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern XZ</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="blocks"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="cells">Indicates the cells that contains extra digit.</param>
/// <param name="extraCell">Indicates the extra cell used.</param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="extraDigitsMask">Indicates the mask of extra digits.</param>
public sealed partial class ChromaticPatternXzStep(
	Conclusion[] conclusions,
	View[]? views,
	House[] blocks,
	scoped in CellMap pattern,
	[PrimaryConstructorParameter] scoped in CellMap cells,
	[PrimaryConstructorParameter] Cell extraCell,
	Mask digitsMask,
	[PrimaryConstructorParameter] Mask extraDigitsMask
) : ChromaticPatternStep(conclusions, views, blocks, pattern, digitsMask)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.ChromaticPatternXzRule;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.ExtraDigit, .2M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [DigitsStr, CellsStr, BlocksStr, ExtraCellStr]),
			new(ChineseLanguage, [BlocksStr, CellsStr, DigitsStr, ExtraCellStr])
		];

	private string ExtraCellStr => CellConceptNotation.ToString(ExtraCell);
}
