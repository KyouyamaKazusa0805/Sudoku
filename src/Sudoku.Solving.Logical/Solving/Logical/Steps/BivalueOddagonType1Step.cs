namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon Type 1</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="ExtraCell">Indicates the extra cell.</param>
internal sealed record BivalueOddagonType1Step(
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

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { CellStr, Digit1Str, Digit2Str, LoopStr } },
			{ "zh", new[] { CellStr, Digit1Str, Digit2Str, LoopStr } }
		};

	private string CellStr => RxCyNotation.ToCellString(ExtraCell);

	private string Digit1Str => (Digit1 + 1).ToString();

	private string Digit2Str => (Digit2 + 1).ToString();

	private string LoopStr => Loop.ToString();
}
