namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
public sealed class ExtendedRectangleType1Step(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	in CellMap cells,
	Mask digitsMask
) : ExtendedRectangleStep(conclusions, views, options, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [DigitsStr, CellsStr]), new(SR.ChineseLanguage, [DigitsStr, CellsStr])];
}
