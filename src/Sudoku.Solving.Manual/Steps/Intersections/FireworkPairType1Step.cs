namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">The cells used.</param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="ExtraCell1">The extra cell 1.</param>
/// <param name="ExtraCell2">The extra cell 2.</param>
internal sealed record FireworkPairType1Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	int ExtraCell1,
	int ExtraCell2
) : FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkPairType1;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);
	}

	[FormatItem]
	internal string ExtraCell1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => RxCyNotation.ToCellString(ExtraCell1);
	}

	[FormatItem]
	internal string ExtraCell2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => RxCyNotation.ToCellString(ExtraCell2);
	}
}
