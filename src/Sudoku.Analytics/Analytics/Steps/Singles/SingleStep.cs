namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for a technique of <b>Single</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="Step.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="Step.Views" path="/summary"/></param>
/// <param name="options"><inheritdoc cref="Step.Options" path="/summary"/></param>
/// <param name="cell">Indicates the cell used.</param>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="subtype">Indicates the subtype of the technique.</param>
public abstract partial class SingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Cell cell,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter] SingleSubtype subtype
) : Step(conclusions, views, options);
