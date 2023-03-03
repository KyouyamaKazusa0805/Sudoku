namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
/// <param name="ExtraDigitsMask">Indicates the mask that contains all extra digits used.</param>
internal sealed record BivalueOddagonType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Loop,
	int Digit1,
	int Digit2,
	scoped in CellMap ExtraCells,
	short ExtraDigitsMask
) : BivalueOddagonStep(Conclusions, Views, Loop, Digit1, Digit2)
{
	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueOddagonType3;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, (ExtraCells.Count >> 1) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { LoopStr, Digit1Str, Digit2Str, DigitsStr, ExtraCellsStr } },
			{ "zh", new[] { Digit1Str, Digit2Str, LoopStr, ExtraCellsStr, DigitsStr } }
		};

	private string LoopStr => Loop.ToString();

	private string Digit1Str => (Digit1 + 1).ToString();

	private string Digit2Str => (Digit2 + 1).ToString();

	private string DigitsStr => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	private string ExtraCellsStr => ExtraCells.ToString();
}
