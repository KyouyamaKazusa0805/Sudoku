namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Unique Rectangle</b> technique.
/// </summary>
public sealed class HiddenUniqueRectangleStep(
	Conclusion[] conclusions,
	View[]? views,
	int digit1,
	int digit2,
	scoped in CellMap cells,
	bool isAvoidable,
	Conjugate[] conjugatePairs,
	int absoluteOffset
) : UniqueRectangleWithConjugatePairStep(
	conclusions,
	views,
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
	public override string Format => R["TechniqueFormat_UniqueRectangleWithConjugatePairStep"]!;
}
