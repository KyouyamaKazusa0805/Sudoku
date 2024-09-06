namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class WingStep(Conclusion[] conclusions, View[]? views, StepGathererOptions options) :
	FullMarkStep(conclusions, views, options);
