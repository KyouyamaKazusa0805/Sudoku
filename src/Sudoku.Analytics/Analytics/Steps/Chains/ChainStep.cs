namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Chain</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public abstract class ChainStep(StepConclusions conclusions, View[]? views, StepGathererOptions options) :
	FullPencilmarkingStep(conclusions, views, options)
{
	/// <summary>
	/// Indicates whether the chain pattern consists of multiple sub-chains.
	/// </summary>
	public abstract bool IsMultiple { get; }

	/// <summary>
	/// Indicates whether the chain pattern is dynamic, which means it should be checked dynamically inside searching algorithm;
	/// also, dynamic chains always contain internal branches.
	/// </summary>
	public abstract bool IsDynamic { get; }

	/// <summary>
	/// Indicates the length of the whole chain pattern, i.e. the number of links used in a pattern.
	/// </summary>
	public abstract int Complexity { get; }
}
