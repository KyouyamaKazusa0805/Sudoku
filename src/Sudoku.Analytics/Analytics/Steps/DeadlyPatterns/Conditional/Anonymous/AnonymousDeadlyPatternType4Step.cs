namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Anonymous Deadly Pattern Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="patternCandidates"><inheritdoc/></param>
/// <param name="conjugateHouse">Indicates the target house.</param>
/// <param name="extraDigitsMask">Indicates the extra digits used.</param>
public sealed partial class AnonymousDeadlyPatternType4Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	[PrimaryConstructorParameter] ref readonly CandidateMap patternCandidates,
	[PrimaryConstructorParameter] House conjugateHouse,
	[PrimaryConstructorParameter] Mask extraDigitsMask
) : AnonymousDeadlyPatternStep(conclusions, views, options, patternCandidates.Digits, patternCandidates.Cells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override int Type => 4;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [DigitsStr, CellsStr, ConjHouseStr, ExtraCombStr]),
			new(SR.ChineseLanguage, [DigitsStr, CellsStr, ExtraCombStr, ConjHouseStr])
		];

	private string ExtraCombStr => Options.Converter.DigitConverter(ExtraDigitsMask);

	private string ConjHouseStr => Options.Converter.HouseConverter(ConjugateHouse);
}
