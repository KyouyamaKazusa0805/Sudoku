namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="guardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="subsetCells">The extra cells that forms the subset.</param>
/// <param name="subsetDigitsMask">Indicates the digits that the subset are used.</param>
/// <param name="isIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleExternalType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap cells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap guardianCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap subsetCells,
	[PrimaryConstructorParameter] Mask subsetDigitsMask,
	[PrimaryConstructorParameter] bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	isAvoidable ? Technique.AvoidableRectangleExternalType3 : Technique.UniqueRectangleExternalType3,
	digit1,
	digit2,
	in cells,
	false,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .1M;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [
			new(ExtraDifficultyFactorNames.Size, PopCount((uint)SubsetDigitsMask) * .1M),
			new(ExtraDifficultyFactorNames.Avoidable, IsAvoidable ? .1M : 0),
			new(ExtraDifficultyFactorNames.Incompleteness, IsIncomplete ? .1M : 0)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, SubsetCellsStr, DigitsStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, DigitsStr, SubsetCellsStr])
		];

	private string DigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string SubsetCellsStr => Options.Converter.CellConverter(SubsetCells);
}
