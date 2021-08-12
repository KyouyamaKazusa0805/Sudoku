namespace Sudoku.Solving.Manual.Uniqueness.Square;

/// <summary>
/// Provides a usage of <b>unique square type 2</b> (US) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Cells">The cells.</param>
/// <param name="DigitsMask">The digits mask.</param>
/// <param name="ExtraDigit">The extra digit.</param>
public sealed record UsType2StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	in Cells Cells, short DigitsMask, int ExtraDigit
) : UsStepInfo(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.UsType2;

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}
