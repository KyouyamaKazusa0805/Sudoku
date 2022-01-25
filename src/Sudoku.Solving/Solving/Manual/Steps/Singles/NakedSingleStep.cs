namespace Sudoku.Solving.Manual.Steps.Singles;

/// <summary>
/// Provides with a step that is a <b>Naked Single</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cell"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
public sealed record NakedSingleStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Cell,
	int Digit
) : SingleStep(Conclusions, Views, Cell, Digit)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 2.3M;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.NakedSingle;

	[FormatItem]
	private string CellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Coordinate((byte)Cell).ToString();
	}

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}
}
