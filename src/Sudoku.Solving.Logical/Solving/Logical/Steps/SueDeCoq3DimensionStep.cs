namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>3-dimensional Sue de Coq</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="RowDigitsMask">The row digits mask.</param>
/// <param name="ColumnDigitsMask">The column digits mask.</param>
/// <param name="BlockDigitsMask">The block digits mask.</param>
/// <param name="RowCells">The row cells map.</param>
/// <param name="ColumnCells">The column cells map.</param>
/// <param name="BlockCells">The block cells map.</param>
internal sealed record SueDeCoq3DimensionStep(
	ConclusionList Conclusions,
	ViewList Views,
	short RowDigitsMask,
	short ColumnDigitsMask,
	short BlockDigitsMask,
	scoped in CellMap RowCells,
	scoped in CellMap ColumnCells,
	scoped in CellMap BlockCells
) : NonnegativeRankStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.5M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.SueDeCoq3Dimension;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.Als;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.SueDeCoq;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { Cells1Str, Digits1Str, Cells2Str, Digits2Str, Cells3Str, Digits3Str } },
			{ "zh", new[] { Cells1Str, Digits1Str, Cells2Str, Digits2Str, Cells3Str, Digits3Str } }
		};

	private string Cells1Str => RowCells.ToString();

	private string Digits1Str => DigitMaskFormatter.Format(RowDigitsMask, FormattingMode.Normal);

	private string Cells2Str => ColumnCells.ToString();

	private string Digits2Str => DigitMaskFormatter.Format(ColumnDigitsMask, FormattingMode.Normal);

	private string Cells3Str => BlockCells.ToString();

	private string Digits3Str => DigitMaskFormatter.Format(BlockDigitsMask, FormattingMode.Normal);
}
