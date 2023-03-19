namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 2</b> technique.
/// </summary>
public sealed class QiuDeadlyPatternType2Step(Conclusion[] conclusions, View[]? views, scoped in QiuDeadlyPattern pattern, int extraDigit) :
	QiuDeadlyPatternStep(conclusions, views, pattern)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <summary>
	/// Indicates the extra digit used.
	/// </summary>
	public int ExtraDigit { get; } = extraDigit;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraDigit, .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { PatternStr, ExtraDigitStr } }, { "zh", new[] { PatternStr, ExtraDigitStr } } };

	private string ExtraDigitStr => (ExtraDigit + 1).ToString();
}
