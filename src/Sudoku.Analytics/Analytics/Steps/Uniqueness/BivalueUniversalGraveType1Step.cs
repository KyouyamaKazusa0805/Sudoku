namespace Sudoku.Analytics.Steps.Uniqueness;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public sealed class BivalueUniversalGraveType1Step(StepConclusions conclusions, View[]? views, StepGathererOptions options) :
	BivalueUniversalGraveStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(1 << Conclusions.Span[0].Digit);

	/// <inheritdoc/>
	public override Technique Code => Technique.BivalueUniversalGraveType1;
}
