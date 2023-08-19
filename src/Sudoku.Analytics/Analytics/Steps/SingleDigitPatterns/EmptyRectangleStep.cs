namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="block">Indicates the block that the real empty rectangle pattern lis in.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class EmptyRectangleStep(
	Conclusion[] conclusions,
	View[]? views,
	Digit digit,
	[DataMember] House block,
	[DataMember] scoped in Conjugate conjugatePair
) : SingleDigitPatternStep(conclusions, views, digit)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.6M;

	/// <inheritdoc/>
	public override Technique Code => Technique.EmptyRectangle;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitStr, HouseStr, ConjStr]), new(ChineseLanguage, [DigitStr, HouseStr, ConjStr])];

	private string DigitStr => DigitNotation.ToString(Digit);

	private string HouseStr => HouseNotation.ToString(Block);

	private string ConjStr => ConjugatePair.ToString();
}
