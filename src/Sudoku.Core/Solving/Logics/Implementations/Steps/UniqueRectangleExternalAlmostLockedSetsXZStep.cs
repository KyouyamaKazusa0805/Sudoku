namespace Sudoku.Solving.Logics.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle External Almost Locked Sets XZ Rule</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="GuardianCells">Indicates the cells that the guardians lie in.</param>
/// <param name="AlmostLockedSet">The almost locked sets used.</param>
/// <param name="IsIncomplete">Indicates whether the rectangle is incomplete.</param>
/// <param name="IsAvoidable">Indicates whether the structure is based on avoidable rectangle.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable | StepDisplayingFeature.ConstructedTechnique)]
internal sealed record UniqueRectangleExternalAlmostLockedSetsXzStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	scoped in CellMap GuardianCells,
	AlmostLockedSet AlmostLockedSet,
	bool IsIncomplete,
	bool IsAvoidable,
	int AbsoluteOffset
) :
	UniqueRectangleStep(
		Conclusions,
		Views,
		IsAvoidable
			? Technique.AvoidableRectangleExternalAlmostLockedSetsXz
			: Technique.UniqueRectangleExternalAlmostLockedSetsXz,
		Digit1,
		Digit2,
		Cells,
		false,
		AbsoluteOffset
	),
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 4.5M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			(PhasedDifficultyRatingKinds.Guardian, A004526(GuardianCells.Count) * .1M),
			(PhasedDifficultyRatingKinds.Avoidable, IsAvoidable ? .1M : 0),
			(PhasedDifficultyRatingKinds.Incompleteness, IsIncomplete ? .1M : 0)
		};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;


	[ResourceTextFormatter]
	internal string GuardianCellsStr() => GuardianCells.ToString();

	[ResourceTextFormatter]
	internal string AnotherAlsStr() => AlmostLockedSet.ToString();
}
