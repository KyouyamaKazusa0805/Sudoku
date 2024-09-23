namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Negative Rank</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class NegativeRankStep(Conclusion[] conclusions, View[]? views, StepGathererOptions options) :
	FullPencilmarkingStep(conclusions, views, options);
