namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="Candidate">Indicates the extra candidate used.</param>
[StepDisplayingFeature(StepDisplayingFeature.VeryRare)]
internal sealed record QiuDeadlyPatternType1Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in QiuDeadlyPattern Pattern,
	int Candidate
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern)
{
	/// <inheritdoc/>
	public override int Type => 1;


	[ResourceTextFormatter]
	internal string CandidateStr() => (Candidates.Empty + Candidate).ToString();
}
