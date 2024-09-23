namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Reverse Bi-value Universal Grave Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="emptyCells"><inheritdoc/></param>
public sealed class ReverseBivalueUniversalGraveType1Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	Digit digit1,
	Digit digit2,
	ref readonly CellMap pattern,
	ref readonly CellMap emptyCells
) : ReverseBivalueUniversalGraveStep(conclusions, views, options, digit1, digit2, in pattern, in emptyCells)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [Cell1Str, Cell2Str, PatternStr]), new(SR.ChineseLanguage, [PatternStr, Cell1Str, Cell2Str])];
}