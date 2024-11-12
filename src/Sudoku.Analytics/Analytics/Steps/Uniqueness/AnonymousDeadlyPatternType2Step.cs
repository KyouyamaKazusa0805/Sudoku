namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is an <b>Anonymous Deadly Pattern Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="patternCandidates"><inheritdoc/></param>
/// <param name="targetCells">Indicates the target cells.</param>
/// <param name="extraDigit">Indicates the extra digit.</param>
/// <param name="technique"><inheritdoc/></param>
public sealed partial class AnonymousDeadlyPatternType2Step(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] ref readonly CandidateMap patternCandidates,
	[Property] ref readonly CellMap targetCells,
	[Property] Digit extraDigit,
	Technique technique
) : AnonymousDeadlyPatternStep(conclusions, views, options, patternCandidates.Digits, patternCandidates.Cells, technique)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(base.DigitsUsed | (Mask)(1 << ExtraDigit));

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [DigitsStr, CellsStr, ExtraDigitStr]),
			new(SR.ChineseLanguage, [DigitsStr, CellsStr, ExtraDigitStr])
		];

	private string ExtraDigitStr => Options.Converter.DigitConverter((Mask)(1 << ExtraDigit));
}
