namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Naked Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="subtype"><inheritdoc/></param>
public sealed partial class NakedSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	SingleTechniqueSubtype subtype
) : SingleStep(conclusions, views, options, cell, digit, subtype)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => Options.IsDirectMode ? 2.3M : 1.0M;

	/// <inheritdoc/>
	public override Technique Code => Technique.NakedSingle;
}
