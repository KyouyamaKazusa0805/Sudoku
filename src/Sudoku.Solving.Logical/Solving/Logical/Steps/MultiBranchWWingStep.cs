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
) : IrregularWingStep(Conclusions, Views)
{
	/// <summary>
	/// Indicates the number of branches of the technique.
	/// </summary>
	public int Size => Leaves.Count;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => .3M, 4 => .6M, 5 => 1.0M, _ => 1.4M }) };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.MultiBranchWWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Rarity Rarity => Size switch { 2 => Rarity.Often, 3 => Rarity.Seldom, _ => Rarity.HardlyEver };


	[ResourceTextFormatter]
	internal string LeavesStr() => RxCyNotation.ToCellsString(Leaves);

	[ResourceTextFormatter]
	internal string RootStr() => RxCyNotation.ToCellsString(Root);

	[ResourceTextFormatter]
	internal string HouseStr() => HouseFormatter.Format(House);
}
