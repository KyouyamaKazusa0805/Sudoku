namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Single</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell">Indicates the cell used.</param>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="subtype">Indicates the subtype of the technique.</param>
public abstract partial class SingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Cell cell,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter] SingleTechniqueSubtype subtype
) : Step(conclusions, views, options);
