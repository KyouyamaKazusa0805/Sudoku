namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Sue de Coq</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
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
	[PrimaryConstructorParameter] int block,
	[PrimaryConstructorParameter] int line,
	[PrimaryConstructorParameter] Mask blockMask,
	[PrimaryConstructorParameter] Mask lineMask,
	[PrimaryConstructorParameter] Mask intersectionMask,
	[PrimaryConstructorParameter] bool isCannibalistic,
	[PrimaryConstructorParameter] Mask isolatedDigitsMask,
	[PrimaryConstructorParameter] scoped in CellMap blockCells,
	[PrimaryConstructorParameter] scoped in CellMap lineCells,
	[PrimaryConstructorParameter] scoped in CellMap intersectionCells
) : ZeroRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.0M;

	/// <inheritdoc/>
	public override Technique Code => IsCannibalistic ? Technique.SueDeCoqCannibalism : Technique.SueDeCoq;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[]
		{
			(ExtraDifficultyCaseNames.Isolated, IsolatedDigitsMask != 0 ? .1M : 0),
			(ExtraDifficultyCaseNames.Cannibalism, IsCannibalistic ? .2M : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { IntersectionCellsStr, IntersectionDigitsStr, BlockCellsStr, BlockDigitsStr, LineCellsStr, LineDigitsStr } },
			{ "zh", new[] { IntersectionCellsStr, IntersectionDigitsStr, BlockCellsStr, BlockDigitsStr, LineCellsStr, LineDigitsStr } }
		};

	private string IntersectionCellsStr => IntersectionCells.ToString();

	private string IntersectionDigitsStr => DigitMaskFormatter.Format(IntersectionMask);

	private string BlockCellsStr => BlockCells.ToString();

	private string BlockDigitsStr => DigitMaskFormatter.Format(BlockMask);

	private string LineCellsStr => LineCells.ToString();

	private string LineDigitsStr => DigitMaskFormatter.Format(LineMask);
}
