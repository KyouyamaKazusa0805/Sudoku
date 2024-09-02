namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for baba grouping techniques.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="Step.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="Step.Views" path="/summary"/></param>
/// <param name="options"><inheritdoc cref="Step.Options" path="/summary"/></param>
public abstract class BabaGroupingStep(Conclusion[] conclusions, View[]? views, StepGathererOptions options) :
	Step(conclusions, views, options);
