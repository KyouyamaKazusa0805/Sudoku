namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class ExtendedRectangleType4Step(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	in CellMap cells,
	Mask digitsMask,
	[Property] in Conjugate conjugatePair
) : ExtendedRectangleStep(conclusions, views, options, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [DigitsStr, CellsStr, ConjStr]), new(SR.ChineseLanguage, [DigitsStr, CellsStr, ConjStr])];

	private string ConjStr => Options.Converter.ConjugateConverter([ConjugatePair]);
}
