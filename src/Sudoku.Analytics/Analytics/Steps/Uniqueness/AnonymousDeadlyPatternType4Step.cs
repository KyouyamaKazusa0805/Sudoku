namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is an <b>Anonymous Deadly Pattern Type 4</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="patternCandidates"><inheritdoc/></param>
/// <param name="conjugateHouse">Indicates the target house.</param>
/// <param name="extraDigitsMask">Indicates the extra digits used.</param>
/// <param name="technique"><inheritdoc/></param>
public sealed partial class AnonymousDeadlyPatternType4Step(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] ref readonly CandidateMap patternCandidates,
	[Property] House conjugateHouse,
	[Property] Mask extraDigitsMask,
	Technique technique
) : AnonymousDeadlyPatternStep(conclusions, views, options, patternCandidates.Digits, patternCandidates.Cells, technique)
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

	private string ConjHouseStr => Options.Converter.HouseConverter(1 << ConjugateHouse);
}
