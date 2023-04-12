namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bivalue Oddagon Type 2</b> technique.
/// </summary>
public sealed class BivalueOddagonType2Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in CellMap loopCells,
	int digit1,
	int digit2,
	int extraDigit
) : BivalueOddagonStep(conclusions, views, loopCells, digit1, digit2)
{
	/// <summary>
	/// Indicates the extra digit used.
	/// </summary>
	public int ExtraDigit { get; } = extraDigit;

	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new[] { (ExtraDifficultyCaseNames.ExtraDigit, .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { ExtraDigitStr, LoopStr } }, { "zh", new[] { LoopStr, ExtraDigitStr } } };

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
