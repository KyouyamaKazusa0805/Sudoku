namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is a <b>Unique Matrix Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="candidate">Indicates the true candidate.</param>
public sealed partial class UniqueMatrixType1Step(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	ref readonly CellMap cells,
	Mask digitsMask,
	[Property] Candidate candidate
) : UniqueMatrixStep(conclusions, views, options, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [DigitsStr, CellsStr, CandidateStr]), new(SR.ChineseLanguage, [CandidateStr, CellsStr, DigitsStr])];

	private string CandidateStr => Options.Converter.CandidateConverter([Candidate]);
}