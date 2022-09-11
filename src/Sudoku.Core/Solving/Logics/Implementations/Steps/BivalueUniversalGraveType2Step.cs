namespace Sudoku.Solving.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit">Indicates the digit used.</param>
/// <param name="Cells">Indicates the cells used.</param>
internal sealed record BivalueUniversalGraveType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit,
	scoped in CellMap Cells
) : BivalueUniversalGraveStep(Conclusions, Views), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.ExtraDigit, A002024(Cells.Count) * .1M) };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueUniversalGraveType2;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	internal string ExtraDigitStr() => (Digit + 1).ToString();

	[ResourceTextFormatter]
	internal string CellsStr() => Cells.ToString();
}
