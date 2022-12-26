namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TechniqueCode2"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="Branches">Indicates the branches used.</param>
/// <param name="Petals">Indicates the petals used.</param>
/// <param name="ExtraDigitsMask">Indicates the mask that contains all extra digits.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable | StepDisplayingFeature.ConstructedTechnique)]
internal sealed record UniqueRectangleWithWingStep(
	ConclusionList Conclusions,
	ViewList Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	bool IsAvoidable,
	scoped in CellMap Branches,
	scoped in CellMap Petals,
	short ExtraDigitsMask,
	int AbsoluteOffset
) :
	UniqueRectangleStep(
		Conclusions,
		Views,
		TechniqueCode2,
		Digit1,
		Digit2,
		Cells,
		IsAvoidable,
		AbsoluteOffset
	),
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			(PhasedDifficultyRatingKinds.Avoidable, IsAvoidable ? .1M : 0),
			(
				PhasedDifficultyRatingKinds.WingSize,
				TechniqueCode switch
				{
					Technique.UniqueRectangleXyWing or Technique.AvoidableRectangleXyWing => .2M,
					Technique.UniqueRectangleXyzWing or Technique.AvoidableRectangleXyzWing => .3M,
					Technique.UniqueRectangleWxyzWing or Technique.AvoidableRectangleWxyzWing => .5M,
					_ => throw new NotSupportedException("The specified technique code is not supported.")
				}
			)
		};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;


	[ResourceTextFormatter]
	internal string BranchesStr() => Branches.ToString();

	[ResourceTextFormatter]
	internal string DigitsStr() => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);
}
