namespace Sudoku.Solving.Manual.Steps;

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

	[FormatItem]
	internal string Cells1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => RowCells.ToString();
	}

	[FormatItem]
	internal string Digits1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format(RowDigitsMask, FormattingMode.Normal);
	}

	[FormatItem]
	internal string Cells2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ColumnCells.ToString();
	}

	[FormatItem]
	internal string Digits2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format(ColumnDigitsMask, FormattingMode.Normal);
	}

	[FormatItem]
	internal string Cells3Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => BlockCells.ToString();
	}

	[FormatItem]
	internal string Digits3Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format(BlockDigitsMask, FormattingMode.Normal);
	}
}
