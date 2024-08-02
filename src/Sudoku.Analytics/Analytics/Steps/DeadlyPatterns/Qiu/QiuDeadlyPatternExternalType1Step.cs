namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern External Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="is2LinesWith2Cells"><inheritdoc/></param>
/// <param name="houses"><inheritdoc/></param>
/// <param name="corner1"><inheritdoc/></param>
/// <param name="corner2"><inheritdoc/></param>
/// <param name="targetCell">Indicates the target cell.</param>
/// <param name="targetDigits">Indicates the target digits.</param>
public sealed partial class QiuDeadlyPatternExternalType1Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	bool is2LinesWith2Cells,
	HouseMask houses,
	Cell? corner1,
	Cell? corner2,
	[PrimaryConstructorParameter] Cell targetCell,
	[PrimaryConstructorParameter] Mask targetDigits
) : QiuDeadlyPatternExternalTypeStep(conclusions, views, options, is2LinesWith2Cells, houses, corner1, corner2)
{
	/// <inheritdoc/>
	public override int Type => 6;

	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [new(EnglishLanguage, [PatternStr, DigitsStr, CellStr]), new(ChineseLanguage, [PatternStr, CellStr, DigitsStr])];

	private string CellStr => Options.Converter.CellConverter(in TargetCell.AsCellMap());

	private string DigitsStr => Options.Converter.DigitConverter(TargetDigits);
}
