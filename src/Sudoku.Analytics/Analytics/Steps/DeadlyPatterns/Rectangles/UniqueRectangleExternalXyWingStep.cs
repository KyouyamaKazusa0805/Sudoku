namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External XY-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="guardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="cellPair">Indicates the cell pair.</param>
/// <param name="isIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleExternalXyWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	[PrimaryConstructorParameter] ref readonly CellMap guardianCells,
	[PrimaryConstructorParameter] ref readonly CellMap cellPair,
	[PrimaryConstructorParameter] bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) :
	UniqueRectangleStep(
		conclusions,
		views,
		options,
		isAvoidable ? Technique.AvoidableRectangleExternalXyWing : Technique.UniqueRectangleExternalXyWing,
		digit1,
		digit2,
		in cells,
		isAvoidable,
		absoluteOffset
	),
	IGuardianTrait,
	IIncompleteTrait
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr]),
			new(SR.ChineseLanguage, [D1Str, D2Str, CellsStr, GuardianCellsStr, CellPairStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [
			new UniqueRectangleExternalXyWingGuardianFactor(),
			new RectangleIsAvoidableFactor(),
			new UniqueRectangleExternalXyWingIsCompleteFactor()
		];

	/// <inheritdoc/>
	int IGuardianTrait.GuardianCellsCount => GuardianCells.Count;

	private string GuardianCellsStr => Options.Converter.CellConverter(GuardianCells);

	private string CellPairStr => Options.Converter.CellConverter(CellPair);
}
