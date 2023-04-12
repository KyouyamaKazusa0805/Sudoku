namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Sue de Coq</b> technique.
/// </summary>
public sealed class UniqueRectangleWithSueDeCoqStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	bool isAvoidable,
	int block,
	int line,
	Mask blockMask,
	Mask lineMask,
	Mask intersectionMask,
	bool isCannibalistic,
	Mask isolatedDigitsMask,
	scoped in CellMap blockCells,
	scoped in CellMap lineCells,
	scoped in CellMap intersectionCells,
	int absoluteOffset
) : UniqueRectangleStep(
	conclusions,
	views,
	isAvoidable ? Technique.AvoidableRectangleSueDeCoq : Technique.UniqueRectangleSueDeCoq,
	digit1,
	digit2,
	cells,
	isAvoidable,
	absoluteOffset
)
{
	/// <summary>
	/// Indicates whether the Sue de Coq structure is a cannibalism.
	/// </summary>
	public bool IsCannibalistic { get; } = isCannibalistic;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .5M;

	/// <summary>
	/// Indicates the block index that the Sue de Coq structure used.
	/// </summary>
	public int Block { get; } = block;

	/// <summary>
	/// Indicates the line (row or column) index that the Sue de Coq structure used.
	/// </summary>
	public int Line { get; } = line;

	/// <summary>
	/// Indicates the mask that contains all digits from the block of the Sue de Coq structure.
	/// </summary>
	public Mask BlockMask { get; } = blockMask;

	/// <summary>
	/// Indicates the cells in the line of the Sue de Coq structure.
	/// </summary>
	public Mask LineMask { get; } = lineMask;

	/// <summary>
	/// Indicates the mask that contains all digits from the intersection of houses <see cref="Block"/> and <see cref="Line"/>.
	/// </summary>
	/// <seealso cref="Block"/>
	/// <seealso cref="Line"/>
	public Mask IntersectionMask { get; } = intersectionMask;

	/// <summary>
	/// Indicates the mask that contains all isolated digits.
	/// </summary>
	public Mask IsolatedDigitsMask { get; } = isolatedDigitsMask;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <summary>
	/// Indicates the cells in the block of the Sue de Coq structure.
	/// </summary>
	public CellMap BlockCells { get; } = blockCells;

	/// <summary>
	/// Indicates the cells in the line (row or column) of the Sue de Coq structure.
	/// </summary>
	public CellMap LineCells { get; } = lineCells;

	/// <summary>
	/// Indicates the cells in the intersection from houses <see cref="Block"/> and <see cref="Line"/>.
	/// </summary>
	/// <seealso cref="Block"/>
	/// <seealso cref="Line"/>
	public CellMap IntersectionCells { get; } = intersectionCells;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[]
		{
			(ExtraDifficultyCaseNames.Size, (LineCells | BlockCells).Count * .1M),
			(ExtraDifficultyCaseNames.Isolated, !IsCannibalistic && IsolatedDigitsMask != 0 ? .1M : 0),
			(ExtraDifficultyCaseNames.Cannibalism, IsCannibalistic ? .1M : 0),
			(ExtraDifficultyCaseNames.Avoidable, IsAvoidable ? .1M : 0)
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { D1Str, D2Str, CellsStr, MergedCellsStr, DigitsStr } },
			{ "zh", new[] { D1Str, D2Str, CellsStr, MergedCellsStr, DigitsStr } }
		};

	private string MergedCellsStr => (LineCells | BlockCells).ToString();

	private string DigitsStr => DigitMaskFormatter.Format((Mask)(LineMask | BlockMask), FormattingMode.Normal);
}
