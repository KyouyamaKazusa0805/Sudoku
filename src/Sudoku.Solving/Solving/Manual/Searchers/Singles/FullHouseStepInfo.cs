namespace Sudoku.Solving.Manual.Singles;

/// <summary>
/// Indicates a usage of <b>full house</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Cell">The cell.</param>
/// <param name="Digit">The digit.</param>
public sealed record FullHouseStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit
) : SingleStepInfo(Conclusions, Views, Cell, Digit)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 1.0M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FullHouse;

	[FormatItem]
	private string CellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { Cell }.ToString();
	}

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}
}
