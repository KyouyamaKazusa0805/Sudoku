namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Subset Principle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the digits used.</param>
/// <param name="extraDigit">Indicates the extra digit used.</param>
public sealed partial class ExtendedSubsetPrincipleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] ref readonly CellMap cells,
	[PrimaryConstructorParameter] Mask digitsMask,
	[PrimaryConstructorParameter] Digit extraDigit
) : AlmostLockedSetsStep(conclusions, views, options), ICellListTrait
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 55;

	/// <inheritdoc/>
	public override Technique Code => Technique.ExtendedSubsetPrinciple;

	/// <inheritdoc/>
	public override Mask DigitsUsed => DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [EspDigitStr, CellsStr]), new(SR.ChineseLanguage, [EspDigitStr, CellsStr])];

	/// <inheritdoc/>
	public override FactorArray Factors => [new ExtendedSubsetPrincipleSizeFactor()];

	/// <inheritdoc/>
	int ICellListTrait.CellSize => Cells.Count;

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string EspDigitStr => Options.Converter.DigitConverter((Mask)(1 << ExtraDigit));
}
