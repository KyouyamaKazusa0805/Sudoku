namespace Sudoku.Solving.Manual.Uniqueness.Extended;

/// <summary>
/// Provides a usage of <b>extended rectangle</b> (XR) type 2 technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Cells">All cells.</param>
/// <param name="DigitsMask">All digits mask.</param>
/// <param name="ExtraDigit">The extra digit.</param>
public sealed record class XrType2StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	in Cells Cells, short DigitsMask, int ExtraDigit
) : XrStepInfo(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.XrType2;

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}
