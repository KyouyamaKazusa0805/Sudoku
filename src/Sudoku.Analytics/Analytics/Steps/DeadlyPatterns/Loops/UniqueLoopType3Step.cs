namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
/// <param name="subsetCells">Indicates the cells that are subset cells.</param>
/// <param name="subsetDigitsMask">Indicates the mask that contains the subset digits used in this instance.</param>
/// <param name="loopPath"><inheritdoc/></param>
public sealed partial class UniqueLoopType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap loop,
	[PrimaryConstructorParameter] scoped ref readonly CellMap subsetCells,
	[PrimaryConstructorParameter] Mask subsetDigitsMask,
	Cell[] loopPath
) : UniqueLoopStep(conclusions, views, options, digit1, digit2, in loop, loopPath), IPatternType3StepTrait<UniqueLoopType3Step>
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Digit1Str, Digit2Str, LoopStr, SubsetName, DigitsStr, SubsetCellsStr]),
			new(ChineseLanguage, [Digit1Str, Digit2Str, LoopStr, SubsetName, DigitsStr, SubsetCellsStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors => [.. base.Factors, new UniqueLoopSubsetSizeFactor()];

	/// <inheritdoc/>
	bool IPatternType3StepTrait<UniqueLoopType3Step>.IsHidden => false;

	/// <inheritdoc/>
	int IPatternType3StepTrait<UniqueLoopType3Step>.SubsetSize => PopCount((uint)SubsetDigitsMask);

	private string SubsetCellsStr => Options.Converter.CellConverter(SubsetCells);

	private string DigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string SubsetName => TechniqueMarshal.GetSubsetName(SubsetCells.Count);
}
