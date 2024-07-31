namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle Almost Locked Sets XZ Rule</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isIncomplete">Indicates whether the pattern is incomplete.</param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="isDoublyLinked">Indicates whether the ALS-XZ pattern is doubly-linked.</param>
/// <param name="almostLockedSet">The extra ALS.</param>
/// <param name="multivalueCellsCount">Indicates the number of multi-value cells.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleAlmostLockedSetsXzStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap cells,
	[PrimaryConstructorParameter] bool isIncomplete,
	bool isAvoidable,
	[PrimaryConstructorParameter] bool isDoublyLinked,
	[PrimaryConstructorParameter] AlmostLockedSet almostLockedSet,
	int multivalueCellsCount,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	(almostLockedSet.IsBivalueCell, multivalueCellsCount, isAvoidable, isDoublyLinked) switch
	{
		(true, 2, true, _) => Technique.AvoidableRectangle2D,
		(true, 3, true, _) => Technique.AvoidableRectangle3X,
		(true, 2, _, _) => Technique.UniqueRectangle2D,
		(true, 3, _, _) => Technique.UniqueRectangle3X,
		(_, _, true, true) => Technique.AvoidableRectangleDoublyLinkedAlmostLockedSetsXz,
		(_, _, true, _) => Technique.AvoidableRectangleSinglyLinkedAlmostLockedSetsXz,
		(_, _, _, true) => Technique.UniqueRectangleDoublyLinkedAlmostLockedSetsXz,
		_ => Technique.UniqueRectangleSinglyLinkedAlmostLockedSetsXz
	},
	digit1,
	digit2,
	in cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + (IsDoublyLinked ? 7 : 6);

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [D1Str, D2Str, CellsStr, AlsStr]), new(ChineseLanguage, [D1Str, D2Str, CellsStr, AlsStr])];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [new RectangleIsAvoidableFactor(), new UniqueRectangleAlmostLockedSetsXzIsIncompleteFactor()];

	private string AlsStr => AlmostLockedSet.ToString(Options.Converter);
}
