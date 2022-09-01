namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="DigitsMask">Indicates the mask that contains the digits used.</param>
internal abstract partial record ExtendedRectangleStep(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask
) : DeadlyPatternStep(Conclusions, Views), IStepWithPhasedDifficulty, IStepWithDistinctionDegree
{
	/// <summary>
	/// Indicates the type of the step. The value must be between 1 and 4.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public int DistinctionDegree => 1;

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 4.5M;

	/// <inheritdoc/>
	public virtual (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.Size, (A004526(Cells.Count) - 2) * .1M) };

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode => Enum.Parse<Technique>($"ExtendedRectangleType{Type}");

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.ExtendedRectangle;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => base.TechniqueTags;

	/// <summary>
	/// Indicates the digits string.
	/// </summary>
	[ResourceTextFormatter]
	private partial string DigitsStr() => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	/// <summary>
	/// Indicates the cells string.
	/// </summary>
	[ResourceTextFormatter]
	private partial string CellsStr() => Cells.ToString();
}
