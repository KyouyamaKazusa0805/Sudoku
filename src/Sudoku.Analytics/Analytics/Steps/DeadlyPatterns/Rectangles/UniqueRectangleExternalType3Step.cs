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
	ref readonly CellMap cells,
	[PrimaryConstructorParameter] ref readonly CellMap guardianCells,
	[PrimaryConstructorParameter] ref readonly CellMap subsetCells,
	[PrimaryConstructorParameter] Mask subsetDigitsMask,
	[PrimaryConstructorParameter] bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) :
	UniqueRectangleStep(
		conclusions,
		views,
		options,
		isAvoidable ? Technique.AvoidableRectangleExternalType3 : Technique.UniqueRectangleExternalType3,
		digit1,
		digit2,
		in cells,
		false,
		absoluteOffset
	),
	IIncompleteTrait,
	IPatternType3StepTrait<UniqueRectangleExternalType3Step>
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, SubsetCellsStr, SubsetDigitsStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, SubsetDigitsStr, SubsetCellsStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [
			new RectangleIsAvoidableFactor(),
			new UniqueRectangleExternalSubsetSizeFactor(),
			new UniqueRectangleExternalType3IsIncompleteFactor()
		];

	/// <inheritdoc/>
	bool IPatternType3StepTrait<UniqueRectangleExternalType3Step>.IsHidden => false;

	/// <inheritdoc/>
	int IPatternType3StepTrait<UniqueRectangleExternalType3Step>.SubsetSize => PopCount((uint)SubsetDigitsMask);

	private string SubsetDigitsStr => Options.Converter.DigitConverter(SubsetDigitsMask);

	private string SubsetCellsStr => Options.Converter.CellConverter(SubsetCells);
}
