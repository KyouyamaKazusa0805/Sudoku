namespace Sudoku.Solving.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Death Blossom Cell Type</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="HubCell">Indicates a cell as the hub of petals.</param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="Petals">Indicates the petals used.</param>
internal sealed record DeathBlossomCellTypeStep(
	ConclusionList Conclusions,
	ViewList Views,
	int HubCell,
	short DigitsMask,
	AlmostLockedSet[] Petals
) : DeathBlossomStep(Conclusions, Views, DigitsMask, Petals), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 8.3M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			(
				PhasedDifficultyRatingKinds.Petals,
				Petals.Length switch { >= 2 and < 5 => .1M, >= 5 and < 7 => .2M, _ => .3M }
			)
		};

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.DeathBlossomCellType;

	[ResourceTextFormatter]
	internal string CellStr() => RxCyNotation.ToCellString(HubCell);
}
