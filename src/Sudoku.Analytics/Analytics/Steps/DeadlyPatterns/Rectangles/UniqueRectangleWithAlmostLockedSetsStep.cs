namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Almost Locked Sets XZ Rule</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isIncomplete">Indicates whether the pattern is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="almostLockedSet">The extra ALS.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleWithAlmostLockedSetsXzStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	[PrimaryConstructorParameter] bool isIncomplete,
	bool isAvoidable,
	[PrimaryConstructorParameter] AlmostLockedSet almostLockedSet,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	isAvoidable ? Technique.AvoidableRectangleAlmostLockedSetsXz : Technique.UniqueRectangleAlmostLockedSetsXz,
	digit1,
	digit2,
	in cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 6;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, AlsStr]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, AlsStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [new RectangleIsAvoidableFactor(), new UniqueRectangleAlmostLockedSetsXzIsIncompleteFactor()];

	private string AlsStr => AlmostLockedSet.ToString(Options.Converter);
}
