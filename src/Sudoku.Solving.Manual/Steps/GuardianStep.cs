namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Guardian</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="Loop">Indicates the loop cells used.</param>
/// <param name="Guardians">Indicates the guardian cells used.</param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed partial record GuardianStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit,
	scoped in CellMap Loop,
	scoped in CellMap Guardians
) :
	NegativeRankStep(Conclusions, Views),
	IDistinctableStep<GuardianStep>,
	ILoopLikeStep,
	IStepWithRank,
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public bool? IsNice => null;

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 5.5M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.Size, A004526(Loop.Count + A004526(Guardians.Count)) * .1M) };

	/// <inheritdoc/>
	public int Rank => -1;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BrokenWing;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.LongChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.BrokenWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[ResourceTextFormatter]
	private partial string CellsStr() => Loop.ToString();

	[ResourceTextFormatter]
	private partial string GuardianSingularOrPlural() => R[Guardians.Count == 1 ? "GuardianSingular" : "GuardianPlural"]!;

	[ResourceTextFormatter]
	private partial string GuardianStr() => Guardians.ToString();


	/// <inheritdoc/>
	public static bool Equals(GuardianStep left, GuardianStep right)
		=> left.Digit == right.Digit && left.Loop == right.Loop && left.Guardians == right.Guardians;
}
