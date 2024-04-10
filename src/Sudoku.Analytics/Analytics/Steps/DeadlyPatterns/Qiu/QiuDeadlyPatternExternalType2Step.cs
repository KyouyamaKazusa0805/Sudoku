namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern External Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="is2LinesWith2Cells"><inheritdoc/></param>
/// <param name="houses"><inheritdoc/></param>
/// <param name="corner1"><inheritdoc/></param>
/// <param name="corner2"><inheritdoc/></param>
/// <param name="mirrorCells">Indicates the mirror cells.</param>
/// <param name="targetDigit">Indicates the target digit.</param>
public sealed partial class QiuDeadlyPatternExternalType2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	bool is2LinesWith2Cells,
	HouseMask houses,
	Cell? corner1,
	Cell? corner2,
	[PrimaryConstructorParameter] scoped ref readonly CellMap mirrorCells,
	[PrimaryConstructorParameter] Digit targetDigit
) : QiuDeadlyPatternExternalTypeStep(conclusions, views, options, is2LinesWith2Cells, houses, corner1, corner2)
{
	/// <inheritdoc/>
	public override int Type => 5;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [PatternStr, DigitStr, CellsStr]), new(ChineseLanguage, [PatternStr, CellsStr, DigitStr])];

	private string CellsStr => Options.Converter.CellConverter(MirrorCells);

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << TargetDigit));
}
