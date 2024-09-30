namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Anonymous Deadly Pattern Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="patternCandidates"><inheritdoc/></param>
/// <param name="targetCell">Indicates the target cell used.</param>
/// <param name="targetDigitsMask">
/// Indicates the target digits that the pattern will be formed if the target cell only holds such digits.
/// </param>
public sealed partial class AnonymousDeadlyPatternType1Step(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	[PrimaryConstructorParameter] ref readonly CandidateMap patternCandidates,
	[PrimaryConstructorParameter] Cell targetCell,
	[PrimaryConstructorParameter] Mask targetDigitsMask
) : AnonymousDeadlyPatternStep(conclusions, views, options, patternCandidates.Digits, patternCandidates.Cells)
{
	/// <inheritdoc/>
	public override int Type => 1;
}
