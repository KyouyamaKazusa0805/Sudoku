namespace Sudoku.Solving.Manual.RankTheory;

/// <summary>
/// Provides a usage of <b>bi-value oddagon type 3</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Loop">The loop used.</param>
/// <param name="Digit1">The digit 1.</param>
/// <param name="Digit2">The digit 2.</param>
/// <param name="ExtraDigits">All extra digits.</param>
/// <param name="ExtraCells">All extra cells.</param>
public sealed record class BivalueOddagonType3StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	in Cells Loop, int Digit1, int Digit2, short ExtraDigits, in Cells ExtraCells
) : BivalueOddagonStepInfo(Conclusions, Views, Loop, Digit1, Digit2)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.0M + (ExtraCells.Count >> 1) * .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueOddagonType3;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	[FormatItem]
	private string LoopStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Loop.ToString();
	}

	[FormatItem]
	private string Digit1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit1 + 1).ToString();
	}

	[FormatItem]
	private string Digit2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit2 + 1).ToString();
	}

	[FormatItem]
	private string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(ExtraDigits).ToString();
	}

	[FormatItem]
	private string ExtraCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ExtraCells.ToString();
	}
}
