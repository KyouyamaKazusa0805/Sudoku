namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Firework Quadruple</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
public sealed partial class FireworkQuadrupleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	ref readonly CellMap cells,
	Mask digitsMask
) : FireworkStep(conclusions, views, options, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 4;

	/// <inheritdoc/>
	public override int Size => 4;

	/// <inheritdoc/>
	public override Technique Code => Technique.FireworkQuadruple;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [CellsStr, DigitsStr]), new(SR.ChineseLanguage, [CellsStr, DigitsStr])];

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);
}
