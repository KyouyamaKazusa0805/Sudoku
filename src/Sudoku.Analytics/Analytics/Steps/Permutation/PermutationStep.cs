namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Permutation</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class PermutationStep(Conclusion[] conclusions, View[]? views, StepGathererOptions options) :
	IndirectStep(conclusions, views, options);
