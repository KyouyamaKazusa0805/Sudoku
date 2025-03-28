namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is a <b>Unique Loop Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="loop"><inheritdoc/></param>
/// <param name="loopPath"><inheritdoc/></param>
public sealed class UniqueLoopType1Step(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Digit digit1,
	Digit digit2,
	in CellMap loop,
	Cell[] loopPath
) : UniqueLoopStep(conclusions, views, options, digit1, digit2, loop, loopPath)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [Digit1Str, Digit2Str, LoopStr]), new(SR.ChineseLanguage, [Digit1Str, Digit2Str, LoopStr])];
}
