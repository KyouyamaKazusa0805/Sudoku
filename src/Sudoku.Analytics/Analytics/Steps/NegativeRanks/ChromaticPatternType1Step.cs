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
	scoped ref readonly CellMap pattern,
	[Data] Cell extraCell,
	Mask digitsMask
) : ChromaticPatternStep(conclusions, views, options, blocks, in pattern, digitsMask)
{
	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [CellsStr, BlocksStr, DigitsStr]), new(ChineseLanguage, [BlocksStr, CellsStr, DigitsStr])];

	/// <inheritdoc/>
	public override Technique Code => Technique.ChromaticPatternType1;
}
