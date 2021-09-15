namespace Sudoku.Solving.Manual.Uniqueness.Polygons;

/// <summary>
/// Provides a usage of <b>Borescoper's deadly pattern type 2</b> (BDP) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Map">The cells used.</param>
/// <param name="DigitsMask">The digits mask.</param>
/// <param name="ExtraDigit">The extra digit.</param>
public sealed record class BdpType2StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	in Cells Map, short DigitsMask, int ExtraDigit
) : BdpStepInfo(Conclusions, Views, Map, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.4M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BdpType2;

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}
