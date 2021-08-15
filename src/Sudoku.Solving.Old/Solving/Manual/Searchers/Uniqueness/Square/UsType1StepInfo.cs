namespace Sudoku.Solving.Manual.Uniqueness.Square;

/// <summary>
/// Provides a usage of <b>unique square type 1</b> (US) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Cells">The cells.</param>
/// <param name="DigitsMask">The digits mask.</param>
/// <param name="Candidate">Indicates the true candidate.</param>
public sealed record UsType1StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	in Cells Cells, short DigitsMask, int Candidate
) : UsStepInfo(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.UsType1;

	[FormatItem]
	private string CandidateStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates { Candidate }.ToString();
	}
}
