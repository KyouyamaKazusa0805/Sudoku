namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Qiu's Deadly Pattern Type 4</b> technique.
/// </summary>
public sealed class QiuDeadlyPatternType4Step(
	Conclusion[] conclusions,
	View[]? views,
	scoped in QiuDeadlyPattern pattern,
	scoped in Conjugate conjugatePair
) : QiuDeadlyPatternStep(conclusions, views, pattern)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <summary>
	/// Indicates the conjugate pair used.
	/// </summary>
	public Conjugate ConjugatePair { get; } = conjugatePair;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => new[] { (ExtraDifficultyCaseNames.ConjugatePair, .2M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { PatternStr, ConjStr } }, { "zh", new[] { ConjStr, PatternStr } } };

	private string ConjStr => ConjugatePair.ToString();
}
