namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Clue Cover RW's Type</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern">Indicates the pattern.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="ChuteIndex">Indicates the global chute index.</param>
internal sealed record UniquenessClueCoverRwTypeStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in Cells Pattern,
	short DigitsMask,
	int ChuteIndex
) : UniquenessClueCoverStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .3M;

	/// <inheritdoc/>
	public override int Type
	{
		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => throw new NotSupportedException("This type is special.");
	}

	public override Technique TechniqueCode => Technique.UniquenessClueCoverRwType;

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
