namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Anonymous Deadly Pattern Type 3</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="patternCandidates"><inheritdoc/></param>
/// <param name="targetCells">Indicates the target cells.</param>
/// <param name="extraDigitsMask">Indicates the extra digits used.</param>
public sealed partial class AnonymousDeadlyPatternType3Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	[PrimaryConstructorParameter] ref readonly CandidateMap patternCandidates,
	[PrimaryConstructorParameter] ref readonly CellMap targetCells,
	[PrimaryConstructorParameter] Mask extraDigitsMask
) : AnonymousDeadlyPatternStep(conclusions, views, options, patternCandidates.Digits, patternCandidates.Cells)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 2;

	/// <inheritdoc/>
	public override int Type => 3;
}
