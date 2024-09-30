namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Anonymous Deadly Pattern Type 2</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="patternCandidates"><inheritdoc/></param>
/// <param name="targetCells">Indicates the target cells.</param>
/// <param name="extraDigit">Indicates the extra digit.</param>
public sealed partial class AnonymousDeadlyPatternType2Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	[PrimaryConstructorParameter] ref readonly CandidateMap patternCandidates,
	[PrimaryConstructorParameter] ref readonly CellMap targetCells,
	[PrimaryConstructorParameter] Digit extraDigit
) : AnonymousDeadlyPatternStep(conclusions, views, options, patternCandidates.Digits, patternCandidates.Cells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 1;

	/// <inheritdoc/>
	public override int Type => 2;
}
