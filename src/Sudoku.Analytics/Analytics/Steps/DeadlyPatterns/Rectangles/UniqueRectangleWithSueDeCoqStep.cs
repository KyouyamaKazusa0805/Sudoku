namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Sue de Coq</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="block">Indicates the block index that the Sue de Coq pattern used.</param>
/// <param name="line">Indicates the line (row or column) index that the Sue de Coq pattern used.</param>
/// <param name="blockMask">Indicates the mask that contains all digits from the block of the Sue de Coq pattern.</param>
/// <param name="lineMask">Indicates the cells in the line of the Sue de Coq pattern.</param>
/// <param name="intersectionMask">
/// Indicates the mask that contains all digits from the intersection of houses <see cref="Block"/> and <see cref="Line"/>.
/// </param>
/// <param name="isCannibalistic">Indicates whether the Sue de Coq pattern is a cannibalism.</param>
/// <param name="isolatedDigitsMask">Indicates the mask that contains all isolated digits.</param>
/// <param name="blockCells">Indicates the cells in the block of the Sue de Coq pattern.</param>
/// <param name="lineCells">Indicates the cells in the line (row or column) of the Sue de Coq pattern.</param>
/// <param name="intersectionCells">Indicates the cells in the intersection from houses <see cref="Block"/> and <see cref="Line"/>.</param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed partial class UniqueRectangleWithSueDeCoqStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap cells,
	bool isAvoidable,
	[PrimaryConstructorParameter] House block,
	[PrimaryConstructorParameter] House line,
	[PrimaryConstructorParameter] Mask blockMask,
	[PrimaryConstructorParameter] Mask lineMask,
	[PrimaryConstructorParameter] Mask intersectionMask,
	[PrimaryConstructorParameter] bool isCannibalistic,
	[PrimaryConstructorParameter] Mask isolatedDigitsMask,
	[PrimaryConstructorParameter] scoped ref readonly CellMap blockCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap lineCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap intersectionCells,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	options,
	isAvoidable ? Technique.AvoidableRectangleSueDeCoq : Technique.UniqueRectangleSueDeCoq,
	digit1,
	digit2,
	in cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 5;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [D1Str, D2Str, CellsStr, MergedCellsStr, SueDeCoqDigitsMask]),
			new(ChineseLanguage, [D1Str, D2Str, CellsStr, MergedCellsStr, SueDeCoqDigitsMask])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [
			new RectangleSueDeCoqIsolatedFactor(),
			new RectangleSueDeCoqCannibalismFactor(),
			new RectangleIsAvoidableFactor()
		];

	private string MergedCellsStr => Options.Converter.CellConverter(LineCells | BlockCells);

	private string SueDeCoqDigitsMask => Options.Converter.DigitConverter((Mask)(LineMask | BlockMask));
}
