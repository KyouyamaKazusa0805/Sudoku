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
	[DataMember] House block,
	[DataMember] House line,
	[DataMember] Mask blockMask,
	[DataMember] Mask lineMask,
	[DataMember] Mask intersectionMask,
	[DataMember] bool isCannibalistic,
	[DataMember] Mask isolatedDigitsMask,
	[DataMember] scoped in CellMap blockCells,
	[DataMember] scoped in CellMap lineCells,
	[DataMember] scoped in CellMap intersectionCells
) : ZeroRankStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.0M;

	/// <inheritdoc/>
	public override Technique Code
		=> (IsCannibalistic, IsolatedDigitsMask) switch
		{
			(true, _) => Technique.SueDeCoqCannibalism,
			(_, not 0) => Technique.SueDeCoqIsolated,
			_ => Technique.SueDeCoq
		};

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [
			new(ExtraDifficultyCaseNames.Isolated, IsolatedDigitsMask != 0 ? .1M : 0),
			new(ExtraDifficultyCaseNames.Cannibalism, IsCannibalistic ? .2M : 0)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [IntersectionCellsStr, IntersectionDigitsStr, BlockCellsStr, BlockDigitsStr, LineCellsStr, LineDigitsStr]),
			new(ChineseLanguage, [IntersectionCellsStr, IntersectionDigitsStr, BlockCellsStr, BlockDigitsStr, LineCellsStr, LineDigitsStr])
		];

	private string IntersectionCellsStr => IntersectionCells.ToString();

	private string IntersectionDigitsStr => DigitNotation.ToString(IntersectionMask, DigitNotation.Kind.Compact);

	private string BlockCellsStr => BlockCells.ToString();

	private string BlockDigitsStr => DigitNotation.ToString(BlockMask, DigitNotation.Kind.Compact);

	private string LineCellsStr => LineCells.ToString();

	private string LineDigitsStr => DigitNotation.ToString(LineMask, DigitNotation.Kind.Compact);
}
