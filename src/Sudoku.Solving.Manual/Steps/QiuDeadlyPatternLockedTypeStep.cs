namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Locked Type</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Pattern"><inheritdoc/></param>
/// <param name="Candidates">Indicates the candidates used.</param>
[StepDisplayingFeature(StepDisplayingFeature.VeryRare)]
internal sealed partial record QiuDeadlyPatternLockedTypeStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in QiuDeadlyPattern Pattern,
	IReadOnlyList<int> Candidates
) : QiuDeadlyPatternStep(Conclusions, Views, Pattern), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.LockedDigit, .2M) };

	/// <inheritdoc/>
	public override int Type => 5;


	[ResourceTextFormatter]
	private partial string CandidateStr() => new Candidates(Candidates).ToString();

	[ResourceTextFormatter]
	private partial string Quantifier() => Candidates.Count switch { 1 => string.Empty, 2 => " both", _ => " all" };

	[ResourceTextFormatter]
	private partial string Number() => Candidates.Count == 1 ? " the" : $" {Candidates.Count}";

	[ResourceTextFormatter]
	private partial string SingularOrPlural() => Candidates.Count == 1 ? "candidate" : "candidates";

	[ResourceTextFormatter]
	private partial string BeVerb() => Candidates.Count == 1 ? "is" : "are";
}
