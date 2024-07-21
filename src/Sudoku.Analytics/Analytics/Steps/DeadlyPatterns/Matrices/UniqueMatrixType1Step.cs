namespace Sudoku.Analytics.Steps;

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
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	ref readonly CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] Candidate candidate
) : UniqueMatrixStep(conclusions, views, options, in cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [DigitsStr, CellsStr, CandidateStr]), new(ChineseLanguage, [CandidateStr, CellsStr, DigitsStr])];

	private string CandidateStr => Options.Converter.CandidateConverter([Candidate]);
}
