namespace Sudoku.Solving.Manual.Uniqueness.Reversal;

/// <summary>
/// Provides a usage of <b>reverse bi-value universal grave type 4</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Loop">All cells used.</param>
/// <param name="ExtraCells">All extra cells.</param>
/// <param name="Digit1">The digit 1 used.</param>
/// <param name="Digit2">The digit 2 used.</param>
/// <param name="ConjugatePair">Indicates the conjugate pair.</param>
public sealed record class ReverseBugType4StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Loop,
	in Cells ExtraCells, int Digit1, int Digit2, in ConjugatePair ConjugatePair
) : ReverseBugStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.7M;

	/// <inheritdoc/>
	public override Technique TechniqueCode =>
		Loop.Count == 4 ? Technique.ReverseUrType4 : Technique.ReverseUlType4;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ((ConjugatePair.Digit == Digit1 ? Digit2 : Digit1) + 1).ToString();
	}

	[FormatItem]
	private string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Loop.ToString();
	}
}
