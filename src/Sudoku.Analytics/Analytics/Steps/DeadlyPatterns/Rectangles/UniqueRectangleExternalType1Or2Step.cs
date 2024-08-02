namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Type 1/2</b> or <b>Avoidable Rectangle External Type 1/2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="guardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="guardianDigit">Indicates the digit that the guardians are used.</param>
/// <param name="isIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleExternalType1Or2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	[PrimaryConstructorParameter] ref readonly CellMap guardianCells,
	[PrimaryConstructorParameter] Digit guardianDigit,
	[PrimaryConstructorParameter] bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) :
	UniqueRectangleStep(
		conclusions,
		views,
		options,
		(isAvoidable, guardianCells.Count == 1) switch
		{
			(true, true) => Technique.AvoidableRectangleExternalType1,
			(true, false) => Technique.AvoidableRectangleExternalType2,
			(false, true) => Technique.UniqueRectangleExternalType1,
			_ => Technique.UniqueRectangleExternalType2
		},
		digit1,
		digit2,
		in cells,
		false,
		absoluteOffset
	),
	IIncompleteTrait,
	IGuardianTrait
{
	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, GuardianDigitStr, GuardianCellsStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, GuardianDigitStr, GuardianCellsStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [
			new UniqueRectangleExternalType1Or2GuardianFactor(),
			new RectangleIsAvoidableFactor(),
			new UniqueRectangleExternalType1Or2IsIncompleteFactor()
		];

	/// <inheritdoc/>
	int IGuardianTrait.GuardianCellsCount => GuardianCells.Count;

	private string GuardianDigitStr => Options.Converter.DigitConverter((Mask)(1 << GuardianDigit));

	private string GuardianCellsStr => Options.Converter.CellConverter(GuardianCells);
}
