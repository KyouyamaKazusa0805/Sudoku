namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Oddagon Type 3</b> technique.
/// </summary>
public sealed class BivalueOddagonType3Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap loopCells,
	int digit1,
	int digit2,
	scoped in CellMap extraCells,
	short extraDigitsMask
) : BivalueOddagonStep(conclusions, views, loopCells, digit1, digit2)
{
	/// <inheritdoc/>
	public override int Type => 3;

	/// <summary>
	/// Indicates the mask that contains all extra digits used.
	/// </summary>
	public short ExtraDigitsMask { get; } = extraDigitsMask;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <summary>
	/// Indicates the extra cells used.
	/// </summary>
	public CellMap ExtraCells { get; } = extraCells;

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

	private string Digit1Str => (Digit1 + 1).ToString();

	private string Digit2Str => (Digit2 + 1).ToString();

	private string DigitsStr => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);

	private string ExtraCellsStr => ExtraCells.ToString();
}
