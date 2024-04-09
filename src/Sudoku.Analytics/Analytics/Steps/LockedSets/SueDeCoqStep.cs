namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Sue de Coq</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="block">Indicates the block index that the current pattern used.</param>
/// <param name="line">Indicates the line (row or column) index that the current pattern used.</param>
/// <param name="blockMask">Indicates the block mask.</param>
/// <param name="lineMask">Indicates the line mask.</param>
/// <param name="intersectionMask">Indicates the intersection mask.</param>
/// <param name="isCannibalistic">Indicates whether the SdC is cannibalistic.</param>
/// <param name="isolatedDigitsMask">The isolated digits mask.</param>
/// <param name="blockCells">Indicates the cells that the current pattern used in a block.</param>
/// <param name="lineCells">Indicates the cells that the current pattern used in a line (row or column).</param>
/// <param name="intersectionCells">
/// Indicates the cells that the current pattern used in an intersection of <see cref="BlockCells"/> and <see cref="LineCells"/>.
/// </param>
public sealed partial class SueDeCoqStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] House block,
	[PrimaryConstructorParameter] House line,
	[PrimaryConstructorParameter] Mask blockMask,
	[PrimaryConstructorParameter] Mask lineMask,
	[PrimaryConstructorParameter] Mask intersectionMask,
	[PrimaryConstructorParameter] bool isCannibalistic,
	[PrimaryConstructorParameter] Mask isolatedDigitsMask,
	[PrimaryConstructorParameter] scoped ref readonly CellMap blockCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap lineCells,
	[PrimaryConstructorParameter] scoped ref readonly CellMap intersectionCells
) : LockedSetStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 50;

	/// <inheritdoc/>
	public override Technique Code
		=> (IsCannibalistic, IsolatedDigitsMask) switch
		{
			(true, _) => Technique.SueDeCoqCannibalism,
			(_, not 0) => Technique.SueDeCoqIsolated,
			_ => Technique.SueDeCoq
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [IntersectionCellsStr, IntersectionDigitsStr, BlockCellsStr, BlockDigitsStr, LineCellsStr, LineDigitsStr]),
			new(ChineseLanguage, [IntersectionCellsStr, IntersectionDigitsStr, BlockCellsStr, BlockDigitsStr, LineCellsStr, LineDigitsStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new SueDeCoqIsolatedFactor(), new SueDeCoqCannibalismFactor()];

	private string IntersectionCellsStr => Options.Converter.CellConverter(IntersectionCells);

	private string IntersectionDigitsStr
		=> new LiteralCoordinateConverter(DigitsSeparator: string.Empty).DigitConverter(IntersectionMask);

	private string BlockCellsStr => Options.Converter.CellConverter(BlockCells);

	private string BlockDigitsStr => new LiteralCoordinateConverter(DigitsSeparator: string.Empty).DigitConverter(BlockMask);

	private string LineCellsStr => Options.Converter.CellConverter(LineCells);

	private string LineDigitsStr => new LiteralCoordinateConverter(DigitsSeparator: string.Empty).DigitConverter(LineMask);
}
