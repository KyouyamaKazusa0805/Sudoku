namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chromatic Pattern Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="blocks"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="extraCell">Indicates the extra cell used.</param>
/// <param name="digitsMask"><inheritdoc/></param>
public sealed partial class ChromaticPatternType1Step(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	House[] blocks,
	ref readonly CellMap pattern,
	[PrimaryConstructorParameter] Cell extraCell,
	Mask digitsMask
) : ChromaticPatternStep(conclusions, views, options, blocks, in pattern, digitsMask)
{
	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [CellsStr, BlocksStr, DigitsStr]), new(SR.ChineseLanguage, [BlocksStr, CellsStr, DigitsStr])];

	/// <inheritdoc/>
	public override Technique Code => Technique.ChromaticPatternType1;
}
