namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="Candidate">Indicates the extra candidate used.</param>
public sealed record QiuDeadlyPatternType1Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<View> Views,
	in QiuDeadlyPattern Pattern,
	int Candidate
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override int Type => 1;

	[FormatItem]
	internal string CandidateStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Candidates.Empty + Candidate).ToString();
	}
}
