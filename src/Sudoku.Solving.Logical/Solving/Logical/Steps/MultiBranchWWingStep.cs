namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-Branch W-Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Leaves">The leaves of the pattern.</param>
/// <param name="Root">The root cells that corresponds to each leaf.</param>
/// <param name="House">The house used.</param>
internal sealed record MultiBranchWWingStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Leaves,
	scoped in CellMap Root,
	int House
) : IrregularWingStep(Conclusions, Views), IStepWithSize, IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public int Size => Leaves.Count;

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 3.7M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues => new[] { (PhasedDifficultyRatingKinds.Size, Size * .3M) };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.MultiBranchWWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;


	[ResourceTextFormatter]
	internal string LeavesStr() => RxCyNotation.ToCellsString(Leaves);

	[ResourceTextFormatter]
	internal string RootStr() => RxCyNotation.ToCellsString(Root);

	[ResourceTextFormatter]
	internal string HouseStr() => HouseFormatter.Format(House);
}
