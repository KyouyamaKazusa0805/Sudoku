namespace Sudoku.Solving.Manual.Uniqueness.Reversal;

/// <summary>
/// Provides a usage of <b>reverse bi-value universal grave type 3</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Loop">All cells used.</param>
/// <param name="ExtraCells">All extra cells.</param>
/// <param name="Digit1">The digit 1 used.</param>
/// <param name="Digit2">The digit 2 used.</param>
/// <param name="DigitsMask">All extra digits used.</param>
/// <param name="IsNaked">Indicates whether the subset is naked.</param>
public sealed record ReverseBugType3StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Loop,
	in Cells ExtraCells, int Digit1, int Digit2, short DigitsMask, bool IsNaked
) : ReverseBugStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.8M;

	/// <inheritdoc/>
	public override Technique TechniqueCode =>
		Loop.Count == 4 ? Technique.ReverseUrType3 : Technique.ReverseUlType3;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Loop.ToString();
	}

	[FormatItem]
	private string SubsetCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ExtraCells.ToString();
	}

	[FormatItem]
	private string SubsetDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	private string SubsetName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TextResources.Current[$"SubsetNamesSize{(ExtraCells.Count + 1).ToString()}"];
	}
}
