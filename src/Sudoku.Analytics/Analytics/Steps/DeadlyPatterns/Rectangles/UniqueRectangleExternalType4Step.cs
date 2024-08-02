namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="guardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
/// <param name="isIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleExternalType4Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	[PrimaryConstructorParameter] ref readonly CellMap guardianCells,
	[PrimaryConstructorParameter] Conjugate conjugatePair,
	[PrimaryConstructorParameter] bool isIncomplete,
	bool isAvoidable,
	int absoluteOffset
) :
	UniqueRectangleStep(
		conclusions,
		views,
		options,
		isAvoidable ? Technique.AvoidableRectangleExternalType4 : Technique.UniqueRectangleExternalType4,
		digit1,
		digit2,
		in cells,
		isAvoidable,
		absoluteOffset
	),
	IIncompleteTrait
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <inheritdoc/>
	public override Interpolation[] Interpolations
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, ConjugatePairStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, ConjugatePairStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [new RectangleIsAvoidableFactor(), new UniqueRectangleExternalType4IsIncompleteFactor()];

	private string ConjugatePairStr => Options.Converter.ConjugateConverter([ConjugatePair]);
}
