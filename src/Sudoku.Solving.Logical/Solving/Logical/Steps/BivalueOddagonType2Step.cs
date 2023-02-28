namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="ExtraDigit">Indicates the extra digit.</param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record BivalueOddagonType2Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Loop,
	int Digit1,
	int Digit2,
	int ExtraDigit
) : BivalueOddagonStep(Conclusions, Views, Loop, Digit1, Digit2)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueOddagonType2;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, .1M) };


	[ResourceTextFormatter]
	internal string ExtraDigitStr() => (ExtraDigit + 1).ToString();

	[ResourceTextFormatter]
	internal string LoopStr() => Loop.ToString();
}
