namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Sue de Coq</b> technique.
/// </summary>
public sealed class SueDeCoqStep(
	Conclusion[] conclusions,
	View[]? views,
	int block,
	int line,
	short blockMask,
	short lineMask,
	short intersectionMask,
	bool isCannibalistic,
	short isolatedDigitsMask,
	scoped in CellMap blockCells,
	scoped in CellMap lineCells,
	scoped in CellMap intersectionCells
) : ZeroRankStep(conclusions, views)
{
	/// <summary>
	/// Indicates whether the SdC is cannibalistic.
	/// </summary>
	public bool IsCannibalistic { get; } = isCannibalistic;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.0M;

	/// <summary>
	/// Indicates the block index that the current pattern used.
	/// </summary>
	public int Block { get; } = block;

	/// <summary>
	/// Indicates the line (row or column) index that the current pattern used.
	/// </summary>
	public int Line { get; } = line;

	/// <summary>
	/// Indicates the block mask.
	/// </summary>
	public short BlockMask { get; } = blockMask;

	/// <summary>
	/// Indicates the line mask.
	/// </summary>
	public short LineMask { get; } = lineMask;

	/// <summary>
	/// Indicates the intersection mask.
	/// </summary>
	public short IntersectionMask { get; } = intersectionMask;

	/// <summary>
	/// The isolated digits mask.
	/// </summary>
	public short IsolatedDigitsMask { get; } = isolatedDigitsMask;

	/// <inheritdoc/>
	public override Technique Code => IsCannibalistic ? Technique.SueDeCoqCannibalism : Technique.SueDeCoq;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <summary>
	/// Indicates the cells that the current pattern used in a block.
	/// </summary>
	public CellMap BlockCells { get; } = blockCells;

	/// <summary>
	/// Indicates the cells that the current pattern used in a line (row or column).
	/// </summary>
	public CellMap LineCells { get; } = lineCells;

	/// <summary>
	/// Indicates the cells that the current pattern used in an intersection of <see cref="BlockCells"/> and <see cref="LineCells"/>.
	/// </summary>
	/// <seealso cref="BlockCells"/>
	/// <seealso cref="LineCells"/>
	public CellMap IntersectionCells { get; } = intersectionCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Isolated, IsolatedDigitsMask != 0 ? .1M : 0),
			new(ExtraDifficultyCaseNames.Cannibalism, IsCannibalistic ? .2M : 0)
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
