namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave + n</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Candidates">Indicates the true candidates.</param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record BivalueUniversalGraveMultipleStep(
	ConclusionList Conclusions,
	ViewList Views,
	IReadOnlyList<int> Candidates
) : BivalueUniversalGraveStep(Conclusions, Views), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override string Name => $"{base.Name} + {Candidates.Count}";

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty + .1M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.Size, A002024(Candidates.Count) * .1M) };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGravePlusN;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	internal string CandidatesStr() => new Candidates(Candidates).ToString();
}
