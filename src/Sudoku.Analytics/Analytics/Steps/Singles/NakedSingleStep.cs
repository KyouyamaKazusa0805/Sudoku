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
/// <param name="lasting"><inheritdoc cref="ILastingTrait.Lasting" path="/summary"/></param>
public sealed partial class NakedSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	SingleSubtype subtype,
	[PrimaryConstructorParameter] int lasting
) : SingleStep(conclusions, views, options, cell, digit, subtype), ILastingTrait
{
	/// <inheritdoc/>
	public override int BaseDifficulty => Options.IsDirectMode ? 23 : 10;

	/// <inheritdoc/>
	public override Technique Code => Technique.NakedSingle;
}
