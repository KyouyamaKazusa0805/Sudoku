namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="conjugateHouse">Indicates the cells used as generalized conjugate.</param>
/// <param name="extraDigitsMask">Indicates the mask of extra digits used.</param>
public sealed partial class BorescoperDeadlyPatternType4Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	ref readonly CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] ref readonly CellMap conjugateHouse,
	[PrimaryConstructorParameter] Mask extraDigitsMask
) : BorescoperDeadlyPatternStep(conclusions, views, options, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [DigitsStr, CellsStr, ConjHouseStr, ExtraCombStr]),
			new(SR.ChineseLanguage, [DigitsStr, CellsStr, ExtraCombStr, ConjHouseStr])
		];

	private string ExtraCombStr => Options.Converter.DigitConverter(ExtraDigitsMask);

	private string ConjHouseStr => Options.Converter.CellConverter(ConjugateHouse);
}
