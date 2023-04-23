namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="block">Indicates the block that the real empty rectangle structure lis in.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class EmptyRectangleStep(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit,
	[PrimaryConstructorParameter] House block,
	[PrimaryConstructorParameter] scoped in Conjugate conjugatePair
) : SingleDigitPatternStep(conclusions, views, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.6M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override Technique Code => Technique.EmptyRectangle;

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
