namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Universal Grave Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="conjugatePair">Indicates the conjugate pair used.</param>
public sealed partial class BivalueUniversalGraveType4Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	[PrimaryConstructorParameter] Mask digitsMask,
	[PrimaryConstructorParameter] ref readonly CellMap cells,
	[PrimaryConstructorParameter] ref readonly Conjugate conjugatePair
) : BivalueUniversalGraveStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType4;

	/// <inheritdoc/>
	public override Mask DigitsUsed => DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [DigitsStr, CellsStr, ConjStr]), new(SR.ChineseLanguage, [CellsStr, DigitsStr, ConjStr])];

	private string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string ConjStr => Options.Converter.ConjugateConverter([ConjugatePair]);
}
