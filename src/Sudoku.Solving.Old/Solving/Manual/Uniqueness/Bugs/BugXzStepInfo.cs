namespace Sudoku.Solving.Manual.Uniqueness.Bugs;

/// <summary>
/// Provides a usage of <b>bivalue universal grave XZ rule</b> (BUG-XZ) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="DigitsMask">The digits mask.</param>
/// <param name="Cells">All cells.</param>
/// <param name="ExtraCell">The extra cell.</param>
public sealed record BugXzStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	short DigitsMask, IReadOnlyList<int> Cells, int ExtraCell
) : BugStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .2M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BugXz;

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells(Cells).ToString();
	}

	[FormatItem]
	private string ExtraCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { ExtraCell }.ToString();
	}
}
