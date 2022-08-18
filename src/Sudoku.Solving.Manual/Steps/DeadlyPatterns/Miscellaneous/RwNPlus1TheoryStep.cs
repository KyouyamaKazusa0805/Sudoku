namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>RW's "n + 1" Theory</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern">Indicates the pattern.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="ChuteIndex">Indicates the global chute index.</param>
internal sealed record RwNPlus1TheoryStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in Cells Pattern,
	short DigitsMask,
	int ChuteIndex
) : DeadlyPatternStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 7.5M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.RwNPlus1Theory;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.HardlyEver;

	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => RxCyNotation.ToCellsString(Pattern);
	}

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format(DigitsMask);
	}
}
