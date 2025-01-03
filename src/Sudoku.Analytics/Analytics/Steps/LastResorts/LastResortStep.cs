namespace Sudoku.Analytics.Steps.LastResorts;

/// <summary>
/// Provides with a step that is a <b>Last Resort</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class LastResortStep(StepConclusions conclusions, View[]? views, StepGathererOptions options) :
	FullPencilmarkingStep(conclusions, views, options);
