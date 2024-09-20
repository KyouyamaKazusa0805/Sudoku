namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Rotating Deadly Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the digits used.</param>
/// <param name="cells">Indicates cells used.</param>
public sealed partial class RotatingDeadlyPatternStep(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	[PrimaryConstructorParameter] Mask digitsMask,
	[PrimaryConstructorParameter] ref readonly CellMap cells
) : ConditionalDeadlyPatternStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool OnlyUseBivalueCells => false;

	/// <inheritdoc/>
	public override int BaseDifficulty => 58;

	/// <inheritdoc/>
	public override Technique Code => Technique.RotatingDeadlyPattern;

	/// <inheritdoc/>
	public override Mask DigitsUsed => DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [CellsStr, DigitsStr]), new(SR.ChineseLanguage, [CellsStr, DigitsStr])];

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);
}
