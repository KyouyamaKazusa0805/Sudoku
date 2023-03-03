namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Sue de Coq</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Block">The block.</param>
/// <param name="Line">The line.</param>
/// <param name="BlockMask">The block mask.</param>
/// <param name="LineMask">The line mask.</param>
/// <param name="IntersectionMask">The intersection mask.</param>
/// <param name="IsCannibalistic">Indicates whether the SdC is cannibalistic.</param>
/// <param name="IsolatedDigitsMask">The isolated digits mask.</param>
/// <param name="BlockCells">The map of block cells.</param>
/// <param name="LineCells">The map of line cells.</param>
/// <param name="IntersectionCells">The map of intersection cells.</param>
internal sealed record SueDeCoqStep(
	Conclusion[] Conclusions,
	View[]? Views,
	int Block,
	int Line,
	short BlockMask,
	short LineMask,
	short IntersectionMask,
	bool IsCannibalistic,
	short IsolatedDigitsMask,
	scoped in CellMap BlockCells,
	scoped in CellMap LineCells,
	scoped in CellMap IntersectionCells
) : NonnegativeRankStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.0M;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.RankTheory | TechniqueTags.Als;

	/// <inheritdoc/>
	public override Technique TechniqueCode => IsCannibalistic ? Technique.SueDeCoqCannibalism : Technique.SueDeCoq;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.SueDeCoq;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

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
