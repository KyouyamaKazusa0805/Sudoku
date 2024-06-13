namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>XYZ-Wing Construction</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="pattern"><inheritdoc/></param>
/// <param name="constructedPattern"><inheritdoc/></param>
public sealed class XyzWingConstructionStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	ChainOrLoop pattern,
	XyzWing constructedPattern
) : PatternConstructionStep(conclusions, views, options, pattern, constructedPattern)
{
	/// <inheritdoc/>
	public override Technique Code => Technique.XyzWingConstruction;
}
