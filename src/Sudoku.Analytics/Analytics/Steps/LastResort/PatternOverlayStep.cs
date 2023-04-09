namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Pattern Overlay</b> technique.
/// </summary>
public sealed class PatternOverlayStep(Conclusion[] conclusions) : LastResortStep(conclusions, null)
{
	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit => Conclusions[0].Digit;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.5M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique Code => Technique.PatternOverlay;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitStr } }, { "zh", new[] { DigitStr } } };

	private string DigitStr => (Digit + 1).ToString();
}
