namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
public sealed class BorescoperDeadlyPatternType1Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	ref readonly CellMap cells,
	Mask digitsMask
) : BorescoperDeadlyPatternStep(conclusions, views, options, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [DigitsStr, CellsStr]), new(SR.ChineseLanguage, [DigitsStr, CellsStr])];
}
