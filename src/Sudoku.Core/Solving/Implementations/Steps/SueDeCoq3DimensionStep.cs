namespace Sudoku.Solving.Implementations.Steps;

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
) : NonnegativeRankStep(Conclusions, Views), IStepWithRank
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.5M;

	/// <inheritdoc/>
	public int Rank => 0;

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

	[ResourceTextFormatter]
	internal string Cells1Str() => RowCells.ToString();

	[ResourceTextFormatter]
	internal string Digits1Str() => DigitMaskFormatter.Format(RowDigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string Cells2Str() => ColumnCells.ToString();

	[ResourceTextFormatter]
	internal string Digits2Str() => DigitMaskFormatter.Format(ColumnDigitsMask, FormattingMode.Normal);

	[ResourceTextFormatter]
	internal string Cells3Str() => BlockCells.ToString();

	[ResourceTextFormatter]
	internal string Digits3Str() => DigitMaskFormatter.Format(BlockDigitsMask, FormattingMode.Normal);
}
