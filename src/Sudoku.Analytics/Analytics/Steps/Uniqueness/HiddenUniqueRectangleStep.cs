namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is a <b>Hidden Unique Rectangle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1"><inheritdoc/></param>
/// <param name="digit2"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="isAvoidable"><inheritdoc/></param>
/// <param name="conjugatePairs"><inheritdoc/></param>
/// <param name="absoluteOffset"><inheritdoc/></param>
public sealed class HiddenUniqueRectangleStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	Digit digit1,
	Digit digit2,
	in CellMap cells,
	bool isAvoidable,
	Conjugate[] conjugatePairs,
	int absoluteOffset
) : UniqueRectangleConjugatePairStep(
	conclusions,
	views,
	options,
	isAvoidable ? Technique.HiddenAvoidableRectangle : Technique.HiddenUniqueRectangle,
	digit1,
	digit2,
	cells,
	isAvoidable,
	conjugatePairs,
	absoluteOffset
)
{
	/// <inheritdoc/>
	protected override bool TechniqueResourceKeyInheritsFromBase => true;
}
