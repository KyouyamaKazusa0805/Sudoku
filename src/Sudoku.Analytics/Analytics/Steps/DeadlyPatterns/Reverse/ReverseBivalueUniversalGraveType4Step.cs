namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Reverse Bi-value Universal Grave Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="emptyCells"><inheritdoc/></param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class ReverseBivalueUniversalGraveType4Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit1,
	Digit digit2,
	scoped ref readonly CellMap pattern,
	scoped ref readonly CellMap emptyCells,
	[PrimaryConstructorParameter] scoped ref readonly Conjugate conjugatePair
) : ReverseBivalueUniversalGraveStep(conclusions, views, options, digit1, digit2, in pattern, in emptyCells)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 3;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [ConjugatePairStr]), new(ChineseLanguage, [ConjugatePairStr])];

	/// <inheritdoc/>
	private string ConjugatePairStr => Options.Converter.ConjugateConverter([ConjugatePair]);
}
