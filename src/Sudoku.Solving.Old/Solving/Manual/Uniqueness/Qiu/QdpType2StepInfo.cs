namespace Sudoku.Solving.Manual.Uniqueness.Qiu;

/// <summary>
/// Provides a usage of <b>Qiu's deadly pattern type 2</b> (QDP) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Pattern">The pattern.</param>
/// <param name="ExtraDigit">The extra digit.</param>
public sealed record class QdpType2StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Pattern Pattern, int ExtraDigit
) : QdpStepInfo(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override decimal Difficulty => base.Difficulty + .1M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.QdpType2;

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}
