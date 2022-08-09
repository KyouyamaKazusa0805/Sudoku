namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Pair Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">The cells used.</param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="EmptyRectangleBlock">The empty rectangle block used.</param>
internal sealed record FireworkPairType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in Cells Cells,
	short DigitsMask,
	int EmptyRectangleBlock
) : FireworkStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .2M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FireworkPairType3;

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
	internal string EmptyRectangleStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => HouseFormatter.Format(1 << EmptyRectangleBlock);
	}
}
