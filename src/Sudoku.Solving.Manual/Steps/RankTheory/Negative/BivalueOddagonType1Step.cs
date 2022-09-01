namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="ExtraCell">Indicates the extra cell.</param>
internal sealed partial record BivalueOddagonType1Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Loop,
	int Digit1,
	int Digit2,
	int ExtraCell
) : BivalueOddagonStep(Conclusions, Views, Loop, Digit1, Digit2)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueOddagonType1;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.ReplacedByOtherTechniques;

	[ResourceTextFormatter]
	private partial string CellStr() => RxCyNotation.ToCellString(ExtraCell);

	[ResourceTextFormatter]
	private partial string Digit1Str() => (Digit1 + 1).ToString();

	[ResourceTextFormatter]
	private partial string Digit2Str() => (Digit2 + 1).ToString();

	[ResourceTextFormatter]
	private partial string LoopStr() => Loop.ToString();
}
