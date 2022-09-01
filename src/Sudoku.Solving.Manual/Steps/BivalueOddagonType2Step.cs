namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit.</param>
internal sealed partial record BivalueOddagonType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Loop,
	int Digit1,
	int Digit2,
	int ExtraDigit
) : BivalueOddagonStep(Conclusions, Views, Loop, Digit1, Digit2), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.ExtraDigit, .1M) };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueOddagonType2;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[ResourceTextFormatter]
	private partial string ExtraDigitStr() => (ExtraDigit + 1).ToString();

	[ResourceTextFormatter]
	private partial string LoopStr() => Loop.ToString();
}
