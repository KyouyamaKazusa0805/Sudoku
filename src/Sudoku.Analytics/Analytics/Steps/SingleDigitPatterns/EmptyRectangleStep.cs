namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle</b> technique.
/// </summary>
public sealed class EmptyRectangleStep(Conclusion[] conclusions, View[]? views, int digit, int block, scoped in Conjugate conjugatePair) :
	SingleDigitPatternStep(conclusions, views, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.6M;

	/// <summary>
	/// Indicates the block that the real empty rectangle structure lis in.
	/// </summary>
	public int Block { get; } = block;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Technique Code => Technique.EmptyRectangle;

	/// <summary>
	/// Indicates the conjugate pair used.
	/// </summary>
	public Conjugate ConjugatePair { get; } = conjugatePair;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitStr, HouseStr, ConjStr } },
			{ "zh", new[] { DigitStr, HouseStr, ConjStr } }
		};

	private string DigitStr => (Digit + 1).ToString();

	private string HouseStr => HouseFormatter.Format(1 << Block);

	private string ConjStr => ConjugatePair.ToString();
}
