namespace Sudoku.Analytics.Steps.SingleDigitPatterns;

/// <summary>
/// Provides with a step that is a <b>Single Digit Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit">Indicates the digit used in this pattern.</param>
public abstract partial class SingleDigitPatternStep(
	StepConclusions conclusions,
	View[]? views,
	StepGathererOptions options,
	[Property] Digit digit
) : FullPencilmarkingStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public sealed override Mask DigitsUsed => (Mask)(1 << Digit);
}
